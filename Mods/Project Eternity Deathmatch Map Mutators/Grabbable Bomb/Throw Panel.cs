using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelThrowBomb : ActionPanelDeathmatch
    {
        private const string PanelName = "Throw";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private PERAttack AttackToThrow;
        private Squad ActiveSquad;
        private List<MovementAlgorithmTile> ListThrowLocation;
        private int MaxThrowDistance = 5;
        BasicEffect BeamEffect;
        VertexBuffer BeamVertexBuffer;
        IndexBuffer BeamIndexBuffer;
        private List<Vector3> ListLinePoint = new List<Vector3>();
        float Gravity = 1;
        Vector3 ThrowSpeed;

        public ActionPanelThrowBomb(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelThrowBomb(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, PERAttack AttackToThrow)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.AttackToThrow = AttackToThrow;

            ListThrowLocation = new List<MovementAlgorithmTile>();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
        }

        private void InitBillboard()
        {
            BeamEffect = new BasicEffect(GameScreen.GraphicsDevice);
            BeamEffect.TextureEnabled = true;
            BeamEffect.Texture = Map.Content.Load<Texture2D>("Line 2");

            float x = 1;
            float y = 1;

            Vector3 upperLeft = new Vector3(-x * 0.5f, 0, y);
            Vector3 upperRight = new Vector3(x * 0.5f, 0, y);
            Vector3 lowerLeft = new Vector3(-x * 0.5f, 0, 0);
            Vector3 lowerRight = new Vector3(x * 0.5f, 0, 0);

            VertexPositionTexture[] vertices =
            {
                new VertexPositionTexture(upperLeft,  new Vector2(0.0f, 0.0f)),  // 0
                new VertexPositionTexture(upperRight, new Vector2(1.0f, 0.0f)),  // 1
                new VertexPositionTexture(lowerLeft,  new Vector2(0.0f, 1.0f)),  // 2
                new VertexPositionTexture(lowerRight, new Vector2(1.0f, 1.0f)),  // 3
            };

            BeamVertexBuffer = new VertexBuffer(GameScreen.GraphicsDevice, typeof(VertexPositionTexture), vertices.Length, BufferUsage.WriteOnly);
            BeamVertexBuffer.SetData(vertices);

            short[] indices =
            {
                0, 1, 2,
                2, 1, 3
            };

            BeamIndexBuffer = new IndexBuffer(GameScreen.GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            BeamIndexBuffer.SetData(indices);
        }

        float travel_time(float Distance, float Speed)
        {
            return (float)(Distance / Speed);
        }

        public static int solve_ballistic_arc(Vector3 proj_pos, float proj_speed, Vector3 target, float gravity, out Vector3 s0, out Vector3 s1)
        {
            s0 = Vector3.Zero;
            s1 = Vector3.Zero;

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

            Vector3 diff = target - proj_pos;
            Vector3 diffXZ = new Vector3(diff.X, 0f, diff.Z);
            float groundDist = diffXZ.Length();

            float speed2 = proj_speed * proj_speed;
            float speed4 = proj_speed * proj_speed * proj_speed * proj_speed;
            float y = diff.Y;
            float x = groundDist;
            float gx = gravity * x;

            float root = speed4 - gravity * (gravity * x * x + 2 * y * speed2);

            // No solution
            if (root < 0)
                return 0;

            root = (float)Math.Sqrt(root);

            float lowAng = (float)Math.Atan2(speed2 - root, gx);
            float highAng = (float)Math.Atan2(speed2 + root, gx);
            int numSolutions = lowAng != highAng ? 2 : 1;

            Vector3 groundDir = Vector3.Normalize(diffXZ);
            s0 = groundDir * (float)Math.Cos(lowAng) * proj_speed + Vector3.Up * (float)Math.Sin(lowAng) * proj_speed;
            if (numSolutions > 1)
                s1 = groundDir * (float)Math.Cos(highAng) * proj_speed + Vector3.Up * (float)Math.Sin(highAng) * proj_speed;

            return numSolutions;
        }

        private void UpdateList()
        {
            Vector3 StartPosition = Map.GetTerrain(ActiveSquad.Position.X, ActiveSquad.Position.Y, (int)ActiveSquad.Position.Z).GetRealPosition(ActiveSquad.Position + new Vector3(0.5f, 0.5f, 0f)) * 32;
            Vector3 TargetPosition = Map.GetTerrain(Map.CursorPosition.X, Map.CursorPosition.Y, (int)Map.CursorPosition.Z).GetRealPosition(Map.CursorPosition + new Vector3(0.5f, 0.5f, 0f)) * 32;
            Vector3 FinalStartPosition = new Vector3(StartPosition.X, StartPosition.Z, StartPosition.Y);
            Vector3 FinalTargetPosition = new Vector3(TargetPosition.X, TargetPosition.Z, TargetPosition.Y);
            Vector3 Distance = FinalTargetPosition - FinalStartPosition;
            float Speed = 13;
            Vector3 LowAngle, HighAngle;
            float GravityValue = 0;

            solve_ballistic_arc(FinalStartPosition, Speed, FinalTargetPosition, Gravity, out LowAngle, out HighAngle);
            ThrowSpeed = HighAngle;

            Vector3 Position = FinalStartPosition;

            Distance.Y = 0;
            float TimeRemaing = travel_time(HighAngle.X, Speed) + travel_time(HighAngle.Z, Speed);

            ListLinePoint.Clear();
            while (TimeRemaing >= 0 && Position.Y >= TargetPosition.Z)
            {
                TimeRemaing -= 0.1f;
                ListLinePoint.Add(Position);
                //Position.X += CosA * 13 * 0.1f;
                //Position.Y += SinA * Speed * 0.1f;
                //Position.Z += CosA2 * 13 * 0.1f;
                Position.X += HighAngle.X * 0.1f;
                Position.Y += (HighAngle.Y - (GravityValue)) * 0.1f;
                Position.Z += HighAngle.Z * 0.1f;
                Speed -= Gravity * 0.1f;
                GravityValue += Gravity * 0.1f;
            }
        }

        public override void OnSelect()
        {
            ListThrowLocation = Map.GetAttackChoice(ActiveSquad, MaxThrowDistance);
            InitBillboard();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListThrowLocation, Color.FromNonPremultiplied(255, 0, 0, 190));
            if (ActiveInputManager.InputConfirmPressed())
            {
                int TargetSelect = 0;
                //Verify if the cursor is over one of the possible position.
                while ((Map.CursorPosition.X != ListThrowLocation[TargetSelect].InternalPosition.X || Map.CursorPosition.Y != ListThrowLocation[TargetSelect].InternalPosition.Y)
                    && ++TargetSelect < ListThrowLocation.Count) ;

                //If nothing was found.
                if (TargetSelect >= ListThrowLocation.Count)
                    return;

                List<PERAttack> ListAttackToUpdate = new List<PERAttack>();
                AttackToThrow.Position = ActiveSquad.Position + new Vector3(0.5f, 0.5f, 0.0f);
                AttackToThrow.Speed = new Vector3(ThrowSpeed.X, ThrowSpeed.Z, ThrowSpeed.Y);
                AttackToThrow.IsOnGround = false;
                ListAttackToUpdate.Add(AttackToThrow);
                Map.ListActionMenuChoice.Add(new ActionPanelUpdatePERAttacks(Map, ListAttackToUpdate));
            }
            else
            {
                if (Map.UpdateMapNavigation(ActiveInputManager))
                { 
                    UpdateList();
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelThrowBomb(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawLine(g.GraphicsDevice);
        }

        public void DrawLine(GraphicsDevice g)
        {
            g.SetVertexBuffer(BeamVertexBuffer);
            g.Indices = BeamIndexBuffer;
            g.BlendState = BlendState.NonPremultiplied;
            g.RasterizerState = RasterizerState.CullNone;
            BeamEffect.View = Map.Camera.View;
            BeamEffect.Projection = Map.Camera.Projection;

            Vector3 BeamOrigin = new Vector3(ActiveSquad.Position.X + 0.5f, ActiveSquad.Position.Z + 0.5f, ActiveSquad.Position.Y + 0.5f) * 32;

            Vector3 BeamTarget = BeamOrigin;
            for (int L = 1; L < ListLinePoint.Count; L++)
            {
                Vector3 ActivePoint = ListLinePoint[L];
                BeamOrigin = BeamTarget;
                BeamTarget = ActivePoint;
                Vector3 BeamDirection = BeamTarget - BeamOrigin;
                BeamDirection.Normalize();

                Vector3 BeamOriginToCamera = Map.Camera.CameraPosition3D - BeamOrigin;
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
