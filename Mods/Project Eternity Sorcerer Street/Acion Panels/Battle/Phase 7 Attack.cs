using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using static ProjectEternity.GameScreens.SorcererStreetScreen.SorcererStreetBattleContext;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleAttackPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleAttackPhase";

        private enum PhasesChoices { InvaderAttackBonusAnimation, InvaderAttackPhase1, InvaderAttackPhase2, CounterAttackBonusAnimation, CounterPhase1, CounterPhase2, End };
        public enum AttackTypes { NonScrolls, Scrolls, Penetrate, All, Neutral, Fire, Water, Earth, Air }

        /*
         * Battle End = you have to survive the battle
         * Upon Defeat = you have to die in the battle (aka spells won't activate it)
         * Upon Victory = you kill
         * Attack Bonus / Instant Death = activates immediately upon doing at least 1 point of damage 
         * Battle Start = activates at the beginning of battle
         * In Battle = activates after the Battle Start things have processed*/

        //Activate creature ability and then activate item battle ability

        public const string InvasionRequirement = "Sorcerer Street Invasion";
        public const string BeforeAttackRequirement = "Sorcerer Street Before Attack";
        public const string BeforeDefenseRequirement = "Sorcerer Street Before Defense";
        public const string AfterEnemySurvivedRequirement = "Sorcerer Street After Enemy Survived";

        public const string BeforeBattleStartRequirement = "Sorcerer Street Before Battle Start";
        public const string BattleStartRequirement = "Sorcerer Street Battle Start";
        public const string AttackBonusRequirement = "Sorcerer Street Attack Bonus";
        public const string BattleEndRequirement = "Sorcerer Street Battle End";
        public const string UponDefeatRequirement = "Sorcerer Street Upon Defeat";
        public const string UponVictoryRequirement = "Sorcerer Street Upon Victory";

        public static bool PlayAnimations = true;
        private PhasesChoices PhasesChoice;

        private BattleContent BattleAssets;

        public BattleCreatureInfo FirstAttacker;
        public BattleCreatureInfo SecondAttacker;

        public ActionPanelBattleAttackPhase(SorcererStreetMap Map, BattleContent BattleAssets)
            : base(Map, PanelName)
        {
            this.BattleAssets = BattleAssets;
        }

        public override void OnSelect()
        {
            PhasesChoice = PhasesChoices.InvaderAttackBonusAnimation;
            Map.GlobalSorcererStreetBattleContext.SetCreatures(Map.GlobalSorcererStreetBattleContext.InvaderCreature, Map.GlobalSorcererStreetBattleContext.DefenderCreature);
            DetermineAttackOrder(Map.GlobalSorcererStreetBattleContext, out FirstAttacker, out SecondAttacker);

            ActionPanelSorcererStreet AttackerActivationScreen = FirstAttacker.Creature.ActivateInBattle(Map, Map.ListPlayer.IndexOf(FirstAttacker.Owner));
            ActionPanelSorcererStreet DefenderActivationScreen = SecondAttacker.Creature.ActivateInBattle(Map, Map.ListPlayer.IndexOf(SecondAttacker.Owner));

            if (PlayAnimations && DefenderActivationScreen != null)
            {
                AddToPanelListAndSelect(DefenderActivationScreen);
            }
            if (PlayAnimations && AttackerActivationScreen != null)
            {
                AddToPanelListAndSelect(AttackerActivationScreen);
            }

            ProcessAttackBonusAnimation(FirstAttacker, SecondAttacker);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            if (ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            if (SecondAttacker.FinalHP < 0)
            {
                PhasesChoice = PhasesChoices.End;
            }
            if (PhasesChoice == PhasesChoices.InvaderAttackBonusAnimation)
            {
                ExecuteFirstAttack();
                PhasesChoice = PhasesChoices.InvaderAttackPhase1;
            }
            else if (PhasesChoice == PhasesChoices.CounterAttackBonusAnimation)
            {
                ExecuteCounterAttack();
                PhasesChoice = PhasesChoices.CounterPhase1;
            }
            else if (PhasesChoice == PhasesChoices.InvaderAttackPhase1)
            {
                if (FirstAttacker.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).AttackTwice)
                {
                    ExecuteFirstAttack();
                    PhasesChoice = PhasesChoices.InvaderAttackPhase2;
                }
                else
                {
                    ProcessAttackBonusAnimation(SecondAttacker, FirstAttacker);
                    PhasesChoice = PhasesChoices.CounterAttackBonusAnimation;
                }
            }
            else if (PhasesChoice == PhasesChoices.InvaderAttackPhase2)
            {
                ProcessAttackBonusAnimation(SecondAttacker, FirstAttacker);
                PhasesChoice = PhasesChoices.CounterAttackBonusAnimation;
            }
            else if (PhasesChoice == PhasesChoices.CounterPhase1)
            {
                if (SecondAttacker.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).AttackTwice)
                {
                    ExecuteCounterAttack();
                    PhasesChoice = PhasesChoices.CounterPhase2;
                }
                else
                {
                    PhasesChoice = PhasesChoices.End;
                }
            }
            else if (PhasesChoice == PhasesChoices.CounterPhase2)
            {
                PhasesChoice = PhasesChoices.End;
            }
            else if (PhasesChoice == PhasesChoices.End)
            {
                ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Map.GlobalSorcererStreetBattleContext, Map.GlobalSorcererStreetBattleContext.InvaderCreature.FinalHP > 0, ActionPanelBattleAttackPhase.BattleEndRequirement);
                RemoveFromPanelList(this);
                AddToPanelListAndSelect(new ActionPanelBattleBattleResultPhase(Map, BattleAssets));
            }
        }

        //Attacks First Invader, Attacks First Defender, Normal Attack Invader, Normal Attack Defender, Attacks Last Invader and Attacks Last Defender
        public static void DetermineAttackOrder(SorcererStreetBattleContext GlobalSorcererStreetBattleContext, out BattleCreatureInfo FirstAttacker, out BattleCreatureInfo SecondAttacker)
        {
            bool InvaderFirst = true;
            bool InvaderDecided = false;

            if (!GlobalSorcererStreetBattleContext.SelfCreature.Creature.GetCurrentAbilities(GlobalSorcererStreetBattleContext.EffectActivationPhase).AttackLast && GlobalSorcererStreetBattleContext.SelfCreature.Creature.GetCurrentAbilities(GlobalSorcererStreetBattleContext.EffectActivationPhase).AttackFirst)
            {
                InvaderFirst = true;
                InvaderDecided = true;
            }
            else if (!GlobalSorcererStreetBattleContext.OpponentCreature.Creature.GetCurrentAbilities(GlobalSorcererStreetBattleContext.EffectActivationPhase).AttackLast && GlobalSorcererStreetBattleContext.OpponentCreature.Creature.GetCurrentAbilities(GlobalSorcererStreetBattleContext.EffectActivationPhase).AttackFirst)
            {
                InvaderFirst = false;
                InvaderDecided = true;
            }

            if (!InvaderDecided && GlobalSorcererStreetBattleContext.OpponentCreature.Creature.GetCurrentAbilities(GlobalSorcererStreetBattleContext.EffectActivationPhase).AttackLast)
            {
                InvaderFirst = true;
            }
            else if (!InvaderDecided && GlobalSorcererStreetBattleContext.SelfCreature.Creature.GetCurrentAbilities(GlobalSorcererStreetBattleContext.EffectActivationPhase).AttackLast)
            {
                InvaderFirst = false;
            }

            if (InvaderFirst)
            {
                FirstAttacker = GlobalSorcererStreetBattleContext.SelfCreature;
                SecondAttacker = GlobalSorcererStreetBattleContext.OpponentCreature;
            }
            else
            {
                FirstAttacker = GlobalSorcererStreetBattleContext.OpponentCreature;
                SecondAttacker = GlobalSorcererStreetBattleContext.SelfCreature;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GlobalSorcererStreetBattleContext"></param>
        /// <param name="Attacker"></param>
        /// <param name="Defender"></param>
        /// <returns>True if defender dies</returns>
        public static void ComputeDamage(BattleCreatureInfo Attacker, BattleCreatureInfo Defender)
        {
            int ReflectedDamage = 0;
            int NeutralizedDamage = 0;
            int FinalDamage = Attacker.FinalST;

            CardAbilities AttackerAbilities = Attacker.Creature.GetCurrentAbilities(EffectActivationPhases.Battle);
            CardAbilities DefenderAbilities = Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle);
            if (AttackerAbilities.DamageModifier > 0)
            {
                FinalDamage = AttackerAbilities.DamageModifier;
            }

            FinalDamage = (int)(FinalDamage * AttackerAbilities.DamageMultiplier);

            bool IsScrollAttack = AttackerAbilities.ScrollAttack;
            bool IsPenetratingAttack = AttackerAbilities.ScrollAttack;

            if (AttackerAbilities.CriticalHit && !IsScrollAttack)
            {
                FinalDamage += FinalDamage / 2;
            }
            else if (IsScrollAttack)
            {
                if (AttackerAbilities.ScrollValue == 0)
                {
                }
                else
                {
                    FinalDamage = AttackerAbilities.ScrollValue;
                }

                if (AttackerAbilities.ScrollCriticalHit)
                {
                    FinalDamage += FinalDamage / 2;
                }
            }

            if (!AttackerAbilities.ScrollAttack && Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).NeutralizeValue != null)
            {
                bool CanNeutralize = Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).ListNeutralizeType.Count > 0;

                foreach (AttackTypes ActiveType in Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).ListNeutralizeType)
                {
                    if (ActiveType == AttackTypes.All
                        || (ActiveType == AttackTypes.Air && AttackerAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                        || (ActiveType == AttackTypes.Fire && AttackerAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                        || (ActiveType == AttackTypes.Earth && AttackerAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                        || (ActiveType == AttackTypes.Water && AttackerAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Water))
                        || (ActiveType == AttackTypes.Neutral && AttackerAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Neutral)))
                    {
                        CanNeutralize = true;
                        break;
                    }
                    else if (ActiveType == AttackTypes.NonScrolls && !AttackerAbilities.ScrollAttack)
                    {
                        CanNeutralize = true;
                        break;
                    }
                }

                if (CanNeutralize)
                {
                    if (Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).NeutralizeSignOperator == Core.Operators.NumberTypes.Relative)
                    {
                        NeutralizedDamage = (int)(FinalDamage * (double.Parse(Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).NeutralizeValue) / 100d));
                        FinalDamage -= NeutralizedDamage;
                    }
                }
            }

            if (!AttackerAbilities.ScrollAttack && Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).ReflectValue != null)
            {
                bool CanReflect = Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).ListReflectType.Count > 0;

                foreach (AttackTypes ActiveType in Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).ListReflectType)
                {
                    if (ActiveType == AttackTypes.All
                    || (ActiveType == AttackTypes.Air && AttackerAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                    || (ActiveType == AttackTypes.Fire && AttackerAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                    || (ActiveType == AttackTypes.Earth && AttackerAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                    || (ActiveType == AttackTypes.Water && AttackerAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Water))
                    || (ActiveType == AttackTypes.Neutral && AttackerAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Neutral)))
                    {
                        CanReflect = true;
                    }
                    else if (ActiveType == AttackTypes.NonScrolls && !AttackerAbilities.ScrollAttack)
                    {
                        CanReflect = true;
                    }
                }

                if (CanReflect)
                {
                    if (Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).ReflectSignOperator == Core.Operators.NumberTypes.Relative)
                    {
                        ReflectedDamage = (int)(FinalDamage * (double.Parse(Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).ReflectValue) / 100d));
                        FinalDamage -= ReflectedDamage;
                    }
                }
            }

            if (!IsScrollAttack && AttackerAbilities.ArrayPenetrateAffinity != null)
            {
                if (Attacker.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).ArrayPenetrateAffinity.Length > 0)
                {
                    IsPenetratingAttack = true;
                }
                else
                {
                    foreach (CreatureCard.ElementalAffinity ActiveAffinity in Attacker.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).ArrayPenetrateAffinity)
                    {
                        if ((ActiveAffinity == CreatureCard.ElementalAffinity.Air && DefenderAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                            || (ActiveAffinity == CreatureCard.ElementalAffinity.Fire && DefenderAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                            || (ActiveAffinity == CreatureCard.ElementalAffinity.Earth && DefenderAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                            || (ActiveAffinity == CreatureCard.ElementalAffinity.Water && DefenderAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Water))
                            || (ActiveAffinity == CreatureCard.ElementalAffinity.Neutral && DefenderAbilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Neutral)))
                        {
                            IsPenetratingAttack = true;
                        }
                    }
                }
            }

            if ((IsScrollAttack && Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).ListNeutralizeType.Contains(AttackTypes.Scrolls))
                || IsPenetratingAttack && Defender.Creature.GetCurrentAbilities(EffectActivationPhases.Battle).ListNeutralizeType.Contains(AttackTypes.Penetrate))
            {
                Attacker.DamageReceived = 0;
                Defender.DamageReceived = 0;
                Defender.DamageReceivedIgnoreLandBonus = false;
                return;
            }

            if (ReflectedDamage > 0)
            {
                Attacker.DamageReflectedByOpponent = ReflectedDamage;
                Attacker.DamageReceived = ReflectedDamage;
            }

            Attacker.DamageNeutralizedByOpponent = NeutralizedDamage;

            Defender.DamageReceived = FinalDamage;
            Defender.DamageReceivedIgnoreLandBonus = IsScrollAttack || IsPenetratingAttack;
        }

        public void ProcessAttackBonusAnimation(BattleCreatureInfo FirstAttacker, BattleCreatureInfo SecondAttacker)
        {
            ComputeDamage(FirstAttacker, SecondAttacker);

            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Map.GlobalSorcererStreetBattleContext, FirstAttacker == Map.GlobalSorcererStreetBattleContext.InvaderCreature, AttackBonusRequirement);
        }

        public void ExecuteFirstAttack()
        {
            if (PlayAnimations)
            {
                foreach (string ActiveAnimationPath in FirstAttacker.GetAttackAnimationPaths())
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, SecondAttacker, ActiveAnimationPath, FirstAttacker == Map.GlobalSorcererStreetBattleContext.InvaderCreature));
                }
            }
        }

        public void ExecuteCounterAttack()
        {
            if (PlayAnimations)
            {
                foreach (string ActiveAnimationPath in SecondAttacker.GetAttackAnimationPaths())
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, FirstAttacker, ActiveAnimationPath, FirstAttacker == Map.GlobalSorcererStreetBattleContext.DefenderCreature));
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ReadPlayerInfo(BR, Map);
            DetermineAttackOrder(Map.GlobalSorcererStreetBattleContext, out FirstAttacker, out SecondAttacker);
        }

        public override void DoWrite(ByteWriter BW)
        {
            WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleAttackPhase(Map, BattleAssets);
        }
    }
}
