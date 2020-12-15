using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class RotateRobotEffect : TripleThunderRobotEffect
    {
        public static string Name = "Rotate Robot";

        private float _Angle;


        public RotateRobotEffect()
            : base(Name, false)
        {
            _Angle = 0;
        }

        public RotateRobotEffect(TripleThunderRobotParams Params)
            : base(Name, false, Params)
        {
            _Angle = 0;
        }

        protected override void Load(BinaryReader BR)
        {
            _Angle = BR.ReadSingle();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_Angle);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            RobotAnimation ActiveRobot = Params.LocalContext.Target;
            ActiveRobot.Angle += _Angle;

            return null;

        }

        protected override BaseEffect DoCopy()
        {
            RotateRobotEffect NewEffect = new RotateRobotEffect(Params);

            NewEffect._Angle = _Angle;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            RotateRobotEffect NewEffect = (RotateRobotEffect)Copy;

            _Angle = NewEffect._Angle;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("")]
        public float Angle
        {
            get { return _Angle; }
            set { _Angle = value; }
        }

        #endregion
    }
}
