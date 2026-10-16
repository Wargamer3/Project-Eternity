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
            HashSet<Assembly> SeenAssemblies = new HashSet<Assembly>();
            Dictionary<string, AutomaticSkillTargetType> DicTargetType = LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll", SearchOption.AllDirectories), SeenAssemblies, typeof(AutomaticSkillTargetType));

            ReflectionHelper.ForEachUnseenAssembly(SeenAssemblies, delegate (Assembly ActiveAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveRequirement in LoadFromAssembly(ActiveAssembly, typeof(AutomaticSkillTargetType)))
                {
                    DicTargetType.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            });

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                if (!ReflectionHelper.TrackPluginAssembly(SeenAssemblies, ActiveAssembly))
                {
                    continue;
                }

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
            return LoadFromAssemblyFiles(ArrayFilePath, new HashSet<Assembly>(), TypeOfRequirement, Args);
        }

        public static Dictionary<string, AutomaticSkillTargetType> LoadFromAssemblyFiles(string[] ArrayFilePath, HashSet<Assembly> SeenAssemblies, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, AutomaticSkillTargetType> DicTargetType = new Dictionary<string, AutomaticSkillTargetType>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = ReflectionHelper.LoadPluginAssembly(ArrayFilePath[F]);
                if (!ReflectionHelper.TrackPluginAssembly(SeenAssemblies, ActiveAssembly))
                {
                    continue;
                }
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveRequirement in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicTargetType.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            return DicTargetType;
        }
    }

    public class EffectActivationExecuteOnly : AutomaticSkillTargetType
    {
        public static string Name = "Execute Only";

        public EffectActivationExecuteOnly()
            : base(Name)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            return true;
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            ActiveSkillEffect.Copy().ExecuteEffect();
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationExecuteOnly();
        }
    }
}
