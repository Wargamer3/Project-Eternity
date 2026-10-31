using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class TraversalController
    {
        public readonly Traversal Kind;
        protected readonly PlayerCharacter Owner;
        protected readonly PathSegment Segment;

        protected TraversalController(PlayerCharacter Owner, PathSegment Segment)
        {
            this.Owner = Owner;
            this.Segment = Segment;
            Kind = Segment.Kind;
            Owner.CurrentPose = Segment.RequiredPose;
            Owner.PlayClip(ClipName(Kind));
        }

        public abstract bool Tick(double DeltaTime);

        public static string ClipName(Traversal Kind)
        {
            if (Kind == Traversal.Walk)
            {
                return "Walking";
            }

            return Kind.ToString();
        }
    }

    public static class TraversalControllers
    {
        public static TraversalController Create(PlayerCharacter Owner, PathSegment Segment)
        {
            switch (Segment.Kind)
            {
                case Traversal.Walk:
                case Traversal.Run:
                case Traversal.Crouch:
                case Traversal.Crawl:
                case Traversal.Slide:
                case Traversal.Swim:
                    return new GroundController(Owner, Segment);
                case Traversal.Jump:
                    return new JumpController(Owner, Segment);
                case Traversal.Leap:
                    return new LeapController(Owner, Segment);
                case Traversal.LeapOver:
                    return new LeapOverController(Owner, Segment);
                case Traversal.JumpPad:
                    return new JumpPadController(Owner, Segment);
                case Traversal.Ladder:
                    return new LadderController(Owner, Segment);
                case Traversal.Rope:
                    return new RopeController(Owner, Segment);
                case Traversal.WallRun:
                    return new WallRunController(Owner, Segment);
                case Traversal.WallClimb:
                    return new WallClimbController(Owner, Segment);
                case Traversal.LedgeClimb:
                    return new LedgeClimbController(Owner, Segment);
                default:
                    return new GroundController(Owner, Segment);
            }
        }
    }

    public class GroundController : TraversalController
    {
        private int Waypoint;

        public GroundController(PlayerCharacter Owner, PathSegment Segment)
            : base(Owner, Segment)
        {
        }

        public override bool Tick(double DeltaTime)
        {
            SkipReachedPoints();
            if (Waypoint >= Segment.Polyline.Count)
            {
                return true;
            }

            Vector3 Target = Segment.Polyline[Waypoint];
            Vector3 Offset = Target - Owner.Position;
            Vector3 Flat = new Vector3(Offset.X, Offset.Y, 0f);
            float Strength = StrengthFor(Segment);
            if (Flat.Length() > 0.001f)
            {
                Vector3 Before = Owner.Position;
                Owner.Move(Vector3.Normalize(Flat), Strength, DeltaTime);
                Vector3 Moved = Owner.Position - Before;
                Moved.Z = 0f;
                if (Moved.LengthSquared() > 0.0001f)
                {
                    Owner.PlayClip(ClipName(Kind));
                }
                else
                {
                    Owner.PlayClip("Idle");
                }
            }
            else
            {
                Owner.PlayClip("Idle");
            }

            float Rise = Target.Z - Owner.Position.Z;
            float MaxRise = Owner.BaseMoveSpeed * Strength * (float)DeltaTime;
            if (Math.Abs(Rise) > MaxRise)
            {
                Rise = Math.Sign(Rise) * MaxRise;
            }

            if (Math.Abs(Rise) > 0.001f)
            {
                Owner.StepBody(new Vector3(0f, 0f, Rise));
            }

            SkipReachedPoints();
            return Waypoint >= Segment.Polyline.Count;
        }

        private void SkipReachedPoints()
        {
            while (Waypoint < Segment.Polyline.Count)
            {
                Vector3 Offset = Segment.Polyline[Waypoint] - Owner.Position;
                float Flat = new Vector3(Offset.X, Offset.Y, 0f).Length();
                float Reach = Waypoint == Segment.Polyline.Count - 1 ? 1f : NavMeshLimits.AgentRadius;
                if (Flat > Reach || Math.Abs(Offset.Z) > Reach)
                {
                    return;
                }

                Waypoint++;
            }
        }

        private float StrengthFor(PathSegment Segment)
        {
            float Strength = 1f;
            switch (Segment.Kind)
            {
                case Traversal.Run:
                    Strength = 1.75f;
                    break;
                case Traversal.Crouch:
                    Strength = 0.5f;
                    break;
                case Traversal.Crawl:
                    Strength = 0.35f;
                    break;
                case Traversal.Slide:
                    Strength = 2f;
                    break;
                case Traversal.Swim:
                    Strength = 0.65f * Owner.SwimPace;
                    break;
            }

            if (Segment.Surface == NavArea.ShallowWater)
            {
                Strength *= 0.6f;
            }

            return Strength;
        }
    }

    public class LinkController : TraversalController
    {
        private readonly float Speed;
        private readonly float Arc;
        private readonly float Length;
        private float Travelled;

        protected LinkController(PlayerCharacter Owner, PathSegment Segment, float Speed, float Arc)
            : base(Owner, Segment)
        {
            this.Speed = Speed;
            this.Arc = Arc;
            Length = Measure(Segment.Polyline);
        }

        public override bool Tick(double DeltaTime)
        {
            if (Segment.Polyline.Count == 0)
            {
                return true;
            }

            Vector3 End = Segment.Polyline[Segment.Polyline.Count - 1];
            if (Length < 0.001f)
            {
                Owner.StepBody(End - Owner.Position);
                return true;
            }

            Travelled += Speed * (float)DeltaTime;
            if (Travelled >= Length)
            {
                Owner.StepBody(End - Owner.Position);
                return true;
            }

            float Along = Travelled / Length;
            Vector3 At = PointAt(Travelled);
            At.Z += (float)Math.Sin(Along * Math.PI) * Arc;
            Owner.StepBody(At - Owner.Position);
            return false;
        }

        private Vector3 PointAt(float Distance)
        {
            float Left = Distance;
            for (int i = 1; i < Segment.Polyline.Count; i++)
            {
                Vector3 Previous = Segment.Polyline[i - 1];
                Vector3 Point = Segment.Polyline[i];
                float Edge = Vector3.Distance(Previous, Point);
                if (Left <= Edge || i == Segment.Polyline.Count - 1)
                {
                    float Blend = Edge < 0.001f ? 1f : Left / Edge;
                    if (Blend > 1f)
                    {
                        Blend = 1f;
                    }

                    return Vector3.Lerp(Previous, Point, Blend);
                }

                Left -= Edge;
            }

            return Segment.Polyline[Segment.Polyline.Count - 1];
        }

        private static float Measure(List<Vector3> Polyline)
        {
            float Total = 0f;
            for (int i = 1; i < Polyline.Count; i++)
            {
                Total += Vector3.Distance(Polyline[i - 1], Polyline[i]);
            }

            return Total;
        }
    }

    public class JumpController : LinkController
    {
        public JumpController(PlayerCharacter Owner, PathSegment Segment)
            : base(Owner, Segment, 40f, 16f)
        {
        }
    }

    public class LeapController : LinkController
    {
        public LeapController(PlayerCharacter Owner, PathSegment Segment)
            : base(Owner, Segment, 55f, 28f)
        {
        }
    }

    public class LeapOverController : LinkController
    {
        public LeapOverController(PlayerCharacter Owner, PathSegment Segment)
            : base(Owner, Segment, 48f, 12f)
        {
        }
    }

    public class JumpPadController : LinkController
    {
        public JumpPadController(PlayerCharacter Owner, PathSegment Segment)
            : base(Owner, Segment, 70f, 40f)
        {
        }
    }

    public class LadderController : LinkController
    {
        public LadderController(PlayerCharacter Owner, PathSegment Segment)
            : base(Owner, Segment, 12f, 0f)
        {
        }
    }

    public class RopeController : LinkController
    {
        public RopeController(PlayerCharacter Owner, PathSegment Segment)
            : base(Owner, Segment, 10f, 0f)
        {
        }
    }

    public class WallRunController : LinkController
    {
        public WallRunController(PlayerCharacter Owner, PathSegment Segment)
            : base(Owner, Segment, 36f * Owner.WallRunPace, 0f)
        {
        }
    }

    public class WallClimbController : LinkController
    {
        public WallClimbController(PlayerCharacter Owner, PathSegment Segment)
            : base(Owner, Segment, 10f, 0f)
        {
        }
    }

    public class LedgeClimbController : LinkController
    {
        public LedgeClimbController(PlayerCharacter Owner, PathSegment Segment)
            : base(Owner, Segment, 14f, 8f)
        {
        }
    }
}
