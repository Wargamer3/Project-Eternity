using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleStartPhase : GameScreen
    {
        private readonly SorcererStreetMap Map;
        private readonly int ActivePlayerIndex;
        private readonly Player ActivePlayer;
        private readonly CreatureCard Invader;

        private readonly ActionPanelHolder ListActionMenuChoice;
        private int InvaderHP;
        private int DefenderHP;
        private int InvaderST;
        private int DefenderST;
        private double AnimationTime;

        public ActionPanelBattleStartPhase(SorcererStreetMap Map, int ActivePlayerIndex, CreatureCard Invader)
        {
            this.Map = Map;
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            this.Invader = Invader;

            ListActionMenuChoice = new ActionPanelHolder();
        }

        public override void Load()
        {
            TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(ActivePlayer.GamePiece);

            Map.GlobalSorcererStreetBattleContext.Invader = Invader;
            Map.GlobalSorcererStreetBattleContext.Defender = ActiveTerrain.DefendingCreature;

            Map.GlobalSorcererStreetBattleContext.InvaderPlayer = ActivePlayer;
            Map.GlobalSorcererStreetBattleContext.DefenderPlayer = ActiveTerrain.PlayerOwner;

            Map.GlobalSorcererStreetBattleContext.Invader.ResetBonuses();
            Map.GlobalSorcererStreetBattleContext.Defender.ResetBonuses();

            Map.GlobalSorcererStreetBattleContext.InvaderFinalHP = Map.GlobalSorcererStreetBattleContext.Invader.CurrentHP;
            Map.GlobalSorcererStreetBattleContext.DefenderFinalHP = Map.GlobalSorcererStreetBattleContext.Defender.CurrentHP;
            Map.GlobalSorcererStreetBattleContext.InvaderFinalST = Map.GlobalSorcererStreetBattleContext.Invader.CurrentST;
            Map.GlobalSorcererStreetBattleContext.DefenderFinalST = Map.GlobalSorcererStreetBattleContext.Defender.CurrentST;

            Map.GlobalSorcererStreetBattleContext.InvaderCard = new SimpleAnimation("Invader", "Invader", Invader.sprCard);
            Map.GlobalSorcererStreetBattleContext.InvaderCard.Position = new Vector2(37, 48);
            Map.GlobalSorcererStreetBattleContext.InvaderCard.Scale = new Vector2(0.6f);
            Map.GlobalSorcererStreetBattleContext.DefenderCard = new SimpleAnimation("Defender", "Defender", ActiveTerrain.DefendingCreature.sprCard);
            Map.GlobalSorcererStreetBattleContext.DefenderCard.Position = new Vector2(Constants.Width - 282, 48);
            Map.GlobalSorcererStreetBattleContext.DefenderCard.Scale = new Vector2(0.6f);

            ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelBattleItemSelectionPhase(ListActionMenuChoice, Map, ActivePlayerIndex));

            Map.GlobalSorcererStreetBattleContext.Background = AnimationBackground.LoadAnimationBackground("Backgrounds 3D/Grass", Content, GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            Map.GlobalSorcererStreetBattleContext.Background.Update(gameTime);

            if (AnimationTime < 2)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                Map.GlobalSorcererStreetBattleContext.Background.MoveSpeed = new Vector3(0, 0, 1);
            }
            else if (AnimationTime < 8)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                Map.GlobalSorcererStreetBattleContext.Background.MoveSpeed = new Vector3(0, 0, 0);
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (InvaderHP < Map.GlobalSorcererStreetBattleContext.InvaderFinalHP)
            {
                ++InvaderHP;
            }
            else if (InvaderST < Map.GlobalSorcererStreetBattleContext.InvaderFinalST)
            {
                ++InvaderST;
            }
            else if (DefenderHP < Map.GlobalSorcererStreetBattleContext.DefenderFinalHP)
            {
                ++DefenderHP;
            }
            else if (DefenderST < Map.GlobalSorcererStreetBattleContext.DefenderFinalST)
            {
                ++DefenderST;
            }
            else
            {
                ListActionMenuChoice.Last().Update(gameTime);
            }

            if (!ListActionMenuChoice.HasMainPanel)
            {
                RemoveScreen(this);
                if (Map.GlobalSorcererStreetBattleContext.Defender.CurrentHP > 0)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelBattleDefenderWinPhase(Map));
                }
                else
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelBattleDefenderDefeatedPhase(Map));
                }
            }

            if (Map.GlobalSorcererStreetBattleContext.InvaderCard != null)
            {
                Map.GlobalSorcererStreetBattleContext.InvaderCard.Update(gameTime);
            }
            if (Map.GlobalSorcererStreetBattleContext.DefenderCard != null)
            {
                Map.GlobalSorcererStreetBattleContext.DefenderCard.Update(gameTime);
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

            if (AnimationTime >= 2)
            {
                if (AnimationTime < 4)
                {
                    IntroduceInvader(g);
                }
                else if (AnimationTime < 6)
                {
                    IntroduceInvader(g);
                    IntroduceDefender(g);
                }
                else if (AnimationTime < 8)
                {
                    IntroduceInvader(g);
                    IntroduceDefender(g);
                    DisplayVersusText(g);
                }
                else
                {
                    if (ListActionMenuChoice.HasMainPanel)
                    {
                        if (ListActionMenuChoice.Last() is ActionPanelBattleItemSelectionPhase)
                        {
                            IntroduceInvader(g);
                            IntroduceDefender(g);
                        }
                        else if (Map.GlobalSorcererStreetBattleContext.Invader != null && Map.GlobalSorcererStreetBattleContext.DefenderCard != null)
                        {
                            DrawInvaderBattle(g);
                            DrawDefenderBattle(g);
                        }

                        ListActionMenuChoice.Last().Draw(g);
                    }
                }
            }
        }

        public void DrawInvaderBattle(CustomSpriteBatch g)
        {
            //Left Card
            if (Map.GlobalSorcererStreetBattleContext.InvaderCard != null)
            {
                Map.GlobalSorcererStreetBattleContext.InvaderCard.Draw(g);
            }
            //Item Card
            if (Map.GlobalSorcererStreetBattleContext.InvaderItem != null)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle(10, 25, 30, 50), Color.White);
            }

            //ST Bar
            float CurrentY = 380;
            GameScreen.DrawBox(g, new Vector2(20, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalST.ToString(), new Vector2(35, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(70, CurrentY), 30, 30, Color.White);
            g.DrawString(Map.fntArial12, "ST", new Vector2(75, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(100, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(105, (int)CurrentY + 5, (int)(200f * Map.GlobalSorcererStreetBattleContext.InvaderFinalST / 100f), 20), Color.Blue);

            //HP Bar
            CurrentY = 410;
            GameScreen.DrawBox(g, new Vector2(20, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalHP.ToString(), new Vector2(35, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(70, CurrentY), 30, 30, Color.White);
            g.DrawString(Map.fntArial12, "HP", new Vector2(75, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(100, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(105, (int)CurrentY + 5, (int)(200f * Map.GlobalSorcererStreetBattleContext.InvaderFinalHP / 100f), 20), Color.Green);
        }

        public void DrawDefenderBattle(CustomSpriteBatch g)
        {
            //Right Card
            if (Map.GlobalSorcererStreetBattleContext.DefenderCard != null)
            {
                Map.GlobalSorcererStreetBattleContext.DefenderCard.Draw(g);
            }
            //Item Card
            if (Map.GlobalSorcererStreetBattleContext.DefenderItem != null)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 210, 25, 30, 50), Color.White);
            }

            //ST Bar
            float CurrentY = 380;
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 300, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 295, (int)CurrentY + 5, (int)(200f * Map.GlobalSorcererStreetBattleContext.DefenderFinalST / 100f), 20), Color.Blue);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 100, CurrentY), 30, 30, Color.White);
            g.DrawString(Map.fntArial12, "ST", new Vector2(Constants.Width - 95, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 70, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderFinalST.ToString(), new Vector2(Constants.Width - 55, CurrentY + 5), Color.Black);

            //HP Bar
            CurrentY = 410;
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 300, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 295, (int)CurrentY + 5, (int)(200f * Map.GlobalSorcererStreetBattleContext.DefenderFinalHP / 100f), 20), Color.Green);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 100, CurrentY), 30, 30, Color.White);
            g.DrawString(Map.fntArial12, "HP", new Vector2(Constants.Width - 95, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 70, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderFinalHP.ToString(), new Vector2(Constants.Width - 55, CurrentY + 5), Color.Black);
        }

        private void IntroduceInvader(CustomSpriteBatch g)
        {
            //Spin card from the left
            float MaxScale = 0.6f;
            if (AnimationTime < 1)
            {
                MaxScale = 0.5f;
            }
            float RealRotationTimer = (float)AnimationTime - 2;
            float FinalX = Constants.Width / 4;
            float StartX = -10;
            float DistanceX = FinalX - StartX;
            float X = StartX + (RealRotationTimer) * DistanceX;
            float Y = Constants.Height / 10;
            RealRotationTimer *= 5;
            float FinalScale = (float)Math.Sin(RealRotationTimer) * MaxScale;
            if (X > FinalX)
            {
                FinalScale = -MaxScale;
                RealRotationTimer = MathHelper.Pi;
                X = FinalX;
            }

            Card.DrawCardMiniature(g, Map.GlobalSorcererStreetBattleContext.Invader.sprCard, Map.sprCardBack, Color.White, X, Y, FinalScale, MaxScale, RealRotationTimer);
        }

        private void IntroduceDefender(CustomSpriteBatch g)
        {
            //Spin card from the left
            float MaxScale = 0.6f;
            if (AnimationTime < 1)
            {
                MaxScale = 0.5f;
            }
            float RealRotationTimer = (float)AnimationTime - 4;
            float FinalX = Constants.Width - Constants.Width / 4;
            float StartX = Constants.Width + 10;
            float DistanceX = FinalX - StartX;
            float X = StartX + (RealRotationTimer) * DistanceX;
            float Y = Constants.Height / 10;
            RealRotationTimer *= 5;
            float FinalScale = (float)Math.Sin(RealRotationTimer) * MaxScale;
            if (X < FinalX)
            {
                FinalScale = -MaxScale;
                RealRotationTimer = MathHelper.Pi;
                X = FinalX;
            }

            Card.DrawCardMiniature(g, Map.GlobalSorcererStreetBattleContext.Invader.sprCard, Map.sprCardBack, Color.White, X, Y, FinalScale, MaxScale, RealRotationTimer);
        }

        private void DisplayVersusText(CustomSpriteBatch g)
        {
            int Y = Constants.Height / 4 - 25;
            TextHelper.DrawTextMiddleAligned(g, "BATTLE", new Vector2(Constants.Width / 2, Y), Color.White);
            Y = Constants.Height / 4;
            DrawBox(g, new Vector2(Constants.Width / 16, Y), Constants.Width - Constants.Width / 8, Constants.Height / 5, Color.White);
            Y = Constants.Height / 4 + 10;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderPlayer.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderPlayer.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
            Y = Constants.Height / 4 + 35;
            g.DrawLine(sprPixel, new Vector2(Constants.Width / 12, Y), new Vector2(Constants.Width - Constants.Width / 12, Y), Color.White);
            g.Draw(Map.sprVS, new Rectangle(Constants.Width / 2 - 30, Y, 60, 60), Color.White);
            Y = Constants.Height / 4 + 40;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderCard.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderCard.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
        }
    }
}
