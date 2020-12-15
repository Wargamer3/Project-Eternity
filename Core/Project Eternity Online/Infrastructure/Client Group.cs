using System;

namespace ProjectEternity.Core.Online
{
    public interface ClientGroup
    {
        IOnlineGame CurrentGame { get; }
        IRoomInformations Room { get; }

        bool IsRunningSlow();

        ClientGroup CreateFromTemplate(IRoomInformations Room);
    }
}
