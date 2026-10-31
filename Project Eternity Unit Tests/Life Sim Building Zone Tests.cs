using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimBuildingZoneTests
    {
        [TestMethod]
        public void ARectangleAcrossAChunkEdgeIsOneZone()
        {
            MapInfo West = Chunk("West", 0, 100);
            MapInfo East = Chunk("East", 100, 100);
            List<MapInfo> Maps = new List<MapInfo> { West, East };
            List<ZoneFootprint> Feet = BuildingZones.Cover(Maps, new Vector3(80f, 10f, 0f), new Vector3(140f, 40f, 0f));
            BuildingZone Zone = new BuildingZone();
            Zone.Name = "House";
            Zone.Public = true;
            Zone.Footprints.AddRange(Feet);
            List<BuildingZone> World = new List<BuildingZone>();
            BuildingZones.Add(World, Zone);

            Assert.AreEqual(2, Zone.Footprints.Count);
            Assert.AreSame(Zone, BuildingZones.At(West, 90f, 20f));
            Assert.AreSame(Zone, BuildingZones.At(East, 20f, 20f));
            Assert.IsNull(BuildingZones.At(West, 10f, 20f));
        }

        [TestMethod]
        public void DeleteRemovesBoxesInsideEveryFootprint()
        {
            MapInfo West = Chunk("West", 0, 100);
            MapInfo East = Chunk("East", 100, 100);
            FurnitureItem InsideWest = Box(20f, 20f);
            FurnitureItem InsideEast = Box(10f, 10f);
            FurnitureItem Outside = Box(80f, 80f);
            West.Furniture.Add(InsideWest);
            East.Furniture.Add(InsideEast);
            East.Furniture.Add(Outside);
            BuildingZone Zone = new BuildingZone();
            Zone.Footprints.Add(Foot(West, 0f, 0f, 50f, 50f));
            Zone.Footprints.Add(Foot(East, 0f, 0f, 50f, 50f));
            List<BuildingZone> World = new List<BuildingZone>();
            BuildingZones.Add(World, Zone);

            BuildingZones.Delete(World, Zone);

            Assert.AreEqual(0, World.Count);
            Assert.IsFalse(West.Furniture.Contains(InsideWest));
            Assert.IsFalse(East.Furniture.Contains(InsideEast));
            Assert.IsTrue(East.Furniture.Contains(Outside));
        }

        [TestMethod]
        public void ANeighborIsABorderWalk()
        {
            LifeSimMap WestMap = Map(4, 4);
            LifeSimMap EastMap = Map(4, 4);
            MapInfo West = Live("West", WestMap, 0f);
            MapInfo East = Live("East", EastMap, West.MapMapContainer.SpanX);
            Border(West, East);
            PlayerCharacter Ada = new PlayerCharacter(null, West, Center(WestMap, 0, 2));
            Ada.Belief.LearnArea(West);
            Ada.Belief.LearnArea(East);
            Ada.Belief.LearnLink(West.ListLink[0]);
            ChunkHop Portal = PortalBetween(West, East);
            Ada.Belief.LearnLink(Portal);

            List<AutomatedAction> Steps = LifePlan.GoTo(Ada, East, Center(EastMap, 2, 2));

            Assert.IsTrue(Has(Steps, "Follow Path"));
            Assert.IsTrue(Has(Steps, "Cross Border"));
            Assert.IsFalse(Has(Steps, "Cross Portal"));
        }

        [TestMethod]
        public void TheSameZoneWalksAcrossItsOwnBorder()
        {
            LifeSimMap WestMap = Map(4, 4);
            LifeSimMap EastMap = Map(4, 4);
            MapInfo West = Live("West", WestMap, 0f);
            MapInfo East = Live("East", EastMap, West.MapMapContainer.SpanX);
            Border(West, East);
            West.ListLink.Add(PortalBetween(West, East));
            BuildingZone Zone = new BuildingZone();
            Zone.Name = "House";
            Zone.Public = false;
            Zone.Footprints.Add(Foot(West, 0f, 0f, West.MapMapContainer.SpanX, West.MapMapContainer.SpanY));
            Zone.Footprints.Add(Foot(East, 0f, 0f, East.MapMapContainer.SpanX, East.MapMapContainer.SpanY));
            BuildingZones.Add(null, Zone);
            PlayerCharacter Ada = new PlayerCharacter(null, West, Center(WestMap, 0, 2));
            Ada.Belief.LearnLink(West.ListLink[1]);

            List<AutomatedAction> Steps = LifePlan.GoTo(Ada, East, Center(EastMap, 2, 2));

            Assert.IsTrue(Has(Steps, "Follow Path"));
            Assert.IsFalse(Has(Steps, "Cross Portal"));
        }

        [TestMethod]
        public void ALongRouteCanTakeAPortal()
        {
            MapInfo[] Maps = new MapInfo[18];
            for (int i = 0; i < Maps.Length; i++)
            {
                Maps[i] = Chunk(i == Maps.Length - 1 ? "Far" : "Step " + i, i * 100f, 100f);
            }

            for (int i = 0; i < Maps.Length - 1; i++)
            {
                Border(Maps[i], Maps[i + 1]);
            }

            ChunkHop Skip = new ChunkHop();
            Skip.Name = "Gate";
            Skip.FromChunk = Maps[0];
            Skip.ToChunk = Maps[Maps.Length - 1];
            Skip.FromPosition = Vector3.Zero;
            Skip.ToPosition = new Vector3(8f, 0f, 0f);
            Skip.IsOpen = true;
            Maps[0].ListLink.Add(Skip);
            Maps[0].AddNeighbour(Maps[Maps.Length - 1]);
            Maps[Maps.Length - 1].AddNeighbour(Maps[0]);

            PlayerCharacter Ada = new PlayerCharacter(null, Maps[0], Vector3.Zero);
            for (int i = 0; i < Maps.Length; i++)
            {
                Ada.Belief.LearnArea(Maps[i]);
                for (int Link = 0; Link < Maps[i].ListLink.Count; Link++)
                {
                    if (Maps[i].ListLink[Link].FromChunk == Maps[i])
                    {
                        Ada.Belief.LearnLink(Maps[i].ListLink[Link]);
                    }
                }
            }

            List<AutomatedAction> Steps = LifePlan.GoTo(Ada, Maps[Maps.Length - 1], new Vector3(8f, 0f, 0f));

            Assert.IsTrue(Has(Steps, "Cross Portal"));
        }

        [TestMethod]
        public void KnowsRestaurantReturnsTheZoneEntry()
        {
            MapInfo Room = Chunk("Room", 0f, 100f);
            BuildingZone Zone = new BuildingZone();
            Zone.Name = "Restaurant";
            Zone.Public = true;
            Zone.EntryChunk = Room;
            Zone.Entry = new Vector3(12f, 40f, 0f);
            Zone.Footprints.Add(Foot(Room, 0f, 0f, 80f, 80f));
            BuildingZones.Add(null, Zone);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, new Vector3(4f, 4f, 0f));

            MapInfo Found;
            Vector3 Place;
            Assert.IsTrue(LifePlan.KnowsRestaurant(Ada, out Found, out Place));
            Assert.AreSame(Room, Found);
            Assert.AreEqual(Zone.Entry, Place);
        }

        static MapInfo Chunk(string Name, float X, float Span)
        {
            MapInfo Map = new MapInfo(new MapContainer(Name, null), null, null);
            Map.MapMapContainer.OffsetX = (long)X;
            Map.MapMapContainer.SpanX = Span;
            Map.MapMapContainer.SpanY = Span;
            return Map;
        }

        static MapInfo Live(string Name, LifeSimMap Map, float X)
        {
            MapInfo Info = Chunk(Name, X, Map.MapSize.X * Map.TileSize.X);
            Info.MapMapContainer.ActiveMap = Map;
            Map.Chunk = Info;
            return Info;
        }

        static ZoneFootprint Foot(MapInfo Chunk, float MinX, float MinY, float MaxX, float MaxY)
        {
            ZoneFootprint Footprint = new ZoneFootprint();
            Footprint.Chunk = Chunk;
            Footprint.Min = new Vector3(MinX, MinY, 0f);
            Footprint.Max = new Vector3(MaxX, MaxY, 0f);
            return Footprint;
        }

        static FurnitureItem Box(float X, float Y)
        {
            FurnitureItem Box = new FurnitureItem();
            Box.Position = new Vector3(X, Y, 0f);
            return Box;
        }

        static void Border(MapInfo A, MapInfo B)
        {
            float Span = A.MapMapContainer.SpanX;
            ChunkHop Hop = new ChunkHop();
            Hop.Name = "Border";
            Hop.FromChunk = A;
            Hop.ToChunk = B;
            Hop.FromPosition = new Vector3(Span, Span * 0.5f, 0f);
            Hop.ToPosition = new Vector3(0f, Span * 0.5f, 0f);
            Hop.IsOpen = true;
            A.ListLink.Add(Hop);
            B.ListLink.Add(Hop.Reversed());
            A.AddNeighbour(B);
            B.AddNeighbour(A);
        }

        static ChunkHop PortalBetween(MapInfo A, MapInfo B)
        {
            ChunkHop Hop = new ChunkHop();
            Hop.Name = "Gate";
            Hop.FromChunk = A;
            Hop.ToChunk = B;
            Hop.FromPosition = new Vector3(4f, 4f, 0f);
            Hop.ToPosition = new Vector3(8f, 4f, 0f);
            Hop.IsOpen = true;
            A.ListLink.Add(Hop);
            return Hop;
        }

        static LifeSimMap Map(int Width, int Height)
        {
            LifeSimMap Map = new LifeSimMap();
            Map.TileSize = new Point(32, 32);
            Map.MapSize = new Point(Width, Height);
            Map.LayerManager.ListLayer.Add(new MapLayer(Map, 0));
            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    Terrain Tile = Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y];
                    Tile.TerrainTypeIndex = UnitStats.TerrainLandIndex;
                    Tile.Height = 0f;
                    Tile.WorldPosition.X = X * Map.TileSize.X;
                    Tile.WorldPosition.Y = Y * Map.TileSize.Y;
                    Tile.WorldPosition.Z = 0f;
                }
            }

            return Map;
        }

        static Vector3 Center(LifeSimMap Map, int X, int Y)
        {
            return GridNavGeometry.CellCenter(Map, Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y]);
        }

        static bool Has(List<AutomatedAction> Steps, string Name)
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                if (Steps[i].ActionName == Name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
