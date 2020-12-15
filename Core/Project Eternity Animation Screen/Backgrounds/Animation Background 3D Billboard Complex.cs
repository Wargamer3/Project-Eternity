using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimationBackground3DBillboardComplex : AnimationBackground3DBase
    {
        public const string BackgroundTypeName = "Object";

        public string Path;
        public AnimationBackgroundObject2D BackgroundChain;

        public AnimationBackground3DBillboardComplex(ContentManager Content, string Path)
            : base(BackgroundTypeName)
        {
            this.Path = Path;
            BackgroundChain = new AnimationBackgroundObject2D(Content, Path);
        }

        public AnimationBackground3DBillboardComplex(ContentManager Content, BinaryReader BR)
            : base(BackgroundTypeName)
        {
            Path = BR.ReadString();
            BackgroundChain = new AnimationBackgroundObject2D(Content, Path);
        }

        public AnimationBackground3DBillboardComplex(AnimationBackgroundObject2D BackgroundChain)
            : base(BackgroundTypeName)
        {
            this.BackgroundChain = BackgroundChain;
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override List<string> GetChild()
        {
            List<string> ListChild = new List<string>();


            return ListChild;
        }

        public override float GetDistance(float MouseX, float MouseY, Matrix View, Matrix Projection, Viewport Viewport)
        {
            return 0f;
        }

        public override object GetEditableObject(int Index)
        {
            return this;
        }

        public override void Draw(CustomSpriteBatch g, Matrix View, Matrix Projection, int ScreenWidth, int ScreenHeight)
        {
            //g.GraphicsDevice.Viewport.Unproject(MyVector3Location, Projection, View, World);
        }

        public override string ToString()
        {
            return "";
        }
    }
}
