using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class ChangeMapScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Map";
        private readonly RoomInformations Room;

        public ChangeMapScriptServer(RoomInformations Room)
            : base(ScriptName)
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

        protected override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
