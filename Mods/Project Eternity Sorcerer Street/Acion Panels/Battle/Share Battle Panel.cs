using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class ActionPanelBattle : ActionPanelSorcererStreet
    {
        public class BattleContent
        {
            public SpriteFont fntMenuText;

            public Texture2D sprBarBackground;
            public Texture2D sprBlueBar;
            public Texture2D sprGreenBar;
            public Texture2D sprRedBar;
            public Texture2D sprYellowBar;
            public Texture2D sprHP;
            public Texture2D sprST;

            public Texture2D sprBattle;
            public Texture2D sprResult;
            public Texture2D sprVS;

            public BattleContent(ContentManager Content)
            {
                fntMenuText = Content.Load<SpriteFont>("Fonts/Oxanium Bold Bigger");

                sprBarBackground = Content.Load<Texture2D>("Sorcerer Street/Ressources/Battle/Bar Background");
                sprBlueBar = Content.Load<Texture2D>("Sorcerer Street/Ressources/Battle/Blue Bar");
                sprGreenBar = Content.Load<Texture2D>("Sorcerer Street/Ressources/Battle/Green Bar");
                sprRedBar = Content.Load<Texture2D>("Sorcerer Street/Ressources/Battle/Red Bar");
                sprYellowBar = Content.Load<Texture2D>("Sorcerer Street/Ressources/Battle/Yellow Bar");
                sprHP = Content.Load<Texture2D>("Sorcerer Street/Ressources/Battle/HP");
                sprST = Content.Load<Texture2D>("Sorcerer Street/Ressources/Battle/ST");
                sprBattle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Battle/Battle");
                sprResult = Content.Load<Texture2D>("Sorcerer Street/Ressources/Battle/Result");
                sprVS = Content.Load<Texture2D>("Sorcerer Street/Ressources/Battle/VS");
            }
        }

        private const float ItemCardScale = 0.4f;

        protected static int InvaderHPBar;
        protected static int DefenderHPBar;
        protected static int InvaderSTBar;
        protected static int DefenderSTBar;

        private BattleContent BattleAssets;

        public ActionPanelBattle(SorcererStreetMap Map, string PanelName)
            : base(PanelName, Map, false)
        {
            BattleAssets = new BattleContent(Map.Content);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext);
        }

        public static bool HasFinishedUpdatingBars(GameTime gameTime, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            if (InvaderHPBar > GlobalSorcererStreetBattleContext.InvaderCreature.FinalHP)
            {
                --InvaderHPBar;
                return false;
            }
            else if (InvaderSTBar > GlobalSorcererStreetBattleContext.InvaderCreature.FinalST)
            {
                --InvaderSTBar;
                return false;
            }
            else if (DefenderHPBar > GlobalSorcererStreetBattleContext.DefenderCreature.FinalHP)
            {
                --DefenderHPBar;
                return false;
            }
            else if (DefenderSTBar > GlobalSorcererStreetBattleContext.DefenderCreature.FinalST)
            {
                --DefenderSTBar;
                return false;
            }

            if (InvaderHPBar < GlobalSorcererStreetBattleContext.InvaderCreature.FinalHP)
            {
                ++InvaderHPBar;
                return false;
            }
            else if (InvaderSTBar < GlobalSorcererStreetBattleContext.InvaderCreature.FinalST)
            {
                ++InvaderSTBar;
                return false;
            }
            else if (DefenderHPBar < GlobalSorcererStreetBattleContext.DefenderCreature.FinalHP)
            {
                ++DefenderHPBar;
                return false;
            }
            else if (DefenderSTBar < GlobalSorcererStreetBattleContext.DefenderCreature.FinalST)
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
            int InvaderPlayerIndex = BR.ReadInt32();

            string CardType = BR.ReadString();
            string CardPath = BR.ReadString();
            CreatureCard Creature = null;
            foreach (Card ActiveCard in Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner.ListCardInHand)
            {
                if (ActiveCard.CardType == CardType && ActiveCard.Path == CardPath)
                {
                    Creature = (CreatureCard)ActiveCard;
                    break;
                }
            }

            TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));

            if (Map.GlobalSorcererStreetBattleContext.SelfCreature == null)
            {
                SorcererStreetBattleContext.BattleCreatureInfo InvaderCreature = new SorcererStreetBattleContext.BattleCreatureInfo();
                SorcererStreetBattleContext.BattleCreatureInfo DefenderCreature = new SorcererStreetBattleContext.BattleCreatureInfo();

                InvaderCreature.Creature = Creature;
                InvaderCreature.Owner = Map.ListPlayer[Map.GlobalSorcererStreetBattleContext.SelfCreature.PlayerIndex];
                DefenderCreature.PlayerIndex = InvaderPlayerIndex;
                DefenderCreature.Creature = ActiveTerrain.DefendingCreature;
                DefenderCreature.Owner = ActiveTerrain.PlayerOwner;
                DefenderCreature.PlayerIndex = Map.ListPlayer.IndexOf(ActiveTerrain.PlayerOwner);

                InvaderCreature.OwnerTeam = Map.DicTeam[InvaderCreature.Owner.TeamIndex];
                DefenderCreature.OwnerTeam = Map.DicTeam[ActiveTerrain.PlayerOwner.TeamIndex];

                Map.GlobalSorcererStreetBattleContext.ActiveTerrain = ActiveTerrain;
                Map.GlobalSorcererStreetBattleContext.ListBoostCreature = ActionPanelBattleBoostsModifierPhase.GetListBoostCreatures(Map);

                Map.GlobalSorcererStreetBattleContext.ListBattlePanelHolder = Map.ListActionMenuChoice;

                if (!Map.IsServer)
                {
                    InvaderCreature.Animation = new SimpleAnimation("Invader", "Invader", InvaderCreature.Creature.sprCard);
                    InvaderCreature.Animation.Position = new Vector2(37, 48);
                    InvaderCreature.Animation.Scale = new Vector2(0.6f);
                    DefenderCreature.Animation = new SimpleAnimation("Defender", "Defender", ActiveTerrain.DefendingCreature.sprCard);
                    DefenderCreature.Animation.Position = new Vector2(Constants.Width - 282, 48);
                    DefenderCreature.Animation.Scale = new Vector2(0.6f);

                    Map.GlobalSorcererStreetBattleContext.Background = AnimationBackground.LoadAnimationBackground("Backgrounds 3D/Grass", Map.Content, GameScreen.GraphicsDevice);
                }

                InvaderCreature.Creature.CurrentHP = BR.ReadInt32();
                InvaderCreature.Creature.CurrentST = BR.ReadInt32();

                DefenderCreature.Creature.CurrentHP = BR.ReadInt32();
                DefenderCreature.Creature.CurrentST = BR.ReadInt32();

                InvaderCreature.BonusHP = 0;
                DefenderCreature.BonusHP = 0;
                InvaderCreature.BonusST = 0;
                DefenderCreature.BonusST = 0;

                InvaderHPBar = InvaderCreature.FinalHP;
                InvaderSTBar = InvaderCreature.FinalST;
                DefenderHPBar = DefenderCreature.FinalHP;
                DefenderSTBar = DefenderCreature.FinalST;

                Map.GlobalSorcererStreetBattleContext.SetBeforeBattle(InvaderCreature, DefenderCreature);
            }

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
                DrawInvaderBattle(g, BattleAssets, Map.GlobalSorcererStreetBattleContext);
                DrawDefenderBattle(g, BattleAssets, Map.GlobalSorcererStreetBattleContext);
            }
        }

        public static void DrawInvaderBattle(CustomSpriteBatch g, BattleContent BattleAssets, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            //Left Card
            if (GlobalSorcererStreetBattleContext.InvaderCreature.Animation != null)
            {
                GlobalSorcererStreetBattleContext.InvaderCreature.Animation.Draw(g);
            }
            //Item Card
            if (GlobalSorcererStreetBattleContext.InvaderCreature.Item != null)
            {
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.InvaderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, Constants.Height / 16, -ItemCardScale, ItemCardScale, false);
            }

            float Ratio = Constants.Height / 720f;

            //ST Bar
            float CurrentY = (int)(580 * Ratio);
            int BarWeight = (int)(304 * Ratio);
            int BarX = (int)(80 * Ratio);

            g.Draw(BattleAssets.sprBarBackground, new Vector2(BarX, CurrentY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(BattleAssets.fntMenuText, GlobalSorcererStreetBattleContext.InvaderCreature.FinalST.ToString(),
                new Vector2((int)(BarX + 40 * Ratio), (int)(CurrentY + 10 * Ratio)), Color.White);

            g.Draw(BattleAssets.sprST, new Vector2((int)(BarX + 130 * Ratio), (int)(CurrentY + 10 * Ratio)), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.Draw(BattleAssets.sprBlueBar, new Rectangle((int)(BarX + 190 * Ratio), (int)(CurrentY + 12 * Ratio), (int)Math.Min(BarWeight - 10, (BarWeight - 10) * InvaderSTBar / 100f), (int)(20 * Ratio)), null, Color.White);

            //HP Bar
            CurrentY += (int)(50 * Ratio);
            g.Draw(BattleAssets.sprBarBackground, new Vector2(BarX, CurrentY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(BattleAssets.fntMenuText, GlobalSorcererStreetBattleContext.InvaderCreature.FinalHP.ToString(), new Vector2((int)(BarX + 40 * Ratio), (int)(CurrentY + 10 * Ratio)), Color.White);
            g.Draw(BattleAssets.sprHP, new Vector2((int)(BarX + 130 * Ratio), (int)(CurrentY + 10 * Ratio)), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.Draw(BattleAssets.sprGreenBar, new Rectangle((int)(BarX + 190 * Ratio), (int)(CurrentY + 12 * Ratio), (int)Math.Min(BarWeight - 10, (BarWeight - 10) * InvaderHPBar / 100f), (int)(20 * Ratio)), Color.White);
        }

        public static void DrawDefenderBattle(CustomSpriteBatch g, BattleContent BattleAssets, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            //Right Card
            if (GlobalSorcererStreetBattleContext.DefenderCreature.Animation != null)
            {
                GlobalSorcererStreetBattleContext.DefenderCreature.Animation.Draw(g);
            }
            //Item Card
            if (GlobalSorcererStreetBattleContext.DefenderCreature.Item != null)
            {
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.DefenderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, Constants.Height / 16, - ItemCardScale, ItemCardScale, false);
            }


            float Ratio = Constants.Height / 720f;

            //ST Bar
            float CurrentY = (int)(580 * Ratio);
            int BarWeight = (int)(304 * Ratio);
            int BarX = (int)(Constants.Width - BattleAssets.sprBarBackground.Width * Ratio - 80 * Ratio);

            int STBarWidth = (int)Math.Min(BarWeight - 10, (BarWeight - 10) * DefenderSTBar / 100f);
            g.Draw(BattleAssets.sprBarBackground, new Vector2(BarX, CurrentY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(BattleAssets.fntMenuText, GlobalSorcererStreetBattleContext.DefenderCreature.FinalST.ToString(),
                new Vector2((int)(BarX + 40 * Ratio), (int)(CurrentY + 10 * Ratio)), Color.White);

            g.Draw(BattleAssets.sprST, new Vector2((int)(BarX + 130 * Ratio), (int)(CurrentY + 10 * Ratio)), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.Draw(BattleAssets.sprBlueBar, new Rectangle((int)(BarX + 184 * Ratio) + BarWeight - STBarWidth, (int)(CurrentY + 12 * Ratio), STBarWidth, (int)(20 * Ratio)), Color.White);

            //HP Bar
            int HPBarWidth = (int)Math.Min(BarWeight - 10, (BarWeight - 10) * DefenderHPBar / 100f);
            CurrentY += (int)(50 * Ratio);
            g.Draw(BattleAssets.sprBarBackground, new Vector2(BarX, CurrentY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(BattleAssets.fntMenuText, GlobalSorcererStreetBattleContext.DefenderCreature.FinalHP.ToString(), new Vector2((int)(BarX + 40 * Ratio), (int)(CurrentY + 10 * Ratio)), Color.White);
            g.Draw(BattleAssets.sprHP, new Vector2((int)(BarX + 130 * Ratio), (int)(CurrentY + 10 * Ratio)), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.Draw(BattleAssets.sprGreenBar, new Rectangle((int)(BarX + 184 * Ratio) + BarWeight - HPBarWidth, (int)(CurrentY + 12 * Ratio), HPBarWidth, (int)(20 * Ratio)), Color.White);
        }

        public static void DisplayVersusText(CustomSpriteBatch g, BattleContent BattleAssets, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            float Ratio = Constants.Height / 720f;
            int Y = Constants.Height / 4 - 25;
            g.Draw(BattleAssets.sprBattle, new Vector2(Constants.Width / 2, Y + 16 * Ratio), null, Color.White, 0f, new Vector2((int)(BattleAssets.sprBattle.Width / 2f), BattleAssets.sprBattle.Height), 1f, SpriteEffects.None, 0f);
            Y = Constants.Height / 4;
            MenuHelper.DrawBox(g, new Vector2(Constants.Width / 16, Y), Constants.Width - Constants.Width / 8, Constants.Height / 5, Ratio);
            Y = Constants.Height / 4 + 10;
            g.DrawStringMiddleAligned(BattleAssets.fntMenuText, GlobalSorcererStreetBattleContext.InvaderCreature.Owner.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(BattleAssets.fntMenuText, GlobalSorcererStreetBattleContext.DefenderCreature.Owner.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
            Y = Constants.Height / 4 + 35;
            g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 12, Y), new Vector2(Constants.Width - Constants.Width / 12, Y), Color.White);
            g.Draw(BattleAssets.sprVS, new Vector2(Constants.Width / 2 - 30, Y + 40 * Ratio), null, Color.White, 0f, new Vector2(BattleAssets.sprVS.Width / 2, BattleAssets.sprVS.Height / 2), 1f, SpriteEffects.None, 0f);
            Y = Constants.Height / 4 + 40;
            g.DrawStringMiddleAligned(BattleAssets.fntMenuText, GlobalSorcererStreetBattleContext.InvaderCreature.Animation.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(BattleAssets.fntMenuText, GlobalSorcererStreetBattleContext.DefenderCreature.Animation.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
        }
    }
}
