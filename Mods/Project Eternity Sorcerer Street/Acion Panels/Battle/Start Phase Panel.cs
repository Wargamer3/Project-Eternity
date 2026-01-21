using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleStartPhase : ActionPanelBattle
    {
        private const string PanelName = "StartBattle";

        private static double AnimationTime;

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        protected CreatureCard Invader;
        private static AnimationScreen InvaderAnimation;
        BattleContent BattleAssets;

        public ActionPanelBattleStartPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
        }

        public ActionPanelBattleStartPhase(SorcererStreetMap Map, int ActivePlayerIndex, CreatureCard Invader)
            : base(Map, PanelName)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            this.Invader = Invader;
            BattleAssets = new BattleContent(Map.Content);
        }

        public override void OnSelect()
        {
            //Ensure something will look off if the Invader.Animation is not properly disposed at the end of a battle
            if (Map.GlobalSorcererStreetBattleContext.SelfCreature == null)
            {
                SorcererStreetBattleContext.BattleCreatureInfo InvaderCreature = new SorcererStreetBattleContext.BattleCreatureInfo();
                SorcererStreetBattleContext.BattleCreatureInfo DefenderCreature = new SorcererStreetBattleContext.BattleCreatureInfo();

                TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(ActivePlayer.GamePiece.Position);

                Map.GlobalSorcererStreetBattleContext.ListBattlePanelHolder = ListActionMenuChoice;

                Map.GlobalSorcererStreetBattleContext.DicCreatureCountByElementType = Map.DicCreatureCountByElementType;
                Map.GlobalSorcererStreetBattleContext.ListSummonedCreature = Map.ListSummonedCreature;
                Map.GlobalSorcererStreetBattleContext.TotalCreaturesDestroyed = Map.TotalCreaturesDestroyed;
                Map.GlobalSorcererStreetBattleContext.CurrentTurn = Map.GameTurn;
                Map.GlobalSorcererStreetBattleContext.ActiveTerrain = ActiveTerrain;


                InvaderCreature.Creature = Invader;
                DefenderCreature.Creature = ActiveTerrain.DefendingCreature;

                InvaderCreature.Owner = ActivePlayer;
                InvaderCreature.PlayerIndex = ActivePlayerIndex;
                DefenderCreature.Owner = ActiveTerrain.PlayerOwner;
                DefenderCreature.PlayerIndex = Map.ListPlayer.IndexOf(ActiveTerrain.PlayerOwner);

                InvaderCreature.BonusHP = 0;
                DefenderCreature.BonusHP = 0;
                InvaderCreature.BonusST = 0;
                DefenderCreature.BonusST = 0;

                InvaderCreature.Animation = new SimpleAnimation("Invader", "Invader", Invader.sprCard);
                InvaderCreature.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
                InvaderCreature.Animation.Scale = new Vector2(1);
                DefenderCreature.Animation = new SimpleAnimation("Defender", "Defender", ActiveTerrain.DefendingCreature.sprCard);
                DefenderCreature.Animation.Position = new Vector2(Constants.Width - DefenderCreature.Animation.StaticSprite.Width - Constants.Width / 9, Constants.Height / 12);
                DefenderCreature.Animation.Scale = new Vector2(1);

                Map.GlobalSorcererStreetBattleContext.Background = AnimationBackground.LoadAnimationBackground("Backgrounds 3D/Grass", Map.Content, GameScreen.GraphicsDevice);

                InvaderHPBar = InvaderCreature.FinalHP;
                InvaderSTBar = InvaderCreature.FinalST;
                DefenderHPBar = DefenderCreature.FinalHP;
                DefenderSTBar = DefenderCreature.FinalST;

                Map.GlobalSorcererStreetBattleContext.SetBeforeBattle(InvaderCreature, DefenderCreature);
            }
            else
            {

            }
        }

        public static void InitIntroAnimation(SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            AnimationTime = 0;
            if (!GlobalSorcererStreetBattleContext.SelfCreature.Creature.UseCardAnimation)
            {
                ActionPanelBattleAttackAnimationPhase.InitAnimation(true, GlobalSorcererStreetBattleContext.SelfCreature.Creature.MoveInAnimationPath, GlobalSorcererStreetBattleContext.SelfCreature, true);
                GlobalSorcererStreetBattleContext.SelfCreature.Animation.Position = new Vector2(1750, 190);
            }
        }

        public override void DoUpdate(GameTime gameTime)
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
            else
            {
                RemoveFromPanelList(this);
                ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelBattleItemSelectionPhase(Map, ActivePlayerIndex));
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            DoUpdate(gameTime);
        }

        public static bool UpdateAnimation(GameTime gameTime, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            if (AnimationTime < 2)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                return true;
            }
            else if (AnimationTime < 8)
            {
                if (GlobalSorcererStreetBattleContext.SelfCreature.Animation != null)
                {
                    GlobalSorcererStreetBattleContext.SelfCreature.Animation.Update(gameTime);
                }

                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                return true;
            }
            else
            {
                return false;
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

            DrawAnimation(g, BattleAssets, Map.GlobalSorcererStreetBattleContext);
        }

        public static void DrawAnimation(CustomSpriteBatch g, BattleContent BattleAssets, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            if (AnimationTime >= 2)
            {
                if (AnimationTime < 4)
                {
                    IntroduceInvader(g, GlobalSorcererStreetBattleContext);
                }
                else if (AnimationTime < 6)
                {
                    IntroduceInvader(g, GlobalSorcererStreetBattleContext);
                    IntroduceDefender(g, GlobalSorcererStreetBattleContext);
                }
                else
                {
                    IntroduceInvader(g, GlobalSorcererStreetBattleContext);
                    IntroduceDefender(g, GlobalSorcererStreetBattleContext);
                    DisplayVersusText(g, BattleAssets, GlobalSorcererStreetBattleContext);
                }
            }
        }

        public static void IntroduceInvader(CustomSpriteBatch g, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            if (GlobalSorcererStreetBattleContext.SelfCreature.Creature.UseCardAnimation)
            {
                //Spin card from the left
                float MaxScale = 1f;

                float RealRotationTimer = (float)AnimationTime - 2;
                float FinalX = Constants.Width / 9 + GlobalSorcererStreetBattleContext.SelfCreature.Creature.sprCard.Width / 2;
                float StartX = -10;
                float DistanceX = FinalX - StartX;
                float X = StartX + (RealRotationTimer) * DistanceX;
                float Y = Constants.Height / 12;
                RealRotationTimer *= 5;
                float FinalScale = (float)Math.Sin(RealRotationTimer) * MaxScale;
                if (X > FinalX)
                {
                    FinalScale = -MaxScale;
                    RealRotationTimer = MathHelper.Pi;
                    X = FinalX;
                }

                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.SelfCreature.Creature.sprCard, MenuHelper.sprCardBack, Color.White, X, Y, FinalScale, MaxScale, RealRotationTimer < MathHelper.Pi);
            }
            else if (GlobalSorcererStreetBattleContext.SelfCreature.Animation != null)
            {
                GlobalSorcererStreetBattleContext.SelfCreature.Animation.Draw(g);
            }
        }

        public static void IntroduceDefender(CustomSpriteBatch g, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            //Spin card from the right
            float MaxScale = 1f;

            float RealRotationTimer = (float)AnimationTime - 4;
            float FinalX = Constants.Width - Constants.Width / 9 - GlobalSorcererStreetBattleContext.OpponentCreature.Creature.sprCard.Width / 2;
            float StartX = Constants.Width + 10;
            float DistanceX = FinalX - StartX;
            float X = StartX + (RealRotationTimer) * DistanceX;
            float Y = Constants.Height / 12;
            RealRotationTimer *= 5;
            float FinalScale = (float)Math.Sin(RealRotationTimer) * MaxScale;
            if (X < FinalX)
            {
                FinalScale = -MaxScale;
                RealRotationTimer = MathHelper.Pi;
                X = FinalX;
            }

            Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.OpponentCreature.Creature.sprCard, MenuHelper.sprCardBack, Color.White, X, Y, FinalScale, MaxScale, RealRotationTimer < MathHelper.Pi);
        }

        public override void DoRead(ByteReader BR)
        {
            ReadPlayerInfo(BR, Map);
        }

        public override void DoWrite(ByteWriter BW)
        {
            WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleStartPhase(Map);
        }
    }
}
