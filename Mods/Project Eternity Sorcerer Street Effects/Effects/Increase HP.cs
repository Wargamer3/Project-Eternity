using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class IncreaseHPEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Increase HP";

        private string _HPIncrease;

        public IncreaseHPEffect()
            : base(Name, false)
        {
            _HPIncrease = string.Empty;
        }

        public IncreaseHPEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _HPIncrease = string.Empty;
        }

        protected override void Load(BinaryReader BR)
        {
            _HPIncrease = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_HPIncrease);
        }


        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.ActiveParser.Evaluate(_HPIncrease);

            Params.IncreaseSelfHP(int.Parse(EvaluationResult, CultureInfo.InvariantCulture));

            return "HP" + _HPIncrease;
        }

        protected override BaseEffect DoCopy()
        {
            IncreaseHPEffect NewEffect = new IncreaseHPEffect(Params);

            NewEffect.HP = HP;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            IncreaseHPEffect NewEffect = (IncreaseHPEffect)Copy;

            HP = NewEffect.HP;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string HP
        {
            get
            {
                return _HPIncrease;
            }
            set
            {
                _HPIncrease = value;
            }
        }

        #endregion
    }
}
