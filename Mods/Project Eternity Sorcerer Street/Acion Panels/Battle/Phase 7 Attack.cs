using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using static ProjectEternity.GameScreens.SorcererStreetScreen.SorcererStreetBattleContext;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleAttackPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleAttackPhase";

        private enum AttackSequences { FirstAttack, SecondAttack, End };
        public enum AttackTypes { NonScrolls, Scrolls, Penetrate, All, Neutral, Fire, Water, Earth, Air }

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

        public BattleCreatureInfo FirstAttacker;
        public BattleCreatureInfo SecondAttacker;

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
        public static void DetermineAttackOrder(SorcererStreetBattleContext GlobalSorcererStreetBattleContext, out BattleCreatureInfo FirstAttacker, out BattleCreatureInfo SecondAttacker)
        {
            bool InvaderFirst = true;
            bool InvaderDecided = false;

            if (!GlobalSorcererStreetBattleContext.Invader.Creature.BattleAbilities.AttackLast && GlobalSorcererStreetBattleContext.Invader.Creature.BattleAbilities.AttackFirst)
            {
                InvaderFirst = true;
                InvaderDecided = true;
            }
            else if (!GlobalSorcererStreetBattleContext.Defender.Creature.BattleAbilities.AttackLast && GlobalSorcererStreetBattleContext.Defender.Creature.BattleAbilities.AttackFirst)
            {
                InvaderFirst = false;
                InvaderDecided = true;
            }

            if (!InvaderDecided && GlobalSorcererStreetBattleContext.Defender.Creature.BattleAbilities.AttackLast)
            {
                InvaderFirst = true;
            }
            else if (!InvaderDecided && GlobalSorcererStreetBattleContext.Invader.Creature.BattleAbilities.AttackLast)
            {
                InvaderFirst = true;
            }

            if (InvaderFirst)
            {
                FirstAttacker = GlobalSorcererStreetBattleContext.Invader;
                SecondAttacker = GlobalSorcererStreetBattleContext.Defender;
            }
            else
            {
                FirstAttacker = GlobalSorcererStreetBattleContext.Defender;
                SecondAttacker = GlobalSorcererStreetBattleContext.Invader;
            }
        }

        private void ExecutePreAttack(BattleCreatureInfo Attacker, BattleCreatureInfo Defender)
        {
            ActionPanelSorcererStreet AttackerActivationScreen = Attacker.Creature.ActivateInBattle(Map, Map.ListPlayer.IndexOf(Map.GlobalSorcererStreetBattleContext.Invader.Owner));
            ActionPanelSorcererStreet DefenderActivationScreen = Defender.Creature.ActivateInBattle(Map, Map.ListPlayer.IndexOf(Map.GlobalSorcererStreetBattleContext.Defender.Owner));

            if (PlayAnimations && DefenderActivationScreen != null)
            {
                AddToPanelListAndSelect(DefenderActivationScreen);
            }
            if (PlayAnimations && AttackerActivationScreen != null)
            {
                AddToPanelListAndSelect(AttackerActivationScreen);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GlobalSorcererStreetBattleContext"></param>
        /// <param name="FirstAttacker"></param>
        /// <param name="SecondAttacker"></param>
        /// <returns>True if defender dies</returns>
        public static bool ExecutetAttack(SorcererStreetBattleContext GlobalSorcererStreetBattleContext, BattleCreatureInfo FirstAttacker, BattleCreatureInfo SecondAttacker)
        {
            int FinalST;
            int FinalHP;

            if (SecondAttacker == GlobalSorcererStreetBattleContext.Defender)
            {
                FinalST = GlobalSorcererStreetBattleContext.Invader.FinalST;
                FinalHP = GlobalSorcererStreetBattleContext.Defender.FinalHP;

                GlobalSorcererStreetBattleContext.ActivateSkill(FirstAttacker, SecondAttacker, BeforeAttackRequirement);
                GlobalSorcererStreetBattleContext.ActivateSkill(SecondAttacker, FirstAttacker, BeforeAttackRequirement);
            }
            else
            {
                FinalST = GlobalSorcererStreetBattleContext.Defender.FinalST;
                FinalHP = GlobalSorcererStreetBattleContext.Invader.FinalHP;

                GlobalSorcererStreetBattleContext.ActivateSkill(FirstAttacker, SecondAttacker, BeforeDefenseRequirement);
                GlobalSorcererStreetBattleContext.ActivateSkill(SecondAttacker, FirstAttacker, BeforeDefenseRequirement);
            }

            if (FinalST > 0)
            {
                GlobalSorcererStreetBattleContext.ActivateSkill(FirstAttacker, SecondAttacker, AttackBonusRequirement);
            }

            FinalHP = Math.Max(0, FinalHP - FinalST);
            SecondAttacker.FinalHP = SecondAttacker.Creature.CurrentHP = Math.Min(SecondAttacker.FinalHP, FinalHP);

            if (SecondAttacker.Creature.CurrentHP > 0)
            {
                GlobalSorcererStreetBattleContext.ActivateSkill(FirstAttacker, SecondAttacker, AfterEnemySurvivedRequirement);
                GlobalSorcererStreetBattleContext.ActivateSkill(SecondAttacker, FirstAttacker, BattleEndRequirement);
                return false;
            }
            else
            {
                GlobalSorcererStreetBattleContext.ActivateSkill(FirstAttacker, SecondAttacker, UponVictoryRequirement);
                GlobalSorcererStreetBattleContext.ActivateSkill(SecondAttacker, FirstAttacker, UponDefeatRequirement);
                return true;

            }
        }

        public void ExecuteFirstAttack()
        {
            ExecutePreAttack(FirstAttacker, SecondAttacker);

            if (ExecutetAttack(Map.GlobalSorcererStreetBattleContext, FirstAttacker, SecondAttacker))
            {
                AttackSequence = AttackSequences.End;
            }
            else
            {
                AttackSequence = AttackSequences.SecondAttack;
            }

            if (PlayAnimations)
            {
                foreach (string ActiveAnimationPath in FirstAttacker.GetAttackAnimationPaths())
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, ActiveAnimationPath, SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender));
                }
            }
        }

        public void ExecuteSecondAttack()
        {
            ExecutePreAttack(SecondAttacker, FirstAttacker);

            ExecutetAttack(Map.GlobalSorcererStreetBattleContext, SecondAttacker, FirstAttacker);

            if (PlayAnimations)
            {
                foreach (string ActiveAnimationPath in SecondAttacker.GetAttackAnimationPaths())
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, ActiveAnimationPath, SecondAttacker != Map.GlobalSorcererStreetBattleContext.Defender));
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
