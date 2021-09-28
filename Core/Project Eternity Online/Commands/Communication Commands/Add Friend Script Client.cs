using System;

namespace ProjectEternity.Core.Online
{
    public class AddFriendScriptClient : OnlineScript
    {
        private readonly string FriendID;

        public AddFriendScriptClient(string FriendID)
            : base("Add Friend")
        {
            this.FriendID = FriendID;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(FriendID);
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
