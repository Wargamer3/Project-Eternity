using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleBattleResultPhase : ActionPanelBattle
    {
        public static string RequirementName = "Sorcerer Street Battle Result Phase";

        private const string PanelName = "BattleBattleResultPhase";

        private BattleContent BattleAssets;

        private double AnimationTime;

        public ActionPanelBattleBattleResultPhase(SorcererStreetMap Map, BattleContent BattleAssets)
            : base(Map, PanelName)
        {
            this.BattleAssets = BattleAssets;
        }
        
        public override void OnSelect()
        {
            if (Map.GlobalSorcererStreetBattleContext.InvaderCreature.Creature.CurrentHP <= 0)
            {
                ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Map.GlobalSorcererStreetBattleContext, false, ActionPanelBattleAttackPhase.UponVictoryRequirement);
            }
            else
            {
                ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Map.GlobalSorcererStreetBattleContext, true, ActionPanelBattleAttackPhase.UponVictoryRequirement);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            if (ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            Map.GlobalSorcererStreetBattleContext.Background.Update(gameTime);

            if (AnimationTime < 1)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (AnimationTime < 4.2f)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation = null;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation = null;
            }
            else if (AnimationTime < 6)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                Map.GlobalSorcererStreetBattleContext.Background.MoveSpeed = new Vector3(0, 0, -1);
            }
            else
            {
                RemoveFromPanelList(this);
                if (Map.GlobalSorcererStreetBattleContext.DefenderCreature.Creature.CurrentHP > 0)
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
            if (!HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            Map.GlobalSorcererStreetBattleContext.Background.Update(gameTime);

            if (AnimationTime < 1)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (AnimationTime < 4.2f)
            {
                AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation = null;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation = null;
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
            return new ActionPanelBattleBattleResultPhase(Map, BattleAssets);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);

            if (AnimationTime < 1)
            {
                if (Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.CurrentHP > 0)
                {
                    DefenderReturnToTerrain(g);
                    if (Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.CurrentHP > 0)
                    {
                        InvaderLoseAndLeave(g);
                    }
                }
                else
                {
                    if (Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.CurrentHP > 0)
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

            Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation.Position = new Vector2(X, Y);
            Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation.Scale = new Vector2(MaxScale);
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

            Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation.Position = new Vector2(X, Y);
            Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation.Scale = new Vector2(MaxScale);
        }

        private void DefenderReturnToTerrain(CustomSpriteBatch g)
        {
            //Spin card from the left
            float MaxScale = 1f;
            float RealRotationTimer = (float)AnimationTime / 0.7f;
            float FinalX = Constants.Width + 200;
            float StartX = Constants.Width - Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.sprCard.Width - Constants.Width / 9;
            float DistanceX = FinalX - StartX;
            RealRotationTimer *= (float)Math.Sin(RealRotationTimer * MathHelper.PiOver4);
            float X = StartX + (RealRotationTimer) * DistanceX;
            float Y = 48 + RealRotationTimer * 45;
            MaxScale += RealRotationTimer / 9;

            Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation.Position = new Vector2(X, Y);
            Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation.Scale = new Vector2(MaxScale);
        }

        private void DisplayVersusText(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 720f;
            int Y = (int)(165 * Ratio);
            int BoxX = Constants.Width / 16;
            int BoxWidth = Constants.Width - Constants.Width / 8;
            g.Draw(BattleAssets.sprBattle, new Vector2(Constants.Width / 2, Y + 16 * Ratio), null, Color.White, 0f, new Vector2((int)(BattleAssets.sprBattle.Width / 2f), BattleAssets.sprBattle.Height), 1f, SpriteEffects.None, 0f);
            Y += 23;
            MenuHelper.DrawBox(g, new Vector2(BoxX, Y), BoxWidth, Constants.Height / 5, 1);
            Y = (int)(190 * Ratio);
            g.DrawStringMiddleAligned(BattleAssets.fntMenuText, Map.GlobalSorcererStreetBattleContext.InvaderCreature.Owner.Name, new Vector2(Constants.Width / 4, Y), SorcererStreetMap.TextColor);
            g.DrawStringMiddleAligned(BattleAssets.fntMenuText, Map.GlobalSorcererStreetBattleContext.DefenderCreature.Owner.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), SorcererStreetMap.TextColor);
            Y = (int)(225 * Ratio);
            g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 12, Y), new Vector2(Constants.Width - Constants.Width / 12, Y), Color.White);

            Y = (int)(230 * Ratio);
            g.DrawStringMiddleAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.InvaderCreature.Creature.Name, new Vector2(Constants.Width / 4, Y), SorcererStreetMap.TextColor);
            g.DrawStringMiddleAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.DefenderCreature.Creature.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), SorcererStreetMap.TextColor);

            Y += 43;
            if (Map.GlobalSorcererStreetBattleContext.DefenderCreature.Creature.CurrentHP > 0)
            {
                if (Map.GlobalSorcererStreetBattleContext.InvaderCreature.Creature.CurrentHP > 0)
                {
                    g.DrawStringMiddleAligned(Map.fntMenuText, "survived", new Vector2(Constants.Width / 4, Y), SorcererStreetMap.TextColor);
                }
                else
                {
                    g.DrawStringMiddleAligned(Map.fntMenuText, "was destroyed", new Vector2(Constants.Width / 4, Y), SorcererStreetMap.TextColor);
                }
                g.DrawStringMiddleAligned(Map.fntMenuText, "defended the land", new Vector2(Constants.Width - Constants.Width / 4, Y), SorcererStreetMap.TextColor);
            }
            else
            {
                g.DrawStringMiddleAligned(Map.fntMenuText, "took the land", new Vector2(Constants.Width / 4, Y), SorcererStreetMap.TextColor);
                g.DrawStringMiddleAligned(Map.fntMenuText, "was destroyed", new Vector2(Constants.Width - Constants.Width / 4, Y), SorcererStreetMap.TextColor);
            }
        }
    }
}
