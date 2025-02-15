using Roslyn;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public class BaseEffectLifetime
    {
        public string LifetimeType;
        public int LifetimeTypeValue;//Not used if Permanent, define how long it last for turn and battle.
        public int Lifetime;//Time left alive, set by LifetimeTypeValue on creation

        public BaseEffectLifetime(string LifetimeType)
        {
            this.LifetimeType = LifetimeType;
            LifetimeTypeValue = 0;
            Lifetime = 0;
        }
        public BaseEffectLifetime(string LifetimeType, int LifetimeTypeValue)
        {
            this.LifetimeType = LifetimeType;
            this.LifetimeTypeValue = LifetimeTypeValue;
            Lifetime = 0;
        }
    }

    /// <summary>
    /// An Effect can only affect its user.
    /// To affect multiple users it need to be indivually assigned to multiple users.
    /// </summary>
    public abstract class BaseEffect
    {
        public static readonly Dictionary<string, BaseEffect> DicDefaultEffect = new Dictionary<string, BaseEffect>();//When you just need a placeholder outside of a game.

        public List<BaseEffectLifetime> Lifetime;
        public bool IsStacking;
        public int MaximumStack;//Limit the number of stacks for Stacking
        private bool IsUsed;//Used to not activate the same effect multiple times after a reset.
        protected readonly bool IsPassive;//If true, the skill will activate even if IsUsed is true.
        public int Range;//Range of the skill, 0 for infinite.
        public string ActivationInfo;

        public List<BaseAutomaticSkill> ListFollowingSkill;

        public readonly string EffectTypeName;

        public BaseEffect(string EffectTypeName, bool IsPassive)
        {
            IsUsed = false;
            Lifetime = new List<BaseEffectLifetime>() { new BaseEffectLifetime(string.Empty) };
            this.EffectTypeName = EffectTypeName;
            this.IsPassive = IsPassive;
            ListFollowingSkill = new List<BaseAutomaticSkill>();
        }

        public static BaseEffect FromFile(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            string EffectName = BR.ReadString();

            bool IsStacking = BR.ReadBoolean();
            int MaximumStack = BR.ReadInt32();
            int Range = BR.ReadInt32();

            BaseEffect NewSkillEffect = DicEffect[EffectName].Copy();

            NewSkillEffect.Lifetime.Clear();
            int LifetimeCount = BR.ReadByte();
            for (int L = 0; L < LifetimeCount; ++L)
            {
                string LifetimeType = BR.ReadString();
                int LifetimeTypeValue = BR.ReadInt32();

                BaseEffectLifetime NewLifetime = new BaseEffectLifetime(LifetimeType, LifetimeTypeValue);
                NewLifetime.Lifetime = 0;
                NewSkillEffect.Lifetime.Add(NewLifetime);
            }

            NewSkillEffect.Load(BR);
            
            NewSkillEffect.IsStacking = IsStacking;
            NewSkillEffect.MaximumStack = MaximumStack;
            NewSkillEffect.Range = Range;

            int ListFollowingSkillCount = BR.ReadInt32();
            for (int S = ListFollowingSkillCount - 1; S >= 0; --S)
            {
                NewSkillEffect.ListFollowingSkill.Add(new BaseAutomaticSkill(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget));
            }

            return NewSkillEffect;
        }

        public static BaseEffect FromQuickSaveFile(BinaryReader BR, FormulaParser ActiveParser, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            string EffectName = BR.ReadString();

            BaseEffect NewSkillEffect = DicEffect[EffectName].Copy();

            NewSkillEffect.QuickLoad(BR, ActiveParser, DicRequirement, DicEffect, DicAutomaticSkillTarget);

            return NewSkillEffect;
        }

        protected abstract void Load(BinaryReader BR);

        protected void QuickLoad(BinaryReader BR, FormulaParser ActiveParser, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            //Regular Load
            bool IsStacking = BR.ReadBoolean();
            int MaximumStack = BR.ReadInt32();
            int Range = BR.ReadInt32();

            this.Lifetime.Clear();
            int LifetimeCount = BR.ReadByte();
            for (int L = 0; L < LifetimeCount; ++L)
            {

                string LifetimeType = BR.ReadString();
                int LifetimeTypeValue = BR.ReadInt32();
                BaseEffectLifetime NewLifetime = new BaseEffectLifetime(LifetimeType, LifetimeTypeValue);
                this.Lifetime.Add(NewLifetime);
            }

            Load(BR);

            int ListFollowingSkillCount = BR.ReadInt32();
            for (int S = ListFollowingSkillCount - 1; S >= 0; --S)
            {
                ListFollowingSkill.Add(new BaseAutomaticSkill(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget));
            }

            this.IsStacking = IsStacking;
            this.MaximumStack = MaximumStack;
            this.Range = Range;

            //Quick Load data
            int LifetimeCount2 = BR.ReadByte();
            for (int L = 0; L < LifetimeCount2; ++L)
            {
                int Lifetime = BR.ReadInt32();
                this.Lifetime[L].Lifetime = Lifetime;
            }

            DoQuickLoad(BR, ActiveParser);
        }

        protected abstract void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser);

        protected abstract void Save(BinaryWriter BW);

        public void QuickSave(BinaryWriter BW)
        {
            WriteEffect(BW);

            BW.Write((byte)Lifetime.Count);
            for (int L = 0; L < Lifetime.Count; ++L)
            {
                BW.Write(Lifetime[L].Lifetime);
            }

            DoQuickSave(BW);
        }

        protected abstract void DoQuickSave(BinaryWriter BW);

        public void WriteEffect(BinaryWriter BW)
        {
            BW.Write(EffectTypeName);

            BW.Write(IsStacking);
            BW.Write(MaximumStack);
            BW.Write(Range);

            BW.Write((byte)Lifetime.Count);
            for (int L = 0; L < Lifetime.Count; ++L)
            {
                BW.Write(this.Lifetime[L].LifetimeType);
                BW.Write(this.Lifetime[L].LifetimeTypeValue);
            }

            Save(BW);

            BW.Write(ListFollowingSkill.Count);
            for (int S = 0; S < ListFollowingSkill.Count; ++S)
            {
                ListFollowingSkill[S].Save(BW);
            }
        }

        public static Dictionary<string, BaseEffect> LoadAllEffects()
        {
            Dictionary<string, BaseEffect> DicEffect = LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll", SearchOption.AllDirectories), typeof(BaseEffect));

            var ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach(KeyValuePair<string, BaseEffect> ActiveEffect in LoadFromAssembly(ActiveAssembly, typeof(BaseEffect)))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            return DicEffect;
        }

        public static Dictionary<string, BaseEffect> LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, BaseEffect> DicEffect = new Dictionary<string, BaseEffect>();

            List<BaseEffect> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<BaseEffect>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (BaseEffect Instance in ListSkillEffect)
            {
                DicEffect.Add(Instance.EffectTypeName, Instance);
            }

            return DicEffect;
        }

        public static Dictionary<string, BaseEffect> LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, BaseEffect> DicEffect = new Dictionary<string, BaseEffect>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
            
            return DicEffect;
        }

        public abstract bool CanActivate();

        public virtual void SetupParamsBeforeCopy()
        {

        }

        public BaseEffect Copy()
        {
            BaseEffect NewCopy = DoCopy();

            NewCopy.IsStacking = IsStacking;
            NewCopy.MaximumStack = MaximumStack;

            for (int L = 0; L < 0; L++)
            {
                NewCopy.Lifetime[L].Lifetime = 0;
                NewCopy.Lifetime[L].LifetimeType = Lifetime[L].LifetimeType;
                NewCopy.Lifetime[L].LifetimeTypeValue = Lifetime[L].LifetimeTypeValue;
            }

            NewCopy.ListFollowingSkill = new List<BaseAutomaticSkill>(ListFollowingSkill.Count);

            foreach (BaseAutomaticSkill ActiveFollowingSkill in ListFollowingSkill)
            {
                NewCopy.ListFollowingSkill.Add(new BaseAutomaticSkill(ActiveFollowingSkill));
            }

            return NewCopy;
        }
        
        /// <summary>
        /// Reload memebers
        /// </summary>
        /// <param name="Copy"></param>
        public void CopyMembers(BaseEffect Copy)
        {
            Lifetime = Copy.Lifetime;
            IsStacking = Copy.IsStacking;
            MaximumStack = Copy.MaximumStack;

            for (int L = 0; L < 0; L++)
            {
                Lifetime[L].LifetimeType = Copy.Lifetime[L].LifetimeType;
                Lifetime[L].LifetimeTypeValue = Copy.Lifetime[L].LifetimeTypeValue;
            }

            if (Copy.GetType() == GetType())
            {
                DoCopyMembers(Copy);
            }

            ListFollowingSkill = new List<BaseAutomaticSkill>(Copy.ListFollowingSkill.Count);

            foreach (BaseAutomaticSkill ActiveFollowingSkill in Copy.ListFollowingSkill)
            {
                ListFollowingSkill.Add(ActiveFollowingSkill);
            }
        }

        public virtual BaseEffect CopyAndReload(string ParamsID)
        {
            BaseEffect NewCopy = Copy();
            NewCopy.CopyMembers(NewCopy);
            NewCopy.DoReload(ParamsID);

            return NewCopy;
        }

        protected virtual void DoReload(string ParamsID)
        {
        }

        protected abstract BaseEffect DoCopy();//Do a copy that will copy global params into the local params

        protected abstract void DoCopyMembers(BaseEffect Copy);

        public void ResetState()
        {
            IsUsed = false;
        }

        public void ExecuteEffect()
        {
            if (!IsPassive && IsUsed)
                return;

            IsUsed = true;

            ActivationInfo = DoExecuteEffect();

            foreach (BaseAutomaticSkill ActiveSkill in ListFollowingSkill)
            {
                ActiveSkill.AddSkillEffectsToTarget(BaseSkillRequirement.OnCreatedRequirementName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Activation info used for logs</returns>
        protected abstract string DoExecuteEffect();

        protected internal abstract void ReactivateEffect();

        public override bool Equals(object obj)
        {
            if (obj is BaseEffect)
                return EffectTypeName == ((BaseEffect)obj).EffectTypeName;
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return EffectTypeName;
        }
    }
}
