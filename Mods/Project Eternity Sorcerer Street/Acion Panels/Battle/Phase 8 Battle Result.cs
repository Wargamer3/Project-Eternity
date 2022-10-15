﻿using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleBattleResultPhase : BattleMapActionPanel
    {
        private const string PanelName = "BattleBattleResultPhase";

        public static string RequirementName = "Sorcerer Street Battle Result Phase";

        private readonly SorcererStreetMap Map;

        private double AnimationTime;

        public ActionPanelBattleBattleResultPhase(SorcererStreetMap Map)
            : base(PanelName, Map.ListActionMenuChoice, null, false)
        {
            this.Map = Map;
        }

        public ActionPanelBattleBattleResultPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map)
            : base(PanelName, ListActionMenuChoice, null, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (AnimationTime < 1)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (AnimationTime < 4.2f)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                Map.GlobalSorcererStreetBattleContext.InvaderCard = null;
                Map.GlobalSorcererStreetBattleContext.DefenderCard = null;
            }
            else if (AnimationTime < 6)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                Map.GlobalSorcererStreetBattleContext.Background.MoveSpeed = new Vector3(0, 0, -1);
            }
            else
            {
                RemoveFromPanelList(this);
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleBattleResultPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (AnimationTime < 1)
            {
                if (Map.GlobalSorcererStreetBattleContext.Defender.CurrentHP > 0)
                {
                    DefenderReturnToTerrain(g);
                    if (Map.GlobalSorcererStreetBattleContext.Invader.CurrentHP > 0)
                    {
                        InvaderLoseAndLeave(g);
                    }
                }
                else
                {
                    if (Map.GlobalSorcererStreetBattleContext.Invader.CurrentHP > 0)
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

        }

        private void KillDefender(CustomSpriteBatch g)
        {

        }

        private void InvaderWinAndLeave(CustomSpriteBatch g)
        {
            //Spin card from the left
            float MaxScale = 0.6f;
            float RealRotationTimer = (float)AnimationTime / 0.7f;
            float FinalX = Constants.Width + 200;
            float StartX = 37;
            float DistanceX = FinalX - StartX;
            RealRotationTimer *= (float)Math.Sin(RealRotationTimer * MathHelper.PiOver4);
            float X = StartX + (RealRotationTimer) * DistanceX;
            float Y = 48 + RealRotationTimer * 45;
            MaxScale += RealRotationTimer / 9;

            Map.GlobalSorcererStreetBattleContext.InvaderCard.Position = new Vector2(X, Y);
            Map.GlobalSorcererStreetBattleContext.InvaderCard.Scale = new Vector2(MaxScale);
        }

        private void InvaderLoseAndLeave(CustomSpriteBatch g)
        {
            //Spin card from the left
            float MaxScale = 0.6f;
            float RealRotationTimer = (float)AnimationTime / 0.7f;
            float FinalX = - 200;
            float StartX = 37;
            float DistanceX = FinalX - StartX;
            RealRotationTimer *= (float)Math.Sin(RealRotationTimer * MathHelper.PiOver4);
            float X = StartX + (RealRotationTimer) * DistanceX;
            float Y = 48 + RealRotationTimer * 45;
            MaxScale -= RealRotationTimer / 9;

            Map.GlobalSorcererStreetBattleContext.InvaderCard.Position = new Vector2(X, Y);
            Map.GlobalSorcererStreetBattleContext.InvaderCard.Scale = new Vector2(MaxScale);
        }

        private void DefenderReturnToTerrain(CustomSpriteBatch g)
        {
            //Spin card from the left
            float MaxScale = 0.6f;
            float RealRotationTimer = (float)AnimationTime / 0.7f;
            float FinalX = Constants.Width + 200;
            float StartX = Constants.Width - 282;
            float DistanceX = FinalX - StartX;
            RealRotationTimer *= (float)Math.Sin(RealRotationTimer * MathHelper.PiOver4);
            float X = StartX + (RealRotationTimer) * DistanceX;
            float Y = 48 + RealRotationTimer * 45;
            MaxScale += RealRotationTimer / 9;

            Map.GlobalSorcererStreetBattleContext.DefenderCard.Position = new Vector2(X, Y);
            Map.GlobalSorcererStreetBattleContext.DefenderCard.Scale = new Vector2(MaxScale);
        }

        private void DisplayVersusText(CustomSpriteBatch g)
        {
            int Y = Constants.Height / 4 - 25;
            TextHelper.DrawTextMiddleAligned(g, "BATTLE", new Vector2(Constants.Width / 2, Y), Color.White);
            Y = Constants.Height / 4;
            GameScreen.DrawBox(g, new Vector2(Constants.Width / 16, Y), Constants.Width - Constants.Width / 8, Constants.Height / 5, Color.White);
            Y = Constants.Height / 4 + 10;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderPlayer.Name + "'s", new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderPlayer.Name + "'s", new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
            Y = Constants.Height / 4 + 35;
            g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 12, Y), new Vector2(Constants.Width - Constants.Width / 12, Y), Color.White);
            Y = Constants.Height / 4 + 40;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);

            Y += 30;
            if (Map.GlobalSorcererStreetBattleContext.Defender.CurrentHP > 0)
            {
                if (Map.GlobalSorcererStreetBattleContext.Invader.CurrentHP > 0)
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
