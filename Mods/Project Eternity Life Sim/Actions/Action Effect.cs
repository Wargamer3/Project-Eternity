using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class ActionEffect
    {
        public readonly string EffectTypeName;

        protected ActionEffect(string EffectTypeName)
        {
            this.EffectTypeName = EffectTypeName;
        }

        public void Write(BinaryWriter BW)
        {
            BW.Write(EffectTypeName);

            DoWrite(BW);
        }

        public abstract void DoWrite(BinaryWriter BW);
        public abstract ActionEffect LoadCopy(BinaryReader BR);

        public abstract void OnEquip();

        public override string ToString()
        {
            return EffectTypeName;
        }

        public static Dictionary<string, ActionEffect> LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, ActionEffect> DicEffect = new Dictionary<string, ActionEffect>();

            List<ActionEffect> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<ActionEffect>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (ActionEffect Instance in ListSkillEffect)
            {
                DicEffect.Add(Instance.EffectTypeName, Instance);
            }

            return DicEffect;
        }

        public static Dictionary<string, ActionEffect> LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, ActionEffect> DicEffect = new Dictionary<string, ActionEffect>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, ActionEffect> ActiveEffect in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            return DicEffect;
        }
    }
}
