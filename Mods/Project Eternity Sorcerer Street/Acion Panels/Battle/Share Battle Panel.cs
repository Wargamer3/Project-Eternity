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
            if (InvaderHPBar > GlobalSorcererStreetBattleContext.SelfCreature.FinalHP)
            {
                --InvaderHPBar;
                return false;
            }
            else if (InvaderSTBar > GlobalSorcererStreetBattleContext.SelfCreature.FinalST)
            {
                --InvaderSTBar;
                return false;
            }
            else if (DefenderHPBar > GlobalSorcererStreetBattleContext.OpponentCreature.FinalHP)
            {
                --DefenderHPBar;
                return false;
            }
            else if (DefenderSTBar > GlobalSorcererStreetBattleContext.OpponentCreature.FinalST)
            {
                --DefenderSTBar;
                return false;
            }

            if (InvaderHPBar < GlobalSorcererStreetBattleContext.SelfCreature.FinalHP)
            {
                ++InvaderHPBar;
                return false;
            }
            else if (InvaderSTBar < GlobalSorcererStreetBattleContext.SelfCreature.FinalST)
            {
                ++InvaderSTBar;
                return false;
            }
            else if (DefenderHPBar < GlobalSorcererStreetBattleContext.OpponentCreature.FinalHP)
            {
                ++DefenderHPBar;
                return false;
            }
            else if (DefenderSTBar < GlobalSorcererStreetBattleContext.OpponentCreature.FinalST)
            {
                ++DefenderSTBar;
                return false;
            }

            return true;
        }

        public static void UpdateCards(GameTime gameTime, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            if (GlobalSorcererStreetBattleContext.SelfCreature.Animation != null)
            {
                GlobalSorcererStreetBattleContext.SelfCreature.Animation.Update(gameTime);
            }
            if (GlobalSorcererStreetBattleContext.OpponentCreature.Animation != null)
            {
                GlobalSorcererStreetBattleContext.OpponentCreature.Animation.Update(gameTime);
            }
        }

        public static void ReadPlayerInfo(ByteReader BR, SorcererStreetMap Map)
        {
            Map.GlobalSorcererStreetBattleContext.SelfCreature.PlayerIndex = BR.ReadInt32();
            Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner = Map.ListPlayer[Map.GlobalSorcererStreetBattleContext.SelfCreature.PlayerIndex];

            string CardType = BR.ReadString();
            string CardPath = BR.ReadString();
            foreach (Card ActiveCard in Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner.ListCardInHand)
            {
                if (ActiveCard.CardType == CardType && ActiveCard.Path == CardPath)
                {
                    Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature = (CreatureCard)ActiveCard;
                    break;
                }
            }

            TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));

            if (Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation == null)
            {
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature = ActiveTerrain.DefendingCreature;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner = ActiveTerrain.PlayerOwner;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.PlayerIndex = Map.ListPlayer.IndexOf(ActiveTerrain.PlayerOwner);
                Map.GlobalSorcererStreetBattleContext.ActiveTerrain = ActiveTerrain;

                Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.InitBattleBonuses();
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.InitBattleBonuses();

                Map.GlobalSorcererStreetBattleContext.ListBattlePanelHolder = Map.ListActionMenuChoice;

                if (!Map.IsServer)
                {
                    Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation = new SimpleAnimation("Invader", "Invader", Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.sprCard);
                    Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation.Position = new Vector2(37, 48);
                    Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation.Scale = new Vector2(0.6f);
                    Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation = new SimpleAnimation("Defender", "Defender", ActiveTerrain.DefendingCreature.sprCard);
                    Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation.Position = new Vector2(Constants.Width - 282, 48);
                    Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation.Scale = new Vector2(0.6f);

                    Map.GlobalSorcererStreetBattleContext.Background = AnimationBackground.LoadAnimationBackground("Backgrounds 3D/Grass", Map.Content, GameScreen.GraphicsDevice);
                }
            }

            Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.CurrentHP = BR.ReadInt32();
            Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.CurrentST = BR.ReadInt32();

            Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.CurrentHP = BR.ReadInt32();
            Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.CurrentST = BR.ReadInt32();

            Map.GlobalSorcererStreetBattleContext.SelfCreature.BonusHP = 0;
            Map.GlobalSorcererStreetBattleContext.OpponentCreature.BonusHP = 0;
            Map.GlobalSorcererStreetBattleContext.SelfCreature.BonusST = 0;
            Map.GlobalSorcererStreetBattleContext.OpponentCreature.BonusST = 0;

            InvaderHPBar = Map.GlobalSorcererStreetBattleContext.SelfCreature.FinalHP;
            InvaderSTBar = Map.GlobalSorcererStreetBattleContext.SelfCreature.FinalST;
            DefenderHPBar = Map.GlobalSorcererStreetBattleContext.OpponentCreature.FinalHP;
            DefenderSTBar = Map.GlobalSorcererStreetBattleContext.OpponentCreature.FinalST;

            string InvaderItemCardType = BR.ReadString();
            if (!string.IsNullOrEmpty(InvaderItemCardType))
            {
                string InvaderItemCardPath = BR.ReadString();

                if (Map.GlobalSorcererStreetBattleContext.SelfCreature.Item == null)
                {
                    foreach (Card ActiveCard in Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner.ListCardInHand)
                    {
                        if (ActiveCard.CardType == InvaderItemCardType && ActiveCard.Path == InvaderItemCardPath)
                        {
                            Map.GlobalSorcererStreetBattleContext.SelfCreature.Item = (CreatureCard)ActiveCard;
                            break;
                        }
                    }
                }
            }

            string DefenderItemCardType = BR.ReadString();
            if (!string.IsNullOrEmpty(DefenderItemCardType))
            {
                string DefenderItemCardPath = BR.ReadString();

                if (Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item == null)
                {
                    foreach (Card ActiveCard in Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner.ListCardInHand)
                    {
                        if (ActiveCard.CardType == DefenderItemCardType && ActiveCard.Path == DefenderItemCardPath)
                        {
                            Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item = (CreatureCard)ActiveCard;
                            break;
                        }
                    }
                }
            }
        }

        public static void WritePlayerInfo(ByteWriter BW, SorcererStreetMap Map)
        {
            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.SelfCreature.PlayerIndex);
            BW.AppendString(Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.CardType);
            BW.AppendString(Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.Path);

            BW.AppendFloat(Map.GlobalSorcererStreetBattleContext.ActiveTerrain.GridPosition.X);
            BW.AppendFloat(Map.GlobalSorcererStreetBattleContext.ActiveTerrain.GridPosition.Y);
            BW.AppendFloat(Map.GlobalSorcererStreetBattleContext.ActiveTerrain.LayerIndex);

            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.CurrentHP);
            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.CurrentST);

            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.CurrentHP);
            BW.AppendInt32(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.CurrentST);

            if (Map.GlobalSorcererStreetBattleContext.SelfCreature.Item == null)
            {
                BW.AppendString(string.Empty);
            }
            else
            {
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.SelfCreature.Item.CardType);
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.SelfCreature.Item.Path);
            }
            if (Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item == null)
            {
                BW.AppendString(string.Empty);
            }
            else
            {
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item.CardType);
                BW.AppendString(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item.Path);
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            g.GraphicsDevice.Clear(Color.Black);

            if (Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation != null)
            {
                Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation.BeginDraw(g);
            }
            if (Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation != null)
            {
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation.BeginDraw(g);
            }

            g.End();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1, 0);
            Map.GlobalSorcererStreetBattleContext.Background.Draw(g, Constants.Width, Constants.Height);

            if (Map.GlobalSorcererStreetBattleContext.SelfCreature != null && Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation != null)
            {
                DrawInvaderBattle(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext, g);
                DrawDefenderBattle(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext, g);
            }
        }

        public static void DrawInvaderBattle(SpriteFont fntMenuText, SorcererStreetBattleContext GlobalSorcererStreetBattleContext, CustomSpriteBatch g)
        {
            //Left Card
            if (GlobalSorcererStreetBattleContext.SelfCreature.Animation != null)
            {
                GlobalSorcererStreetBattleContext.SelfCreature.Animation.Draw(g);
            }
            //Item Card
            if (GlobalSorcererStreetBattleContext.SelfCreature.Item != null)
            {
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.SelfCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, Constants.Height / 16, -ItemCardScale, ItemCardScale, false);
            }

            //ST Bar
            float CurrentY = Constants.Height - Constants.Height / 8;
            int BarWeight = Constants.Width / 4;
            int BarX = Constants.Width / 2  - BarWeight - Constants.Width / 10;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntMenuText, GlobalSorcererStreetBattleContext.SelfCreature.FinalST.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 50, CurrentY), 30, 30, Color.White);
            g.DrawString(fntMenuText, "ST", new Vector2(BarX + 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 80, CurrentY), BarWeight, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(BarX + 85, (int)CurrentY + 5, (int)Math.Min(BarWeight - 10, (BarWeight - 10) * InvaderSTBar / 100f), 20), Color.Blue);

            //HP Bar
            CurrentY +=30;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntMenuText, GlobalSorcererStreetBattleContext.SelfCreature.FinalHP.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 50, CurrentY), 30, 30, Color.White);
            g.DrawString(fntMenuText, "HP", new Vector2(BarX + 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 80, CurrentY), BarWeight, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(BarX + 85, (int)CurrentY + 5, (int)Math.Min(BarWeight - 10, (BarWeight - 10) * InvaderHPBar / 100f), 20), Color.Green);
        }

        public static void DrawDefenderBattle(SpriteFont fntMenuText, SorcererStreetBattleContext GlobalSorcererStreetBattleContext, CustomSpriteBatch g)
        {
            //Right Card
            if (GlobalSorcererStreetBattleContext.OpponentCreature.Animation != null)
            {
                GlobalSorcererStreetBattleContext.OpponentCreature.Animation.Draw(g);
            }
            //Item Card
            if (GlobalSorcererStreetBattleContext.OpponentCreature.Item != null)
            {
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.OpponentCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, Constants.Height / 16, - ItemCardScale, ItemCardScale, false);
            }

            //ST Bar
            float CurrentY = Constants.Height - Constants.Height / 8;
            int BarWeight = Constants.Width / 4;
            int BarX = Constants.Width / 2 + Constants.Width / 10;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntMenuText, GlobalSorcererStreetBattleContext.OpponentCreature.FinalST.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 50, CurrentY), 30, 30, Color.White);
            g.DrawString(fntMenuText, "ST", new Vector2(BarX + 55, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(BarX + 80, CurrentY), BarWeight, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(BarX + 85, (int)CurrentY + 5, (int)Math.Min(BarWeight - 10, (BarWeight - 10) * DefenderSTBar / 100f), 20), Color.Blue);

            //HP Bar
            CurrentY += 30;
            GameScreen.DrawBox(g, new Vector2(BarX, CurrentY), 50, 30, Color.White);
            g.DrawString(fntMenuText, GlobalSorcererStreetBattleContext.OpponentCreature.FinalHP.ToString(), new Vector2(BarX + 15, CurrentY + 5), Color.Black);
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
            g.DrawStringMiddleAligned(fntMenuText, GlobalSorcererStreetBattleContext.SelfCreature.Owner.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(fntMenuText, GlobalSorcererStreetBattleContext.OpponentCreature.Owner.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
            Y = Constants.Height / 4 + 35;
            g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 12, Y), new Vector2(Constants.Width - Constants.Width / 12, Y), Color.White);
            g.Draw(sprVS, new Rectangle(Constants.Width / 2 - 30, Y, 60, 60), Color.White);
            Y = Constants.Height / 4 + 40;
            g.DrawStringMiddleAligned(fntMenuText, GlobalSorcererStreetBattleContext.SelfCreature.Animation.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(fntMenuText, GlobalSorcererStreetBattleContext.OpponentCreature.Animation.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
        }
    }
}
