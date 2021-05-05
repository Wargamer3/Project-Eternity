using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public class BaseSkillActivation
    {
        public List<BaseSkillRequirement> ListRequirement;//List of every requirement criterias needed to active the Skill.
        public List<BaseEffect> ListEffect;//List of Effect to execute once activated.
        public List<List<string>> ListEffectTarget;//List activation to use for each effect, one activation per effect.
        public List<List<AutomaticSkillTargetType>> ListEffectTargetReal;//List activation to use for each effect, one activation per effect.

        private byte _ActivationPercentage;
        private int _Weight;

        public BaseSkillActivation()
        {
            ListRequirement = new List<BaseSkillRequirement>();
            ListEffect = new List<BaseEffect>();
            ListEffectTarget = new List<List<string>>();
            ListEffectTargetReal = new List<List<AutomaticSkillTargetType>>();
            _ActivationPercentage = 100;
            _Weight = -1;
        }

        public BaseSkillActivation(BaseSkillActivation Clone)
        {
            ListRequirement = new List<BaseSkillRequirement>(Clone.ListRequirement.Count);
            foreach (BaseSkillRequirement ActiveRequirement in Clone.ListRequirement)
            {
                ListRequirement.Add(ActiveRequirement.Copy());
            }

            ListEffect = new List<BaseEffect>(Clone.ListEffect.Count);
            foreach (BaseEffect ActiveEffect in Clone.ListEffect)
            {
                ListEffect.Add(ActiveEffect.Copy());
            }

            ListEffectTarget = new List<List<string>>(Clone.ListEffectTarget.Count);
            foreach (List<string> ActiveEffectTarget in Clone.ListEffectTarget)
            {
                ListEffectTarget.Add(ActiveEffectTarget);
            }

            ListEffectTargetReal = new List<List<AutomaticSkillTargetType>>(Clone.ListEffectTargetReal.Count);
            foreach (List<AutomaticSkillTargetType> ActiveListEffectTargetReal in Clone.ListEffectTargetReal)
            {
                List<AutomaticSkillTargetType> ActiveAutomaticSkillTargetType = new List<AutomaticSkillTargetType>(ActiveListEffectTargetReal.Count);
                foreach (AutomaticSkillTargetType ActiveEffectTargetReal in ActiveListEffectTargetReal)
                {
                    ActiveAutomaticSkillTargetType.Add(ActiveEffectTargetReal.Copy());
                }
                ListEffectTargetReal.Add(ActiveAutomaticSkillTargetType);
            }

            _ActivationPercentage = Clone._ActivationPercentage;
        }

        public BaseSkillActivation(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
        {
            ActivationPercentage = BR.ReadByte();
            _Weight = BR.ReadInt32();

            //Requirements
            int ListRequirementCount = BR.ReadInt32();
            ListRequirement = new List<BaseSkillRequirement>(ListRequirementCount);
            for (int R = 0; R < ListRequirementCount; R++)
            {
                ListRequirement.Add(BaseSkillRequirement.LoadCopy(BR, DicRequirement));
            }

            //Effects.
            int ListEffectCount = BR.ReadInt32();
            ListEffect = new List<BaseEffect>(ListEffectCount);
            ListEffectTarget = new List<List<string>>(ListEffectCount);
            ListEffectTargetReal = new List<List<AutomaticSkillTargetType>>(ListEffectCount);
            for (int E = 0; E < ListEffectCount; E++)
            {
                int ListActivationTypesCount = BR.ReadInt32();

                List<string> NewListActivationType = new List<string>(ListActivationTypesCount);
                List<AutomaticSkillTargetType> NewListEffectTargetReal = new List<AutomaticSkillTargetType>(ListActivationTypesCount);
                for (int A = 0; A < ListActivationTypesCount; A++)
                {
                    string ListActivationType = BR.ReadString();
                    NewListActivationType.Add(ListActivationType);
                    NewListEffectTargetReal.Add(AutomaticSkillTargetType.DicTargetType[ListActivationType].Copy());
                }

                ListEffectTarget.Add(NewListActivationType);
                ListEffectTargetReal.Add(NewListEffectTargetReal);

                ListEffect.Add(BaseEffect.FromFile(BR, DicRequirement, DicEffect));
            }
        }

        public bool CanActivate(string SkillRequirementToActivate, string SkillName)
        {
            bool CanActivate = true;
            //Check if you can attack with ActivationPercentage.
            if (!RandomHelper.RandomActivationCheck(ActivationPercentage))
                return false;

            for (int R = 0; R < ListRequirement.Count && CanActivate; R++)
            {
                bool IsManuallyActivated = ListRequirement[R].CanActicateManually(SkillRequirementToActivate);
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

        public void Activate(string SkillName)
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

        public void Save(BinaryWriter BW)
        {
            BW.Write(ActivationPercentage);
            BW.Write(_Weight);

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

        [CategoryAttribute("Activation Attributes"),
        DescriptionAttribute(".")]
        public int Weight
        {
            get { return _Weight; }
            set { _Weight = value; }
        }
    }
}
