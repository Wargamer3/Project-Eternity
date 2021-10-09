using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMap
    {
        public abstract byte[] GetSnapshotData();
        public abstract void Update(double ElapsedSeconds);
        public abstract void RemoveOnlinePlayer(string PlayerID, IOnlineConnection ActivePlayer);
        public abstract void Load(byte[] ArrayGameData);
        public abstract GameScreen GetMultiplayerScreen();

        public abstract void AddLocalPlayer(BattleMapPlayer NewPlayer);

        public abstract void Save(string FilePath);

        public abstract void SaveTemporaryMap();

        public abstract BattleMap LoadTemporaryMap(BinaryReader BR);

        public abstract BattleMap GetNewMap(int GameMode);

        public abstract string GetMapType();

        public abstract bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement);
    }
}
