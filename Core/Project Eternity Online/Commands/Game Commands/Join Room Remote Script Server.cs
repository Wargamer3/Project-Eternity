using System;

namespace ProjectEternity.Core.Online
{
    public class JoinRoomRemoteScriptServer : OnlineScript
    {
        private readonly string RoomID;
        private readonly string RemoteIP;
        private readonly int RemotePort;

        public JoinRoomRemoteScriptServer(string RoomID, string RemoteIP, int RemotePort)
            : base("Join Room Remote")
        {
            this.RoomID = RoomID;
            this.RemoteIP = RemoteIP;
            this.RemotePort = RemotePort;
        }

        public override OnlineScript Copy()
        {
            return null;
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(RoomID);
            WriteBuffer.AppendString(RemoteIP);
            WriteBuffer.AppendInt32(RemotePort);
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
