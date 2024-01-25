using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item.ParticleSystem
{
    public class ArcBeamHelper
    {
        private BasicEffect BeamEffect;
        private VertexBuffer BeamVertexBuffer;
        private IndexBuffer BeamIndexBuffer;
        private List<Vector3> ListLinePoint = new List<Vector3>();

        public Vector3 HighAngleSpeed;

        public void InitBillboard(GraphicsDevice g, ContentManager Content)
        {
            BeamEffect = new BasicEffect(g);
            BeamEffect.TextureEnabled = true;
            BeamEffect.Texture = Content.Load<Texture2D>("Line 2");

            float X = 1;
            float Y = 1;

            Vector3 UpperLeft = new Vector3(-X * 0.5f, 0, Y);
            Vector3 UpperRight = new Vector3(X * 0.5f, 0, Y);
            Vector3 LowerLeft = new Vector3(-X * 0.5f, 0, 0);
            Vector3 LowerRight = new Vector3(X * 0.5f, 0, 0);

            VertexPositionTexture[] ArrayVertex =
            {
                new VertexPositionTexture(UpperLeft,  new Vector2(0.0f, 0.0f)),  // 0
                new VertexPositionTexture(UpperRight, new Vector2(1.0f, 0.0f)),  // 1
                new VertexPositionTexture(LowerLeft,  new Vector2(0.0f, 1.0f)),  // 2
                new VertexPositionTexture(LowerRight, new Vector2(1.0f, 1.0f)),  // 3
            };

            BeamVertexBuffer = new VertexBuffer(g, typeof(VertexPositionTexture), ArrayVertex.Length, BufferUsage.WriteOnly);
            BeamVertexBuffer.SetData(ArrayVertex);

            short[] ArrayIndex =
            {
                0, 1, 2,
                2, 1, 3
            };

            BeamIndexBuffer = new IndexBuffer(g, IndexElementSize.SixteenBits, ArrayIndex.Length, BufferUsage.WriteOnly);
            BeamIndexBuffer.SetData(ArrayIndex);
        }

        private float GetTravelTime(float Distance, float Speed)
        {
            return (float)(Distance / Speed);
        }

        public static int SolveBallisticArc(Vector3 StartPosition, float Speed, Vector3 TargetPosition, float Gravity, out Vector3 LowAngleSpeed, out Vector3 HighAngleSpeed)
        {
            LowAngleSpeed = Vector3.Zero;
            HighAngleSpeed = Vector3.Zero;

            // Derivation
            //   (1) x = v*t*cos O
            //   (2) y = v*t*sin O - .5*g*t^2
            // 
            //   (3) t = x/(cos O*v)                                        [solve t from (1)]
            //   (4) y = v*x*sin O/(cos O * v) - .5*g*x^2/(cos^2 O*v^2)     [plug t into y=...]
            //   (5) y = x*tan O - g*x^2/(2*v^2*cos^2 O)                    [reduce; cos/sin = tan]
            //   (6) y = x*tan O - (g*x^2/(2*v^2))*(1+tan^2 O)              [reduce; 1+tan O = 1/cos^2 O]
            //   (7) 0 = ((-g*x^2)/(2*v^2))*tan^2 O + x*tan O - (g*x^2)/(2*v^2) - y    [re-arrange]
            //   Quadratic! a*p^2 + b*p + c where p = tan O
            //
            //   (8) let gxv = -g*x*x/(2*v*v)
            //   (9) p = (-x +- sqrt(x*x - 4gxv*(gxv - y)))/2*gxv           [quadratic formula]
            //   (10) p = (v^2 +- sqrt(v^4 - g(g*x^2 + 2*y*v^2)))/gx        [multiply top/bottom by -2*v*v/x; move 4*v^4/x^2 into root]
            //   (11) O = atan(p)

            Vector3 GravityVector = new Vector3(0, Gravity, 0);
            Vector3 GroundVector = new Vector3(1, 0, 1);

            Vector3 diff = TargetPosition - StartPosition;
            Vector3 HorizontalDistance = diff * GroundVector;
            float VerticalDistance = Vector3.Dot(GravityVector, diff);
            float GroundDist = HorizontalDistance.Length();

            float speed2 = Speed * Speed;
            float speed4 = Speed * Speed * Speed * Speed;
            float TotalGravityApplied = Gravity * GroundDist;

            float root = speed4 - Gravity * (Gravity * GroundDist * GroundDist + 2 * VerticalDistance * speed2);

            // No solution
            if (root < 0)
                return 0;

            root = (float)Math.Sqrt(root);

            float LowAngle = (float)Math.Atan2(speed2 - root, TotalGravityApplied);
            float HighAngle = (float)Math.Atan2(speed2 + root, TotalGravityApplied);
            int numSolutions = LowAngle != HighAngle ? 2 : 1;

            Vector3 TravelDir = Vector3.Normalize(HorizontalDistance);
            LowAngleSpeed = TravelDir * (float)Math.Cos(LowAngle) * Speed + GravityVector * (float)Math.Sin(LowAngle) * Speed;
            if (numSolutions > 1)
                HighAngleSpeed = TravelDir * (float)Math.Cos(HighAngle) * Speed + GravityVector * (float)Math.Sin(HighAngle) * Speed;

            return numSolutions;
        }

        public void UpdateList(Vector3 StartPosition, Vector3 TargetPosition, float Speed, float Gravity)
        {
            Vector3 FinalStartPosition = StartPosition;
            Vector3 FinalTargetPosition = TargetPosition;
            Vector3 Distance = FinalTargetPosition - FinalStartPosition;
            Vector3 LowAngleSpeed, HighAngleSpeed;
            float AccumulatedGravity = 0;

            ListLinePoint.Clear();
            int ThrowAnglesFound = SolveBallisticArc(FinalStartPosition, Speed, FinalTargetPosition, Gravity, out LowAngleSpeed, out HighAngleSpeed);

            this.HighAngleSpeed = HighAngleSpeed;

            if (ThrowAnglesFound == 2)
            {
                Vector3 Position = FinalStartPosition;

                Distance.Y = 0;
                float TimeRemaing = GetTravelTime(HighAngleSpeed.X, Speed) + GetTravelTime(HighAngleSpeed.Z, Speed);
                float FrameLength = 0.1f;

                while (Position.Y >= TargetPosition.Y)
                {
                    TimeRemaing -= FrameLength;
                    ListLinePoint.Add(Position);
                    Position.X += HighAngleSpeed.X * FrameLength;
                    Position.Y += (HighAngleSpeed.Y - AccumulatedGravity) * FrameLength;
                    Position.Z += HighAngleSpeed.Z * FrameLength;
                    AccumulatedGravity += Gravity * FrameLength;
                }
            }
        }

        public void DrawLine(GraphicsDevice g, Camera3D Camera, Vector3 StartPosition)
        {
            g.SetVertexBuffer(BeamVertexBuffer);
            g.Indices = BeamIndexBuffer;
            g.BlendState = BlendState.NonPremultiplied;
            g.RasterizerState = RasterizerState.CullNone;
            BeamEffect.View = Camera.View;
            BeamEffect.Projection = Camera.Projection;

            Vector3 BeamOrigin = StartPosition;

            Vector3 BeamTarget = BeamOrigin;
            for (int L = 1; L < ListLinePoint.Count; L++)
            {
                Vector3 ActivePoint = ListLinePoint[L];
                BeamOrigin = BeamTarget;
                BeamTarget = ActivePoint;
                Vector3 BeamDirection = BeamTarget - BeamOrigin;
                BeamDirection.Normalize();

                Vector3 BeamOriginToCamera = Camera.CameraPosition3D - BeamOrigin;
                Vector3 BeamRight = Vector3.Cross(BeamDirection, BeamOriginToCamera);
                BeamRight.Normalize();

                Vector3 BeamUp = Vector3.Cross(BeamRight, BeamDirection);

                float ScaleFactor = 2f;
                Matrix BeamWord = Matrix.Identity;

                BeamWord.Forward = (-BeamDirection * ScaleFactor) * 1f;
                BeamWord.Right = BeamRight * 4f;
                BeamWord.Up = BeamUp;
                BeamWord.Translation = BeamOrigin;

                BeamEffect.World = BeamWord;
                foreach (EffectPass ActivePass in BeamEffect.CurrentTechnique.Passes)
                {
                    ActivePass.Apply();

                    g.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                        0, 0, BeamVertexBuffer.VertexCount, 0, 2);
                }
            }
        }
    }
}
