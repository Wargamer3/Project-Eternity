using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetAllCreaturesTargetType : ManualSkillActivationSorcererStreet
    {
        public static string Name = "Sorcerer Street All Creatures";

        private ActionPanelAllCreatureSpellConfirm.CreatureTypes _CreatureType;

        public SorcererStreetAllCreaturesTargetType()
            : this(null)
        {
        }

        public SorcererStreetAllCreaturesTargetType(SorcererStreetBattleParams Context)
            : base(Name, true, Context)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_CreatureType);
        }

        protected override void Load(BinaryReader BR)
        {
            _CreatureType = (ActionPanelAllCreatureSpellConfirm.CreatureTypes)BR.ReadByte();
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            return false;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAllCreatureSpellConfirm(Params.Map, ActiveSkill, Params.GlobalPlayerContext.ActivePlayerIndex, _CreatureType));
        }

        public override ManualSkillTarget Copy()
        {
            SorcererStreetAllCreaturesTargetType NewRequirement = new SorcererStreetAllCreaturesTargetType(Params);

            NewRequirement._CreatureType = _CreatureType;

            return NewRequirement;
        }

        public override void CopyMembers(ManualSkillTarget Copy)
        {
            SorcererStreetAllCreaturesTargetType NewTarget = (SorcererStreetAllCreaturesTargetType)Copy;

            _CreatureType = NewTarget._CreatureType;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public ActionPanelAllCreatureSpellConfirm.CreatureTypes CreatureType
        {
            get
            {
                return _CreatureType;
            }
            set
            {
                _CreatureType = value;
            }
        }

        #endregion
    }
}
