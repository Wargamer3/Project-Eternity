using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetTerriotryRequirement : SorcererStreetRequirement
    {
        private string _CostToActivate;

        public SorcererStreetTerriotryRequirement()
            : this(null)
        {
        }

        public SorcererStreetTerriotryRequirement(SorcererStreetBattleParams Params)
            : base(CreatureCard.TerritoryRequirementName, Params)
        {
            _CostToActivate = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_CostToActivate);
        }

        protected override void Load(BinaryReader BR)
        {
            _CostToActivate = BR.ReadString();
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetTerriotryRequirement NewRequirement = new SorcererStreetTerriotryRequirement(Params);

            NewRequirement._CostToActivate = _CostToActivate;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetTerriotryRequirement CopyRequirement = (SorcererStreetTerriotryRequirement)Copy;

            _CostToActivate = CopyRequirement._CostToActivate;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string Price
        {
            get
            {
                return _CostToActivate;
            }
            set
            {
                _CostToActivate = value;
            }
        }

        #endregion
    }
}
