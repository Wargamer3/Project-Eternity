using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimationBackground3DObject
    {
        public readonly AnimatedModel Model;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Size;

        public AnimationBackground3DObject(AnimatedModel Model)
        {
            this.Model = Model;

            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Size = Vector3.One;
        }
    }
    public class AnimationBackground3DModel : AnimationBackground3DBase
    {
        public const string BackgroundTypeName = "Model";

        string FilePath;
        private List<AnimationBackground3DObject> ListBackgroundModel;

        public AnimationBackground3DModel(AnimatedModel BackgroundModel)
            : base(BackgroundTypeName)
        {
            ListBackgroundModel = new List<AnimationBackground3DObject>();
            ListBackgroundModel.Add(new AnimationBackground3DObject(BackgroundModel));
            FilePath = BackgroundModel.FilePath;
        }

        public AnimationBackground3DModel(ContentManager Content, BinaryReader BR)
            : base(BackgroundTypeName)
        {
            FilePath = BR.ReadString();
            int ListBackgroundModelCount = BR.ReadInt32();
            ListBackgroundModel = new List<AnimationBackground3DObject>(ListBackgroundModelCount);

            for (int B = 0; B < ListBackgroundModelCount; ++B)
            {
                AnimatedModel NewModel = new AnimatedModel(FilePath);
                NewModel.LoadContent(Content);
                AnimationBackground3DObject NewObject = new AnimationBackground3DObject(NewModel);
                NewObject.Position = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
                NewObject.Rotation = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
                NewObject.Size = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
                ListBackgroundModel.Add(NewObject);
            }
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(FilePath);

            BW.Write(ListBackgroundModel.Count);
            for (int B = 0; B < ListBackgroundModel.Count; ++B)
            {
                BW.Write(ListBackgroundModel[B].Position.X);
                BW.Write(ListBackgroundModel[B].Position.Y);
                BW.Write(ListBackgroundModel[B].Position.Z);
                BW.Write(ListBackgroundModel[B].Rotation.X);
                BW.Write(ListBackgroundModel[B].Rotation.Y);
                BW.Write(ListBackgroundModel[B].Rotation.Z);
                BW.Write(ListBackgroundModel[B].Size.X);
                BW.Write(ListBackgroundModel[B].Size.Y);
                BW.Write(ListBackgroundModel[B].Size.Z);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (AnimationBackground3DObject ActiveModel in ListBackgroundModel)
            {
                ActiveModel.Model.Update(gameTime);
            }
        }

        public override void AddItem(Vector3 Position)
        {
            //ListBackgroundModel.Add(Position);
        }

        public override void RemoveItem(int Index)
        {
            ListBackgroundModel.RemoveAt(Index);
        }

        public override List<string> GetChild()
        {
            List<string> ListChild = new List<string>();

            foreach (AnimationBackground3DObject ActiveModel in ListBackgroundModel)
            {
                ListChild.Add("New Model");
            }

            return ListChild;
        }

        public override float GetDistance(float MouseX, float MouseY, Matrix View, Matrix Projection, Viewport Viewport)
        {
            return 0;
        }

        public override object GetEditableObject(int Index)
        {
            return new AnimationBackground3D.TemporaryBackgroundModelObject(ListBackgroundModel[Index]);
        }

        public override void Draw(CustomSpriteBatch g, Matrix View, Matrix Projection, int ScreenWidth, int ScreenHeight)
        {
            foreach (AnimationBackground3DObject ActiveModel in ListBackgroundModel)
            {
                ActiveModel.Model.Draw(View, Projection, Matrix.CreateScale(ActiveModel.Size.X, ActiveModel.Size.Y, ActiveModel.Size.Z)
                    * Matrix.CreateRotationX(ActiveModel.Rotation.X) * Matrix.CreateRotationY(ActiveModel.Rotation.Y) * Matrix.CreateRotationZ(ActiveModel.Rotation.Z)
                    * Matrix.CreateTranslation(ActiveModel.Position));
            }
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}
