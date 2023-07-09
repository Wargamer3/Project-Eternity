﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using static ProjectEternity.GameScreens.SorcererStreetScreen.SorcererStreetBattleContext;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleAttackPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleAttackPhase";

        private enum AttackSequences { ProcessFirstAttack, ExecuteFirstAttack, ExecuteFirstReflect, ProcessSecondAttack, ExecuteSecondAttack, End };
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
        public int FirstAttackDamage;
        public int FirstReflectDamage;
        public BattleCreatureInfo SecondAttacker;

        public ActionPanelBattleAttackPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            AttackSequence = AttackSequences.ProcessFirstAttack;
            DetermineAttackOrder(Map.GlobalSorcererStreetBattleContext, out FirstAttacker, out SecondAttacker);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            if (AttackSequence == AttackSequences.ProcessFirstAttack)
            {
                ProcessFirstAttack();
            }
            else if (AttackSequence == AttackSequences.ExecuteFirstAttack)
            {
                ExecuteFirstAttack(FirstAttackDamage);
            }
            else if (AttackSequence == AttackSequences.ExecuteFirstReflect)
            {
                ExecuteFirstAttack(FirstAttackDamage);
            }
            else if (AttackSequence == AttackSequences.ProcessSecondAttack)
            {
                ProcessSecondAttack();
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
        /// <param name="Attacker"></param>
        /// <param name="Defender"></param>
        /// <returns>True if defender dies</returns>
        public static int ProcessAttack(SorcererStreetBattleContext GlobalSorcererStreetBattleContext, BattleCreatureInfo Attacker, BattleCreatureInfo Defender, out int ReflectedDamage)
        {
            ReflectedDamage = 0;
            int FinalDamage = GlobalSorcererStreetBattleContext.SelfCreature.FinalST;

            GlobalSorcererStreetBattleContext.ActivateSkill(Attacker, Defender, BeforeAttackRequirement);
            GlobalSorcererStreetBattleContext.ActivateSkill(Defender, Attacker, BeforeDefenseRequirement);

            if (Defender.Creature.BattleAbilities.NeutralizeValue != null)
            {
                bool CanNeutralize = false;
                foreach (AttackTypes ActiveType in Defender.Creature.BattleAbilities.ArrayNeutralizeType)
                {
                    if (ActiveType == AttackTypes.All
                        || (ActiveType == AttackTypes.Air && Attacker.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                        || (ActiveType == AttackTypes.Fire && Attacker.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                        || (ActiveType == AttackTypes.Earth && Attacker.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                        || (ActiveType == AttackTypes.Water && Attacker.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Water)))
                    {
                        CanNeutralize = true;
                    }
                }

                if (CanNeutralize)
                {
                    if (Defender.Creature.BattleAbilities.NeutralizeSignOperator == Core.Operators.NumberTypes.Relative)
                    {
                        FinalDamage -= (int)(FinalDamage * (double.Parse(Defender.Creature.BattleAbilities.NeutralizeValue) / 100d));
                    }
                }
            }

            if (Defender.Creature.BattleAbilities.ReflectValue != null)
            {
                bool CanReflect = false;
                AttackTypes ActiveType = Defender.Creature.BattleAbilities.ReflectType;
                {
                    if (ActiveType == AttackTypes.All
                        || (ActiveType == AttackTypes.Air && Attacker.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                        || (ActiveType == AttackTypes.Fire && Attacker.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                        || (ActiveType == AttackTypes.Earth && Attacker.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                        || (ActiveType == AttackTypes.Water && Attacker.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Water)))
                    {
                        CanReflect = true;
                    }
                }

                if (CanReflect)
                {
                    if (Defender.Creature.BattleAbilities.ReflectSignOperator == Core.Operators.NumberTypes.Relative)
                    {
                        ReflectedDamage = FinalDamage;
                        FinalDamage = (int)(FinalDamage * (double.Parse(Defender.Creature.BattleAbilities.ReflectValue) / 100d));
                        ReflectedDamage -= FinalDamage;
                    }
                }
            }

            if (FinalDamage > 0)
            {
                Attacker.DamageReceived = ReflectedDamage;
                Defender.DamageReceived = FinalDamage;
                GlobalSorcererStreetBattleContext.ActivateSkill(Attacker, Defender, AttackBonusRequirement);
            }

            return FinalDamage;
        }

        public void ProcessFirstAttack()
        {
            ExecutePreAttack(FirstAttacker, SecondAttacker);

            FirstAttackDamage = ProcessAttack(Map.GlobalSorcererStreetBattleContext, FirstAttacker, SecondAttacker, out FirstReflectDamage);
            if (FirstAttackDamage < 0)
            {
                FirstAttackDamage = -FirstAttackDamage;
                AttackSequence = AttackSequences.ExecuteFirstReflect;
            }
            else
            {
                AttackSequence = AttackSequences.ExecuteFirstAttack;
            }
        }

        public void ExecuteFirstAttack(int Damage)
        {
            SecondAttacker.FinalHP = SecondAttacker.Creature.CurrentHP = Math.Max(0, SecondAttacker.FinalHP - Damage);

            if (SecondAttacker.Creature.CurrentHP > 0)
            {
                Map.GlobalSorcererStreetBattleContext.ActivateSkill(FirstAttacker, SecondAttacker, AfterEnemySurvivedRequirement);
                Map.GlobalSorcererStreetBattleContext.ActivateSkill(SecondAttacker, FirstAttacker, BattleEndRequirement);
                AttackSequence = AttackSequences.End;
            }
            else
            {
                Map.GlobalSorcererStreetBattleContext.ActivateSkill(FirstAttacker, SecondAttacker, UponVictoryRequirement);
                Map.GlobalSorcererStreetBattleContext.ActivateSkill(SecondAttacker, FirstAttacker, UponDefeatRequirement);
                AttackSequence = AttackSequences.ProcessSecondAttack;
            }

            if (PlayAnimations)
            {
                foreach (string ActiveAnimationPath in FirstAttacker.GetAttackAnimationPaths())
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, SecondAttacker, ActiveAnimationPath, SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender));
                }
            }
        }

        public void ProcessSecondAttack()
        {
            ExecutePreAttack(SecondAttacker, FirstAttacker);

            FirstAttackDamage = ProcessAttack(Map.GlobalSorcererStreetBattleContext, SecondAttacker, FirstAttacker, out FirstReflectDamage);
            AttackSequence = AttackSequences.ExecuteFirstAttack;
        }

        public void ExecuteSecondAttack(int Damage)
        {
            FirstAttacker.FinalHP = FirstAttacker.Creature.CurrentHP = Math.Max(0, FirstAttacker.FinalHP - Damage);

            if (FirstAttacker.Creature.CurrentHP > 0)
            {
                Map.GlobalSorcererStreetBattleContext.ActivateSkill(SecondAttacker, FirstAttacker, AfterEnemySurvivedRequirement);
                Map.GlobalSorcererStreetBattleContext.ActivateSkill(FirstAttacker, SecondAttacker, BattleEndRequirement);
            }
            else
            {
                Map.GlobalSorcererStreetBattleContext.ActivateSkill(SecondAttacker, FirstAttacker, UponVictoryRequirement);
                Map.GlobalSorcererStreetBattleContext.ActivateSkill(FirstAttacker, SecondAttacker, UponDefeatRequirement);
            }

            AttackSequence = AttackSequences.End;

            if (PlayAnimations)
            {
                foreach (string ActiveAnimationPath in SecondAttacker.GetAttackAnimationPaths())
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, FirstAttacker, ActiveAnimationPath, FirstAttacker == Map.GlobalSorcererStreetBattleContext.Defender));
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ReadPlayerInfo(BR, Map);
            AttackSequence = AttackSequences.ProcessFirstAttack;
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
