using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DealDamageEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Deal Damage";

        private string _DamageToDeal;

        public DealDamageEffect()
            : base(Name, false)
        {
            _DamageToDeal = string.Empty;
        }

        public DealDamageEffect(SorcererStreetBattleParams Params)
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

            Params.GlobalContext.OpponentCreature.ReceiveDamage(int.Parse(EvaluationResult, CultureInfo.InvariantCulture));

            if (Params.GlobalContext.SelfCreature.FinalHP <= 0)
            {
                ActionPanelBattleDefenderDefeatedPhase.DestroyDeadCreature(Params.Map);
            }

            return "Damage dealt" + _DamageToDeal;
        }

        protected override BaseEffect DoCopy()
        {
            DealDamageEffect NewEffect = new DealDamageEffect(Params);

            NewEffect.DamageToDeal = DamageToDeal;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            DealDamageEffect NewEffect = (DealDamageEffect)Copy;

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
