using System;

namespace ProjectEternity.Core.Online
{
    public interface IOnlineGame
    {
        byte[] GetSnapshotData();

        void Update(double ElapsedSeconds);

        void RemoveOnlinePlayer(string PlayerID, IOnlineConnection activePlayer);
    }
}
