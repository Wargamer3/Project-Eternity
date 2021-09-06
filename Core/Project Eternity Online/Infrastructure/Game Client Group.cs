using System;

namespace ProjectEternity.Core.Online
{
    public interface GameClientGroup
    {
        IOnlineGame CurrentGame { get; }
        IRoomInformations Room { get; }

        bool IsRunningSlow();

        GameClientGroup CreateFromTemplate(IRoomInformations Room);
    }
}
