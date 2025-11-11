using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelUseTerritoryAbility : ActionPanelSorcererStreet
    {
        private const string PanelName = "Terrain Commands";

        private enum CreatureCommands { LevelLand, CreatureMovement, TerrainChange }

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private TerrainSorcererStreet ActiveTerrain;

        public ActionPanelUseTerritoryAbility(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelUseTerritoryAbility(SorcererStreetMap Map, int ActivePlayerIndex, TerrainSorcererStreet ActiveTerrain)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            this.ActiveTerrain = ActiveTerrain;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                if (--ActionMenuCursor < 0)
                {
                    ActionMenuCursor = 5;
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (++ActionMenuCursor > 5)
                {
                    ActionMenuCursor = 0;
                }
            }
            else if (InputHelper.InputConfirmPressed())
            {
                if (ActionMenuCursor == 0)
                {
                    ActiveTerrain.DefendingCreature.TerritoryAbility.ActiveSkillFromMenu();
                }
                else
                {
                    RemoveFromPanelList(this);
                }
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveFromPanelList(this);
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            ActiveTerrain = Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveTerrain.GridPosition.X);
            BW.AppendInt32(ActiveTerrain.GridPosition.Y);
            BW.AppendInt32(ActiveTerrain.LayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelUseTerritoryAbility(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
