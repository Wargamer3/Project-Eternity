using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelHub3DStep : ActionPanelDeathmatch
    {
        private const string PanelName = "ActionPanelHub3DStep";

        private float CameraHeight = 50;
        private float CameraDistance = 75;

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad HubSquad;
        private Camera3D Camera;
        public float Yaw;
        public float Pitch;
        public float Roll;

        private bool AffectedByGravity = true;
        private bool FallDamage = true;
        private float MaxInclineDeviationAllowed = 0.1f;

        public ActionPanelHub3DStep(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelHub3DStep(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;

            this.HubSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            HubSquad.IsOnGround = true;
        }

        public override void OnSelect()
        {
            Camera = new DefaultCamera(GameScreen.GraphicsDevice);
            Map.CameraOverride = Camera;
            Yaw = MathHelper.PiOver2;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Matrix RotationMatrix = Matrix.CreateFromYawPitchRoll(Yaw - MathHelper.PiOver2, Pitch, Roll);
            Vector3 Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Forward = new Vector3(Forward.X, Forward.Z, Forward.Y);

            Terrain StartTerrain = Map.GetTerrain(HubSquad.Position + new Vector3(0.5f, 0.5f, 0));
            Vector3 NextTerrainRealPosition = StartTerrain.GetRealPosition(HubSquad.Position + new Vector3(0.5f, 0.5f, 0));
            Vector3 Position = new Vector3(NextTerrainRealPosition.X * Map.TileSize.X,
                                (NextTerrainRealPosition.Z + 1f) * 32,
                                (NextTerrainRealPosition.Y) * Map.TileSize.Y);

            Camera.CameraPosition3D = Vector3.Transform(new Vector3(0, 0, CameraDistance), Matrix.CreateRotationY(Yaw - MathHelper.PiOver2))
                + Position;
            Camera.CameraPosition3D = Vector3.Transform(Camera.CameraPosition3D, Matrix.CreateTranslation(0f, CameraHeight, 0f));
            Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Position, Vector3.Up);

            if (ActiveInputManager.InputUpHold())
            {
                Move(gameTime);
                //HubSquad.SetPosition(HubSquad.Position + Forward * (float)gameTime.ElapsedGameTime.TotalSeconds * 10);
            }
            if (ActiveInputManager.InputLeftHold())
            {
                Yaw += 3 * 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (ActiveInputManager.InputRightHold())
            {
                Yaw += 3 * -1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            HubSquad.Direction = MathHelper.ToDegrees(Yaw) + 90;

            Matrix World = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll) * Matrix.CreateTranslation(HubSquad.Position);

        }

        private void Move(GameTime gameTime)
        {
            Matrix RotationMatrix = Matrix.CreateFromYawPitchRoll(Yaw - MathHelper.PiOver2, Pitch, Roll);
            Vector3 Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Forward = new Vector3(Forward.X, Forward.Z, Forward.Y);
            Terrain StartTerrain = Map.GetTerrain(HubSquad.Position);

            double Movement = gameTime.ElapsedGameTime.TotalSeconds * 2;

            Vector3 NextPosition = HubSquad.Position + Forward * (float)Movement;

            if (AffectedByGravity && !HubSquad.IsOnGround)
            {
                NextPosition += -new Vector3(0, 0, 16) * (float)Movement;
            }

            if (NextPosition.X < 0 || NextPosition.X >= Map.MapSize.X || NextPosition.Y < 0 || NextPosition.Y >= Map.MapSize.Y || NextPosition.Z < 0 || NextPosition.Z >= Map.LayerManager.ListLayer.Count)
            {
                HubSquad.IsOnGround = true;

                if (FallDamage)
                {
                }
            }

            Terrain NextTerrain = Map.GetTerrain(new Vector3((int)NextPosition.X, (int)NextPosition.Y, (int)NextPosition.Z));
            Vector3 NextTerrainRealPosition = NextTerrain.GetRealPosition(NextPosition);
            float Incline = NextPosition.Z - HubSquad.LastRealPosition.Z;
            if (HubSquad.IsOnGround)
            {
                Incline = NextTerrainRealPosition.Z - HubSquad.LastRealPosition.Z;
            }

            if (HubSquad.IsOnGround && NextTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex && HubSquad.LastRealPosition.Z + MaxInclineDeviationAllowed < Map.LayerManager.ListLayer.Count)
            {
                Terrain UpperNextTerrain = Map.GetTerrain(new Vector3(NextPosition.X, NextPosition.Y, NextPosition.Z + MaxInclineDeviationAllowed));
                if (UpperNextTerrain != NextTerrain)
                {
                    Vector3 UpperNextTerrainRealPosition = NextTerrain.GetRealPosition(NextPosition);
                    Incline = UpperNextTerrainRealPosition.Z - HubSquad.LastRealPosition.Z;
                }
            }

            if (Incline > 0 && Incline < MaxInclineDeviationAllowed && HubSquad.Speed.Z == 0)
            {
                HubSquad.SetPosition(NextPosition);
                HubSquad.IsOnGround = true;
            }
            else if (!HubSquad.IsOnGround && StartTerrain != NextTerrain && NextPosition.Z < NextTerrainRealPosition.Z && NextTerrain.TerrainTypeIndex == UnitStats.TerrainLandIndex)
            {
                NextPosition.Z = NextTerrain.WorldPosition.Z;
                HubSquad.SetPosition(NextPosition);

                HubSquad.IsOnGround = true;

                if (FallDamage)
                {
                }
            }
            else
            {
                HubSquad.SetPosition(NextPosition);
            }


            HubSquad. SetPosition(NextPosition);
            HubSquad.LastRealPosition = NextTerrainRealPosition;
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            HubSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
        }

        protected override ActionPanel Copy()
        {
            throw new NotImplementedException();
        }
        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
