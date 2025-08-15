using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

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
            if (GameScreen.FMODSystem != null && GameScreen.FMODSystem.sndActiveBGMName != Map.sndBattleThemePath && !string.IsNullOrEmpty(Map.sndBattleThemePath))
            {
                 Map.sndBattleTheme.Stop();
                 Map.sndBattleTheme.SetLoop(true);
                Map.sndBattleTheme.PlayAsBGM();
                GameScreen.FMODSystem.sndActiveBGMName = Map.sndBattleThemePath;
            }

            do
            {
                Map.ActivePlayerIndex++;

                if (Map.ActivePlayerIndex >= Map.ListPlayer.Count)
                {
                    OnNewTurn(Map);
                }

                if (Map.ListPlayer[Map.ActivePlayerIndex].OnlinePlayerType == OnlinePlayerBase.PlayerTypeNA)
                {
                    continue;
                }

                Map.GameRule.OnNewTurn(Map.ActivePlayerIndex);

                UpdateProps(Map, Map.ActivePlayerIndex);
                UpdateDelayedAttacks(Map, Map.ActivePlayerIndex);
                UpdatePERAttacks(Map, Map.ActivePlayerIndex);
                UpdateSquadMovement(Map, Map.ActivePlayerIndex);

                foreach (Player ActivePlayer in Map.ListPlayer)
                {
                    if (ActivePlayer.OnlinePlayerType == OnlinePlayerBase.PlayerTypeNA)
                    {
                        continue;
                    }

                    if (ActivePlayer.Enchant != null)
                    {
                        EnchantHelper.UpdateLifetime(ActivePlayer, SkillEffect.LifetimeTypeTurns + Map.ActivePlayerIndex);
                    }
                }
            }
            while (Map.ListPlayer[Map.ActivePlayerIndex].TeamIndex < 0);

            Map.ListPassedTerrein.Clear();
            DeleteBattleInformation();
        }

        public static void OnNewTurn(SorcererStreetMap Map)
        {
            Map.ActivePlayerIndex = 0;
            Map.GameTurn++;

            Map.UpdateMapEvent(SorcererStreetMap.EventTypeTurn, 0);

            foreach (Player ActivePlayer in Map.ListPlayer)
            {
                if (ActivePlayer.OnlinePlayerType == OnlinePlayerBase.PlayerTypeNA)
                {
                    continue;
                }

                ActivePlayer.GamePiece.StartTurn();
                ActivePlayer.GamePiece.EndTurn();
            }
        }

        public static void UpdateDelayedAttacks(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            for (int A = Map.ListDelayedAttack.Count - 1; A >= 0; --A)
            {
                DelayedAttack ActiveDelayedAttack = Map.ListDelayedAttack[A];

                if (ActiveDelayedAttack.PlayerIndex == ActivePlayerIndex)
                {
                    if (--ActiveDelayedAttack.TurnsRemaining == 0)
                    {
                        Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelInitDelayedAttackMAP(Map, ActivePlayerIndex, ActiveDelayedAttack));

                        Map.ListDelayedAttack.RemoveAt(A);
                    }
                }
            }
        }

        public static void UpdatePERAttacks(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            for (int A = Map.ListPERAttack.Count - 1; A >= 0; --A)
            {
                PERAttack ActivePERAttack = Map.ListPERAttack[A];

                if (ActivePERAttack.PlayerIndex == ActivePlayerIndex)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelUpdatePERAttacks(Map));
                    return;
                }
            }
        }

        public static void UpdateSquadMovement(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            if (Map.ListPlayer[ActivePlayerIndex].GamePiece.Speed != Vector3.Zero)
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAutoMove(Map));
                return;
            }

            for (int S = Map.ListPlayer[ActivePlayerIndex].ListCreatureOnBoard.Count - 1; S >= 0; --S)
            {
                if (Map.ListPlayer[ActivePlayerIndex].ListCreatureOnBoard[S].Speed != Vector3.Zero)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAutoMove(Map));
                    return;
                }
            }
        }

        public static void UpdateProps(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            foreach (MapLayer ActiveLayer in Map.LayerManager.ListLayer)
            {
                foreach (InteractiveProp ActiveProp in ActiveLayer.ListProp)
                {
                    ActiveProp.OnTurnEnd(ActivePlayerIndex);
                }
            }
        }

        public static void UpdateHoldableItems(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            foreach (MapLayer ActiveLayer in Map.LayerManager.ListLayer)
            {
                foreach (HoldableItem ActiveProp in ActiveLayer.ListHoldableItem)
                {
                    ActiveProp.OnTurnEnd(null, ActivePlayerIndex);
                }
            }
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
            int BoxHeight = 200;

            int BoxPostion = 100;
            for (int P = 0; P < Map.ListPlayer.Count; ++P)
            {
                if (Map.ListPlayer[P].Inventory == null)
                    continue;

                DrawPlayerInformation(g, Map, Map.ListPlayer[P], 160, BoxPostion);
                BoxPostion += BoxHeight;
            }

            int BoxWidth = 400;

            //Draw Player Name's turn
            System.Collections.Generic.List<string> ListLine = TextHelper.FitToWidth(Map.fntMenuText, Map.ListPlayer[Map.ActivePlayerIndex].Name + "'s turn", BoxWidth);

            BoxHeight = 62 + Map.fntMenuText.LineSpacing + Map.fntMenuText.LineSpacing * ListLine.Count;
            float X = (Constants.Width - BoxWidth) / 2;
            float Y = 830 - BoxHeight / 2;
            MenuHelper.DrawBorderlessBox(g, new Vector2(X, Y), BoxWidth, BoxHeight);
            MenuHelper.DrawConfirmIcon(g, new Vector2(X + BoxWidth - 70, Y + BoxHeight - 80));

            X += 70;
            Y += 26;
            //Draw Round + round number
            g.DrawString(Map.fntMenuText, "Round " + Map.GameTurn, new Vector2(X, Y), SorcererStreetMap.TextColor);

            Y += Map.fntMenuText.LineSpacing;
            TextHelper.DrawTextMultiline(g, Map.fntMenuText, ListLine, TextHelper.TextAligns.Left, X + BoxWidth / 2, Y, BoxWidth, SorcererStreetMap.TextColor);
        }

        public static void DrawPhase(CustomSpriteBatch g, SorcererStreetMap Map, string PhaseName)
        {
            int ActionInfoBoxX = 160;
            int ActionInfoBoxY = 446;
            int ActionInfoBoxWidth = 620;
            int ActionInfoBoxHeight = 90;
            MenuHelper.DrawBorderlessBox(g, new Vector2(ActionInfoBoxX, ActionInfoBoxY), ActionInfoBoxWidth, ActionInfoBoxHeight);
            g.DrawStringCentered(Map.fntMenuText, PhaseName, new Vector2(ActionInfoBoxX + ActionInfoBoxWidth / 2, ActionInfoBoxY + ActionInfoBoxHeight / 2 + 6), SorcererStreetMap.TextColor);
        }

        public static void DrawPlayerInformation(CustomSpriteBatch g, SorcererStreetMap Map, Player ActivePlayer)
        {
            DrawPlayerInformation(g, Map, ActivePlayer, 140, 100);
        }

        public static void DrawPlayerInformation(CustomSpriteBatch g, SorcererStreetMap Map, Player ActivePlayer, float X, float Y)
        {
            int BoxWidth = 530;
            int BoxHeight = 180;
            int IconWidth = (int)(17 * 1.5);
            int IconHeight = (int)(32 * 1.5);
            int LineHeight = Map.fntMenuText.LineSpacing;

            MenuHelper.DrawBox(g, new Vector2(X, Y), BoxWidth, BoxHeight);

            X += 40;
            Y += 14;
            //Draw Player name
            g.DrawString(Map.fntMenuText, ActivePlayer.Name, new Vector2(X, Y), SorcererStreetMap.TextColor);

            Y += LineHeight + 6;
            //Draw Player Magic
            g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)X, (int)Y, IconWidth, IconHeight), Color.White);
            g.DrawString(Map.fntMenuText, ActivePlayer.Gold.ToString(), new Vector2(X + 30, Y + 6), SorcererStreetMap.TextColor);

            //Draw Player Total Magic
            g.Draw(Map.Symbols.sprMenuTG, new Rectangle((int)X + 160, (int)Y, IconWidth, IconHeight), Color.White);
            g.DrawString(Map.fntMenuText, Map.DicTeam[ActivePlayer.TeamIndex].TotalMagic.ToString(), new Vector2(X + 190, Y + 6), SorcererStreetMap.TextColor);

            Y += LineHeight +8;
            //Draw Player color and it's position
            //Position if based on the number of checkpoints and then player order
            if (ActivePlayer.Color == Color.Red)
            {
                if (Map.DicTeam[ActivePlayer.TeamIndex].Rank == 1)
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
                if (Map.DicTeam[ActivePlayer.TeamIndex].Rank == 1)
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
                g.DrawStringCentered(Map.fntMenuText, Map.DicTeam[ActivePlayer.TeamIndex].Rank.ToString(), new Vector2(X + IconHeight / 2 - 2, Y + IconHeight / 2 + 4), SorcererStreetMap.TextColor);
            }

            X += 80;
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

        public static void DrawLandInformationTop(CustomSpriteBatch g, SorcererStreetMap Map, TerrainSorcererStreet HoverTerrain)
        {
            DrawLandInformation(g, Map, HoverTerrain, 160, 100);
        }

        public static void DrawLandInformationBottom(CustomSpriteBatch g, SorcererStreetMap Map, TerrainSorcererStreet HoverTerrain)
        {
            DrawLandInformation(g, Map, HoverTerrain, Constants.Width / 16f, Constants.Height - Constants.Height / 3.5f);
        }

        public static void DrawLandInformation(CustomSpriteBatch g, SorcererStreetMap Map, TerrainSorcererStreet HoverTerrain, float X, float Y)
        {
            int BoxWidth = 685;
            int BoxHeight = 236;
            int IconWidth = (int)(32 * 1.5);
            int IconHeight = (int)(32 * 1.5);

            MenuHelper.DrawNamedBox(g, "Land Information", new Vector2(X, Y), BoxWidth, BoxHeight);

            float CurrentX = X + 50;
            float CurrentY = Y + 18;

            float LineHight = 56;

            g.DrawString(Map.fntMenuText, "Owner: " + "None", new Vector2(CurrentX, CurrentY), Color.White);
            CurrentY += LineHight;
            g.DrawString(Map.fntMenuText, "Value: " + HoverTerrain.CurrentValue, new Vector2(CurrentX, CurrentY), Color.White);
            CurrentY += LineHight;
            g.DrawString(Map.fntMenuText, "Toll: " + HoverTerrain.CurrentToll, new Vector2(CurrentX, CurrentY), Color.White);
            CurrentY += LineHight;
            g.DrawString(Map.fntMenuText, "Level: " + HoverTerrain.LandLevel, new Vector2(CurrentX, CurrentY), Color.White);

            CurrentX = X + 180;
            switch (Map.ListTerrainType[HoverTerrain.TerrainTypeIndex])
            {
                case TerrainSorcererStreet.FireElement:
                    g.Draw(Map.Symbols.sprElementFire, new Rectangle((int)CurrentX, (int)CurrentY - 2, IconWidth, IconHeight), Color.White);
                    break;
                case TerrainSorcererStreet.WaterElement:
                    g.Draw(Map.Symbols.sprElementWater, new Rectangle((int)CurrentX, (int)CurrentY - 2, IconWidth, IconHeight), Color.White);
                    break;
                case TerrainSorcererStreet.EarthElement:
                    g.Draw(Map.Symbols.sprElementEarth, new Rectangle((int)CurrentX, (int)CurrentY - 2, IconWidth, IconHeight), Color.White);
                    break;
                case TerrainSorcererStreet.AirElement:
                    g.Draw(Map.Symbols.sprElementAir, new Rectangle((int)CurrentX, (int)CurrentY - 2, IconWidth, IconHeight), Color.White);
                    break;
            }
        }
    }
}
