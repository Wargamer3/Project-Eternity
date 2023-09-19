using System;
using System.Collections.Generic;
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

        public const string RequirementName = "Sorcerer Street Item Phase";

        private enum AnimationPhases { Finished, InvaderIntro, InvaderActivation, DefenderIntro, DefenderActivation }

        private const float ItemCardScale = 0.4f;
        private const float CreatureCardScale = 1f;

        private static double ItemAnimationTime;
        private static AnimationPhases AnimationPhase;
        public static string ActivePhase;
        public static List<SkillActivationContext> ListSkillActivation;

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

            if (GlobalSorcererStreetBattleContext.Invader.Item != null)
            {
                ListSkillActivation = GlobalSorcererStreetBattleContext.GetAvailableActivation(GlobalSorcererStreetBattleContext.Invader, GlobalSorcererStreetBattleContext.Defender, RequirementName);
                if (ListSkillActivation.Count > 0)
                {
                    ItemAnimationTime = 0;
                    AnimationPhase = AnimationPhases.InvaderIntro;
                    ActivePhase = RequirementName;
                    return true;
                }
            }
            else if (GlobalSorcererStreetBattleContext.Defender.Item != null)
            {
                ListSkillActivation = GlobalSorcererStreetBattleContext.GetAvailableActivation(GlobalSorcererStreetBattleContext.Defender, GlobalSorcererStreetBattleContext.Invader, RequirementName);
                if (ListSkillActivation.Count > 0)
                {
                    ItemAnimationTime = 0;
                    AnimationPhase = AnimationPhases.DefenderIntro;
                    ActivePhase = RequirementName;
                    return true;
                }
            }

            return false;
        }

        public static void StartAnimation(bool InvaderSide, List<SkillActivationContext> ListSkillActivation)
        {
            ActionPanelBattleItemModifierPhase.ListSkillActivation = ListSkillActivation;

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

        public static void StartAnimationIfAvailable(SorcererStreetBattleContext Context, bool InvaderSide, string RequirementName)
        {
            ActionPanelBattleItemModifierPhase.ActivePhase = RequirementName;

            List<SkillActivationContext> ListSkillActivation;
            if (InvaderSide)
            {
                ListSkillActivation = Context.GetAvailableActivation(Context.Invader, Context.Defender, RequirementName);
            }
            else
            {
                ListSkillActivation = Context.GetAvailableActivation(Context.Defender, Context.Invader, RequirementName);
            }

            if (ListSkillActivation.Count > 0)
            {
                StartAnimation(InvaderSide, ListSkillActivation);
            }
        }

        public static void StartAnimationIfAvailable(SorcererStreetBattleContext Context, string RequirementName)
        {
            ActionPanelBattleItemModifierPhase.ActivePhase = RequirementName;

            List<SkillActivationContext> ListSkillActivation = Context.GetAvailableActivation(Context.Invader, Context.Defender, RequirementName);
            if (ListSkillActivation.Count > 0)
            {
                StartAnimation(true, ListSkillActivation);
            }
            else
            {
                ListSkillActivation = Context.GetAvailableActivation(Context.Defender, Context.Invader, RequirementName);
                if (ListSkillActivation.Count > 0)
                {
                    StartAnimation(false, ListSkillActivation);
                }
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

        public static bool UpdateAnimations(GameTime gameTime, SorcererStreetBattleContext Context)
        {
            ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (AnimationPhase == AnimationPhases.InvaderIntro)
            {
                if (ItemAnimationTime > 2.5)
                {
                    Context.ListActivatedEffect.Clear();

                    Context.ActivateSkill(Context.Invader, Context.Defender, ListSkillActivation[0].DicSkillActivation);

                    AnimationPhase = AnimationPhases.InvaderActivation;
                }
            }
            if (AnimationPhase == AnimationPhases.DefenderIntro)
            {
                if (ItemAnimationTime > 2.5)
                {
                    Context.ListActivatedEffect.Clear();

                    Context.ActivateSkill(Context.Defender, Context.Invader, ListSkillActivation[0].DicSkillActivation);

                    AnimationPhase = AnimationPhases.DefenderActivation;
                }
            }
            else if (AnimationPhase == AnimationPhases.InvaderActivation)
            {
                if (ItemAnimationTime > 2.5 + Context.ListActivatedEffect.Count)
                {
                    ItemAnimationTime = 0;
                    ListSkillActivation.RemoveAt(0);
                    if (ListSkillActivation.Count == 0)
                    {
                        if (Context.Defender.Item != null)
                        {
                            ListSkillActivation = Context.GetAvailableActivation(Context.Defender, Context.Invader, ActivePhase);
                            if (ListSkillActivation.Count > 0)
                            {
                                ItemAnimationTime = 0;
                                AnimationPhase = AnimationPhases.DefenderIntro;
                                return true;
                            }
                            else
                            {
                                ItemAnimationTime = 2.5 + Context.ListActivatedEffect.Count;
                                Context.ListActivatedEffect.Clear();
                                AnimationPhase = AnimationPhases.Finished;
                                return false;
                            }
                        }
                        else
                        {
                            ItemAnimationTime = 2.5 + Context.ListActivatedEffect.Count;
                            Context.ListActivatedEffect.Clear();
                            AnimationPhase = AnimationPhases.Finished;
                            return false;
                        }
                    }
                }
            }
            else if (AnimationPhase == AnimationPhases.DefenderActivation)
            {
                if (ItemAnimationTime > 2.5 + Context.ListActivatedEffect.Count)
                {
                    ItemAnimationTime = 0;
                    ListSkillActivation.RemoveAt(0);

                    if (ListSkillActivation.Count == 0)
                    {
                        Context.ListActivatedEffect.Clear();
                        AnimationPhase = AnimationPhases.Finished;
                        return false;
                    }
                }
            }
            else if (AnimationPhase == AnimationPhases.Finished)
            {
                Context.ListActivatedEffect.Clear();
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

            InitAnimations(Map.GlobalSorcererStreetBattleContext);
            Map.GlobalSorcererStreetBattleContext.ListActivatedEffect.Clear();
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
            if (AnimationPhase == AnimationPhases.Finished)
                return;

            if (ListSkillActivation[0].ActivatedByItem)
            {
                if (AnimationPhase == AnimationPhases.InvaderIntro || AnimationPhase == AnimationPhases.InvaderActivation)
                {
                    DrawInvaderItemCardActivation(g, fntArial12, GlobalSorcererStreetBattleContext);
                }
                else
                {
                    DrawDenderItemCardActivation(g, fntArial12, GlobalSorcererStreetBattleContext);
                }
            }
            else
            {
                if (AnimationPhase == AnimationPhases.InvaderIntro || AnimationPhase == AnimationPhases.InvaderActivation)
                {
                    DrawInvaderCreatureCardActivation(g, fntArial12, GlobalSorcererStreetBattleContext);
                }
                else
                {
                    DrawDenderCreatureCardActivation(g, fntArial12, GlobalSorcererStreetBattleContext);
                }
            }
        }

        public static void DrawInvaderItemCardActivation(CustomSpriteBatch g, SpriteFont fntArial12, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
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

        public static void DrawInvaderCreatureCardActivation(CustomSpriteBatch g, SpriteFont fntArial12, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            float CardX = Constants.Width / 9 + GlobalSorcererStreetBattleContext.Invader.Creature.sprCard.Width / 2;
            float CardY = Constants.Height / 12 * CreatureCardScale;

            if (ItemAnimationTime < 0.3)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + (float)Math.Sin((ItemAnimationTime / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.Invader.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.6)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.Invader.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.9)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + (float)Math.Sin(((ItemAnimationTime - 0.3) / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.Invader.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 1.5)//Bonus text + effect activation animaton
            {
            }
            else if (ItemAnimationTime <= 2.5)//Bonus text + effect activation animaton
            {
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);
                g.DrawStringCentered(fntArial12, "Ability Values Changed", new Vector2(Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
            else if ((int)Math.Ceiling(ItemAnimationTime - 3.5) < GlobalSorcererStreetBattleContext.ListActivatedEffect.Count)//Effect text
            {
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);

                g.DrawStringCentered(fntArial12, GlobalSorcererStreetBattleContext.ListActivatedEffect[(int)Math.Ceiling(ItemAnimationTime - 3.5)].ActivationInfo, new Vector2(Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
        }

        public static void DrawDenderItemCardActivation(CustomSpriteBatch g, SpriteFont fntArial12, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
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

        public static void DrawDenderCreatureCardActivation(CustomSpriteBatch g, SpriteFont fntArial12, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            float CardX = Constants.Width - Constants.Width / 9 - GlobalSorcererStreetBattleContext.Defender.Creature.sprCard.Width / 2;
            float CardY = Constants.Height / 12 * CreatureCardScale;

            if (ItemAnimationTime < 0.3)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + (float)Math.Sin((ItemAnimationTime / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.Defender.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.6)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.Defender.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.9)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + (float)Math.Sin(((ItemAnimationTime - 0.3) / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.Defender.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 1.5)//Bonus text + effect activation animaton
            {
            }
            else if (ItemAnimationTime <= 2.5)//Bonus text + effect activation animaton
            {
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);
                g.DrawStringCentered(fntArial12, "Ability Values Changed", new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
            else if ((int)Math.Ceiling(ItemAnimationTime - 3.5) < GlobalSorcererStreetBattleContext.ListActivatedEffect.Count)//Effect text
            {
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);

                g.DrawStringCentered(fntArial12, GlobalSorcererStreetBattleContext.ListActivatedEffect[(int)Math.Ceiling(ItemAnimationTime - 3.5)].ActivationInfo, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
        }
    }
}
