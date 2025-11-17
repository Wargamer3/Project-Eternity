using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetTerritoryTargetType : ManualSkillActivationSorcererStreet
    {
        public static string Name = "Sorcerer Street Territory";

        private byte _TerritoryLevel;

        public SorcererStreetTerritoryTargetType()
            : this(null)
        {

        }

        public SorcererStreetTerritoryTargetType(SorcererStreetBattleParams Context)
            : base(Name, true, Context)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            return true;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelSelectCreatureSpell(Params.Map, ActiveSkill, true, true));
        }

        public override ManualSkillTarget Copy()
        {
            return new SorcererStreetTerritoryTargetType(Params);
        }

        public override void CopyMembers(ManualSkillTarget Copy)
        {
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public byte CreatureType
        {
            get
            {
                return _TerritoryLevel;
            }
            set
            {
                _TerritoryLevel = value;
            }
        }

        #endregion
    }
}
