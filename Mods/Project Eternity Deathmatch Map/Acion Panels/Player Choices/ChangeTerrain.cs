﻿using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelChangeTerrain : ActionPanelDeathmatch
    {
        private readonly Squad ActiveSquad;
        private readonly ActionPanel Owner;
        private readonly TerrainType StartingTerrain;
        private readonly TerrainType TerrainToChangeTo;
        private readonly Terrain NeighbourTerrain;

        public ActionPanelChangeTerrain(DeathmatchMap Map)
            : base("Change Terrain", Map)
        {
        }

        public ActionPanelChangeTerrain(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad, TerrainType StartingTerrain, TerrainType TerrainToChangeTo, Terrain NeighbourTerrain)
            : base(!string.IsNullOrEmpty(TerrainToChangeTo.ActivationName) ? TerrainToChangeTo.ActivationName : StartingTerrain.AnnulatioName, Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.Owner = Owner;
            this.StartingTerrain = StartingTerrain;
            this.TerrainToChangeTo = TerrainToChangeTo;
            this.NeighbourTerrain = NeighbourTerrain;
        }

        public override void OnSelect()
        {
            ActiveSquad.SetPosition(new Vector3(NeighbourTerrain.InternalPosition.X, NeighbourTerrain.InternalPosition.Y, NeighbourTerrain.LayerIndex));
            ActiveSquad.CurrentTerrainIndex = NeighbourTerrain.TerrainTypeIndex;
            Map.CursorPosition = ActiveSquad.Position;
            Map.LayerManager.CursorMoved();

            Map.ActiveSquadIndex = -1;
            RemoveAllSubActionPanels();
        }

        public static void AddIfUsable(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad)
        {
            TerrainType CurrentTerrain = Map.TerrainRestrictions.ListTerrainType[Map.GetTerrainType(ActiveSquad.X, ActiveSquad.Y, (int)ActiveSquad.Z)];

            #region X - 1

            if (ActiveSquad.X - 1 >= 0)
            {
                Terrain NeighbourTerrain = Map.GetTerrain(ActiveSquad.X - 1, ActiveSquad.Y, (int)ActiveSquad.Z);
                TerrainType NeighbourTerrainType = Map.TerrainRestrictions.ListTerrainType[NeighbourTerrain.TerrainTypeIndex];

                if (CurrentTerrain != NeighbourTerrainType && CurrentTerrain.ActivationName != NeighbourTerrainType.ActivationName && NeighbourTerrainType.CanMove(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat))
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelChangeTerrain(Map, Owner, ActiveSquad, CurrentTerrain, NeighbourTerrainType, NeighbourTerrain));
                }
            }

            #endregion

            #region X + 1

            if (ActiveSquad.X + 1 < Map.MapSize.X)
            {
                Terrain NeighbourTerrain = Map.GetTerrain(ActiveSquad.X + 1, ActiveSquad.Y, (int)ActiveSquad.Z);
                TerrainType NeighbourTerrainType = Map.TerrainRestrictions.ListTerrainType[NeighbourTerrain.TerrainTypeIndex];

                if (CurrentTerrain != NeighbourTerrainType && CurrentTerrain.ActivationName != NeighbourTerrainType.ActivationName && NeighbourTerrainType.CanMove(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat))
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelChangeTerrain(Map, Owner, ActiveSquad, CurrentTerrain, NeighbourTerrainType, NeighbourTerrain));
                }
            }

            #endregion

            #region Y - 1

            if (ActiveSquad.Y - 1 >= 0)
            {
                Terrain NeighbourTerrain = Map.GetTerrain(ActiveSquad.X, ActiveSquad.Y - 1, (int)ActiveSquad.Z);
                TerrainType NeighbourTerrainType = Map.TerrainRestrictions.ListTerrainType[NeighbourTerrain.TerrainTypeIndex];

                if (CurrentTerrain != NeighbourTerrainType && CurrentTerrain.ActivationName != NeighbourTerrainType.ActivationName && NeighbourTerrainType.CanMove(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat))
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelChangeTerrain(Map, Owner, ActiveSquad, CurrentTerrain, NeighbourTerrainType, NeighbourTerrain));
                }
            }

            #endregion

            #region Y + 1

            if (ActiveSquad.Y + 1 < Map.MapSize.Y)
            {
                Terrain NeighbourTerrain = Map.GetTerrain(ActiveSquad.X, ActiveSquad.Y + 1, (int)ActiveSquad.Z);
                TerrainType NeighbourTerrainType = Map.TerrainRestrictions.ListTerrainType[NeighbourTerrain.TerrainTypeIndex];

                if (CurrentTerrain != NeighbourTerrainType && CurrentTerrain.ActivationName != NeighbourTerrainType.ActivationName && NeighbourTerrainType.CanMove(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat))
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelChangeTerrain(Map, Owner, ActiveSquad, CurrentTerrain, NeighbourTerrainType, NeighbourTerrain));
                }
            }


            #endregion

            #region Z - 1

            if (ActiveSquad.Z - 1 >= 0)
            {
                Terrain NeighbourTerrain = Map.GetTerrain(ActiveSquad.X, ActiveSquad.Y, (int)ActiveSquad.Z - 1);
                TerrainType NeighbourTerrainType = Map.TerrainRestrictions.ListTerrainType[NeighbourTerrain.TerrainTypeIndex];

                if (CurrentTerrain != NeighbourTerrainType && CurrentTerrain.ActivationName != NeighbourTerrainType.ActivationName && NeighbourTerrainType.CanMove(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat))
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelChangeTerrain(Map, Owner, ActiveSquad, CurrentTerrain, NeighbourTerrainType, NeighbourTerrain));
                }
            }

            #endregion

            #region Z + 1

            if (ActiveSquad.Z + 1 < Map.LayerManager.ListLayer.Count)
            {
                Terrain NeighbourTerrain = Map.GetTerrain(ActiveSquad.X, ActiveSquad.Y, (int)ActiveSquad.Z + 1);
                TerrainType NeighbourTerrainType = Map.TerrainRestrictions.ListTerrainType[NeighbourTerrain.TerrainTypeIndex];

                if (CurrentTerrain != NeighbourTerrainType && CurrentTerrain.ActivationName != NeighbourTerrainType.ActivationName && NeighbourTerrainType.CanMove(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat))
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelChangeTerrain(Map, Owner, ActiveSquad, CurrentTerrain, NeighbourTerrainType, NeighbourTerrain));
                }
            }

            #endregion

        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveInputManager.InputConfirmPressed())
            {
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
            return new ActionPanelChangeTerrain(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
