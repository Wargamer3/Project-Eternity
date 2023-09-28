using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelPlayerDefault : ActionPanelSorcererStreet
    {
        private const string PanelName = "PlayerDefault";

        int OriginalPlayerIndex;

        public ActionPanelPlayerDefault(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
            OriginalPlayerIndex = Map.ActivePlayerIndex;
        }

        public override void OnSelect()
        {
            //Reset the cursor.
            if (GameScreen.FMODSystem != null && GameScreen.FMODSystem.sndActiveBGMName != Map.sndBattleThemeName && !string.IsNullOrEmpty(Map.sndBattleThemeName))
            {
                 Map.sndBattleTheme.Stop();
                 Map.sndBattleTheme.SetLoop(true);
                Map.sndBattleTheme.PlayAsBGM();
                GameScreen.FMODSystem.sndActiveBGMName = Map.sndBattleThemeName;
            }
            
            Map.ActivePlayerIndex++;

            if (Map.ActivePlayerIndex >= Map.ListPlayer.Count)
            {
                Map.OnNewTurn();
            }

            Map.ListPassedTerrein.Clear();
            DeleteBattleInformation();
        }

        private void DeleteBattleInformation()
        {
            Map.GlobalSorcererStreetBattleContext.ListBattlePanelHolder = null;

            if (Map.GlobalSorcererStreetBattleContext.Invader != null)
            {
                Map.GlobalSorcererStreetBattleContext.Invader.Owner = null;
                Map.GlobalSorcererStreetBattleContext.Invader.Animation = null;
            }
            if (Map.GlobalSorcererStreetBattleContext.Defender != null)
            {
                Map.GlobalSorcererStreetBattleContext.Defender.Owner = null;
                Map.GlobalSorcererStreetBattleContext.Defender.Animation = null;
            }

            Map.GlobalSorcererStreetBattleContext.Invader = null;
            Map.GlobalSorcererStreetBattleContext.Defender = null;

            Map.GlobalSorcererStreetBattleContext.DefenderTerrain = null;

            Map.GlobalSorcererStreetBattleContext.Background = null;
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
            AddToPanelListAndSelect(new ActionPanelDrawCardPhase(Map, Map.ActivePlayerIndex));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            OriginalPlayerIndex = BR.ReadInt32();
            int NextPlayerIndex = BR.ReadInt32();
            Map.ActivePlayerIndex = OriginalPlayerIndex;

            OnSelect();

            Map.ActivePlayerIndex = NextPlayerIndex;
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(OriginalPlayerIndex);
            BW.AppendInt32(Map.ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlayerDefault(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int BoxHeight = Constants.Height / 6;

            int BoxPostion = Constants.Height / 10;
            for (int P = 0; P < Map.ListPlayer.Count; ++P)
            {
                if (Map.ListPlayer[P].Inventory == null)
                    continue;

                DrawPlayerInformation(g, Map, Map.ListPlayer[P], Constants.Width / 16, BoxPostion);
                BoxPostion += BoxHeight;
            }

            int BoxWidth = Constants.Width / 10;
            BoxHeight = Constants.Height / 10;
            float X = (Constants.Width - BoxWidth) / 2;
            float Y = Constants.Height * 0.8f;
            MenuHelper.DrawBorderlessBox(g, new Vector2(X, Y), BoxWidth, BoxHeight);
            MenuHelper.DrawConfirmIcon(g, new Vector2(X + BoxWidth - 45, Y + BoxHeight - 50));

            X += 7;
            //Draw Round + round number
            g.DrawString(Map.fntArial12, "Round " + Map.GameTurn, new Vector2(X + 30, Y + 15), Color.White);

            Y += 25;
            //Draw Player Name's turn
            g.DrawString(Map.fntArial12, Map.ListPlayer[Map.ActivePlayerIndex].Name + "'s turn", new Vector2(X + 30, Y + 15), Color.White);
        }

        public static void DrawPlayerInformation(CustomSpriteBatch g, SorcererStreetMap Map, Player ActivePlayer, float X, float Y)
        {
            int BoxWidth = (int)(Constants.Width / 3);
            int BoxHeight = (Constants.Height / 9);
            int IconWidth = Constants.Width / 112;
            int IconHeight = Constants.Width / 60;
            int LineHeight = IconHeight + 5;

            MenuHelper.DrawBox(g, new Vector2(X, Y), BoxWidth, BoxHeight);

            X += 7;
            //Draw Player name
            g.DrawString(Map.fntArial12, ActivePlayer.Name, new Vector2(X, Y + 5), Color.White);

            Y += LineHeight;
            //Draw Player Magic
            g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)X, (int)Y, IconWidth, IconHeight), Color.White);
            g.DrawString(Map.fntArial12, ActivePlayer.Gold.ToString(), new Vector2(X + 20, Y), Color.White);

            //Draw Player Total Magic
            g.Draw(Map.Symbols.sprMenuTG, new Rectangle((int)X + 60, (int)Y, IconWidth, IconHeight), Color.White);
            g.DrawString(Map.fntArial12, ActivePlayer.TotalMagic.ToString(), new Vector2(X + 80, Y), Color.White);

            Y += LineHeight;
            //Draw Player color and it's position
            //Position if based on the number of checkpoints and then player order
            if (ActivePlayer.Color == Color.Red)
            {
                if (ActivePlayer.Rank == 1)
                {
                    g.Draw(Map.sprPlayerRed1, new Rectangle((int)X, (int)Y, IconHeight, IconHeight), Color.White);
                }
                else
                {
                    g.Draw(Map.sprPlayerRed2, new Rectangle((int)X, (int)Y, IconHeight, IconHeight), Color.White);
                }
            }
            else if (ActivePlayer.Color == Color.Blue)
            {
                if (ActivePlayer.Rank == 1)
                {
                    g.Draw(Map.sprPlayerBlue1, new Rectangle((int)X, (int)Y, IconHeight, IconHeight), Color.White);
                }
                else
                {
                    g.Draw(Map.sprPlayerBlue2, new Rectangle((int)X, (int)Y, IconHeight, IconHeight), Color.White);
                }
            }
            else
            {
                g.Draw(Map.sprPlayerBackground, new Rectangle((int)X, (int)Y, IconHeight, IconHeight), ActivePlayer.Color);
                g.DrawStringCentered(Map.fntArial12, ActivePlayer.Rank.ToString(), new Vector2(X + IconHeight / 2, Y + IconHeight / 2), Color.White);
            }

            for (int C = 0; C < Map.ListCheckpoint.Count; C++)
            {
                SorcererStreetMap.Checkpoints ActiveCheckpoint = Map.ListCheckpoint[C];
                if (ActivePlayer.ListPassedCheckpoint.Contains(ActiveCheckpoint))
                {
                    switch (ActiveCheckpoint)
                    {
                        case SorcererStreetMap.Checkpoints.North:
                            g.Draw(Map.sprDirectionNorthFilled, new Rectangle((int)X + 60 + C * IconHeight, (int)Y, IconHeight, IconHeight), Color.White);
                            break;
                        case SorcererStreetMap.Checkpoints.South:
                            g.Draw(Map.sprDirectionSouthFilled, new Rectangle((int)X + 60 + C * IconHeight, (int)Y, IconHeight, IconHeight), Color.White);
                            break;
                        case SorcererStreetMap.Checkpoints.East:
                            g.Draw(Map.sprDirectionEastFilled, new Rectangle((int)X + 60 + C * IconHeight, (int)Y, IconHeight, IconHeight), Color.White);
                            break;
                        case SorcererStreetMap.Checkpoints.West:
                            g.Draw(Map.sprDirectionWestFilled, new Rectangle((int)X + 60 + C * IconHeight, (int)Y, IconHeight, IconHeight), Color.White);
                            break;
                    }
                }
                else
                {
                    switch (ActiveCheckpoint)
                    {
                        case SorcererStreetMap.Checkpoints.North:
                            g.Draw(Map.sprDirectionNorth, new Rectangle((int)X + 60 + C * IconHeight, (int)Y, IconHeight, IconHeight), Color.White);
                            break;
                        case SorcererStreetMap.Checkpoints.South:
                            g.Draw(Map.sprDirectionSouth, new Rectangle((int)X + 60 + C * IconHeight, (int)Y, IconHeight, IconHeight), Color.White);
                            break;
                        case SorcererStreetMap.Checkpoints.East:
                            g.Draw(Map.sprDirectionEast, new Rectangle((int)X + 60 + C * IconHeight, (int)Y, IconHeight, IconHeight), Color.White);
                            break;
                        case SorcererStreetMap.Checkpoints.West:
                            g.Draw(Map.sprDirectionWest, new Rectangle((int)X + 60 + C * IconHeight, (int)Y, IconHeight, IconHeight), Color.White);
                            break;
                    }
                }
            }
        }

        public static void DrawLandInformation(CustomSpriteBatch g, SorcererStreetMap Map, TerrainSorcererStreet HoverTerrain, float X, float Y)
        {
            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 5);

            MenuHelper.DrawNamedBox(g, "Land Information", new Vector2(X, Y), BoxWidth, BoxHeight);

            float CurrentX = X + 50;
            float CurrentY = Y - 10;

            float LineHight = Constants.Height / 30;

            CurrentY += 20;

            g.DrawString(Map.fntArial12, "Owner: " + "None", new Vector2(CurrentX, CurrentY), Color.White);
            CurrentY += LineHight;
            g.DrawString(Map.fntArial12, "Value: " + HoverTerrain.CurrentValue, new Vector2(CurrentX, CurrentY), Color.White);
            CurrentY += LineHight;
            g.DrawString(Map.fntArial12, "Toll: " + HoverTerrain.CurrentToll, new Vector2(CurrentX, CurrentY), Color.White);
            CurrentY += LineHight;
            g.DrawString(Map.fntArial12, "Level: " + HoverTerrain.LandLevel, new Vector2(CurrentX, CurrentY), Color.White);

            switch (Map.ListTerrainType[HoverTerrain.TerrainTypeIndex])
            {
                case TerrainSorcererStreet.FireElement:
                    g.Draw(Map.Symbols.sprElementFire, new Vector2((int)X + 110, (int)CurrentY), Color.White);
                    break;
                case TerrainSorcererStreet.WaterElement:
                    g.Draw(Map.Symbols.sprElementWater, new Vector2((int)X + 110, (int)CurrentY), Color.White);
                    break;
                case TerrainSorcererStreet.EarthElement:
                    g.Draw(Map.Symbols.sprElementEarth, new Vector2((int)X + 110, (int)CurrentY), Color.White);
                    break;
                case TerrainSorcererStreet.AirElement:
                    g.Draw(Map.Symbols.sprElementAir, new Vector2((int)X + 110, (int)CurrentY), Color.White);
                    break;
            }
        }
    }
}
