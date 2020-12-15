using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using static ProjectEternity.GameScreens.TripleThunderScreen.UnitSounds;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class SFXGenerator : ISFXGenerator
    {
        private readonly Random Random = new Random();
        private readonly List<Tuple<Vector2, FMODSound>> ListExplosionSound;

        #region Ressources

        private FMODSound ChangeGunInvalidSound;
        private FMODSound ChangeGunValidSound;
        private FMODSound CrouchStartSound;
        private FMODSound CrouchEndSound;
        private FMODSound DashSound;
        private FMODSound GroundHitSound;

        private FMODSound JetpackEndSound;
        private FMODSound JetpackLoopSound;
        private FMODSound JetpackStartSound;
        private FMODSound CurrentJetpackSound;

        private FMODSound JumpEndSound;
        private FMODSound JumpStartSound;

        private FMODSound ProneEndSound;
        private FMODSound ProneMoveSound;
        private FMODSound ProneStartSound;
        private FMODSound CurrentProneSound;

        private FMODSound RollSound;
        private FMODSound SpawnSound;

        private FMODSound[] ArrayBulletHitObjectSound;
        private FMODSound CurrentBulletHitObjectSound;

        private FMODSound[] ArrayCrouchMoveSound;
        private FMODSound CurrentCrouchMoveSound;

        private FMODSound[] ArrayDeathFemaleSound;
        private FMODSound[] ArrayDeathMaleSound;

        private FMODSound[] ArrayGetHitSound;
        private FMODSound CurrentGetHitSound;

        private FMODSound[] ArrayLandSound;

        private FMODSound[] ArrayStepGrassSound;
        private FMODSound[] ArrayStepNormalSound;
        private FMODSound[] ArrayStepWaterSound;
        private FMODSound CurrentStepSound;

        //Play with jump sound
        private FMODSound[] ArrayYellFemaleSound;
        private FMODSound[] ArrayYellMaleSound;

        #endregion

        public SFXGenerator()
        {
            ListExplosionSound = new List<Tuple<Vector2, FMODSound>>();
        }

        public void LoadAllSounds()
        {
            ChangeGunInvalidSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Change Gun Invalid.wav");
            ChangeGunValidSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Change Gun Valid.wav");
            CrouchStartSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Crouch Start.wav");
            CrouchEndSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Crouch End.wav");
            DashSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Dash.wav");
            GroundHitSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Ground Hit.wav");

            JetpackEndSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Jetpack End.wav");
            JetpackLoopSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Jetpack Loop.wav");
            JetpackStartSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Jetpack Start.wav");
            CurrentJetpackSound = JetpackEndSound;

            JumpEndSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Jump End.wav");
            JumpStartSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Jump Start.wav");

            ProneMoveSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Prone End.wav");
            ProneEndSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Prone Move.wav");
            ProneStartSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Prone Start.wav");
            CurrentProneSound = ProneEndSound;

            RollSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Roll.wav");
            SpawnSound = new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Spawn.wav");

            SetBulletHitObjectSounds(new FMODSound[]
            {
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Bullet Hit Object 1.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Bullet Hit Object 2.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Bullet Hit Object 3.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Bullet Hit Object 4.wav"),
            });

            SetCrouchMoveSounds(new FMODSound[]
            {
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Crouch Move 1.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Crouch Move 2.wav"),
            });

            SetLandSounds(new FMODSound[]
            {
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Land 1.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Land 2.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Land 3.wav"),
            });

            SetStepSounds(new FMODSound[]
            {
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Grass 1.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Grass 2.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Grass 3.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Grass 4.wav"),
            },
            new FMODSound[]
            {
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Normal 1.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Normal 2.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Normal 3.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Normal 4.wav"),
            },
            new FMODSound[]
            {
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Water 1.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Water 2.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Water 3.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Step Water 4.wav"),
            });

            SetDeathSounds(new FMODSound[]
            {
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Death Female 1.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Death Female 2.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Death Female 3.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Death Female 4.wav"),
            },
            new FMODSound[]
            {
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Death Male 1.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Death Male 2.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Death Male 3.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Death Male 4.wav"),
            });

            SetGetHitSounds(new FMODSound[]
            {
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Get Hit 1.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Get Hit 2.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Get Hit 3.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Get Hit 4.wav"),
            });

            SetYellSounds(new FMODSound[]
            {
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Yell Female 1.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Yell Female 2.wav"),
            },
            new FMODSound[]
            {
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Yell Male 1.wav"),
                new FMODSound(GameScreen.FMODSystem, "Content/Triple Thunder/SFX/Yell Male 2.wav"),
            });
        }

        public ISFXGenerator Copy()
        {
            SFXGenerator NewSFXGenerator = new SFXGenerator();

            NewSFXGenerator.ChangeGunInvalidSound = ChangeGunInvalidSound;
            NewSFXGenerator.ChangeGunValidSound = ChangeGunValidSound;
            NewSFXGenerator.CrouchStartSound = CrouchStartSound;
            NewSFXGenerator.CrouchEndSound = CrouchEndSound;
            NewSFXGenerator.DashSound = DashSound;
            NewSFXGenerator.GroundHitSound = GroundHitSound;

            NewSFXGenerator.JetpackEndSound = JetpackEndSound;
            NewSFXGenerator.JetpackLoopSound = JetpackLoopSound;
            NewSFXGenerator.JetpackStartSound = JetpackStartSound;
            NewSFXGenerator.CurrentJetpackSound = CurrentJetpackSound;

            NewSFXGenerator.JumpEndSound = JumpEndSound;
            NewSFXGenerator.JumpStartSound = JumpStartSound;

            NewSFXGenerator.ProneEndSound = ProneEndSound;
            NewSFXGenerator.ProneMoveSound = ProneMoveSound;
            NewSFXGenerator.ProneStartSound = ProneStartSound;
            NewSFXGenerator.CurrentProneSound = CurrentProneSound;

            NewSFXGenerator.RollSound = RollSound;
            NewSFXGenerator.SpawnSound = SpawnSound;

            NewSFXGenerator.ArrayBulletHitObjectSound = ArrayBulletHitObjectSound;
            NewSFXGenerator.CurrentBulletHitObjectSound = CurrentBulletHitObjectSound;

            NewSFXGenerator.ArrayCrouchMoveSound = ArrayCrouchMoveSound;
            NewSFXGenerator.CurrentCrouchMoveSound = CurrentCrouchMoveSound;

            NewSFXGenerator.ArrayDeathFemaleSound = ArrayDeathFemaleSound;
            NewSFXGenerator.ArrayDeathMaleSound = ArrayDeathMaleSound;

            NewSFXGenerator.ArrayGetHitSound = ArrayGetHitSound;
            NewSFXGenerator.CurrentGetHitSound = CurrentGetHitSound;

            NewSFXGenerator.ArrayLandSound = ArrayLandSound;

            NewSFXGenerator.ArrayStepGrassSound = ArrayStepGrassSound;
            NewSFXGenerator.ArrayStepNormalSound = ArrayStepNormalSound;
            NewSFXGenerator.ArrayStepWaterSound = ArrayStepWaterSound;
            NewSFXGenerator.CurrentStepSound = CurrentStepSound;

            NewSFXGenerator.ArrayYellFemaleSound = ArrayYellFemaleSound;
            NewSFXGenerator.ArrayYellMaleSound = ArrayYellMaleSound;

            return NewSFXGenerator;
        }

        private void SetBulletHitObjectSounds(FMODSound[] ArrayBulletHitObjectSound)
        {
            this.ArrayBulletHitObjectSound = ArrayBulletHitObjectSound;
            CurrentBulletHitObjectSound = ArrayBulletHitObjectSound[0];
        }

        private void SetCrouchMoveSounds(FMODSound[] ArrayCrouchMoveSound)
        {
            this.ArrayCrouchMoveSound = ArrayCrouchMoveSound;
            CurrentCrouchMoveSound = ArrayCrouchMoveSound[0];
        }

        private void SetDeathSounds(FMODSound[] ArrayDeathFemaleSound, FMODSound[] ArrayDeathMaleSound)
        {
            this.ArrayDeathFemaleSound = ArrayDeathFemaleSound;
            this.ArrayDeathMaleSound = ArrayDeathMaleSound;
        }

        private void SetLandSounds(FMODSound[] ArrayLandSound)
        {
            this.ArrayLandSound = ArrayLandSound;
        }

        private void SetStepSounds(FMODSound[] ArrayStepGrassSound, FMODSound[] ArrayStepNormalSound, FMODSound[] ArrayStepWaterSound)
        {
            this.ArrayStepGrassSound = ArrayStepGrassSound;
            this.ArrayStepNormalSound = ArrayStepNormalSound;
            this.ArrayStepWaterSound = ArrayStepWaterSound;
            CurrentStepSound = ArrayStepGrassSound[0];
        }

        private void SetGetHitSounds(FMODSound[] ArrayGetHitSound)
        {
            this.ArrayGetHitSound = ArrayGetHitSound;
            CurrentGetHitSound = ArrayGetHitSound[0];
        }

        private void SetYellSounds(FMODSound[] ArrayYellFemaleSound, FMODSound[] ArrayYellMaleSound)
        {
            this.ArrayYellFemaleSound = ArrayYellFemaleSound;
            this.ArrayYellMaleSound = ArrayYellMaleSound;
        }

        public void PlayCrouchStartSound(CrouchStartSounds CrouchStartSound)
        {
            this.CrouchStartSound.Play();
        }

        public void PlayCrouchMoveSound(CrouchMoveSounds CrouchMoveSound)
        {
            if (!CurrentCrouchMoveSound.IsPlaying())
            {
                CurrentCrouchMoveSound = ArrayCrouchMoveSound[Random.Next(0, ArrayCrouchMoveSound.Length)];
                CurrentCrouchMoveSound.Play();
            }
        }

        public void PlayCrouchEndSound(CrouchEndSounds CrouchEndSound)
        {
            this.CrouchEndSound.Play();
        }

        public void PlayJetpackStartSound(JetpackStartSounds JetpackStartSound)
        {
            if (!CurrentJetpackSound.IsPlaying())
            {
                this.JetpackStartSound.PlayAsBackgroundSFX();
                CurrentJetpackSound = this.JetpackStartSound;
            }
        }

        public void PlayJetpackLoopSound(JetpackUseSounds JetpackUseSound)
        {
            if (!CurrentJetpackSound.IsPlaying())
            {
                JetpackLoopSound.PlayAsBackgroundSFX();
                CurrentJetpackSound = JetpackLoopSound;
            }
        }

        public void PlayJetpackEndSound(JetpackEndSounds JetpackEndSound)
        {
            if (!CurrentJetpackSound.IsPlaying())
            {
                this.JetpackEndSound.PlayAsBackgroundSFX();
                CurrentJetpackSound = this.JetpackEndSound;
            }
        }

        public void PlayProneStartSound(ProneStartSounds ProneStartSound)
        {
            if (!CurrentProneSound.IsPlaying())
            {
                this.ProneStartSound.Play();
                CurrentProneSound = this.ProneStartSound;
            }
        }

        public void PlayProneMoveSound(ProneMoveSounds ProneMoveSound)
        {
            if (!CurrentProneSound.IsPlaying())
            {
                this.ProneMoveSound.Play();
                CurrentProneSound = this.ProneMoveSound;
            }
        }

        public void PlayProneEndSound(ProneEndSounds ProneEndSound)
        {
            if (!CurrentProneSound.IsPlaying())
            {
                this.ProneEndSound.Play();
                CurrentProneSound = this.ProneEndSound;
            }
        }

        public void PlayJumpStartSound(JumpStartSounds JumpStartSound)
        {
            this.JumpStartSound.Play();
        }

        public void PlayJumpEndSound(JumpEndSounds JumpEndSound)
        {
            this.JumpEndSound.Play();
        }

        public void PlayStrainSound(JumpStrainSounds JumpStrainSound)
        {
            if (JumpStrainSound == JumpStrainSounds.Male)
            {
                ArrayYellMaleSound[Random.Next(0, ArrayYellMaleSound.Length)].Play();
            }
            else if (JumpStrainSound == JumpStrainSounds.Female)
            {
                ArrayYellFemaleSound[Random.Next(0, ArrayYellFemaleSound.Length)].Play();
            }
        }

        public void PlayStepNormalSound(StepNormalSounds StepNormalSound)
        {
            if (!CurrentStepSound.IsPlaying())
            {
                CurrentStepSound = ArrayStepNormalSound[Random.Next(0, ArrayStepNormalSound.Length)];
                CurrentStepSound.Play();
            }
        }

        public void PlayStepGrassSound(StepGrassSounds StepGrassSound)
        {
            if (!CurrentStepSound.IsPlaying())
            {
                CurrentStepSound = ArrayStepGrassSound[Random.Next(0, ArrayStepGrassSound.Length)];
                CurrentStepSound.Play();
            }
        }

        public void PlayStepWaterSound(StepWaterSounds StepWaterSound)
        {
            if (!CurrentStepSound.IsPlaying())
            {
                CurrentStepSound = ArrayStepWaterSound[Random.Next(0, ArrayStepWaterSound.Length)];
                CurrentStepSound.Play();
            }
        }

        public void PlayDeathSound(DeathSounds DeathSound)
        {
            if (DeathSound == DeathSounds.Male)
            {
                ArrayDeathMaleSound[Random.Next(0, ArrayDeathMaleSound.Length)].Play();
            }
            else if (DeathSound == DeathSounds.Female)
            {
                ArrayDeathFemaleSound[Random.Next(0, ArrayDeathFemaleSound.Length)].Play();
            }
        }

        public void PlayRollSound(RollSounds RollSound)
        {
            this.RollSound.Play();
        }

        public void PlayDashSound(DashSounds DashSound)
        {
            this.DashSound.Play();
        }

        public void PlayGroundHitSound()
        {
            GroundHitSound.Play();
        }

        public void PlaySpawnSound()
        {
            SpawnSound.Play();
        }

        public void PlayBulletHitObjectSound()
        {
            if (!CurrentBulletHitObjectSound.IsPlaying())
            {
                CurrentBulletHitObjectSound = ArrayBulletHitObjectSound[Random.Next(0, ArrayBulletHitObjectSound.Length)];
                CurrentBulletHitObjectSound.Play();
            }
        }

        public void PlayGetHitSound()
        {
            if (!CurrentGetHitSound.IsPlaying())
            {
                CurrentGetHitSound = ArrayGetHitSound[Random.Next(0, ArrayGetHitSound.Length)];
                CurrentGetHitSound.Play();
            }
        }

        public void PlayLandSound()
        {
            ArrayLandSound[Random.Next(0, ArrayLandSound.Length)].Play();
        }

        public void PlayChangeGunInvalidSound()
        {
            ChangeGunInvalidSound.Play();
        }

        public void PlayChangeGunValidSound()
        {
            ChangeGunValidSound.Play();
        }

        public void PrepareExplosionSound(FMODSound sndExplosion, Vector2 ExplosionCenter)
        {
            ListExplosionSound.Add(new Tuple<Vector2, FMODSound>(ExplosionCenter, sndExplosion));
        }

        public void PlayExplosionSounds(List<Player> ListLocalPlayer)
        {
            foreach (Tuple<Vector2, FMODSound> ActiveExplosionSound in ListExplosionSound)
            {
                foreach (Player ActivePlayer in ListLocalPlayer)
                {
                    if (ActivePlayer.InGameRobot.Camera.Contains((int)ActiveExplosionSound.Item1.X, (int)ActiveExplosionSound.Item1.Y))
                    {
                        ActiveExplosionSound.Item2.Play();
                        break;
                    }
                }
            }

            ListExplosionSound.Clear();
        }
    }
}
