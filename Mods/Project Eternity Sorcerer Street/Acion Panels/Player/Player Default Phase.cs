using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelPlayerDefault : ActionPanelSorcererStreet
    {
        private const string PanelName = "PlayerDefault";

        private int ActivePlayerIndex;

        public ActionPanelPlayerDefault(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelPlayerDefault(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                ConfirmStartOfTurn();
            }
        }

        public void ConfirmStartOfTurn()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelDrawCardPhase(Map, ActivePlayerIndex));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlayerDefault(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int BoxHeight = 70;

            int BoxPostion = Constants.Height / 20;
            for (int P = 0; P < Map.ListPlayer.Count; ++P)
            {
                if (Map.ListPlayer[P].Inventory == null)
                    continue;

                DrawPlayerInformation(g, Map, Map.ListPlayer[P], 30, BoxPostion);
                BoxPostion += BoxHeight;
            }

            int BoxWidth = 110;
            BoxHeight = 55;
            float X = (Constants.Width - BoxWidth) / 2;
            float Y = Constants.Height * 0.8f;
            GameScreen.DrawBox(g, new Vector2(X, Y), BoxWidth, BoxHeight, Color.White);
            g.Draw(Map.sprMenuHand, new Vector2((int)X + BoxWidth - 23, (int)Y - Map.sprMenuHand.Height + BoxHeight + 8), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

            X += 7;
            //Draw Round + round number
            g.DrawString(Map.fntArial12, "Round " + Map.GameTurn, new Vector2(X, Y + 5), Color.White);

            Y += 25;
            //Draw Player Name's turn
            g.DrawString(Map.fntArial12, Map.ListPlayer[Map.ActivePlayerIndex].Name + "'s turn", new Vector2(X, Y + 5), Color.White);
        }

        public static void DrawPlayerInformation(CustomSpriteBatch g, SorcererStreetMap Map, Player ActivePlayer, float X, float Y)
        {
            int BoxHeight = 70;

            GameScreen.DrawBox(g, new Vector2(X, Y), 150, BoxHeight, Color.White);

            X += 7;
            //Draw Player name
            g.DrawString(Map.fntArial12, ActivePlayer.Name, new Vector2(X, Y + 5), Color.White);

            Y += 25;
            //Draw Player Magic
            g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)X, (int)Y, 18, 18), Color.White);
            g.DrawString(Map.fntArial12, ActivePlayer.Magic.ToString(), new Vector2(X + 20, Y), Color.White);

            //Draw Player Total Magic
            g.Draw(Map.Symbols.sprMenuTG, new Rectangle((int)X + 60, (int)Y, 18, 18), Color.White);
            g.DrawString(Map.fntArial12, ActivePlayer.TotalMagic.ToString(), new Vector2(X + 80, Y), Color.White);

            Y += 20;
            //Draw Player color and it's position
            //Position if based on the number of checkpoints and then player order
            g.Draw(Map.sprPlayerBackground, new Rectangle((int)X, (int)Y, 18, 18), ActivePlayer.Color);
            g.DrawStringCentered(Map.fntArial12, ActivePlayer.Rank.ToString(), new Vector2(X + 8, Y + 9), Color.White);

            for (int C = 0; C < Map.ListCheckpoint.Count; C++)
            {
                SorcererStreetMap.Checkpoints ActiveCheckpoint = Map.ListCheckpoint[C];
                if (ActivePlayer.ListPassedCheckpoint.Contains(ActiveCheckpoint))
                {
                    g.Draw(Map.sprDirectionSouthFilled, new Rectangle((int)X + 60 + C * 20, (int)Y, 18, 18), Color.White);
                }
                else
                {
                    g.Draw(Map.sprDirectionSouth, new Rectangle((int)X + 60 + C * 20, (int)Y, 18, 18), Color.White);
                }
            }
        }

        public static void DrawLandInformation(CustomSpriteBatch g, SorcererStreetMap Map, TerrainSorcererStreet HoverTerrain)
        {
            float InfoBoxX = Constants.Width / 12f;
            float InfoBoxY = Constants.Height / 10;
            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 5);

            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY - 20), BoxWidth, 20, Color.White);
            g.DrawString(Map.fntArial12, "Land Information", new Vector2(InfoBoxX + 10, InfoBoxY - 20), Color.White);
            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight, Color.White);

            float CurrentX = InfoBoxX + 10;
            float CurrentY = InfoBoxY - 10;

            CurrentY += 20;

            g.DrawString(Map.fntArial12, "Owner: " + "None", new Vector2(CurrentX, CurrentY), Color.White);
            CurrentY += 20;
            g.DrawString(Map.fntArial12, "Value: " + HoverTerrain.CurrentValue, new Vector2(CurrentX, CurrentY), Color.White);
            CurrentY += 20;
            g.DrawString(Map.fntArial12, "Toll: " + HoverTerrain.CurrentToll, new Vector2(CurrentX, CurrentY), Color.White);
            CurrentY += 20;
            g.DrawString(Map.fntArial12, "Level: " + HoverTerrain.LandLevel, new Vector2(CurrentX, CurrentY), Color.White);

            switch (Map.ListTerrainType[HoverTerrain.TerrainTypeIndex])
            {
                case TerrainSorcererStreet.FireElement:
                    g.Draw(Map.Symbols.sprElementFire, new Vector2((int)InfoBoxX + 70, (int)CurrentY), Color.White);
                    break;
                case TerrainSorcererStreet.WaterElement:
                    g.Draw(Map.Symbols.sprElementWater, new Vector2((int)InfoBoxX + 70, (int)CurrentY), Color.White);
                    break;
                case TerrainSorcererStreet.EarthElement:
                    g.Draw(Map.Symbols.sprElementEarth, new Vector2((int)InfoBoxX + 70, (int)CurrentY), Color.White);
                    break;
                case TerrainSorcererStreet.AirElement:
                    g.Draw(Map.Symbols.sprElementAir, new Vector2((int)InfoBoxX + 70, (int)CurrentY), Color.White);
                    break;
            }
        }
    }
}
