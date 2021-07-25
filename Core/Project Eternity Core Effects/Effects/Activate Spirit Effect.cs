using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class ActivateSpiritEffect : SkillEffect
    {
        public static string Name = "Activate Spirit Effect";

        private string _ActivateSpiritValue;

        public ActivateSpiritEffect()
            : base(Name, false)
        {
            _ActivateSpiritValue = string.Empty;
        }

        public ActivateSpiritEffect(UnitEffectParams Params)
            : base(Name, false, Params)
        {
            _ActivateSpiritValue = string.Empty;
        }

        protected override void Load(BinaryReader BR)
        {
            _ActivateSpiritValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_ActivateSpiritValue);
        }

        protected override string DoExecuteEffect()
        {
            throw new NotImplementedException();
            //AddPilotSpiritEffect(ActiveUnit, ActiveUnit.Pilot, TargetSquad, ActivePlayerIndex);
        }

        protected override BaseEffect DoCopy()
        {
            ActivateSpiritEffect NewEffect = new ActivateSpiritEffect(Params);

            NewEffect._ActivateSpiritValue = _ActivateSpiritValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ActivateSpiritEffect NewEffect = (ActivateSpiritEffect)Copy;

            _ActivateSpiritValue = NewEffect._ActivateSpiritValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _ActivateSpiritValue; }
            set { _ActivateSpiritValue = value; }
        }

        #endregion
    }
}
