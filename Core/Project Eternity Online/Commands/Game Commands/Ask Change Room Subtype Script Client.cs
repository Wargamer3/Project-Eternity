using System;

namespace ProjectEternity.Core.Online
{
    public class AskChangeRoomSubtypeScriptClient : OnlineScript
    {
        private readonly string RoomSubtype;

        public AskChangeRoomSubtypeScriptClient(string RoomSubtype)
            : base("Ask Change Room Subtype")
        {
            this.RoomSubtype = RoomSubtype;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(RoomSubtype);
        }

        protected internal override void Execute(IOnlineConnection Host)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
