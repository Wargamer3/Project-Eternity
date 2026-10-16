using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;

namespace ProjectEternity.Core
{
    public static class Operators
    {
        public enum LogicOperators : byte { Equal, NotEqual, Greater, GreaterOrEqual, Lower, LowerOrEqual };

        public enum SignOperators : byte { Equal, PlusEqual, MinusEqual, DividedEqual, MultiplicatedEqual, ModuloEqual };

        public enum NumberTypes : byte { Absolute = 0, Relative = 1 };

        public static bool CompareValue(LogicOperators Operator, int Value1, int Value2)
        {
            switch (Operator)
            {
                case LogicOperators.Equal:
                    if (Value1 == Value2)
                        return true;
                    else
                        return false;

                case LogicOperators.Greater:
                    if (Value1 > Value2)
                        return true;
                    else
                        return false;

                case LogicOperators.GreaterOrEqual:
                    if (Value1 >= Value2)
                        return true;
                    else
                        return false;

                case LogicOperators.Lower:
                    if (Value1 < Value2)
                        return true;
                    else
                        return false;

                case LogicOperators.LowerOrEqual:
                    if (Value1 <= Value2)
                        return true;
                    else
                        return false;

                case LogicOperators.NotEqual:
                    if (Value1 != Value2)
                        return true;
                    else
                        return false;
            }

            return false;
        }
    }

    public static class RandomHelper
    {
        public static Random Random = new Random();

        /// <summary>
        /// Return a number equal or greater than 0 and lower but not equeal than MaxValue
        /// </summary>
        /// <param name="MaxValue"></param>
        /// <returns></returns>
        public static int Next(int MaxValue)
        {
            return Random.Next(MaxValue);
        }

        public static double NextDouble()
        {
            return Random.NextDouble();
        }

        /// <summary>
        /// Check if you pass a random check.
        /// </summary>
        /// <param name="ChanceToActivate">% of chance to activate</param>
        /// <returns>Returns true if the activation check passed</returns>
        public static bool RandomActivationCheck(int ChanceToActivate)
        {
            if (ChanceToActivate <= 0)
                return false;
            else if (ChanceToActivate >= 100)
                return true;
            else
            {
                float Precision = Random.Next(100) + 1;

                if (Precision <= ChanceToActivate)
                    return true;
            }
            return false;
        }
    }

    public abstract class Holder<T>
    {
        public abstract KeyValuePair<string, List<T>> GetNameAndContent(params object[] args);
    }

    public static class ReflectionHelper
    {
        private static readonly object ResolverGate = new object();
        private static bool ResolverReady;
        private static Dictionary<string, string> PluginAssemblyPaths;

        /// <summary>
        /// New system to load .net 6 plugins. This is required because .net 6 uses AssemblyLoadContext instead of AppDomain.
        /// </summary>
        /// <param name="AssemblyPath"></param>
        /// <returns></returns>
        public static Assembly LoadPluginAssembly(string AssemblyPath)
        {
            EnsurePluginResolver();
            string FullPath = Path.GetFullPath(AssemblyPath);
            try
            {
                // LoadFile uses a separate context, so a plugin and the game end up with two copies of the same type.
                return AssemblyLoadContext.Default.LoadFromAssemblyPath(FullPath);
            }
            catch (BadImageFormatException)
            {
                return null;
            }
            catch (Exception LoadError)
            {
                bool AlreadyLoaded = LoadError is FileLoadException || (LoadError is InvalidOperationException
                    && LoadError.Message.IndexOf("already loaded", StringComparison.OrdinalIgnoreCase) >= 0);

                if (!AlreadyLoaded)
                {
                    throw;
                }

                string AssemblyFileName = Path.GetFileNameWithoutExtension(FullPath);
                Assembly[] LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int A = 0; A < LoadedAssemblies.Length; A++)
                {
                    if (LoadedAssemblies[A].GetName().Name == AssemblyFileName)
                    {
                        return LoadedAssemblies[A];
                    }
                }

                throw;
            }
        }

        private static void EnsurePluginResolver()
        {
            if (ResolverReady)
            {
                return;
            }

            lock (ResolverGate)
            {
                if (ResolverReady)
                {
                    return;
                }

                AssemblyLoadContext.Default.Resolving += ResolvePluginDependency;
                ResolverReady = true;
            }
        }

        private static Assembly ResolvePluginDependency(AssemblyLoadContext Context, AssemblyName Name)
        {
            if (Name.Name == null)
            {
                return null;
            }

            string AssemblyPath = FindPluginAssembly(Name.Name);
            if (AssemblyPath == null)
            {
                return null;
            }

            try
            {
                return Context.LoadFromAssemblyPath(AssemblyPath);
            }
            catch (FileLoadException)
            {
                return FindLoadedAssembly(Name.Name);
            }
            catch (BadImageFormatException)
            {
                return null;
            }
        }

        private static Assembly FindLoadedAssembly(string AssemblyName)
        {
            Assembly[] LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int A = 0; A < LoadedAssemblies.Length; A++)
            {
                if (LoadedAssemblies[A].GetName().Name == AssemblyName)
                {
                    return LoadedAssemblies[A];
                }
            }

            return null;
        }

        private static string FindPluginAssembly(string AssemblyName)
        {
            Dictionary<string, string> Paths = PluginAssemblyPaths;
            if (Paths == null)
            {
                Paths = IndexPluginAssemblies();
            }

            string AssemblyPath;
            if (!Paths.TryGetValue(AssemblyName, out AssemblyPath))
            {
                return null;
            }

            return AssemblyPath;
        }

        private static Dictionary<string, string> IndexPluginAssemblies()
        {
            lock (ResolverGate)
            {
                if (PluginAssemblyPaths != null)
                {
                    return PluginAssemblyPaths;
                }

                Dictionary<string, string> Paths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string BaseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                string[] Files = Directory.GetFiles(BaseDir, "*.dll", SearchOption.AllDirectories);
                for (int F = 0; F < Files.Length; F++)
                {
                    string Name = Path.GetFileNameWithoutExtension(Files[F]);
                    string Existing;
                    if (!Paths.TryGetValue(Name, out Existing))
                    {
                        Paths[Name] = Files[F];
                        continue;
                    }

                    bool ExistingAtRoot = string.Equals(Path.GetDirectoryName(Existing), BaseDir, StringComparison.OrdinalIgnoreCase);
                    bool CandidateAtRoot = string.Equals(Path.GetDirectoryName(Files[F]), BaseDir, StringComparison.OrdinalIgnoreCase);
                    if (ExistingAtRoot && !CandidateAtRoot)
                    {
                        Paths[Name] = Files[F];
                    }
                    else if (CandidateAtRoot == ExistingAtRoot && Files[F].Length < Existing.Length)
                    {
                        Paths[Name] = Files[F];
                    }
                }

                PluginAssemblyPaths = Paths;
                return Paths;
            }
        }

        public static bool TrackPluginAssembly(HashSet<Assembly> SeenAssemblies, Assembly ActiveAssembly)
        {
            if (ActiveAssembly == null)
            {
                return false;
            }

            return SeenAssemblies.Add(ActiveAssembly);
        }

        public static void ForEachUnseenAssembly(HashSet<Assembly> SeenAssemblies, Action<Assembly> VisitAssembly)
        {
            Assembly[] LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int A = 0; A < LoadedAssemblies.Length; A++)
            {
                Assembly ActiveAssembly = LoadedAssemblies[A];
                if (ActiveAssembly == null || ActiveAssembly.IsDynamic)
                {
                    continue;
                }

                // Mod and External assemblies are already loaded because the game references them.
                // Their types take constructor arguments and are created by their own factories.
                // This pass only picks up plugins compiled into Core, such as Change Attack Speed.
                string Location;
                try
                {
                    Location = ActiveAssembly.Location;
                }
                catch (NotSupportedException)
                {
                    continue;
                }

                if (Location.IndexOf("\\Core\\", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                if (!SeenAssemblies.Add(ActiveAssembly))
                {
                    continue;
                }

                try
                {
                    VisitAssembly(ActiveAssembly);
                }
                catch (ReflectionTypeLoadException)
                {
                }
                catch (Exception)
                {
                    if (Location.IndexOf("\\Project Eternity\\bin\\", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        throw;
                    }
                }
            }
        }

        private static bool TryCreateInstance<T>(Type ActiveType, object[] Args, out T Created)
        {
            try
            {
                Created = (T)Activator.CreateInstance(ActiveType, Args);
                return true;
            }
            catch (MissingMethodException)
            {
                Created = default(T);
                return false;
            }
        }

        public static List<T> GetObjectsFromBaseTypes<T>(Type TypeOfT, Type[] ArrayTypes, params object[] Args)
        {
            List<T> ListObject = new List<T>();
            foreach (Type ActiveType in ArrayTypes)
            {
                if (ActiveType.IsAbstract || !ActiveType.IsVisible)
                {
                    continue;
                }

                Type ObjectType = ActiveType.BaseType;
                bool InstanceIsBaseObject = false;
                while (ObjectType != null && !InstanceIsBaseObject)
                {
                    if (ObjectType == TypeOfT)
                        InstanceIsBaseObject = true;

                    ObjectType = ObjectType.BaseType;
                }

                if (InstanceIsBaseObject)
                {
                    T NewObject;
                    if (TryCreateInstance(ActiveType, Args, out NewObject))
                    {
                        ListObject.Add(NewObject);
                    }
                }
            }
            return ListObject;
        }
        public static List<T> GetObjectsFromBaseInterface<T>(Type TypeOfT, Type[] ArrayTypes, params object[] Args)
        {
            List<T> ListObject = new List<T>();
            foreach (Type ActiveType in ArrayTypes)
            {
                if (ActiveType.IsAbstract)
                {
                    continue;
                }

                foreach (Type ActiveInterface in ActiveType.GetInterfaces())
                {
                    Type ObjectType = ActiveInterface;
                    bool InstanceIsBaseObject = false;
                    while (ObjectType != null && !InstanceIsBaseObject)
                    {
                        if (ObjectType == TypeOfT)
                            InstanceIsBaseObject = true;

                        ObjectType = ObjectType.BaseType;
                    }

                    if (InstanceIsBaseObject)
                    {
                        T NewObject;
                        if (TryCreateInstance(ActiveType, Args, out NewObject))
                        {
                            ListObject.Add(NewObject);
                        }
                    }
                }
            }
            return ListObject;
        }

        public static List<T> GetObjectsFromTypes<T>(Type TypeOfT, Type[] ArrayTypes, params object[] Args)
        {
            List<T> ListObject = new List<T>();
            foreach (Type ActiveType in ArrayTypes)
            {
                if (ActiveType.IsAbstract)
                {
                    continue;
                }

                Type ObjectType = ActiveType.BaseType;

                if (ObjectType == TypeOfT)
                {
                    T NewObject;
                    if (TryCreateInstance(ActiveType, Args, out NewObject))
                    {
                        ListObject.Add(NewObject);
                    }
                }
            }
            return ListObject;
        }

        public static List<T> GetNestedTypes<T>(Type HolderType, params object[] Args)
        {
            return GetObjectsFromBaseTypes<T>(typeof(T), HolderType.GetNestedTypes(), Args);
        }

        public static Dictionary<string, List<T>> GetContentByName<T>(string[] ArrayFilePath, params object[] Args)
        {
            return GetContentByName<T>(typeof(Holder<T>), ArrayFilePath, Args);
        }

        public static Dictionary<string, List<T>> GetContentByName<T>(Type TypeOfHolder, string[] ArrayFilePath, params object[] Args)
        {
            Dictionary<string, List<T>> DicGetContentByName = new Dictionary<string, List<T>>();
            HashSet<Assembly> SeenAssemblies = new HashSet<Assembly>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = LoadPluginAssembly(ArrayFilePath[F]);
                if (!TrackPluginAssembly(SeenAssemblies, ActiveAssembly))
                {
                    continue;
                }
                List<Holder<T>> ListHolder = GetObjectsFromBaseTypes<Holder<T>>(TypeOfHolder, ActiveAssembly.GetTypes());

                foreach (var NewHolder in ListHolder)
                {
                    KeyValuePair<string, List<T>> ContentByName = NewHolder.GetNameAndContent(Args);

                    if (!DicGetContentByName.ContainsKey(ContentByName.Key))
                    {
                        DicGetContentByName.Add(ContentByName.Key, ContentByName.Value);
                    }
                    else
                    {
                        DicGetContentByName[ContentByName.Key].AddRange(ContentByName.Value);
                    }
                }
            }
            return DicGetContentByName;
        }
    }
}
