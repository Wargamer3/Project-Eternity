using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimationBackground2D : AnimationBackground
    {
        public List<AnimationBackground2DBase> ListBackground;

        public AnimationBackground2D(ContentManager Content, GraphicsDevice g)
            : this("", Content, g)
        {
            ListBackground = new List<AnimationBackground2DBase>();
        }

        public AnimationBackground2D(string AnimationBackgroundPath, ContentManager Content, GraphicsDevice g)
            : base("2D", AnimationBackgroundPath, Content, g)
        {
            FileStream FS = new FileStream("Content/Animations/" + AnimationBackgroundPath + ".peab", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            int ListBackgroundCount = BR.ReadInt32();

            ListBackground = new List<AnimationBackground2DBase>(ListBackgroundCount);
            for (int B = 0; B < ListBackgroundCount; ++B)
            {
                ListBackground.Add(AnimationBackground2DBase.LoadFromFile(Content, BR));
            }

            FS.Close();
            BR.Close();
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(ListBackground.Count);
            for (int B = 0; B< ListBackground.Count; ++B)
            {
                ListBackground[B].Save(BW);
            }
        }

        public void MoveCamera(Vector3 CameraMovement)
        {
            CameraPosition += CameraMovement;
        }

        public override void Update(GameTime gameTime)
        {
            CameraPosition += MoveSpeed;

            for (int B = ListBackground.Count - 1; B >= 0; --B)
            {
                ListBackground[B].Move(gameTime);
            }
        }

        public override void Draw(CustomSpriteBatch g, int ScreenWidth, int ScreenHeight)
        {
            g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            for (int B = ListBackground.Count - 1; B >= 0; --B)
            {
                ListBackground[B].Draw(g, CameraPosition.X, CameraPosition.Y, ScreenWidth, ScreenHeight);
            }

            g.End();
        }

        public override void Draw3D(Camera3D Camera, Matrix World)
        {
        }
    }
}
