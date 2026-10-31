using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

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

        public PassageState State
        {
            get { return Passage.Read(IsClosed, IsLocked, HP); }
            set { Passage.Apply(value, ref IsClosed, ref IsLocked, ref HP); }
        }

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
        public bool IsClosed = true;
        public int TimeToOpen;
        public int TimeToClose;
        public bool CanBeLocked;
        public bool IsLocked;
        public bool RequireKeyName;
        public int TimeToUnlock;
        public int TimeToLock;
        public int HP = 10;

        public Vector3 Position;

        public PassageState State
        {
            get { return Passage.Read(IsClosed, IsLocked, HP); }
            set { Passage.Apply(value, ref IsClosed, ref IsLocked, ref HP); }
        }
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
            ListLink = new List<ChunkHop>();
            ListMarker = new List<WorldMarker>();
            Furniture = new List<FurnitureItem>();
            Buildings = new List<BuildingItem>();
            Zones = new List<BuildingZone>();
            Water = new List<WaterVolume>();
            Tracks = new WaterTracks();
            Landmarks = new List<LandmarkHeader>();
            Bands = new List<MeshBand>();
            Scatter = new List<ScatterSpot>();
            Forest = new ForestRecipe();
            People = new List<PlayerCharacter>();
            if (MapMapContainer != null && MapMapContainer.ActiveMap is LifeSimMap Live)
            {
                Live.Furniture = Furniture;
                Live.Buildings = Buildings;
                Live.Water = Water;
            }
        }

        public List<ChunkHop> ListLink;
        public List<WorldMarker> ListMarker;
        public List<FurnitureItem> Furniture;
        public List<BuildingItem> Buildings;
        public List<BuildingZone> Zones;
        public List<WaterVolume> Water;
        public WaterTracks Tracks;
        public int ChunkX;
        public int ChunkZ;
        public GroundKind Kind;
        public bool HeaderReady;
        public bool BodyReady;
        public HeightField Heights;
        public string GridPath = string.Empty;
        public string MeshPath = string.Empty;
        public List<MeshBand> Bands;
        public int ActiveBand = -1;
        public ForestRecipe Forest;
        public List<LandmarkHeader> Landmarks;
        public List<ScatterSpot> Scatter;
        public List<PlayerCharacter> People;
        public bool ViewReady;
        public Vector3[] MeshVertices;
        public int[] MeshIndices;
        public ProjectEternity.Core.Graphics.FbxModel MeshModel;
        public float MeshScale = 16f;

        public bool HasNeighbour(string AreaName)
        {
            return DicNeighbourByName.ContainsKey(AreaName);
        }

        public bool HasNested(string AreaName)
        {
            return DicNestedMapByName.ContainsKey(AreaName);
        }

        public void Adopt(PlayerCharacter Who)
        {
            if (Who == null)
            {
                return;
            }

            if (People == null)
            {
                People = new List<PlayerCharacter>();
            }

            if (!People.Contains(Who))
            {
                People.Add(Who);
            }

            LifeSimMap Map = MapMapContainer == null ? null : MapMapContainer.ActiveMap as LifeSimMap;
            if (Map != null && Map.ListCharacter != null && !Map.ListCharacter.Contains(Who))
            {
                Map.ListCharacter.Add(Who);
            }
        }

        public void Drop(PlayerCharacter Who)
        {
            if (Who == null)
            {
                return;
            }

            if (People != null)
            {
                People.Remove(Who);
            }

            LifeSimMap Map = MapMapContainer == null ? null : MapMapContainer.ActiveMap as LifeSimMap;
            if (Map != null && Map.ListCharacter != null)
            {
                Map.ListCharacter.Remove(Who);
            }
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
        public float SpanX;
        public float SpanY;

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
