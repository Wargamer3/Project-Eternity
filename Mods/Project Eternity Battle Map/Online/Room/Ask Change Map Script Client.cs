using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class AskChangeMapScriptClient : OnlineScript
    {
        private readonly RoomInformations Room;

        public AskChangeMapScriptClient(RoomInformations Room)
            : base("Ask Change Map")
        {
            this.Room = Room;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            Room.WriteSelectedMap(WriteBuffer);
        }

        protected override void Execute(IOnlineConnection Host)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
