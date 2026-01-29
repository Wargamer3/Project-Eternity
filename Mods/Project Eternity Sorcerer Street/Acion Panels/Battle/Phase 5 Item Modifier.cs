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
            if (!InitAnimations(Map.GlobalSorcererStreetBattleContext))
            {
                ContinueBattlePhase();
            }
        }

        private static bool InitAnimations(SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            AnimationPhase = AnimationPhases.Finished;
            ItemAnimationTime = 9999;

            if (GlobalSorcererStreetBattleContext.InvaderCreature.Item != null)
            {
                GlobalSorcererStreetBattleContext.SetCreatures(GlobalSorcererStreetBattleContext.InvaderCreature, GlobalSorcererStreetBattleContext.DefenderCreature);
                ListSkillActivation = GlobalSorcererStreetBattleContext.GetAvailableActivation(RequirementName);

                if (ListSkillActivation.Count > 0)
                {
                    ItemAnimationTime = 0;
                    AnimationPhase = AnimationPhases.InvaderIntro;
                    ActivePhase = RequirementName;
                    return true;
                }
            }

            if (GlobalSorcererStreetBattleContext.DefenderCreature.Item != null)
            {
                GlobalSorcererStreetBattleContext.SetCreatures(GlobalSorcererStreetBattleContext.DefenderCreature, GlobalSorcererStreetBattleContext.InvaderCreature);
                ListSkillActivation = GlobalSorcererStreetBattleContext.GetAvailableActivation(RequirementName);

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

        public static bool InitAnimations(SorcererStreetBattleContext GlobalSorcererStreetBattleContext, string RequirementName)
        {
            AnimationPhase = AnimationPhases.Finished;
            ItemAnimationTime = 9999;

            GlobalSorcererStreetBattleContext.SetCreatures(GlobalSorcererStreetBattleContext.InvaderCreature, GlobalSorcererStreetBattleContext.DefenderCreature);
            ListSkillActivation = GlobalSorcererStreetBattleContext.GetAvailableActivation(RequirementName);

            if (ListSkillActivation.Count > 0)
            {
                ItemAnimationTime = 0;
                AnimationPhase = AnimationPhases.InvaderIntro;
                ActivePhase = RequirementName;
                return true;
            }

            GlobalSorcererStreetBattleContext.SetCreatures(GlobalSorcererStreetBattleContext.DefenderCreature, GlobalSorcererStreetBattleContext.InvaderCreature);
            ListSkillActivation = GlobalSorcererStreetBattleContext.GetAvailableActivation(RequirementName);

            if (ListSkillActivation.Count > 0)
            {
                ItemAnimationTime = 0;
                AnimationPhase = AnimationPhases.DefenderIntro;
                ActivePhase = RequirementName;
                return true;
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
                Context.SetCreatures(Context.InvaderCreature, Context.DefenderCreature);

                ListSkillActivation = Context.GetAvailableActivation(RequirementName);
            }
            else
            {
                Context.SetCreatures(Context.DefenderCreature, Context.InvaderCreature);

                ListSkillActivation = Context.GetAvailableActivation(RequirementName);
            }

            if (ListSkillActivation.Count > 0)
            {
                StartAnimation(InvaderSide, ListSkillActivation);
            }
        }

        public static void StartAnimationIfAvailable(SorcererStreetBattleContext Context, string RequirementName)
        {
            ActionPanelBattleItemModifierPhase.ActivePhase = RequirementName;

            List<SkillActivationContext> ListSkillActivation = Context.GetAvailableActivation(RequirementName);
            if (ListSkillActivation.Count > 0)
            {
                Context.SetCreatures(Context.InvaderCreature, Context.DefenderCreature);

                StartAnimation(true, ListSkillActivation);
            }
            else
            {
                Context.SetCreatures(Context.DefenderCreature, Context.InvaderCreature);

                ListSkillActivation = Context.GetAvailableActivation(RequirementName);

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

                    Context.ActivateSkill(ListSkillActivation[0].DicSkillActivation);

                    AnimationPhase = AnimationPhases.InvaderActivation;
                }
            }
            if (AnimationPhase == AnimationPhases.DefenderIntro)
            {
                if (ItemAnimationTime > 2.5)
                {
                    Context.ListActivatedEffect.Clear();

                    Context.ActivateSkill(ListSkillActivation[0].DicSkillActivation);

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
                        if (Context.OpponentCreature.FinalHP > 0)
                        {
                            Context.SetCreatures(Context.DefenderCreature, Context.InvaderCreature);
                            ListSkillActivation = Context.GetAvailableActivation(ActivePhase);
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
                        Context.SetCreatures(Context.InvaderCreature, Context.DefenderCreature);
                        return false;
                    }
                }
            }
            else if (AnimationPhase == AnimationPhases.Finished)
            {
                Context.SetCreatures(Context.InvaderCreature, Context.DefenderCreature);
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

            DrawItemActivation(g, Map.fntMenuText, Map.GlobalSorcererStreetBattleContext);
        }

        public static void DrawItemActivation(CustomSpriteBatch g, SpriteFont fntMenuText, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            if (AnimationPhase == AnimationPhases.Finished)
                return;

            if (ListSkillActivation[0].ActivatedByItem)
            {
                if (AnimationPhase == AnimationPhases.InvaderIntro || AnimationPhase == AnimationPhases.InvaderActivation)
                {
                    DrawInvaderItemCardActivation(g, fntMenuText, GlobalSorcererStreetBattleContext);
                }
                else
                {
                    DrawDefenderItemCardActivation(g, fntMenuText, GlobalSorcererStreetBattleContext);
                }
            }
            else
            {
                if (AnimationPhase == AnimationPhases.InvaderIntro || AnimationPhase == AnimationPhases.InvaderActivation)
                {
                    DrawInvaderCreatureCardActivation(g, fntMenuText, GlobalSorcererStreetBattleContext);
                }
                else
                {
                    DrawDefenderCreatureCardActivation(g, fntMenuText, GlobalSorcererStreetBattleContext);
                }
            }
        }

        public static void DrawInvaderItemCardActivation(CustomSpriteBatch g, SpriteFont fntMenuText, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            float CardHeight = Constants.Height / 16 + (GlobalSorcererStreetBattleContext.InvaderCreature.Item.sprCard.Height / 2) * ItemCardScale;

            if (ItemAnimationTime < 0.3)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin((ItemAnimationTime / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.InvaderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.6)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.InvaderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.9)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin(((ItemAnimationTime - 0.3) / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.InvaderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 1.5)//Bonus text + effect activation animaton
            {
            }
            else if (ItemAnimationTime <= 2.5)//Bonus text + effect activation animaton
            {
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.InvaderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);
                g.DrawStringCentered(fntMenuText, "Ability Values Changed", new Vector2(Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
            else if ((int)Math.Ceiling(ItemAnimationTime - 3.5) < GlobalSorcererStreetBattleContext.ListActivatedEffect.Count)//Effect text
            {
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.InvaderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);

                g.DrawStringCentered(fntMenuText, GlobalSorcererStreetBattleContext.ListActivatedEffect[(int)Math.Ceiling(ItemAnimationTime - 3.5)].ActivationInfo, new Vector2(Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
        }

        public static void DrawInvaderCreatureCardActivation(CustomSpriteBatch g, SpriteFont fntMenuText, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            float CardX = Constants.Width / 9 + GlobalSorcererStreetBattleContext.InvaderCreature.Creature.sprCard.Width / 2;
            float CardY = Constants.Height / 12 * CreatureCardScale;

            if (ItemAnimationTime < 0.3)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + (float)Math.Sin((ItemAnimationTime / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.InvaderCreature.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.6)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.InvaderCreature.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.9)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + (float)Math.Sin(((ItemAnimationTime - 0.3) / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.InvaderCreature.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 1.5)//Bonus text + effect activation animaton
            {
            }
            else if (ItemAnimationTime <= 2.5)//Bonus text + effect activation animaton
            {
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);
                g.DrawStringCentered(fntMenuText, "Ability Values Changed", new Vector2(Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
            else if ((int)Math.Ceiling(ItemAnimationTime - 3.5) < GlobalSorcererStreetBattleContext.ListActivatedEffect.Count)//Effect text
            {
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);

                g.DrawStringCentered(fntMenuText, GlobalSorcererStreetBattleContext.ListActivatedEffect[(int)Math.Ceiling(ItemAnimationTime - 3.5)].ActivationInfo, new Vector2(Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
        }

        public static void DrawDefenderItemCardActivation(CustomSpriteBatch g, SpriteFont fntMenuText, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            float CardHeight = Constants.Height / 16 + (GlobalSorcererStreetBattleContext.DefenderCreature.Item.sprCard.Height / 2) * ItemCardScale;

            if (ItemAnimationTime < 0.3)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin((ItemAnimationTime / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.DefenderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.6)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.DefenderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.9)
            {
                float StartScale = ItemCardScale;
                float FinalScale = StartScale + (float)Math.Sin(((ItemAnimationTime - 0.3) / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.DefenderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 1.5)//Bonus text + effect activation animaton
            {
            }
            else if (ItemAnimationTime <= 2.5)//Bonus text + effect activation animaton
            {
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.DefenderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);
                g.DrawStringCentered(fntMenuText, "Ability Values Changed", new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
            else if ((int)Math.Ceiling(ItemAnimationTime - 3.5) < GlobalSorcererStreetBattleContext.ListActivatedEffect.Count)//Effect text
            {
                Card.DrawCardMiniatureCentered(g, GlobalSorcererStreetBattleContext.DefenderCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, Constants.Width - Constants.Width / 8, CardHeight, -ItemCardScale, ItemCardScale, false);
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);

                g.DrawStringCentered(fntMenuText, GlobalSorcererStreetBattleContext.ListActivatedEffect[(int)Math.Ceiling(ItemAnimationTime - 3.5)].ActivationInfo, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
        }

        public static void DrawDefenderCreatureCardActivation(CustomSpriteBatch g, SpriteFont fntMenuText, SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            float CardX = Constants.Width - Constants.Width / 9 - GlobalSorcererStreetBattleContext.DefenderCreature.Creature.sprCard.Width / 2;
            float CardY = Constants.Height / 12 * CreatureCardScale;

            if (ItemAnimationTime < 0.3)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + (float)Math.Sin((ItemAnimationTime / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.DefenderCreature.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.6)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.DefenderCreature.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 0.9)
            {
                float StartScale = CreatureCardScale;
                float FinalScale = StartScale + (float)Math.Sin(((ItemAnimationTime - 0.3) / 0.6) * MathHelper.Pi) * 0.07f;
                Card.DrawCardMiniature(g, GlobalSorcererStreetBattleContext.DefenderCreature.Creature.sprCard, MenuHelper.sprCardBack, Color.White, CardX, CardY, -FinalScale, FinalScale, false);
            }
            else if (ItemAnimationTime < 1.5)//Bonus text + effect activation animaton
            {
            }
            else if (ItemAnimationTime <= 2.5)//Bonus text + effect activation animaton
            {
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);
                g.DrawStringCentered(fntMenuText, "Ability Values Changed", new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
            else if ((int)Math.Ceiling(ItemAnimationTime - 3.5) < GlobalSorcererStreetBattleContext.ListActivatedEffect.Count)//Effect text
            {
                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12, (int)(Constants.Height / 1.8f)), Constants.Width / 3, Constants.Height / 14);

                g.DrawStringCentered(fntMenuText, GlobalSorcererStreetBattleContext.ListActivatedEffect[(int)Math.Ceiling(ItemAnimationTime - 3.5)].ActivationInfo, new Vector2(Constants.Width - Constants.Width / 3 - Constants.Width / 12 + Constants.Width / 6, Constants.Height / 1.8f + Constants.Height / 28), Color.White);
            }
        }
    }
}
