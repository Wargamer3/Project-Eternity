using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimSplitTests
    {
        [TestMethod]
        public void ACharacterOnTheCenterDoesNotWakeTheFarChunk()
        {
            MapInfo Center = Room("Center", 0, 0, 200);
            MapInfo East = Room("East", 200, 0, 200);
            MapInfo Far = Room("Far", 10000, 0, 200);
            Join(Center, East);
            PlayerCharacter Walker = new PlayerCharacter(null, Center, new Vector3(10f, 10f, 0f));

            HashSet<MapInfo> Keep = ChunkStreaming.Needed(null, new List<PlayerCharacter> { Walker }, 100f);

            Assert.IsTrue(Keep.Contains(Center));
            Assert.IsFalse(Keep.Contains(East));
            Assert.IsFalse(Keep.Contains(Far));
        }

        [TestMethod]
        public void TheEastChunkWakesWhenTheCharacterIsWithinOneHundredOfTheEdge()
        {
            MapInfo Center = Room("Center", 0, 0, 200);
            MapInfo East = Room("East", 200, 0, 200);
            MapInfo Far = Room("Far", 10000, 0, 200);
            Join(Center, East);
            PlayerCharacter Walker = new PlayerCharacter(null, Center, new Vector3(150f, 10f, 0f));

            HashSet<MapInfo> Keep = ChunkStreaming.Needed(null, new List<PlayerCharacter> { Walker }, 100f);

            Assert.IsTrue(Keep.Contains(East));
            Assert.IsFalse(Keep.Contains(Far));
        }

        [TestMethod]
        public void TheRingSitsFlushAroundTheCenterAndTheTenthMapIsFar()
        {
            MapInfo Center = Room("Center", 5, 5, 1);
            List<MapInfo> All;
            MapInfo Far;
            ChunkRing.Around(Center, 200f, 200f, out All, out Far);

            Assert.AreEqual(10, All.Count);
            Assert.AreEqual(0, Center.MapMapContainer.OffsetX);
            Assert.AreEqual(0, Center.MapMapContainer.OffsetY);
            Assert.AreEqual(200, OffsetOf(All, "Grass 2 1").X);
            Assert.AreEqual(0, OffsetOf(All, "Grass 2 1").Y);
            Assert.AreEqual(-200, OffsetOf(All, "Grass 0 1").X);
            Assert.AreEqual(200, OffsetOf(All, "Grass 1 2").Y);
            Assert.AreEqual(-200, OffsetOf(All, "Grass 1 0").Y);
            Assert.AreEqual(10000, Far.MapMapContainer.OffsetX);
            Assert.AreEqual(0, Far.ListLink.Count);
            Assert.AreEqual(4, Center.ListLink.Count);
        }

        [TestMethod]
        public void GamepadOneSharesPlayerOneAndTheNextPadAddsPlayerTwo()
        {
            List<List<LocalDevice>> Shared = LocalSeats.Resolve(PlayerOneDevices.KeyboardAndPad1, true, false, false, false);
            Assert.IsTrue(Shared[0].Contains(LocalDevice.Keyboard));
            Assert.IsTrue(Shared[0].Contains(LocalDevice.Pad1));
            Assert.AreEqual(0, Shared[1].Count);
            Assert.AreEqual(1, Occupied(Shared));

            List<List<LocalDevice>> Second = LocalSeats.Resolve(PlayerOneDevices.KeyboardAndPad1, true, true, false, false);
            Assert.IsTrue(Second[1].Contains(LocalDevice.Pad2));
            Assert.IsFalse(Second[1].Contains(LocalDevice.Keyboard));
            Assert.AreEqual(2, Occupied(Second));

            Rectangle[] One = SplitLayout.Panes(1, 640, 480);
            Rectangle[] Two = SplitLayout.Panes(2, 640, 480);
            Assert.AreEqual(640, One[0].Width);
            Assert.AreEqual(320, Two[0].Width);
            Assert.AreEqual(0, Two[0].X);
            Assert.AreEqual(320, Two[1].X);

            Rectangle[] Swapped = SplitLayout.Panes(2, 640, 480, true);
            Assert.AreEqual(320, Swapped[0].X);
            Assert.AreEqual(0, Swapped[1].X);
        }

        [TestMethod]
        public void GamepadOneOnlyMovesTheKeyboardAndJoiningItBackRemovesAnEmptyPlayerTwo()
        {
            List<List<LocalDevice>> PadOnly = LocalSeats.Resolve(PlayerOneDevices.Pad1, true, false, false, false);
            Assert.IsTrue(PadOnly[0].Contains(LocalDevice.Pad1));
            Assert.IsFalse(PadOnly[0].Contains(LocalDevice.Keyboard));
            Assert.IsTrue(PadOnly[1].Contains(LocalDevice.Keyboard));
            Assert.AreEqual(1, PadOnly[1].Count);

            List<List<LocalDevice>> HeldByPad = LocalSeats.Resolve(PlayerOneDevices.KeyboardAndPad1, true, true, false, false);
            Assert.IsTrue(HeldByPad[1].Contains(LocalDevice.Pad2));
            Assert.AreEqual(1, HeldByPad[1].Count);

            List<List<LocalDevice>> Joined = LocalSeats.Resolve(PlayerOneDevices.KeyboardAndPad1, true, false, false, false);
            Assert.AreEqual(0, Joined[1].Count);
        }

        [TestMethod]
        public void TheCursorSquareSlidesAcrossTheBorder()
        {
            LifeSimMap West = new LifeSimMap();
            LifeSimMap East = new LifeSimMap();
            West.MapSize = new Point(4, 4);
            East.MapSize = new Point(4, 4);
            West.TileSize = new Point(32, 32);
            East.TileSize = new Point(32, 32);
            MapCursor.ShowTile(West, new Vector3(127.5f, 40f, 0f), 32f);
            MapCursor.ShowTile(East, new Vector3(0.5f, 40f, 0f), 32f);

            float WestWorld = West.CursorPosition.X;
            float EastWorld = 128f + East.CursorPosition.X;
            Assert.AreEqual(111.5f, WestWorld, 0.01f);
            Assert.AreEqual(1f, EastWorld - WestWorld, 0.01f);
            Assert.IsTrue(East.CursorPosition.X < 0f);
        }

        [TestMethod]
        public void CrossingTheEastEdgeKeepsTheSameWorldPoint()
        {
            LifeSimMap WestMap = new LifeSimMap();
            LifeSimMap EastMap = new LifeSimMap();
            MapInfo West = new MapInfo(new MapContainer("West", WestMap), null, null);
            MapInfo East = new MapInfo(new MapContainer("East", EastMap), null, null);
            West.MapMapContainer.SpanX = 200f;
            West.MapMapContainer.SpanY = 200f;
            East.MapMapContainer.OffsetX = 200;
            East.MapMapContainer.SpanX = 200f;
            East.MapMapContainer.SpanY = 200f;
            WestMap.ListCharacter.Add(new PlayerCharacter(null, West, new Vector3(190f, 10f, 4f)));
            PlayerCharacter Walker = WestMap.ListCharacter[0];
            ChunkCross.Maps = new List<MapInfo> { West, East };

            float WorldX = 210f;
            float WorldY = 10f;
            bool Moved = ChunkCross.Settle(Walker, WorldX, WorldY, 4f);

            Assert.IsTrue(Moved);
            Assert.AreSame(East, Walker.SharedMapContex.CurrentMapInfo);
            Assert.AreEqual(WorldX, Walker.Position.X + East.MapMapContainer.OffsetX, 0.01f);
            Assert.AreEqual(WorldY, Walker.Position.Y + East.MapMapContainer.OffsetY, 0.01f);
            Assert.IsTrue(EastMap.ListCharacter.Contains(Walker));
            Assert.IsFalse(WestMap.ListCharacter.Contains(Walker));
        }

        [TestMethod]
        public void ACornerLoadsTheDiagonalAndLeavesTheFarMapAlone()
        {
            List<MapInfo> All;
            MapInfo Far;
            ChunkRing.Around(Room("Center", 0, 0, 1), 200f, 200f, out All, out Far);
            HashSet<MapInfo> Keep = new HashSet<MapInfo>();

            ChunkStreaming.KeepAround(Keep, All, 190f, 190f, 40f, Far);

            Assert.IsTrue(Keep.Contains(Named(All, "Grass 2 2")));
            Assert.IsTrue(Keep.Contains(Named(All, "Grass 2 1")));
            Assert.IsFalse(Keep.Contains(Far));
        }

        [TestMethod]
        public void TheCursorStepsOntoTheLoadedMapUnderItsWorldPoint()
        {
            LifeSimMap WestMap = new LifeSimMap();
            LifeSimMap EastMap = new LifeSimMap();
            MapInfo West = new MapInfo(new MapContainer("West", WestMap), null, null);
            MapInfo East = new MapInfo(new MapContainer("East", EastMap), null, null);
            West.MapMapContainer.SpanX = 200f;
            West.MapMapContainer.SpanY = 200f;
            East.MapMapContainer.OffsetX = 200;
            East.MapMapContainer.SpanX = 200f;
            East.MapMapContainer.SpanY = 200f;
            PlayerCharacter Cursor = new PlayerCharacter(null, West, new Vector3(190f, 4f, 10f));
            ChunkCross.Maps = new List<MapInfo> { West, East };

            bool Placed = ChunkCross.MoveCursor(Cursor, 210f, 10f, 4f);

            Assert.IsTrue(Placed);
            Assert.AreSame(East, Cursor.SharedMapContex.CurrentMapInfo);
            Assert.AreEqual(10f, Cursor.Position.X, 0.01f);
            Assert.AreEqual(10f, Cursor.Position.Z, 0.01f);
            Assert.IsFalse(EastMap.ListCharacter.Contains(Cursor));
        }

        [TestMethod]
        public void TheCursorStepsOntoAChunkBeforeThatChunkIsAwake()
        {
            LifeSimMap WestMap = new LifeSimMap();
            MapInfo West = new MapInfo(new MapContainer("West", WestMap), null, null);
            MapInfo East = new MapInfo(new MapContainer("East", null), null, null);
            West.MapMapContainer.SpanX = 200f;
            West.MapMapContainer.SpanY = 200f;
            East.MapMapContainer.OffsetX = 200;
            East.MapMapContainer.SpanX = 200f;
            East.MapMapContainer.SpanY = 200f;
            PlayerCharacter Cursor = new PlayerCharacter(null, West, new Vector3(190f, 4f, 10f));
            ChunkCross.Maps = new List<MapInfo> { West, East };

            bool Placed = ChunkCross.MoveCursor(Cursor, 210f, 10f, 4f);

            Assert.IsTrue(Placed);
            Assert.AreSame(East, Cursor.SharedMapContex.CurrentMapInfo);
            Assert.AreEqual(10f, Cursor.Position.X, 0.01f);
            Assert.AreEqual(10f, Cursor.Position.Z, 0.01f);
        }

        [TestMethod]
        public void AModelOnTheNextChunkIsDrawnAtThatChunksOrigin()
        {
            Vector3 OnEastEdge = ChunkOrigin.ModelWorld(new Vector3(8f, 40f, 0f), new Vector3(200f, 0f, 0f));

            Assert.AreEqual(208f, OnEastEdge.X, 0.01f);
            Assert.AreEqual(40f, OnEastEdge.Z, 0.01f);
            Assert.AreEqual(1f, OnEastEdge.Y, 0.01f);
        }

        static MapInfo Named(List<MapInfo> Maps, string Name)
        {
            for (int i = 0; i < Maps.Count; i++)
            {
                if (Maps[i].MapMapContainer.AreaName == Name)
                {
                    return Maps[i];
                }
            }

            Assert.Fail(Name);
            return null;
        }

        static MapInfo Room(string Name, long X, long Y, float Span)
        {
            MapInfo Map = new MapInfo(new MapContainer(Name, null), null, null);
            Map.MapMapContainer.OffsetX = X;
            Map.MapMapContainer.OffsetY = Y;
            Map.MapMapContainer.SpanX = Span;
            Map.MapMapContainer.SpanY = Span;
            return Map;
        }

        static void Join(MapInfo A, MapInfo B)
        {
            ChunkHop Hop = new ChunkHop();
            Hop.Name = "Border";
            Hop.FromChunk = A;
            Hop.ToChunk = B;
            Hop.IsOpen = true;
            A.ListLink.Add(Hop);
            B.ListLink.Add(Hop.Reversed());
        }

        static Vector2 OffsetOf(List<MapInfo> Maps, string Name)
        {
            for (int i = 0; i < Maps.Count; i++)
            {
                if (Maps[i].MapMapContainer.AreaName == Name)
                {
                    return new Vector2(Maps[i].MapMapContainer.OffsetX, Maps[i].MapMapContainer.OffsetY);
                }
            }

            Assert.Fail(Name);
            return Vector2.Zero;
        }

        static int Occupied(List<List<LocalDevice>> Seats)
        {
            int Count = 0;
            for (int i = 0; i < Seats.Count; i++)
            {
                if (Seats[i].Count > 0)
                {
                    Count++;
                }
            }

            return Count;
        }
    }
}
