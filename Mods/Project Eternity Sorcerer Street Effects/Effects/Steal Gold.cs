using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SteaGoldEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Steal Gold";

        private string _Value;

        public SteaGoldEffect()
            : base(Name, false)
        {
            _Value = string.Empty;
        }

        public SteaGoldEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _Value = string.Empty;
        }

        protected override void Load(BinaryReader BR)
        {
            _Value = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_Value);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            int GoldToSteal = int.Parse(Params.ActiveParser.Evaluate(_Value), CultureInfo.InvariantCulture);
            GoldToSteal = Math.Min(Params.GlobalContext.OpponentCreature.Owner.Magic, GoldToSteal);

            Params.GlobalContext.OpponentCreature.Owner.Magic -= GoldToSteal;
            Params.GlobalContext.SelfCreature.Owner.Magic += GoldToSteal;

            return "Stole " + GoldToSteal;
        }
        protected override BaseEffect DoCopy()
        {
            SteaGoldEffect NewEffect = new SteaGoldEffect(Params);

            NewEffect._Value = _Value;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SteaGoldEffect NewEffect = (SteaGoldEffect)Copy;

            _Value = NewEffect._Value;
        }

        #region Properties

        [CategoryAttribute(""),
        DescriptionAttribute("How much to steal."),
        DefaultValueAttribute(0)]
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
