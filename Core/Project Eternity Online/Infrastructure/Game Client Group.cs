using System;

namespace ProjectEternity.Core.Online
{
    public interface GameClientGroup
    {
        IOnlineGame CurrentGame { get; }
        IRoomInformations Room { get; }
        bool IsGameReady { get; set; }

        bool IsRunningSlow();

        GameClientGroup CreateFromTemplate(IRoomInformations Room);
    }
}
