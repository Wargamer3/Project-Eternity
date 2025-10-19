using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelTowerCommands : ActionPanelSorcererStreet
    {
        private const string PanelName = "TowerCommands";

        private enum Commands { Territory, Map, Info, Options, Help, End }

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private byte TerrainTypeIndex;
        private int MovementRemaining;

        private double ItemAnimationTime;

        public ActionPanelTowerCommands(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelTowerCommands(SorcererStreetMap Map, int ActivePlayerIndex, byte TerrainTypeIndex, int MovementRemaining)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.TerrainTypeIndex = TerrainTypeIndex;
            this.MovementRemaining = MovementRemaining;

            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            switch (Map.ListTerrainType[TerrainTypeIndex])
            {
                case TerrainSorcererStreet.EastTower:
                    ActivePlayer.ListPassedCheckpoint.Add(SorcererStreetMap.Checkpoints.East);
                    break;
                case TerrainSorcererStreet.WestTower:
                    ActivePlayer.ListPassedCheckpoint.Add(SorcererStreetMap.Checkpoints.West);
                    break;
                case TerrainSorcererStreet.SouthTower:
                    ActivePlayer.ListPassedCheckpoint.Add(SorcererStreetMap.Checkpoints.South);
                    break;
                case TerrainSorcererStreet.NorthTower:
                    ActivePlayer.ListPassedCheckpoint.Add(SorcererStreetMap.Checkpoints.North);
                    break;
            }
            ActivePlayer.Gold += Map.TowerMagicGain;
            Map.DicTeam[ActivePlayer.TeamIndex].TotalMagic += Map.TowerMagicGain;
            Map.UpdatePlayersRank();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ItemAnimationTime < 1)
            {
                ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (ItemAnimationTime < 5)
            {
                ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                RemoveFromPanelList(this);

                if (MovementRemaining <= 0)
                {
                    AddToPanelListAndSelect(new ActionPanelTerritoryMenuPhase(Map, ActivePlayerIndex, true));
                }
            }

            if (InputHelper.InputConfirmPressed())
            {
                RemoveFromPanelList(this);

                if (MovementRemaining <= 0)
                {
                    AddToPanelListAndSelect(new ActionPanelTerritoryMenuPhase(Map, ActivePlayerIndex, true));
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelTowerCommands(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            MenuHelper.DrawDiceHolder(g, new Vector2(Constants.Width / 8, Constants.Height / 4), MovementRemaining);

            if (ItemAnimationTime < 1)
            {
                DrawTowerSymbol(g);
            }
            else if (ItemAnimationTime < 5)
            {
                ItemAnimationTime = 2f;
                int IconWidth = (int)(32 * 1.5);
                int IconHeight = (int)(32 * 1.5);
                int BoxWidth = 650;
                int BoxHeight = 150;
                int BoxX = (Constants.Width - BoxWidth) / 2;
                int BoxY = Constants.Height / 2 - BoxHeight / 2;
                MenuHelper.DrawBorderlessBox(g, new Vector2(BoxX, BoxY), BoxWidth, BoxHeight);

                g.DrawString(Map.fntMenuText, "Passed checkpoint", new Vector2(BoxX + 60, BoxY + 30), Color.White);
                switch (Map.ListTerrainType[TerrainTypeIndex])
                {
                    case TerrainSorcererStreet.EastTower:
                        g.Draw(Map.sprDirectionEastFilled, new Rectangle(BoxX + 380, BoxY + 28, IconWidth, IconHeight), Color.White);
                        break;
                    case TerrainSorcererStreet.WestTower:
                        g.Draw(Map.sprDirectionWestFilled, new Rectangle(BoxX + 380, BoxY + 28, IconWidth, IconHeight), Color.White);
                        break;
                    case TerrainSorcererStreet.SouthTower:
                        g.Draw(Map.sprDirectionSouthFilled, new Rectangle(BoxX + 380, BoxY + 28, IconWidth, IconHeight), Color.White);
                        break;
                    case TerrainSorcererStreet.NorthTower:
                        g.Draw(Map.sprDirectionNorthFilled, new Rectangle(BoxX + 380, BoxY + 28, IconWidth, IconHeight), Color.White);
                        break;
                }

                g.DrawString(Map.fntMenuText, "Earned fort bonus of " + 100 + "G", new Vector2(BoxX + 60, BoxY + Map.fntMenuText.LineSpacing + 40), Color.White);

                MenuHelper.DrawConfirmIcon(g, new Vector2(BoxX + BoxWidth - 70, BoxY + BoxHeight - 80));
            }
            else
            {
                for (int P = 0; P < ListNextChoice.Count; ++P)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle(P * 80 + 10, Constants.Height - 150, 70, 100), Color.Green);
                }
            }
        }

        private void DrawTowerSymbol(CustomSpriteBatch g)
        {
            int Size = Constants.Width / 4;
            int RingSize = Constants.Width / 5;
            int PosX = Constants.Width / 2;
            int PosY = Constants.Height / 2;

            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            Color FinalColor = Color.FromNonPremultiplied(255, 255, 255, (int)(Math.Sin(ItemAnimationTime * MathHelper.Pi) * 255));

            g.Draw(Map.sprTowerPopupRing, new Rectangle(PosX, PosY, RingSize, RingSize), null, FinalColor,
                0f, new Vector2(Map.sprTowerPopupRing.Width, Map.sprTowerPopupRing.Height), SpriteEffects.None, 0f);

            g.Draw(Map.sprTowerPopupRing, new Rectangle(PosX, PosY, RingSize, RingSize), null, FinalColor,
                0f, new Vector2(0, Map.sprTowerPopupRing.Height), SpriteEffects.FlipHorizontally, 0f);

            g.Draw(Map.sprTowerPopupRing, new Rectangle(PosX, PosY, RingSize, RingSize), null, FinalColor,
                0f, new Vector2(Map.sprTowerPopupRing.Width, 0), SpriteEffects.FlipVertically, 0f);

            g.Draw(Map.sprTowerPopupRing, new Rectangle(PosX, PosY, RingSize, RingSize), null, FinalColor,
                0f, new Vector2(0, 0), SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0f);

            g.Draw(Map.sprTowerPopupBackground, new Rectangle(PosX, PosY, Size, Size), null, FinalColor,
                0f, new Vector2(Map.sprTowerPopupBackground.Width / 2, Map.sprTowerPopupBackground.Height / 2), SpriteEffects.None, 0f);

            switch (Map.ListTerrainType[TerrainTypeIndex])
            {
                case TerrainSorcererStreet.EastTower:
                    g.Draw(Map.sprTowerPopupEast, new Rectangle(PosX, PosY, Size, Size), null, FinalColor,
                        0f, new Vector2(Map.sprTowerPopupEast.Width / 2, Map.sprTowerPopupEast.Height / 2), SpriteEffects.None, 0f);
                    break;
                case TerrainSorcererStreet.WestTower:
                    g.Draw(Map.sprTowerPopupWest, new Rectangle(PosX, PosY, Size, Size), null, FinalColor,
                        0f, new Vector2(Map.sprTowerPopupWest.Width / 2, Map.sprTowerPopupWest.Height / 2), SpriteEffects.None, 0f);
                    break;
                case TerrainSorcererStreet.SouthTower:
                    g.Draw(Map.sprTowerPopupSouth, new Rectangle(PosX, PosY, Size, Size), null, FinalColor,
                        0f, new Vector2(Map.sprTowerPopupSouth.Width / 2, Map.sprTowerPopupSouth.Height / 2), SpriteEffects.None, 0f);
                    break;
                case TerrainSorcererStreet.NorthTower:
                    g.Draw(Map.sprTowerPopupNorth, new Rectangle(PosX, PosY, Size, Size), null, FinalColor,
                        0f, new Vector2(Map.sprTowerPopupNorth.Width / 2, Map.sprTowerPopupNorth.Height / 2), SpriteEffects.None, 0f);
                    break;
            }

            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        }
    }
}
