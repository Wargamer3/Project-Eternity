using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelTerrainLevelUpCommands : ActionPanelSorcererStreet
    {
        private const string PanelName = "Terrain Commands";

        private enum CreatureCommands { LevelLand, CreatureMovement, TerrainChange }

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private TerrainSorcererStreet ActiveTerrain;

        public ActionPanelTerrainLevelUpCommands(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelTerrainLevelUpCommands(SorcererStreetMap Map, int ActivePlayerIndex, TerrainSorcererStreet ActiveTerrain)
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
                if (ActionMenuCursor < 5 && ActionMenuCursor + 2 > ActiveTerrain.LandLevel)
                {
                    int UpgradePrice = GetUpgradePrice(ActionMenuCursor + 2 - ActiveTerrain.LandLevel);
                    if (UpgradePrice <= ActivePlayer.Magic)
                    {
                        ActivePlayer.Magic -= UpgradePrice;
                        ActiveTerrain.UpdateValue(ActivePlayer.DicChainLevelByTerrainTypeIndex[ActiveTerrain.TerrainTypeIndex], ActiveTerrain.DefendingCreature);
                        Map.EndPlayerPhase();
                    }
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
            BW.AppendInt32(ActiveTerrain.InternalPosition.X);
            BW.AppendInt32(ActiveTerrain.InternalPosition.Y);
            BW.AppendInt32(ActiveTerrain.LayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelTerrainLevelUpCommands(Map);
        }

        private int GetUpgradePrice(int Level)
        {
            return (int)(ActiveTerrain.BaseValue * Math.Pow(2, Level) - ActiveTerrain.BaseValue);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActionPanelPlayerDefault.DrawLandInformation(g, Map, ActiveTerrain, Constants.Width / 12f, Constants.Height / 10f);

            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = 137;
            float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth;
            float InfoBoxY = Constants.Height / 10;

            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY - 20), BoxWidth, 20, Color.White);
            g.DrawString(Map.fntArial12, "Menu", new Vector2(InfoBoxX + 10, InfoBoxY - 20), Color.White);
            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight, Color.White);

            float CurrentX = InfoBoxX + 10;
            float CurrentY = InfoBoxY - 10;

            CurrentY += 20;
            g.DrawString(Map.fntArial12, "Make it what terrain?", new Vector2(CurrentX, CurrentY), Color.White);

            CurrentY += 20;
            Color TextColor = Color.White;
            int UpgradePrice = GetUpgradePrice(2 - ActiveTerrain.LandLevel);
            if (ActivePlayer.Magic < UpgradePrice)
            {
                TextColor = Color.Gray;
            }
            g.DrawString(Map.fntArial12, "Level 2", new Vector2(CurrentX + 10, CurrentY), TextColor);
            g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)CurrentX + 130, (int)CurrentY, 18, 18), Color.White);
            g.DrawStringRightAligned(Map.fntArial12, UpgradePrice.ToString(), new Vector2(CurrentX + BoxWidth - 30, CurrentY), TextColor);

            CurrentY += 20;
            UpgradePrice = GetUpgradePrice(3 - ActiveTerrain.LandLevel);
            if (ActivePlayer.Magic < UpgradePrice)
            {
                TextColor = Color.Gray;
            }

            g.DrawString(Map.fntArial12, "Level 3", new Vector2(CurrentX + 10, CurrentY), TextColor);
            g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)CurrentX + 130, (int)CurrentY, 18, 18), Color.White);
            g.DrawStringRightAligned(Map.fntArial12, UpgradePrice.ToString(), new Vector2(CurrentX + BoxWidth - 30, CurrentY), TextColor);

            CurrentY += 20;
            UpgradePrice = GetUpgradePrice(4 - ActiveTerrain.LandLevel);
            if (ActivePlayer.Magic < UpgradePrice)
            {
                TextColor = Color.Gray;
            }

            g.DrawString(Map.fntArial12, "Level 4", new Vector2(CurrentX + 10, CurrentY), TextColor);
            g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)CurrentX + 130, (int)CurrentY, 18, 18), Color.White);
            g.DrawStringRightAligned(Map.fntArial12, UpgradePrice.ToString(), new Vector2(CurrentX + BoxWidth - 30, CurrentY), TextColor);

            CurrentY += 20;
            UpgradePrice = GetUpgradePrice(5 - ActiveTerrain.LandLevel);
            if (ActivePlayer.Magic < UpgradePrice)
            {
                TextColor = Color.Gray;
            }

            g.DrawString(Map.fntArial12, "Level 5", new Vector2(CurrentX + 10, CurrentY), TextColor);
            g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)CurrentX + 130, (int)CurrentY, 18, 18), Color.White);
            g.DrawStringRightAligned(Map.fntArial12, UpgradePrice.ToString(), new Vector2(CurrentX + BoxWidth - 30, CurrentY), TextColor);

            CurrentY += 20;
            g.DrawString(Map.fntArial12, "Return", new Vector2(CurrentX + 10, CurrentY), Color.White);

            MenuHelper.DrawFingerIcon(g, new Vector2(InfoBoxX - 20, InfoBoxY + 30 + 20 * ActionMenuCursor));
        }
    }
}
