using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DealDamageSelfEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Deal Damage Self";

        private string _DamageToDeal;

        public DealDamageSelfEffect()
            : base(Name, false)
        {
            _DamageToDeal = string.Empty;
        }

        public DealDamageSelfEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _DamageToDeal = string.Empty;
        }

        protected override void Load(BinaryReader BR)
        {
            _DamageToDeal = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_DamageToDeal);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.ActiveParser.Evaluate(_DamageToDeal);

            Params.GlobalContext.SelfCreature.ReceiveDamage(int.Parse(EvaluationResult, CultureInfo.InvariantCulture));

            ActionPanelBattleDefenderDefeatedPhase.DestroyDeadCreatures(Params.Map);

            return "Damage dealt" + _DamageToDeal;
        }

        protected override BaseEffect DoCopy()
        {
            DealDamageSelfEffect NewEffect = new DealDamageSelfEffect(Params);

            NewEffect.DamageToDeal = DamageToDeal;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            DealDamageSelfEffect NewEffect = (DealDamageSelfEffect)Copy;

            DamageToDeal = NewEffect.DamageToDeal;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string DamageToDeal
        {
            get
            {
                return _DamageToDeal;
            }
            set
            {
                _DamageToDeal = value;
            }
        }

        #endregion
    }
}
