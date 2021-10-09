using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class FriendListScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Friend List";

        private readonly Lobby Owner;
        private readonly CommunicationClient OnlineCommunicationClient;

        private Player[] ArrayLobbyFriend;

        public FriendListScriptClient(CommunicationClient OnlineCommunicationClient, Lobby Owner)
            : base(ScriptName)
        {
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new FriendListScriptClient(OnlineCommunicationClient, Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            OnlineCommunicationClient.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            Owner.PopulateFriends(ArrayLobbyFriend);
        }

        protected override void Read(OnlineReader Sender)
        {
            int ListPlayerNameCount = Sender.ReadInt32();
            ArrayLobbyFriend = new Player[ListPlayerNameCount];
            for (int P = 0; P < ListPlayerNameCount; ++P)
            {
                string PlayerID = Sender.ReadString();
                byte[] ArrayPlayerInfo = Sender.ReadByteArray();
                ByteReader BR = new ByteReader(ArrayPlayerInfo);

                ArrayLobbyFriend[P] = new Player(PlayerID, BR.ReadString(), Player.PlayerTypes.Online, 0);
                ArrayLobbyFriend[P].Level = BR.ReadInt32();

                BR.Clear();
            }
        }
    }
}
