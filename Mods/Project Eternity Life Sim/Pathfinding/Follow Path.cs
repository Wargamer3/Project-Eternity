using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class FollowPath : AutomatedAction
    {
        public List<PathSegment> Segments;
        public int SegmentIndex;
        public TraversalController Active;

        public FollowPath(PlayerCharacter Owner, List<PathSegment> Segments)
            : base("Follow Path", Owner)
        {
            this.Segments = Segments ?? new List<PathSegment>();
        }

        public override Pose PoseNeeded
        {
            get
            {
                if (Segments.Count == 0)
                {
                    return Pose.Stand;
                }

                return Segments[0].RequiredPose;
            }
        }

        public override Pose PoseLeft
        {
            get
            {
                if (Segments.Count == 0)
                {
                    return Pose.Stand;
                }

                return Segments[Segments.Count - 1].RequiredPose;
            }
        }

        public override void OnStarted()
        {
            SegmentIndex = 0;
            Active = null;
            if (Segments.Count > 0)
            {
                BeginSegment();
            }
        }

        public override bool Update(GameTime gameTime)
        {
            if (Segments.Count == 0 || SegmentIndex >= Segments.Count)
            {
                return false;
            }

            if (Active == null)
            {
                BeginSegment();
            }

            if (!Active.Tick(gameTime.ElapsedGameTime.TotalSeconds))
            {
                return true;
            }

            SegmentIndex++;
            if (SegmentIndex >= Segments.Count)
            {
                Active = null;
                if (Owner != null)
                {
                    Owner.PlayClip("Idle");
                }

                return false;
            }

            BeginSegment();
            return true;
        }

        public override void OnCancel()
        {
            Active = null;
            Owner.PlayClip("Idle");
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override AutomatedAction Copy()
        {
            return new FollowPath(Owner, Segments);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override void DrawIcon(CustomSpriteBatch g, Vector2 Position)
        {
        }

        private void BeginSegment()
        {
            Active = TraversalControllers.Create(Owner, Segments[SegmentIndex]);
        }
    }
}
