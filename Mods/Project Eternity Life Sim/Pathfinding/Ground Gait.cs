using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class GroundGait
    {
        public static void Apply(List<PathSegment> Segments, bool Hurried, TraversalCapabilities Body)
        {
            if (Segments == null || Body == null)
            {
                return;
            }

            for (int i = 0; i < Segments.Count; i++)
            {
                PathSegment Segment = Segments[i];
                bool OpenGround = Segment.Surface == NavArea.Ground || Segment.Surface == NavArea.Slope;
                if (Segment.Kind == Traversal.Walk && OpenGround && Hurried && Body.CanRun)
                {
                    Segment.Kind = Traversal.Run;
                    Segment.RequiredPose = TraversalPoses.For(Traversal.Run);
                }
            }

            for (int i = Segments.Count - 2; i >= 0; i--)
            {
                if (Segments[i].Kind != Traversal.Run || Segments[i + 1].Kind != Traversal.Crouch)
                {
                    continue;
                }

                if (Body.CanSlide)
                {
                    SplitEntry(Segments, i + 1, Traversal.Slide);
                }
                else
                {
                    SplitTail(Segments, i, Traversal.Crouch);
                }
            }
        }

        private static void SplitEntry(List<PathSegment> Segments, int Index, Traversal Kind)
        {
            PathSegment Source = Segments[Index];
            float Budget = NavMeshLimits.CellSize * 2f;
            List<Vector3> Prefix = new List<Vector3>();
            List<Vector3> Rest = new List<Vector3>();
            Prefix.Add(Source.Polyline[0]);
            float Covered = 0f;
            for (int i = 1; i < Source.Polyline.Count; i++)
            {
                Vector3 Previous = Source.Polyline[i - 1];
                Vector3 Point = Source.Polyline[i];
                float Edge = Vector3.Distance(Previous, Point);
                bool RoomAfter = i < Source.Polyline.Count - 1;
                if (RoomAfter && Covered + Edge >= Budget)
                {
                    float Need = Budget - Covered;
                    float Blend = Edge < 0.001f ? 1f : Need / Edge;
                    Vector3 At = Vector3.Lerp(Previous, Point, Blend);
                    Prefix.Add(At);
                    Rest.Add(At);
                    for (int j = i; j < Source.Polyline.Count; j++)
                    {
                        Rest.Add(Source.Polyline[j]);
                    }

                    break;
                }

                Prefix.Add(Point);
                Covered += Edge;
            }

            Source.Kind = Kind;
            Source.RequiredPose = TraversalPoses.For(Kind);
            if (Rest.Count < 2)
            {
                return;
            }

            Source.Polyline = Prefix;
            PathSegment Remain = new PathSegment();
            Remain.Polyline = Rest;
            Remain.Kind = Traversal.Crouch;
            Remain.RequiredPose = Pose.Crouch;
            Remain.Surface = Source.Surface;
            Segments.Insert(Index + 1, Remain);
        }

        private static void SplitTail(List<PathSegment> Segments, int Index, Traversal Kind)
        {
            PathSegment Source = Segments[Index];
            float Budget = NavMeshLimits.CellSize * 2f;
            float Total = 0f;
            for (int i = 1; i < Source.Polyline.Count; i++)
            {
                Total += Vector3.Distance(Source.Polyline[i - 1], Source.Polyline[i]);
            }

            if (Total <= Budget)
            {
                Source.Kind = Kind;
                Source.RequiredPose = TraversalPoses.For(Kind);
                return;
            }

            float Keep = Total - Budget;
            List<Vector3> Head = new List<Vector3>();
            List<Vector3> Tail = new List<Vector3>();
            Head.Add(Source.Polyline[0]);
            float Covered = 0f;
            for (int i = 1; i < Source.Polyline.Count; i++)
            {
                Vector3 Previous = Source.Polyline[i - 1];
                Vector3 Point = Source.Polyline[i];
                float Edge = Vector3.Distance(Previous, Point);
                if (Tail.Count == 0 && Covered + Edge >= Keep)
                {
                    float Need = Keep - Covered;
                    float Blend = Edge < 0.001f ? 1f : Need / Edge;
                    Vector3 At = Vector3.Lerp(Previous, Point, Blend);
                    Head.Add(At);
                    Tail.Add(At);
                    Tail.Add(Point);
                    for (int j = i + 1; j < Source.Polyline.Count; j++)
                    {
                        Tail.Add(Source.Polyline[j]);
                    }

                    break;
                }

                Head.Add(Point);
                Covered += Edge;
            }

            if (Head.Count < 2 || Tail.Count < 2)
            {
                Source.Kind = Kind;
                Source.RequiredPose = TraversalPoses.For(Kind);
                return;
            }

            Source.Polyline = Head;
            PathSegment Slow = new PathSegment();
            Slow.Polyline = Tail;
            Slow.Kind = Kind;
            Slow.RequiredPose = TraversalPoses.For(Kind);
            Slow.Surface = Source.Surface;
            Segments.Insert(Index + 1, Slow);
        }
    }
}
