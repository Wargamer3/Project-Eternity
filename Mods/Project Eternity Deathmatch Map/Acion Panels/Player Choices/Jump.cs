using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelJump : ActionPanelDeathmatch
    {
        private const string PanelName = "Jump";

        private readonly Squad ActiveSquad;

        public ActionPanelJump(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelJump(DeathmatchMap Map, Squad ActiveSquad)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
        }

        public static void AddIfUsable(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad)
        {
            if (ActiveSquad.CurrentMovement != UnitStats.TerrainLand)
            {
                return;
            }

            if (ActiveSquad.Position.X + 1 < Map.MapSize.Y)
            {
                Terrain NeighborTerrin = Map.GetTerrain(ActiveSquad.Position.X + 1, ActiveSquad.Position.Y, (int)ActiveSquad.Z);
                if (NeighborTerrin.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelJump(Map, ActiveSquad));
                    return;
                }
            }

            if (ActiveSquad.Position.X - 1 >= 0)
            {
                Terrain NeighborTerrin = Map.GetTerrain(ActiveSquad.Position.X - 1, ActiveSquad.Position.Y, (int)ActiveSquad.Z);
                if (NeighborTerrin.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelJump(Map, ActiveSquad));
                    return;
                }
            }

            if (ActiveSquad.Position.Y + 1 < Map.MapSize.Y)
            {
                Terrain NeighborTerrin = Map.GetTerrain(ActiveSquad.Position.X, ActiveSquad.Position.Y + 1, (int)ActiveSquad.Z);
                if (NeighborTerrin.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelJump(Map, ActiveSquad));
                    return;
                }
            }

            if (ActiveSquad.Position.Y - 1 >= 0)
            {
                Terrain NeighborTerrin = Map.GetTerrain(ActiveSquad.Position.X, ActiveSquad.Position.Y - 1, (int)ActiveSquad.Z);
                if (NeighborTerrin.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    Owner.AddChoiceToCurrentPanel(new ActionPanelJump(Map, ActiveSquad));
                    return;
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.CursorControl(ActiveInputManager);//Move the cursor

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
            return new ActionPanelJump(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
