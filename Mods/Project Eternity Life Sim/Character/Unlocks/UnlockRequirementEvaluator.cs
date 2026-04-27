using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class UnlockRequirementEvaluator
    {
        public readonly string RequirementTypeName;

        protected LifeSimParams Params;

        public abstract bool CanBeUnlocked();

        protected UnlockRequirementEvaluator(string RequirementName)
        {
            this.RequirementTypeName = RequirementName;
        }

        public void Init(LifeSimParams Params)
        {
            this.Params = Params;
        }

        public void Write(BinaryWriter BW)
        {
            BW.Write(RequirementTypeName);
            DoWrite(BW);
        }
        
        protected abstract void DoWrite(BinaryWriter BW);

        public abstract UnlockRequirementEvaluator Copy();

        public abstract UnlockRequirementEvaluator LoadCopy(BinaryReader BR);

        public override string ToString()
        {
            return RequirementTypeName;
        }

        public static Dictionary<string, UnlockRequirementEvaluator> LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, UnlockRequirementEvaluator> DicEffect = new Dictionary<string, UnlockRequirementEvaluator>();

            List<UnlockRequirementEvaluator> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<UnlockRequirementEvaluator>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (UnlockRequirementEvaluator Instance in ListSkillEffect)
            {
                DicEffect.Add(Instance.RequirementTypeName, Instance);
            }

            return DicEffect;
        }

        public static Dictionary<string, UnlockRequirementEvaluator> LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, UnlockRequirementEvaluator> DicEffect = new Dictionary<string, UnlockRequirementEvaluator>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, UnlockRequirementEvaluator> ActiveEffect in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            return DicEffect;
        }
    }
}
