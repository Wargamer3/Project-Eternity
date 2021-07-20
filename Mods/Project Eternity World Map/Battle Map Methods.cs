using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public partial class WorldMap
    {
        public Terrain GetTerrain(int X, int Y, int LayerIndex)
        {
            return ListLayer[LayerIndex].ArrayTerrain[X, Y];
        }

        public override void SaveTemporaryMap()
        {
            throw new NotImplementedException();
        }

        public override BattleMap LoadTemporaryMap(BinaryReader BR)
        {
            throw new NotImplementedException();
        }

        public override BattleMap GetNewMap(string BattleMapPath, int GameMode, List<Squad> ListSpawnSquad)
        {
            return new WorldMap(BattleMapPath, GameMode, ListSpawnSquad);
        }

        public override GameScreen GetMultiplayerScreen()
        {
            throw new NotImplementedException();
        }

        public override string GetMapType()
        {
            return "World Map";
        }
    }
}
