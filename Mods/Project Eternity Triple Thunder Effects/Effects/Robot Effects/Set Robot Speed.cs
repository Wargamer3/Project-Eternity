using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class SetRobotSpeedEffect : TripleThunderRobotEffect
    {
        public static string Name = "Set Robot Speed";

        private float _SpeedX;
        private float _SpeedY;

        public SetRobotSpeedEffect()
            : base(Name, false)
        {
        }

        public SetRobotSpeedEffect(TripleThunderRobotParams Params)
            : base(Name, false, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _SpeedX = BR.ReadSingle();
            _SpeedY = BR.ReadSingle();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_SpeedX);
            BW.Write(_SpeedY);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            if (Params.LocalContext.Target.DicStoredVariable.ContainsKey("SpeedX"))
            {
                Params.LocalContext.Target.DicStoredVariable["SpeedX"] = _SpeedX.ToString();
            }
            else
            {
                Params.LocalContext.Target.DicStoredVariable.Add("SpeedX", _SpeedX.ToString());
            }
            if (Params.LocalContext.Target.DicStoredVariable.ContainsKey("SpeedY"))
            {
                Params.LocalContext.Target.DicStoredVariable["SpeedY"] = _SpeedY.ToString();
            }
            else
            {
                Params.LocalContext.Target.DicStoredVariable.Add("SpeedY", _SpeedY.ToString());
            }

            Params.LocalContext.Target.Speed = new Vector2(_SpeedX, _SpeedY);

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            SetRobotSpeedEffect NewEffect = new SetRobotSpeedEffect(Params);

            NewEffect._SpeedX = _SpeedX;
            NewEffect._SpeedY = _SpeedY;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SetRobotSpeedEffect NewEffect = (SetRobotSpeedEffect)Copy;

            _SpeedX = NewEffect._SpeedX;
            _SpeedY = NewEffect._SpeedY;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("")]
        public float SpeedX
        {
            get { return _SpeedX; }
            set { _SpeedX = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("")]
        public float SpeedY
        {
            get { return _SpeedY; }
            set { _SpeedY = value; }
        }

        #endregion
    }
}
