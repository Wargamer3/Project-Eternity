using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetTerriotryLevelRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent }

        private Targets _Target;
        private Operators.LogicOperators _LogicOperator;
        private int _TerritoryLevel;

        public SorcererStreetTerriotryLevelRequirement()
            : this(null)
        {
        }

        public SorcererStreetTerriotryLevelRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Territory Level", GlobalContext)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.GreaterOrEqual;
            _TerritoryLevel = 0;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_LogicOperator);
            BW.Write((byte)_TerritoryLevel);
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _TerritoryLevel = BR.ReadByte();
        }

        public override bool CanActivatePassive()
        {
            int TerritoryLevel = GlobalContext.ActiveTerrain.LandLevel;

            return Operators.CompareValue(LogicOperator, TerritoryLevel, _TerritoryLevel);
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetTerriotryLevelRequirement NewRequirement = new SorcererStreetTerriotryLevelRequirement(GlobalContext);

            NewRequirement._Target = _Target;
            NewRequirement._LogicOperator = _LogicOperator;
            NewRequirement._TerritoryLevel = _TerritoryLevel;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetTerriotryLevelRequirement CopyRequirement = Copy as SorcererStreetTerriotryLevelRequirement;

            if (CopyRequirement != null)
            {
                _Target = CopyRequirement._Target;
                _LogicOperator = CopyRequirement._LogicOperator;
                _TerritoryLevel = CopyRequirement._TerritoryLevel;
            }
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
        public Operators.LogicOperators LogicOperator
        {
            get
            {
                return _LogicOperator;
            }
            set
            {
                _LogicOperator = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public int TerritoryLevel
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
