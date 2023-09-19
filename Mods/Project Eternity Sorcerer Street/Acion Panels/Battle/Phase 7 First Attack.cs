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
                InvaderFirst = false;
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

            if (Attacker.Creature.BattleAbilities.CriticalHit)
            {
                FinalDamage += FinalDamage / 2;
            }

            if (!Attacker.Creature.BattleAbilities.ScrollAttack && Defender.Creature.BattleAbilities.NeutralizeValue != null)
            {
                bool CanNeutralize = false;

                foreach (AttackTypes ActiveType in Defender.Creature.BattleAbilities.ListNeutralizeType)
                {
                    if (ActiveType == AttackTypes.All
                        || (ActiveType == AttackTypes.Air && Attacker.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                        || (ActiveType == AttackTypes.Fire && Attacker.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                        || (ActiveType == AttackTypes.Earth && Attacker.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                        || (ActiveType == AttackTypes.Water && Attacker.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Water)))
                    {
                        CanNeutralize = true;
                        break;
                    }
                    else if (ActiveType == AttackTypes.NonScrolls && !Attacker.Creature.BattleAbilities.ScrollAttack)
                    {
                        CanNeutralize = true;
                        break;
                    }
                }

                if (CanNeutralize)
                {
                    if (Defender.Creature.BattleAbilities.NeutralizeSignOperator == Core.Operators.NumberTypes.Relative)
                    {
                        NeutralizedDamage = (int)(FinalDamage * (double.Parse(Defender.Creature.BattleAbilities.NeutralizeValue) / 100d));
                        FinalDamage -= NeutralizedDamage;
                    }
                }
            }

            if (!Attacker.Creature.BattleAbilities.ScrollAttack && Defender.Creature.BattleAbilities.ReflectValue != null)
            {
                bool CanReflect = false;
                foreach (AttackTypes ActiveType in Defender.Creature.BattleAbilities.ListReflectType)
                {
                    if (ActiveType == AttackTypes.All
                    || (ActiveType == AttackTypes.Air && Attacker.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                    || (ActiveType == AttackTypes.Fire && Attacker.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                    || (ActiveType == AttackTypes.Earth && Attacker.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                    || (ActiveType == AttackTypes.Water && Attacker.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Water)))
                    {
                        CanReflect = true;
                    }
                    else if (ActiveType == AttackTypes.NonScrolls && !Attacker.Creature.BattleAbilities.ScrollAttack)
                    {
                        CanReflect = true;
                    }
                }

                if (CanReflect)
                {
                    if (Defender.Creature.BattleAbilities.ReflectSignOperator == Core.Operators.NumberTypes.Relative)
                    {
                        ReflectedDamage = (int)(FinalDamage * (double.Parse(Defender.Creature.BattleAbilities.ReflectValue) / 100d));
                        FinalDamage -= ReflectedDamage;
                    }
                }
            }

            bool IsScrollAttack = Attacker.Creature.BattleAbilities.ScrollAttack;

            bool IsPenetratingAttack = Attacker.Creature.BattleAbilities.ScrollAttack;

            if (!IsScrollAttack && Attacker.Creature.BattleAbilities.ArrayPenetrateAffinity != null)
            {
                if (Attacker.Creature.BattleAbilities.ArrayPenetrateAffinity.Length == 0)
                {
                    IsPenetratingAttack = true;
                }
                else
                {
                    foreach (CreatureCard.ElementalAffinity ActiveAffinity in Attacker.Creature.BattleAbilities.ArrayPenetrateAffinity)
                    {
                        if ((ActiveAffinity == CreatureCard.ElementalAffinity.Air && Defender.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                            || (ActiveAffinity == CreatureCard.ElementalAffinity.Fire && Defender.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                            || (ActiveAffinity == CreatureCard.ElementalAffinity.Earth && Defender.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                            || (ActiveAffinity == CreatureCard.ElementalAffinity.Water && Defender.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Water))
                            || (ActiveAffinity == CreatureCard.ElementalAffinity.Neutral && Defender.Creature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Neutral)))
                        {
                            IsPenetratingAttack = true;
                        }
                    }
                }
            }

            if ((IsScrollAttack && Defender.Creature.BattleAbilities.ListNeutralizeType.Contains(AttackTypes.Scrolls))
                || IsPenetratingAttack && Defender.Creature.BattleAbilities.ListNeutralizeType.Contains(AttackTypes.Penetrate))
            {
                Attacker.DamageReceived = 0;
                Defender.DamageReceived = 0;
                Defender.DamageReceivedIgnoreLandBonus = false;
            }

            if (Attacker.Creature.BattleAbilities.ScrollAttack)
            {
                FinalDamage = int.Parse(Attacker.Creature.BattleAbilities.ScrollValue);
            }

            Attacker.DamageReflected = ReflectedDamage;
            Attacker.DamageNeutralized = NeutralizedDamage;
            Attacker.DamageReceived = ReflectedDamage;
            Defender.DamageReceived = FinalDamage;
            Defender.DamageReceivedIgnoreLandBonus = IsScrollAttack || IsPenetratingAttack;
        }

        public void ProcessAttack()
        {
            ActionPanelSorcererStreet AttackerActivationScreen = FirstAttacker.Creature.ActivateInBattle(Map, Map.ListPlayer.IndexOf(Map.GlobalSorcererStreetBattleContext.Invader.Owner));
            ActionPanelSorcererStreet DefenderActivationScreen = SecondAttacker.Creature.ActivateInBattle(Map, Map.ListPlayer.IndexOf(Map.GlobalSorcererStreetBattleContext.Defender.Owner));

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
                List<SkillActivationContext> ListSkillActivation = Map.GlobalSorcererStreetBattleContext.GetAvailableActivation(FirstAttacker, SecondAttacker, AttackBonusRequirement);

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
                    AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Map, SecondAttacker, ActiveAnimationPath, SecondAttacker == Map.GlobalSorcererStreetBattleContext.Defender));
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
