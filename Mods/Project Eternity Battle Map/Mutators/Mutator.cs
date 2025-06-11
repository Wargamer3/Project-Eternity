using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Mutator
    {
        public string Name;
        public string Description;

        public Mutator()
        {
        }

        public Mutator(string Name, string Description)
        {
            this.Name = Name;
            this.Description = Description;
        }

        /// <summary>
        /// When selected from the menu. Used to modify the map parameters immediately.
        /// </summary>
        public virtual void OnSelected()
        {
        }

        public static Dictionary<string, Mutator> LoadProps(params object[] Args)
        {
            Dictionary<string, Mutator> DicMapCondition = new Dictionary<string, Mutator>();
            string[] Files = Directory.GetFiles("Mutators/Battle Map", "*.dll", SearchOption.AllDirectories);
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<Mutator> ListMutator = ReflectionHelper.GetObjectsFromBaseTypes<Mutator>(typeof(Mutator), ActiveAssembly.GetTypes(), Args);

                foreach (Mutator Instance in ListMutator)
                {
                    DicMapCondition.Add(Instance.Name, Instance);
                }
            }

            return DicMapCondition;
        }

        public static Dictionary<string, Mutator> LoadFromAssemblyFiles(string[] ArrayFilePath, params object[] Args)
        {
            Dictionary<string, Mutator> DicUnitType = new Dictionary<string, Mutator>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, Mutator> ActiveUnit in LoadFromAssembly(ActiveAssembly, Args))
                {
                    DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
                }
            }

            return DicUnitType;
        }

        public static Dictionary<string, Mutator> LoadFromAssembly(Assembly ActiveAssembly, params object[] Args)
        {
            Dictionary<string, Mutator> DicUnitType = new Dictionary<string, Mutator>();
            List<Mutator> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<Mutator>(typeof(Mutator), ActiveAssembly.GetTypes(), Args);

            foreach (Mutator Instance in ListSkillEffect)
            {
                DicUnitType.Add(Instance.Name, Instance);
            }

            return DicUnitType;
        }
    }
}
