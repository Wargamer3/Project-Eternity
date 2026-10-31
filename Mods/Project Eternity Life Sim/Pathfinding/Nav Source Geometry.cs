using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum NavArea
    {
        Ground = 1,
        Slope = 2,
        Crouch = 3,
        Crawl = 4,
        ShallowWater = 5,
        DeepWater = 6,
    }

    public static class NavMeshLimits
    {
        public const float MaxWalkableSlope = 45f;
        public const float AgentMaxClimb = 16f;
        public const float StandingHeight = 48f;
        public const float CrouchHeight = 28f;
        public const float CrawlHeight = 12f;
        public const float AgentRadius = 4f;
        public const float CellSize = 4f;
        public const float CellHeight = 2f;
    }

    public class NavTriangle
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;
        public NavArea Area;

        public NavTriangle(Vector3 A, Vector3 B, Vector3 C, NavArea Area)
        {
            this.A = A;
            this.B = B;
            this.C = C;
            this.Area = Area;
        }
    }

    public class NavCut
    {
        public const float DefaultRadius = 12f;

        public Vector3 Center;
        public float Radius;
    }

    public class NavOffMeshLink
    {
        public Traversal Kind;
        public Vector3 Start;
        public Vector3 End;

        public NavOffMeshLink(Traversal Kind, Vector3 Start, Vector3 End)
        {
            this.Kind = Kind;
            this.Start = Start;
            this.End = End;
        }
    }

    public class NavSourceGeometry
    {
        public List<NavTriangle> Triangles;
        public List<NavOffMeshLink> Links;
        public List<BoxVolume> Blockers;

        public NavSourceGeometry()
        {
            Triangles = new List<NavTriangle>();
            Links = new List<NavOffMeshLink>();
            Blockers = new List<BoxVolume>();
        }
    }
}
