using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public class EffectHolder
    {
        private Dictionary<string, List<BaseEffect>> DicActiveEffectsBySkillName;

        public EffectHolder()
        {
            DicActiveEffectsBySkillName = new Dictionary<string, List<BaseEffect>>();
        }

        public void QuickSave(BinaryWriter BW)
        {
            BW.Write(DicActiveEffectsBySkillName.Count);
            foreach (KeyValuePair<string, List<BaseEffect>> ListActiveEffect in DicActiveEffectsBySkillName)
            {
                BW.Write(ListActiveEffect.Key);

                BW.Write(ListActiveEffect.Value.Count);
                for (int E = 0; E < ListActiveEffect.Value.Count; ++E)
                    ListActiveEffect.Value[E].QuickSave(BW);
            }
        }

        public void QuickLoad(BinaryReader BR, FormulaParser ActiveParser, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            int DicActivePilotEffectCount = BR.ReadInt32();
            DicActiveEffectsBySkillName = new Dictionary<string, List<BaseEffect>>(DicActivePilotEffectCount);
            for (int i = 0; i < DicActivePilotEffectCount; ++i)
            {
                string Key = BR.ReadString();

                int ListEffectValueCount = BR.ReadInt32();
                List<BaseEffect> ListEffect = new List<BaseEffect>(ListEffectValueCount);
                for (int E = 0; E < ListEffectValueCount; ++E)
                {
                    ListEffect.Add(BaseEffect.FromQuickSaveFile(BR, ActiveParser, DicRequirement, DicEffect, DicAutomaticSkillTarget));
                }

                DicActiveEffectsBySkillName.Add(Key, ListEffect);
            }
        }

        /// <summary>
        /// Add an effect into the holder then execute it, a copy will be made internaly to avoid references issues.
        /// </summary>
        /// <param name="ActiveEffect"></param>
        /// <param name="ActiveSkillName"></param>
        public void AddAndExecuteEffect(BaseEffect ActiveEffect, string ActiveSkillName, string NewLifetimeType = "")
        {
            ActiveEffect = ActiveEffect.Copy();
            
            if (CanAddEffect(ActiveEffect, ActiveSkillName))
            {
                if (NewLifetimeType != string.Empty)
                {
                    ActiveEffect.Lifetime[0].LifetimeType = NewLifetimeType;
                }

                foreach (BaseEffectLifetime ActiveLifetime in ActiveEffect.Lifetime)
                {
                    ActiveLifetime.Lifetime = ActiveLifetime.LifetimeTypeValue;
                }

                DicActiveEffectsBySkillName[ActiveSkillName].Add(ActiveEffect);
                ActiveEffect.ExecuteEffect();
            }
        }

        /// <summary>
        /// Add an effect into the holder then execute it.
        /// </summary>
        /// <param name="ActiveEffect"></param>
        /// <param name="ActiveSkillName"></param>
        public void AddAndExecuteEffectWithoutCopy(BaseEffect ActiveEffect, string ActiveSkillName)
        {
            if (CanAddEffect(ActiveEffect, ActiveSkillName))
            {
                foreach (BaseEffectLifetime ActiveLifetime in ActiveEffect.Lifetime)
                {
                    ActiveLifetime.Lifetime = ActiveLifetime.LifetimeTypeValue;
                }

                DicActiveEffectsBySkillName[ActiveSkillName].Add(ActiveEffect);
                ActiveEffect.ExecuteEffect();
            }
        }

        public bool CanAddEffect(BaseEffect ActiveSkillEffect, string ActiveSkillName)
        {
            //Add the SkillEffect to the DicActiveEffect to activate them later.
            if (!DicActiveEffectsBySkillName.ContainsKey(ActiveSkillName))
                DicActiveEffectsBySkillName.Add(ActiveSkillName, new List<BaseEffect>());

            int EffectCount = 0;

            for (int i = DicActiveEffectsBySkillName[ActiveSkillName].Count - 1; i >= 0; --i)
            {
                if (DicActiveEffectsBySkillName[ActiveSkillName][i].Equals(ActiveSkillEffect))
                    ++EffectCount;
            }

            if ((ActiveSkillEffect.IsStacking && EffectCount >= ActiveSkillEffect.MaximumStack && ActiveSkillEffect.MaximumStack >= 0)
                || (!ActiveSkillEffect.IsStacking && EffectCount > 0))
            {
                return false;
            }

            return true;
        }

        public void ReactivateEffects(List<string> ListIgnoreSkill = null)
        {
            foreach (KeyValuePair<string, List<BaseEffect>> ActiveListEffect in DicActiveEffectsBySkillName)
            {
                if (ListIgnoreSkill != null && ListIgnoreSkill.Contains(ActiveListEffect.Key))
                    continue;

                for (int E = ActiveListEffect.Value.Count - 1; E >= 0; --E)
                {
                    BaseEffect ActiveEffect = ActiveListEffect.Value[E];
                    
                    ActiveEffect.ReactivateEffect();
                }
            }
        }

        public void RemoveEffects()
        {
            DicActiveEffectsBySkillName.Clear();
        }

        public void UpdateAllEffectsLifetime(string LifetimeType)
        {
            foreach (KeyValuePair<string, List<BaseEffect>> ActiveListEffect in DicActiveEffectsBySkillName)
            {
                for (int E = ActiveListEffect.Value.Count - 1; E >= 0; --E)
                {
                    BaseEffect ActiveEffect = ActiveListEffect.Value[E];

                    foreach (BaseEffectLifetime ActiveLifetime in ActiveEffect.Lifetime)
                    {
                        if (ActiveLifetime.LifetimeType == LifetimeType)
                        {
                            --ActiveLifetime.Lifetime;
                            ActiveEffect.ResetState();

                            if (ActiveLifetime.Lifetime == 0)
                            {
                                ActiveListEffect.Value.RemoveAt(E);
                                break;
                            }
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
            if (DicActiveEffectsBySkillName.TryGetValue(SkillName, out ListActiveEffect))
                return ListActiveEffect;
            else
                return new List<BaseEffect>();
        }

        public Dictionary<string, List<BaseEffect>> GetEffects()
        {
            return DicActiveEffectsBySkillName;
        }
    }
}
