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

        private enum AttackSequences { ProcessFirstAttack, ExecuteFirstAttack, End };
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
            AttackSequence = AttackSequences.ProcessFirstAttack;
            DetermineAttackOrder(Map.GlobalSorcererStreetBattleContext, out FirstAttacker, out SecondAttacker);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            if (ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            if (AttackSequence == AttackSequences.ProcessFirstAttack)
            {
                ProcessAttack();
                AttackSequence = AttackSequences.ExecuteFirstAttack;
            }
            else if (AttackSequence == AttackSequences.ExecuteFirstAttack)
            {
                ExecuteFirstAttack();
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
        public static void ProcessAttack(BattleCreatureInfo Attacker, BattleCreatureInfo Defender)
        {
            int ReflectedDamage = 0;
            int NeutralizedDamage = 0;
            int FinalDamage = Attacker.FinalST;

            CardAbilities AttackerAbilities = Attacker.Creature.GetCurrentAbilities(EffectActivationPhases.None);
            CardAbilities DefenderAbilities = Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None);
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

            if (!AttackerAbilities.ScrollAttack && Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).NeutralizeValue != null)
            {
                bool CanNeutralize = Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).ListNeutralizeType.Count == 0;

                foreach (AttackTypes ActiveType in Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).ListNeutralizeType)
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
                    if (Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).NeutralizeSignOperator == Core.Operators.NumberTypes.Relative)
                    {
                        NeutralizedDamage = (int)(FinalDamage * (double.Parse(Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).NeutralizeValue) / 100d));
                        FinalDamage -= NeutralizedDamage;
                    }
                }
            }

            if (!AttackerAbilities.ScrollAttack && Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).ReflectValue != null)
            {
                bool CanReflect = Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).ListReflectType.Count == 0;

                foreach (AttackTypes ActiveType in Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).ListReflectType)
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
                    if (Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).ReflectSignOperator == Core.Operators.NumberTypes.Relative)
                    {
                        ReflectedDamage = (int)(FinalDamage * (double.Parse(Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).ReflectValue) / 100d));
                        FinalDamage -= ReflectedDamage;
                    }
                }
            }

            if (!IsScrollAttack && AttackerAbilities.ArrayPenetrateAffinity != null)
            {
                if (Attacker.Creature.GetCurrentAbilities(EffectActivationPhases.None).ArrayPenetrateAffinity.Length == 0)
                {
                    IsPenetratingAttack = true;
                }
                else
                {
                    foreach (CreatureCard.ElementalAffinity ActiveAffinity in Attacker.Creature.GetCurrentAbilities(EffectActivationPhases.None).ArrayPenetrateAffinity)
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

            if ((IsScrollAttack && Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).ListNeutralizeType.Contains(AttackTypes.Scrolls))
                || IsPenetratingAttack && Defender.Creature.GetCurrentAbilities(EffectActivationPhases.None).ListNeutralizeType.Contains(AttackTypes.Penetrate))
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

        public void ProcessAttack()
        {
            ActionPanelSorcererStreet AttackerActivationScreen = FirstAttacker.Creature.ActivateInBattle(Map, Map.ListPlayer.IndexOf(Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner));
            ActionPanelSorcererStreet DefenderActivationScreen = SecondAttacker.Creature.ActivateInBattle(Map, Map.ListPlayer.IndexOf(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner));

            if (PlayAnimations && DefenderActivationScreen != null)
            {
                AddToPanelListAndSelect(DefenderActivationScreen);
            }
            if (PlayAnimations && AttackerActivationScreen != null)
            {
                AddToPanelListAndSelect(AttackerActivationScreen);
            }

            ProcessAttack(FirstAttacker, SecondAttacker);

            if (SecondAttacker.DamageReceived > 0)
            {
                Map.GlobalSorcererStreetBattleContext.SetCreatures(FirstAttacker, SecondAttacker);
                List<SkillActivationContext> ListSkillActivation = Map.GlobalSorcererStreetBattleContext.GetAvailableActivation(AttackBonusRequirement);

                if (ListSkillActivation.Count > 0 && ListSkillActivation[0].DicSkillActivation.Count > 0)
                {
                    ActionPanelBattleItemModifierPhase.StartAnimation(true, ListSkillActivation);
                }
            }
        }

        public void ExecuteFirstAttack()
        {
            if (SecondAttacker.Creature.CurrentHP > 0)
            {
                AddToPanelListAndSelect(new ActionPanelFirstCounterAttackPhase(Map));
            }
            else
            {
            }

            if (PlayAnimations)
            {
                foreach (string ActiveAnimationPath in FirstAttacker.GetAttackAnimationPaths())
                {
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, SecondAttacker, ActiveAnimationPath, SecondAttacker == Map.GlobalSorcererStreetBattleContext.OpponentCreature));
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
