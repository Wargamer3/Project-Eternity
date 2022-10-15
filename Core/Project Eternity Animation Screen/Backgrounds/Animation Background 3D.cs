using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimationBackground3D : AnimationBackground
    {
        public interface TemporaryBackground
        {
            void Move(Vector3 Movement);
        }

        public class TemporaryBackgroundRotatedObject : TemporaryBackground
        {
            protected int Index;
            protected BillboardSystem ActiveParticleSystem;

            public TemporaryBackgroundRotatedObject(BillboardSystem ActiveParticleSystem, int Index)
            {
                this.Index = Index;
                this.ActiveParticleSystem = ActiveParticleSystem;
            }

            public void Move(Vector3 Movement)
            {
                Position += Movement;
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector2 Size
            {
                get
                {
                    return ActiveParticleSystem.Parameters["Size"].GetValueVector2();
                }
                set
                {
                    ActiveParticleSystem.Parameters["Size"].SetValue(value);
                }
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector3 Position
            {
                get
                {
                    return ActiveParticleSystem.ArrayParticles[Index * 4].Position;
                }
                set
                {
                    ActiveParticleSystem.ArrayParticles[Index * 4].Position = value;
                    ActiveParticleSystem.ArrayParticles[Index * 4 + 1].Position = value;
                    ActiveParticleSystem.ArrayParticles[Index * 4 + 2].Position = value;
                    ActiveParticleSystem.ArrayParticles[Index * 4 + 3].Position = value;

                    ActiveParticleSystem.MoveParticle(Index, Vector3.Zero);
                }
            }
        }

        public class TemporaryBackgroundPolygonObject : TemporaryBackground
        {
            protected int Index;
            protected BillboardSystem ActiveParticleSystem;
            private Vector3 _Rotation;
            private Vector3 _Size;
            private Vector3 Center;
            private Vector3[] ArrayOriginalVector;

            public TemporaryBackgroundPolygonObject(BillboardSystem ActiveParticleSystem, int Index)
            {
                this.Index = Index;
                this.ActiveParticleSystem = ActiveParticleSystem;
                _Rotation = Vector3.Zero;
                _Size = Vector3.One;
                Center = Position;

                ArrayOriginalVector = new Vector3[4];
                for (int V = 0; V < 4; V++)
                {
                    ArrayOriginalVector[V] = ActiveParticleSystem.ArrayParticles[Index * 4 + V].Position;
                }
            }

            public void Move(Vector3 Movement)
            {
                Position += Movement;
            }

            private Vector3 this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0:
                            return Vector1;

                        case 1:
                            return Vector2;

                        case 2:
                            return Vector3;

                        case 3:
                            return Vector4;

                        default:
                            return Vector3.Zero;
                    }
                }
                set
                {
                    ActiveParticleSystem.ArrayParticles[Index * 4 + i].Position = value;
                    ActiveParticleSystem.MoveParticle(Index, Vector3.Zero);
                }
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector3 Position
            {
                get
                {
                    Vector2 ParticleSize = ActiveParticleSystem.Parameters["Size"].GetValueVector2();
                    Vector3 CenterPosition = new Vector3();

                    for (int i = 0; i < 4; i++)
                    {
                        CenterPosition += ActiveParticleSystem.ArrayParticles[Index * 4 + i].Position;
                    }
                    CenterPosition /= 4;
                    CenterPosition.Y += ParticleSize.Y;

                    return CenterPosition;
                }
                set
                {
                    ActiveParticleSystem.MoveParticle(Index, value - Position);
                    Center = Position;

                    for (int V = 0; V < 4; V++)
                    {
                        ArrayOriginalVector[V] = ActiveParticleSystem.ArrayParticles[Index * 4 + V].Position;
                    }
                }
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector3 Rotation
            {
                get
                {
                    return _Rotation;
                }
                set
                {
                    _Rotation = value;
                    Vector3 Axis = value * 0.0174533f;
                    
                    for (int V = 3; V >=0; --V)
                    {
                        Matrix TranslationMatrix1 = Matrix.CreateTranslation(-Center);
                        Matrix RotationMatrix = Matrix.CreateRotationX(Axis.X) * Matrix.CreateRotationY(Axis.Y) * Matrix.CreateRotationZ(Axis.Z);
                        Matrix TranslationMatrix2 = Matrix.CreateTranslation(Center);

                        this[V] = Vector3.Transform(ArrayOriginalVector[V], TranslationMatrix1 * RotationMatrix * TranslationMatrix2);
                    }
                }
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector3 Size
            {
                get
                {
                    return _Size;
                }
                set
                {
                    _Size = value;

                    for (int V = 3; V >= 0; --V)
                    {
                        Matrix TranslationMatrix1 = Matrix.CreateTranslation(-Center);
                        Matrix ScaleMatrix = Matrix.CreateScale(value);
                        Matrix TranslationMatrix2 = Matrix.CreateTranslation(Center);

                        this[V] = Vector3.Transform(ArrayOriginalVector[V], TranslationMatrix1 * ScaleMatrix * TranslationMatrix2);
                    }
                }
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector3 Vector1
            {
                get
                {
                    return ActiveParticleSystem.ArrayParticles[Index * 4].Position;
                }
                set
                {
                    ActiveParticleSystem.ArrayParticles[Index * 4].Position = value;
                    ActiveParticleSystem.MoveParticle(Index, Vector3.Zero);
                    ArrayOriginalVector[0] = value;
                }
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector3 Vector2
            {
                get
                {
                    return ActiveParticleSystem.ArrayParticles[Index * 4 + 1].Position;
                }
                set
                {
                    ActiveParticleSystem.ArrayParticles[Index * 4 + 1].Position = value;
                    ActiveParticleSystem.MoveParticle(Index, Vector3.Zero);
                    ArrayOriginalVector[1] = value;
                }
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector3 Vector3
            {
                get
                {
                    return ActiveParticleSystem.ArrayParticles[Index * 4 + 2].Position;
                }
                set
                {
                    ActiveParticleSystem.ArrayParticles[Index * 4 + 2].Position = value;
                    ActiveParticleSystem.MoveParticle(Index, Vector3.Zero);
                    ArrayOriginalVector[2] = value;
                }
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector3 Vector4
            {
                get
                {
                    return ActiveParticleSystem.ArrayParticles[Index * 4 + 3].Position;
                }
                set
                {
                    ActiveParticleSystem.ArrayParticles[Index * 4 + 3].Position = value;
                    ActiveParticleSystem.MoveParticle(Index, Vector3.Zero);
                    ArrayOriginalVector[3] = value;
                }
            }
        }

        public class TemporaryBackgroundModelObject : TemporaryBackground
        {
            AnimationBackground3DObject BackgroundModel;

            public TemporaryBackgroundModelObject(AnimationBackground3DObject BackgroundModel)
            {
                this.BackgroundModel = BackgroundModel;
            }

            public void Move(Vector3 Movement)
            {
                Position += Movement;
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector3 Position
            {
                get
                {
                    return BackgroundModel.Position;
                }
                set
                {
                    BackgroundModel.Position = value;
                }
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector3 Rotation
            {
                get
                {
                    return new Vector3(MathHelper.ToDegrees(BackgroundModel.Rotation.X), MathHelper.ToDegrees(BackgroundModel.Rotation.Y), MathHelper.ToDegrees(BackgroundModel.Rotation.Z));
                }
                set
                {
                    BackgroundModel.Rotation = new Vector3(MathHelper.ToRadians(value.X), MathHelper.ToRadians(value.Y), MathHelper.ToRadians(value.Z));
                }
            }

            [CategoryAttribute("Object Attributes"),
            DescriptionAttribute(".")]
            public Vector3 Size
            {
                get
                {
                    return BackgroundModel.Size;
                }
                set
                {
                    BackgroundModel.Size = value;
                }
            }
        }

        public struct IndexedLines
        {
            private VertexPositionColor[] ArrayVertex;
            private short[] ArrayIndices;

            public IndexedLines(VertexPositionColor[] ArrayVertex, short[] ArrayIndices)
            {
                this.ArrayVertex = ArrayVertex;
                this.ArrayIndices = ArrayIndices;
            }

            internal void Draw(CustomSpriteBatch g)
            {
                g.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,
                    ArrayVertex,
                    0,  // vertex buffer offset to add to each element of the index buffer
                    ArrayVertex.Length,  // number of vertices in pointList
                    ArrayIndices,  // the index buffer
                    0,  // first index element to read
                    ArrayIndices.Length / 2 - 1   // number of primitives to draw
                );
            }
        }

        public enum WorldTypes : byte { Infinite, Limited, Looped }

        public List<AnimationBackground3DBase> ListBackground;

        private float aspectRatio = 600f / 400f;

        public Matrix View;
        public Matrix Projection;
        public WorldTypes WorldType;
        public int WorldWidth;
        public int WorldDepth;
        private IndexedLines BackgroundGrid;
        private IndexedLines Bounds;
        private BasicEffect IndexedLinesEffect;
        public float Ceiling = 100000f;

        public AnimationBackground3D(ContentManager Content, GraphicsDevice g)
            : this("", Content, g)
        {
            ListBackground = new List<AnimationBackground3DBase>();
        }

        public AnimationBackground3D(string AnimationBackgroundPath, ContentManager Content, GraphicsDevice g)
            : base("3D", AnimationBackgroundPath, Content, g)
        {
            IndexedLinesEffect = new BasicEffect(g);

            FileStream FS = new FileStream("Content/Animations/" + AnimationBackgroundPath + ".peab", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            WorldType = (WorldTypes)BR.ReadByte();
            WorldWidth = BR.ReadInt32();
            WorldDepth = BR.ReadInt32();

            DefaultCameraPosition = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            DefaultCameraRotation = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            ResetCamera();

            int ListBackgroundObjectSystemCount = BR.ReadInt32();
            ListBackground = new List<AnimationBackground3DBase>(ListBackgroundObjectSystemCount);
            for (int B = 0; B < ListBackgroundObjectSystemCount; ++B)
            {
                ListBackground.Add(AnimationBackground3DBase.LoadFromFile(Content, BR, g));
            }

            FS.Close();
            BR.Close();

            int GridWidth = 5;
            int GridDepth = 5;
            VertexPositionColor[] ArrayBackgroundGridVertex = new VertexPositionColor[GridWidth * 8];
            // Initialize an array of indices of type short.
            short[] ArrayBackgroundGridIndices = new short[(ArrayBackgroundGridVertex.Length * 2) - 2];

            int LineLength = 100;
            int Index = 0;
            for (int X = -GridWidth; X < GridWidth; ++X)
            {
                ArrayBackgroundGridVertex[Index * 2] = new VertexPositionColor(
                    new Vector3(X * LineLength, 0, -LineLength * GridDepth - LineLength / 2), Color.White);
                ArrayBackgroundGridVertex[Index * 2 + 1] = new VertexPositionColor(
                    new Vector3(X * LineLength, 0, LineLength * GridDepth - LineLength / 2), Color.White);

                ArrayBackgroundGridIndices[Index * 2] = (short)(Index * 2);
                ArrayBackgroundGridIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

                ++Index;
            }
            for (int Z = -GridWidth; Z < GridWidth; ++Z)
            {
                ArrayBackgroundGridVertex[Index * 2] = new VertexPositionColor(
                    new Vector3(-LineLength * GridWidth - LineLength / 2, 0, Z * LineLength), Color.White);
                ArrayBackgroundGridVertex[Index * 2 + 1] = new VertexPositionColor(
                    new Vector3(LineLength * GridWidth - LineLength / 2, 0, Z * LineLength), Color.White);

                ArrayBackgroundGridIndices[Index * 2] = (short)(Index * 2);
                ArrayBackgroundGridIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

                ++Index;
            }

            BackgroundGrid = new IndexedLines(ArrayBackgroundGridVertex, ArrayBackgroundGridIndices);

            MoveSpeed = Vector3.Zero;

            InitBounds();
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write((byte)WorldType);
            BW.Write(WorldWidth);
            BW.Write(WorldDepth);
            BW.Write(DefaultCameraPosition.X);
            BW.Write(DefaultCameraPosition.Y);
            BW.Write(DefaultCameraPosition.Z);
            BW.Write(DefaultCameraRotation.X);
            BW.Write(DefaultCameraRotation.Y);
            BW.Write(DefaultCameraRotation.Z);

            BW.Write(ListBackground.Count);
            for (int B = 0; B < ListBackground.Count; ++B)
            {
                ListBackground[B].Save(BW);
            }
        }

        public override void Update(GameTime gameTime)
        {
            MoveCamera(1, MoveSpeed);

            for (int B = ListBackground.Count - 1; B >= 0; --B)
            {
                ListBackground[B].Update(gameTime);
            }
        }

        private void InitBounds()
        {
            VertexPositionColor[] ArrayBoundsVertex = new VertexPositionColor[8];
            ArrayBoundsVertex[0] = new VertexPositionColor(new Vector3(-WorldWidth, 0, WorldDepth), Color.White);
            ArrayBoundsVertex[1] = new VertexPositionColor(new Vector3(WorldWidth, 0, WorldDepth), Color.White);
            ArrayBoundsVertex[2] = new VertexPositionColor(new Vector3(WorldWidth, 0, -WorldDepth), Color.White);
            ArrayBoundsVertex[3] = new VertexPositionColor(new Vector3(-WorldWidth, 0, -WorldDepth), Color.White);

            ArrayBoundsVertex[4] = new VertexPositionColor(new Vector3(-WorldWidth, Ceiling, WorldDepth), Color.White);
            ArrayBoundsVertex[5] = new VertexPositionColor(new Vector3(WorldWidth, Ceiling, WorldDepth), Color.White);
            ArrayBoundsVertex[6] = new VertexPositionColor(new Vector3(WorldWidth, Ceiling, -WorldDepth), Color.White);
            ArrayBoundsVertex[7] = new VertexPositionColor(new Vector3(-WorldWidth, Ceiling, -WorldDepth), Color.White);

            short[] ArrayBoundsIndices = new short[24];

            ArrayBoundsIndices[0] = 0;
            ArrayBoundsIndices[1] = 1;
            ArrayBoundsIndices[2] = 1;
            ArrayBoundsIndices[3] = 2;
            ArrayBoundsIndices[4] = 2;
            ArrayBoundsIndices[5] = 3;
            ArrayBoundsIndices[6] = 3;
            ArrayBoundsIndices[7] = 0;

            ArrayBoundsIndices[8] = 4;
            ArrayBoundsIndices[9] = 5;
            ArrayBoundsIndices[10] = 5;
            ArrayBoundsIndices[11] = 6;
            ArrayBoundsIndices[12] = 6;
            ArrayBoundsIndices[13] = 7;
            ArrayBoundsIndices[14] = 7;
            ArrayBoundsIndices[15] = 4;

            ArrayBoundsIndices[16] = 0;
            ArrayBoundsIndices[17] = 4;
            ArrayBoundsIndices[18] = 1;
            ArrayBoundsIndices[19] = 5;
            ArrayBoundsIndices[20] = 2;
            ArrayBoundsIndices[21] = 6;
            ArrayBoundsIndices[22] = 3;
            ArrayBoundsIndices[23] = 7;

            Bounds = new IndexedLines(ArrayBoundsVertex, ArrayBoundsIndices);
        }

        public void MoveCamera(float Speed, Vector3 Direction)
        {
            CameraPosition += Speed * Direction;

            if (WorldType == WorldTypes.Limited)
            {
                if (CameraPosition.X > WorldWidth)
                    CameraPosition.X = WorldWidth;
                if (CameraPosition.X < -WorldWidth)
                    CameraPosition.X = -WorldWidth;

                if (CameraPosition.Z > WorldDepth)
                    CameraPosition.Z = WorldDepth;
                if (CameraPosition.Z < -WorldDepth)
                    CameraPosition.Z = -WorldDepth;
            }
            else if (WorldType == WorldTypes.Looped)
            {
                if (CameraPosition.X > WorldWidth)
                    CameraPosition.X -= WorldWidth;
                if (CameraPosition.X < -WorldWidth)
                    CameraPosition.X += WorldWidth;

                if (CameraPosition.Z > WorldDepth)
                    CameraPosition.Z -= WorldDepth;
                if (CameraPosition.Z < -WorldDepth)
                    CameraPosition.Z += WorldDepth;
            }
        }

        public override void Draw(CustomSpriteBatch g, int ScreenWidth, int ScreenHeight)
        {
            CameraRotation.Forward.Normalize();
            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            CameraRotation.Up.Normalize();
            CameraRotation.Right.Normalize();

            CameraRotation *= Matrix.CreateFromAxisAngle(CameraRotation.Right, Pitch);
            CameraRotation *= Matrix.CreateFromAxisAngle(new Vector3(0, -1, 0), Yaw);
            //CameraRotation *= Matrix.CreateFromAxisAngle(CameraRotation.Up, Yaw);
            CameraRotation *= Matrix.CreateFromAxisAngle(CameraRotation.Forward, Roll);

            Pitch = Yaw = Roll = 0;

            Vector3 target = CameraPosition + CameraRotation.Forward;

            View = Matrix.CreateLookAt(CameraPosition, target, CameraRotation.Up);

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);

            foreach (AnimationBackground3DBase ActiveBillboardSystem in ListBackground)
            {
                ActiveBillboardSystem.Draw(g, View, Projection, ScreenWidth, ScreenHeight);
            }

            if (IsEditor)
            {
                IndexedLinesEffect.View = View;
                IndexedLinesEffect.Projection = Projection;
                IndexedLinesEffect.World = Matrix.Identity;
                IndexedLinesEffect.DiffuseColor = new Vector3(0, 0, 0);

                foreach (EffectPass pass in IndexedLinesEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    BackgroundGrid.Draw(g);
                }
                IndexedLinesEffect.DiffuseColor = new Vector3(1f, 0, 0);
                // Activate the particle effect.
                foreach (EffectPass pass in IndexedLinesEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    Bounds.Draw(g);
                }
            }
        }
    }
}
