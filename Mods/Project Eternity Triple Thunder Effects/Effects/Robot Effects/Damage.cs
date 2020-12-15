using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class DamageEffect : TripleThunderRobotEffect
    {
        public static string Name = "Damage";

        private int _Damage;

        public DamageEffect()
            : base(Name, false)
        {
        }

        public DamageEffect(TripleThunderRobotParams Params)
            : base(Name, false, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _Damage = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_Damage);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            Params.LocalContext.Target.HP -= _Damage;

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            DamageEffect NewEffect = new DamageEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("")]
        public int Damage
        {
            get { return _Damage; }
            set { _Damage = value; }
        }

        #endregion
    }
}
