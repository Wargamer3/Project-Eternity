using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.UnitTests.AI
{
    [TestClass]
    public class AITests
    {
        [TestMethod]
        public void TestCharacterEatWhenHungry()
        {
            GameTime gameTime = new GameTime(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(16), false);
            NavMap TestMap = new NavMap();

            PolygonMesh Country1Mesh = new PolygonMesh(new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1000, 0), new Vector3(1000, 0, 0), new Vector3(1000, 1000, 0) });
            PolygonMesh Country2Mesh = new PolygonMesh(new Vector3[] { new Vector3(1000, 0, 0), new Vector3(1000, 1000, 0), new Vector3(2000, 0, 0), new Vector3(2000, 1000, 0) });
            PolygonMesh City1Mesh = new PolygonMesh(new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 100, 0), new Vector3(100, 0, 0), new Vector3(100, 100, 0) });
            PolygonMesh City2Mesh = new PolygonMesh(new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 100, 0), new Vector3(100, 0, 0), new Vector3(100, 100, 0) });

            MapContainer Country1 = new MapContainer("Country1", Country1Mesh);
            MapContainer Country2 = new MapContainer("Country2", Country2Mesh);
            MapContainer City1 = new MapContainer("City1", City1Mesh);
            MapContainer City2 = new MapContainer("City2", City2Mesh);
            MapContainer Restaurant = new MapContainer("Restaurant", null);
            MapContainer Road = new MapContainer("Road", null);
            MapContainer House = new MapContainer("House", null);
            MapContainer CharacterHouse = new MapContainer("House", null);

            MapInfo MapCountry1Info = new MapInfo(Country1, null, null);
            MapInfo MapCountry2Info = new MapInfo(Country2, null, null);

            MapCountry1Info.AddNeighbour(MapCountry2Info);
            MapCountry2Info.AddNeighbour(MapCountry1Info);

            TestMap.AddMapContainer(MapCountry1Info);
            TestMap.AddMapContainer(MapCountry2Info);

            MapInfo CharacterCountry1Info = new MapInfo(Country1, new Vector3[] { new Vector3(0, 500, 0), new Vector3(1000, 500, 0), new Vector3(500, 0, 0), new Vector3(500, 1000, 0) }, new Vector3[] { });
            MapInfo CharacterCountry2Info = new MapInfo(Country2, new Vector3[] { }, new Vector3[] { new Vector3(1000, 1500, 0), new Vector3(2000, 1500, 0), new Vector3(1500, 1000, 0), new Vector3(1500, 2000, 0) });
            MapInfo CharacterCity1Info = new MapInfo(City1, new Vector3[] { }, new Vector3[] { });
            MapInfo CharacterCity2Info = new MapInfo(City2, new Vector3[] { }, new Vector3[] { });
            MapInfo CharacterHouseInfo = new MapInfo(CharacterHouse, new Vector3[] { }, new Vector3[] { });

            CharacterCountry1Info.AddNestedMap(CharacterCity1Info);
            CharacterCountry2Info.AddNestedMap(CharacterCity2Info);
            CharacterCity1Info.AddNestedMap(new MapInfo(Restaurant, new Vector3[] { }, new Vector3[] { }));
            CharacterCity1Info.AddNestedMap(new MapInfo(Road, new Vector3[] { }, new Vector3[] { }));
            CharacterCity1Info.AddNestedMap(new MapInfo(House, new Vector3[] { }, new Vector3[] { }));
            CharacterCity2Info.AddNestedMap(CharacterHouseInfo);

            CharacterCountry1Info.AddNeighbour(CharacterCountry2Info);
            CharacterCountry2Info.AddNeighbour(CharacterCountry1Info);

            Character TestCharacter = new Character(CharacterHouseInfo, new Vector3(0, 0, 0));
            TestCharacter.AddMapKnowledge(CharacterCountry1Info);
            TestCharacter.AddMapKnowledge(CharacterCountry2Info);

            TestCharacter.AddAction(new EatJerkyAction(TestCharacter));
            TestCharacter.AddAction(new GoToRestaurantAction(TestCharacter));
            TestCharacter.AddAction(new WalkToPositionAction(TestCharacter));
            TestCharacter.AddAction(new RunToPositionAction(TestCharacter));

            KnowledgeInfo RestaurantKnowledge = new KnowledgeInfo("Pizza Place", new string[] { "Restaurant" }, new Vector3());
            TestCharacter.AddKnowledge(RestaurantKnowledge);

            Assert.IsTrue(TestCharacter.Hunger == 0);
            Assert.IsTrue(TestCharacter.PrimaryGoalReached);

            TestCharacter.Update(gameTime);
            Assert.IsFalse(TestCharacter.PrimaryGoalReached);

            //Check action stack, get up, move, get key, unlock door, open door, close door, lock door, walk, open door, close door, order, 

            Assert.IsTrue(TestCharacter.Hunger > 0);
            Assert.IsTrue(TestCharacter.PrimaryGoalReached);
        }
    }
}
