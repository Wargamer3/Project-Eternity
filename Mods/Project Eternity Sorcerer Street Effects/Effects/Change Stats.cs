using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeStatsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Change Stats";

        public enum Targets { Self, Opponent }
        public enum Stats { FinalST, BaseST, FinalHP, BaseHP, MaxHP, }//Final = battle only, Base = Modify the card stats

        private Targets _Target;
        private Stats _Stat;
        private SignOperators _SignOperator;
        private string _Value;

        public ChangeStatsEffect()
            : base(Name, false)
        {
            _Target = Targets.Self;
            _Stat = Stats.FinalST;
            _SignOperator = SignOperators.PlusEqual;
            _Value = string.Empty;
        }

        public ChangeStatsEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _Target = Targets.Self;
            _Stat = Stats.FinalST;
            _SignOperator = SignOperators.PlusEqual;
            _Value = string.Empty;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _Stat = (Stats)BR.ReadByte();
            _SignOperator = (SignOperators)BR.ReadByte();
            _Value = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_Stat);
            BW.Write((byte)_SignOperator);
            BW.Write(_Value);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.ActiveParser.Evaluate(_Value);

            SorcererStreetBattleContext.BattleCreatureInfo RealTarget = Params.GlobalContext.SelfCreature;
            if (_Target == Targets.Opponent)
            {
                RealTarget = Params.GlobalContext.OpponentCreature;
            }

            if (_Stat == Stats.FinalST)
            {
                switch (_SignOperator)
                {
                    case SignOperators.Equal:
                        RealTarget.BonusST = int.Parse(EvaluationResult, CultureInfo.InvariantCulture) - RealTarget.Creature.CurrentST;
                        return "ST=" + EvaluationResult;

                    case SignOperators.PlusEqual:
                        RealTarget.BonusST += int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.DividedEqual:
                        RealTarget.BonusST -= RealTarget.Creature.CurrentST - RealTarget.Creature.CurrentST / (int.Parse(EvaluationResult, CultureInfo.InvariantCulture)) - 1;
                        break;

                    case SignOperators.MinusEqual:
                        RealTarget.BonusST -= int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.MultiplicatedEqual:
                        RealTarget.BonusST += RealTarget.Creature.CurrentST * (int.Parse(EvaluationResult, CultureInfo.InvariantCulture)) - 1;
                        break;

                    case SignOperators.ModuloEqual:
                        RealTarget.BonusST += RealTarget.Creature.CurrentST % int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;
                }

                return "ST+" + EvaluationResult;
            }
            else if (_Stat == Stats.BaseST)
            {
                switch (_SignOperator)
                {
                    case SignOperators.Equal:
                        RealTarget.Creature.CurrentST = int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        return "ST=" + EvaluationResult;

                    case SignOperators.PlusEqual:
                        RealTarget.Creature.CurrentST += int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.DividedEqual:
                        RealTarget.Creature.CurrentST -= RealTarget.Creature.CurrentST - RealTarget.Creature.CurrentST / (int.Parse(EvaluationResult, CultureInfo.InvariantCulture)) - 1;
                        break;

                    case SignOperators.MinusEqual:
                        RealTarget.Creature.CurrentST -= int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        RealTarget.Creature.CurrentST = Math.Max(0, RealTarget.Creature.CurrentST);
                        break;

                    case SignOperators.MultiplicatedEqual:
                        RealTarget.Creature.CurrentST += RealTarget.Creature.CurrentST * (int.Parse(EvaluationResult, CultureInfo.InvariantCulture)) - 1;
                        break;

                    case SignOperators.ModuloEqual:
                        RealTarget.Creature.CurrentST += RealTarget.Creature.CurrentST % int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;
                }

                return "ST+" + EvaluationResult;
            }
            else if (_Stat == Stats.FinalHP)
            {
                switch (_SignOperator)
                {
                    case SignOperators.Equal:
                        RealTarget.BonusHP = int.Parse(EvaluationResult, CultureInfo.InvariantCulture) - RealTarget.Creature.CurrentHP;
                        return "HP=" + EvaluationResult;

                    case SignOperators.PlusEqual:
                        RealTarget.BonusHP += int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.DividedEqual:
                        RealTarget.BonusHP -= RealTarget.Creature.CurrentHP - RealTarget.Creature.CurrentHP / (int.Parse(EvaluationResult, CultureInfo.InvariantCulture)) - 1;
                        break;

                    case SignOperators.MinusEqual:
                        RealTarget.BonusHP -= int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        if (RealTarget.FinalHP < 0)
                        {
                            RealTarget.LandHP = 0;
                            RealTarget.BonusHP = 0;
                            RealTarget.Creature.CurrentHP = 0;
                        }
                        ActionPanelBattleDefenderDefeatedPhase.DestroyDeadCreatures(Params.Map);
                        return "HP-" + EvaluationResult;

                    case SignOperators.MultiplicatedEqual:
                        RealTarget.BonusHP += RealTarget.Creature.CurrentHP * (int.Parse(EvaluationResult, CultureInfo.InvariantCulture)) - 1;
                        break;

                    case SignOperators.ModuloEqual:
                        RealTarget.BonusHP += RealTarget.Creature.CurrentHP % int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;
                }

                return "HP+" + EvaluationResult;
            }
            else if (_Stat == Stats.BaseHP)
            {
                switch (_SignOperator)
                {
                    case SignOperators.Equal:
                        RealTarget.Creature.CurrentHP = int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        return "HP=" + EvaluationResult;

                    case SignOperators.PlusEqual:
                        RealTarget.Creature.CurrentHP += int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.DividedEqual:
                        RealTarget.Creature.CurrentHP -= RealTarget.Creature.CurrentHP - RealTarget.Creature.CurrentHP / (int.Parse(EvaluationResult, CultureInfo.InvariantCulture)) - 1;
                        if (RealTarget.FinalHP < 0 || RealTarget.Creature.CurrentHP < 0 || RealTarget.Creature.MaxHP < 0)
                        {
                            RealTarget.LandHP = 0;
                            RealTarget.BonusHP = 0;
                            RealTarget.Creature.CurrentHP = 0;
                        }
                        break;

                    case SignOperators.MinusEqual:
                        RealTarget.Creature.CurrentHP -= int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        if (RealTarget.FinalHP < 0 || RealTarget.Creature.CurrentHP < 0 || RealTarget.Creature.MaxHP < 0)
                        {
                            RealTarget.LandHP = 0;
                            RealTarget.BonusHP = 0;
                            RealTarget.Creature.CurrentHP = 0;
                        }
                        ActionPanelBattleDefenderDefeatedPhase.DestroyDeadCreatures(Params.Map);
                        return "HP-" + EvaluationResult;

                    case SignOperators.MultiplicatedEqual:
                        RealTarget.Creature.CurrentHP += RealTarget.Creature.CurrentHP * (int.Parse(EvaluationResult, CultureInfo.InvariantCulture)) - 1;
                        break;

                    case SignOperators.ModuloEqual:
                        RealTarget.Creature.CurrentHP += RealTarget.Creature.CurrentHP % int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;
                }

                RealTarget.Creature.CurrentHP = Math.Min(100, RealTarget.Creature.CurrentHP);
                ActionPanelBattleDefenderDefeatedPhase.DestroyDeadCreatures(Params.Map);

                return "HP+" + EvaluationResult;
            }
            else if (_Stat == Stats.MaxHP)
            {
                int OldMaxHP = RealTarget.Creature.MaxHP;

                switch (_SignOperator)
                {
                    case SignOperators.Equal:
                        RealTarget.Creature.MaxHP = int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        RealTarget.Creature.CurrentHP = int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        return "Max HP=" + EvaluationResult;

                    case SignOperators.PlusEqual:
                        RealTarget.Creature.MaxHP += int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        RealTarget.Creature.CurrentHP += int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.DividedEqual:
                        RealTarget.Creature.MaxHP -= RealTarget.Creature.MaxHP - RealTarget.Creature.MaxHP / (int.Parse(EvaluationResult, CultureInfo.InvariantCulture)) - 1;
                        RealTarget.Creature.CurrentHP += RealTarget.Creature.MaxHP - OldMaxHP;
                        return "Max HP-" + EvaluationResult;

                    case SignOperators.MinusEqual:
                        RealTarget.Creature.MaxHP -= int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        RealTarget.Creature.CurrentHP += RealTarget.Creature.MaxHP - OldMaxHP;
                        return "Max HP-" + EvaluationResult;

                    case SignOperators.MultiplicatedEqual:
                        RealTarget.Creature.MaxHP += RealTarget.Creature.MaxHP * (int.Parse(EvaluationResult, CultureInfo.InvariantCulture)) - 1;
                        RealTarget.Creature.CurrentHP += RealTarget.Creature.MaxHP - OldMaxHP;
                        break;

                    case SignOperators.ModuloEqual:
                        RealTarget.Creature.MaxHP += RealTarget.Creature.MaxHP % int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        RealTarget.Creature.CurrentHP += RealTarget.Creature.MaxHP - OldMaxHP;
                        break;
                }

                RealTarget.Creature.MaxHP = Math.Min(100, RealTarget.Creature.MaxHP);
                RealTarget.Creature.CurrentHP = Math.Min(100, RealTarget.Creature.CurrentHP);

                if (RealTarget.Creature.MaxHP < RealTarget.Creature.CurrentHP)
                {
                    RealTarget.Creature.CurrentHP = RealTarget.Creature.MaxHP;
                }
                if (RealTarget.FinalHP < 0)
                {
                    RealTarget.LandHP = 0;
                    RealTarget.BonusHP = 0;
                    RealTarget.Creature.CurrentHP = 0;
                }
                ActionPanelBattleDefenderDefeatedPhase.DestroyDeadCreatures(Params.Map);

                return "Max HP+" + EvaluationResult;
            }

            ActionPanelBattleDefenderDefeatedPhase.DestroyDeadCreatures(Params.Map);

            return "ST+" + EvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            ChangeStatsEffect NewEffect = new ChangeStatsEffect(Params);

            NewEffect._Target = _Target;
            NewEffect._Stat = _Stat;
            NewEffect._SignOperator = _SignOperator;
            NewEffect._Value = _Value;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeStatsEffect NewEffect = (ChangeStatsEffect)Copy;

            _Target = NewEffect._Target;
            _Stat = NewEffect._Stat;
            _SignOperator = NewEffect._SignOperator;
            _Value = NewEffect._Value;
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
        public Stats Stat
        {
            get
            {
                return _Stat;
            }
            set
            {
                _Stat = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public SignOperators SignOperator
        {
            get
            {
                return _SignOperator;
            }
            set
            {
                _SignOperator = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        #endregion
    }
}
