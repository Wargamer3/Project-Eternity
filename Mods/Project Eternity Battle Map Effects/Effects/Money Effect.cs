using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class MoneyEffect : SkillEffect
    {
        public static string Name = "Money Effect";

        private string _BonusMoneyValue;

        public MoneyEffect()
            : base(Name, false)
        {
        }

        public MoneyEffect(UnitEffectParams Params)
            : base(Name, false, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _BonusMoneyValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_BonusMoneyValue);
        }

        protected override string DoExecuteEffect()
        {
            throw new NotImplementedException();
        }

        protected override void ReactivateEffect()
        {
            throw new NotImplementedException();
        }

        protected override BaseEffect DoCopy()
        {
            MoneyEffect NewEffect = new MoneyEffect(Params);

            NewEffect._BonusMoneyValue = _BonusMoneyValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            MoneyEffect NewEffect = (MoneyEffect)Copy;

            _BonusMoneyValue = NewEffect._BonusMoneyValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _BonusMoneyValue; }
            set { _BonusMoneyValue = value; }
        }

        #endregion
    }
}
