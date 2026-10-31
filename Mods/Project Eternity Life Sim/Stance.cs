using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class StanceSequence
    {
        public static List<AutomatedAction> Splice(Pose Current, List<AutomatedAction> Steps)
        {
            List<AutomatedAction> Prepared = new List<AutomatedAction>();
            if (Steps == null)
            {
                return Prepared;
            }

            Pose PoseNow = Current;
            for (int i = 0; i < Steps.Count; i++)
            {
                AutomatedAction Step = Steps[i];
                if (Step.HitStaysInPose)
                {
                    Prepared.Add(Step);
                    continue;
                }

                if (PoseNow == Pose.Sit && Step.PoseNeeded == Pose.Stand)
                {
                    Prepared.Add(new StandUpAction(Step.Owner));
                    PoseNow = Pose.Stand;
                }
                else if (PoseNow == Pose.Stand && Step.PoseNeeded == Pose.Sit)
                {
                    Prepared.Add(new SitDownAction(Step.Owner));
                    PoseNow = Pose.Sit;
                }

                Prepared.Add(Step);
                PoseNow = Step.PoseLeft;
            }

            return Prepared;
        }
    }

    public class StandUpAction : AutomatedAction
    {
        private double Remaining;

        public StandUpAction(PlayerCharacter Owner)
            : base("Stand Up", Owner)
        {
        }

        public override Pose PoseNeeded => Pose.Sit;

        public override Pose PoseLeft => Pose.Stand;

        public override void OnStarted()
        {
            Remaining = 0.35;
        }

        public override bool Update(GameTime gameTime)
        {
            Remaining -= gameTime.ElapsedGameTime.TotalSeconds;
            if (Remaining > 0)
            {
                return true;
            }

            Owner.CurrentPose = Pose.Stand;
            Owner.PlayClip("Idle");
            return false;
        }

        public override void OnCancel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override AutomatedAction Copy()
        {
            return new StandUpAction(Owner);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override void DrawIcon(CustomSpriteBatch g, Vector2 Position)
        {
        }
    }

    public class SitDownAction : AutomatedAction
    {
        private double Remaining;

        public SitDownAction(PlayerCharacter Owner)
            : base("Sit Down", Owner)
        {
        }

        public override Pose PoseNeeded => Pose.Stand;

        public override Pose PoseLeft => Pose.Sit;

        public override void OnStarted()
        {
            Remaining = 0.35;
        }

        public override bool Update(GameTime gameTime)
        {
            Remaining -= gameTime.ElapsedGameTime.TotalSeconds;
            if (Remaining > 0)
            {
                return true;
            }

            Owner.CurrentPose = Pose.Sit;
            Owner.PlayClip("Idle");
            return false;
        }

        public override void OnCancel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override AutomatedAction Copy()
        {
            return new SitDownAction(Owner);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override void DrawIcon(CustomSpriteBatch g, Vector2 Position)
        {
        }
    }
}
