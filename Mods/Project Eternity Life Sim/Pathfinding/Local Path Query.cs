using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using DotRecast.Core.Numerics;
using DotRecast.Detour;
using DotRecast.Recast;
using DotRecast.Recast.Geom;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class LocalPathQuery
    {
        public static List<PathSegment> Find(LifeSimMap Chunk, Vector3 Start, Vector3 Goal, TraversalCapabilities Body, RouteStyle Style, bool PreferOptionalLinks = false, IList<NavCut> Cuts = null, IList<NavOffMeshLink> ExtraLinks = null, IList<FurnitureItem> Furniture = null, IList<BuildingItem> Buildings = null)
        {
            NavSourceGeometry Source = null;
            if (Chunk.Chunk != null && Chunk.Chunk.Heights != null)
            {
                Source = FromHeights(Chunk.Chunk);
            }

            if (Source == null || Source.Triangles.Count == 0)
            {
                Source = GridNavGeometry.Build(Chunk);
                if (Source.Triangles.Count == 0)
                {
                    Source = GridNavGeometry.Build(Chunk, true);
                }
            }

            NavSourceGeometry Open = WithoutSolids(Source, Furniture, Buildings);
            if (Chunk.Chunk != null)
            {
                ForestScatter.Collect(Chunk.Chunk, Open.Blockers);
                Hulls(Buildings, Open.Blockers);
            }

            return Find(WithLocalChanges(Open, Cuts, ExtraLinks), Start, Goal, Body, Style, PreferOptionalLinks);
        }

        public static List<PathSegment> Find(MapInfo Chunk, Vector3 Start, Vector3 Goal, TraversalCapabilities Body, RouteStyle Style, bool PreferOptionalLinks = false, IList<NavCut> Cuts = null, IList<NavOffMeshLink> ExtraLinks = null, IList<FurnitureItem> Furniture = null, IList<BuildingItem> Buildings = null)
        {
            if (Chunk != null && Chunk.Kind == GroundKind.Grid && Chunk.MapMapContainer != null && Chunk.MapMapContainer.ActiveMap is LifeSimMap Grid)
            {
                return Find(Grid, Start, Goal, Body, Style, PreferOptionalLinks, Cuts, ExtraLinks, Furniture ?? Grid.Furniture, Buildings ?? Grid.Buildings);
            }

            NavSourceGeometry Source = FromHeights(Chunk);
            if (Source == null || Source.Triangles.Count == 0)
            {
                return new List<PathSegment>();
            }

            IList<FurnitureItem> Props = Furniture ?? (Chunk == null ? null : Chunk.Furniture);
            IList<BuildingItem> Houses = Buildings ?? (Chunk == null ? null : Chunk.Buildings);
            NavSourceGeometry Open = WithoutSolids(Source, Props, Houses);
            if (Chunk != null)
            {
                ForestScatter.Collect(Chunk, Open.Blockers);
                Hulls(Houses, Open.Blockers);
            }

            return Find(WithLocalChanges(Open, Cuts, ExtraLinks), Start, Goal, Body, Style, PreferOptionalLinks);
        }

        static NavSourceGeometry FromHeights(MapInfo Chunk)
        {
            NavSourceGeometry Geometry = new NavSourceGeometry();
            HeightField Field = Chunk == null ? null : Chunk.Heights;
            if (Field == null || Field.Heights == null || Field.Spacing <= 0.01f || Field.SamplesX < 2 || Field.SamplesZ < 2)
            {
                return Geometry;
            }

            float Step = Field.Spacing;
            float SpanX = (Field.SamplesX - 1) * Step;
            float SpanY = (Field.SamplesZ - 1) * Step;
            for (float Y = 0f; Y < SpanY - 0.01f; Y += Step)
            {
                float Y1 = Math.Min(Y + Step, SpanY);
                for (float X = 0f; X < SpanX - 0.01f; X += Step)
                {
                    float X1 = Math.Min(X + Step, SpanX);
                    float A;
                    float B;
                    float C;
                    float D;
                    if (!Field.Sample(X, Y, out A, out _, out _) || !Field.Sample(X, Y1, out B, out _, out _) || !Field.Sample(X1, Y1, out C, out _, out _) || !Field.Sample(X1, Y, out D, out _, out _))
                    {
                        continue;
                    }

                    float Low = Math.Min(Math.Min(A, B), Math.Min(C, D));
                    float High = Math.Max(Math.Max(A, B), Math.Max(C, D));
                    float Run = Math.Max(X1 - X, Y1 - Y);
                    float Angle = (float)(Math.Atan2(High - Low, Math.Max(Run, 0.01f)) * (180.0 / Math.PI));
                    if (Angle > NavMeshLimits.MaxWalkableSlope)
                    {
                        continue;
                    }

                    float Mid = (A + B + C + D) * 0.25f;
                    if (Flooded(Chunk, (X + X1) * 0.5f, (Y + Y1) * 0.5f, Mid))
                    {
                        continue;
                    }

                    NavArea Area = Angle > 20f ? NavArea.Slope : NavArea.Ground;
                    Geometry.Triangles.Add(new NavTriangle(new Vector3(X, Y, A), new Vector3(X, Y1, B), new Vector3(X1, Y1, C), Area));
                    Geometry.Triangles.Add(new NavTriangle(new Vector3(X, Y, A), new Vector3(X1, Y1, C), new Vector3(X1, Y, D), Area));
                }
            }

            return Geometry;
        }

        static bool Flooded(MapInfo Chunk, float X, float Y, float Ground)
        {
            if (Chunk.Water == null)
            {
                return false;
            }

            for (int i = 0; i < Chunk.Water.Count; i++)
            {
                WaterVolume Volume = Chunk.Water[i];
                float Surface;
                Vector2 Flow;
                if (Volume != null && Volume.Footprint(X, Y, out Surface, out Flow) && Surface > Ground + 1.2f)
                {
                    return true;
                }
            }

            return false;
        }

        static NavSourceGeometry WithLocalChanges(NavSourceGeometry Source, IList<NavCut> Cuts, IList<NavOffMeshLink> ExtraLinks)
        {
            bool HasCuts = Cuts != null && Cuts.Count > 0;
            bool HasLinks = ExtraLinks != null && ExtraLinks.Count > 0;
            if (Source == null || (!HasCuts && !HasLinks))
            {
                return Source;
            }

            NavSourceGeometry Copy = new NavSourceGeometry();
            for (int i = 0; i < Source.Triangles.Count; i++)
            {
                NavTriangle Triangle = Source.Triangles[i];
                if (HasCuts && Covered(Triangle, Cuts))
                {
                    continue;
                }

                Copy.Triangles.Add(Triangle);
            }

            Copy.Links.AddRange(Source.Links);
            Copy.Blockers.AddRange(Source.Blockers);
            if (HasLinks)
            {
                Copy.Links.AddRange(ExtraLinks);
            }

            return Copy;
        }

        static bool Covered(NavTriangle Triangle, IList<NavCut> Cuts)
        {
            Vector3 Center = (Triangle.A + Triangle.B + Triangle.C) / 3f;
            for (int i = 0; i < Cuts.Count; i++)
            {
                NavCut Cut = Cuts[i];
                float Radius = Cut.Radius > 0f ? Cut.Radius : NavCut.DefaultRadius;
                float DX = Center.X - Cut.Center.X;
                float DY = Center.Y - Cut.Center.Y;
                if (DX * DX + DY * DY <= Radius * Radius)
                {
                    return true;
                }
            }

            return false;
        }

        static NavSourceGeometry WithoutSolids(NavSourceGeometry Source, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings)
        {
            bool HasFurniture = Furniture != null && Furniture.Count > 0;
            bool HasBuildings = Buildings != null && Buildings.Count > 0;
            if (Source == null || (!HasFurniture && !HasBuildings))
            {
                return Source;
            }

            NavSourceGeometry Open = new NavSourceGeometry();
            Open.Triangles.AddRange(Source.Triangles);
            Open.Links.AddRange(Source.Links);
            Open.Blockers.AddRange(Source.Blockers);
            if (Furniture != null)
            {
                for (int i = 0; i < Furniture.Count; i++)
                {
                    FurnitureItem Box = Furniture[i];
                    if (Box != null && Box.BlocksBody())
                    {
                        Open.Blockers.Add(Box.Body);
                    }
                }
            }

            if (Buildings != null)
            {
                for (int i = 0; i < Buildings.Count; i++)
                {
                    BuildingItem Box = Buildings[i];
                    if (Box != null && Box.BlocksBody())
                    {
                        Open.Blockers.Add(Box.Body);
                    }
                }
            }

            return Open;
        }

        static void Hulls(IList<BuildingItem> Buildings, List<BoxVolume> Solids)
        {
            if (Buildings == null || Solids == null)
            {
                return;
            }

            for (int i = 0; i < Buildings.Count; i++)
            {
                BuildingItem Box = Buildings[i];
                BoxVolume Hull;
                if (Box != null && Box.TryHull(out Hull))
                {
                    Solids.Add(Hull);
                }
            }
        }

        public static List<PathSegment> Find(NavSourceGeometry Source, Vector3 Start, Vector3 Goal, TraversalCapabilities Body, RouteStyle Style, bool PreferOptionalLinks = false)
        {
            ChunkNavMesh Mesh = ChunkNavMesh.Bake(Source);
            if (Mesh == null)
            {
                return new List<PathSegment>();
            }

            return Mesh.Find(Start, Goal, Body, Style, PreferOptionalLinks);
        }
    }

    public class ChunkNavMesh
    {
        private const int LinkAreaBase = 20;
        private const int RecastWalkableArea = 63;
        private const float SnapHorizontal = 12f;
        private const float SnapVertical = 24f;

        private readonly DtNavMesh NavMesh;
        private readonly DtNavMeshQuery Query;
        private readonly List<NavOffMeshLink> Links;

        private ChunkNavMesh(DtNavMesh NavMesh, List<NavOffMeshLink> Links)
        {
            this.NavMesh = NavMesh;
            this.Links = Links;
            Query = new DtNavMeshQuery(NavMesh);
        }

        public static ChunkNavMesh Bake(NavSourceGeometry Source)
        {
            if (Source == null || Source.Triangles.Count == 0)
            {
                return null;
            }

            float[] Vertices = new float[Source.Triangles.Count * 9];
            int[] Faces = new int[Source.Triangles.Count * 3];
            for (int i = 0; i < Source.Triangles.Count; i++)
            {
                NavTriangle Triangle = Source.Triangles[i];
                int First = i * 3;
                WriteVertex(Vertices, First, Triangle.A);
                WriteVertex(Vertices, First + 1, Triangle.B);
                WriteVertex(Vertices, First + 2, Triangle.C);
                Faces[i * 3] = First;
                Faces[i * 3 + 1] = First + 1;
                Faces[i * 3 + 2] = First + 2;
            }

            RcSampleInputGeomProvider Geom = new RcSampleInputGeomProvider(Vertices, Faces);
            MarkBlockers(Geom, Source.Blockers);
            for (int i = 0; i < Source.Triangles.Count; i++)
            {
                NavTriangle Triangle = Source.Triangles[i];
                if (Triangle.Area == NavArea.Ground)
                {
                    continue;
                }

                RcVec3f A = ToRecast(Triangle.A);
                RcVec3f B = ToRecast(Triangle.B);
                RcVec3f C = ToRecast(Triangle.C);
                float[] Shape = new float[] { A.X, A.Y, A.Z, B.X, B.Y, B.Z, C.X, C.Y, C.Z };
                float MinY = Math.Min(A.Y, Math.Min(B.Y, C.Y)) - NavMeshLimits.CellHeight;
                float MaxY = Math.Max(A.Y, Math.Max(B.Y, C.Y)) + NavMeshLimits.CellHeight;
                Geom.AddConvexVolume(Shape, MinY, MaxY, new RcAreaModification((int)Triangle.Area));
            }

            for (int i = 0; i < Source.Links.Count; i++)
            {
                NavOffMeshLink Link = Source.Links[i];
                int Area = LinkAreaBase + (int)Link.Kind;
                Geom.AddOffMeshConnection(ToRecast(Link.Start), ToRecast(Link.End), NavMeshLimits.AgentRadius, true, Area, FlagForArea(Area));
            }

            RcVec3f BoundsMin = Geom.GetMeshBoundsMin();
            RcVec3f BoundsMax = Geom.GetMeshBoundsMax();
            int Border = RcConfig.CalcBorder(NavMeshLimits.AgentRadius, NavMeshLimits.CellSize);
            int SpanX = (int)Math.Ceiling((BoundsMax.X - BoundsMin.X) / NavMeshLimits.CellSize);
            int SpanZ = (int)Math.Ceiling((BoundsMax.Z - BoundsMin.Z) / NavMeshLimits.CellSize);
            int TileSize = Math.Max(16, Math.Max(SpanX, SpanZ) + Border * 2 + 4);
            RcConfig Config = new RcConfig(
                true, TileSize, TileSize, Border, RcPartition.MONOTONE,
                NavMeshLimits.CellSize, NavMeshLimits.CellHeight,
                NavMeshLimits.MaxWalkableSlope, NavMeshLimits.CrawlHeight, NavMeshLimits.AgentRadius, NavMeshLimits.AgentMaxClimb,
                0f, 0f,
                NavMeshLimits.CellSize * 32f, 1.3f,
                6,
                6f, 1f,
                true, false, true,
                new RcAreaModification((int)NavArea.Ground),
                true);

            RcBuilder Builder = new RcBuilder();
            DtNavMesh NavMesh = new DtNavMesh();
            DtNavMeshParams MeshParams = new DtNavMeshParams();
            MeshParams.orig = BoundsMin;
            MeshParams.tileWidth = TileSize * NavMeshLimits.CellSize;
            MeshParams.tileHeight = TileSize * NavMeshLimits.CellSize;
            MeshParams.maxTiles = 32;
            MeshParams.maxPolys = 8192;
            NavMesh.Init(MeshParams, 6);

            bool AddedTile = false;
            foreach (RcBuilderResult Result in Builder.BuildTiles(Geom, Config, false, true, 1, Task.Factory))
            {
                if (Result.Mesh == null)
                {
                    continue;
                }

                DtMeshData Data = BuildTile(Geom, Result, NavMeshLimits.CellSize, NavMeshLimits.CellHeight);
                if (Data == null)
                {
                    continue;
                }

                DtStatus Added = NavMesh.AddTile(Data, 0, 0, out long _);
                if (Added.Succeeded())
                {
                    AddedTile = true;
                }
            }

            if (!AddedTile)
            {
                return null;
            }

            List<NavOffMeshLink> Links = new List<NavOffMeshLink>(Source.Links);
            return new ChunkNavMesh(NavMesh, Links);
        }

        public List<PathSegment> Find(Vector3 Start, Vector3 Goal, TraversalCapabilities Body, RouteStyle Style, bool PreferOptionalLinks = false)
        {
            // Covered and Flank need cover marks on the chunk. This mesh has none, so the short path is used.
            List<PathSegment> Segments = new List<PathSegment>();
            if (Body == null)
            {
                Body = new TraversalCapabilities();
            }

            DtQueryDefaultFilter Filter = new DtQueryDefaultFilter(IncludeFlags(Body), 0, AreaCosts(PreferOptionalLinks));
            Filter.SetIncludeFlags(IncludeFlags(Body));
            RcVec3f Extents = new RcVec3f(SnapHorizontal, SnapVertical, SnapHorizontal);
            DtStatus StartFound = Query.FindNearestPoly(ToRecast(Start), Extents, Filter, out long StartRef, out RcVec3f StartOnMesh, out bool _);
            DtStatus GoalFound = Query.FindNearestPoly(ToRecast(Goal), Extents, Filter, out long GoalRef, out RcVec3f GoalOnMesh, out bool _);
            if (StartFound.Failed() || GoalFound.Failed() || StartRef == 0 || GoalRef == 0)
            {
                return Segments;
            }

            long[] Path = new long[256];
            DtStatus PathStatus = Query.FindPath(StartRef, GoalRef, StartOnMesh, GoalOnMesh, Filter, Path, out int PathCount, Path.Length);
            if (PathStatus.Failed() || PathStatus.IsPartial() || PathCount == 0)
            {
                return Segments;
            }

            DtStraightPath[] Straight = new DtStraightPath[256];
            DtStatus StraightStatus = Query.FindStraightPath(StartOnMesh, GoalOnMesh, Path, PathCount, Straight, out int StraightCount, Straight.Length, DtStraightPathOptions.DT_STRAIGHTPATH_AREA_CROSSINGS);
            if (StraightStatus.Failed() || StraightCount < 2)
            {
                return Segments;
            }

            Vector3 LinkFrom;
            Vector3 LinkTo;
            Traversal LinkKind;
            int LinkEdge = -1;
            if (TryOffMesh(Path, PathCount, out LinkFrom, out LinkTo, out LinkKind))
            {
                float Best = float.MaxValue;
                for (int Edge = 0; Edge < StraightCount - 1; Edge++)
                {
                    Vector3 EdgeStart = ToGame(Straight[Edge].pos);
                    Vector3 EdgeEnd = ToGame(Straight[Edge + 1].pos);
                    float Forward = Vector3.Distance(EdgeStart, LinkFrom) + Vector3.Distance(EdgeEnd, LinkTo);
                    float Backward = Vector3.Distance(EdgeStart, LinkTo) + Vector3.Distance(EdgeEnd, LinkFrom);
                    float Score = Forward < Backward ? Forward : Backward;
                    if (Score < Best)
                    {
                        Best = Score;
                        LinkEdge = Edge;
                    }
                }

                if (Best > NavMeshLimits.AgentRadius * 8f)
                {
                    LinkEdge = -1;
                }
            }

            List<Vector3> Current = new List<Vector3>();
            Traversal CurrentKind = Traversal.Walk;
            NavArea CurrentSurface = NavArea.Ground;
            Pose CurrentPose = Pose.Stand;
            for (int i = 0; i < StraightCount - 1; i++)
            {
                Vector3 From = ToGame(Straight[i].pos);
                Vector3 To = ToGame(Straight[i + 1].pos);
                if (IsOffMesh(Straight[i]) || i == LinkEdge)
                {
                    Traversal ChosenLink = i == LinkEdge ? LinkKind : MatchLink(From, To);
                    AddSample(Segments, ref Current, ref CurrentKind, ref CurrentSurface, ref CurrentPose, From, ChosenLink, NavArea.Ground, TraversalPoses.For(ChosenLink));
                    AddSample(Segments, ref Current, ref CurrentKind, ref CurrentSurface, ref CurrentPose, To, ChosenLink, NavArea.Ground, TraversalPoses.For(ChosenLink));
                    continue;
                }

                float Length = Vector3.Distance(From, To);
                int Steps = (int)Math.Ceiling(Length / NavMeshLimits.CellSize);
                if (Steps < 1)
                {
                    Steps = 1;
                }

                for (int Step = 0; Step <= Steps; Step++)
                {
                    Vector3 Point = Vector3.Lerp(From, To, Step / (float)Steps);
                    Describe(AreaAt(ToRecast(Point), Filter), out Traversal Kind, out NavArea Surface);
                    AddSample(Segments, ref Current, ref CurrentKind, ref CurrentSurface, ref CurrentPose, Point, Kind, Surface, TraversalPoses.For(Kind));
                }
            }

            Flush(Segments, Current, CurrentKind, CurrentSurface, CurrentPose);
            return Segments;
        }

        private static DtMeshData BuildTile(RcSampleInputGeomProvider Geom, RcBuilderResult Result, float CellSize, float CellHeight)
        {
            RcPolyMesh PolyMesh = Result.Mesh;
            RcPolyMeshDetail DetailMesh = Result.MeshDetail;
            if (PolyMesh.flags != null)
            {
                for (int i = 0; i < PolyMesh.npolys; i++)
                {
                    PolyMesh.flags[i] = 1;
                }
            }

            DtNavMeshCreateParams Option = new DtNavMeshCreateParams();
            Option.verts = PolyMesh.verts;
            Option.vertCount = PolyMesh.nverts;
            Option.polys = PolyMesh.polys;
            Option.polyAreas = PolyMesh.areas;
            Option.polyFlags = PolyMesh.flags;
            Option.polyCount = PolyMesh.npolys;
            Option.nvp = PolyMesh.nvp;
            if (DetailMesh != null)
            {
                Option.detailMeshes = DetailMesh.meshes;
                Option.detailVerts = DetailMesh.verts;
                Option.detailVertsCount = DetailMesh.nverts;
                Option.detailTris = DetailMesh.tris;
                Option.detailTriCount = DetailMesh.ntris;
            }

            Option.walkableHeight = NavMeshLimits.CrawlHeight;
            Option.walkableRadius = NavMeshLimits.AgentRadius;
            Option.walkableClimb = NavMeshLimits.AgentMaxClimb;
            Option.bmin = PolyMesh.bmin;
            Option.bmax = PolyMesh.bmax;
            Option.cs = CellSize;
            Option.ch = CellHeight;
            Option.buildBvTree = true;
            Option.tileX = Result.TileX;
            Option.tileZ = Result.TileZ;

            List<RcOffMeshConnection> Connections = Geom.GetOffMeshConnections();
            Option.offMeshConCount = Connections.Count;
            Option.offMeshConVerts = new float[Connections.Count * 6];
            Option.offMeshConRad = new float[Connections.Count];
            Option.offMeshConDir = new int[Connections.Count];
            Option.offMeshConAreas = new int[Connections.Count];
            Option.offMeshConFlags = new int[Connections.Count];
            Option.offMeshConUserID = new int[Connections.Count];
            for (int i = 0; i < Connections.Count; i++)
            {
                RcOffMeshConnection Connection = Connections[i];
                for (int j = 0; j < 6; j++)
                {
                    Option.offMeshConVerts[i * 6 + j] = Connection.verts[j];
                }

                Option.offMeshConRad[i] = Connection.radius;
                Option.offMeshConDir[i] = Connection.bidir ? 1 : 0;
                Option.offMeshConAreas[i] = Connection.area;
                Option.offMeshConFlags[i] = Connection.flags;
                Option.offMeshConUserID[i] = i;
            }

            DtMeshData Data = DtNavMeshBuilder.CreateNavMeshData(Option);
            if (Data == null)
            {
                return null;
            }

            for (int i = 0; i < Data.polys.Length; i++)
            {
                DtPoly Poly = Data.polys[i];
                int Area = Poly.GetArea();
                if (Area == RecastWalkableArea)
                {
                    Poly.SetArea((int)NavArea.Ground);
                    Area = (int)NavArea.Ground;
                }

                Poly.flags = FlagForArea(Area);
            }

            return Data;
        }

        private int AreaAt(RcVec3f Point, DtQueryDefaultFilter Filter)
        {
            DtStatus Found = Query.FindNearestPoly(Point, new RcVec3f(2f, 8f, 2f), Filter, out long PolyRef, out RcVec3f _, out bool _);
            if (Found.Failed() || PolyRef == 0)
            {
                return (int)NavArea.Ground;
            }

            DtStatus Read = NavMesh.GetTileAndPolyByRef(PolyRef, out DtMeshTile _, out DtPoly Poly);
            if (Read.Failed() || Poly == null)
            {
                return (int)NavArea.Ground;
            }

            return Poly.GetArea();
        }

        private bool TryOffMesh(long[] Path, int PathCount, out Vector3 From, out Vector3 To, out Traversal Kind)
        {
            From = Vector3.Zero;
            To = Vector3.Zero;
            Kind = Traversal.Jump;
            for (int i = 0; i < PathCount; i++)
            {
                DtStatus Read = NavMesh.GetTileAndPolyByRef(Path[i], out DtMeshTile _, out DtPoly Poly);
                if (Read.Failed() || Poly == null || Poly.GetPolyType() != DtPolyTypes.DT_POLYTYPE_OFFMESH_CONNECTION)
                {
                    continue;
                }

                Describe(Poly.GetArea(), out Kind, out NavArea _);
                if (Kind < Traversal.Jump)
                {
                    Kind = Traversal.Jump;
                }

                if (i > 0)
                {
                    RcVec3f StartPos = new RcVec3f();
                    RcVec3f EndPos = new RcVec3f();
                    DtStatus Ends = NavMesh.GetOffMeshConnectionPolyEndPoints(Path[i - 1], Path[i], ref StartPos, ref EndPos);
                    if (Ends.Succeeded())
                    {
                        From = ToGame(StartPos);
                        To = ToGame(EndPos);
                        Traversal Matched = MatchLink(From, To);
                        if (Matched != Traversal.Jump || Kind == Traversal.Jump)
                        {
                            Kind = Matched;
                        }

                        return true;
                    }
                }

                for (int LinkIndex = 0; LinkIndex < Links.Count; LinkIndex++)
                {
                    if (Links[LinkIndex].Kind == Kind || Links.Count == 1)
                    {
                        From = Links[LinkIndex].Start;
                        To = Links[LinkIndex].End;
                        Kind = Links[LinkIndex].Kind;
                        return true;
                    }
                }
            }

            return false;
        }

        private Traversal MatchLink(Vector3 From, Vector3 To)
        {
            float Limit = NavMeshLimits.AgentRadius * 4f;
            for (int i = 0; i < Links.Count; i++)
            {
                NavOffMeshLink Link = Links[i];
                bool Forward = Vector3.Distance(Link.Start, From) <= Limit && Vector3.Distance(Link.End, To) <= Limit;
                bool Backward = Vector3.Distance(Link.Start, To) <= Limit && Vector3.Distance(Link.End, From) <= Limit;
                if (Forward || Backward)
                {
                    return Link.Kind;
                }
            }

            return Traversal.Jump;
        }

        private static void Describe(int AreaId, out Traversal Kind, out NavArea Surface)
        {
            if (AreaId == RecastWalkableArea)
            {
                AreaId = (int)NavArea.Ground;
            }

            if (AreaId >= LinkAreaBase)
            {
                Kind = (Traversal)(AreaId - LinkAreaBase);
                Surface = NavArea.Ground;
                return;
            }

            if (AreaId < (int)NavArea.Ground || AreaId > (int)NavArea.DeepWater)
            {
                Kind = Traversal.Walk;
                Surface = NavArea.Ground;
                return;
            }

            Surface = (NavArea)AreaId;
            if (Surface == NavArea.Crouch)
            {
                Kind = Traversal.Crouch;
            }
            else if (Surface == NavArea.Crawl)
            {
                Kind = Traversal.Crawl;
            }
            else if (Surface == NavArea.DeepWater)
            {
                Kind = Traversal.Swim;
            }
            else
            {
                Kind = Traversal.Walk;
            }
        }

        private static float[] AreaCosts(bool PreferOptionalLinks)
        {
            float[] Costs = new float[64];
            for (int i = 0; i < Costs.Length; i++)
            {
                Costs[i] = 1f;
            }

            if (PreferOptionalLinks)
            {
                return Costs;
            }

            // Optional links stay available, but a ground route wins unless the link is much shorter.
            Costs[LinkAreaBase + (int)Traversal.Leap] = 3f;
            Costs[LinkAreaBase + (int)Traversal.LeapOver] = 3f;
            Costs[LinkAreaBase + (int)Traversal.JumpPad] = 3f;
            Costs[LinkAreaBase + (int)Traversal.WallRun] = 3f;
            return Costs;
        }

        private static int IncludeFlags(TraversalCapabilities Body)
        {
            int Flags = 0;
            if (Body.CanWalk || Body.CanRun || Body.CanSlide)
            {
                Flags |= FlagForArea((int)NavArea.Ground);
                Flags |= FlagForArea((int)NavArea.Slope);
                Flags |= FlagForArea((int)NavArea.ShallowWater);
            }

            if (Body.CanCrouch)
            {
                Flags |= FlagForArea((int)NavArea.Crouch);
            }

            if (Body.CanCrawl)
            {
                Flags |= FlagForArea((int)NavArea.Crawl);
            }

            if (Body.CanSwim)
            {
                Flags |= FlagForArea((int)NavArea.DeepWater);
            }

            if (Body.CanJump)
            {
                Flags |= FlagForArea(LinkAreaBase + (int)Traversal.Jump);
            }

            if (Body.CanLeap)
            {
                Flags |= FlagForArea(LinkAreaBase + (int)Traversal.Leap);
            }

            if (Body.CanLeapOver)
            {
                Flags |= FlagForArea(LinkAreaBase + (int)Traversal.LeapOver);
            }

            if (Body.CanJumpPad)
            {
                Flags |= FlagForArea(LinkAreaBase + (int)Traversal.JumpPad);
            }

            if (Body.CanLadder)
            {
                Flags |= FlagForArea(LinkAreaBase + (int)Traversal.Ladder);
            }

            if (Body.CanRope)
            {
                Flags |= FlagForArea(LinkAreaBase + (int)Traversal.Rope);
            }

            if (Body.CanWallRun)
            {
                Flags |= FlagForArea(LinkAreaBase + (int)Traversal.WallRun);
            }

            if (Body.CanWallClimb)
            {
                Flags |= FlagForArea(LinkAreaBase + (int)Traversal.WallClimb);
            }

            if (Body.CanLedgeClimb)
            {
                Flags |= FlagForArea(LinkAreaBase + (int)Traversal.LedgeClimb);
            }

            return Flags;
        }

        private static int FlagForArea(int AreaId)
        {
            if (AreaId == RecastWalkableArea)
            {
                AreaId = (int)NavArea.Ground;
            }

            if (AreaId >= (int)NavArea.Ground && AreaId <= (int)NavArea.DeepWater)
            {
                return 1 << AreaId;
            }

            if (AreaId >= LinkAreaBase)
            {
                int LinkIndex = AreaId - LinkAreaBase - (int)Traversal.Jump;
                if (LinkIndex >= 0 && LinkIndex < 9)
                {
                    return 1 << (7 + LinkIndex);
                }
            }

            return 0;
        }

        private static void AddSample(List<PathSegment> Segments, ref List<Vector3> Current, ref Traversal CurrentKind, ref NavArea CurrentSurface, ref Pose CurrentPose, Vector3 Point, Traversal Kind, NavArea Surface, Pose RequiredPose)
        {
            if (Current.Count > 0 && (Kind != CurrentKind || Surface != CurrentSurface))
            {
                Current.Add(Point);
                Flush(Segments, Current, CurrentKind, CurrentSurface, CurrentPose);
                Current = new List<Vector3>();
            }

            if (Current.Count > 0 && Vector3.Distance(Current[Current.Count - 1], Point) < 0.01f)
            {
                return;
            }

            CurrentKind = Kind;
            CurrentSurface = Surface;
            CurrentPose = RequiredPose;
            Current.Add(Point);
        }

        private static void Flush(List<PathSegment> Segments, List<Vector3> Current, Traversal Kind, NavArea Surface, Pose RequiredPose)
        {
            if (Current.Count < 2)
            {
                return;
            }

            PathSegment Segment = new PathSegment();
            Segment.Polyline = Current;
            Segment.Kind = Kind;
            Segment.Surface = Surface;
            Segment.RequiredPose = RequiredPose;
            Segments.Add(Segment);
        }

        private static bool IsOffMesh(DtStraightPath Corner)
        {
            return (Corner.flags & DtStraightPathFlags.DT_STRAIGHTPATH_OFFMESH_CONNECTION) != 0;
        }

        private static void MarkBlockers(RcSampleInputGeomProvider Geom, List<BoxVolume> Blockers)
        {
            if (Geom == null || Blockers == null)
            {
                return;
            }

            for (int i = 0; i < Blockers.Count; i++)
            {
                BoxVolume Box = Blockers[i];
                float HalfW = Box.Width * 0.5f + NavMeshLimits.CellSize;
                float HalfL = Box.Length * 0.5f + NavMeshLimits.CellSize;
                float C = (float)Math.Cos(Box.Angle);
                float S = (float)Math.Sin(Box.Angle);
                float[] Shape = new float[12];
                WriteCorner(Shape, 0, Box, -HalfW, -HalfL, C, S);
                WriteCorner(Shape, 3, Box, HalfW, -HalfL, C, S);
                WriteCorner(Shape, 6, Box, HalfW, HalfL, C, S);
                WriteCorner(Shape, 9, Box, -HalfW, HalfL, C, S);
                float Bottom = Box.Position.Z - NavMeshLimits.CellHeight;
                float Top = Box.Position.Z + Math.Max(Box.Height, NavMeshLimits.CellHeight) + NavMeshLimits.CellHeight;
                Geom.AddConvexVolume(Shape, Bottom, Top, new RcAreaModification(0));
            }
        }

        private static void WriteCorner(float[] Shape, int Offset, BoxVolume Box, float Along, float Across, float C, float S)
        {
            RcVec3f Point = ToRecast(new Vector3(Box.Position.X + Along * C - Across * S, Box.Position.Y + Along * S + Across * C, Box.Position.Z));
            Shape[Offset] = Point.X;
            Shape[Offset + 1] = Point.Y;
            Shape[Offset + 2] = Point.Z;
        }

        private static void WriteVertex(float[] Vertices, int VertexIndex, Vector3 Game)
        {
            RcVec3f Recast = ToRecast(Game);
            int Offset = VertexIndex * 3;
            Vertices[Offset] = Recast.X;
            Vertices[Offset + 1] = Recast.Y;
            Vertices[Offset + 2] = Recast.Z;
        }

        private static RcVec3f ToRecast(Vector3 Game)
        {
            return new RcVec3f(Game.X, Game.Z, Game.Y);
        }

        private static Vector3 ToGame(RcVec3f Recast)
        {
            return new Vector3(Recast.X, Recast.Z, Recast.Y);
        }
    }
}
