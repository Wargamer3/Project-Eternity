using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetTerriotryOwnedRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent }

        private Targets _Target;
        private Operators.LogicOperators _LogicOperator;
        private string _TerriotryOwned;

        public SorcererStreetTerriotryOwnedRequirement()
            : this(null)
        {
        }

        public SorcererStreetTerriotryOwnedRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Territory Owned", GlobalContext)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.GreaterOrEqual;
            _TerriotryOwned = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_LogicOperator);
            BW.Write(_TerriotryOwned);
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _TerriotryOwned = BR.ReadString();
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetTerriotryOwnedRequirement NewRequirement = new SorcererStreetTerriotryOwnedRequirement(GlobalContext);

            NewRequirement._Target = _Target;
            NewRequirement._LogicOperator = _LogicOperator;
            NewRequirement._TerriotryOwned = _TerriotryOwned;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetTerriotryOwnedRequirement CopyRequirement = (SorcererStreetTerriotryOwnedRequirement)Copy;

            _Target = CopyRequirement._Target;
            _LogicOperator = CopyRequirement._LogicOperator;
            _TerriotryOwned = CopyRequirement._TerriotryOwned;
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
        public string TerriotryOwned
        {
            get
            {
                return _TerriotryOwned;
            }
            set
            {
                _TerriotryOwned = value;
            }
        }

        #endregion
    }
}
