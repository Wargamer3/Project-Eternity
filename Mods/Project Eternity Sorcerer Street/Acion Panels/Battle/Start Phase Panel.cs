using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
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
            //Ensure something will look off if the InvaderCard is not properly disposed at the end of a battle
            if (Map.GlobalSorcererStreetBattleContext.InvaderCard == null)
            {
                Map.GlobalSorcererStreetBattleContext.ListBattlePanelHolder = ListActionMenuChoice;

                TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(ActivePlayer.GamePiece);

                Map.GlobalSorcererStreetBattleContext.Invader = Invader;
                Map.GlobalSorcererStreetBattleContext.Defender = ActiveTerrain.DefendingCreature;

                Map.GlobalSorcererStreetBattleContext.InvaderPlayer = ActivePlayer;
                Map.GlobalSorcererStreetBattleContext.InvaderPlayerIndex = ActivePlayerIndex;
                Map.GlobalSorcererStreetBattleContext.DefenderPlayer = ActiveTerrain.PlayerOwner;
                Map.GlobalSorcererStreetBattleContext.DefenderPlayerIndex = Map.ListPlayer.IndexOf(ActiveTerrain.PlayerOwner);
                Map.GlobalSorcererStreetBattleContext.DefenderTerrain = ActiveTerrain;

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

                Map.GlobalSorcererStreetBattleContext.Background = AnimationBackground.LoadAnimationBackground("Backgrounds 3D/Grass", Map.Content, GameScreen.GraphicsDevice);

                InvaderHP = Map.GlobalSorcererStreetBattleContext.InvaderFinalHP;
                InvaderST = Map.GlobalSorcererStreetBattleContext.InvaderFinalST;
                DefenderHP = Map.GlobalSorcererStreetBattleContext.DefenderFinalHP;
                DefenderST = Map.GlobalSorcererStreetBattleContext.DefenderFinalST;
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
            }
        }

        public void IntroduceInvader(CustomSpriteBatch g)
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

        public void IntroduceDefender(CustomSpriteBatch g)
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

            Card.DrawCardMiniature(g, Map.GlobalSorcererStreetBattleContext.Defender.sprCard, Map.sprCardBack, Color.White, X, Y, FinalScale, MaxScale, RealRotationTimer);
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
