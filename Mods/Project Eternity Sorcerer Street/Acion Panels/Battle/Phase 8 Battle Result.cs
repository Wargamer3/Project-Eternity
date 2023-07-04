using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleBattleResultPhase : ActionPanelBattle
    {
        public static string RequirementName = "Sorcerer Street Battle Result Phase";

        private const string PanelName = "BattleBattleResultPhase";

        private double AnimationTime;

        public ActionPanelBattleBattleResultPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
            this.Map = Map;
        }
        
        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!CanUpdate(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            Map.GlobalSorcererStreetBattleContext.Background.Update(gameTime);

            if (AnimationTime < 1)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (AnimationTime < 4.2f)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                Map.GlobalSorcererStreetBattleContext.Invader.Animation = null;
                Map.GlobalSorcererStreetBattleContext.Defender.Animation = null;
            }
            else if (AnimationTime < 6)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                Map.GlobalSorcererStreetBattleContext.Background.MoveSpeed = new Vector3(0, 0, -1);
            }
            else
            {
                RemoveFromPanelList(this);
                if (Map.GlobalSorcererStreetBattleContext.Defender.Creature.CurrentHP > 0)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelBattleDefenderWinPhase(Map));
                }
                else
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelBattleDefenderDefeatedPhase(Map));
                }
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            if (!CanUpdate(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            Map.GlobalSorcererStreetBattleContext.Background.Update(gameTime);

            if (AnimationTime < 1)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (AnimationTime < 4.2f)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                Map.GlobalSorcererStreetBattleContext.Invader.Animation = null;
                Map.GlobalSorcererStreetBattleContext.Defender.Animation = null;
            }
            else if (AnimationTime < 6)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                Map.GlobalSorcererStreetBattleContext.Background.MoveSpeed = new Vector3(0, 0, -1);
            }
        }

        protected override void OnCancelPanel()
        {
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
            return new ActionPanelBattleBattleResultPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);

            if (AnimationTime < 1)
            {
                if (Map.GlobalSorcererStreetBattleContext.Defender.Creature.CurrentHP > 0)
                {
                    DefenderReturnToTerrain(g);
                    if (Map.GlobalSorcererStreetBattleContext.Invader.Creature.CurrentHP > 0)
                    {
                        InvaderLoseAndLeave(g);
                    }
                }
                else
                {
                    if (Map.GlobalSorcererStreetBattleContext.Invader.Creature.CurrentHP > 0)
                    {
                        InvaderWinAndLeave(g);
                    }
                }
            }
            else if (AnimationTime < 4)
            {
                DisplayVersusText(g);
            }
        }

        private void KillInvader(CustomSpriteBatch g)
        {
            //Play card destruction animation
        }

        private void KillDefender(CustomSpriteBatch g)
        {
            //Play card destruction animation
        }

        private void InvaderWinAndLeave(CustomSpriteBatch g)
        {
            //Spin card from the left
            float MaxScale = 1f;
            float RealRotationTimer = (float)AnimationTime / 0.7f;
            float FinalX = Constants.Width + 200;
            float StartX = Constants.Width / 9;
            float DistanceX = FinalX - StartX;
            RealRotationTimer *= (float)Math.Sin(RealRotationTimer * MathHelper.PiOver4);
            float X = StartX + (RealRotationTimer) * DistanceX;
            float Y = 48 + RealRotationTimer * 45;
            MaxScale += RealRotationTimer / 9;

            Map.GlobalSorcererStreetBattleContext.Invader.Animation.Position = new Vector2(X, Y);
            Map.GlobalSorcererStreetBattleContext.Invader.Animation.Scale = new Vector2(MaxScale);
        }

        private void InvaderLoseAndLeave(CustomSpriteBatch g)
        {
            //Spin card from the left
            float MaxScale = 1f;
            float RealRotationTimer = (float)AnimationTime / 0.7f;
            float FinalX = - 200;
            float StartX = Constants.Width / 9;
            float DistanceX = FinalX - StartX;
            RealRotationTimer *= (float)Math.Sin(RealRotationTimer * MathHelper.PiOver4);
            float X = StartX + (RealRotationTimer) * DistanceX;
            float Y = 48 + RealRotationTimer * 45;
            MaxScale -= RealRotationTimer / 9;

            Map.GlobalSorcererStreetBattleContext.Invader.Animation.Position = new Vector2(X, Y);
            Map.GlobalSorcererStreetBattleContext.Invader.Animation.Scale = new Vector2(MaxScale);
        }

        private void DefenderReturnToTerrain(CustomSpriteBatch g)
        {
            //Spin card from the left
            float MaxScale = 1f;
            float RealRotationTimer = (float)AnimationTime / 0.7f;
            float FinalX = Constants.Width + 200;
            float StartX = Constants.Width - Map.GlobalSorcererStreetBattleContext.Defender.Creature.sprCard.Width - Constants.Width / 9;
            float DistanceX = FinalX - StartX;
            RealRotationTimer *= (float)Math.Sin(RealRotationTimer * MathHelper.PiOver4);
            float X = StartX + (RealRotationTimer) * DistanceX;
            float Y = 48 + RealRotationTimer * 45;
            MaxScale += RealRotationTimer / 9;

            Map.GlobalSorcererStreetBattleContext.Defender.Animation.Position = new Vector2(X, Y);
            Map.GlobalSorcererStreetBattleContext.Defender.Animation.Scale = new Vector2(MaxScale);
        }

        private void DisplayVersusText(CustomSpriteBatch g)
        {
            int Y = Constants.Height / 4 - 25;
            TextHelper.DrawTextMiddleAligned(g, "BATTLE", new Vector2(Constants.Width / 2, Y), Color.White);
            Y = Constants.Height / 4;
            MenuHelper.DrawBox(g, new Vector2(Constants.Width / 16, Y), Constants.Width - Constants.Width / 8, Constants.Height / 5);
            Y = Constants.Height / 4 + 10;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.Owner.Name + "'s", new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.Owner.Name + "'s", new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
            Y = Constants.Height / 4 + 35;
            g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 12, Y), new Vector2(Constants.Width - Constants.Width / 12, Y), Color.White);
            Y = Constants.Height / 4 + 40;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.Creature.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.Creature.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);

            Y += 30;
            if (Map.GlobalSorcererStreetBattleContext.Defender.Creature.CurrentHP > 0)
            {
                if (Map.GlobalSorcererStreetBattleContext.Invader.Creature.CurrentHP > 0)
                {
                    g.DrawStringMiddleAligned(Map.fntArial12, "survived", new Vector2(Constants.Width / 4, Y), Color.White);
                }
                else
                {
                    g.DrawStringMiddleAligned(Map.fntArial12, "was destroyed", new Vector2(Constants.Width / 4, Y), Color.White);
                }
                g.DrawStringMiddleAligned(Map.fntArial12, "defended the land", new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
            }
            else
            {
                g.DrawStringMiddleAligned(Map.fntArial12, "took the land", new Vector2(Constants.Width / 4, Y), Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, "was destroyed", new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
            }
        }
    }
}
