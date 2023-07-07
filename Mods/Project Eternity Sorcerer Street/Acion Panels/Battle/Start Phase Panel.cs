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

        private double AnimationTime;

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        protected CreatureCard Invader;

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
        }

        public override void OnSelect()
        {
            //Ensure something will look off if the Invader.Animation is not properly disposed at the end of a battle
            if (Map.GlobalSorcererStreetBattleContext.Invader.Animation == null)
            {
                Map.GlobalSorcererStreetBattleContext.ListBattlePanelHolder = ListActionMenuChoice;

                TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(ActivePlayer.GamePiece);

                Map.GlobalSorcererStreetBattleContext.Invader.Creature = Invader;
                Map.GlobalSorcererStreetBattleContext.Defender.Creature = ActiveTerrain.DefendingCreature;

                Map.GlobalSorcererStreetBattleContext.Invader.Owner = ActivePlayer;
                Map.GlobalSorcererStreetBattleContext.Invader.PlayerIndex = ActivePlayerIndex;
                Map.GlobalSorcererStreetBattleContext.Defender.Owner = ActiveTerrain.PlayerOwner;
                Map.GlobalSorcererStreetBattleContext.Defender.PlayerIndex = Map.ListPlayer.IndexOf(ActiveTerrain.PlayerOwner);
                Map.GlobalSorcererStreetBattleContext.DefenderTerrain = ActiveTerrain;

                Map.GlobalSorcererStreetBattleContext.Invader.Creature.InitBattleBonuses();
                Map.GlobalSorcererStreetBattleContext.Defender.Creature.InitBattleBonuses();

                Map.GlobalSorcererStreetBattleContext.Invader.FinalHP = Map.GlobalSorcererStreetBattleContext.Invader.Creature.CurrentHP;
                Map.GlobalSorcererStreetBattleContext.Defender.FinalHP = Map.GlobalSorcererStreetBattleContext.Defender.Creature.CurrentHP;
                Map.GlobalSorcererStreetBattleContext.Invader.FinalST = Map.GlobalSorcererStreetBattleContext.Invader.Creature.CurrentST;
                Map.GlobalSorcererStreetBattleContext.Defender.FinalST = Map.GlobalSorcererStreetBattleContext.Defender.Creature.CurrentST;

                Map.GlobalSorcererStreetBattleContext.Invader.Animation = new SimpleAnimation("Invader", "Invader", Invader.sprCard);
                Map.GlobalSorcererStreetBattleContext.Invader.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
                Map.GlobalSorcererStreetBattleContext.Invader.Animation.Scale = new Vector2(1);
                Map.GlobalSorcererStreetBattleContext.Defender.Animation = new SimpleAnimation("Defender", "Defender", ActiveTerrain.DefendingCreature.sprCard);
                Map.GlobalSorcererStreetBattleContext.Defender.Animation.Position = new Vector2(Constants.Width - Map.GlobalSorcererStreetBattleContext.Defender.Animation.StaticSprite.Width - Constants.Width / 9, Constants.Height / 12);
                Map.GlobalSorcererStreetBattleContext.Defender.Animation.Scale = new Vector2(1);

                Map.GlobalSorcererStreetBattleContext.Background = AnimationBackground.LoadAnimationBackground("Backgrounds 3D/Grass", Map.Content, GameScreen.GraphicsDevice);

                InvaderHPBar = Map.GlobalSorcererStreetBattleContext.Invader.FinalHP;
                InvaderSTBar = Map.GlobalSorcererStreetBattleContext.Invader.FinalST;
                DefenderHPBar = Map.GlobalSorcererStreetBattleContext.Defender.FinalHP;
                DefenderSTBar = Map.GlobalSorcererStreetBattleContext.Defender.FinalST;
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
                else
                {
                    IntroduceInvader(g);
                    IntroduceDefender(g);
                    DisplayVersusText(g);
                }
            }
        }

        public void IntroduceInvader(CustomSpriteBatch g)
        {
            //Spin card from the left
            float MaxScale = 1f;
            
            float RealRotationTimer = (float)AnimationTime - 2;
            float FinalX = Constants.Width / 9 + Map.GlobalSorcererStreetBattleContext.Invader.Creature.sprCard.Width / 2;
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

            Card.DrawCardMiniature(g, Map.GlobalSorcererStreetBattleContext.Invader.Creature.sprCard, MenuHelper.sprCardBack, Color.White, X, Y, FinalScale, MaxScale, RealRotationTimer < MathHelper.Pi);
        }

        public void IntroduceDefender(CustomSpriteBatch g)
        {
            //Spin card from the right
            float MaxScale = 1f;

            float RealRotationTimer = (float)AnimationTime - 4;
            float FinalX = Constants.Width - Constants.Width / 9 - Map.GlobalSorcererStreetBattleContext.Defender.Creature.sprCard.Width / 2;
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

            Card.DrawCardMiniature(g, Map.GlobalSorcererStreetBattleContext.Defender.Creature.sprCard, MenuHelper.sprCardBack, Color.White, X, Y, FinalScale, MaxScale, RealRotationTimer < MathHelper.Pi);
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
