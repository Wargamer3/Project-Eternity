using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class ClientInfoScriptClient : OnlineScript
    {
        public const string ScriptName = "Client Info";

        private readonly PlayerInfoScreen Owner;
        private readonly CommunicationClient OnlineCommunicationClient;

        private byte PlayerRanking;
        private byte PlayerLicense;
        private string PlayerGuild;

        public ClientInfoScriptClient(CommunicationClient OnlineCommunicationClient, PlayerInfoScreen Owner)
            : base(ScriptName)
        {
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new ClientInfoScriptClient(OnlineCommunicationClient, Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            Owner.ActivePlayer.Ranking = PlayerRanking;
            Owner.ActivePlayer.License = PlayerLicense;
            Owner.ActivePlayer.Guild = PlayerGuild;
        }

        protected override void Read(OnlineReader Sender)
        {
            byte[] ArrayPlayerInfo = Sender.ReadByteArray();
            ByteReader BR = new ByteReader(ArrayPlayerInfo);

            PlayerRanking = BR.ReadByte();
            PlayerLicense = BR.ReadByte();
            PlayerGuild = BR.ReadString();

            BR.Clear();
        }
    }
}
