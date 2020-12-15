using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    //if KeyFrame[NextKeyFrame].IsProgressive, start a smooth transition
    public abstract class AnimationObjectKeyFrame
    {
        public bool IsUsed;

        protected AnimationObjectKeyFrame()
        {
        }

        public abstract void Save(BinaryWriter BW);

        public abstract AnimationObjectKeyFrame Copy(AnimationClass.AnimationLayer ActiveLayer);
    }

    public class VisibleAnimationObjectKeyFrame : AnimationObjectKeyFrame
    {
        protected bool _IsProgressive;
        public int NextKeyFrame;//Key Frame of the following VisibleAnimationObjectKeyFrame.
        private Vector2 _Position;
        public Vector2 MoveValue;
        private Vector2 _ScaleFactor;
        public Vector2 ScaleValue;
        public float AngleInRad;
        public float AngleValue;
        private int _Alpha;
        public float AlphaValue;
        private float _DrawingDepth;
        public float DrawingDepthValue;
        public List<Vector2> ListSpecialEffectNode;//Used to give special effects while moving.

        protected VisibleAnimationObjectKeyFrame()
        {
        }

        public VisibleAnimationObjectKeyFrame(Vector2 NextPosition, bool IsProgressive, int NextKeyFrame)
            : base()
        {
            this.NextKeyFrame = NextKeyFrame;
            this.IsProgressive = IsProgressive;
            _Position = NextPosition;
            _ScaleFactor = Vector2.One;
            AngleInRad = 0;
            _Alpha = 255;
            _DrawingDepth = 1;
            ListSpecialEffectNode = new List<Vector2>(1);
        }

        public VisibleAnimationObjectKeyFrame(BinaryReader BR)
        {
            _IsProgressive = BR.ReadBoolean();
            NextKeyFrame = BR.ReadInt32();
            _Position = new Vector2(BR.ReadInt32(), BR.ReadInt32());

            _ScaleFactor = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            AngleInRad = BR.ReadSingle();
            _Alpha = BR.ReadInt32();
            _DrawingDepth = BR.ReadSingle();

            int ListSpecialEffectNodeCount = BR.ReadInt32();
            ListSpecialEffectNode = new List<Vector2>(ListSpecialEffectNodeCount);
            for (int N = 0; N < ListSpecialEffectNodeCount; N++)
            {
                ListSpecialEffectNode.Add(new Vector2(BR.ReadInt32(), BR.ReadInt32()));
            }
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(_IsProgressive);
            BW.Write(NextKeyFrame);
            BW.Write((int)_Position.X);
            BW.Write((int)_Position.Y);
            BW.Write(_ScaleFactor.X);
            BW.Write(_ScaleFactor.Y);
            BW.Write(AngleInRad);
            BW.Write(_Alpha);
            BW.Write(_DrawingDepth);

            BW.Write(ListSpecialEffectNode.Count);
            for (int N = 0; N < ListSpecialEffectNode.Count; N++)
            {
                BW.Write((int)ListSpecialEffectNode[N].X);
                BW.Write((int)ListSpecialEffectNode[N].Y);
            }
        }

        public sealed override AnimationObjectKeyFrame Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            return CopyAsVisibleAnimationObjectKeyFrame(ActiveLayer);
        }

        protected virtual VisibleAnimationObjectKeyFrame CopyAsVisibleAnimationObjectKeyFrame(AnimationClass.AnimationLayer ActiveLayer)
        {
            VisibleAnimationObjectKeyFrame NewVisibleAnimationObjectKeyFrame = new VisibleAnimationObjectKeyFrame();

            NewVisibleAnimationObjectKeyFrame.UpdateFrom(this);

            return NewVisibleAnimationObjectKeyFrame;
        }

        public VisibleAnimationObjectKeyFrame Copy(AnimationClass.AnimationLayer ActiveLayer, int NextKeyFrame)
        {
            VisibleAnimationObjectKeyFrame NewKeyFrame = CopyAsVisibleAnimationObjectKeyFrame(ActiveLayer);
            NewKeyFrame.NextKeyFrame = NextKeyFrame;
            return NewKeyFrame;
        }

        protected void UpdateFrom(VisibleAnimationObjectKeyFrame Other)
        {
            NextKeyFrame = Other.NextKeyFrame;
            _IsProgressive = Other._IsProgressive;

            _Position = Other._Position;
            MoveValue = Other.MoveValue;
            _ScaleFactor = Other._ScaleFactor;
            ScaleValue = Other.ScaleValue;
            AngleInRad = Other.AngleInRad;
            AngleValue = Other.AngleValue;
            _Alpha = Other._Alpha;
            AlphaValue = Other.AlphaValue;
            _DrawingDepth = Other._DrawingDepth;
            DrawingDepthValue = Other.DrawingDepthValue;

            ListSpecialEffectNode = new List<Vector2>(Other.ListSpecialEffectNode.Count);
            foreach (Vector2 ActiveNode in Other.ListSpecialEffectNode)
            {
                ListSpecialEffectNode.Add(ActiveNode);
            }
        }

        #region Properties

        [CategoryAttribute("Animated Bitmap Move Event"),
        DescriptionAttribute(".")]
        public Vector2 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }

        [CategoryAttribute("Animated Bitmap Move Event"),
        DescriptionAttribute(".")]
        public Vector2 ScaleFactor
        {
            get
            {
                return _ScaleFactor;
            }
            set
            {
                _ScaleFactor = value;
            }
        }

        [CategoryAttribute("Animated Bitmap Move Event"),
        DescriptionAttribute(".")]
        public float Angle
        {
            get
            {
                return MathHelper.ToDegrees(AngleInRad);
            }
            set
            {
                AngleInRad = MathHelper.ToRadians(value);
            }
        }

        [CategoryAttribute("Animated Bitmap Move Event"),
        DescriptionAttribute(".")]
        public int Alpha
        {
            get
            {
                return _Alpha;
            }
            set
            {
                _Alpha = value;
            }
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public float DrawingDepth
        {
            get
            {
                return _DrawingDepth;
            }
            set
            {
                _DrawingDepth = value;
            }
        }

        [CategoryAttribute("Animated Bitmap Move Event"),
        DescriptionAttribute(".")]
        public byte SpecialEffectNodes
        {
            get
            {
                return (byte)ListSpecialEffectNode.Count;
            }
            set
            {
                if (value < ListSpecialEffectNode.Count)
                    ListSpecialEffectNode.RemoveRange(value, ListSpecialEffectNode.Count - value);
                else
                {
                    while (value > ListSpecialEffectNode.Count)
                        ListSpecialEffectNode.Add(new Vector2(_Position.X, _Position.Y));
                }
            }
        }

        [CategoryAttribute("Animated Bitmap Event Attributes"),
        DescriptionAttribute("Animation tweening.")]
        public bool IsProgressive
        {
            get
            {
                return _IsProgressive;
            }
            set
            {
                _IsProgressive = value;
            }
        }

        #endregion
    }
}
