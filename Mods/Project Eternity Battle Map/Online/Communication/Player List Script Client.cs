using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class PlayerListScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Player List";

        private readonly Lobby Owner;
        private readonly CommunicationClient OnlineCommunicationClient;

        private OnlinePlayer[] ArrayLobbyPlayer;

        public PlayerListScriptClient(CommunicationClient OnlineCommunicationClient, Lobby Owner)
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
            Owner.PopulatePlayers(ArrayLobbyPlayer);
        }

        protected override void Read(OnlineReader Sender)
        {
            int ListPlayerNameCount = Sender.ReadInt32();
            ArrayLobbyPlayer = new OnlinePlayer[ListPlayerNameCount];
            for (int P = 0; P < ListPlayerNameCount; ++P)
            {
                string PlayerID = Sender.ReadString();
                byte[] ArrayPlayerInfo = Sender.ReadByteArray();
                ByteReader BR = new ByteReader(ArrayPlayerInfo);

                ArrayLobbyPlayer[P] = new OnlinePlayer(PlayerID, BR.ReadString(), OnlinePlayer.PlayerTypes.Online, true, 0);
                ArrayLobbyPlayer[P].Level = BR.ReadInt32();

                BR.Clear();
            }
        }
    }
}
