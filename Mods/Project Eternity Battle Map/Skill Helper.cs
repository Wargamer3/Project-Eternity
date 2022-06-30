using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMap
    {
        public void ActivateAutomaticSkills(Squad ActiveSquad, string SkillToUpdate)
        {
            for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; ++U)
            {
                ActivateAutomaticSkills(ActiveSquad, ActiveSquad[U], SkillToUpdate, ActiveSquad, ActiveSquad[U]);
            }
        }

        public void ActivateAutomaticSkills(Squad ActiveSquad, Unit ActiveUnit, string SkillRequirementToActivate, Squad TargetSquad, Unit TargetUnit)
        {
            Character TargetPilot;
            if (TargetUnit != null)
                TargetPilot = TargetUnit.Pilot;
            else
                TargetPilot = null;

            GlobalBattleContext.SetContext(ActiveSquad, ActiveUnit, null, TargetSquad, TargetUnit, TargetPilot, ActiveParser);
            
            // Character Skills
            for (int C = 0; C < ActiveUnit.ArrayCharacterActive.Length; C++)
            {
                GlobalBattleContext.SetContext(ActiveSquad, ActiveUnit, ActiveUnit.ArrayCharacterActive[C], TargetSquad, TargetUnit, TargetPilot, ActiveParser);

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

            GlobalBattleContext.SetContext(ActiveSquad, ActiveUnit, ActiveUnit.Pilot, TargetSquad, TargetUnit, TargetPilot, ActiveParser);
            
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
            ActiveUnit.ReactivateEffects();

            GlobalBattleContext.SetContext(null, null, null, TargetSquad, TargetUnit, TargetPilot, ActiveParser);
        }

        public void ActivateAutomaticSkills(BaseAutomaticSkill SkillToActivate, Squad TargetSquad, Unit TargetUnit)
        {
            Character TargetPilot;
            if (TargetUnit != null)
                TargetPilot = TargetUnit.Pilot;
            else
                TargetPilot = null;

            GlobalBattleContext.SetContext(null, null, null, TargetSquad, TargetUnit, TargetPilot, ActiveParser);

            SkillToActivate.AddSkillEffectsToTarget(string.Empty);

            GlobalBattleContext.SetContext(null, null, null, TargetSquad, TargetUnit, TargetPilot, ActiveParser);
        }
    }
}
