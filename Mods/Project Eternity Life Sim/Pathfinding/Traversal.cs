using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum Traversal
    {
        Walk,
        Run,
        Crouch,
        Crawl,
        Slide,
        Swim,
        Jump,
        Leap,
        LeapOver,
        JumpPad,
        Ladder,
        Rope,
        WallRun,
        WallClimb,
        LedgeClimb,
    }

    public enum Pose
    {
        Stand,
        Crouch,
        Crawl,
        Climb,
        MidAir,
        Sit,
        Incapacitated,
    }

    public static class TraversalPoses
    {
        public static Pose For(Traversal Kind)
        {
            if (Kind == Traversal.Crouch || Kind == Traversal.Slide)
            {
                return Pose.Crouch;
            }

            if (Kind == Traversal.Crawl)
            {
                return Pose.Crawl;
            }

            if (Kind == Traversal.Jump || Kind == Traversal.Leap || Kind == Traversal.LeapOver || Kind == Traversal.JumpPad)
            {
                return Pose.MidAir;
            }

            if (Kind == Traversal.Ladder || Kind == Traversal.Rope || Kind == Traversal.WallRun || Kind == Traversal.WallClimb || Kind == Traversal.LedgeClimb)
            {
                return Pose.Climb;
            }

            return Pose.Stand;
        }
    }

    public class PathSegment
    {
        public List<Vector3> Polyline;
        public Traversal Kind;
        public Pose RequiredPose;
        public NavArea Surface;

        public PathSegment()
        {
            Polyline = new List<Vector3>();
            Kind = Traversal.Walk;
            RequiredPose = Pose.Stand;
            Surface = NavArea.Ground;
        }
    }

    public class TraversalCapabilities
    {
        public bool CanWalk;
        public bool CanRun;
        public bool CanCrouch;
        public bool CanCrawl;
        public bool CanSlide;
        public bool CanSwim;
        public bool CanJump;
        public bool CanLeap;
        public bool CanLeapOver;
        public bool CanJumpPad;
        public bool CanLadder;
        public bool CanRope;
        public bool CanWallRun;
        public bool CanWallClimb;
        public bool CanLedgeClimb;

        public TraversalCapabilities()
        {
            CanWalk = true;
            CanRun = true;
            CanCrouch = true;
            CanCrawl = true;
            CanSlide = true;
        }

        public bool Allows(Traversal Kind)
        {
            switch (Kind)
            {
                case Traversal.Walk:
                    return CanWalk;
                case Traversal.Run:
                    return CanRun;
                case Traversal.Crouch:
                    return CanCrouch;
                case Traversal.Crawl:
                    return CanCrawl;
                case Traversal.Slide:
                    return CanSlide;
                case Traversal.Swim:
                    return CanSwim;
                case Traversal.Jump:
                    return CanJump;
                case Traversal.Leap:
                    return CanLeap;
                case Traversal.LeapOver:
                    return CanLeapOver;
                case Traversal.JumpPad:
                    return CanJumpPad;
                case Traversal.Ladder:
                    return CanLadder;
                case Traversal.Rope:
                    return CanRope;
                case Traversal.WallRun:
                    return CanWallRun;
                case Traversal.WallClimb:
                    return CanWallClimb;
                case Traversal.LedgeClimb:
                    return CanLedgeClimb;
                default:
                    return false;
            }
        }
    }
}
