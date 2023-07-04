using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetElementRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent }
        public enum ElementChoices { DifferentFromOpponent, Neutral, Fire, Water, Earth, Air }

        private Targets _Target;

        public ElementChoices[] ArrayElement;

        public SorcererStreetElementRequirement()
            : this(null)
        {
            _Target = Targets.Self;
        }

        public SorcererStreetElementRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Element", GlobalContext)
        {
            _Target = Targets.Self;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)ArrayElement.Length);
            for (int A = 0; A < ArrayElement.Length; ++A)
            {
                BW.Write((byte)ArrayElement[A]);
            }
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            int ArrayAffinityLength = BR.ReadByte();
            ArrayElement = new ElementChoices[ArrayAffinityLength];
            for (int A = 0; A < ArrayAffinityLength; ++A)
            {
                ArrayElement[A] = (ElementChoices)BR.ReadByte();
            }
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetElementRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public Targets Target
        {
            get
            {
                return _Target;
            }
            set
            {
                _Target = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public ElementChoices[] Elements
        {
            get
            {
                return ArrayElement;
            }
            set
            {
                ArrayElement = value;
            }
        }

        #endregion
    }
}
