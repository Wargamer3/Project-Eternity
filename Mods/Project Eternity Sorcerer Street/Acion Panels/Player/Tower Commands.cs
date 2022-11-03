using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using System;

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
            AddChoiceToCurrentPanel(new ActionPanelChooseTerritory(Map, ActivePlayerIndex));
            AddChoiceToCurrentPanel(new ActionPanelViewMap(Map));
            AddChoiceToCurrentPanel(new ActionPanelInfo(Map));
            AddChoiceToCurrentPanel(new ActionPanelOptions(Map));
            AddChoiceToCurrentPanel(new ActionPanelHelp(Map));
            AddChoiceToCurrentPanel(new ActionPanelEndPlayerPhase(Map));
            ActivePlayer.Magic += Map.TowerMagicGain;
            ActivePlayer.TotalMagic += Map.TowerMagicGain;
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
                if (MovementRemaining == 0)
                {
                    Map.EndPlayerPhase();
                }
            }

            if (InputHelper.InputConfirmPressed())
            {
                RemoveFromPanelList(this);
                if (MovementRemaining == 0)
                {
                    Map.EndPlayerPhase();
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
            GameScreen.DrawBox(g, new Vector2(30, 30), 50, 50, Color.Black);
            g.DrawString(Map.fntArial12, MovementRemaining.ToString(), new Vector2(37, 35), Color.White);

            if (ItemAnimationTime < 1)
            {
                int Size = Constants.Width / 4;
                int PosX = Constants.Width / 2 - Size / 2;
                int PosY = Constants.Height / 2 - Size / 2;

                switch (Map.ListTerrainType[TerrainTypeIndex])
                {
                    case TerrainSorcererStreet.EastTower:
                        g.Draw(Map.sprDirectionEastFilled, new Rectangle(PosX, PosY + 5, Size, Size), Color.FromNonPremultiplied(255, 255, 255, (int)(ItemAnimationTime * 255)));
                        break;
                    case TerrainSorcererStreet.WestTower:
                        g.Draw(Map.sprDirectionWestFilled, new Rectangle(PosX, PosY + 5, Size, Size), Color.FromNonPremultiplied(255, 255, 255, (int)(ItemAnimationTime * 255)));
                        break;
                    case TerrainSorcererStreet.SouthTower:
                        g.Draw(Map.sprDirectionSouthFilled, new Rectangle(PosX, PosY + 5, Size, Size), Color.FromNonPremultiplied(255, 255, 255, (int)(Math.Sin(ItemAnimationTime * MathHelper.Pi) * 255)));
                        break;
                    case TerrainSorcererStreet.NorthTower:
                        g.Draw(Map.sprDirectionNorthFilled, new Rectangle(PosX, PosY + 5, Size, Size), Color.FromNonPremultiplied(255, 255, 255, (int)(ItemAnimationTime * 255)));
                        break;
                }
            }
            else if (ItemAnimationTime < 5)
            {
                int BoxX = (int)(Constants.Width / 3.2);
                int BoxY = Constants.Height - Constants.Height / 2;
                GameScreen.DrawBox(g, new Vector2(BoxX, BoxY), Constants.Width - (int)(Constants.Width / 1.6), 50, Color.White);
                g.DrawString(Map.fntArial12, "Passed checkpoint", new Vector2(BoxX + 15, BoxY + 5), Color.White);
                switch (Map.ListTerrainType[TerrainTypeIndex])
                {
                    case TerrainSorcererStreet.EastTower:
                        g.Draw(Map.sprDirectionEastFilled, new Rectangle(BoxX + 155, BoxY + 5, 18, 18), Color.White);
                        break;
                    case TerrainSorcererStreet.WestTower:
                        g.Draw(Map.sprDirectionWestFilled, new Rectangle(BoxX + 155, BoxY + 5, 18, 18), Color.White);
                        break;
                    case TerrainSorcererStreet.SouthTower:
                        g.Draw(Map.sprDirectionSouthFilled, new Rectangle(BoxX + 155, BoxY + 5, 18, 18), Color.White);
                        break;
                    case TerrainSorcererStreet.NorthTower:
                        g.Draw(Map.sprDirectionNorthFilled, new Rectangle(BoxX + 155, BoxY + 5, 18, 18), Color.White);
                        break;
                }

                g.DrawString(Map.fntArial12, "Earned fort bonus of " + 100 + "G", new Vector2(BoxX + 15, BoxY + 25), Color.White);
                g.Draw(Map.sprMenuHand, new Vector2(Constants.Width / 2 + 100, BoxY + 15), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            }
            else
            {
                for (int P = 0; P < ListNextChoice.Count; ++P)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle(P * 80 + 10, Constants.Height - 150, 70, 100), Color.Green);
                }
            }
        }
    }
}
