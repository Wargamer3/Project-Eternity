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
            HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext);
        }

        public static bool HasFinishedUpdatingBars(GameTime gameTime, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            if (InvaderHPBar > GlobalSorcererStreetBattleContext.Invader.FinalHP)
            {
                --InvaderHPBar;
                return false;
            }
            else if (InvaderSTBar > GlobalSorcererStreetBattleContext.Invader.FinalST)
            {
                --InvaderSTBar;
                return false;
            }
            else if (DefenderHPBar > GlobalSorcererStreetBattleContext.Defender.FinalHP)
            {
                --DefenderHPBar;
                return false;
            }
            else if (DefenderSTBar > GlobalSorcererStreetBattleContext.Defender.FinalST)
            {
                --DefenderSTBar;
                return false;
            }

            if (InvaderHPBar < GlobalSorcererStreetBattleContext.Invader.FinalHP)
            {
                ++InvaderHPBar;
                return false;
            }
            else if (InvaderSTBar < GlobalSorcererStreetBattleContext.Invader.FinalST)
            {
                ++InvaderSTBar;
                return false;
            }
            else if (DefenderHPBar < GlobalSorcererStreetBattleContext.Defender.FinalHP)
            {
                ++DefenderHPBar;
                return false;
            }
            else if (DefenderSTBar < GlobalSorcererStreetBattleContext.Defender.FinalST)
            {
                ++DefenderSTBar;
                return false;
            }

            return true;
        }

        public static void UpdateCards(GameTime gameTime, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            if (GlobalSorcererStreetBattleContext.Invader.Animation != null)
            {
                GlobalSorcererStreetBattleContext.Invader.Animation.Update(gameTime);
            }
            if (GlobalSorcererStreetBattleContext.Defender.Animation != null)
            {
                GlobalSorcererStreetBattleContext.Defender.Animation.Update(gameTime);
            }
        }

        public static void ReadPlayerInfo(ByteReader BR, SorcererStreetMap Map)
        {
            Map.GlobalSorcererStreetBattleContext.Invader.PlayerIndex = BR.ReadInt32();
            Map.GlobalSorcererStreetBattleContext.Invader.Owner = Map.ListPlayer[Map.GlobalSorcererStreetBattleContext.Invader.PlayerIndex];

            string CardType = BR.ReadString();
            string CardPath = BR.ReadString();
            foreach (Card ActiveCard in Map.GlobalSorcererStreetBattleContext.Invader.Owner.ListCardInHand)
            {
                if (ActiveCard.CardType == CardType && ActiveCard.Path == CardPath)
                {
                    Map.GlobalSorcererStreetBattleContext.Invader.Creature = (CreatureCard)ActiveCard;
                    break;
                }
            }

            TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));

            if (Map.GlobalSorcererStreetBattleContext.Invader.Animation == null)
            {
                Map.GlobalSorcererStreetBattleContext.Defender.Creature = ActiveTerrain.DefendingCreature;
                Map.GlobalSorcererStreetBattleContext.Defender.Owner = ActiveTerrain.PlayerOwner;
                Map.GlobalSorcererStreetBattleContext.Defender.PlayerIndex = Map.ListPlayer.IndexOf(ActiveTerrain.PlayerOwner);
                Map.GlobalSorcererStreetBattleContext.DefenderTerrain = ActiveTerrain;

                Map.GlobalSorcererStreetBattleContext.Invader.Creature.InitBattleBonuses();
                Map.GlobalSorcererStreetBattleContext.Defender.Creature.InitBattleBonuses();

                Map.GlobalSorcererStreetBattleContext.ListBattlePanelHolder = Map.ListActionMenuChoice;

                if (!Map.IsServer)
                {
                    Map.GlobalSorcererStreetBattleContext.Invader.Animation = new SimpleAnimation("Invader", "Invader", Map.GlobalSorcererStreetBattleContext.Invader.Creature.sprCard);
                    Map.GlobalSorcererStreetBattleContext.Invader.Animation.Position = new Vector2(37, 48);
                    Map.GlobalSorcererStreetBattleContext.Invader.Animation.Scale = new Vector2(0.6f);
                    Map.GlobalSorcererStreetBattleContext.Defender.Animation = new SimpleAnimation("Defender", "Defender", ActiveTerrain.DefendingCreature.sprCard);
                    Map.GlobalSorcererStreetBattleContext.Defender.Animation.Position = new Vector2(Constants.Width - 282, 48);
                    Map.GlobalSorcererStreetBattleContext.Defender.Animation.Scale = new Vector2(0.6f);

                    Map.GlobalSorcererStreetBattleContext.Background = AnimationBackground.LoadAnimationBackground("Backgrounds 3D/Grass", Map.Content, GameScreen.GraphicsDevice);
                }
            }

            Map.GlobalSorcererStreetBattleContext.Invader.Creature.CurrentHP = BR.ReadInt32();
            Map.GlobalSorcererStreetBattleContext.Invader.Creature.CurrentST = BR.ReadInt32();

            Map.GlobalSorcererStreetBattleContext.Defender.Creature.CurrentHP = BR.ReadInt32();
            Map.GlobalSorcererStreetBattleContext.Defender.Creature.CurrentST = BR.ReadInt32();

            Map.GlobalSorcererStreetBattleContext.Invader.BonusHP = 0;
            Map.GlobalSorcererStreetBattleContext.Defender.BonusHP = 0;
            Map.GlobalSorcererStreetBattleContext.Invader.BonusST = 0;
            Map.GlobalSorcererStreetBattleContext.Defender.BonusST = 0;

            InvaderHPBar = Map.GlobalSorcererStreetBattleContext.Invader.FinalHP;
            InvaderSTBar = Map.GlobalSorcererStreetBattleContext.Invader.FinalST;
            DefenderHPBar = Map.GlobalSorcererStreetBattleContext.Defender.FinalHP;
            DefenderSTBar = Map.GlobalSorcererStreetBattleContext.Defender.FinalST;

            string InvaderItemCardType = BR.ReadString();
            if (!string.IsNullOrEmpty(InvaderItemCardType))
            {
                string InvaderItemCardPath = BR.ReadString();

                if (Map.GlobalSorcererStreetBattleContext.Invader.Item == null)
                {
                    foreach (Card ActiveCard in Map.GlobalSorcererStreetBattleContext.Invader.Owner.ListCardInHand)
                    {
                        if (ActiveCard.CardType == InvaderItemCardType && ActiveCard.Path == InvaderItemCardPath)
                        {
                            Map.GlobalSorcererStreetBattleContext.Invader.Item = (CreatureCard)ActiveCard;
                            break;
                        }
                    }
                }
            }

            string DefenderItemCardType = BR.ReadString();
            if (!string.IsNullOrEmpty(DefenderItemCardType))
            {
                string DefenderItemCardPath = BR.ReadString();

                if (Map.GlobalSorcererStreetBattleContext.Defender.Item == null)
                {
                    foreach (Card ActiveCard in Map.GlobalSorcererStreetBattleContext.Defender.Owner.ListCardInHand)
                    {
                        if (ActiveCard.CardType == DefenderItemCardType && ActiveCard.Path == DefenderItemCardPath)
                        {
                            Map.GlobalSorcererStreetBattleContext.Defender.Item = (CreatureCard)ActiveCard;
                            break;
                        }
                    }
                }
            }
        }

        public static void WritePlayerInfo(ByteWriter BW, SorcererStreetMap Map)
        {
            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.Invader.PlayerIndex);
            BW.AppendString(Map.GlobalSorcererStreetBattleContext.Invader.Creature.CardType);
            BW.AppendString(Map.GlobalSorcererStreetBattleContext.Invader.Creature.Path);

            BW.AppendFloat(Map.GlobalSorcererStreetBattleContext.DefenderTerrain.GridPosition.X);
            BW.AppendFloat(Map.GlobalSorcererStreetBattleContext.DefenderTerrain.GridPosition.Y);
            BW.AppendFloat(Map.GlobalSorcererStreetBattleContext.DefenderTerrain.LayerIndex);

            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.Invader.Creature.CurrentHP);
            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.Invader.Creature.CurrentST);

            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.Defender.Creature.CurrentHP);
            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.Defender.Creature.CurrentST);

            if (Map.GlobalSorcererStreetBattleContext.Invader.Item == null)
            {
                BW.AppendString(string.Empty);
            }
            else
            {
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.Invader.Item.CardType);
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.Invader.Item.Path);
            }
            if (Map.GlobalSorcererStreetBattleContext.Defender.Item == null)
            {
                BW.AppendString(string.Empty);
            }
            else
            {
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.Defender.Item.CardType);
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.Defender.Item.Path);
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            g.GraphicsDevice.Clear(Color.Black);

            if (Map.GlobalSorcererStreetBattleContext.Invader.Animation != null)
            {
                Map.GlobalSorcererStreetBattleContext.Invader.Animation.BeginDraw(g);
            }
            if (Map.GlobalSorcererStreetBattleContext.Defender.Animation != null)
            {
                Map.GlobalSorcererStreetBattleContext.Defender.Animation.BeginDraw(g);
            }

            g.End();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1, 0);
            Map.GlobalSorcererStreetBattleContext.Background.Draw(g, Constants.Width, Constants.Height);

            if (Map.GlobalSorcererStreetBattleContext.Invader != null && Map.GlobalSorcererStreetBattleContext.Defender.Animation != null)
            {
                DrawInvaderBattle(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext, g);
                DrawDefenderBattle(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext, g);
            }
        }

        public static void DrawInvaderBattle(SpriteFont fntMenuText, SorcererStreetBattleContext GlobalSorcererStreetBattleContext, CustomSpriteBatch g)
        {
            //Left Card
            if (GlobalSorcererStreetBattleContext.Invader.Animation != null)
            {
                GlobalSorcererStreetBattleContext.Invader.Animation.Draw(g);
            }
            //Item Card
            if (GlobalSorcererStreetBattleContext.Invader.Item != null)
            {
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.Invader.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, Constants.Height / 16, -ItemCardScale, ItemCardScale, false);
            }

            //ST Bar
            float CurrentY = Constants.Height - Constants.Height / 8;
            int BarWeight = Constants.Width / 4;
            int BarX = Constants.Width / 2  - BarWeight - Constants.Width / 10;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntMenuText, GlobalSorcererStreetBattleContext.Invader.FinalST.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 50, CurrentY), 30, 30, Color.White);
            g.DrawString(fntMenuText, "ST", new Vector2(BarX + 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 80, CurrentY), BarWeight, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(BarX + 85, (int)CurrentY + 5, (int)Math.Min(BarWeight - 10, (BarWeight - 10) * InvaderSTBar / 100f), 20), Color.Blue);

            //HP Bar
            CurrentY +=30;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntMenuText, GlobalSorcererStreetBattleContext.Invader.FinalHP.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 50, CurrentY), 30, 30, Color.White);
            g.DrawString(fntMenuText, "HP", new Vector2(BarX + 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 80, CurrentY), BarWeight, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(BarX + 85, (int)CurrentY + 5, (int)Math.Min(BarWeight - 10, (BarWeight - 10) * InvaderHPBar / 100f), 20), Color.Green);
        }

        public static void DrawDefenderBattle(SpriteFont fntMenuText, SorcererStreetBattleContext GlobalSorcererStreetBattleContext, CustomSpriteBatch g)
        {
            //Right Card
            if (GlobalSorcererStreetBattleContext.Defender.Animation != null)
            {
                GlobalSorcererStreetBattleContext.Defender.Animation.Draw(g);
            }
            //Item Card
            if (GlobalSorcererStreetBattleContext.Defender.Item != null)
            {
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.Defender.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, Constants.Height / 16, - ItemCardScale, ItemCardScale, false);
            }

            //ST Bar
            float CurrentY = Constants.Height - Constants.Height / 8;
            int BarWeight = Constants.Width / 4;
            int BarX = Constants.Width / 2 + Constants.Width / 10;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntMenuText, GlobalSorcererStreetBattleContext.Defender.FinalST.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 50, CurrentY), 30, 30, Color.White);
            g.DrawString(fntMenuText, "ST", new Vector2(BarX + 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 80, CurrentY), BarWeight, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(BarX + 85, (int)CurrentY + 5, (int)Math.Min(BarWeight - 10, (BarWeight - 10) * DefenderSTBar / 100f), 20), Color.Blue);

            //HP Bar
            CurrentY += 30;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntMenuText, GlobalSorcererStreetBattleContext.Defender.FinalHP.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 50, CurrentY), 30, 30, Color.White);
            g.DrawString(fntMenuText, "HP", new Vector2(BarX + 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 80, CurrentY), BarWeight, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(BarX + 85, (int)CurrentY + 5, (int)Math.Min(BarWeight - 10, (BarWeight - 10) * DefenderHPBar / 100f), 20), Color.Green);
        }

        public static void DisplayVersusText(CustomSpriteBatch g, SorcererStreetBattleContext GlobalSorcererStreetBattleContext, SpriteFont fntMenuText, Texture2D sprVS)
        {
            int Y = Constants.Height / 4 - 25;
            TextHelper.DrawTextMiddleAligned(g, "BATTLE", new Vector2(Constants.Width / 2, Y), Color.White);
            Y = Constants.Height / 4;
            GameScreen.DrawBox(g, new Vector2(Constants.Width / 16, Y), Constants.Width - Constants.Width / 8, Constants.Height / 5, Color.White);
            Y = Constants.Height / 4 + 10;
            g.DrawStringMiddleAligned(fntMenuText, GlobalSorcererStreetBattleContext.Invader.Owner.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(fntMenuText, GlobalSorcererStreetBattleContext.Defender.Owner.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
            Y = Constants.Height / 4 + 35;
            g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 12, Y), new Vector2(Constants.Width - Constants.Width / 12, Y), Color.White);
            g.Draw(sprVS, new Rectangle(Constants.Width / 2 - 30, Y, 60, 60), Color.White);
            Y = Constants.Height / 4 + 40;
            g.DrawStringMiddleAligned(fntMenuText, GlobalSorcererStreetBattleContext.Invader.Animation.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(fntMenuText, GlobalSorcererStreetBattleContext.Defender.Animation.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
        }
    }
}
