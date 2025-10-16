using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProjectEternity.Core.Editor
{
    public partial class BaseEditor : Form
    {
        public string FilePath;

        public BaseEditor()
        {
            InitializeComponent();
        }

        public virtual void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false) { throw new NotImplementedException(); }

        public virtual void DeleteItem()
        {
            File.Delete(FilePath);
        }

        public virtual EditorInfo[] LoadEditors() { throw new NotImplementedException(); }

        public static Dictionary<string, T> GetAllExtensions<T>(params object[] Args)
        {
            Dictionary<string, T> DicExtension = LoadFromAssemblyFiles<T>(Directory.GetFiles("Editors Extensions", "*.dll", SearchOption.AllDirectories), typeof(T), Args);

            return DicExtension;
        }

        public static Dictionary<string, T> LoadFromAssembly<T>(Assembly ActiveAssembly, Type TypeOfExtension, params object[] Args)
        {
            Dictionary<string, T> DicExtension = new Dictionary<string, T>();
            List<T> ListExtension = ReflectionHelper.GetObjectsFromBaseInterface<T>(TypeOfExtension, ActiveAssembly.GetTypes(), Args);

            foreach (T Instance in ListExtension)
            {
                DicExtension.Add(Instance.ToString(), Instance);
            }

            return DicExtension;
        }

        public static Dictionary<string, T> LoadFromAssemblyFiles<T>(string[] ArrayFilePath, Type TypeOfExtension, params object[] Args)
        {
            Dictionary<string, T> DicExtension = new Dictionary<string, T>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, T> ActiveExtension in LoadFromAssembly<T>(ActiveAssembly, TypeOfExtension, Args))
                {
                    DicExtension.Add(ActiveExtension.Key, ActiveExtension.Value);
                }
            }

            return DicExtension;
        }
    }
}
