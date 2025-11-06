using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetItemNotEquipedRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent }

        private Targets _Target;

        public SorcererStreetItemNotEquipedRequirement()
            : this(null)
        {
        }

        public SorcererStreetItemNotEquipedRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Item Not Equiped", Params)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
        }

        public override bool CanActivatePassive()
        {
            if (_Target == Targets.Self)
            {
                return Params.GlobalContext.SelfCreature.Item == null;
            }
            else
            {
                return Params.GlobalContext.OpponentCreature.Item == null;
            }
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetItemNotEquipedRequirement NewRequirement = new SorcererStreetItemNotEquipedRequirement(Params);

            NewRequirement._Target = _Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetItemNotEquipedRequirement CopyRequirement = (SorcererStreetItemNotEquipedRequirement)Copy;

            _Target = CopyRequirement._Target;
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

        #endregion
    }
}
