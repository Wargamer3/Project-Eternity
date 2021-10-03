using System;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Roslyn
{
    public class RoslynWrapper
    {
        public static T EvaluateAsync<T>(string Code)
        {
            return CSharpScript.EvaluateAsync<T>(Code).Result;
        }

        public static T EvaluateAsync<T>(string Code, ScriptOptions options, object globals = null, Type globalsType = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CSharpScript.EvaluateAsync<T>(Code, options, globals, globalsType, cancellationToken).Result;
        }

        public static object EvaluateAsync(string Code, ScriptOptions options = null, object globals = null, Type globalsType = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CSharpScript.EvaluateAsync(Code, options, globals, globalsType, cancellationToken).Result;
        }

        public static List<MetadataReference> LoadAppReferences()
        {
            List<MetadataReference> ListReferences = new List<MetadataReference>()
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(TypeConverter).Assembly.Location),
            };
            string[] Files = Directory.GetFiles(".", "*.dll", SearchOption.AllDirectories);
            for (int F = 0; F < Files.Length; F++)
            {
                if (Files[F].StartsWith(".\\Content")
                    || Files[F].StartsWith(".\\Editors")
                    || Files[F] == ".\\External\\fmodex.dll"
                    || Files[F] == ".\\External\\fmodex64.dll"
                    || Files[F] == ".\\External\\fmodexL.dll"
                    || Files[F] == ".\\External\\fmodexL64.dll")
                {
                    continue;
                }
                ListReferences.Add(MetadataReference.CreateFromFile(Path.GetFullPath(Files[F])));
            }

            return ListReferences;
        }

        public static List<Assembly> GetCompiledAssembliesFromFolder(string FolderPath, string SearchPattern, SearchOption SearchOption)
        {
            List<Assembly> ListAssembly = new List<Assembly>();
            string RandomAssemblyName = Path.GetRandomFileName();
            List<MetadataReference> ListReferences = LoadAppReferences();

            string[] Files = Directory.GetFiles(FolderPath, SearchPattern, SearchOption);
            for (int F = 0; F < Files.Length; F++)
            {
                SyntaxTree ActiveSyntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.GetFullPath(Files[F])));

                CSharpCompilation Compilation = CSharpCompilation.Create(
                    RandomAssemblyName,
                    syntaxTrees: new[] { ActiveSyntaxTree },
                    references: ListReferences,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                using (var MS = new MemoryStream())
                {
                    EmitResult CompilationResult = Compilation.Emit(MS);

                    if (!CompilationResult.Success)
                    {
                        IEnumerable<Diagnostic> ListCompilationFailures = CompilationResult.Diagnostics.Where(D =>
                            D.IsWarningAsError ||
                            D.Severity == DiagnosticSeverity.Error);

                        foreach (Diagnostic ActiveDiagnostic in ListCompilationFailures)
                        {
                            Console.Error.WriteLine("{0}: {1}", ActiveDiagnostic.Id, ActiveDiagnostic.GetMessage());
                        }
                    }
                    else
                    {
                        MS.Seek(0, SeekOrigin.Begin);
                        Assembly LoadedAssembly = Assembly.Load(MS.ToArray());
                        ListAssembly.Add(LoadedAssembly);
                    }
                }
            }

            return ListAssembly;
        }
    }
}
