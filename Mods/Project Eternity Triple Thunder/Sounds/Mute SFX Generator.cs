using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using static ProjectEternity.GameScreens.TripleThunderScreen.UnitSounds;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class MuteSFXGenerator : ISFXGenerator
    {
        public ISFXGenerator Copy()
        {
            return this;
        }

        public void PlayBulletHitObjectSound()
        {
        }

        public void PlayChangeGunInvalidSound()
        {
        }

        public void PlayChangeGunValidSound()
        {
        }

        public void PlayCrouchEndSound(CrouchEndSounds CrouchEndSound)
        {
        }

        public void PlayCrouchMoveSound(CrouchMoveSounds CrouchMoveSound)
        {
        }

        public void PlayCrouchStartSound(CrouchStartSounds CrouchStartSound)
        {
        }

        public void PlayDashSound(DashSounds DashSound)
        {
        }

        public void PlayDeathSound(DeathSounds DeathSound)
        {
        }

        public void PlayGetHitSound()
        {
        }

        public void PlayGroundHitSound()
        {
        }

        public void PlayJetpackEndSound(JetpackEndSounds JetpackEndSound)
        {
        }

        public void PlayJetpackLoopSound(JetpackUseSounds JetpackUseSound)
        {
        }

        public void PlayJetpackStartSound(JetpackStartSounds JetpackStartSound)
        {
        }

        public void PlayJumpEndSound(JumpEndSounds JumpEndSound)
        {
        }

        public void PlayJumpStartSound(JumpStartSounds JumpStartSound)
        {
        }

        public void PlayLandSound()
        {
        }

        public void PlayProneEndSound(ProneEndSounds ProneEndSound)
        {
        }

        public void PlayProneMoveSound(ProneMoveSounds ProneMoveSound)
        {
        }

        public void PlayProneStartSound(ProneStartSounds ProneStartSound)
        {
        }

        public void PlayRollSound(RollSounds RollSound)
        {
        }

        public void PlaySpawnSound()
        {
        }

        public void PlayStepGrassSound(StepGrassSounds StepGrassSound)
        {
        }

        public void PlayStepNormalSound(StepNormalSounds StepNormalSound)
        {
        }

        public void PlayStepWaterSound(StepWaterSounds StepWaterSound)
        {
        }

        public void PlayStrainSound(JumpStrainSounds JumpStrainSound)
        {
        }

        public void PrepareExplosionSound(FMODSound sndExplosion, Vector2 ExplosionCenter)
        {
        }

        public void PlayExplosionSounds(List<Player> ListLocalPlayer)
        {
        }
    }
}
