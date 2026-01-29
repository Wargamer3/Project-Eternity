using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using static ProjectEternity.GameScreens.SorcererStreetScreen.SorcererStreetBattleContext;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelFirstCounterAttackPhase : ActionPanelBattle
    {
        private const string PanelName = "FirstCounterAttackPhase";

        private enum AttackSequences { ProcessFirstAttack, ExecuteFirstAttack, End };

        public static bool PlayAnimations = true;
        private AttackSequences AttackSequence;

        public BattleCreatureInfo FirstAttacker;
        public BattleCreatureInfo SecondAttacker;

        public ActionPanelFirstCounterAttackPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            AttackSequence = AttackSequences.ProcessFirstAttack;
            ActionPanelBattleAttackPhase.DetermineAttackOrder(Map.GlobalSorcererStreetBattleContext, out FirstAttacker, out SecondAttacker);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            if (ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelBattleBattleResultPhase(Map));
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

            ActionPanelBattleAttackPhase.ProcessAttack(SecondAttacker, FirstAttacker);

            if (SecondAttacker.DamageReceived > 0)
            {
                Map.GlobalSorcererStreetBattleContext.SetCreatures(FirstAttacker, SecondAttacker);
                List<SkillActivationContext> ListSkillActivation = Map.GlobalSorcererStreetBattleContext.GetAvailableActivation(ActionPanelBattleAttackPhase.AttackBonusRequirement);

                if (ListSkillActivation.Count > 0 && ListSkillActivation[0].DicSkillActivation.Count > 0)
                {
                    ActionPanelBattleItemModifierPhase.StartAnimation(true, ListSkillActivation);
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ReadPlayerInfo(BR, Map);
        }

        public override void DoWrite(ByteWriter BW)
        {
            WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelFirstCounterAttackPhase(Map);
        }
    }
}
