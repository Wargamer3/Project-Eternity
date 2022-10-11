using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimationBackground3DBillboard : AnimationBackground3DBase
    {
        public const string BackgroundTypeName = "Billboard";
        private const int MaxRepeat = 5;

        private BillboardSystem ActiveBillboardSystem;

        public AnimationBackground3DBillboard(BillboardSystem ActiveBillboardSystem)
            : base(BackgroundTypeName)
        {
            this.ActiveBillboardSystem = ActiveBillboardSystem;
        }

        public AnimationBackground3DBillboard(ContentManager Content, BinaryReader BR, GraphicsDevice g)
            : base(BackgroundTypeName)
        {
            ActiveBillboardSystem = new BillboardSystem(BR, 20000, Content, g);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(ActiveBillboardSystem.RotateTowardCamera);
            BW.Write(ActiveBillboardSystem.TextureName);
            BW.Write(ActiveBillboardSystem.NumberOfImages);
            BW.Write(ActiveBillboardSystem.FirstFreeParticle);
            BW.Write(ActiveBillboardSystem.RepeatX);
            BW.Write(ActiveBillboardSystem.RepeatY);
            BW.Write(ActiveBillboardSystem.RepeatZ);
            BW.Write(ActiveBillboardSystem.RepeatOffset.X);
            BW.Write(ActiveBillboardSystem.RepeatOffset.Y);
            BW.Write(ActiveBillboardSystem.RepeatOffset.Z);
            BW.Write(ActiveBillboardSystem.Speed.X);
            BW.Write(ActiveBillboardSystem.Speed.Y);
            BW.Write(ActiveBillboardSystem.Speed.Z);

            if (ActiveBillboardSystem.RotateTowardCamera)
            {
                BW.Write(ActiveBillboardSystem.Parameters["Size"].GetValueVector2().X);
                BW.Write(ActiveBillboardSystem.Parameters["Size"].GetValueVector2().Y);

                for (int P = 0; P < ActiveBillboardSystem.FirstFreeParticle; ++P)
                {
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P * 4].Position.X);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P * 4].Position.Y);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P * 4].Position.Z);
                }
            }
            else
            {
                for (int P = 0; P < ActiveBillboardSystem.FirstFreeParticle; ++P)
                {
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P].Position.X);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P].Position.Y);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P].Position.Z);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P + 1].Position.X);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P + 1].Position.Y);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P + 1].Position.Z);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P + 2].Position.X);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P + 2].Position.Y);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P + 2].Position.Z);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P + 3].Position.X);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P + 3].Position.Y);
                    BW.Write(ActiveBillboardSystem.ArrayParticles[P + 3].Position.Z);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {

            ActiveBillboardSystem.Update(gameTime);
        }

        public override void AddItem(Vector3 Position)
        {
            ActiveBillboardSystem.AddParticle(Position);
        }

        public override void RemoveItem(int Index)
        {
            ActiveBillboardSystem.RemoveParticle(Index);
        }

        public override List<string> GetChild()
        {
            List<string> ListChild = new List<string>();

            for (int P = 0; P < ActiveBillboardSystem.FirstFreeParticle; P += 4)
            {
                ListChild.Add("Prop " + (P / 4 + 1));
            }

            return ListChild;
        }

        public override float GetDistance(float MouseX, float MouseY, Matrix View, Matrix Projection, Viewport Viewport)
        {
            float MinDistanceToTriangle = float.MaxValue;
            Vector3 NearSource = new Vector3(MouseX, MouseY, 0f);
            Vector3 FarSource = new Vector3(MouseX, MouseY, 1f);
            Matrix World = Matrix.CreateTranslation(0, 0, 0);

            if (ActiveBillboardSystem.RotateTowardCamera)
            {
                Vector2 Size = ActiveBillboardSystem.Parameters["Size"].GetValueVector2();
                Vector2 ViewportScale = new Vector2(0.5f / Viewport.AspectRatio, -0.5f);

                for (int P = 0; P < ActiveBillboardSystem.FirstFreeParticle; P += 4)
                {
                    Vector3 Position = ActiveBillboardSystem.ArrayParticles[P].Position;
                    Vector2 UV = ActiveBillboardSystem.ArrayParticles[P].UV;
                    Vector4 ParticlePosition = Vector4.Transform(Vector4.Transform(new Vector4(Position, 1),
                                                                View),
                                                                Projection);

                    Vector2 ParticleSize = Size * Projection.M11;
                    float Angle = 0;
                    Position = new Vector3(new Vector2(Position.X, Position.Y) + Vector2.Transform(UV, Matrix.CreateRotationZ(Angle)) * ParticleSize * ViewportScale, Position.Z);

                    Position = Viewport.Project(Position, Projection, View, World);

                    if (MouseX >= Position.X)
                    {
                        float Distance = (Position - NearSource).Length();
                        if (Distance < MinDistanceToTriangle)
                        {
                            MinDistanceToTriangle = Distance;
                        }
                    }
                }
            }
            else
            {
                Vector3 NearPoint = Viewport.Unproject(NearSource,
                    Projection, View, World);

                Vector3 FarPoint = Viewport.Unproject(FarSource,
                    Projection, View, World);

                // Create a ray from the near clip plane to the far clip plane.
                Vector3 direction = FarPoint - NearPoint;
                direction.Normalize();
                Ray MouseRay = new Ray(NearPoint, direction);

                for (int P = 0; P < ActiveBillboardSystem.FirstFreeParticle; P += 4)
                {
                    var DistanceToTriangle = RayIntersectsTriangle(MouseRay,
                    ActiveBillboardSystem.ArrayParticles[0].Position,
                    ActiveBillboardSystem.ArrayParticles[1].Position,
                    ActiveBillboardSystem.ArrayParticles[2].Position);

                    if (DistanceToTriangle.HasValue)
                    {
                        if (DistanceToTriangle.Value < MinDistanceToTriangle)
                        {
                            MinDistanceToTriangle = DistanceToTriangle.Value;
                        }
                    }
                    else
                    {
                        DistanceToTriangle = RayIntersectsTriangle(MouseRay,
                            ActiveBillboardSystem.ArrayParticles[0].Position,
                            ActiveBillboardSystem.ArrayParticles[2].Position,
                            ActiveBillboardSystem.ArrayParticles[3].Position);

                        if (DistanceToTriangle.HasValue)
                        {
                            if (DistanceToTriangle.Value < MinDistanceToTriangle)
                            {
                                MinDistanceToTriangle = DistanceToTriangle.Value;
                            }
                        }
                    }
                }
            }

            return MinDistanceToTriangle;
        }

        //https://github.com/jakepetroules/jakes-3d-mmo/blob/master/Engine/Picking/Picking.cs
        //Kudos to Jake for understanding how to do that.
        private float? RayIntersectsTriangle(Ray ray, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref vertex2, ref vertex1, out edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                return null;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref vertex1, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                return null;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                return null;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                return null;
            }

            return rayDistance;
        }

        public override object GetEditableObject(int Index)
        {
            if (ActiveBillboardSystem.RotateTowardCamera)
            {
                return new AnimationBackground3D.TemporaryBackgroundRotatedObject(ActiveBillboardSystem, Index);
            }
            else
            {
                return new AnimationBackground3D.TemporaryBackgroundPolygonObject(ActiveBillboardSystem, Index);
            }
        }

        public override void Draw(CustomSpriteBatch g, Matrix View, Matrix Projection, int ScreenWidth, int ScreenHeight)
        {
            int MaxRepeatX = 0;
            if (ActiveBillboardSystem.RepeatX)
                MaxRepeatX = MaxRepeat;

            for (int X = -MaxRepeatX; X <= MaxRepeatX; ++X)
            {
                int MaxRepeatY = 0;
                if (ActiveBillboardSystem.RepeatY)
                    MaxRepeatY = MaxRepeat;

                for (int Y = -MaxRepeatY; Y <= MaxRepeatY; Y++)
                {
                    int MaxRepeatZ = 0;
                    if (ActiveBillboardSystem.RepeatZ)
                        MaxRepeatZ = MaxRepeat;

                    for (int Z = -MaxRepeatZ; Z <= MaxRepeatZ; Z++)
                    {
                        Matrix World = Matrix.CreateTranslation(X * ActiveBillboardSystem.RepeatOffset.X, Y * ActiveBillboardSystem.RepeatOffset.Y, Z * ActiveBillboardSystem.RepeatOffset.Z);

                        ActiveBillboardSystem.SetViewProjection(World * View, Projection);

                        //g.GraphicsDevice.Viewport.Unproject(MyVector3Location, Projection, View, World);
                        ActiveBillboardSystem.Draw(GameScreen.GraphicsDevice);
                    }
                }
            }
        }

        public override string ToString()
        {
            return ActiveBillboardSystem.TextureName.Substring(31);
        }
    }
}
