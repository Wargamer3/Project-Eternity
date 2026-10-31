using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimNoiseTests
    {
        [TestMethod]
        public void StandingStillIsSilentAndAWalkIsLouderThanACrouch()
        {
            Assert.AreEqual(0f, Noise.LoudnessOf(0f, 30f, Pose.Stand, 1f), 0.001f);
            Assert.AreEqual(0f, Noise.RadiusOf(0f), 0.001f);

            float Walk = Noise.LoudnessOf(30f, 30f, Pose.Stand, 1f);
            float Run = Noise.LoudnessOf(52.5f, 30f, Pose.Stand, 1f);
            float Crouch = Noise.LoudnessOf(30f, 30f, Pose.Crouch, 1f);
            float Crawl = Noise.LoudnessOf(30f, 30f, Pose.Crawl, 1f);
            Assert.AreEqual(45f, Walk, 0.001f);
            Assert.AreEqual(28.8f, Noise.RadiusOf(Walk), 0.001f);
            Assert.AreEqual(78.75f, Run, 0.001f);
            Assert.IsTrue(Crouch < Walk);
            Assert.AreEqual(20.25f, Crouch, 0.001f);
            Assert.IsTrue(Crawl < Crouch);
        }

        [TestMethod]
        public void CarpetAndSnowAreQuieterAndMetalAndGlassAreLouder()
        {
            float Hardwood = Noise.LoudnessOf(30f, 30f, Pose.Stand, Noise.SurfaceOf("Hardwood"));
            float Carpet = Noise.LoudnessOf(30f, 30f, Pose.Stand, Noise.SurfaceOf("Carpet"));
            float Snow = Noise.LoudnessOf(30f, 30f, Pose.Stand, Noise.SurfaceOf("Snow"));
            float Metal = Noise.LoudnessOf(30f, 30f, Pose.Stand, Noise.SurfaceOf("Metal"));
            float Glass = Noise.LoudnessOf(30f, 30f, Pose.Stand, Noise.SurfaceOf("Broken Glass"));
            Assert.AreEqual(45f, Hardwood, 0.001f);
            Assert.AreEqual(22.5f, Carpet, 0.001f);
            Assert.AreEqual(15.75f, Snow, 0.001f);
            Assert.IsTrue(Snow < Carpet);
            Assert.AreEqual(78.75f, Metal, 0.001f);
            Assert.AreEqual(99f, Glass, 0.001f);
            Assert.IsTrue(Glass > Metal);
            Assert.AreEqual(100f, Noise.LoudnessOf(200f, 30f, Pose.Stand, Noise.SurfaceOf("Broken Glass")), 0.001f);
        }

        [TestMethod]
        public void AFloorPlateBeatsBareGroundAndOutdoorSnowQuietsIt()
        {
            BuildingItem Plate = new BuildingItem();
            Plate.Kind = BuildingKind.Floor;
            Plate.Floor = 0;
            Plate.Interaction = "Metal";
            Plate.Width = 32f;
            Plate.Length = 32f;
            Plate.Position = new Vector3(16f, 16f, 0f);
            List<BuildingItem> Buildings = new List<BuildingItem>();
            Buildings.Add(Plate);

            Outlook Heavy = new Outlook();
            Heavy.Period = Period(TimePeriod.WeatherTypes.HeavySnow);
            Assert.AreEqual(1.75f, Noise.SurfaceAt(Buildings, 16f, 16f, 0f, Heavy, false), 0.001f);
            Assert.AreEqual(0.35f, Noise.SurfaceAt(Buildings, 80f, 16f, 0f, Heavy, false), 0.001f);
            Assert.AreEqual(1f, Noise.SurfaceAt(Buildings, 80f, 16f, 0f, Heavy, true), 0.001f);

            BuildingItem Packed = new BuildingItem();
            Packed.Kind = BuildingKind.Floor;
            Packed.Floor = 0;
            Packed.Interaction = "Snow";
            Packed.Width = 32f;
            Packed.Length = 32f;
            Packed.Position = new Vector3(64f, 64f, 0f);
            Buildings.Add(Packed);
            Outlook Clear = new Outlook();
            Clear.Period = Period(TimePeriod.WeatherTypes.Regular);
            Assert.AreEqual(0.35f, Noise.SurfaceAt(Buildings, 64f, 64f, 0f, Clear, true), 0.001f);

            Outlook Fading = new Outlook();
            Fading.Period = Period(TimePeriod.WeatherTypes.LightSnow);
            Fading.Dying = Period(TimePeriod.WeatherTypes.HeavySnow);
            Fading.Strength = 0.5f;
            Assert.AreEqual(0.425f, Noise.SnowOnGround(Fading, false), 0.001f);
            Assert.AreEqual(1f, Noise.SnowOnGround(Fading, true), 0.001f);
        }

        [TestMethod]
        public void AStormRaisesTheFloorAndARoofMufflesIt()
        {
            float Storm = Noise.FloorOf(TimePeriod.WeatherTypes.Thunderstorm);
            float Rain = Noise.FloorOf(TimePeriod.WeatherTypes.HeavyRain);
            float Clear = Noise.FloorOf(TimePeriod.WeatherTypes.Regular);
            Assert.IsTrue(Storm > Rain);
            Assert.IsTrue(Rain > Clear);
            Assert.AreEqual(78f, Storm, 0.001f);
            Assert.AreEqual(58f, Rain, 0.001f);
            Assert.AreEqual(8f, Clear, 0.001f);

            TimePeriod Now = Period(TimePeriod.WeatherTypes.Thunderstorm);
            TimePeriod Was = Period(TimePeriod.WeatherTypes.Regular);
            Assert.AreEqual(43f, Noise.Blend(Now, Was, 0.5f), 0.001f);
            Assert.AreEqual(78f, Noise.Blend(Now, Was, 0f), 0.001f);
            Assert.AreEqual(19.5f, Noise.Muffle(Storm, true), 0.001f);
            Assert.AreEqual(6f, Noise.Muffle(Clear, true), 0.001f);
            Assert.AreEqual(Storm, Noise.Muffle(Storm, false), 0.001f);
        }

        [TestMethod]
        public void HearingStopsWhereTheNoiseDropsUnderTheFloor()
        {
            float Loudness = Noise.LoudnessOf(30f, 30f, Pose.Stand, 1f);
            float Radius = Noise.RadiusOf(Loudness);
            Assert.AreEqual(0f, Noise.HearingOf(Loudness, Radius, Noise.FloorOf(TimePeriod.WeatherTypes.Thunderstorm)), 0.001f);
            float Heard = Noise.HearingOf(Loudness, Radius, Noise.FloorOf(TimePeriod.WeatherTypes.Regular));
            Assert.IsTrue(Heard > 0f);
            Assert.IsTrue(Heard < Radius);

            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Noise.Stamp(Who, 30f);
            Assert.AreEqual(45f, Who.Noise.Loudness, 0.001f);
            Assert.AreEqual(28.8f, Who.Noise.Radius, 0.001f);
            Assert.AreEqual(8f, Who.Noise.Background, 0.001f);
            Assert.IsTrue(Who.Noise.HearingRadius > 0f);
            Assert.IsTrue(Who.Noise.HearingRadius < Who.Noise.Radius);
        }

        static TimePeriod Period(TimePeriod.WeatherTypes Weather)
        {
            TimePeriod Period = new TimePeriod(Weather.ToString());
            Period.WeatherType = Weather;
            return Period;
        }
    }
}
