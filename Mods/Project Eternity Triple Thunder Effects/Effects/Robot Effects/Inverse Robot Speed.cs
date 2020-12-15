using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class InverseRobotSpeedEffect : TripleThunderRobotEffect
    {
        public static string Name = "Inverse Robot Speed";

        public enum InverseDirections { Horizontal, Vertical, Both }

        private InverseDirections _InverseDirection;

        public InverseRobotSpeedEffect()
            : base(Name, false)
        {
            _InverseDirection = InverseDirections.Horizontal;
        }

        public InverseRobotSpeedEffect(TripleThunderRobotParams Params)
            : base(Name, false, Params)
        {
            _InverseDirection = InverseDirections.Horizontal;
        }

        protected override void Load(BinaryReader BR)
        {
            _InverseDirection = (InverseDirections)BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_InverseDirection);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            RobotAnimation ActiveRobot = Params.LocalContext.Target;
            float SpeedX = ActiveRobot.Speed.X;
            float SpeedY = ActiveRobot.Speed.Y;

            if (Params.LocalContext.Target.DicStoredVariable.ContainsKey("SpeedX"))
            {
                SpeedX = float.Parse(Params.LocalContext.Target.DicStoredVariable["SpeedX"]);
            }
            if (Params.LocalContext.Target.DicStoredVariable.ContainsKey("SpeedY"))
            {
                SpeedY = float.Parse(Params.LocalContext.Target.DicStoredVariable["SpeedY"]);
            }

            if (_InverseDirection == InverseDirections.Horizontal)
            {
                SpeedX = -SpeedX;
            }
            else if (_InverseDirection == InverseDirections.Vertical)
            {
                SpeedY = -SpeedY;
            }
            else if (_InverseDirection == InverseDirections.Both)
            {
                SpeedX = -SpeedX;
                SpeedY = -SpeedY;
            }
            ActiveRobot.Speed.X = SpeedX;
            ActiveRobot.Speed.Y = SpeedY;

            if (Params.LocalContext.Target.DicStoredVariable.ContainsKey("SpeedX"))
            {
                Params.LocalContext.Target.DicStoredVariable["SpeedX"] = SpeedX.ToString();
            }
            if (Params.LocalContext.Target.DicStoredVariable.ContainsKey("SpeedY"))
            {
                Params.LocalContext.Target.DicStoredVariable["SpeedY"] = SpeedY.ToString();
            }

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            InverseRobotSpeedEffect NewEffect = new InverseRobotSpeedEffect(Params);

            NewEffect._InverseDirection = _InverseDirection;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            InverseRobotSpeedEffect NewEffect = (InverseRobotSpeedEffect)Copy;

            _InverseDirection = NewEffect._InverseDirection;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("")]
        public InverseDirections InverseDirection
        {
            get { return _InverseDirection; }
            set { _InverseDirection = value; }
        }

        #endregion
    }
}
