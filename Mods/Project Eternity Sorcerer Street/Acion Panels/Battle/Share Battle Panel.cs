using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class ActionPanelBattle : ActionPanelSorcererStreet
    {
        private const float ItemCardScale = 0.2f;

        protected static int InvaderHP;
        protected static int DefenderHP;
        protected static int InvaderST;
        protected static int DefenderST;

        public ActionPanelBattle(SorcererStreetMap Map, string PanelName)
            : base(PanelName, Map, false)
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            CanUpdate(gameTime);
        }

        public bool CanUpdate(GameTime gameTime)
        {
            if (InvaderHP > Map.GlobalSorcererStreetBattleContext.InvaderFinalHP)
            {
                --InvaderHP;
                return false;
            }
            else if (InvaderST > Map.GlobalSorcererStreetBattleContext.InvaderFinalST)
            {
                --InvaderST;
                return false;
            }
            else if (DefenderHP > Map.GlobalSorcererStreetBattleContext.DefenderFinalHP)
            {
                --DefenderHP;
                return false;
            }
            else if (DefenderST > Map.GlobalSorcererStreetBattleContext.DefenderFinalST)
            {
                --DefenderST;
                return false;
            }

            if (InvaderHP < Map.GlobalSorcererStreetBattleContext.InvaderFinalHP)
            {
                ++InvaderHP;
                return false;
            }
            else if (InvaderST < Map.GlobalSorcererStreetBattleContext.InvaderFinalST)
            {
                ++InvaderST;
                return false;
            }
            else if (DefenderHP < Map.GlobalSorcererStreetBattleContext.DefenderFinalHP)
            {
                ++DefenderHP;
                return false;
            }
            else if (DefenderST < Map.GlobalSorcererStreetBattleContext.DefenderFinalST)
            {
                ++DefenderST;
                return false;
            }

            if (Map.GlobalSorcererStreetBattleContext.InvaderCard != null)
            {
                Map.GlobalSorcererStreetBattleContext.InvaderCard.Update(gameTime);
            }
            if (Map.GlobalSorcererStreetBattleContext.DefenderCard != null)
            {
                Map.GlobalSorcererStreetBattleContext.DefenderCard.Update(gameTime);
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

            InvaderHP = Map.GlobalSorcererStreetBattleContext.InvaderFinalHP;
            InvaderST = Map.GlobalSorcererStreetBattleContext.InvaderFinalST;
            DefenderHP = Map.GlobalSorcererStreetBattleContext.DefenderFinalHP;
            DefenderST = Map.GlobalSorcererStreetBattleContext.DefenderFinalST;

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
                DrawInvaderBattle(Map, g);
                DrawDefenderBattle(Map, g);
            }
        }

        public static void DrawInvaderBattle(SorcererStreetMap Map, CustomSpriteBatch g)
        {
            //Left Card
            if (Map.GlobalSorcererStreetBattleContext.InvaderCard != null)
            {
                Map.GlobalSorcererStreetBattleContext.InvaderCard.Draw(g);
            }
            //Item Card
            if (Map.GlobalSorcererStreetBattleContext.InvaderItem != null)
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.InvaderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width / 24f, Constants.Height / 10.6f, -ItemCardScale, ItemCardScale, false);
            }

            //ST Bar
            float CurrentY = 380;
            GameScreen.DrawBox(g, new Vector2(20, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalST.ToString(), new Vector2(35, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(70, CurrentY), 30, 30, Color.White);
            g.DrawString(Map.fntArial12, "ST", new Vector2(75, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(100, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(105, (int)CurrentY + 5, (int)Math.Min(190, 200f * InvaderST / 100f), 20), Color.Blue);

            //HP Bar
            CurrentY = 410;
            GameScreen.DrawBox(g, new Vector2(20, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalHP.ToString(), new Vector2(35, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(70, CurrentY), 30, 30, Color.White);
            g.DrawString(Map.fntArial12, "HP", new Vector2(75, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(100, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(105, (int)CurrentY + 5, (int)Math.Min(190, 200f * InvaderHP / 100f), 20), Color.Green);
        }

        public static void DrawDefenderBattle(SorcererStreetMap Map, CustomSpriteBatch g)
        {
            //Right Card
            if (Map.GlobalSorcererStreetBattleContext.DefenderCard != null)
            {
                Map.GlobalSorcererStreetBattleContext.DefenderCard.Draw(g);
            }
            //Item Card
            if (Map.GlobalSorcererStreetBattleContext.DefenderItem != null)
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.DefenderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width - Constants.Width / 24f, Constants.Height / 10.6f, -ItemCardScale, ItemCardScale, false);
            }

            //ST Bar
            float CurrentY = 380;
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 70, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderFinalST.ToString(), new Vector2(Constants.Width - 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 100, CurrentY), 30, 30, Color.White);
            g.DrawString(Map.fntArial12, "ST", new Vector2(Constants.Width - 95, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 300, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 295, (int)CurrentY + 5, (int)Math.Min(190, 200f * DefenderST / 100f), 20), Color.Blue);

            //HP Bar
            CurrentY = 410;
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 70, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderFinalHP.ToString(), new Vector2(Constants.Width - 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 100, CurrentY), 30, 30, Color.White);
            g.DrawString(Map.fntArial12, "HP", new Vector2(Constants.Width - 95, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 300, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 295, (int)CurrentY + 5, (int)Math.Min(190, 200f * DefenderHP / 100f), 20), Color.Green);
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
