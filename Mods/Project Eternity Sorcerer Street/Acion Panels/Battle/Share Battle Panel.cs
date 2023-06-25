using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class ActionPanelBattle : ActionPanelSorcererStreet
    {
        private const float ItemCardScale = 0.4f;

        protected static int InvaderHPBar;
        protected static int DefenderHPBar;
        protected static int InvaderSTBar;
        protected static int DefenderSTBar;

        public ActionPanelBattle(SorcererStreetMap Map, string PanelName)
            : base(PanelName, Map, false)
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            CanUpdate(gameTime, Map.GlobalSorcererStreetBattleContext);
        }

        public static bool CanUpdate(GameTime gameTime, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            if (InvaderHPBar > GlobalSorcererStreetBattleContext.InvaderFinalHP)
            {
                --InvaderHPBar;
                return false;
            }
            else if (InvaderSTBar > GlobalSorcererStreetBattleContext.InvaderFinalST)
            {
                --InvaderSTBar;
                return false;
            }
            else if (DefenderHPBar > GlobalSorcererStreetBattleContext.DefenderFinalHP)
            {
                --DefenderHPBar;
                return false;
            }
            else if (DefenderSTBar > GlobalSorcererStreetBattleContext.DefenderFinalST)
            {
                --DefenderSTBar;
                return false;
            }

            if (InvaderHPBar < GlobalSorcererStreetBattleContext.InvaderFinalHP)
            {
                ++InvaderHPBar;
                return false;
            }
            else if (InvaderSTBar < GlobalSorcererStreetBattleContext.InvaderFinalST)
            {
                ++InvaderSTBar;
                return false;
            }
            else if (DefenderHPBar < GlobalSorcererStreetBattleContext.DefenderFinalHP)
            {
                ++DefenderHPBar;
                return false;
            }
            else if (DefenderSTBar < GlobalSorcererStreetBattleContext.DefenderFinalST)
            {
                ++DefenderSTBar;
                return false;
            }

            if (GlobalSorcererStreetBattleContext.InvaderCard != null)
            {
                GlobalSorcererStreetBattleContext.InvaderCard.Update(gameTime);
            }
            if (GlobalSorcererStreetBattleContext.DefenderCard != null)
            {
                GlobalSorcererStreetBattleContext.DefenderCard.Update(gameTime);
            }

            return true;
        }

        public static void ReadPlayerInfo(ByteReader BR, SorcererStreetMap Map)
        {
            Map.GlobalSorcererStreetBattleContext.InvaderPlayerIndex = BR.ReadInt32();
            Map.GlobalSorcererStreetBattleContext.InvaderPlayer = Map.ListPlayer[Map.GlobalSorcererStreetBattleContext.InvaderPlayerIndex];

            string CardType = BR.ReadString();
            string CardPath = BR.ReadString();
            foreach (Card ActiveCard in Map.GlobalSorcererStreetBattleContext.InvaderPlayer.ListCardInHand)
            {
                if (ActiveCard.CardType == CardType && ActiveCard.Path == CardPath)
                {
                    Map.GlobalSorcererStreetBattleContext.Invader = (CreatureCard)ActiveCard;
                    break;
                }
            }

            TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));

            if (Map.GlobalSorcererStreetBattleContext.InvaderCard == null)
            {
                Map.GlobalSorcererStreetBattleContext.Defender = ActiveTerrain.DefendingCreature;
                Map.GlobalSorcererStreetBattleContext.DefenderPlayer = ActiveTerrain.PlayerOwner;
                Map.GlobalSorcererStreetBattleContext.DefenderPlayerIndex = Map.ListPlayer.IndexOf(ActiveTerrain.PlayerOwner);
                Map.GlobalSorcererStreetBattleContext.DefenderTerrain = ActiveTerrain;

                Map.GlobalSorcererStreetBattleContext.Invader.ResetBonuses();
                Map.GlobalSorcererStreetBattleContext.Defender.ResetBonuses();

                Map.GlobalSorcererStreetBattleContext.ListBattlePanelHolder = Map.ListActionMenuChoice;

                if (!Map.IsServer)
                {
                    Map.GlobalSorcererStreetBattleContext.InvaderCard = new SimpleAnimation("Invader", "Invader", Map.GlobalSorcererStreetBattleContext.Invader.sprCard);
                    Map.GlobalSorcererStreetBattleContext.InvaderCard.Position = new Vector2(37, 48);
                    Map.GlobalSorcererStreetBattleContext.InvaderCard.Scale = new Vector2(0.6f);
                    Map.GlobalSorcererStreetBattleContext.DefenderCard = new SimpleAnimation("Defender", "Defender", ActiveTerrain.DefendingCreature.sprCard);
                    Map.GlobalSorcererStreetBattleContext.DefenderCard.Position = new Vector2(Constants.Width - 282, 48);
                    Map.GlobalSorcererStreetBattleContext.DefenderCard.Scale = new Vector2(0.6f);

                    Map.GlobalSorcererStreetBattleContext.Background = AnimationBackground.LoadAnimationBackground("Backgrounds 3D/Grass", Map.Content, GameScreen.GraphicsDevice);
                }
            }

            Map.GlobalSorcererStreetBattleContext.Invader.CurrentHP = BR.ReadInt32();
            Map.GlobalSorcererStreetBattleContext.Invader.CurrentST = BR.ReadInt32();

            Map.GlobalSorcererStreetBattleContext.Defender.CurrentHP = BR.ReadInt32();
            Map.GlobalSorcererStreetBattleContext.Defender.CurrentST = BR.ReadInt32();

            Map.GlobalSorcererStreetBattleContext.InvaderFinalHP = Map.GlobalSorcererStreetBattleContext.Invader.CurrentHP;
            Map.GlobalSorcererStreetBattleContext.DefenderFinalHP = Map.GlobalSorcererStreetBattleContext.Defender.CurrentHP;
            Map.GlobalSorcererStreetBattleContext.InvaderFinalST = Map.GlobalSorcererStreetBattleContext.Invader.CurrentST;
            Map.GlobalSorcererStreetBattleContext.DefenderFinalST = Map.GlobalSorcererStreetBattleContext.Defender.CurrentST;

            InvaderHPBar = Map.GlobalSorcererStreetBattleContext.InvaderFinalHP;
            InvaderSTBar = Map.GlobalSorcererStreetBattleContext.InvaderFinalST;
            DefenderHPBar = Map.GlobalSorcererStreetBattleContext.DefenderFinalHP;
            DefenderSTBar = Map.GlobalSorcererStreetBattleContext.DefenderFinalST;

            string InvaderItemCardType = BR.ReadString();
            if (!string.IsNullOrEmpty(InvaderItemCardType))
            {
                string InvaderItemCardPath = BR.ReadString();

                if (Map.GlobalSorcererStreetBattleContext.InvaderItem == null)
                {
                    foreach (Card ActiveCard in Map.GlobalSorcererStreetBattleContext.InvaderPlayer.ListCardInHand)
                    {
                        if (ActiveCard.CardType == InvaderItemCardType && ActiveCard.Path == InvaderItemCardPath)
                        {
                            Map.GlobalSorcererStreetBattleContext.InvaderItem = (CreatureCard)ActiveCard;
                            break;
                        }
                    }
                }
            }

            string DefenderItemCardType = BR.ReadString();
            if (!string.IsNullOrEmpty(DefenderItemCardType))
            {
                string DefenderItemCardPath = BR.ReadString();

                if (Map.GlobalSorcererStreetBattleContext.DefenderItem == null)
                {
                    foreach (Card ActiveCard in Map.GlobalSorcererStreetBattleContext.DefenderPlayer.ListCardInHand)
                    {
                        if (ActiveCard.CardType == DefenderItemCardType && ActiveCard.Path == DefenderItemCardPath)
                        {
                            Map.GlobalSorcererStreetBattleContext.DefenderItem = (CreatureCard)ActiveCard;
                            break;
                        }
                    }
                }
            }
        }

        public static void WritePlayerInfo(ByteWriter BW, SorcererStreetMap Map)
        {
            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.InvaderPlayerIndex);
            BW.AppendString(Map.GlobalSorcererStreetBattleContext.Invader.CardType);
            BW.AppendString(Map.GlobalSorcererStreetBattleContext.Invader.Path);

            BW.AppendFloat(Map.GlobalSorcererStreetBattleContext.DefenderTerrain.InternalPosition.X);
            BW.AppendFloat(Map.GlobalSorcererStreetBattleContext.DefenderTerrain.InternalPosition.Y);
            BW.AppendFloat(Map.GlobalSorcererStreetBattleContext.DefenderTerrain.LayerIndex);

            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.Invader.CurrentHP);
            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.Invader.CurrentST);

            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.Defender.CurrentHP);
            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.Defender.CurrentST);

            if (Map.GlobalSorcererStreetBattleContext.InvaderItem == null)
            {
                BW.AppendString(string.Empty);
            }
            else
            {
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.InvaderItem.CardType);
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.InvaderItem.Path);
            }
            if (Map.GlobalSorcererStreetBattleContext.DefenderItem == null)
            {
                BW.AppendString(string.Empty);
            }
            else
            {
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.DefenderItem.CardType);
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.DefenderItem.Path);
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            g.GraphicsDevice.Clear(Color.Black);

            if (Map.GlobalSorcererStreetBattleContext.InvaderCard != null)
            {
                Map.GlobalSorcererStreetBattleContext.InvaderCard.BeginDraw(g);
            }
            if (Map.GlobalSorcererStreetBattleContext.DefenderCard != null)
            {
                Map.GlobalSorcererStreetBattleContext.DefenderCard.BeginDraw(g);
            }

            g.End();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1, 0);
            Map.GlobalSorcererStreetBattleContext.Background.Draw(g, Constants.Width, Constants.Height);

            if (Map.GlobalSorcererStreetBattleContext.Invader != null && Map.GlobalSorcererStreetBattleContext.DefenderCard != null)
            {
                DrawInvaderBattle(Map.fntArial12, Map.GlobalSorcererStreetBattleContext, g);
                DrawDefenderBattle(Map.fntArial12, Map.GlobalSorcererStreetBattleContext, g);
            }
        }

        public static void DrawInvaderBattle( SpriteFont fntArial12, SorcererStreetBattleContext GlobalSorcererStreetBattleContext, CustomSpriteBatch g)
        {
            //Left Card
            if (GlobalSorcererStreetBattleContext.InvaderCard != null)
            {
                GlobalSorcererStreetBattleContext.InvaderCard.Draw(g);
            }
            //Item Card
            if (GlobalSorcererStreetBattleContext.InvaderItem != null)
            {
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.InvaderItem.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, Constants.Height / 16, -ItemCardScale, ItemCardScale, false);
            }

            //ST Bar
            float CurrentY = Constants.Height - Constants.Height / 8;
            int BarWeight = Constants.Width / 4;
            int BarX = Constants.Width / 2  - BarWeight - Constants.Width / 10;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntArial12, GlobalSorcererStreetBattleContext.InvaderFinalST.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 50, CurrentY), 30, 30, Color.White);
            g.DrawString(fntArial12, "ST", new Vector2(BarX + 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 80, CurrentY), BarWeight, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(BarX + 85, (int)CurrentY + 5, (int)Math.Min(BarWeight - 10, (BarWeight - 10) * InvaderSTBar / 100f), 20), Color.Blue);

            //HP Bar
            CurrentY +=30;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntArial12, GlobalSorcererStreetBattleContext.InvaderFinalHP.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 50, CurrentY), 30, 30, Color.White);
            g.DrawString(fntArial12, "HP", new Vector2(BarX + 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 80, CurrentY), BarWeight, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(BarX + 85, (int)CurrentY + 5, (int)Math.Min(BarWeight - 10, (BarWeight - 10) * InvaderHPBar / 100f), 20), Color.Green);
        }

        public static void DrawDefenderBattle(SpriteFont fntArial12, SorcererStreetBattleContext GlobalSorcererStreetBattleContext, CustomSpriteBatch g)
        {
            //Right Card
            if (GlobalSorcererStreetBattleContext.DefenderCard != null)
            {
                GlobalSorcererStreetBattleContext.DefenderCard.Draw(g);
            }
            //Item Card
            if (GlobalSorcererStreetBattleContext.DefenderItem != null)
            {
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.DefenderItem.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, Constants.Height / 16, - ItemCardScale, ItemCardScale, false);
            }

            //ST Bar
            float CurrentY = Constants.Height - Constants.Height / 8;
            int BarWeight = Constants.Width / 4;
            int BarX = Constants.Width / 2 + Constants.Width / 10;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntArial12, GlobalSorcererStreetBattleContext.DefenderFinalST.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 50, CurrentY), 30, 30, Color.White);
            g.DrawString(fntArial12, "ST", new Vector2(BarX + 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 80, CurrentY), BarWeight, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(BarX + 85, (int)CurrentY + 5, (int)Math.Min(BarWeight - 10, (BarWeight - 10) * DefenderSTBar / 100f), 20), Color.Blue);

            //HP Bar
            CurrentY += 30;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntArial12, GlobalSorcererStreetBattleContext.DefenderFinalHP.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 50, CurrentY), 30, 30, Color.White);
            g.DrawString(fntArial12, "HP", new Vector2(BarX + 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 80, CurrentY), BarWeight, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(BarX + 85, (int)CurrentY + 5, (int)Math.Min(BarWeight - 10, (BarWeight - 10) * DefenderHPBar / 100f), 20), Color.Green);
        }

        public void DisplayVersusText(CustomSpriteBatch g)
        {
            int Y = Constants.Height / 4 - 25;
            TextHelper.DrawTextMiddleAligned(g, "BATTLE", new Vector2(Constants.Width / 2, Y), Color.White);
            Y = Constants.Height / 4;
            GameScreen.DrawBox(g, new Vector2(Constants.Width / 16, Y), Constants.Width - Constants.Width / 8, Constants.Height / 5, Color.White);
            Y = Constants.Height / 4 + 10;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderPlayer.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderPlayer.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
            Y = Constants.Height / 4 + 35;
            g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 12, Y), new Vector2(Constants.Width - Constants.Width / 12, Y), Color.White);
            g.Draw(Map.sprVS, new Rectangle(Constants.Width / 2 - 30, Y, 60, 60), Color.White);
            Y = Constants.Height / 4 + 40;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderCard.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderCard.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
        }
    }
}
