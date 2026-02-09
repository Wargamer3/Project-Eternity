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
        private double AITimer;
        private int PlayerAIChoiceIndex;
        int SelectedUpgradeLevel => ActiveTerrain.LandLevel + ActionMenuCursor;

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
            PlayerAIChoiceIndex = 0;

            for (int i = ActiveTerrain.LandLevel + 1; i <= 5; i++)
            {
                int NextUpgradePrice = ActionPanelTerrainLevelUpCommands.GetFinalUpgradePrice(ActiveTerrain, i);

                if (ActivePlayer.Inventory.Character.Character.PlayerCharacterAIParameter.IsWillingToPurchase(ActivePlayer.Gold, NextUpgradePrice))
                {
                    PlayerAIChoiceIndex = i - ActiveTerrain.LandLevel;
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!ActivePlayer.IsPlayerControlled)
            {
                AITimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (AITimer > 0.6f)
                {
                    AITimer = 0;
                    if (ActionMenuCursor < PlayerAIChoiceIndex)
                    {
                        ++ActionMenuCursor;
                    }
                    else if (ActionMenuCursor > PlayerAIChoiceIndex)
                    {
                        --ActionMenuCursor;
                    }
                    else
                    {
                        int UpgradePrice = GetFinalUpgradePrice(ActiveTerrain, SelectedUpgradeLevel);
                        if (UpgradePrice <= ActivePlayer.Gold)
                        {
                            BuyUpgrade(UpgradePrice);
                        }
                    }
                }
                return;
            }

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
                if (ActionMenuCursor < 5 - ActiveTerrain.LandLevel)
                {
                    int UpgradePrice = GetFinalUpgradePrice(ActiveTerrain, ActiveTerrain.LandLevel + ActionMenuCursor);
                    if (UpgradePrice <= ActivePlayer.Gold)
                    {
                        BuyUpgrade(UpgradePrice);
                    }
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

        private void BuyUpgrade(int UpgradePrice)
        {
            ActivePlayer.Gold -= UpgradePrice;
            ActiveTerrain.LandLevel += ActionMenuCursor + 1;
            Map.UpdateTotalMagic();
            Map.EndPlayerPhase();
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
            return new ActionPanelTerrainLevelUpCommands(Map);
        }

        public static int GetUpgradePrice(TerrainSorcererStreet ActiveTerrain, int Level)
        {
            return (int)(ActiveTerrain.BaseValue * Math.Pow(2, Level) - ActiveTerrain.BaseValue);
        }

        public static int GetFinalUpgradePrice(TerrainSorcererStreet ActiveTerrain, int Level)
        {
            return GetUpgradePrice(ActiveTerrain, Level) - GetUpgradePrice(ActiveTerrain, Math.Max(0, ActiveTerrain.LandLevel - 1));
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActionPanelPlayerDefault.DrawLandInformationTop(g, Map, ActiveTerrain);

            float Ratio = Constants.Height / 720f;
            int BoxWidth = (int)(456 * Ratio);
            int BoxHeight = (int)(200 * Ratio);
            float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth;
            float InfoBoxY = Constants.Height / 10;

            MenuHelper.DrawNamedBox(g, "Menu", new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight);

            float CurrentX = InfoBoxX + 40 * Ratio;
            float CurrentY = InfoBoxY - 20 * Ratio;

            CurrentY += Map.fntMenuText.LineSpacing;
            g.DrawString(Map.fntMenuText, "Raise to how much?", new Vector2(CurrentX, CurrentY), SorcererStreetMap.TextColor);

            CurrentY += Map.fntMenuText.LineSpacing;
            Color TextColor = SorcererStreetMap.TextColor;

            if (ActiveTerrain.LandLevel < 2)
            {
                int UpgradePrice = GetUpgradePrice(ActiveTerrain, 1);
                if (ActivePlayer.Gold < UpgradePrice)
                {
                    TextColor = Color.Gray;
                    g.Draw(IconHolder.Icons.sprMenuLimitG, new Rectangle((int)(CurrentX + 390 * Ratio), (int)CurrentY, (int)(32 * Ratio), (int)(32 * Ratio)), Color.White);
                }
                g.DrawString(Map.fntMenuText, "Level 2", new Vector2((int)(CurrentX + 10 * Ratio), CurrentY), TextColor);
                g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)(CurrentX + 280 * Ratio), (int)CurrentY, (int)(14 * Ratio), (int)(23 * Ratio)), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, UpgradePrice.ToString(), new Vector2((int)(CurrentX + 380 * Ratio), CurrentY), TextColor);

                CurrentY += Map.fntMenuText.LineSpacing;
            }

            if (ActiveTerrain.LandLevel < 3)
            {
                int UpgradePrice = GetFinalUpgradePrice(ActiveTerrain, 2);
                if (ActivePlayer.Gold < UpgradePrice)
                {
                    TextColor = Color.Gray;
                    g.Draw(IconHolder.Icons.sprMenuLimitG, new Rectangle((int)(CurrentX + 390 * Ratio), (int)CurrentY, (int)(32 * Ratio), (int)(32 * Ratio)), Color.White);
                }

                g.DrawString(Map.fntMenuText, "Level 3", new Vector2((int)(CurrentX + 10 * Ratio), CurrentY), TextColor);
                g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)(CurrentX + 280 * Ratio), (int)CurrentY, (int)(14 * Ratio), (int)(23 * Ratio)), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, UpgradePrice.ToString(), new Vector2((int)(CurrentX + 380 * Ratio), CurrentY), TextColor);

                CurrentY += Map.fntMenuText.LineSpacing;
            }

            if (ActiveTerrain.LandLevel < 4)
            {
                int UpgradePrice = GetFinalUpgradePrice(ActiveTerrain, 3);
                if (ActivePlayer.Gold < UpgradePrice)
                {
                    TextColor = Color.Gray;
                    g.Draw(IconHolder.Icons.sprMenuLimitG, new Rectangle((int)(CurrentX + 390 * Ratio), (int)CurrentY, (int)(32 * Ratio), (int)(32 * Ratio)), Color.White);
                }

                g.DrawString(Map.fntMenuText, "Level 4", new Vector2((int)(CurrentX + 10 * Ratio), CurrentY), TextColor);
                g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)(CurrentX + 280 * Ratio), (int)CurrentY, (int)(14 * Ratio), (int)(23 * Ratio)), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, UpgradePrice.ToString(), new Vector2((int)(CurrentX + 380 * Ratio), CurrentY), TextColor);

                CurrentY += Map.fntMenuText.LineSpacing;
            }

            if (ActiveTerrain.LandLevel < 5)
            {
                int UpgradePrice = GetFinalUpgradePrice(ActiveTerrain, 4);
                if (ActivePlayer.Gold < UpgradePrice)
                {
                    TextColor = Color.Gray;
                    g.Draw(IconHolder.Icons.sprMenuLimitG, new Rectangle((int)(CurrentX + 390 * Ratio), (int)CurrentY, (int)(32 * Ratio), (int)(32 * Ratio)), Color.White);
                }

                g.DrawString(Map.fntMenuText, "Level 5", new Vector2((int)(CurrentX + 10 * Ratio), CurrentY), TextColor);
                g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)(CurrentX + 280 * Ratio), (int)CurrentY, (int)(14 * Ratio), (int)(23 * Ratio)), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, UpgradePrice.ToString(), new Vector2((int)(CurrentX + 380 * Ratio), CurrentY), TextColor);

                CurrentY += Map.fntMenuText.LineSpacing;
            }

            g.DrawString(Map.fntMenuText, "Return", new Vector2(CurrentX + 10, CurrentY), SorcererStreetMap.TextColor);

            MenuHelper.DrawFingerIcon(g, new Vector2(InfoBoxX - 20, (int)(InfoBoxY + 44 * Ratio + Map.fntMenuText.LineSpacing * ActionMenuCursor)));
        }
    }
}
