#region File Description
//-----------------------------------------------------------------------------
// ContentBuilder.cs
//
// Editors compile a PNG into an XNB through Add, Build, and CopyBuildOutput.
// mgcb writes the MonoGame XNB. Content.Load reads that file, not the PNG.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Content.Builder
{
    /// <summary>
    /// Builds content files into temporary .xnb files for the editors.
    /// </summary>
    public class ContentBuilder : IDisposable
    {
        class PendingContent
        {
            public string Filename;
            public string Name;
            public string Importer;
            public string Processor;
        }

        readonly List<PendingContent> Items = new List<PendingContent>();
        readonly string OutputDirectory;
        readonly string BuildDirectory;
        readonly string ProcessDirectory;
        readonly string BaseDirectory;

        static int DirectorySalt;
        bool IsDisposed;

        public ContentBuilder()
        {
            BaseDirectory = Path.Combine(Path.GetTempPath(), GetType().FullName);
            int ProcessId = Process.GetCurrentProcess().Id;
            ProcessDirectory = Path.Combine(BaseDirectory, ProcessId.ToString());
            DirectorySalt++;
            BuildDirectory = Path.Combine(ProcessDirectory, DirectorySalt.ToString());
            OutputDirectory = Path.Combine(BuildDirectory, "Content");
            Directory.CreateDirectory(OutputDirectory);
            PurgeStaleTempDirectories();
        }

        ~ContentBuilder()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            DeleteTempDirectory();
        }

        public void CopyBuildOutput(string FolderToCopyTo)
        {
            if (!Directory.Exists(FolderToCopyTo))
            {
                Directory.CreateDirectory(FolderToCopyTo);
            }

            string[] BuiltFiles = Directory.GetFiles(OutputDirectory);
            for (int F = 0; F < BuiltFiles.Length; F++)
            {
                string FileName = Path.GetFileName(BuiltFiles[F]);
                string Destination = Path.Combine(FolderToCopyTo, FileName);
                if (File.Exists(Destination))
                {
                    File.Delete(Destination);
                }

                File.Move(BuiltFiles[F], Destination);
            }
        }

        public void CopyBuildOutput(string OriginalSpriteFileName, string SpriteFileName, string FolderToCopyTo)
        {
            string Source = Path.Combine(OutputDirectory, OriginalSpriteFileName + ".xnb");
            if (!Directory.Exists(FolderToCopyTo))
            {
                Directory.CreateDirectory(FolderToCopyTo);
            }

            string Destination = Path.Combine(FolderToCopyTo, SpriteFileName + ".xnb");
            if (File.Exists(Destination))
            {
                File.Delete(Destination);
            }

            File.Move(Source, Destination);
        }

        public void Add(string filename, string name, string importer, string processor)
        {
            PendingContent Item = new PendingContent();
            Item.Filename = filename;
            Item.Name = name;
            Item.Importer = importer;
            Item.Processor = processor;
            Items.Add(Item);
        }

        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Builds every added file. Returns an error message, or null when the build succeeds.
        /// </summary>
        public string Build()
        {
            string MgcbDirectory = Path.Combine(Path.GetDirectoryName(typeof(ContentBuilder).Assembly.Location), "mgcb");
            string MgcbExecutable = Path.Combine(MgcbDirectory, "mgcb.exe");
            string MgcbAssembly = Path.Combine(MgcbDirectory, "mgcb.dll");
            if (!File.Exists(MgcbExecutable) && !File.Exists(MgcbAssembly))
            {
                return "mgcb was not found beside Microsoft Content Builder.";
            }

            string IntermediateDirectory = Path.Combine(BuildDirectory, "obj");
            Directory.CreateDirectory(IntermediateDirectory);
            Directory.CreateDirectory(OutputDirectory);

            List<string> Lines = new List<string>();
            Lines.Add("/platform:Windows");
            Lines.Add("/profile:HiDef");
            Lines.Add("/outputDir:" + OutputDirectory);
            Lines.Add("/intermediateDir:" + IntermediateDirectory);
            Lines.Add("/quiet");
            string ProcessorPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(ContentBuilder).Assembly.Location), "..", "Core", "Project Eternity Content Processors.dll"));
            bool NeedsAnimationProcessor = false;
            for (int I = 0; I < Items.Count; I++)
            {
                if (Items[I].Processor == "AnimationProcessor")
                {
                    NeedsAnimationProcessor = true;
                    break;
                }
            }

            if (NeedsAnimationProcessor && !File.Exists(ProcessorPath))
            {
                return "Project Eternity Content Processors.dll was not found in Core.";
            }

            if (NeedsAnimationProcessor)
            {
                Lines.Add("/reference:" + ProcessorPath);
            }

            for (int I = 0; I < Items.Count; I++)
            {
                PendingContent Item = Items[I];
                if (!string.IsNullOrEmpty(Item.Importer))
                {
                    Lines.Add("/importer:" + Item.Importer);
                }

                if (!string.IsNullOrEmpty(Item.Processor))
                {
                    Lines.Add("/processor:" + Item.Processor);
                }

                Lines.Add("/build:" + Item.Filename + ";" + Item.Name);
            }

            string ResponseFile = Path.Combine(BuildDirectory, "content.mgcb");
            File.WriteAllLines(ResponseFile, Lines.ToArray());

            ProcessStartInfo Start = new ProcessStartInfo();
            if (File.Exists(MgcbExecutable))
            {
                Start.FileName = MgcbExecutable;
                Start.Arguments = "/@:content.mgcb";
            }
            else
            {
                Start.FileName = "dotnet";
                Start.Arguments = "exec \"" + MgcbAssembly + "\" /@:content.mgcb";
            }

            Start.WorkingDirectory = BuildDirectory;
            Start.UseShellExecute = false;
            Start.CreateNoWindow = true;
            Start.RedirectStandardOutput = true;
            Start.RedirectStandardError = true;

            Process Compiler = Process.Start(Start);
            string Output = Compiler.StandardOutput.ReadToEnd();
            string Error = Compiler.StandardError.ReadToEnd();
            Compiler.WaitForExit();
            if (Compiler.ExitCode == 0)
            {
                return null;
            }

            string Message = (Output + "\n" + Error).Trim();
            if (Message.Length == 0)
            {
                Message = "mgcb failed with exit code " + Compiler.ExitCode + ".";
            }

            return Message;
        }

        void DeleteTempDirectory()
        {
            if (Directory.Exists(BuildDirectory))
            {
                Directory.Delete(BuildDirectory, true);
            }

            if (!Directory.Exists(ProcessDirectory))
            {
                return;
            }

            if (Directory.GetDirectories(ProcessDirectory).Length == 0)
            {
                Directory.Delete(ProcessDirectory);
            }

            if (Directory.Exists(BaseDirectory) && Directory.GetDirectories(BaseDirectory).Length == 0)
            {
                Directory.Delete(BaseDirectory);
            }
        }

        void PurgeStaleTempDirectories()
        {
            if (!Directory.Exists(BaseDirectory))
            {
                return;
            }

            string[] Directories = Directory.GetDirectories(BaseDirectory);
            for (int D = 0; D < Directories.Length; D++)
            {
                int ProcessId;
                if (!int.TryParse(Path.GetFileName(Directories[D]), out ProcessId))
                {
                    continue;
                }

                if (ProcessId == Process.GetCurrentProcess().Id)
                {
                    continue;
                }

                try
                {
                    Process.GetProcessById(ProcessId);
                }
                catch (ArgumentException)
                {
                    Directory.Delete(Directories[D], true);
                }
            }
        }
    }
}
