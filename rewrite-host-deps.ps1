param(
    [Parameter(Mandatory = $true)][string]$DepsPath,
    [Parameter(Mandatory = $true)][string]$BinDir
)

$ErrorActionPreference = 'Stop'
$DepsPath = $DepsPath.Trim().Trim([char]34)
$BinDir = $BinDir.Trim().Trim([char]34).TrimEnd('\')
if (-not (Test-Path -LiteralPath $DepsPath)) {
    exit 0
}

Add-Type -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

public static class HostDepsRewrite
{
    public static void Rewrite(string DepsPath, string BinDir)
    {
        JavaScriptSerializer Serializer = new JavaScriptSerializer();
        Serializer.MaxJsonLength = 67108864;
        Serializer.RecursionLimit = 256;
        Dictionary<string, object> Document = Serializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(DepsPath));
        Dictionary<string, int> Rank = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        Rank["Core"] = 0;
        Rank["Mods"] = 2;
        Rank["AI"] = 3;
        Rank["Magic"] = 4;
        Rank["Scripts"] = 5;
        Rank["Timelines"] = 6;
        Rank["Scenes"] = 7;
        Rank["Online"] = 8;
        Rank["Effects"] = 9;
        Rank["Units"] = 10;
        Rank["Props"] = 11;
        Rank["Mutators"] = 12;
        Rank["Editors"] = 13;
        Rank["External"] = 40;

        string BinFull = Path.GetFullPath(BinDir).TrimEnd('\\');
        Dictionary<string, string> LocationByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, int> RankByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (string TopPath in Directory.GetDirectories(BinFull))
        {
            string TopName = Path.GetFileName(TopPath);
            if (TopName.Equals("Content", StringComparison.OrdinalIgnoreCase) || TopName.Equals("User Data", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            foreach (string FilePath in Directory.GetFiles(TopPath, "*.dll", SearchOption.AllDirectories))
            {
                string RelativeDir = Path.GetDirectoryName(FilePath).Substring(BinFull.Length).TrimStart('\\').Replace('\\', '/');
                string Top = RelativeDir.Split('/')[0];
                int FileRank = 50;
                int KnownRank;
                if (Rank.TryGetValue(Top, out KnownRank))
                {
                    FileRank = KnownRank;
                }

                string Name = Path.GetFileName(FilePath);
                int ExistingRank;
                if (!RankByName.TryGetValue(Name, out ExistingRank) || FileRank < ExistingRank)
                {
                    LocationByName[Name] = RelativeDir + "/" + Name;
                    RankByName[Name] = FileRank;
                }
            }
        }

        Dictionary<string, object> Targets = (Dictionary<string, object>)Document["targets"];
        foreach (string FrameworkName in new List<string>(Targets.Keys))
        {
            Dictionary<string, object> Libraries = (Dictionary<string, object>)Targets[FrameworkName];
            foreach (string LibraryName in new List<string>(Libraries.Keys))
            {
                Dictionary<string, object> Library = Libraries[LibraryName] as Dictionary<string, object>;
                if (Library == null)
                {
                    continue;
                }

                string RuntimeDir = RewriteSection(Library, "runtime", LocationByName, false);
                string NativeDir = RewriteSection(Library, "native", LocationByName, false);
                string RidDir = RewriteSection(Library, "runtimeTargets", LocationByName, true);
                string RelativeDir = RuntimeDir;
                if (string.IsNullOrEmpty(RelativeDir))
                {
                    RelativeDir = NativeDir;
                }
                if (string.IsNullOrEmpty(RelativeDir))
                {
                    RelativeDir = RidDir;
                }
                if (!string.IsNullOrEmpty(RelativeDir))
                {
                    SetLibraryFolder(Document, LibraryName, RelativeDir);
                }
            }
        }

        AddAppProbe(DepsPath, BinDir, Serializer);
        File.WriteAllText(DepsPath, Serializer.Serialize(Document), new System.Text.UTF8Encoding(false));
    }

    private static void SetLibraryFolder(Dictionary<string, object> Document, string LibraryName, string RelativeDir)
    {
        object LibrariesObject;
        if (!Document.TryGetValue("libraries", out LibrariesObject))
        {
            return;
        }

        Dictionary<string, object> Libraries = LibrariesObject as Dictionary<string, object>;
        if (Libraries == null || !Libraries.ContainsKey(LibraryName))
        {
            return;
        }

        Dictionary<string, object> Entry = Libraries[LibraryName] as Dictionary<string, object>;
        if (Entry == null)
        {
            return;
        }

        Entry["path"] = RelativeDir.Replace('/', '\\');
        Entry.Remove("hashPath");
    }

    private static void AddAppProbe(string DepsPath, string BinDir, JavaScriptSerializer Serializer)
    {
        if (!DepsPath.EndsWith(".deps.json", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        string RuntimePath = DepsPath.Substring(0, DepsPath.Length - ".deps.json".Length) + ".runtimeconfig.json";
        if (!File.Exists(RuntimePath))
        {
            return;
        }

        Dictionary<string, object> RuntimeConfig = Serializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(RuntimePath));
        object OptionsObject;
        if (!RuntimeConfig.TryGetValue("runtimeOptions", out OptionsObject))
        {
            return;
        }

        Dictionary<string, object> Options = OptionsObject as Dictionary<string, object>;
        if (Options == null)
        {
            return;
        }

        string Probe = Path.GetFullPath(BinDir).TrimEnd('\\') + "\\";
        Options["additionalProbingPaths"] = new List<string> { Probe };
        File.WriteAllText(RuntimePath, Serializer.Serialize(RuntimeConfig), new System.Text.UTF8Encoding(false));
    }

    private static string AssetRid(object AssetValue)
    {
        Dictionary<string, object> Meta = AssetValue as Dictionary<string, object>;
        if (Meta == null || !Meta.ContainsKey("rid") || Meta["rid"] == null)
        {
            return "";
        }

        return Meta["rid"].ToString();
    }

    private static string RewriteSection(Dictionary<string, object> Library, string SectionName, Dictionary<string, string> LocationByName, bool RidSpecific)
    {
        object SectionObject;
        string RelativeDir = null;
        if (!Library.TryGetValue(SectionName, out SectionObject) || SectionObject == null)
        {
            return null;
        }

        Dictionary<string, object> Section = SectionObject as Dictionary<string, object>;
        if (Section == null)
        {
            return null;
        }

        Dictionary<string, object> Rebuilt = new Dictionary<string, object>();
        foreach (string AssetPath in new List<string>(Section.Keys))
        {
            string FileName = Path.GetFileName(AssetPath.Replace('/', '\\'));
            if (FileName.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase))
            {
                if (!Rebuilt.ContainsKey(AssetPath))
                {
                    Rebuilt.Add(AssetPath, Section[AssetPath]);
                }
                continue;
            }

            string NewPath = AssetPath;
            string Rid = AssetRid(Section[AssetPath]);
            string Located;
            if (LocationByName.TryGetValue(FileName, out Located))
            {
                int Slash = Located.LastIndexOf('/');
                if (Slash > 0 && (!RidSpecific || Rid.StartsWith("win", StringComparison.OrdinalIgnoreCase)))
                {
                    NewPath = RidSpecific ? Located : FileName;
                    if (RelativeDir == null)
                    {
                        RelativeDir = Located.Substring(0, Slash);
                    }
                }
            }

            if (!Rebuilt.ContainsKey(NewPath))
            {
                Rebuilt.Add(NewPath, Section[AssetPath]);
            }
            else if (Rid.Equals("win7-x64", StringComparison.OrdinalIgnoreCase))
            {
                Rebuilt[NewPath] = Section[AssetPath];
            }
        }

        Library[SectionName] = Rebuilt;
        return RelativeDir;
    }
}
'@ -ReferencedAssemblies System.Web.Extensions

[HostDepsRewrite]::Rewrite($DepsPath, $BinDir)
