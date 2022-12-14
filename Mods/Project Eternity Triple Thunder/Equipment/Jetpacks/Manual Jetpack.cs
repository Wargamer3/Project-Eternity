using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class ManualJetpack : JetpackBase
    {
        private ISFXGenerator JetpackSFXGenerator;
        private bool UsedLastUpdate;
        private readonly Random Random;

        public ManualJetpack(ISFXGenerator JetpackSFXGenerator, RobotAnimation Owner)
            : base(Owner, 100)
        {
            this.JetpackSFXGenerator = JetpackSFXGenerator;

            JetpackFuel = JetpackFuelMax;
            UsedLastUpdate = false;
            Random = new Random();
        }

        public override void OnJetpackUse(GameTime gameTime)
        {
            if (JetpackFuel > 0)
            {
                if (JetpackFuel % 2 == 0)
                {
                    float ExtraSpeed = (float)Random.NextDouble() - 0.5f;
                    Propulsor.ParticleSystem.AddParticle(Owner.Position, new Vector2(ExtraSpeed, 0.2f));
                    Owner.SendOnlineVFX(Owner.Position, new Vector2(ExtraSpeed, 0.2f), Online.CreateVFXScriptClient.VFXTypes.Jetpack);
                }

                if (Owner.IsOnGround && Owner.Sounds.JetpackStartSound != UnitSounds.JetpackStartSounds.None)
                {
                    JetpackSFXGenerator.PlayJetpackStartSound(Owner.Sounds.JetpackStartSound);
                    Owner.SendOnlineSFX(Owner.Position, Online.CreateSFXScriptClient.SFXTypes.JetpackLoop);
                }
                else if (Owner.Sounds.JetpackUseSound != UnitSounds.JetpackUseSounds.None)
                {
                    JetpackSFXGenerator.PlayJetpackLoopSound(Owner.Sounds.JetpackUseSound);
                    Owner.SendOnlineSFX(Owner.Position, Online.CreateSFXScriptClient.SFXTypes.JetpackStart);
                }

                UsedLastUpdate = true;
                JetpackFuel--;

                if (JetpackFuel <= 0)
                {
                    if (Owner.Sounds.JetpackEndSound != UnitSounds.JetpackEndSounds.None)
                    {
                        JetpackSFXGenerator.PlayJetpackEndSound(Owner.Sounds.JetpackEndSound);
                        Owner.SendOnlineSFX(Owner.Position, Online.CreateSFXScriptClient.SFXTypes.JetpackEnd);
                    }

                    UsedLastUpdate = false;
                }

                if (Owner.Speed.Y > -MaxJetpackTrust)
                {
                    Owner.Speed -= new Vector2(0, JetpackTrust);
                }
            }
        }

        public override void OnJetpackRest(GameTime gameTime)
        {
            if (UsedLastUpdate && Owner.Sounds.JetpackEndSound != UnitSounds.JetpackEndSounds.None)
            {
                JetpackSFXGenerator.PlayJetpackEndSound(Owner.Sounds.JetpackEndSound);
                Owner.SendOnlineSFX(Owner.Position, Online.CreateSFXScriptClient.SFXTypes.JetpackEnd);
            }

            if (JetpackFuel < JetpackFuelMax)
            {
                JetpackFuel++;
            }

            UsedLastUpdate = false;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnJump()
        {
        }

        public override void OnStopJump()
        {
        }

        public override void OnLand()
        {
        }

        public override void OnFall()
        {
        }

        public override void Move(MovementInputs MovementInput)
        {
        }

        public override void OnIdle()
        {
        }

        public override void OnAnyCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListAllCollidingPolygon)
        {
        }

        public override void OnFloorCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListFloorCollidingPolygon)
        {
        }

        public override void OnCeilingCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListCeilingCollidingPolygon)
        {
        }

        public override void OnWallCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListWallCollidingPolygon)
        {
        }
    }
}
