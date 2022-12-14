using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public abstract class AnimationBackground2DBase
    {
        public readonly string BackgroundType;

        public Vector2 CurrentPosition;
        protected bool _UseParallaxScrolling;
        protected Vector2 _StartPosition;
        protected Vector2 _Speed;
        protected Vector2 _Scale;
        protected float _Depth;
        protected bool _RepeatX;
        protected bool _RepeatY;
        protected int _RepeatXOffset;
        protected int _RepeatYOffset;
        protected bool _FlipOnRepeatX;
        protected bool _FlipOnRepeatY;
        protected Vector2 SpriteCenter;
        protected Color _Color;

        protected AnimationBackground2DBase(string BackgroundType)
        {
            this.BackgroundType = BackgroundType;
            CurrentPosition = Vector2.Zero;
            _StartPosition = Vector2.Zero;
            _Speed = Vector2.Zero;
            _Scale = Vector2.One;
            _Depth = 0;
            _UseParallaxScrolling = true;

            _RepeatX = true;
            _RepeatY = true;
            _FlipOnRepeatX = false;
            _FlipOnRepeatY = false;
            _Color = Color.White;
        }

        public static AnimationBackground2DBase LoadFromFile(ContentManager Content, BinaryReader BR)
        {
            string BackgroundType = BR.ReadString();

            if (BackgroundType == AnimationBackground2DImage.BackgroundTypeName)
            {
                return new AnimationBackground2DImage(Content, BR);
            }
            else if (BackgroundType == AnimationBackground2DImageComplex.BackgroundTypeName)
            {
                return new AnimationBackground2DImageComplex(Content, BR);
            }

            return null;
        }

        protected void LoadBase(BinaryReader BR)
        {
            _StartPosition = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            CurrentPosition = _StartPosition;
            _Speed = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            _Scale = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            _Depth = BR.ReadSingle();
            _UseParallaxScrolling = BR.ReadBoolean();

            _RepeatX = BR.ReadBoolean();
            _RepeatY = BR.ReadBoolean();
            _RepeatXOffset = BR.ReadInt32();
            _RepeatYOffset = BR.ReadInt32();
            _FlipOnRepeatX = BR.ReadBoolean();
            _FlipOnRepeatY = BR.ReadBoolean();
            _Color = new Color(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), BR.ReadByte());
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(BackgroundType);

            DoSave(BW);
        }

        protected virtual void DoSave(BinaryWriter BW)
        {
            BW.Write(_StartPosition.X);
            BW.Write(_StartPosition.Y);
            BW.Write(_Speed.X);
            BW.Write(_Speed.Y);
            BW.Write(_Scale.X);
            BW.Write(_Scale.Y);
            BW.Write(_Depth);
            BW.Write(_UseParallaxScrolling);

            BW.Write(_RepeatX);
            BW.Write(_RepeatY);
            BW.Write(_RepeatXOffset);
            BW.Write(_RepeatYOffset);
            BW.Write(_FlipOnRepeatX);
            BW.Write(_FlipOnRepeatY);
            BW.Write(_Color.R);
            BW.Write(_Color.G);
            BW.Write(_Color.B);
            BW.Write(_Color.A);
        }

        public void Move(GameTime gameTime)
        {
            CurrentPosition += Speed;
        }

        public abstract bool CollideWith(Vector2 Position, float CameraX, float CameraY, int ScreenWidth, int ScreenHeight);

        public abstract void Draw(CustomSpriteBatch g, float CameraX, float CameraY, int ScreenWidth, int ScreenHeight);

        #region Properties

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public Vector2 StartPosition
        {
            get
            {
                return _StartPosition;
            }
            set
            {
                _StartPosition = value;
                CurrentPosition = _StartPosition;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public Vector2 Speed
        {
            get
            {
                return _Speed;
            }
            set
            {
                _Speed = value;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public Vector2 Scale
        {
            get
            {
                return _Scale;
            }
            set
            {
                _Scale = value;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public float Depth
        {
            get
            {
                return _Depth;
            }
            set
            {
                _Depth = value;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public Color Color
        {
            get
            {
                return _Color;
            }
            set
            {
                _Color = value;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public bool UseParallaxScrolling
        {
            get
            {
                return _UseParallaxScrolling;
            }
            set
            {
                _UseParallaxScrolling = value;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public bool RepeatX
        {
            get
            {
                return _RepeatX;
            }
            set
            {
                _RepeatX = value;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public bool RepeatY
        {
            get
            {
                return _RepeatY;
            }
            set
            {
                _RepeatY = value;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public int RepeatXOffset
        {
            get
            {
                return _RepeatXOffset;
            }
            set
            {
                _RepeatXOffset = value;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public int RepeatYOffset
        {
            get
            {
                return _RepeatYOffset;
            }
            set
            {
                _RepeatYOffset = value;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public bool FlipOnRepeatX
        {
            get
            {
                return _FlipOnRepeatX;
            }
            set
            {
                _FlipOnRepeatX = value;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public bool FlipOnRepeatY
        {
            get
            {
                return _FlipOnRepeatY;
            }
            set
            {
                _FlipOnRepeatY = value;
            }
        }

        #endregion
    }
}
