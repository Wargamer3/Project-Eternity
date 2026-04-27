using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.UnitTests.AI
{
    public class NavMap
    {
        private Pathfinder Pathfinder;
        private InterMapMovementAlgorithm InterMapPathfinder;
        public MapInfo RootMapContainer;

        public NavMap()
        {
            InterMapPathfinder = new InterMapMovementAlgorithm();
            RootMapContainer = new MapInfo(new MapContainer("Root", null), new Vector3[0], new Vector3[0]);
        }

        public List<Action> FindPath(MapInfo StartMap, Vector3 StartPosition, string Destination)
        {
            List<Vector3> ListDestination = new List<Vector3>();
            MapInfo IntersectionPoint;

            List<MapInfo> FinalPath = FindLogicRoute(StartMap, Destination, out IntersectionPoint);

            MapInfo LastMapPoint = FinalPath[FinalPath.Count - 1];
            Vector3 LastPosition = LastMapPoint.ArrayExitPoint[0];
            float LastDistance = float.MaxValue;

            for (int i = FinalPath.Count - 2; i > 0; --i)
            {
                MapInfo ActiveMapPath = FinalPath[i];

                for (int j = 0; j < ActiveMapPath.ArrayExitPoint.Length; ++j)
                {
                    float CurrentDistance = (LastPosition - ActiveMapPath.ArrayEntryPointIntoOtherMap[j]).Length();

                    if (CurrentDistance < LastDistance)
                    {
                        LastDistance = CurrentDistance;
                        LastPosition = ActiveMapPath.ArrayExitPoint[j];
                    }
                }


                ListDestination.Add(LastPosition);
            }

            return null;
        }

        private List<MapInfo> FindLogicRoute(MapInfo StartMap, string Destination, out MapInfo IntersectionPoint)
        {
            List<MapInfo> ListFoundPathToRoot = RouteToRoot(StartMap);
            List<MapInfo> ListFoundPathToDestination = InterMapPathfinder.FindPath(RootMapContainer, Destination);

            List<MapInfo> FinalPath = new List<MapInfo>();

            foreach (MapInfo ActiveNode in ListFoundPathToRoot)
            {
                FinalPath.Add(ActiveNode);

                List<MapInfo> ListAllNeighbour = FindAllNeighbour(ActiveNode);
                foreach (MapInfo ActiveNeighbour in ListAllNeighbour)
                {
                    if (ListFoundPathToDestination.Contains(ActiveNeighbour))
                    {
                        int FoundCorrespondence = ListFoundPathToDestination.IndexOf(ActiveNeighbour);

                        for (int i = FoundCorrespondence; i < ListFoundPathToDestination.Count; ++i)
                        {
                            FinalPath.Add(ListFoundPathToDestination[i]);
                        }

                        IntersectionPoint = ActiveNode;

                        return FinalPath;
                    }
                }
            }

            IntersectionPoint = RootMapContainer;
            FinalPath.AddRange(ListFoundPathToDestination);
            return FinalPath;
        }

        private List<MapInfo> FindAllNeighbour(MapInfo StartNode)
        {
            MapInfo ActiveNode = StartNode;
            List<MapInfo> ListRemainingNode = new List<MapInfo>();
            List<MapInfo> ListAllNode = new List<MapInfo>();

            ListRemainingNode.Add(ActiveNode);
            ListRemainingNode.AddRange(StartNode.DicNeighbourByName.Values);

            while (ActiveNode != null)
            {
                ListAllNode.Add(ActiveNode);

                foreach (MapInfo ActiveNeighbour in StartNode.DicNeighbourByName.Values)
                {
                    if (!ListRemainingNode.Contains(ActiveNeighbour))
                    {
                        ListRemainingNode.Add(ActiveNeighbour);
                    }
                }

                ListRemainingNode.RemoveAt(0);

                if (ListRemainingNode.Count > 0)
                {
                    ActiveNode = ListRemainingNode[0];
                }
                else
                {
                    ActiveNode = null;
                }
            }

            return ListAllNode;
        }

        public List<Action> FindPath(Character ActiveCharacter, Vector3 Start, Vector3 End)
        {
            return Pathfinder.FindPath(ActiveCharacter, Start, End);
        }

        public void AddMapContainer(MapInfo ItemToAdd)
        {
            RootMapContainer.AddNestedMap(ItemToAdd);
        }

        private List<MapInfo> RouteToRoot(MapInfo StartMap)
        {
            List<MapInfo> ListFoundPathToRoot = new List<MapInfo>();

            MapInfo StartNode = StartMap;

            while (StartNode != null)
            {
                ListFoundPathToRoot.Add(StartNode);

                StartNode = StartNode.HierarchicParent;
            }

            return ListFoundPathToRoot;
        }
    }

    public struct Door
    {
        public bool IsClosed;
        public int TimeToOpen;
        public int TimeToClose;
        public bool CanBeLocked;
        public bool IsLocked;
        public bool RequireKeyName;
        public int TimeToUnlock;
        public int TimeToLock;
        public int HP;

        public Vector3 Position;

        public Door(bool IsClosed, int TimeToOpen, int TimeToClose, bool CanBeLocked, bool IsLocked, bool RequireKeyName, int TimeToUnlock, int TimeToLock, int HP, Vector3 Position)
        {
            this.IsClosed = IsClosed;
            this.TimeToOpen = TimeToOpen;
            this.TimeToClose = TimeToClose;
            this.CanBeLocked = CanBeLocked;
            this.IsLocked = IsLocked;
            this.RequireKeyName = RequireKeyName;
            this.TimeToUnlock = TimeToUnlock;
            this.TimeToLock = TimeToLock;
            this.HP = HP;
            this.Position = Position;
        }

        public static Door CreateDefaultDoor(Vector3 Position)
        {
            return new Door(true, 1, 1, false, false, false, 0, 0, 10, Position);
        }
    }

    public class Window
    {
        public bool IsClosed;
        public int TimeToOpen;
        public int TimeToClose;
        public bool CanBeLocked;
        public bool IsLocked;
        public bool RequireKeyName;
        public int TimeToUnlock;
        public int TimeToLock;
        public int HP;

        public Vector3 Position;
    }

    public class Building
    {
        public Window[] ArrayWindow;
        public Door[] ArrayDoor;
        public PolygonMesh[] ArrayCollision;
    }

    public class Road
    {
        public string RoadName;
        public List<AITunnel> ListAITunnel;
    }

    /// <summary>
    /// Contains unique information for each character such as knowledge of where a building is
    /// </summary>
    public class MapInfo
    {
        public Vector3[] ArrayExitPoint;
        public Vector3[] ArrayEntryPointIntoOtherMap;
        public MapContainer MapMapContainer;

        public float MovementCost;
        public MapInfo ParentTemp;
        public MapInfo HierarchicParent;

        internal List<MapInfo> ListConnectedMapByName;
        internal Dictionary<string, MapInfo> DicNeighbourByName;
        internal Dictionary<string, MapInfo> DicNestedMapByName;

        public MapInfo(MapContainer MapMapContainer, Vector3[] ArrayExitPoint, Vector3[] ArrayEntryPointIntoOtherMap)
        {
            this.ArrayExitPoint = ArrayExitPoint;
            this.ArrayEntryPointIntoOtherMap = ArrayEntryPointIntoOtherMap;
            this.MapMapContainer = MapMapContainer;

            MovementCost = 1;
            ParentTemp = null;
            HierarchicParent = null;

            ListConnectedMapByName = new List<MapInfo>();
            DicNeighbourByName = new Dictionary<string, MapInfo>();
            DicNestedMapByName = new Dictionary<string, MapInfo>();
        }

        public void AddNeighbour(MapInfo MapToAdd)
        {
            if (!DicNeighbourByName.ContainsKey(MapToAdd.MapMapContainer.AreaName))
            {
                ListConnectedMapByName.Add(MapToAdd);
                DicNeighbourByName.Add(MapToAdd.MapMapContainer.AreaName, MapToAdd);
            }
        }

        public void AddNestedMap(MapInfo MapToAdd)
        {
            MapToAdd.HierarchicParent = this;

            if (!DicNestedMapByName.ContainsKey(MapToAdd.MapMapContainer.AreaName))
            {
                ListConnectedMapByName.Add(MapToAdd);
                DicNestedMapByName.Add(MapToAdd.MapMapContainer.AreaName, MapToAdd);
            }
        }

        public MapInfo FindOwnerMap(string LocationName)
        {
            if (DicNestedMapByName.ContainsKey(LocationName))
            {
                return this;
            }

            foreach (MapInfo ActiveNestedMap in DicNestedMapByName.Values)
            {
                MapInfo FoundNestedMap = ActiveNestedMap.FindOwnerMap(LocationName);

                if (FoundNestedMap != null)
                {
                    return FoundNestedMap;
                }
            }

            return null;
        }

        public override string ToString()
        {
            return MapMapContainer.ToString();
        }
    }

    /// <summary>
    /// Contains reusable information
    /// </summary>
    public class MapContainer
    {
        public string AreaName;
        public string[] ArrayTag;

        private Dictionary<string, Road> DicRoadByName;
        private Dictionary<string, Building> DicBuildingByName;
        private PolygonMesh Collision;
        public MovementAlgorithmTile[,] ArrayGrid;

        public MapContainer(string AreaName, PolygonMesh Collision)
        {
            this.AreaName = AreaName;
            this.Collision = Collision;
        }

        public bool InInside(Vector3 WorldPosition)
        {
            PolygonMesh.PolygonMeshCollisionResult CollisionResult = PolygonMesh.PolygonCollisionPoint(Collision, WorldPosition, Vector3.Zero);

            return CollisionResult.Collided;
        }

        public override string ToString()
        {
            return AreaName;
        }
    }
}
