using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimWaterTests
    {
        [TestMethod]
        public void TwoClicksKeepTheRiverAndTheNextPairMakesAnother()
        {
            WaterTool Tool = new WaterTool();
            List<WaterVolume> Volumes = new List<WaterVolume>();
            Vector3 Start = new Vector3(4f, 8f, 2f);
            Vector3 End = new Vector3(84f, 8f, 2f);
            Assert.IsNull(Tool.Confirm(Volumes, Start, true));
            Assert.IsTrue(Tool.HasStart);

            WaterVolume River = Tool.Confirm(Volumes, End, true);
            Assert.AreEqual(Start, River.Start);
            Assert.AreEqual(End, River.End);
            Assert.IsFalse(Tool.HasStart);
            Assert.AreEqual(1, Volumes.Count);

            Tool.Cancel();
            Assert.IsFalse(Tool.HasStart);
            Assert.IsNull(Tool.Confirm(Volumes, new Vector3(4f, 40f, 2f), true));
            WaterVolume Second = Tool.Confirm(Volumes, new Vector3(84f, 40f, 2f), true);
            Assert.IsNotNull(Second);
            Assert.AreEqual(2, Volumes.Count);
            Assert.IsTrue(WaterPlace.RemoveAt(Volumes, new Vector3(40f, 8f, 2f)));
            Assert.AreEqual(1, Volumes.Count);
        }

        [TestMethod]
        public void FlowAtBlendsACrossingAndFadesARiverIntoAPool()
        {
            List<WaterVolume> Crossing = new List<WaterVolume>();
            Crossing.Add(WaterPlace.MakeRiver(new Vector3(0f, 0f, 0f), new Vector3(100f, 0f, 0f), WaterLook.Tropical, false));
            Crossing.Add(WaterPlace.MakeRiver(new Vector3(50f, -50f, 0f), new Vector3(50f, 50f, 0f), WaterLook.Tropical, false));
            Vector2 Blended = WaterPlace.FlowAt(Crossing, new Vector3(50f, 0f, 0f));
            Assert.AreEqual(WaterPlace.Speed * 0.5f, Blended.X, 0.01f);
            Assert.AreEqual(WaterPlace.Speed * 0.5f, Blended.Y, 0.01f);
            Vector2 Arm = WaterPlace.FlowAt(Crossing, new Vector3(10f, 0f, 0f));
            Assert.AreEqual(WaterPlace.Speed, Arm.X, 0.01f);
            Assert.AreEqual(0f, Arm.Y, 0.01f);

            List<WaterVolume> Same = new List<WaterVolume>();
            Same.Add(WaterPlace.MakeRiver(new Vector3(0f, 0f, 0f), new Vector3(100f, 0f, 0f), WaterLook.Tropical, false));
            Same.Add(WaterPlace.MakeRiver(new Vector3(0f, 0f, 0f), new Vector3(100f, 0f, 0f), WaterLook.Tropical, false));
            Vector2 Doubled = WaterPlace.FlowAt(Same, new Vector3(40f, 0f, 0f));
            Assert.AreEqual(WaterPlace.Speed, Doubled.X, 0.01f);

            List<WaterVolume> Mouth = new List<WaterVolume>();
            Mouth.Add(WaterPlace.MakeRiver(new Vector3(-40f, 0f, 0f), new Vector3(10f, 0f, 0f), WaterLook.Tropical, false));
            Mouth.Add(WaterPlace.MakePool(new Vector3(40f, 0f, 0f), WaterLook.Northern));
            Vector2 Faded = WaterPlace.FlowAt(Mouth, new Vector3(0f, 0f, 0f));
            Assert.AreEqual(WaterPlace.Speed * 0.5f, Faded.X, 0.01f);
            Vector2 Still = WaterPlace.FlowAt(Mouth, new Vector3(40f, 0f, 0f));
            Assert.AreEqual(0f, Still.X, 0.01f);
            Assert.AreEqual(0f, Still.Y, 0.01f);
        }

        [TestMethod]
        public void ASharedJoinStoresTheBlendedFlowAndAColorBetweenTheLooks()
        {
            List<WaterVolume> Crossing = new List<WaterVolume>();
            Crossing.Add(WaterPlace.MakeRiver(new Vector3(0f, 0f, 0f), new Vector3(100f, 0f, 0f), WaterLook.Tropical, false));
            Crossing.Add(WaterPlace.MakeRiver(new Vector3(50f, -50f, 0f), new Vector3(50f, 50f, 0f), WaterLook.Northern, false));
            WaterSheet Sheet = WaterMesh.Build(Crossing, null)[0];
            bool FoundFlow = false;
            for (int i = 0; i < Sheet.Vertices.Length; i++)
            {
                WaterVertex Vertex = Sheet.Vertices[i];
                if (Vertex.Flow.X > 1f && Vertex.Flow.Y > 1f)
                {
                    FoundFlow = true;
                    break;
                }
            }

            Assert.IsTrue(FoundFlow);

            List<WaterVolume> Mouth = new List<WaterVolume>();
            Mouth.Add(WaterPlace.MakeRiver(new Vector3(0f, 0f, 0f), new Vector3(80f, 0f, 0f), WaterLook.Tropical, false));
            Mouth.Add(WaterPlace.MakePool(new Vector3(40f, 0f, 0f), WaterLook.Northern));
            WaterSheet Mixed = WaterMesh.Build(Mouth, null)[0];
            WaterLookStyle Tropical = WaterLookStyle.Of(WaterLook.Tropical);
            WaterLookStyle Northern = WaterLookStyle.Of(WaterLook.Northern);
            bool FoundColor = false;
            for (int i = 0; i < Mixed.Vertices.Length; i++)
            {
                Vector4 Shallow = Mixed.Vertices[i].Shallow;
                if (Shallow.X > Tropical.Shallow.X && Shallow.X < Northern.Shallow.X && Shallow.Y > Tropical.Shallow.Y && Shallow.Y < Northern.Shallow.Y)
                {
                    FoundColor = true;
                    break;
                }
            }

            Assert.IsTrue(FoundColor);
        }

        [TestMethod]
        public void ALightBodyRisesAndDriftsAndAHeavyBodySinks()
        {
            List<WaterVolume> River = new List<WaterVolume>();
            River.Add(WaterPlace.MakeRiver(new Vector3(-20f, 0f, 0f), new Vector3(120f, 0f, 0f), WaterLook.Tropical, false));
            float Height = 20f;
            Vector3 Deep = new Vector3(20f, 0f, -20f);
            WaterShift Light = WaterPlace.Move(River, Deep, Height, WaterPlace.CharacterDensity, false, 1f, null);
            Assert.IsTrue(Light.Position.Z > Deep.Z);
            Assert.AreEqual(0f - WaterPlace.CharacterDensity * Height, Light.Position.Z, 0.01f);
            Assert.IsTrue(Light.Position.X > Deep.X);

            WaterShift Heavy = WaterPlace.Move(River, new Vector3(20f, 0f, 0f), Height, WaterPlace.HeavyDensity, false, 1f, null);
            Assert.AreEqual(-WaterPlace.Depth, Heavy.Position.Z, 0.01f);
            Assert.IsTrue(Heavy.Position.X > 20f);

            List<WaterVolume> Pool = new List<WaterVolume>();
            Pool.Add(WaterPlace.MakePool(new Vector3(0f, 0f, 0f), WaterLook.Cave));
            WaterShift Calm = WaterPlace.Move(Pool, new Vector3(0f, 0f, -20f), Height, WaterPlace.CharacterDensity, false, 1f, null);
            Assert.AreEqual(0f, Calm.Position.X, 0.001f);
            Assert.AreEqual(0f, Calm.Position.Y, 0.001f);
            Assert.IsTrue(Calm.Position.Z > -20f);
        }

        [TestMethod]
        public void WetnessStaysAtTheKneeAndDriesAfterAMinute()
        {
            float Line = 0f;
            float Amount = 0f;
            WaterPlace.Soak(ref Line, ref Amount, true, 16f, 0.1f);
            Assert.AreEqual(16f, Line, 0.001f);
            Assert.AreEqual(1f, Amount, 0.001f);

            WaterPlace.Soak(ref Line, ref Amount, true, 8f, 0.1f);
            Assert.AreEqual(16f, Line, 0.001f);

            WaterPlace.Soak(ref Line, ref Amount, true, 30f, 0.1f);
            Assert.AreEqual(30f, Line, 0.001f);

            WaterPlace.Soak(ref Line, ref Amount, false, 0f, 30f);
            Assert.AreEqual(0.5f, Amount, 0.001f);
            Assert.AreEqual(30f, Line, 0.001f);

            WaterPlace.Soak(ref Line, ref Amount, false, 0f, 30f);
            Assert.AreEqual(0f, Amount, 0.001f);
            Assert.AreEqual(0f, Line, 0.001f);
        }

        [TestMethod]
        public void EnteringAddsOneRingAndMovingExtendsOneWake()
        {
            WaterTracks Tracks = new WaterTracks();
            object Body = new object();
            Tracks.Note(Body, new Vector2(0f, 0f), true, false, 0.016f);
            Assert.AreEqual(1, Tracks.Rings.Count);
            Assert.IsTrue(Tracks.Rings[0].Strength > 0.5f);
            Assert.AreEqual(0, Tracks.Wakes.Count);

            Tracks.Note(Body, new Vector2(0f, 0f), true, true, 0.016f);
            Tracks.Note(Body, new Vector2(20f, 0f), true, true, 0.016f);
            Assert.AreEqual(1, Tracks.Wakes.Count);
            Assert.IsTrue(Tracks.Wakes[0].Points.Count >= 2);
            int Held = Tracks.Wakes[0].Points.Count;
            Vector2 Tail = Tracks.Wakes[0].Points[0];

            Tracks.Note(Body, new Vector2(20f, 0f), true, false, 0.016f);
            Assert.AreEqual(Held, Tracks.Wakes[0].Points.Count);

            Tracks.Note(Body, new Vector2(20f, 0f), true, false, 0.25f);
            Assert.AreEqual(Held - 1, Tracks.Wakes[0].Points.Count);
            Assert.AreNotEqual(Tail, Tracks.Wakes[0].Points[0]);

            Tracks.Note(Body, new Vector2(20f, 0f), true, false, WaterPlace.RingLife);
            Assert.AreEqual(0, Tracks.Rings.Count);

            for (int i = 0; i < WaterPlace.RingCap + 1; i++)
            {
                object Other = new object();
                Tracks.Note(Other, new Vector2(i * 10f, 4f), true, false, 0.01f);
            }

            Assert.AreEqual(WaterPlace.RingCap, Tracks.Rings.Count);
            Assert.AreNotEqual(0f, Tracks.Rings[0].Center.X);
        }
    }
}
