using Roslyn;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.Core.Item
{
    /// <summary>
    /// An Effect can only affect its user.
    /// To affect multiple users it need to be indivually assigned to multiple users.
    /// </summary>
    public abstract class BaseEffect
    {
        public string LifetimeType;
        public int LifetimeTypeValue;//Not used if Permanent, limit the number of stacks for Stacking, define how long it last for turn and battle.
        public bool IsStacking;
        public int MaximumStack;
        public int Lifetime;
        private bool IsUsed;//Used to not activate the same effect multiple times after a reset.
        protected readonly bool IsPassive;//If true, the skill will activate even if IsUsed is true.
        public int Range;//Range of the skill, 0 for infinite.
        public string ActivationInfo;

        public List<BaseAutomaticSkill> ListFollowingSkill;

        public readonly string EffectTypeName;

        public BaseEffect(string EffectTypeName, bool IsPassive)
        {
            IsUsed = false;
            LifetimeType = string.Empty;
            this.EffectTypeName = EffectTypeName;
            this.IsPassive = IsPassive;
            ListFollowingSkill = new List<BaseAutomaticSkill>();
        }

        public static BaseEffect FromFile(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
        {
            string EffectName = BR.ReadString();

            string LifetimeType = BR.ReadString();
            int LifetimeTypeValue = BR.ReadInt32();

            bool IsStacking = BR.ReadBoolean();
            int MaximumStack = BR.ReadInt32();
            int Range = BR.ReadInt32();

            BaseEffect NewSkillEffect = DicEffect[EffectName].Copy();
            NewSkillEffect.LifetimeType = LifetimeType;
            NewSkillEffect.LifetimeTypeValue = LifetimeTypeValue;

            NewSkillEffect.Load(BR);
            
            NewSkillEffect.Lifetime = 0;

            NewSkillEffect.IsStacking = IsStacking;
            NewSkillEffect.MaximumStack = MaximumStack;
            NewSkillEffect.Range = Range;

            int ListFollowingSkillCount = BR.ReadInt32();
            for (int S = ListFollowingSkillCount - 1; S >= 0; --S)
            {
                NewSkillEffect.ListFollowingSkill.Add(new BaseAutomaticSkill(BR, DicRequirement, DicEffect));
            }

            return NewSkillEffect;
        }

        protected abstract void Load(BinaryReader BR);

        protected abstract void Save(BinaryWriter BW);

        public void WriteEffect(BinaryWriter BW)
        {
            BW.Write(EffectTypeName);

            BW.Write(LifetimeType);
            BW.Write(LifetimeTypeValue);

            BW.Write(IsStacking);
            BW.Write(MaximumStack);
            BW.Write(Range);

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

            NewCopy.Lifetime = 0;
            NewCopy.IsStacking = IsStacking;
            NewCopy.MaximumStack = MaximumStack;
            NewCopy.LifetimeType = LifetimeType;
            NewCopy.LifetimeTypeValue = LifetimeTypeValue;
            NewCopy.ListFollowingSkill = new List<BaseAutomaticSkill>(ListFollowingSkill.Count);

            foreach (BaseAutomaticSkill ActiveFollowingSkill in ListFollowingSkill)
            {
                NewCopy.ListFollowingSkill.Add(new BaseAutomaticSkill(ActiveFollowingSkill));
            }

            return NewCopy;
        }
        
        public void CopyMembers(BaseEffect Copy)
        {
            Lifetime = Copy.Lifetime;
            IsStacking = Copy.IsStacking;
            MaximumStack = Copy.MaximumStack;
            LifetimeType = Copy.LifetimeType;
            LifetimeTypeValue = Copy.LifetimeTypeValue;

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

        protected abstract BaseEffect DoCopy();

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
