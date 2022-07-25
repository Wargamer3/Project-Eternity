using Roslyn;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public abstract class AutomaticSkillTargetType
    {
        public static readonly Dictionary<string, AutomaticSkillTargetType> DicDefaultTarget = new Dictionary<string, AutomaticSkillTargetType>();//When you just need a placeholder outside of a game.

        public readonly string TargetType;

        public AutomaticSkillTargetType(string TargetType)
        {
            this.TargetType = TargetType;
        }

        public abstract bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect);
        public abstract void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName);
        public abstract AutomaticSkillTargetType Copy();

        public virtual AutomaticSkillTargetType CopyAndReload(string ParamsID)
        {
            AutomaticSkillTargetType NewCopy = Copy();
            NewCopy.DoReload(ParamsID);

            return NewCopy;
        }

        protected virtual void DoReload(string ParamsID)
        {
        }

        public static Dictionary<string, AutomaticSkillTargetType> LoadAllTargetTypes()
        {
            Dictionary<string, AutomaticSkillTargetType> DicTargetType = LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll", SearchOption.AllDirectories), typeof(AutomaticSkillTargetType));

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveRequirement in LoadFromAssembly(ActiveAssembly, typeof(AutomaticSkillTargetType)))
                {
                    DicTargetType.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            return DicTargetType;
        }

        public static Dictionary<string, AutomaticSkillTargetType> LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, AutomaticSkillTargetType> DicTargetType = new Dictionary<string, AutomaticSkillTargetType>();

            List<AutomaticSkillTargetType> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<AutomaticSkillTargetType>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (AutomaticSkillTargetType Instance in ListSkillEffect)
            {
                DicTargetType.Add(Instance.TargetType, Instance);
            }

            return DicTargetType;
        }

        public static Dictionary<string, AutomaticSkillTargetType> LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, AutomaticSkillTargetType> DicTargetType = new Dictionary<string, AutomaticSkillTargetType>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveRequirement in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicTargetType.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            return DicTargetType;
        }
    }
}
