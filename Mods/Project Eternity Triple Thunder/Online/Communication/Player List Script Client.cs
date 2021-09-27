using System;
using System.IO;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class PlayerListScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Player List";

        private readonly Loby Owner;
        private readonly CommunicationClient OnlineCommunicationClient;

        private Player[] ArrayPlayerName;

        public PlayerListScriptClient(CommunicationClient OnlineCommunicationClient, Loby Owner)
            : base(ScriptName)
        {
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new PlayerListScriptClient(OnlineCommunicationClient, Owner);
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
            Owner.PopulatePlayerNames(ArrayPlayerName);
        }

        protected override void Read(OnlineReader Sender)
        {
            int ListPlayerNameCount = Sender.ReadInt32();
            ArrayPlayerName = new Player[ListPlayerNameCount];
            for (int P = 0; P < ListPlayerNameCount; ++P)
            {
                byte[] ArrayPlayerInfo = Sender.ReadByteArray();
                ByteReader BR = new ByteReader(ArrayPlayerInfo);

                ArrayPlayerName[P] = new Player(null, BR.ReadString(), Player.PlayerTypes.Online, true, 0);
                ArrayPlayerName[P].Level = BR.ReadInt32();

                BR.Clear();
            }
        }
    }
}
