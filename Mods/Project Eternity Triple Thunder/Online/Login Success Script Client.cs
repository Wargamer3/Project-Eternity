using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class LoginSuccessScriptClient : OnlineScript
    {
        private delegate void SafeCallDelegate(IOnlineConnection Host);

        public const string ScriptName = "Login Success";

        private readonly Loby Owner;
        private string PlayerID;
        private byte[] PlayerInfo;

        public LoginSuccessScriptClient(Loby Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new LoginSuccessScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            PlayerManager.OnlinePlayerID = PlayerID;
            PlayerManager.ListLocalPlayer.Add(new Player(PlayerManager.OnlinePlayerID, PlayerManager.OnlinePlayerName, Player.PlayerTypes.Online, false, 0));
            ByteReader BR = new ByteReader(PlayerInfo);

            PlayerManager.OnlinePlayerName = BR.ReadString();
            PlayerManager.OnlinePlayerLevel = BR.ReadInt32();

            BR.Clear();
            Owner.IdentifyToCommunicationServer(PlayerManager.OnlinePlayerName, PlayerInfo);
            Owner.AskForPlayerList();
        }

        protected override void Read(OnlineReader Sender)
        {
            PlayerID = Sender.ReadString();
            PlayerInfo = Sender.ReadByteArray();
        }
    }
}
