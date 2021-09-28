using System;

namespace ProjectEternity.Core.Online
{
    public class AddFriendScriptServer : OnlineScript
    {
        public const string ScriptName = "Add Friend";

        private readonly CommunicationServer OnlineCommunicationServer;

        private string FriendID;

        public AddFriendScriptServer(CommunicationServer OnlineCommunicationServer)
            : base(ScriptName)
        {
            this.OnlineCommunicationServer = OnlineCommunicationServer;
        }

        public override OnlineScript Copy()
        {
            return new AddFriendScriptServer(OnlineCommunicationServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            OnlineCommunicationServer.AddFriend(Sender, FriendID);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            FriendID = Sender.ReadString();
        }
    }
}
