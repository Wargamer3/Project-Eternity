using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMap
    {
        public abstract GameScreen GetMultiplayerScreen();

        public abstract void Save(string FilePath);

        public abstract void SaveTemporaryMap();

        public abstract BattleMap LoadTemporaryMap(BinaryReader BR);

        public abstract BattleMap GetNewMap(string BattleMapPath, int GameMode, List<Squad> ListSpawnSquad);

        public abstract string GetMapType();

        public abstract bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement);
    }
}
