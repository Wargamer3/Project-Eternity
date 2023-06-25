using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class IncreaseSTEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Increase ST";

        private string _STIncrease;

        public IncreaseSTEffect()
            : base(Name, false)
        {
            _STIncrease = string.Empty;
        }

        public IncreaseSTEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _STIncrease = string.Empty;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _STIncrease = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_STIncrease);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.ActiveParser.Evaluate(_STIncrease);

            Params.IncreaseSelfST(int.Parse(EvaluationResult, CultureInfo.InvariantCulture));

            return "ST+" + EvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            IncreaseSTEffect NewEffect = new IncreaseSTEffect(Params);

            NewEffect._STIncrease = _STIncrease;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            IncreaseSTEffect NewEffect = (IncreaseSTEffect)Copy;

            _STIncrease = NewEffect._STIncrease;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string ST
        {
            get
            {
                return _STIncrease;
            }
            set
            {
                _STIncrease = value;
            }
        }

        #endregion
    }
}
