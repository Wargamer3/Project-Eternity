using Roslyn;
using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public abstract class AutomaticSkillTargetType
    {
        public static Dictionary<string, AutomaticSkillTargetType> DicTargetType = new Dictionary<string, AutomaticSkillTargetType>();

        public readonly string TargetType;

        public AutomaticSkillTargetType(string TargetType)
        {
            this.TargetType = TargetType;
        }

        public abstract bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect);
        public abstract void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName);
        public abstract AutomaticSkillTargetType Copy();

        public static void LoadAllTargetTypes()
        {
            DicTargetType.Clear();

            LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll", SearchOption.AllDirectories), typeof(AutomaticSkillTargetType));

            var ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                LoadFromAssembly(ActiveAssembly, typeof(AutomaticSkillTargetType));
            }
        }

        public static void LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            List<AutomaticSkillTargetType> ListSkillEffect = ReflectionHelper.GetObjectsFromTypes<AutomaticSkillTargetType>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (AutomaticSkillTargetType Instance in ListSkillEffect)
            {
                DicTargetType.Add(Instance.TargetType, Instance);
            }
        }

        public static void LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args);
            }
        }
    }

    public abstract class BaseSkillRequirement
    {
        public static Dictionary<string, BaseSkillRequirement> DicRequirement = new Dictionary<string, BaseSkillRequirement>();

        public readonly string SkillRequirementName;

        public BaseSkillRequirement(string SkillRequirementName)
        {
            this.SkillRequirementName = SkillRequirementName;
        }

        protected abstract void Load(BinaryReader BR);

        public void Save(BinaryWriter BW)
        {
            BW.Write(SkillRequirementName);
            DoSave(BW);
        }

        public static BaseSkillRequirement LoadCopy(BinaryReader BR)
        {
            string RequirementType = BR.ReadString();
            BaseSkillRequirement NewSkillRequirement = DicRequirement[RequirementType].Copy();
            NewSkillRequirement.Load(BR);

            return NewSkillRequirement;
        }

        protected abstract void DoSave(BinaryWriter BW);

        public abstract bool CanActivatePassive();

        public abstract BaseSkillRequirement Copy();

        public override string ToString()
        {
            return SkillRequirementName;
        }

        public static void LoadAllRequirements()
        {
            DicRequirement.Clear();

            LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll", SearchOption.AllDirectories), typeof(BaseSkillRequirement));

            var ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                LoadFromAssembly(ActiveAssembly, typeof(BaseSkillRequirement));
            }
        }

        public static void LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            List<BaseSkillRequirement> ListSkillEffect = ReflectionHelper.GetObjectsFromTypes<BaseSkillRequirement>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (BaseSkillRequirement Instance in ListSkillEffect)
            {
                DicRequirement.Add(Instance.SkillRequirementName, Instance);
            }
        }

        public static void LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args);
            }
        }
    }
    
    public class BaseSkillActivation
    {
        public List<BaseSkillRequirement> ListRequirement;//List of every requirement criterias needed to active the Skill.
        public List<BaseEffect> ListEffect;//List of Effect to execute once activated.
        public List<List<string>> ListEffectTarget;//List activation to use for each effect, one activation per effect.
        public List<List<AutomaticSkillTargetType>> ListEffectTargetReal;//List activation to use for each effect, one activation per effect.
        private byte _ActivationPercentage;

        public BaseSkillActivation()
        {
            ListRequirement = new List<BaseSkillRequirement>();
            ListEffect = new List<BaseEffect>();
            ListEffectTarget = new List<List<string>>();
            ListEffectTargetReal = new List<List<AutomaticSkillTargetType>>();
            _ActivationPercentage = 100;
        }

        public BaseSkillActivation(BinaryReader BR)
        {
            ActivationPercentage = BR.ReadByte();

            //Requirements
            int ListRequirementCount = BR.ReadInt32();
            ListRequirement = new List<BaseSkillRequirement>(ListRequirementCount);
            for (int R = 0; R < ListRequirementCount; R++)
            {
                ListRequirement.Add(BaseSkillRequirement.LoadCopy(BR));
            }

            //Effects.
            int ListEffectCount = BR.ReadInt32();
            ListEffect = new List<BaseEffect>(ListEffectCount);
            ListEffectTarget = new List<List<string>>(ListEffectCount);
            for (int E = 0; E < ListEffectCount; E++)
            {
                int ListActivationTypesCount = BR.ReadInt32();

                List<string> NewListActivationType = new List<string>(ListActivationTypesCount);
                for (int A = 0; A < ListActivationTypesCount; A++)
                {
                    NewListActivationType.Add(BR.ReadString());
                }

                ListEffectTarget.Add(NewListActivationType);

                ListEffect.Add(BaseEffect.FromFile(BR));
            }
        }
        public bool Activate(string SkillRequirementToActivate, string SkillName)
        {
            bool CanActivate = true;
            //Check if you can attack with ActivationPercentage.
            if (!RandomHelper.RandomActivationCheck(ActivationPercentage))
                return false;

            for (int R = 0; R < ListRequirement.Count && CanActivate; R++)
            {
                bool IsManuallyActivated = ListRequirement[R].SkillRequirementName == SkillRequirementToActivate;
                bool CanPassiveRequirement = ListRequirement[R].CanActivatePassive();

                if (!CanPassiveRequirement && !IsManuallyActivated)
                {
                    CanActivate = false;
                }
            }

            if (CanActivate)
            {
                for (int E = 0; E < ListEffect.Count; E++)
                {
                    foreach (AutomaticSkillTargetType ActiveActivation in ListEffectTargetReal[E])
                    {
                        if (ActiveActivation.CanExecuteEffectOnTarget(ListEffect[E]))
                        {
                            ActiveActivation.ExecuteAndAddEffectToTarget(ListEffect[E], SkillName);
                        }
                    }
                }
            }

            return CanActivate;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ActivationPercentage);

            //Activations.
            BW.Write(ListRequirement.Count);
            for (int A = 0; A < ListRequirement.Count; A++)
            {
                ListRequirement[A].Save(BW);
            }

            //Effects.
            BW.Write(ListEffect.Count);
            for (int E = 0; E < ListEffect.Count; E++)
            {
                BW.Write(ListEffectTarget[E].Count);

                for (int A = 0; A < ListEffectTarget[E].Count; A++)
                {
                    BW.Write(ListEffectTarget[E][A]);
                }

                ListEffect[E].WriteEffect(BW);
            }
        }

        [CategoryAttribute("Activation Attributes"),
        DescriptionAttribute(".")]
        public byte ActivationPercentage
        {
            get { return _ActivationPercentage; }
            set { _ActivationPercentage = value; }
        }
    }

    public class BaseSkillLevel
    {
        public List<BaseSkillActivation> ListActivation;//List of requirements, each of them activate the Skill when completed.

        private int _ActivationsCount;//Number of times the Skill can be used, -1 for infinite.
        public int Price;

        public BaseSkillLevel()
        {
            ListActivation = new List<BaseSkillActivation>();

            _ActivationsCount = -1;
            Price = 0;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ActivationsCount);
            BW.Write(Price);

            BW.Write(ListActivation.Count);
            for (int R = 0; R < ListActivation.Count; R++)
            {
                ListActivation[R].Save(BW);
            }
        }

        [CategoryAttribute("Level Attributes"),
        DescriptionAttribute(".")]
        public int ActivationsCount
        {
            get { return _ActivationsCount; }
            set { _ActivationsCount = value; }
        }
    }

    public class BaseAutomaticSkill
    {

        public string Name;
        public string Description;
        public int CurrentLevel;
        public List<BaseSkillLevel> ListSkillLevel;

        public BaseSkillLevel CurrentSkillLevel { get { return ListSkillLevel[CurrentLevel - 1]; } }

        public BaseAutomaticSkill()
        {
            ListSkillLevel = new List<BaseSkillLevel>();
        }

        public BaseAutomaticSkill(string SkillPath)
            : this()
        {
            Name = Path.GetFileNameWithoutExtension(SkillPath);
            CurrentLevel = 1;

            FileStream FS = new FileStream(SkillPath, FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Description = BR.ReadString();

            int ListSkillLevelCount = BR.ReadInt32();
            for (int L = 0; L < ListSkillLevelCount; L++)
            {
            }

            FS.Close();
            BR.Close();
        }
        
        public void AddSkillEffectsToTarget(string SkillRequirementToActivate)
        {
            bool CanActivate;

            //No activations remaining.
            if (CurrentSkillLevel.ActivationsCount == 0)
                return;

            for (int A = 0; A < CurrentSkillLevel.ListActivation.Count; A++)
            {
                CanActivate = CurrentSkillLevel.ListActivation[A].Activate(SkillRequirementToActivate, Name);

                if (CanActivate)
                {
                    if (CurrentSkillLevel.ActivationsCount > 0)
                        CurrentSkillLevel.ActivationsCount--;
                }
            }
        }
    }

    /// <summary>
    /// An Effect can only affect its user.
    /// To affect multiple users it need to be indivually assigned to multiple users.
    /// </summary>
    public abstract class BaseEffect
    {
        public static Dictionary<string, BaseEffect> DicEffects = new Dictionary<string, BaseEffect>();

        public string LifetimeType;
        public int LifetimeTypeValue;//Not used if Permanent, limit the number of stacks for Stacking, define how long it last for turn and battle.
        public bool IsStacking;
        public int MaximumStack;
        public int Lifetime;
        public bool IsUsed;
        public int Range;//Range of the skill, 0 for infinite.

        public readonly string EffectTypeName;

        public BaseEffect(string EffectTypeName)
        {
            IsUsed = false;
            this.EffectTypeName = EffectTypeName;
        }

        public static BaseEffect FromFile(BinaryReader BR)
        {
            string EffectName = BR.ReadString();

            string LifetimeType = BR.ReadString();
            int LifetimeTypeValue = BR.ReadInt32();

            bool IsStacking = BR.ReadBoolean();
            int MaximumStack = BR.ReadInt32();
            int Range = BR.ReadInt32();

            BaseEffect NewSkillEffect = DicEffects[EffectName].Copy();
            NewSkillEffect.LifetimeType = LifetimeType;
            NewSkillEffect.LifetimeTypeValue = LifetimeTypeValue;

            NewSkillEffect.Load(BR);
            
            NewSkillEffect.Lifetime = 0;

            NewSkillEffect.IsStacking = IsStacking;
            NewSkillEffect.MaximumStack = MaximumStack;
            NewSkillEffect.Range = Range;

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
        }

        public static void LoadAllEffects()
        {
            DicEffects.Clear();

            LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll", SearchOption.AllDirectories), typeof(BaseEffect));

            var ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                LoadFromAssembly(ActiveAssembly, typeof(BaseEffect));
            }
        }

        public static void LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            List<BaseEffect> ListSkillEffect = ReflectionHelper.GetObjectsFromTypes<BaseEffect>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (BaseEffect Instance in ListSkillEffect)
            {
                DicEffects.Add(Instance.EffectTypeName, Instance);
            }
        }

        public static void LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args);
            }
        }

        public abstract bool CanActivate();

        public BaseEffect Copy()
        {
            BaseEffect NewCopy = DoCopy();

            NewCopy.Lifetime = 0;
            NewCopy.IsStacking = IsStacking;
            NewCopy.MaximumStack = MaximumStack;
            NewCopy.LifetimeType = LifetimeType;
            NewCopy.LifetimeTypeValue = LifetimeTypeValue;

            return NewCopy;
        }

        protected abstract BaseEffect DoCopy();

        public void ExecuteEffect()
        {
            if (IsUsed)
                return;

            DoExecuteEffect();
        }

        protected abstract void DoExecuteEffect();

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

    public class EffectHolder
    {
        private Dictionary<string, List<BaseEffect>> DicActiveEffect;

        public EffectHolder()
        {
            DicActiveEffect = new Dictionary<string, List<BaseEffect>>();
        }

        public void QuickSave(BinaryWriter BW)
        {
            foreach (KeyValuePair<string, List<BaseEffect>> ListEffect in DicActiveEffect)
            {
                BW.Write(ListEffect.Key);

                BW.Write(ListEffect.Value.Count);
                for (int E = 0; E < ListEffect.Value.Count; ++E)
                    ListEffect.Value[E].WriteEffect(BW);
            }
        }

        public void QuickLoad(BinaryReader BR)
        {
            int DicActivePilotEffectCount = BR.ReadInt32();
            DicActiveEffect = new Dictionary<string, List<BaseEffect>>(DicActivePilotEffectCount);
            for (int i = 0; i < DicActivePilotEffectCount; ++i)
            {
                string Key = BR.ReadString();

                int ListEffectValueCount = BR.ReadInt32();
                List<BaseEffect> ListEffect = new List<BaseEffect>(ListEffectValueCount);
                for (int E = 0; E < ListEffectValueCount; ++E)
                    ListEffect.Add(BaseEffect.FromFile(BR));

                DicActiveEffect.Add(Key, ListEffect);
            }
        }

        /// <summary>
        /// Add an effect into the holder then execute it, a copy will be made internaly to avoid references issues.
        /// </summary>
        /// <param name="ActiveSkillEffect"></param>
        /// <param name="ActiveSkillName"></param>
        public void AddAndExecuteEffect(BaseEffect ActiveSkillEffect, string ActiveSkillName)
        {
            ActiveSkillEffect = ActiveSkillEffect.Copy();

            //Add the SkillEffect to the DicActiveEffect to activate them later.
            if (!DicActiveEffect.ContainsKey(ActiveSkillName))
                DicActiveEffect.Add(ActiveSkillName, new List<BaseEffect>());

            int EffectCount = 0;

            for (int i = DicActiveEffect[ActiveSkillName].Count - 1; i >= 0; --i)
            {
                if (DicActiveEffect[ActiveSkillName][i].Equals(ActiveSkillEffect))
                    ++EffectCount;
            }

            if ((ActiveSkillEffect.IsStacking && EffectCount < ActiveSkillEffect.MaximumStack)
                || EffectCount <= 0)
            {
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
                || EffectCount > 0)
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
                        ActiveEffect.IsUsed = false;

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
