using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleAttackPhase : BattleMapActionPanel
    {
        private const string PanelName = "BattleAttackPhase";

        private enum AttackSequences { FirstAttack, SecondAttack, End };
        public const string BeforeAttackRequirement = "Sorcerer Street Before Attack";
        public const string BeforeDefenseRequirement = "Sorcerer Street Before Defense";

        private readonly SorcererStreetMap Map;

        public static bool PlayAnimations = true;
        private AttackSequences AttackSequence;

        public CreatureCard FirstAttacker;
        public ItemCard FirstAttackerItem;
        public CreatureCard SecondAttacker;
        public ItemCard SecondAttackerItem;

        public ActionPanelBattleAttackPhase(SorcererStreetMap Map)
            : base(PanelName, Map.ListActionMenuChoice, null, false)
        {
            this.Map = Map;
        }

        public ActionPanelBattleAttackPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map)
            : base(PanelName, ListActionMenuChoice, null, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            AttackSequence = AttackSequences.FirstAttack;
            DetermineAttackOrder(out FirstAttacker, out SecondAttacker);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (AttackSequence == AttackSequences.FirstAttack)
            {
                ExecuteFirstAttack();
                AttackSequence = AttackSequences.SecondAttack;
            }
            else if (AttackSequence == AttackSequences.SecondAttack)
            {
                ExecuteSecondAttack();
                AttackSequence = AttackSequences.End;
            }
            else
            {
                RemoveFromPanelList(this);
            }
        }

        public void FinishPhase()
        {
            RemoveFromPanelList(this);
        }

        //Attacks First Invader, Attacks First Defender, Normal Attack Invader, Normal Attack Defender, Attacks Last Invader and Attacks Last Defender
        private void DetermineAttackOrder(out CreatureCard FirstAttacker, out CreatureCard SecondAttacker)
        {
            bool InvaderFirst = true;
            bool InvaderDecided = false;

            if (!Map.GlobalSorcererStreetBattleContext.Invader.BonusAttackLast && (Map.GlobalSorcererStreetBattleContext.Invader.Abilities.AttackFirst || Map.GlobalSorcererStreetBattleContext.Invader.BonusAttackFirst))
            {
                InvaderFirst = true;
                InvaderDecided = true;
            }
            else if (!Map.GlobalSorcererStreetBattleContext.Defender.BonusAttackLast && (Map.GlobalSorcererStreetBattleContext.Defender.Abilities.AttackFirst || Map.GlobalSorcererStreetBattleContext.Defender.BonusAttackFirst))
            {
                InvaderFirst = false;
                InvaderDecided = true;
            }

            if (!InvaderDecided && (Map.GlobalSorcererStreetBattleContext.Defender.BonusAttackLast || Map.GlobalSorcererStreetBattleContext.Defender.Abilities.AttackLast))
            {
                InvaderFirst = true;
            }
            else if (!InvaderDecided && (Map.GlobalSorcererStreetBattleContext.Invader.BonusAttackLast || Map.GlobalSorcererStreetBattleContext.Invader.Abilities.AttackLast))
            {
                InvaderFirst = true;
            }

            if (InvaderFirst)
            {
                FirstAttacker = Map.GlobalSorcererStreetBattleContext.Invader;
                SecondAttacker = Map.GlobalSorcererStreetBattleContext.Defender;
            }
            else
            {
                FirstAttacker = Map.GlobalSorcererStreetBattleContext.Defender;
                SecondAttacker = Map.GlobalSorcererStreetBattleContext.Invader;
            }
        }

        private void ExecutePreAttack(CreatureCard Attacker, CreatureCard Defender)
        {
            ActionPanelSorcererStreet AttackerActivationScreen = Attacker.ActivateInBattle(Map, Map.ListPlayer.IndexOf(Map.GlobalSorcererStreetBattleContext.InvaderPlayer));
            ActionPanelSorcererStreet DefenderActivationScreen = Defender.ActivateInBattle(Map, Map.ListPlayer.IndexOf(Map.GlobalSorcererStreetBattleContext.DefenderPlayer));

            if (PlayAnimations && DefenderActivationScreen != null)
            {
                AddToPanelListAndSelect(DefenderActivationScreen);
            }
            if (PlayAnimations && AttackerActivationScreen != null)
            {
                AddToPanelListAndSelect(AttackerActivationScreen);
            }
        }

        public void ExecuteFirstAttack()
        {
            ExecutePreAttack(FirstAttacker, SecondAttacker);

            int FinalST;
            int FinalHP;

            if (SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender)
            {
                FinalST = Map.GlobalSorcererStreetBattleContext.InvaderFinalST;
                FinalHP = Map.GlobalSorcererStreetBattleContext.DefenderFinalHP;

                Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Invader;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Defender;

                Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(BeforeAttackRequirement);

                Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Defender;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Invader;

                Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(BeforeDefenseRequirement);
            }
            else
            {
                FinalST = Map.GlobalSorcererStreetBattleContext.DefenderFinalST;
                FinalHP = Map.GlobalSorcererStreetBattleContext.InvaderFinalHP;

                Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Invader;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Defender;

                Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(BeforeDefenseRequirement);

                Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Defender;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Invader;

                Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(BeforeAttackRequirement);
            }

            FinalHP = Math.Max(0, FinalHP - FinalST);
            SecondAttacker.CurrentHP = Math.Min(SecondAttacker.CurrentHP, FinalHP);

            if (PlayAnimations)
            {
                if (FirstAttackerItem == null)
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(ListActionMenuChoice, Map, FirstAttacker.AttackAnimationPath, SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender));
                }
                else
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(ListActionMenuChoice, Map, FirstAttackerItem.AttackAnimationPath, SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender));
                }
            }
        }

        public void ExecuteSecondAttack()
        {
            ExecutePreAttack(SecondAttacker, FirstAttacker);

            int FinalST;
            int FinalHP;

            if (SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender)
            {
                FinalST = Map.GlobalSorcererStreetBattleContext.DefenderFinalST;
                FinalHP = Map.GlobalSorcererStreetBattleContext.InvaderFinalHP;

                Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Defender;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Invader;

                Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(BeforeAttackRequirement);

                Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Invader;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Defender;

                Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(BeforeDefenseRequirement);
            }
            else
            {
                FinalST = Map.GlobalSorcererStreetBattleContext.InvaderFinalST;
                FinalHP = Map.GlobalSorcererStreetBattleContext.DefenderFinalHP;

                Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Defender;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Invader;

                Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(BeforeDefenseRequirement);

                Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Invader;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Defender;

                Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(BeforeAttackRequirement);
            }

            FinalHP = Math.Max(0, FinalHP - FinalST);
            FirstAttacker.CurrentHP = Math.Min(FirstAttacker.CurrentHP, FinalHP);

            if (PlayAnimations)
            {
                if (FirstAttackerItem == null)
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(ListActionMenuChoice, Map, SecondAttacker.AttackAnimationPath, SecondAttacker != Map.GlobalSorcererStreetBattleContext.Defender));
                }
                else
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(ListActionMenuChoice, Map, SecondAttacker.AttackAnimationPath, SecondAttacker != Map.GlobalSorcererStreetBattleContext.Defender));
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            AttackSequence = AttackSequences.FirstAttack;
            DetermineAttackOrder(out FirstAttacker, out SecondAttacker);
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleAttackPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
