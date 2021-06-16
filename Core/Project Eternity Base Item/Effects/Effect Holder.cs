using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public class EffectHolder
    {
        private Dictionary<string, List<BaseEffect>> DicActiveEffect;

        public EffectHolder()
        {
            DicActiveEffect = new Dictionary<string, List<BaseEffect>>();
        }

        public void QuickSave(BinaryWriter BW)
        {
            BW.Write(DicActiveEffect.Count);
            foreach (KeyValuePair<string, List<BaseEffect>> ListActiveEffect in DicActiveEffect)
            {
                BW.Write(ListActiveEffect.Key);

                BW.Write(ListActiveEffect.Value.Count);
                for (int E = 0; E < ListActiveEffect.Value.Count; ++E)
                    ListActiveEffect.Value[E].WriteEffect(BW);
            }
        }

        public void QuickLoad(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
        {
            int DicActivePilotEffectCount = BR.ReadInt32();
            DicActiveEffect = new Dictionary<string, List<BaseEffect>>(DicActivePilotEffectCount);
            for (int i = 0; i < DicActivePilotEffectCount; ++i)
            {
                string Key = BR.ReadString();

                int ListEffectValueCount = BR.ReadInt32();
                List<BaseEffect> ListEffect = new List<BaseEffect>(ListEffectValueCount);
                for (int E = 0; E < ListEffectValueCount; ++E)
                {
                    ListEffect.Add(BaseEffect.FromFile(BR, DicRequirement, DicEffect));
                }

                DicActiveEffect.Add(Key, ListEffect);
            }
        }

        /// <summary>
        /// Add an effect into the holder then execute it, a copy will be made internaly to avoid references issues.
        /// </summary>
        /// <param name="ActiveSkillEffect"></param>
        /// <param name="ActiveSkillName"></param>
        public void AddAndExecuteEffect(BaseEffect ActiveSkillEffect, string ActiveSkillName, string NewLifetimeType = "")
        {
            ActiveSkillEffect = ActiveSkillEffect.Copy();
            
            if (CanAddEffect(ActiveSkillEffect, ActiveSkillName))
            {
                if (NewLifetimeType != string.Empty)
                {
                    ActiveSkillEffect.LifetimeType = NewLifetimeType;
                }

                ActiveSkillEffect.Lifetime = ActiveSkillEffect.LifetimeTypeValue;
                DicActiveEffect[ActiveSkillName].Add(ActiveSkillEffect);
                ActiveSkillEffect.ExecuteEffect();
            }
        }

        /// <summary>
        /// Add an effect into the holder then execute it.
        /// </summary>
        /// <param name="ActiveSkillEffect"></param>
        /// <param name="ActiveSkillName"></param>
        public void AddAndExecuteEffectWithoutCopy(BaseEffect ActiveSkillEffect, string ActiveSkillName)
        {
            if (CanAddEffect(ActiveSkillEffect, ActiveSkillName))
            {
                ActiveSkillEffect.Lifetime = ActiveSkillEffect.LifetimeTypeValue;
                DicActiveEffect[ActiveSkillName].Add(ActiveSkillEffect);
                ActiveSkillEffect.ExecuteEffect();
            }
        }

        public bool CanAddEffect(BaseEffect ActiveSkillEffect, string ActiveSkillName)
        {
            //Add the SkillEffect to the DicActiveEffect to activate them later.
            if (!DicActiveEffect.ContainsKey(ActiveSkillName))
                DicActiveEffect.Add(ActiveSkillName, new List<BaseEffect>());

            int EffectCount = 0;

            for (int i = DicActiveEffect[ActiveSkillName].Count - 1; i >= 0; --i)
            {
                if (DicActiveEffect[ActiveSkillName][i].Equals(ActiveSkillEffect))
                    ++EffectCount;
            }

            if ((ActiveSkillEffect.IsStacking && EffectCount >= ActiveSkillEffect.MaximumStack && ActiveSkillEffect.MaximumStack >= 0)
                || (!ActiveSkillEffect.IsStacking && EffectCount > 0))
            {
                return false;
            }

            return true;
        }

        public void ExecuteAllEffects(List<string> ListIgnoreSkill = null)
        {
            foreach (KeyValuePair<string, List<BaseEffect>> ActiveListEffect in DicActiveEffect)
            {
                if (ListIgnoreSkill != null && ListIgnoreSkill.Contains(ActiveListEffect.Key))
                    continue;

                for (int E = ActiveListEffect.Value.Count - 1; E >= 0; --E)
                {
                    BaseEffect ActiveEffect = ActiveListEffect.Value[E];
                    
                    ActiveEffect.ExecuteEffect();
                }
            }
        }

        public void UpdateAllEffectsLifetime(string LifetimeType)
        {
            foreach (KeyValuePair<string, List<BaseEffect>> ActiveListEffect in DicActiveEffect)
            {
                for (int E = ActiveListEffect.Value.Count - 1; E >= 0; --E)
                {
                    BaseEffect ActiveEffect = ActiveListEffect.Value[E];

                    if (ActiveEffect.LifetimeType == LifetimeType)
                    {
                        --ActiveEffect.Lifetime;
                        ActiveEffect.ResetState();

                        if (ActiveEffect.Lifetime == 0)
                        {
                            ActiveListEffect.Value.RemoveAt(E);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unsafe method used for unit tests.
        /// </summary>
        /// <param name="SkillName"></param>
        /// <returns></returns>
        public List<BaseEffect> GetActiveEffects(string SkillName)
        {
            List<BaseEffect> ListActiveEffect;
            if (DicActiveEffect.TryGetValue(SkillName, out ListActiveEffect))
                return ListActiveEffect;
            else
                return new List<BaseEffect>();
        }

        public Dictionary<string, List<BaseEffect>> GetEffects()
        {
            return DicActiveEffect;
        }
    }
}
