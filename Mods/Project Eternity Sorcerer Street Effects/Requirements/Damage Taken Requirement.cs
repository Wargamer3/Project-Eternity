using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetDamageTakenRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent }

        private Targets _Target;
        private Operators.LogicOperators _LogicOperator;
        private int _DamageTaken;

        public SorcererStreetDamageTakenRequirement()
            : this(null)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.GreaterOrEqual;
            _DamageTaken = 0;
        }

        public SorcererStreetDamageTakenRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Damage Taken", Params)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.GreaterOrEqual;
            _DamageTaken = 0;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_LogicOperator);
            BW.Write(_DamageTaken);
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _DamageTaken = BR.ReadInt32();
        }

        public override bool CanActivatePassive()
        {
            int CreatureHP = 0;
            int CreatureMaxHP = 0;
            switch (_Target)
            {
                case Targets.Self:
                    CreatureHP = Params.GlobalContext.SelfCreature.FinalHP;
                    CreatureMaxHP = Params.GlobalContext.SelfCreature.Creature.MaxHP;
                    break;
                case Targets.Opponent:
                    CreatureHP = Params.GlobalContext.OpponentCreature.FinalHP;
                    CreatureMaxHP = Params.GlobalContext.OpponentCreature.Creature.MaxHP;
                    break;
            }

            int CurrentDamageTaken = CreatureMaxHP - CreatureHP;
            return Operators.CompareValue(LogicOperator, CurrentDamageTaken, DamageTaken);
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetDamageTakenRequirement NewRequirement = new SorcererStreetDamageTakenRequirement(Params);

            NewRequirement._Target = _Target;
            NewRequirement._LogicOperator = _LogicOperator;
            NewRequirement._DamageTaken = _DamageTaken;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetDamageTakenRequirement CopyRequirement = Copy as SorcererStreetDamageTakenRequirement;

            if (CopyRequirement != null)
            {
                _Target = CopyRequirement._Target;
                _LogicOperator = CopyRequirement._LogicOperator;
                _DamageTaken = CopyRequirement._DamageTaken;
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
        public int DamageTaken
        {
            get
            {
                return _DamageTaken;
            }
            set
            {
                _DamageTaken = value;
            }
        }

        #endregion
    }
}
