using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleItemModifierPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleItemModifierPhase";

        public static string RequirementName = "Sorcerer Street Item Phase";

        private enum AnimationPhases { InvaderIntro, InvaderActivation, DefenderIntro, DefenderActivation }

        private const float ItemCardScale = 0.4f;

        private double ItemAnimationTime;
        private AnimationPhases AnimationPhase;

        public ActionPanelBattleItemModifierPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            Map.GlobalSorcererStreetBattleContext.ActivatedEffect = null;

            if (Map.GlobalSorcererStreetBattleContext.InvaderItem != null && Map.GlobalSorcererStreetBattleContext.InvaderItem.CanActivateSkill(RequirementName))
            {
                AnimationPhase = AnimationPhases.InvaderIntro;
            }
            else if (Map.GlobalSorcererStreetBattleContext.DefenderItem != null && Map.GlobalSorcererStreetBattleContext.DefenderItem.CanActivateSkill(RequirementName))
            {
                AnimationPhase = AnimationPhases.DefenderIntro;
            }
            else
            {
                ContinueBattlePhase();
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!CanUpdate(gameTime))
                return;

            ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (AnimationPhase == AnimationPhases.InvaderIntro || AnimationPhase == AnimationPhases.DefenderIntro)
            {
                if (ItemAnimationTime >= 2.5)
                {
                    if (AnimationPhase == AnimationPhases.InvaderIntro)
                    {
                        Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, RequirementName);

                        Map.GlobalSorcererStreetBattleContext.InvaderItem.ActivateSkill(RequirementName);
                        AnimationPhase = AnimationPhases.InvaderActivation;
                    }
                    else if (AnimationPhase == AnimationPhases.DefenderIntro)
                    {
                        Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, RequirementName);

                        Map.GlobalSorcererStreetBattleContext.DefenderItem.ActivateSkill(RequirementName);
                        AnimationPhase = AnimationPhases.DefenderActivation;
                    }
                }
            }
            else if (AnimationPhase == AnimationPhases.InvaderActivation)
            {
                if (ItemAnimationTime >= 3.5)
                {
                    ItemAnimationTime = 0;
                    if (Map.GlobalSorcererStreetBattleContext.DefenderItem != null)
                    {
                        AnimationPhase = AnimationPhases.DefenderIntro;
                    }
                    else
                    {
                        ContinueBattlePhase();
                    }
                }
            }
            else if (AnimationPhase == AnimationPhases.DefenderActivation)
            {
                if (ItemAnimationTime >= 3.5)
                {
                    ContinueBattlePhase();
                }
            }
        }

        private void ContinueBattlePhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelBattleBoostsModifierPhase(Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ReadPlayerInfo(BR, Map);

            Map.GlobalSorcererStreetBattleContext.ActivatedEffect = null;

            if (Map.GlobalSorcererStreetBattleContext.InvaderItem != null && Map.GlobalSorcererStreetBattleContext.InvaderItem.CanActivateSkill(RequirementName))
            {
                AnimationPhase = AnimationPhases.InvaderIntro;
            }
            else if (Map.GlobalSorcererStreetBattleContext.DefenderItem != null && Map.GlobalSorcererStreetBattleContext.DefenderItem.CanActivateSkill(RequirementName))
            {
                AnimationPhase = AnimationPhases.DefenderIntro;
            }
        }

        public override void DoWrite(ByteWriter BW)
        {
            WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleItemModifierPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);

            if (AnimationPhase == AnimationPhases.InvaderIntro || AnimationPhase == AnimationPhases.InvaderActivation)
            {
                DrawInvaderCardActivation(g);
            }
            else if (AnimationPhase == AnimationPhases.DefenderIntro || AnimationPhase == AnimationPhases.DefenderActivation)
            {
                DrawDenderCardActivation(g);
            }
        }

        private void DrawInvaderCardActivation(CustomSpriteBatch g)
        {
            float CardHeight = Constants.Height / 16 + (Map.GlobalSorcererStreetBattleContext.InvaderItem.sprCard.Height / 2) * ItemCardScale;

            if (ItemAnimationTime < 0.3)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin((ItemAnimationTime / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.InvaderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.6)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + 0.07f;
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.InvaderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);

            }
            else if (ItemAnimationTime < 0.9)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin(((ItemAnimationTime - 0.3) / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.InvaderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);

            }
            else if (ItemAnimationTime < 1.5)//Bonus text + effect activation animaton
            {
            }
            else if (ItemAnimationTime < 2.5)//Bonus text + effect activation animaton
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.InvaderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);
                g.DrawStringCentered(Map.fntArial12, "Ability Values Changed", new Vector2(Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
            else if (ItemAnimationTime < 3.5)//Effect text
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.InvaderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);

                g.DrawStringCentered(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.ActivatedEffect.ActivationInfo, new Vector2(Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
        }

        private void DrawDenderCardActivation(CustomSpriteBatch g)
        {
            float CardHeight = Constants.Height / 16 + (Map.GlobalSorcererStreetBattleContext.DefenderItem.sprCard.Height / 2) * ItemCardScale;

            if (ItemAnimationTime < 0.3)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin((ItemAnimationTime / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.DefenderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.6)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + 0.07f;
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.DefenderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);

            }
            else if (ItemAnimationTime < 0.9)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin(((ItemAnimationTime - 0.3) / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.DefenderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);

            }
            else if (ItemAnimationTime < 1.5)//Bonus text + effect activation animaton
            {
            }
            else if (ItemAnimationTime < 2.5)//Bonus text + effect activation animaton
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.DefenderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);
                g.DrawStringCentered(Map.fntArial12, "Ability Values Changed", new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
            else if (ItemAnimationTime < 3.5)//Effect text
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.DefenderItem.sprCard, Map.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);

                g.DrawStringCentered(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.ActivatedEffect.ActivationInfo, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
        }
    }
}
