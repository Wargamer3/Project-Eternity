using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public interface IObject3D
    {
        void Teleport(Vector3 Destination);

        void Rotate(float TotalYaw, float TotalPitch, float TotalRoll);

        void Scale(Vector3 TotalScale);

        void Draw(CustomSpriteBatch g, Matrix View);

        #region Properties

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        Vector3 Position { get; set; }

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        Vector3 Direction { get; set; }

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        Vector3 Size { get; set; }

        #endregion
    }

    public abstract class Object3D : IObject3D
    {
        protected readonly BasicEffect ObjectEffect;
        private PolygonMesh _CollisionBox;
        public PolygonMesh CollisionBox { get { return _CollisionBox; } }

        private Vector3 _Forward;
        private Vector3 _Right;
        private Vector3 _Up;
        public Vector3 Forward { get { return _Forward; } }
        public Vector3 Right { get { return _Right; } }
        public Vector3 Up { get { return _Up; } }
        public Matrix Rotation { get { return RotationMatrix; } }

        private Matrix PositionMatrix;
        private Matrix RotationMatrix;
        private Matrix ScaleMatrix;

        private Vector3 _Position;
        private float TotalYaw, TotalPitch, TotalRoll;
        private Vector3 TotalScale;

        public Object3D()
        {
            _CollisionBox = new PolygonMesh();

            PositionMatrix = Matrix.Identity;
            RotationMatrix = Matrix.Identity;
            ScaleMatrix = Matrix.Identity;

            _Forward = Vector3.Forward;
            _Right = Vector3.Right;
            _Up = Vector3.Up;
            _Position = Vector3.Zero;
            TotalYaw = 0;
            TotalPitch = 0;
            TotalRoll = 0;
            TotalScale = Vector3.One;
        }

        public Object3D(GraphicsDevice g, Matrix Projection)
            : this()
        {
            ObjectEffect = new BasicEffect(g);
            ObjectEffect.World = Matrix.Identity;
            ObjectEffect.Projection = Projection;
        }

        public Object3D(BinaryReader BR, GraphicsDevice g, Matrix Projection)
            : this(g, Projection)
        {
            Position = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            Rotate(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            Scale(new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle()));
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Position.X);
            BW.Write(Position.Y);
            BW.Write(Position.Z);

            BW.Write(TotalYaw);
            BW.Write(TotalPitch);
            BW.Write(TotalRoll);

            BW.Write(TotalScale.X);
            BW.Write(TotalScale.Y);
            BW.Write(TotalScale.Z);
        }

        public void Teleport(Vector3 Destination)
        {
            _Position = Destination;
            PositionMatrix = Matrix.CreateTranslation(Position);
            ReCreateCollisionBox();

            UpdateWorld();
        }

        public void Rotate(float TotalYaw, float TotalPitch, float TotalRoll)
        {
            this.TotalYaw = TotalYaw;
            this.TotalPitch = TotalPitch;
            this.TotalRoll = TotalRoll;

            RotationMatrix = Matrix.CreateFromYawPitchRoll(TotalYaw, TotalPitch, TotalRoll);
            ReCreateCollisionBox();
            _Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            _Right = Vector3.Transform(Vector3.Right, RotationMatrix);
            _Up = Vector3.Transform(Vector3.Up, RotationMatrix);

            UpdateWorld();
        }

        public void Scale(Vector3 TotalScale)
        {
            this.TotalScale = TotalScale;
            ScaleMatrix = Matrix.CreateScale(TotalScale);
            ReCreateCollisionBox();

            UpdateWorld();
        }

        public PolygonMesh.PolygonMeshCollisionResult GetClosestObject(List<Object3D> ListOther, Vector3 Speed)
        {
            PolygonMesh.PolygonMeshCollisionResult FinalCollisionResult = new PolygonMesh.PolygonMeshCollisionResult(Vector3.Zero, -1);
            Object3D FinalLayerPolygon = null;

            foreach (Object3D ActiveObject3D in ListOther)
            {
                PolygonMesh.PolygonMeshCollisionResult CollisionResult = PolygonMesh.PolygonCollisionSAT(CollisionBox, ActiveObject3D.CollisionBox, Speed);

                if (FinalCollisionResult.Distance < 0 || (CollisionResult.Distance >= 0 && CollisionResult.Distance > FinalCollisionResult.Distance))
                {
                    FinalCollisionResult = CollisionResult;
                    FinalLayerPolygon = ActiveObject3D;
                }
            }

            return FinalCollisionResult;
        }

        public bool CollideWith(Object3D Other)
        {
            return PolygonMesh.PolygonCollisionSAT(CollisionBox, Other.CollisionBox, Vector3.Zero).Collided;
        }

        public bool CollideWithMouse(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection, Matrix World)
        {
            Vector3 NearSource = new Vector3(MouseX, MouseY, 0f);
            Vector3 FarSource = new Vector3(MouseX, MouseY, 1f);

            Vector3 NearPoint = ActiveViewport.Unproject(NearSource, Projection, View, World);
            Vector3 FarPoint = ActiveViewport.Unproject(FarSource, Projection, View, World);

            // Create a ray from the near clip plane to the far clip plane.
            Vector3 RayDirection = FarPoint - NearPoint;
            RayDirection.Normalize();

            Matrix RotationMatrix = Matrix.Identity;
            RotationMatrix.Forward = RayDirection;
            RotationMatrix.Right = Vector3.Normalize(Vector3.Cross(RotationMatrix.Forward, Vector3.Up));
            RotationMatrix.Up = Vector3.Cross(RotationMatrix.Right, RotationMatrix.Forward);
            Vector3 Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Vector3 Right = Vector3.Transform(Vector3.Right, RotationMatrix);
            Vector3 Up = Vector3.Transform(Vector3.Up, RotationMatrix);

            float MinRight, MaxRight;
            float MinUp, MaxUp;
            PolygonMesh.ProjectPolygon(Right, CollisionBox, out MinRight, out MaxRight);
            PolygonMesh.ProjectPolygon(Up, CollisionBox, out MinUp, out MaxUp);
            float MouseRight = Vector3.Dot(Right, NearPoint);
            float MouseUp = Vector3.Dot(Up, NearPoint);

            return MouseRight >= MinRight && MouseRight <= MaxRight && MouseUp >= MinUp && MouseUp <= MaxUp;
        }
        
        public void MoveWithMouse(int MouseX, int MouseY, int OldMouseX, int OldMouseY, Viewport ActiveViewport, Matrix View, Matrix Projection, Matrix World)
        {
            Vector3 NearSource = new Vector3(MouseX, MouseY, 0f);
            Vector3 FarSource = new Vector3(MouseX, MouseY, 1f);
            Vector3 NearSourceOld = new Vector3(OldMouseX, OldMouseY, 0f);

            Vector3 NearPoint = ActiveViewport.Unproject(NearSource, Projection, View, World);
            Vector3 NearPointOld = ActiveViewport.Unproject(NearSourceOld, Projection, View, World);
            Vector3 FarPoint = ActiveViewport.Unproject(FarSource, Projection, View, World);

            // Create a ray from the near clip plane to the far clip plane.
            Vector3 RayDirection = FarPoint - NearPoint;
            RayDirection.Normalize();

            Matrix RotationMatrix = Matrix.Identity;
            RotationMatrix.Forward = RayDirection;
            RotationMatrix.Right = Vector3.Normalize(Vector3.Cross(RotationMatrix.Forward, Vector3.Up));
            RotationMatrix.Up = Vector3.Cross(RotationMatrix.Right, RotationMatrix.Forward);
            Vector3 Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Vector3 Right = Vector3.Transform(Vector3.Right, RotationMatrix);
            Vector3 Up = Vector3.Transform(Vector3.Up, RotationMatrix);

            float MinRight, MaxRight;
            float MinUp, MaxUp;
            PolygonMesh.ProjectPolygon(Right, CollisionBox, out MinRight, out MaxRight);
            PolygonMesh.ProjectPolygon(Up, CollisionBox, out MinUp, out MaxUp);
            float MouseRight = Vector3.Dot(Right, NearPoint);
            float MouseUp = Vector3.Dot(Up, NearPoint);
            float MouseRightOld = Vector3.Dot(Right, NearPointOld);
            float MouseUpOld = Vector3.Dot(Up, NearPointOld);

            Position += Right * (MouseRightOld - MouseRight);
            Position += Right * (MouseUpOld - MouseUp);
        }

        public void MoveWithMouseX(int MouseX, int MouseY, int OldMouseX, int OldMouseY, Viewport ActiveViewport, Matrix View, Matrix Projection, Matrix World)
        {
            Vector3 NearSource = new Vector3(MouseX, MouseY, 0f);
            Vector3 NearSourceOld = new Vector3(OldMouseX, OldMouseY, 0f);

            Vector3 NearPoint = ActiveViewport.Unproject(NearSource, Projection, View, World);
            Vector3 NearPointOld = ActiveViewport.Unproject(NearSourceOld, Projection, View, World);

            Vector3 MovementDirection = Vector3.Forward;

            float MouseValue = Vector3.Dot(MovementDirection, NearPoint);
            float MouseValueOld = Vector3.Dot(MovementDirection, NearPointOld);

            Position += MovementDirection * (MouseValue - MouseValueOld) * 500;
        }

        public void MoveWithMouseY(int MouseX, int MouseY, int OldMouseX, int OldMouseY, Viewport ActiveViewport, Matrix View, Matrix Projection, Matrix World)
        {
            Vector3 NearSource = new Vector3(MouseX, MouseY, 0f);
            Vector3 NearSourceOld = new Vector3(OldMouseX, OldMouseY, 0f);

            Vector3 NearPoint = ActiveViewport.Unproject(NearSource, Projection, View, World);
            Vector3 NearPointOld = ActiveViewport.Unproject(NearSourceOld, Projection, View, World);

            Vector3 MovementDirection = Vector3.Up;

            float MouseValue = Vector3.Dot(MovementDirection, NearPoint);
            float MouseValueOld = Vector3.Dot(MovementDirection, NearPointOld);

            Position += MovementDirection * (MouseValue - MouseValueOld) * 500;
        }

        public void MoveWithMouseZ(int MouseX, int MouseY, int OldMouseX, int OldMouseY, Viewport ActiveViewport, Matrix View, Matrix Projection, Matrix World)
        {
            Vector3 NearSource = new Vector3(MouseX, MouseY, 0f);
            Vector3 NearSourceOld = new Vector3(OldMouseX, OldMouseY, 0f);

            Vector3 NearPoint = ActiveViewport.Unproject(NearSource, Projection, View, World);
            Vector3 NearPointOld = ActiveViewport.Unproject(NearSourceOld, Projection, View, World);

            Vector3 MovementDirection = Vector3.Right;
            
            float MouseValue= Vector3.Dot(MovementDirection, NearPoint);
            float MouseValueOld = Vector3.Dot(MovementDirection, NearPointOld);

            Position += MovementDirection * (MouseValue - MouseValueOld) * 500;
        }

        public void RotateWithMouseX(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection, Matrix World)
        {
            Vector3 NearSource = new Vector3(MouseX, MouseY, 0f);

            Vector3 NearPoint = ActiveViewport.Unproject(NearSource, Projection, View, World);
            Vector3 ProjectedPoint = ProjectPointOnPlane(Vector3.Right, Position, NearPoint);
            Vector3 NewAngle = Vector3.Normalize(ProjectedPoint - Position);

            float FinalAngle = (float)Math.Acos(Vector3.Dot(NewAngle, Vector3.Up));

            Rotate(Vector3.Dot(NewAngle, Vector3.Up), TotalPitch, TotalRoll);
        }

        public void RotateWithMouseY(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection, Matrix World)
        {
            Vector3 NearSource = new Vector3(MouseX, MouseY, 0f);
            Vector3 FarSource = new Vector3(MouseX, MouseY, 1f);
            Vector3 NearPoint = ActiveViewport.Unproject(NearSource, Projection, View, World);
            Vector3 FarPoint = ActiveViewport.Unproject(FarSource, Projection, View, World);

            Vector3 direction = FarPoint - NearPoint;

            float zFactor = -NearPoint.Y / direction.Y;
            Vector3 zeroWorldPoint = NearPoint + direction * zFactor;
            
            Vector3 ProjectedPoint = ProjectPointOnPlane(Vector3.Up, Position, zeroWorldPoint);
            
            float FinalAngle = (float)Math.Atan2(Position.X - ProjectedPoint.X, Position.Z - ProjectedPoint.Z);

            Rotate(FinalAngle, TotalPitch, TotalRoll);
        }

        public Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {
            var distance = -Vector3.Dot(planeNormal, (point - planePoint));
            return point + planeNormal * distance;
        }

        private void ReCreateCollisionBox()
        {
            _CollisionBox = new PolygonMesh();
            _CollisionBox.Scale(TotalScale);
            _CollisionBox.Rotate(TotalYaw, TotalPitch, TotalRoll);
            _CollisionBox.Offset(Position.X, Position.Y, Position.Z);
        }

        protected void UpdateWorld()
        {
            if (ObjectEffect != null)
            {
                ObjectEffect.World = ScaleMatrix * RotationMatrix * PositionMatrix;
            }
        }

        public abstract void Draw(CustomSpriteBatch g, Matrix View);

        #region Properties

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        public Vector3 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                float X = value.X - Position.X;
                float Y = value.Y - Position.Y;
                float Z = value.Z - Position.Z;
                _Position += new Vector3(X, Y, Z);
                _CollisionBox.Offset(X, Y, Z);
                PositionMatrix = Matrix.CreateTranslation(Position);
                UpdateWorld();
            }
        }

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        public Vector3 Direction
        {
            get
            {
                return new Vector3(TotalYaw, TotalPitch, TotalRoll);
            }
            set
            {
                Rotate(value.X, value.Y, value.Z);
            }
        }

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        public Vector3 Size
        {
            get
            {
                return TotalScale;
            }
            set
            {
                Scale(value);
            }
        }

        #endregion
    }

    public class Object3DColored : Object3D
    {
        protected VertexPositionColor[] ArrayVertex;

        private int TriangleCount { get { return ArrayVertex.Length / 3; } }

        public Object3DColored(GraphicsDevice g, Matrix Projection)
            : base(g, Projection)
        {
            ObjectEffect.VertexColorEnabled = true;
        }

        public Object3DColored(GraphicsDevice g, Matrix Projection, VertexPositionColor[] ArrayVertex)
        : this(g, Projection)
        {
            this.ArrayVertex = ArrayVertex;
        }

        public override void Draw(CustomSpriteBatch g, Matrix View)
        {
            ObjectEffect.View = View;

            ObjectEffect.CurrentTechnique.Passes[0].Apply();
            g.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, ArrayVertex, 0, TriangleCount);
        }
    }
}
