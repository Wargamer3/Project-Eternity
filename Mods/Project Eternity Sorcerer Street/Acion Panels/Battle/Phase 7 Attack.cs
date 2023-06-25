using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleAttackPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleAttackPhase";

        private enum AttackSequences { FirstAttack, SecondAttack, End };
        public const string InvasionRequirement = "Sorcerer Street Invasion";
        public const string BeforeAttackRequirement = "Sorcerer Street Before Attack";
        public const string BeforeDefenseRequirement = "Sorcerer Street Before Defense";

        public static bool PlayAnimations = true;
        private AttackSequences AttackSequence;

        public CreatureCard FirstAttacker;
        public ItemCard FirstAttackerItem;
        public CreatureCard SecondAttacker;
        public ItemCard SecondAttackerItem;

        public ActionPanelBattleAttackPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
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
            if (!CanUpdate(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            if (AttackSequence == AttackSequences.FirstAttack)
            {
                ExecuteFirstAttack();
            }
            else if (AttackSequence == AttackSequences.SecondAttack)
            {
                ExecuteSecondAttack();
                AttackSequence = AttackSequences.End;
            }
            else
            {
                RemoveFromPanelList(this);
                AddToPanelListAndSelect(new ActionPanelBattleBattleResultPhase(Map));
            }
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

                Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, BeforeAttackRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, BeforeAttackRequirement);
            }
            else
            {
                FinalST = Map.GlobalSorcererStreetBattleContext.DefenderFinalST;
                FinalHP = Map.GlobalSorcererStreetBattleContext.InvaderFinalHP;

                Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, BeforeDefenseRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, BeforeDefenseRequirement);
            }

            FinalHP = Math.Max(0, FinalHP - FinalST);
            SecondAttacker.CurrentHP = Math.Min(SecondAttacker.CurrentHP, FinalHP);

            if (SecondAttacker.CurrentHP > 0)
            {
                AttackSequence = AttackSequences.SecondAttack;
            }
            else
            {
                AttackSequence = AttackSequences.End;
            }

            if (PlayAnimations)
            {
                if (FirstAttackerItem == null)
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, FirstAttacker.AttackAnimationPath, SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender));
                }
                else
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, FirstAttackerItem.ItemActivationAnimationPath, SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender));
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

                Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, BeforeAttackRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, BeforeAttackRequirement);
            }
            else
            {
                FinalST = Map.GlobalSorcererStreetBattleContext.InvaderFinalST;
                FinalHP = Map.GlobalSorcererStreetBattleContext.DefenderFinalHP;

                Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, BeforeDefenseRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, BeforeDefenseRequirement);
            }

            FinalHP = Math.Max(0, FinalHP - FinalST);
            FirstAttacker.CurrentHP = Math.Min(FirstAttacker.CurrentHP, FinalHP);

            if (PlayAnimations)
            {
                if (FirstAttackerItem == null)
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, SecondAttacker.AttackAnimationPath, SecondAttacker != Map.GlobalSorcererStreetBattleContext.Defender));
                }
                else
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, SecondAttacker.AttackAnimationPath, SecondAttacker != Map.GlobalSorcererStreetBattleContext.Defender));
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ReadPlayerInfo(BR, Map);
            AttackSequence = AttackSequences.FirstAttack;
            DetermineAttackOrder(out FirstAttacker, out SecondAttacker);
        }

        public override void DoWrite(ByteWriter BW)
        {
            WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleAttackPhase(Map);
        }
    }
}
