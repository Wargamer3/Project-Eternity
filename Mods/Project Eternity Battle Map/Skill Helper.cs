using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMap
    {
        protected void ActivateAutomaticSkills(Squad ActiveSquad, string SkillToUpdate)
        {
            for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; ++U)
            {
                ActivateAutomaticSkills(ActiveSquad, ActiveSquad[U], SkillToUpdate, ActiveSquad, ActiveSquad[U]);
            }
        }

        public void ActivateAutomaticSkills(Squad ActiveSquad, Unit ActiveUnit, string SkillRequirementToActivate, Squad TargetSquad = null, Unit TargetUnit = null)
        {
            Character TargetPilot;
            if (TargetUnit != null)
                TargetPilot = TargetUnit.Pilot;
            else
                TargetPilot = null;

            GlobalBattleContext.SetContext(ActiveSquad, ActiveUnit, null, TargetSquad, TargetUnit, TargetPilot);
            
            // Character Skills
            for (int C = 0; C < ActiveUnit.ArrayCharacterActive.Length; C++)
            {
                GlobalBattleContext.SetContext(ActiveSquad, ActiveUnit, ActiveUnit.ArrayCharacterActive[C], TargetSquad, TargetUnit, TargetPilot);

                for (int S = 0; S < ActiveUnit.ArrayCharacterActive[C].ArrayPilotSkill.Length; S++)
                {
                    BaseAutomaticSkill ActiveSkill = ActiveUnit.ArrayCharacterActive[C].ArrayPilotSkill[S];
                    ActiveSkill.AddSkillEffectsToTarget(SkillRequirementToActivate);
                }

                for (int S = 0; S < ActiveUnit.ArrayCharacterActive[C].ArrayRelationshipBonus.Length; S++)
                {
                    BaseAutomaticSkill ActiveSkill = ActiveUnit.ArrayCharacterActive[C].ArrayRelationshipBonus[S];
                    ActiveSkill.AddSkillEffectsToTarget(SkillRequirementToActivate);
                }
            }

            GlobalBattleContext.SetContext(ActiveSquad, ActiveUnit, ActiveUnit.Pilot, TargetSquad, TargetUnit, TargetPilot);
            
            // Unit Abilities
            for (int S = 0; S < ActiveUnit.ArrayUnitAbility.Length; S++)
            {
                BaseAutomaticSkill ActiveSkill = ActiveUnit.ArrayUnitAbility[S];
                ActiveSkill.AddSkillEffectsToTarget(SkillRequirementToActivate);
            }

            // Attack Attributes
            if (ActiveUnit.CurrentAttack != null)
            {
                for (int S = 0; S < ActiveUnit.CurrentAttack.ArrayAttackAttributes.Length; S++)
                {
                    BaseAutomaticSkill ActiveSkill = ActiveUnit.CurrentAttack.ArrayAttackAttributes[S];
                    ActiveSkill.AddSkillEffectsToTarget(SkillRequirementToActivate);
                }
            }

            //Reset active effects in case an effect was removed.
            ActiveUnit.ExecuteSkillsEffects();

            GlobalBattleContext.SetContext(null, null, null, TargetSquad, TargetUnit, TargetPilot);

        }
    }
}
