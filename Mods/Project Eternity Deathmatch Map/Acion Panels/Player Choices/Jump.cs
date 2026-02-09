using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Item.ParticleSystem;
using ProjectEternity.GameScreens.BattleMapScreen;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelJump : ActionPanelDeathmatch
    {
        private const string PanelName = "Jump";

        private int PlayerIndex;
        private int SquadIndex;
        private readonly Squad ActiveSquad;
        private List<MovementAlgorithmTile> ListMVChoice;

        private static ArcBeamHelper ArcBeamEffect;
        private int Gravity = 1;
        private float JumpSpeed = 13;

        public ActionPanelJump(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelJump(DeathmatchMap Map, int PlayerIndex, int SquadIndex, Squad ActiveSquad)
            : base(PanelName, Map)
        {
            this.PlayerIndex = PlayerIndex;
            this.SquadIndex = SquadIndex;
            this.ActiveSquad = ActiveSquad;

            ListMVChoice = new List<MovementAlgorithmTile>();
        }

        public override void OnSelect()
        {
            if (ArcBeamEffect == null)
            {
                ArcBeamEffect = new ArcBeamHelper();
                ArcBeamEffect.InitBillboard(GameScreen.GraphicsDevice, Map.Content);
            }
        }

        public static void AddIfUsable(DeathmatchMap Map, ActionPanel Owner, int PlayerIndex, int SquadIndex, Squad ActiveSquad)
        {
            if (ActiveSquad.CurrentTerrainIndex != UnitStats.TerrainLandIndex)
            {
                return;
            }

            if (ActiveSquad.Position.X + 1 < Map.MapSize.Y)
            {
                Terrain NeighborTerrin = Map.GetTerrain(new Vector3(ActiveSquad.Position.X + 1, ActiveSquad.Position.Y, ActiveSquad.Z));
                if (NeighborTerrin.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelJump(Map, PlayerIndex, SquadIndex, ActiveSquad));
                    return;
                }
            }

            if (ActiveSquad.Position.X - 1 >= 0)
            {
                Terrain NeighborTerrin = Map.GetTerrain(new Vector3(ActiveSquad.Position.X - 1, ActiveSquad.Position.Y, ActiveSquad.Z));
                if (NeighborTerrin.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelJump(Map, PlayerIndex, SquadIndex, ActiveSquad));
                    return;
                }
            }

            if (ActiveSquad.Position.Y + 1 < Map.MapSize.Y)
            {
                Terrain NeighborTerrin = Map.GetTerrain(new Vector3(ActiveSquad.Position.X, ActiveSquad.Position.Y + 1, ActiveSquad.Z));
                if (NeighborTerrin.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelJump(Map, PlayerIndex, SquadIndex, ActiveSquad));
                    return;
                }
            }

            if (ActiveSquad.Position.Y - 1 >= 0)
            {
                Terrain NeighborTerrin = Map.GetTerrain(new Vector3(ActiveSquad.Position.X, ActiveSquad.Position.Y - 1, ActiveSquad.Z));
                if (NeighborTerrin.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelJump(Map, PlayerIndex, SquadIndex, ActiveSquad));
                    return;
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Map.CursorControl(gameTime, ActiveInputManager))
            {
                Vector3 StartPosition = Map.GetFinalPosition(ActiveSquad.Position + new Vector3(0.5f, 0.5f, 0f)) * 32;
                Vector3 TargetPosition = Map.GetFinalPosition(Map.CursorPosition + new Vector3(0.5f, 0.5f, 0f)) * 32;
                StartPosition = new Vector3(StartPosition.X, StartPosition.Z, StartPosition.Y);
                TargetPosition = new Vector3(TargetPosition.X, TargetPosition.Z, TargetPosition.Y);

                ArcBeamEffect.UpdateList(StartPosition, TargetPosition, JumpSpeed, Gravity);
                ListMVChoice = GetJumpPositions();
            }

            if (ActiveInputManager.InputConfirmPressed())
            {
                Vector3 StartPosition = Map.GetFinalPosition(ActiveSquad.Position + new Vector3(0.5f, 0.5f, 0f));
                ActiveSquad.SetPosition(StartPosition);
                ActiveSquad.IsOnGround = false;
                ActiveSquad.CurrentTerrainIndex = UnitStats.TerrainAirIndex;
                ActiveSquad.Speed = new Vector3(ArcBeamEffect.HighAngleSpeed.X, ArcBeamEffect.HighAngleSpeed.Z, ArcBeamEffect.HighAngleSpeed.Y);

                RemoveAllSubActionPanels();
                AddToPanelList(new ActionPanelAutoMove(Map, PlayerIndex, SquadIndex, ActiveSquad));
            }

            Map.LayerManager.AddDrawablePoints(ListMVChoice, Color.FromNonPremultiplied(0, 128, 0, 190));
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListMVChoice, Color.FromNonPremultiplied(0, 128, 0, 190));
        }

        private List<MovementAlgorithmTile> GetJumpPositions()
        {
            List<MovementAlgorithmTile> ListMVChoice = new List<MovementAlgorithmTile>();

            int MaxDist = 5;

            int StartX = (int)Math.Max(0, ActiveSquad.Position.X - MaxDist);
            int StartY = (int)Math.Max(0, ActiveSquad.Position.Y - MaxDist);
            int StartZ = (int)ActiveSquad.Position.Z;
            Vector3 StartPosition = ActiveSquad.Position;
            StartPosition = new Vector3(StartPosition.X, StartPosition.Z, StartPosition.Y);

            int EndX = (int)Math.Min(Map.MapSize.X - 1, ActiveSquad.Position.X + MaxDist);
            int EndY = (int)Math.Min(Map.MapSize.Y - 1, ActiveSquad.Position.Y + MaxDist);
            int EndZ = Math.Min(Map.LayerManager.ListLayer.Count - 1, StartZ + 2);

            for (int X = EndX; X >= StartX; --X)
            {
                for (int Y = EndY; Y >= StartY; --Y)
                {
                    bool TerrainFound = false;

                    for (int Z = EndZ; Z >= 0; --Z)
                    {
                        Vector3 Position = new Vector3(X, Y, Z);
                        Vector3 TargetPosition = Map.GetFinalPosition(new Vector3(X + 0.5f, Y + 0.5f, Z)) * 32;
                        TargetPosition = new Vector3(TargetPosition.X, TargetPosition.Z, TargetPosition.Y);

                        Terrain ActiveTerrain = Map.GetTerrain(new Vector3(X, Y, Z));

                        byte CurrentTerrainIndex = ActiveTerrain.TerrainTypeIndex;
                        TerrainType CurrentTerrainType = Map.TerrainRestrictions.ListTerrainType[CurrentTerrainIndex];

                        bool IsOnUsableTerrain = CurrentTerrainType.ListRestriction.Count > 0;

                        if (IsOnUsableTerrain && ArcBeamHelper.SolveBallisticArc(StartPosition, JumpSpeed, TargetPosition, 1, out _, out _) == 2)
                        {
                            ListMVChoice.Add(ActiveTerrain);
                            TerrainFound = true;
                            break;
                        }
                    }

                    if (!TerrainFound)
                    {
                        int Z = Math.Max(0, StartZ - 1);
                        Vector3 Position = new Vector3(X, Y, Z);
                        Vector3 TargetPosition = Map.GetFinalPosition(new Vector3(X + 0.5f, Y + 0.5f, Z)) * 32;
                        TargetPosition = new Vector3(TargetPosition.X, TargetPosition.Z, TargetPosition.Y);

                        if (ArcBeamHelper.SolveBallisticArc(StartPosition, JumpSpeed, TargetPosition, 1, out _, out _) == 2)
                        {
                            Terrain ActiveTerrain = Map.GetTerrain(new Vector3(X, Y, Z));
                            ListMVChoice.Add(ActiveTerrain);
                        }
                    }
                }
            }

            return ListMVChoice;
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelJump(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            Vector3 BeamOrigin = new Vector3(ActiveSquad.Position.X + 0.5f, ActiveSquad.Position.Z + 0.5f, ActiveSquad.Position.Y + 0.5f) * 32;

            ArcBeamEffect.DrawLine(g.GraphicsDevice, Map.Camera3D, BeamOrigin);
        }
    }
}
