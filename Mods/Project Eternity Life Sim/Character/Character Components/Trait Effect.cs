using System;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class TraitEffect
    {
        public static readonly Dictionary<string, TraitEffect> DicEffect = new Dictionary<string, TraitEffect>();

        [Browsable(false)]
        public readonly string EffectTypeName;

        protected TraitEffect(string EffectTypeName)
        {
            this.EffectTypeName = EffectTypeName;
        }

        public static void Register(TraitEffect Effect)
        {
            if (Effect == null || DicEffect.ContainsKey(Effect.EffectTypeName))
            {
                return;
            }

            DicEffect.Add(Effect.EffectTypeName, Effect);
        }

        public static TraitEffect Create(string TypeName)
        {
            TraitEffect Prototype;
            if (string.IsNullOrEmpty(TypeName) || !DicEffect.TryGetValue(TypeName, out Prototype))
            {
                return null;
            }

            return Prototype.Copy();
        }

        public void Write(BinaryWriter BW)
        {
            BW.Write(EffectTypeName);
            DoWrite(BW);
        }

        public TraitEffect Copy()
        {
            MemoryStream Stream = new MemoryStream();
            BinaryWriter BW = new BinaryWriter(Stream);
            DoWrite(BW);
            BW.Flush();
            Stream.Position = 0;
            return LoadCopy(new BinaryReader(Stream));
        }

        public virtual void AlterAttack(StrikeRoll Roll)
        {
        }

        public virtual void AlterDamage(StrikeRoll Roll)
        {
        }

        protected abstract void DoWrite(BinaryWriter BW);

        public abstract TraitEffect LoadCopy(BinaryReader BR);

        public string Label
        {
            get { return EffectTypeName; }
        }

        public override string ToString()
        {
            return Label;
        }

        public static Dictionary<string, TraitEffect> LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, TraitEffect> Found = new Dictionary<string, TraitEffect>();
            if (ActiveAssembly == null)
            {
                return Found;
            }

            List<TraitEffect> ListEffect = ReflectionHelper.GetObjectsFromBaseTypes<TraitEffect>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);
            foreach (TraitEffect Instance in ListEffect)
            {
                if (!Found.ContainsKey(Instance.EffectTypeName))
                {
                    Found.Add(Instance.EffectTypeName, Instance);
                }
            }

            return Found;
        }

        public static Dictionary<string, TraitEffect> LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, TraitEffect> Found = new Dictionary<string, TraitEffect>();
            if (ArrayFilePath == null)
            {
                return Found;
            }

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = ReflectionHelper.LoadPluginAssembly(ArrayFilePath[F]);
                if (ActiveAssembly == null)
                {
                    continue;
                }

                foreach (KeyValuePair<string, TraitEffect> Active in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    if (!Found.ContainsKey(Active.Key))
                    {
                        Found.Add(Active.Key, Active.Value);
                    }
                }
            }

            return Found;
        }
    }
}
