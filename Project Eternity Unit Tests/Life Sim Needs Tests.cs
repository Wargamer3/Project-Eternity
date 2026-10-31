using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimNeedsTests
    {
        [TestMethod]
        public void AFullNeedIsGreenAndACriticalNeedIsLow()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            Ada.Hunger = 0;
            Ada.Sleep = 80;
            Ada.Hygiene = 100;
            Ada.Bladder = 140;

            MotiveBar[] Bars = Motives.Bars(Ada);

            Assert.AreEqual("Hunger", Bars[0].Name);
            Assert.AreEqual(100, Bars[0].Fill);
            Assert.AreEqual("Energy", Bars[2].Name);
            Assert.AreEqual(20, Bars[2].Fill);
            Assert.AreEqual(0, Bars[1].Fill);
            Assert.AreEqual(0, Bars[5].Fill);
            Assert.AreEqual(100, Bars[6].Fill);
        }

        [TestMethod]
        public void RoomUsesDecorationInsideTheHouse()
        {
            MapInfo House = Lot();
            WallRing(House);
            House.Furniture.Add(Piece("Painting", 1, 1));
            House.Furniture.Add(Piece("Plant", 2, 2));
            House.Furniture.Add(Piece("Bed", 1, 2));
            House.Furniture.Add(Piece("College Fountain", 4, 2));
            PlayerCharacter Ada = new PlayerCharacter(null, House, Center(1, 1));

            Assert.AreEqual(40, Motives.Room(Ada));
        }

        [TestMethod]
        public void TheYardCountsTreesFountainsLitterAndTrash()
        {
            MapInfo House = Lot();
            WallRing(House);
            House.Furniture.Add(Piece("Painting", 1, 1));
            House.Furniture.Add(Piece("College Fountain", 4, 2));
            House.Furniture.Add(Piece("Tree", 4, 4));
            House.Furniture.Add(Piece("Litter", 4, 1));
            House.Furniture.Add(Piece("Trash", 4, 3));
            House.Furniture.Add(Piece("Trash Can", 4, 0));
            House.Furniture.Add(Piece("Bed", 4, 2));
            PlayerCharacter Ada = new PlayerCharacter(null, House, Center(4, 2));

            Assert.AreEqual(30, Motives.Room(Ada));
        }

        [TestMethod]
        public void RoomIsEmptyOutsideAZone()
        {
            MapInfo Yard = new MapInfo(new MapContainer("Yard", null), null, null);
            Yard.MapMapContainer.SpanX = 100f;
            Yard.MapMapContainer.SpanY = 100f;
            PlayerCharacter Ada = new PlayerCharacter(null, Yard, new Vector3(10f, 10f, 0f));

            Assert.AreEqual(0, Motives.Room(Ada));
        }

        static MapInfo Lot()
        {
            MapInfo House = new MapInfo(new MapContainer("House", null), null, null);
            House.MapMapContainer.SpanX = 160f;
            House.MapMapContainer.SpanY = 160f;
            BuildingZone Zone = new BuildingZone();
            Zone.Name = "Home";
            Zone.Public = false;
            ZoneFootprint Foot = new ZoneFootprint();
            Foot.Chunk = House;
            Foot.Min = Vector3.Zero;
            Foot.Max = new Vector3(160f, 160f, 0f);
            Zone.Footprints.Add(Foot);
            BuildingZones.Add(null, Zone);
            return House;
        }

        static void WallRing(MapInfo House)
        {
            for (int Y = 0; Y <= 3; Y++)
            {
                for (int X = 0; X <= 3; X++)
                {
                    if (X != 0 && X != 3 && Y != 0 && Y != 3)
                    {
                        continue;
                    }

                    BuildingItem Wall = new BuildingItem();
                    Wall.Kind = BuildingKind.Wall;
                    Wall.Position = Center(X, Y);
                    Wall.Width = 32f;
                    Wall.Length = 32f;
                    House.Buildings.Add(Wall);
                }
            }
        }

        static FurnitureItem Piece(string Name, int X, int Y)
        {
            FurnitureItem Box = new FurnitureItem();
            Box.Interaction = Name;
            Box.Position = Center(X, Y);
            return Box;
        }

        static Vector3 Center(int X, int Y)
        {
            return new Vector3((X + 0.5f) * 32f, (Y + 0.5f) * 32f, 0f);
        }
    }
}
