using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelHubStep : ActionPanelDeathmatch
    {
        private const string PanelName = "ActionPanelHubStep";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad HubSquad;

        public ActionPanelHubStep(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelHubStep(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;

            this.HubSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.CursorPosition = new Vector3(-1, -1, -1);

            Vector3 NextPosition = HubSquad.Position;
            if (InputHelper.InputUpHold() && CheckTerrain(HubSquad.Position.X, HubSquad.Position.Y - 0.1f))
            {
                NextPosition = new Vector3(HubSquad.Position.X, HubSquad.Position.Y - 0.1f, HubSquad.Z);
            }
            if (InputHelper.InputDownHold() && CheckTerrain(HubSquad.Position.X, HubSquad.Position.Y + 0.1f))
            {
                NextPosition = new Vector3(HubSquad.Position.X, HubSquad.Position.Y + 0.1f, HubSquad.Z);
            }
            if (InputHelper.InputLeftHold() && CheckTerrain(HubSquad.Position.X - 0.1f, HubSquad.Position.Y))
            {
                NextPosition = new Vector3(HubSquad.Position.X - 0.1f, HubSquad.Position.Y, HubSquad.Z);
            }
            if (InputHelper.InputRightHold() && CheckTerrain(HubSquad.Position.X + 0.1f, HubSquad.Position.Y))
            {
                NextPosition = new Vector3(HubSquad.Position.X + 0.1f, HubSquad.Position.Y, HubSquad.Z);
            }

            if (NextPosition != HubSquad.Position)
            {
                Squad CollidingSquad = CheckHubSquad(NextPosition);
                if (CollidingSquad == null)
                {
                    HubSquad.SetPosition(NextPosition);
                }
                else
                {
                    Map.CursorPosition = CollidingSquad.Position;
                    Map.CursorPositionVisible = Map.CursorPosition;
                    List<ActionPanel> SquadSelect = CollidingSquad.OnMenuSelect(ActivePlayerIndex, Map.ListActionMenuChoice);
                    foreach (ActionPanel ActivePanel in SquadSelect)
                    {
                        Map.ListActionMenuChoice.Add(ActivePanel);
                    }
                }
            }

            foreach (MapSwitchPoint ActiveSwitchPoint in Map.ListMapSwitchPoint)
            {
                if (CheckPositionOverlap(ActiveSwitchPoint.Position, NextPosition))
                {
                    ActionPanelMapSwitch.ChangeSquadBetweenMaps(Map, HubSquad, ActiveSwitchPoint);
                }
            }

            //Update the camera if needed.
            while (HubSquad.Position.X - Map.CameraPosition.X - 3 < 0 && Map.CameraPosition.X > 0)
            {
                Map.CameraPosition.X -= 0.1f;
            }
            while (HubSquad.Position.X - Map.CameraPosition.X >= Map.ScreenSize.X / 2 && Map.CameraPosition.X + Map.ScreenSize.X < Map.MapSize.X)
            {
                Map.CameraPosition.X += 0.1f;
            }

            if (HubSquad.Position.Y - Map.CameraPosition.Y - 3 < 0 && Map.CameraPosition.Y > 0)
            {
                Map.CameraPosition.Y -= 0.1f;
            }
            while (HubSquad.Position.Y - Map.CameraPosition.Y >= Map.ScreenSize.Y / 2 && Map.CameraPosition.Y + Map.ScreenSize.Y < Map.MapSize.Y)
            {
                Map.CameraPosition.Y += 0.1f;
            }
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

        private Squad CheckHubSquad(Vector3 Position)
        {
            foreach (Squad ActiveSquad in Map.ListPlayer[Map.ActivePlayerIndex].ListSquad)
            {
                if (ActiveSquad == HubSquad)
                    continue;

                if (CheckPositionOverlap(ActiveSquad.Position, Position))
                    return ActiveSquad;
            }

            return null;
        }

        private bool CheckPositionOverlap(Vector3 Position1, Vector3 Position2)
        {
            float FinalPos1XTrunc = (float)Math.Truncate(Position1.X);
            float FinalPos1XRound = (float)Math.Round(Position1.X);
            float FinalPos1YTrunc = (float)Math.Truncate(Position1.Y);
            float FinalPos1YRound = (float)Math.Round(Position1.Y);

            float FinalPos2XTrunc = (float)Math.Truncate(Position2.X);
            float FinalPos2XRound = (float)Math.Round(Position2.X);
            float FinalPos2YTrunc = (float)Math.Truncate(Position2.Y);
            float FinalPos2YRound = (float)Math.Round(Position2.Y);

            if ((FinalPos1XTrunc == FinalPos2XTrunc || FinalPos1XTrunc == FinalPos2XRound || FinalPos1XRound == FinalPos2XTrunc || FinalPos1XRound == FinalPos2XRound)
                && (FinalPos1YTrunc == FinalPos2YTrunc || FinalPos1YTrunc == FinalPos2YRound || FinalPos1YRound == FinalPos2YTrunc || FinalPos1YRound == FinalPos2YRound))
                return true;

            return false;
        }

        private bool CheckTerrain(float PositionX, float PositionY)
        {
            if (PositionX < 0 || PositionX >= Map.MapSize.X || PositionY < 0 || PositionY >= Map.MapSize.Y)
                return false;

            float FinalPos1XTrunc = (float)Math.Truncate(PositionX);
            float FinalPos1XRound = (float)Math.Round(PositionX);
            float FinalPos1XCeil = (float)Math.Ceiling(PositionX);
            float FinalPos1YTrunc = (float)Math.Truncate(PositionY);
            float FinalPos1YRound = (float)Math.Round(PositionY);
            float FinalPos1YCeil = (float)Math.Ceiling(PositionY);

            List<Vector2> ListPosition = new List<Vector2>();
            ListPosition.Add(new Vector2(FinalPos1XTrunc, FinalPos1YTrunc));
            ListPosition.Add(new Vector2(FinalPos1XTrunc, FinalPos1YRound));
            ListPosition.Add(new Vector2(FinalPos1XTrunc, FinalPos1YCeil));

            ListPosition.Add(new Vector2(FinalPos1XRound, FinalPos1YTrunc));
            ListPosition.Add(new Vector2(FinalPos1XRound, FinalPos1YRound));
            ListPosition.Add(new Vector2(FinalPos1XRound, FinalPos1YCeil));

            ListPosition.Add(new Vector2(FinalPos1XCeil, FinalPos1YTrunc));
            ListPosition.Add(new Vector2(FinalPos1XCeil, FinalPos1YRound));
            ListPosition.Add(new Vector2(FinalPos1XCeil, FinalPos1YCeil));

            foreach (Vector2 ActivePosition in ListPosition)
            {
                Terrain ActiveTerrain = Map.GetTerrain(ActivePosition.X, ActivePosition.Y, 0);
                if (ActiveTerrain.MVEnterCost < 0)
                    return false;
            }

            return true;
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
