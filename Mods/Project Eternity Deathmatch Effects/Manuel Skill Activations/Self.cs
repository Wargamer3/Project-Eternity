using ProjectEternity.Core.Effects;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Skill
{
    public class PilotSkillActivationSelf : ManualSkillActivationDeathmatch
    {
        public PilotSkillActivationSelf()
            : this(null)
        {

        }

        public PilotSkillActivationSelf(DeathmatchParams Params)
            : base("Self", false, Params)
        {

        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            if (Params.GlobalContext.EffectOwnerSquad.CurrentLeader == null || Params.GlobalContext.EffectOwnerSquad.CurrentLeader.Pilot == null || Params.GlobalContext.EffectOwnerSquad.IsDead)
                return false;

            Params.GlobalContext.SetContext(Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter, Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter, Params.Map.ActiveParser);
            
            return ActiveSkill.CanActivateEffectsOnTarget(Params.GlobalContext.EffectOwnerCharacter.Effects);
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Params.GlobalContext.SetContext(Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter, Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter, Params.Map.ActiveParser);

            AddAndExecuteEffect(ActiveSkill, Params.GlobalContext.EffectOwnerCharacter.Effects, SkillEffect.LifetimeTypeTurns + Params.Map.ActivePlayerIndex);
            Params.GlobalContext.EffectOwnerCharacter.SP -= ActiveSkill.SPCost;
        }

        public override ManualSkillTarget Copy()
        {
            return new PilotSkillActivationSelf(Params);
        }
    }
}
