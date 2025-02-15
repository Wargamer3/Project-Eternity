using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;
using Roslyn;

namespace ProjectEternity.Core.Skill
{
    public abstract class ManualSkillTarget
    {
        public static readonly Dictionary<string, ManualSkillTarget> DicDefaultTarget = new Dictionary<string, ManualSkillTarget>();//When you just need a placeholder outside of a game.

        public readonly string TargetType;
        public readonly bool MustBeUsedAlone;

        protected ManualSkillTarget(string TargetType, bool MustBeUsedAlone)
        {
            this.TargetType = TargetType;
            this.MustBeUsedAlone = MustBeUsedAlone;
        }

        public abstract ManualSkillTarget Copy();

        public abstract void ActivateSkillFromMenu(ManualSkill ActiveSkill);

        public abstract bool CanActivateOnTarget(ManualSkill ActiveSkill);

        public void AddAndExecuteEffect(ManualSkill ActiveSkill, EffectHolder Effects, string LifetimeTurnValue)
        {
            for (int E = ActiveSkill.ListEffect.Count - 1; E >= 0; --E)
            {
                string LifeType = "";

                //Assign lifetype
                if (ActiveSkill.ListEffect[E].Lifetime[0].LifetimeType == SkillEffect.LifetimeTypeTurns)
                {
                    LifeType = LifetimeTurnValue;
                }

                Effects.AddAndExecuteEffect(ActiveSkill.ListEffect[E], ActiveSkill.Name, LifeType);
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(TargetType);

            DoSave(BW);
        }

        protected virtual void DoSave(BinaryWriter BW)
        {
        }

        protected virtual void Load(BinaryReader BR)
        {
        }

        public static ManualSkillTarget LoadCopy(BinaryReader BR, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            string TargetType = BR.ReadString();

            ManualSkillTarget NewManualSkillTarget = DicManualSkillTarget[TargetType].Copy();

            NewManualSkillTarget.Load(BR);

            return NewManualSkillTarget;
        }

        public virtual ManualSkillTarget CopyAndReload(string ParamsID)
        {
            ManualSkillTarget NewCopy = Copy();
            NewCopy.DoReload(ParamsID);

            return NewCopy;
        }

        protected virtual void DoReload(string ParamsID)
        {
        }

        public static Dictionary<string, ManualSkillTarget> LoadAllTargetTypes()
        {
            Dictionary<string, ManualSkillTarget> DicManualSkillTarget = LoadFromAssemblyFilesFromBaseType(Directory.GetFiles("Effects", "*.dll", SearchOption.AllDirectories), typeof(ManualSkillTarget));

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveTarget in LoadFromAssemblyFilesFromBaseType(ActiveAssembly, typeof(ManualSkillTarget)))
                {
                    DicManualSkillTarget.Add(ActiveTarget.Key, ActiveTarget.Value);
                }
            }

            return DicManualSkillTarget;
        }

        public static Dictionary<string, ManualSkillTarget> LoadFromAssembly(Assembly ActiveAssembly, params object[] Args)
        {
            Dictionary<string, ManualSkillTarget> DicManualSkillTarget = new Dictionary<string, ManualSkillTarget>();

            List<ManualSkillTarget> ListActivation = ReflectionHelper.GetObjectsFromBaseTypes<ManualSkillTarget>(typeof(ManualSkillTarget), ActiveAssembly.GetTypes(), Args);

            foreach (ManualSkillTarget Instance in ListActivation)
            {
                DicManualSkillTarget.Add(Instance.TargetType, Instance);
            }

            return DicManualSkillTarget;
        }

        public static Dictionary<string, ManualSkillTarget> LoadFromAssemblyFiles(string[] ArrayFilePath, params object[] Args)
        {
            Dictionary<string, ManualSkillTarget> DicManualSkillTarget = new Dictionary<string, ManualSkillTarget>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveTarget in LoadFromAssembly(ActiveAssembly, Args))
                {
                    DicManualSkillTarget.Add(ActiveTarget.Key, ActiveTarget.Value);
                }
            }

            return DicManualSkillTarget;
        }

        public static Dictionary<string, ManualSkillTarget> LoadFromAssemblyFilesFromBaseType(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, ManualSkillTarget> DicManualSkillTarget = new Dictionary<string, ManualSkillTarget>();
            List<ManualSkillTarget> ListTarget = ReflectionHelper.GetObjectsFromBaseTypes<ManualSkillTarget>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (ManualSkillTarget Instance in ListTarget)
            {
                DicManualSkillTarget.Add(Instance.TargetType, Instance);
            }

            return DicManualSkillTarget;
        }

        public static Dictionary<string, ManualSkillTarget> LoadFromAssemblyFilesFromBaseType(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, ManualSkillTarget> DicManualSkillTarget = new Dictionary<string, ManualSkillTarget>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveTarget in LoadFromAssemblyFilesFromBaseType(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicManualSkillTarget.Add(ActiveTarget.Key, ActiveTarget.Value);
                }
            }

            return DicManualSkillTarget;
        }

        public override string ToString()
        {
            return TargetType;
        }
    }
}
