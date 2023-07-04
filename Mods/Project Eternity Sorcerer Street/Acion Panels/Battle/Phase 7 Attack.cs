using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleAttackPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleAttackPhase";

        private enum AttackSequences { FirstAttack, SecondAttack, End };

        /*
         * Battle End = you have to survive the battle
         * Upon Defeat = you have to die
         * Upon Victory = you kill
         * Attack Bonus / Instant Death = activates immediately upon doing at least 1 point of damage 
         * Battle Start = activates at the beginning of battle
         * In Battle = activates after the Battle Start things have processed*/

        //Activate creature ability and then activate item battle ability

        public const string InvasionRequirement = "Sorcerer Street Invasion";
        public const string BeforeAttackRequirement = "Sorcerer Street Before Attack";
        public const string BeforeDefenseRequirement = "Sorcerer Street Before Defense";
        public const string AfterEnemySurvivedRequirement = "Sorcerer Street After Enemy Survived";

        public const string AttackBonusRequirement = "Sorcerer Street Attack Bonus";
        public const string BattleStartRequirement = "Sorcerer Street Battle Start";
        public const string BattleEndRequirement = "Sorcerer Street Battle End";
        public const string UponDefeatRequirement = "Sorcerer Street Upon Defeat";
        public const string UponVictoryRequirement = "Sorcerer Street Upon Victory";

        public static bool PlayAnimations = true;
        private AttackSequences AttackSequence;

        public CreatureCard FirstAttacker;
        public ItemCard FirstAttackerItem;
        public Player FirstAttackerPlayer;
        public CreatureCard SecondAttacker;
        public ItemCard SecondAttackerItem;
        public Player SecondAttackerPlayer;

        public ActionPanelBattleAttackPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            AttackSequence = AttackSequences.FirstAttack;
            DetermineAttackOrder(Map.GlobalSorcererStreetBattleContext, out FirstAttacker, out SecondAttacker);
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
        public static void DetermineAttackOrder(SorcererStreetBattleContext GlobalSorcererStreetBattleContext, out CreatureCard FirstAttacker, out CreatureCard SecondAttacker)
        {
            bool InvaderFirst = true;
            bool InvaderDecided = false;

            if (!GlobalSorcererStreetBattleContext.Invader.Creature.BonusAbilities.AttackLast && (GlobalSorcererStreetBattleContext.Invader.Creature.Abilities.AttackFirst || GlobalSorcererStreetBattleContext.Invader.Creature.BonusAbilities.AttackFirst))
            {
                InvaderFirst = true;
                InvaderDecided = true;
            }
            else if (!GlobalSorcererStreetBattleContext.Defender.Creature.BonusAbilities.AttackLast && (GlobalSorcererStreetBattleContext.Defender.Creature.Abilities.AttackFirst || GlobalSorcererStreetBattleContext.Defender.Creature.BonusAbilities.AttackFirst))
            {
                InvaderFirst = false;
                InvaderDecided = true;
            }

            if (!InvaderDecided && (GlobalSorcererStreetBattleContext.Defender.Creature.BonusAbilities.AttackLast || GlobalSorcererStreetBattleContext.Defender.Creature.Abilities.AttackLast))
            {
                InvaderFirst = true;
            }
            else if (!InvaderDecided && (GlobalSorcererStreetBattleContext.Invader.Creature.BonusAbilities.AttackLast || GlobalSorcererStreetBattleContext.Invader.Creature.Abilities.AttackLast))
            {
                InvaderFirst = true;
            }

            if (InvaderFirst)
            {
                FirstAttacker = GlobalSorcererStreetBattleContext.Invader.Creature;
                SecondAttacker = GlobalSorcererStreetBattleContext.Defender.Creature;
            }
            else
            {
                FirstAttacker = GlobalSorcererStreetBattleContext.Defender.Creature;
                SecondAttacker = GlobalSorcererStreetBattleContext.Invader.Creature;
            }
        }

        private void ExecutePreAttack(CreatureCard Attacker, CreatureCard Defender)
        {
            ActionPanelSorcererStreet AttackerActivationScreen = Attacker.ActivateInBattle(Map, Map.ListPlayer.IndexOf(Map.GlobalSorcererStreetBattleContext.Invader.Owner));
            ActionPanelSorcererStreet DefenderActivationScreen = Defender.ActivateInBattle(Map, Map.ListPlayer.IndexOf(Map.GlobalSorcererStreetBattleContext.Defender.Owner));

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

            if (SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender.Creature)
            {
                FinalST = Map.GlobalSorcererStreetBattleContext.Invader.FinalST;
                FinalHP = Map.GlobalSorcererStreetBattleContext.Defender.FinalHP;

                Map.GlobalSorcererStreetBattleContext.ActiveSkill(FirstAttacker, SecondAttacker, FirstAttackerPlayer, SecondAttackerPlayer, BeforeAttackRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(SecondAttacker, FirstAttacker, SecondAttackerPlayer, FirstAttackerPlayer, BeforeAttackRequirement);
            }
            else
            {
                FinalST = Map.GlobalSorcererStreetBattleContext.Defender.FinalST;
                FinalHP = Map.GlobalSorcererStreetBattleContext.Invader.FinalHP;

                Map.GlobalSorcererStreetBattleContext.ActiveSkill(FirstAttacker, SecondAttacker, FirstAttackerPlayer, SecondAttackerPlayer, BeforeDefenseRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(SecondAttacker, FirstAttacker, SecondAttackerPlayer, FirstAttackerPlayer, BeforeDefenseRequirement);
            }

            if (FinalST > 0)
            {
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(FirstAttacker, SecondAttacker, FirstAttackerPlayer, SecondAttackerPlayer, AttackBonusRequirement);
            }

            FinalHP = Math.Max(0, FinalHP - FinalST);
            SecondAttacker.CurrentHP = Math.Min(SecondAttacker.CurrentHP, FinalHP);

            if (SecondAttacker.CurrentHP > 0)
            {
                AttackSequence = AttackSequences.SecondAttack;
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(FirstAttacker, SecondAttacker, FirstAttackerPlayer, SecondAttackerPlayer, AfterEnemySurvivedRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(SecondAttacker, FirstAttacker, SecondAttackerPlayer, FirstAttackerPlayer, BattleEndRequirement);
            }
            else
            {
                AttackSequence = AttackSequences.End;
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(FirstAttacker, SecondAttacker, FirstAttackerPlayer, SecondAttackerPlayer, UponVictoryRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(SecondAttacker, FirstAttacker, SecondAttackerPlayer, FirstAttackerPlayer, UponDefeatRequirement);

            }

            if (PlayAnimations)
            {
                if (FirstAttackerItem == null)
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, FirstAttacker.AttackAnimationPath, SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender.Creature));
                }
                else
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, FirstAttackerItem.ItemActivationAnimationPath, SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender.Creature));
                }
            }
        }

        public void ExecuteSecondAttack()
        {
            ExecutePreAttack(SecondAttacker, FirstAttacker);

            int FinalST;
            int FinalHP;

            if (SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender.Creature)
            {
                FinalST = Map.GlobalSorcererStreetBattleContext.Defender.FinalST;
                FinalHP = Map.GlobalSorcererStreetBattleContext.Invader.FinalHP;

                Map.GlobalSorcererStreetBattleContext.ActiveSkill(SecondAttacker, FirstAttacker, SecondAttackerPlayer, FirstAttackerPlayer, BeforeAttackRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(FirstAttacker, SecondAttacker, FirstAttackerPlayer, SecondAttackerPlayer, BeforeAttackRequirement);
            }
            else
            {
                FinalST = Map.GlobalSorcererStreetBattleContext.Invader.FinalST;
                FinalHP = Map.GlobalSorcererStreetBattleContext.Defender.FinalHP;

                Map.GlobalSorcererStreetBattleContext.ActiveSkill(SecondAttacker, FirstAttacker, SecondAttackerPlayer, FirstAttackerPlayer, BeforeDefenseRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(FirstAttacker, SecondAttacker, FirstAttackerPlayer, SecondAttackerPlayer, BeforeDefenseRequirement);
            }

            if (FinalST > 0)
            {
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(SecondAttacker, FirstAttacker, SecondAttackerPlayer, FirstAttackerPlayer, AttackBonusRequirement);
            }

            FinalHP = Math.Max(0, FinalHP - FinalST);
            FirstAttacker.CurrentHP = Math.Min(FirstAttacker.CurrentHP, FinalHP);

            if (SecondAttacker.CurrentHP > 0)
            {
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(SecondAttacker, FirstAttacker, SecondAttackerPlayer, FirstAttackerPlayer, AfterEnemySurvivedRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(FirstAttacker, SecondAttacker, FirstAttackerPlayer, SecondAttackerPlayer, BattleEndRequirement);
            }
            else
            {
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(SecondAttacker, FirstAttacker, SecondAttackerPlayer, FirstAttackerPlayer, UponVictoryRequirement);
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(FirstAttacker, SecondAttacker, FirstAttackerPlayer, SecondAttackerPlayer, UponDefeatRequirement);
            }

            if (PlayAnimations)
            {
                if (FirstAttackerItem == null)
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, SecondAttacker.AttackAnimationPath, SecondAttacker != Map.GlobalSorcererStreetBattleContext.Defender.Creature));
                }
                else
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, SecondAttacker.AttackAnimationPath, SecondAttacker != Map.GlobalSorcererStreetBattleContext.Defender.Creature));
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
            DetermineAttackOrder(Map.GlobalSorcererStreetBattleContext, out FirstAttacker, out SecondAttacker);
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
