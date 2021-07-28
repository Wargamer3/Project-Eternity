using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public abstract class AnimationBackground3DBase
    {
        public readonly string BackgroundType;

        protected AnimationBackground3DBase(string BackgroundType)
        {
            this.BackgroundType = BackgroundType;
        }

        public static AnimationBackground3DBase LoadFromFile(ContentManager Content, BinaryReader BR, GraphicsDevice g)
        {
            string BackgroundType = BR.ReadString();

            if (BackgroundType == AnimationBackground3DBillboard.BackgroundTypeName)
            {
                return new AnimationBackground3DBillboard(Content, BR, g);
            }
            else if (BackgroundType == AnimationBackground3DBillboardComplex.BackgroundTypeName)
            {
                return new AnimationBackground3DBillboardComplex(Content, BR);
            }

            return null;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(BackgroundType);

            DoSave(BW);
        }

        protected abstract void DoSave(BinaryWriter BW);

        public abstract void Update(GameTime gameTime);

        public abstract List<string> GetChild();

        public abstract float GetDistance(float MouseX, float MouseY, Matrix View, Matrix Projection, Viewport Viewport);

        public abstract object GetEditableObject(int Index);

        public abstract void Draw(CustomSpriteBatch g, Matrix View, Matrix Projection, int ScreenWidth, int ScreenHeight);
    }
}
