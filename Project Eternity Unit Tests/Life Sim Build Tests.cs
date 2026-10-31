using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimBuildTests
    {
        [TestMethod]
        public void GridSnapCentersACellAndCornersAWall()
        {
            Vector3 Center = BuildFloor.SnapCenter(new Vector3(10f, 40f, 0f), 32f, true);
            Assert.AreEqual(16f, Center.X, 0.001f);
            Assert.AreEqual(48f, Center.Y, 0.001f);

            Vector3 Free = BuildFloor.SnapCenter(new Vector3(10f, 40f, 0f), 32f, false);
            Assert.AreEqual(10f, Free.X, 0.001f);

            BuildingItem Wall = WallPlan.MakeWall(new Vector3(0f, 0f, 0f), new Vector3(96f, 5f, 0f), 0, 32f, true);
            Assert.AreEqual(BuildFloor.Thickness, System.Math.Min(Wall.Width, Wall.Length), 0.001f);
            Assert.IsTrue(System.Math.Max(Wall.Width, Wall.Length) >= 96f);
            Assert.AreEqual(0f, Wall.Position.Y, 0.001f);

            BuildingItem Diagonal = WallPlan.MakeWall(new Vector3(0f, 0f, 0f), new Vector3(96f, 96f, 0f), 0, 32f, true);
            Assert.AreEqual(BuildFloor.Thickness, Diagonal.Length, 0.001f);
            Assert.IsTrue(Diagonal.Width > 96f);
            Assert.AreEqual(0.785f, Diagonal.Angle, 0.02f);
            Assert.IsTrue(Diagonal.FootprintContains(48f, 48f));
            Assert.IsFalse(Diagonal.FootprintContains(96f, 0f));
        }

        [TestMethod]
        public void FurnitureStaysOnTheFloorAndRefusesAStackOrAWall()
        {
            List<FurnitureItem> Furniture = new List<FurnitureItem>();
            List<BuildingItem> Buildings = new List<BuildingItem>();
            BuildResult Bed = BuildPlan.Commit("Bed", new Vector3(16f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings);
            Assert.IsTrue(Bed.Valid);
            Furniture.Add(Bed.AddFurniture[0]);

            BuildResult Stack = BuildPlan.Commit("Fridge", new Vector3(16f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings);
            Assert.IsFalse(Stack.Valid);

            BuildingItem Wall = WallPlan.MakeWall(new Vector3(0f, 64f, 0f), new Vector3(96f, 64f, 0f), 0, 32f, true);
            Buildings.Add(Wall);
            BuildResult OnWall = BuildPlan.Commit("Bed", new Vector3(48f, 64f, 0f), Vector3.Zero, false, 0, false, 32f, Furniture, Buildings);
            Assert.IsFalse(OnWall.Valid);

            BuildResult Upper = BuildPlan.Commit("Bed", new Vector3(16f, 16f, 0f), Vector3.Zero, false, 1, true, 32f, Furniture, Buildings);
            Assert.IsFalse(Upper.Valid);

            BuildResult Plate = WallPlan.Commit("Floor", new Vector3(16f, 200f, 0f), Vector3.Zero, false, 1, true, 32f, Buildings, Furniture);
            Assert.IsTrue(Plate.Valid);
            Buildings.Add(Plate.AddBuilding[0]);
            BuildResult OnPlate = BuildPlan.Commit("Bed", new Vector3(16f, 200f, 0f), Vector3.Zero, false, 1, true, 32f, Furniture, Buildings);
            Assert.IsTrue(OnPlate.Valid);
            Assert.AreEqual(1, OnPlate.AddFurniture[0].Floor);
        }

        [TestMethod]
        public void AWallWaitsForTheSecondAnchorAndADoorOpensIt()
        {
            List<FurnitureItem> Furniture = new List<FurnitureItem>();
            List<BuildingItem> Buildings = new List<BuildingItem>();
            BuildResult First = WallPlan.Preview("Wall", new Vector3(0f, 0f, 0f), Vector3.Zero, false, 0, true, 32f, Buildings, Furniture);
            Assert.IsFalse(WallPlan.Commit("Wall", new Vector3(0f, 0f, 0f), Vector3.Zero, false, 0, true, 32f, Buildings, Furniture).Valid);
            Assert.IsNotNull(First.BuildingGhost);

            BuildResult Span = WallPlan.Commit("Wall", new Vector3(128f, 0f, 0f), new Vector3(0f, 0f, 0f), true, 0, true, 32f, Buildings, Furniture);
            Assert.IsTrue(Span.Valid);
            Buildings.Add(Span.AddBuilding[0]);

            BuildResult Door = WallPlan.Commit("Door", new Vector3(64f, 0f, 0f), Vector3.Zero, false, 0, true, 32f, Buildings, Furniture);
            Assert.IsTrue(Door.Valid);
            Assert.AreEqual(1, Door.RemoveBuilding.Count);
            bool FoundDoor = false;
            float Covered = 0f;
            for (int i = 0; i < Door.AddBuilding.Count; i++)
            {
                Covered += System.Math.Max(Door.AddBuilding[i].Width, Door.AddBuilding[i].Length);
                if (Door.AddBuilding[i].Kind == BuildingKind.Door)
                {
                    FoundDoor = true;
                    Assert.IsFalse(Door.AddBuilding[i].HitsMove(new Vector3(64f, -8f, 0f), new Vector3(64f, 8f, 0f)));
                }
            }

            Assert.IsTrue(FoundDoor);
            Assert.AreEqual(128f, Covered, 1f);

            BuildResult Window = WallPlan.Commit("Window", new Vector3(32f, 0f, 0f), Vector3.Zero, false, 0, true, 32f, Buildings, Furniture);
            Assert.IsTrue(Window.Valid);
            bool Glass = false;
            for (int i = 0; i < Window.AddBuilding.Count; i++)
            {
                if (Window.AddBuilding[i].Kind == BuildingKind.Window)
                {
                    Glass = true;
                }
            }

            Assert.IsTrue(Glass);
        }

        [TestMethod]
        public void ADoorMeshCutsItsOwnWidthAndBlocksTheWall()
        {
            SurfaceItem Door = WallCatalog.Find("Door 1");
            Assert.IsTrue(Door.Model.EndsWith("Door_1.fbx"));
            SurfaceItem Wide = WallCatalog.Find("Double Door");
            SurfaceItem Glass = WallCatalog.Find("Large Window 1");
            Assert.IsTrue(Glass.Model.EndsWith("Window_Large1.fbx"));

            List<FurnitureItem> Furniture = new List<FurnitureItem>();
            List<BuildingItem> Buildings = new List<BuildingItem>();
            Buildings.Add(WallPlan.MakeWall(Vector3.Zero, new Vector3(128f, 0f, 0f), 0, 32f, true));

            BuildResult Cut = WallPlan.Commit("Door 1", new Vector3(64f, 0f, 0f), Vector3.Zero, false, 0, true, 32f, Buildings, Furniture);
            Assert.IsTrue(Cut.Valid);
            Buildings.Remove(Cut.RemoveBuilding[0]);
            BuildingItem Leaf = null;
            for (int i = 0; i < Cut.AddBuilding.Count; i++)
            {
                Buildings.Add(Cut.AddBuilding[i]);
                if (Cut.AddBuilding[i].Kind == BuildingKind.Door)
                {
                    Leaf = Cut.AddBuilding[i];
                }
            }

            Assert.AreEqual(Door.Span / Door.Rise * BuildFloor.Story, Leaf.Width, 0.01f);
            Assert.AreEqual(BuildFloor.Story, Leaf.Height, 0.01f);
            Assert.AreEqual(Door.Model, Leaf.Model);
            Assert.AreEqual("Wall already there", WallPlan.Commit("Wall", new Vector3(128f, 0f, 0f), Vector3.Zero, true, 0, true, 32f, Buildings, Furniture).Reason);

            BuildResult Pane = WallPlan.Commit("Large Window 1", new Vector3(24f, 0f, 0f), Vector3.Zero, false, 0, true, 32f, Buildings, Furniture);
            Assert.IsTrue(Pane.Valid);
            bool Sill = false;
            for (int i = 0; i < Pane.AddBuilding.Count; i++)
            {
                BuildingItem Piece = Pane.AddBuilding[i];
                if (Piece.Kind == BuildingKind.Window)
                {
                    Assert.AreEqual(BuildFloor.Story * Glass.Rise / WallCatalog.KitRise, Piece.Height, 0.01f);
                    Assert.AreEqual(Glass.Model, Piece.Model);
                }

                if (Piece.Kind == BuildingKind.Wall && System.Math.Abs(Piece.Height - BuildFloor.Story * 0.35f) < 0.01f)
                {
                    Sill = true;
                }
            }

            Assert.IsTrue(Sill);

            List<BuildingItem> Short = new List<BuildingItem>();
            Short.Add(WallPlan.MakeWall(Vector3.Zero, new Vector3(32f, 0f, 0f), 0, 32f, true));
            Assert.AreEqual("Wall is too short", WallPlan.Commit("Double Door", new Vector3(16f, 0f, 0f), Vector3.Zero, false, 0, true, 32f, Short, Furniture).Reason);
            Assert.IsTrue(Wide.Span > Door.Span);
        }

        [TestMethod]
        public void StairsClimbAStoryAndUpperFloorsHide()
        {
            Assert.AreEqual(1, BuildFloor.Shift(0, 1));
            Assert.AreEqual(0, BuildFloor.Shift(0, -1));

            List<FurnitureItem> Furniture = new List<FurnitureItem>();
            List<BuildingItem> Buildings = new List<BuildingItem>();
            BuildResult Stairs = WallPlan.Commit("Stairs", new Vector3(16f, 32f, 0f), Vector3.Zero, false, 0, true, 32f, Buildings, Furniture);
            Assert.IsTrue(Stairs.Valid);
            BuildingItem Flight = Stairs.AddBuilding[0];
            Assert.AreEqual(BuildFloor.Story, Flight.Height, 0.001f);
            Assert.IsTrue(Flight.Tread(new Vector3(Flight.Position.X, Flight.Position.Y + Flight.Length * 0.5f, 0f)) > Flight.Tread(new Vector3(Flight.Position.X, Flight.Position.Y - Flight.Length * 0.5f, 0f)));

            BuildingItem Upper = WallPlan.MakeWall(Vector3.Zero, new Vector3(64f, 0f, 0f), 1, 32f, true);
            Assert.IsFalse(WallPlan.Shows(Upper, 0, true));
            Assert.IsTrue(WallPlan.Shows(Upper, 1, true));
            Assert.IsTrue(WallPlan.Shows(Upper, 0, false));
        }

        [TestMethod]
        public void TheCatalogHasACardAndGrabFindsFurniture()
        {
            Assert.AreEqual(7, BuildCatalog.Pages.Length);
            Assert.AreEqual("Seating & Beds", BuildCatalog.Title(CatalogPage.Comfort));
            CatalogItem Seat = BuildCatalog.Find("Diner Seat");
            Assert.IsNotNull(Seat);
            Assert.AreEqual(100, Seat.Price);
            Assert.AreEqual(3, Seat.Comfort);
            Assert.IsTrue(Seat.Description.Length > 0);
            Assert.AreEqual(8, BuildCatalog.In(CatalogPage.Electronics).Count);
            CatalogItem CheapToilet = BuildCatalog.Find("Cheap Toilet");
            Assert.AreEqual(CatalogPage.Plumbing, CheapToilet.Page);
            Assert.AreEqual(80, CheapToilet.Price);
            Assert.IsTrue(CheapToilet.Model.EndsWith("Cheap Toilet.glb"));
            Assert.AreEqual(17, BuildCatalog.In(CatalogPage.Plumbing).Count);

            CatalogItem King = BuildCatalog.Find("King Bed");
            Assert.IsTrue(King.Model.EndsWith("Bed_King.fbx"));
            BuildResult KingPlace = BuildPlan.Commit("King Bed", new Vector3(80f, 80f, 0f), Vector3.Zero, false, 0, true, 32f, new List<FurnitureItem>(), new List<BuildingItem>());
            Assert.IsTrue(KingPlace.Valid);
            Assert.AreEqual(King.Model, KingPlace.AddFurniture[0].Model);
            Assert.IsTrue(KingPlace.AddFurniture[0].Width > 32f);

            CatalogItem Lamp = BuildCatalog.Find("Ceiling Light 1");
            Assert.AreEqual(PlaceRule.Ceiling, Lamp.Rule);
            BuildResult Hung = BuildPlan.Commit("Ceiling Light 1", new Vector3(16f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, new List<FurnitureItem>(), new List<BuildingItem>());
            Assert.IsTrue(Hung.Valid);
            Assert.AreEqual(BuildFloor.Story - 32f * Lamp.Rise * BuildCatalog.FbxFit, Hung.AddFurniture[0].Position.Z, 0.01f);

            BuildResult Rug = BuildPlan.Commit("Carpet 1", new Vector3(200f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, new List<FurnitureItem>(), new List<BuildingItem>());
            Assert.IsTrue(Rug.Valid);
            Assert.IsFalse(Rug.AddFurniture[0].BlocksBody());

            BuildResult Fixture = BuildPlan.Commit("Cheap Toilet", new Vector3(16f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, new List<FurnitureItem>(), new List<BuildingItem>());
            Assert.IsTrue(Fixture.Valid);
            Assert.AreEqual(CheapToilet.Model, Fixture.AddFurniture[0].Model);
            Assert.IsTrue(Fixture.AddFurniture[0].Width < 32f);
            Assert.IsTrue(Fixture.AddFurniture[0].Height > Fixture.AddFurniture[0].Width);

            List<FurnitureItem> Furniture = new List<FurnitureItem>();
            List<BuildingItem> Buildings = new List<BuildingItem>();
            BuildResult Bed = BuildPlan.Commit("Bed", new Vector3(16f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings);
            Furniture.Add(Bed.AddFurniture[0]);
            Assert.AreSame(Bed.AddFurniture[0], BuildPlan.FurnitureAt(Furniture, new Vector3(16f, 16f, 0f), 0));
            Assert.IsNull(BuildPlan.FurnitureAt(Furniture, new Vector3(200f, 200f, 0f), 0));

            Assert.AreEqual(5, WallCatalog.Pages.Length);
            Assert.AreEqual("Walls", WallCatalog.Title(SurfacePage.Walls));
            Assert.AreEqual("Doors", WallCatalog.Title(SurfacePage.Doors));
            Assert.AreEqual("Windows", WallCatalog.Title(SurfacePage.Windows));
            Assert.AreEqual("Stairs", WallCatalog.Title(SurfacePage.Stairs));
            Assert.AreEqual(SurfacePage.Doors, WallCatalog.Find("Door 1").Page);
            Assert.AreEqual(SurfacePage.Windows, WallCatalog.Find("Large Window 1").Page);
            Assert.AreEqual(SurfacePage.Stairs, WallCatalog.Find("Stairs").Page);
            SurfaceItem Brick = WallCatalog.Find("Brick");
            Assert.AreEqual(25, Brick.Price);
            Assert.AreEqual(SurfaceUse.Wall, Brick.Use);
            Assert.IsNotNull(WallCatalog.Find("Hardwood"));

            BuildResult Painted = WallPlan.Commit("Brick", new Vector3(96f, 0f, 0f), Vector3.Zero, true, 0, true, 32f, Buildings, Furniture);
            Assert.IsTrue(Painted.Valid);
            Assert.AreEqual(Brick.Swatch, Painted.AddBuilding[0].Swatch);
            Buildings.Add(Painted.AddBuilding[0]);
            Assert.AreSame(Painted.AddBuilding[0], WallPlan.Under(Buildings, new Vector3(48f, 0f, 0f), 0));

            BuildResult Hardwood = WallPlan.Commit("Hardwood", new Vector3(16f, 200f, 0f), Vector3.Zero, false, 0, true, 32f, Buildings, Furniture);
            Assert.IsTrue(Hardwood.Valid);
            Assert.AreEqual(BuildingKind.Floor, Hardwood.AddBuilding[0].Kind);
            Buildings.Add(Hardwood.AddBuilding[0]);
            Assert.AreSame(Hardwood.AddBuilding[0], WallPlan.Under(Buildings, new Vector3(16f, 200f, 0f), 0));
        }

        [TestMethod]
        public void AComputerSitsOnASurfaceAndAWallScreenClearsShortFurniture()
        {
            CatalogItem Fountain = BuildCatalog.Find("College Fountain");
            Assert.AreEqual(CatalogPage.Decorative, Fountain.Page);
            Assert.IsTrue(Fountain.Model.EndsWith("College Foutain.glb"));
            CatalogItem Workbench = BuildCatalog.Find("Gearhead Table");
            Assert.AreEqual(CatalogPage.Surfaces, Workbench.Page);
            Assert.AreEqual(PlaceRule.Surface, BuildCatalog.Find("Computer").Rule);
            Assert.AreEqual(PlaceRule.Wall, BuildCatalog.Find("Wall Television").Rule);

            List<FurnitureItem> Furniture = new List<FurnitureItem>();
            List<BuildingItem> Buildings = new List<BuildingItem>();
            BuildResult FountainPlace = BuildPlan.Commit("College Fountain", new Vector3(80f, 80f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings);
            Assert.IsTrue(FountainPlace.Valid);
            Assert.IsTrue(FountainPlace.AddFurniture[0].Width > 32f);

            BuildResult Bare = BuildPlan.Commit("Computer", new Vector3(16f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings);
            Assert.IsFalse(Bare.Valid);
            Assert.AreEqual("Needs a surface", Bare.Reason);

            BuildResult Table = BuildPlan.Commit("Table", new Vector3(16f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings);
            Furniture.Add(Table.AddFurniture[0]);
            BuildResult OnTable = BuildPlan.Commit("Computer", new Vector3(16f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings);
            Assert.IsTrue(OnTable.Valid);
            Assert.AreEqual(Table.AddFurniture[0].Position.Z + Table.AddFurniture[0].Height, OnTable.AddFurniture[0].Position.Z, 0.001f);
            Furniture.Add(OnTable.AddFurniture[0]);
            Assert.IsFalse(BuildPlan.Commit("Computer", new Vector3(16f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings).Valid);

            BuildResult Bench = BuildPlan.Commit("Gearhead Table", new Vector3(200f, 48f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings);
            Furniture.Add(Bench.AddFurniture[0]);
            Assert.IsTrue(BuildPlan.Commit("Computer", new Vector3(200f, 48f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings).Valid);
            Assert.IsFalse(BuildPlan.Commit("Computer", new Vector3(26f, 16f, 0f), Vector3.Zero, false, 0, false, 32f, Furniture, Buildings).Valid);

            BuildResult Fridge = BuildPlan.Commit("Fridge", new Vector3(16f, 80f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings);
            Furniture.Add(Fridge.AddFurniture[0]);
            Assert.IsFalse(BuildPlan.Commit("Computer", new Vector3(16f, 80f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings).Valid);

            BuildingItem Wall = WallPlan.MakeWall(Vector3.Zero, new Vector3(128f, 0f, 0f), 0, 32f, true);
            Buildings.Add(Wall);
            BuildResult Screen = BuildPlan.Commit("Wall Television", new Vector3(64f, 16f, 0f), Vector3.Zero, false, 0, false, 32f, Furniture, Buildings);
            Assert.IsTrue(Screen.Valid);
            Assert.IsTrue(Screen.AddFurniture[0].Position.Z > Table.AddFurniture[0].Height);

            Furniture.Add(BuildPlan.Commit("Table", new Vector3(64f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings).AddFurniture[0]);
            Assert.IsTrue(BuildPlan.Commit("Wall Television", new Vector3(64f, 16f, 0f), Vector3.Zero, false, 0, false, 32f, Furniture, Buildings).Valid);

            Furniture.Add(BuildPlan.Commit("Fridge", new Vector3(96f, 16f, 0f), Vector3.Zero, false, 0, true, 32f, Furniture, Buildings).AddFurniture[0]);
            BuildResult BlockedScreen = BuildPlan.Commit("Wall Television", new Vector3(96f, 16f, 0f), Vector3.Zero, false, 0, false, 32f, Furniture, Buildings);
            Assert.IsFalse(BlockedScreen.Valid);
            Assert.AreEqual("Something blocks it", BlockedScreen.Reason);

            BuildingItem Short = WallPlan.MakeWall(new Vector3(0f, 200f, 0f), new Vector3(32f, 200f, 0f), 0, 32f, true);
            Buildings.Add(Short);
            Assert.AreEqual("Wall is too short", BuildPlan.Commit("Wall Television", new Vector3(16f, 210f, 0f), Vector3.Zero, false, 0, false, 32f, Furniture, Buildings).Reason);
        }

        [TestMethod]
        public void TelevisionWatchesFromTheFrontAndOffToTheSide()
        {
            FurnitureItem Screen = Piece("Television", 80f, 48f, 0f);
            List<Vector3> Tiles = FurnitureUse.Tiles(Screen, 32f);
            Assert.IsTrue(Has(Tiles, 80f, 80f));
            Assert.IsTrue(Has(Tiles, 144f, 80f));
            Assert.IsFalse(Has(Tiles, 80f, 16f));
            Assert.IsFalse(Has(Tiles, 208f, 80f));
            Assert.IsFalse(Has(Tiles, 80f, 48f));

            Screen.Angle = MathHelper.PiOver2;
            Tiles = FurnitureUse.Tiles(Screen, 32f);
            Assert.IsTrue(Has(Tiles, 48f, 48f));
            Assert.IsFalse(Has(Tiles, 112f, 48f));
        }

        [TestMethod]
        public void StereoCanBeUsedFromAnySideButNotFromOnTop()
        {
            FurnitureItem Box = Piece("Stereo", 80f, 80f, 0f);
            List<Vector3> Tiles = FurnitureUse.Tiles(Box, 32f);
            Assert.IsTrue(Has(Tiles, 48f, 80f));
            Assert.IsTrue(Has(Tiles, 112f, 80f));
            Assert.IsTrue(Has(Tiles, 80f, 48f));
            Assert.IsFalse(Has(Tiles, 80f, 80f));
        }

        [TestMethod]
        public void WatchPrefersAnEmptySeatAndStandsWhenThatSeatIsTaken()
        {
            FurnitureItem Screen = Piece("Television", 80f, 48f, 0f);
            FurnitureItem Chair = Piece("Chair", 80f, 80f, 0f);
            List<FurnitureItem> Room = new List<FurnitureItem>();
            Room.Add(Screen);
            Room.Add(Chair);
            PlayerCharacter Viewer = new PlayerCharacter(null, null, new Vector3(16f, 80f, 0f));
            List<UseSpot> Open = FurnitureUse.Choices(Screen, Room, null, Viewer, 32f);
            Assert.IsTrue(Open.Count > 0);
            Assert.IsTrue(Open[0].Sitting);
            Assert.AreEqual(Chair.Position.X, Open[0].Point.X, 0.01f);
            Assert.AreEqual(Chair.Position.Y, Open[0].Point.Y, 0.01f);

            PlayerCharacter Sitting = new PlayerCharacter(null, null, Chair.Position);
            List<PlayerCharacter> People = new List<PlayerCharacter>();
            People.Add(Sitting);
            List<UseSpot> Standing = FurnitureUse.Choices(Screen, Room, People, Viewer, 32f);
            Assert.IsTrue(Standing.Count > 0);
            Assert.IsFalse(Standing[0].Sitting);
            Assert.IsFalse(FurnitureUse.Taken(Standing[0].Point, People, Viewer, 32f));
        }

        static FurnitureItem Piece(string Name, float X, float Y, float Angle)
        {
            BuildResult Placed = BuildPlan.Commit(Name, new Vector3(X, Y, 0f), Vector3.Zero, false, 0, true, 32f, new List<FurnitureItem>(), new List<BuildingItem>());
            FurnitureItem Box = Placed.AddFurniture[0];
            Box.Angle = Angle;
            return Box;
        }

        static bool Has(List<Vector3> Tiles, float X, float Y)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (System.Math.Abs(Tiles[i].X - X) < 0.01f && System.Math.Abs(Tiles[i].Y - Y) < 0.01f)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
