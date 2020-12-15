using Roslyn;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public abstract class AutomaticSkillTargetType
    {
        public static Dictionary<string, AutomaticSkillTargetType> DicTargetType = new Dictionary<string, AutomaticSkillTargetType>();

        public readonly string TargetType;

        public AutomaticSkillTargetType(string TargetType)
        {
            this.TargetType = TargetType;
        }

        public abstract bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect);
        public abstract void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName);
        public abstract AutomaticSkillTargetType Copy();

        public static void LoadAllTargetTypes()
        {
            DicTargetType.Clear();

            LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll", SearchOption.AllDirectories), typeof(AutomaticSkillTargetType));

            var ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                LoadFromAssembly(ActiveAssembly, typeof(AutomaticSkillTargetType));
            }
        }

        public static void LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            List<AutomaticSkillTargetType> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<AutomaticSkillTargetType>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (AutomaticSkillTargetType Instance in ListSkillEffect)
            {
                DicTargetType.Add(Instance.TargetType, Instance);
            }
        }

        public static void LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args);
            }
        }
    }
}
