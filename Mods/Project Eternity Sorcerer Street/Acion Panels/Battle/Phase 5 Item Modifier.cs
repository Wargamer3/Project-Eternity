using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleItemModifierPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleItemModifierPhase";

        public static string RequirementName = "Sorcerer Street Item Phase";

        private enum AnimationPhases { Finished, InvaderIntro, InvaderActivation, DefenderIntro, DefenderActivation }

        private const float ItemCardScale = 0.4f;

        private static double ItemAnimationTime;
        private static AnimationPhases AnimationPhase;
        public static string NextAnimationRequirementName;

        public ActionPanelBattleItemModifierPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
            this.Map = Map;

            ItemAnimationTime = 0;
        }

        public override void OnSelect()
        {
            AnimationPhase = AnimationPhases.Finished;
            ItemAnimationTime = 9999;

            if (!InitAnimations(Map.GlobalSorcererStreetBattleContext))
            {
                ContinueBattlePhase();
            }
        }

        public static bool InitAnimations(SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            AnimationPhase = AnimationPhases.Finished;
            ItemAnimationTime = 9999;

            if (GlobalSorcererStreetBattleContext.Invader.Item != null && GlobalSorcererStreetBattleContext.CanActivateSkillItem(GlobalSorcererStreetBattleContext.Invader, GlobalSorcererStreetBattleContext.Defender, RequirementName))
            {
                NextAnimationRequirementName = RequirementName;
                ItemAnimationTime = 0;
                AnimationPhase = AnimationPhases.InvaderIntro;
                return true;
            }
            else if (GlobalSorcererStreetBattleContext.Defender.Item != null && GlobalSorcererStreetBattleContext.CanActivateSkillItem(GlobalSorcererStreetBattleContext.Defender, GlobalSorcererStreetBattleContext.Invader, RequirementName))
            {
                NextAnimationRequirementName = RequirementName;
                ItemAnimationTime = 0;
                AnimationPhase = AnimationPhases.DefenderIntro;
                return true;
            }

            return false;
        }

        public static void StartAnimation(bool InvaderSide)
        {
            if (InvaderSide)
            {
                ItemAnimationTime = 0;
                AnimationPhase = AnimationPhases.InvaderActivation;
            }
            else
            {
                ItemAnimationTime = 0;
                AnimationPhase = AnimationPhases.DefenderActivation;
            }
        }

        public static void StartAnimation(bool InvaderSide, string NextAnimationRequirementName)
        {
            ActionPanelBattleItemModifierPhase.NextAnimationRequirementName = NextAnimationRequirementName;

            if (InvaderSide)
            {
                ItemAnimationTime = 0;
                AnimationPhase = AnimationPhases.InvaderIntro;
            }
            else
            {
                ItemAnimationTime = 0;
                AnimationPhase = AnimationPhases.DefenderIntro;
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            if (!UpdateAnimations(gameTime, Map.GlobalSorcererStreetBattleContext))
            {
                ContinueBattlePhase();
            }
        }

        public static bool UpdateAnimations(GameTime gameTime, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (AnimationPhase == AnimationPhases.InvaderIntro || AnimationPhase == AnimationPhases.DefenderIntro)
            {
                if (ItemAnimationTime > 2.5)
                {
                    if (AnimationPhase == AnimationPhases.InvaderIntro)
                    {
                        GlobalSorcererStreetBattleContext.ListActivatedEffect.Clear();

                        GlobalSorcererStreetBattleContext.ActivateSkill(GlobalSorcererStreetBattleContext.Invader, GlobalSorcererStreetBattleContext.Defender, NextAnimationRequirementName);

                        AnimationPhase = AnimationPhases.InvaderActivation;
                    }
                    else if (AnimationPhase == AnimationPhases.DefenderIntro)
                    {
                        GlobalSorcererStreetBattleContext.ListActivatedEffect.Clear();

                        GlobalSorcererStreetBattleContext.ActivateSkill(GlobalSorcererStreetBattleContext.Defender, GlobalSorcererStreetBattleContext.Invader, NextAnimationRequirementName);

                        AnimationPhase = AnimationPhases.DefenderActivation;
                    }
                }
            }
            else if (AnimationPhase == AnimationPhases.InvaderActivation)
            {
                if (ItemAnimationTime > 2.5 + GlobalSorcererStreetBattleContext.ListActivatedEffect.Count)
                {
                    ItemAnimationTime = 0;
                    if (GlobalSorcererStreetBattleContext.Defender.Item != null && GlobalSorcererStreetBattleContext.CanActivateSkillItem(GlobalSorcererStreetBattleContext.Defender, GlobalSorcererStreetBattleContext.Invader, NextAnimationRequirementName))
                    {
                        AnimationPhase = AnimationPhases.DefenderIntro;
                    }
                    else
                    {
                        ItemAnimationTime = 2.5 + GlobalSorcererStreetBattleContext.ListActivatedEffect.Count;
                        GlobalSorcererStreetBattleContext.ListActivatedEffect.Clear();
                        AnimationPhase = AnimationPhases.Finished;
                        return false;
                    }
                }
            }
            else if (AnimationPhase == AnimationPhases.DefenderActivation)
            {
                if (ItemAnimationTime > 2.5 + GlobalSorcererStreetBattleContext.ListActivatedEffect.Count)
                {
                    ItemAnimationTime = 2.5 + GlobalSorcererStreetBattleContext.ListActivatedEffect.Count;
                    GlobalSorcererStreetBattleContext.ListActivatedEffect.Clear();
                    AnimationPhase = AnimationPhases.Finished;
                    return false;
                }
            }
            else if (AnimationPhase == AnimationPhases.Finished)
            {
                GlobalSorcererStreetBattleContext.ListActivatedEffect.Clear();
                return false;
            }

            return true;
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

            Map.GlobalSorcererStreetBattleContext.ListActivatedEffect.Clear();

            if (Map.GlobalSorcererStreetBattleContext.Invader.Item != null && Map.GlobalSorcererStreetBattleContext.Invader.Item.CanActivateSkill(RequirementName))
            {
                AnimationPhase = AnimationPhases.InvaderIntro;
            }
            else if (Map.GlobalSorcererStreetBattleContext.Defender.Item != null && Map.GlobalSorcererStreetBattleContext.Defender.Item.CanActivateSkill(RequirementName))
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

            DrawItemActivation(g, Map.fntArial12, Map.GlobalSorcererStreetBattleContext);
        }

        public static void DrawItemActivation(CustomSpriteBatch g, SpriteFont fntArial12, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            if (AnimationPhase == AnimationPhases.InvaderIntro || AnimationPhase == AnimationPhases.InvaderActivation)
            {
                DrawInvaderCardActivation(g, fntArial12, GlobalSorcererStreetBattleContext);
            }
            else if (AnimationPhase == AnimationPhases.DefenderIntro || AnimationPhase == AnimationPhases.DefenderActivation)
            {
                DrawDenderCardActivation(g, fntArial12, GlobalSorcererStreetBattleContext);
            }
        }

        public static void DrawInvaderCardActivation(CustomSpriteBatch g, SpriteFont fntArial12, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            float CardHeight = Constants.Height / 16 + (GlobalSorcererStreetBattleContext.Invader.Item.sprCard.Height / 2) * ItemCardScale;

            if (ItemAnimationTime < 0.3)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin((ItemAnimationTime / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.Invader.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.6)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.Invader.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);

            }
            else if (ItemAnimationTime < 0.9)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin(((ItemAnimationTime - 0.3) / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.Invader.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);

            }
            else if (ItemAnimationTime < 1.5)//Bonus text + effect activation animaton
            {
            }
            else if (ItemAnimationTime <= 2.5)//Bonus text + effect activation animaton
            {
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.Invader.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);
                g.DrawStringCentered(fntArial12, "Ability Values Changed", new Vector2(Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
            else if ((int)Math.Ceiling(ItemAnimationTime - 3.5) < GlobalSorcererStreetBattleContext.ListActivatedEffect.Count)//Effect text
            {
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.Invader.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);

                g.DrawStringCentered(fntArial12, GlobalSorcererStreetBattleContext.ListActivatedEffect[(int)Math.Ceiling(ItemAnimationTime - 3.5)].ActivationInfo, new Vector2(Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
        }

        public static void DrawDenderCardActivation(CustomSpriteBatch g, SpriteFont fntArial12, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            float CardHeight = Constants.Height / 16 + (GlobalSorcererStreetBattleContext.Defender.Item.sprCard.Height / 2) * ItemCardScale;

            if (ItemAnimationTime < 0.3)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin((ItemAnimationTime / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.Defender.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.6)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.Defender.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);

            }
            else if (ItemAnimationTime < 0.9)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin(((ItemAnimationTime - 0.3) / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.Defender.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);

            }
            else if (ItemAnimationTime < 1.5)//Bonus text + effect activation animaton
            {
            }
            else if (ItemAnimationTime <= 2.5)//Bonus text + effect activation animaton
            {
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.Defender.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);
                g.DrawStringCentered(fntArial12, "Ability Values Changed", new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
            else if ((int)Math.Ceiling(ItemAnimationTime - 3.5) < GlobalSorcererStreetBattleContext.ListActivatedEffect.Count)//Effect text
            {
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.Defender.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);

                g.DrawStringCentered(fntArial12, GlobalSorcererStreetBattleContext.ListActivatedEffect[(int)Math.Ceiling(ItemAnimationTime - 3.5)].ActivationInfo, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
        }
    }
}
