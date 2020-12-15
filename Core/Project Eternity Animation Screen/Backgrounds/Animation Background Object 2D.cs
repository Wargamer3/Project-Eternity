using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimationBackgroundObject2D
    {
        public List<AnimationBackgroundLink> ListBackgroundLink;
        public Texture2D sprBackground;
        public string BackgroundPath;

        public AnimationBackgroundObject2D(ContentManager Content, string Path)
            : base()
        {
            FileStream FS = new FileStream("Content/Animations/Background Objects 2D/" + Path + ".pebo", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            BackgroundPath = BR.ReadString();
            if (!string.IsNullOrEmpty(Path))
            {
                sprBackground = Content.Load<Texture2D>("Animations/Background Sprites/" + BackgroundPath);
            }

            int BackgroundChainCount = BR.ReadInt32();
            ListBackgroundLink = new List<AnimationBackgroundLink>(BackgroundChainCount);
            for (int B = 0; B < BackgroundChainCount; ++B)
            {
                ListBackgroundLink.Add(new AnimationBackgroundLink(Content, BR));
            }

            FS.Close();
            BR.Close();
        }

        public AnimationBackgroundObject2D(ContentManager Content, BinaryReader BR)
        {
            BackgroundPath = BR.ReadString();
            if (!string.IsNullOrEmpty(BackgroundPath))
            {
                sprBackground = Content.Load<Texture2D>("Animations/Background Sprites/" + BackgroundPath);
            }

            int BackgroundChainCount = BR.ReadInt32();
            ListBackgroundLink = new List<AnimationBackgroundLink>(BackgroundChainCount);
            for (int B = 0; B < BackgroundChainCount; ++B)
            {
                ListBackgroundLink.Add(new AnimationBackgroundLink(Content, BR));
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(BackgroundPath);

            BW.Write(ListBackgroundLink.Count);

            foreach (AnimationBackgroundLink ActiveLink in ListBackgroundLink)
            {
                ActiveLink.Save(BW);
            }
        }
    }

    public class AnimationBackgroundLink
    {
        public string Path;
        public Texture2D sprBackground;
        public Vector2 AnchorPointParent;
        public Vector2 AnchorPointSelf;
        private Vector2 _Scale;
        private float _ExtraDepth;
        private bool _UseParallaxScrolling;
        private bool _ScaleToLeft;
        private bool _ScaleToRight;
        private Color _Color;

        public AnimationBackgroundLink(string Path, ContentManager Content)
        {
            this.Path = Path;
            sprBackground = Content.Load<Texture2D>("Animations/Background Sprites/" + Path);

            AnchorPointParent = Vector2.Zero;
            AnchorPointSelf = Vector2.Zero;
            _Scale = Vector2.One;
            _ExtraDepth = 0f;
            _UseParallaxScrolling = true;

            _ScaleToLeft = false;
            _ScaleToRight = false;
            _Color = Color.White;
        }

        public AnimationBackgroundLink(ContentManager Content, BinaryReader BR)
        {
            Path = BR.ReadString();
            sprBackground = Content.Load<Texture2D>("Animations/Background Sprites/" + Path);
            AnchorPointParent = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            AnchorPointSelf = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            _Scale = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            _ExtraDepth = BR.ReadSingle();
            _UseParallaxScrolling = BR.ReadBoolean();

            _ScaleToLeft = BR.ReadBoolean();
            _ScaleToRight = BR.ReadBoolean();
            _Color = new Color(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), BR.ReadByte());
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Path);
            BW.Write(AnchorPointParent.X);
            BW.Write(AnchorPointParent.Y);
            BW.Write(AnchorPointSelf.X);
            BW.Write(AnchorPointSelf.Y);
            BW.Write(_Scale.X);
            BW.Write(_Scale.Y);
            BW.Write(_ExtraDepth);
            BW.Write(_UseParallaxScrolling);

            BW.Write(_ScaleToLeft);
            BW.Write(_ScaleToRight);
            BW.Write(_Color.R);
            BW.Write(_Color.G);
            BW.Write(_Color.B);
            BW.Write(_Color.A);
        }

        public void Draw(CustomSpriteBatch g, Vector2 ParentPos, float ParentDepth, int ScreenWidth, int ScreenHeight)
        {
            Vector2 AnchorPosition = ParentPos + AnchorPointParent;
            float FinalDepth = ParentDepth + _ExtraDepth;
            Vector2 FinalAnchorPointSelf = AnchorPointSelf;

            SpriteEffects FlipEffect = SpriteEffects.None;
            int CenterX = ScreenWidth / 2;
            bool IsVisible = true;
            Vector2 FinalScale = Scale;

            if (_UseParallaxScrolling)
                AnchorPosition.X += ((AnchorPosition.X - CenterX) * (-_ExtraDepth));

            if (Scale.X < 0)
            {
                FlipEffect = SpriteEffects.FlipHorizontally;
                FinalScale.X = -FinalScale.X;
                FinalAnchorPointSelf.X = sprBackground.Width - FinalAnchorPointSelf.X;
            }

            if (_ScaleToLeft)
            {
                if (AnchorPosition.X > CenterX)
                {
                    float PercentVisible = (AnchorPosition.X - CenterX) / CenterX;
                    FinalScale.X *= PercentVisible;
                }
                else
                {
                    IsVisible = false;
                }
            }
            if (_ScaleToRight)
            {
                if (AnchorPosition.X < CenterX)
                {
                    float PercentVisible = (CenterX - AnchorPosition.X) / CenterX;
                    FinalScale.X *= PercentVisible;
                }
                else
                {
                    IsVisible = false;
                }
            }

            if (IsVisible)
            {
                g.Draw(sprBackground, AnchorPosition, null, _Color, 0f, FinalAnchorPointSelf, FinalScale, FlipEffect, FinalDepth);
            }
        }

        public override string ToString()
        {
            return Path;
        }

        #region Properties

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public bool ScaleToLeft
        {
            get
            {
                return _ScaleToLeft;
            }
            set
            {
                _ScaleToLeft = value;
            }
        }

        [CategoryAttribute("Background Attributes"),
        DescriptionAttribute(".")]
        public bool ScaleToRight
        {
            get
            {
                return _ScaleToRight;
            }
            set
            {
                _ScaleToRight = value;
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
        public float ExtraDepth
        {
            get
            {
                return _ExtraDepth;
            }
            set
            {
                _ExtraDepth = value;
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

        #endregion
    }
}
