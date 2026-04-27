using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
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

        public LifeSimMap ActiveMap;
        public long OffsetX;
        public long OffsetY;
        public long OffsetZ;

        public MapContainer(string AreaName, LifeSimMap ActiveMap)
        {
            this.AreaName = AreaName;
            this.ActiveMap = ActiveMap;
        }

        public override string ToString()
        {
            return AreaName;
        }
    }
}
