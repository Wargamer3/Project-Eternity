using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class EXPEffect : SkillEffect
    {
        public static string Name = "EXP Effect";

        private string _BonusEXPValue;

        public EXPEffect()
            : base(Name, false)
        {
        }

        public EXPEffect(UnitEffectParams Params)
            : base(Name, false, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _BonusEXPValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_BonusEXPValue);
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
            EXPEffect NewEffect = new EXPEffect(Params);

            NewEffect._BonusEXPValue = _BonusEXPValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            EXPEffect NewEffect = (EXPEffect)Copy;

            _BonusEXPValue = NewEffect._BonusEXPValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _BonusEXPValue; }
            set { _BonusEXPValue = value; }
        }

        #endregion
    }
}
