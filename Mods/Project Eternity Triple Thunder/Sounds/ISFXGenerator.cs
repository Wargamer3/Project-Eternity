using FMOD;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static ProjectEternity.GameScreens.TripleThunderScreen.UnitSounds;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public interface ISFXGenerator
    {
        ISFXGenerator Copy();

        void PlayCrouchStartSound(CrouchStartSounds CrouchStartSound);
        void PlayCrouchMoveSound(CrouchMoveSounds CrouchMoveSound);
        void PlayCrouchEndSound(CrouchEndSounds CrouchEndSound);

        void PlayJetpackStartSound(JetpackStartSounds JetpackStartSound);
        void PlayJetpackLoopSound(JetpackUseSounds JetpackUseSound);
        void PlayJetpackEndSound(JetpackEndSounds JetpackEndSound);

        void PlayProneStartSound(ProneStartSounds ProneStartSound);
        void PlayProneMoveSound(ProneMoveSounds ProneMoveSound);
        void PlayProneEndSound(ProneEndSounds ProneEndSound);

        void PlayJumpStartSound(JumpStartSounds JumpStartSound);
        void PlayJumpEndSound(JumpEndSounds JumpEndSound);
        void PlayStrainSound(JumpStrainSounds JumpStrainSound);

        void PlayStepNormalSound(StepNormalSounds StepNormalSound);
        void PlayStepGrassSound(StepGrassSounds StepGrassSound);
        void PlayStepWaterSound(StepWaterSounds StepWaterSound);

        void PlayDeathSound(DeathSounds DeathSound);
        void PlayRollSound(RollSounds RollSound);
        void PlayDashSound(DashSounds DashSound);

        //Non customizable
        void PlayLandSound();
        void PlaySpawnSound();
        void PlayChangeGunInvalidSound();
        void PlayChangeGunValidSound();

        void PlayGetHitSound();

        //Bullet
        void PlayBulletHitObjectSound();
        void PlayGroundHitSound();
        void PrepareExplosionSound(FMODSound sndExplosion, Vector2 ExplosionCenter);
        void PlayExplosionSounds(List<Player> ListLocalPlayer);
    }
}