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
        private string PlayerName;

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
            PlayerManager.OnlinePlayerName = PlayerName;
            PlayerManager.ListLocalPlayer.Add(new Player(PlayerManager.OnlinePlayerID, PlayerManager.OnlinePlayerName, Player.PlayerTypes.Online, false, 0));
        }

        protected override void Read(OnlineReader Sender)
        {
            PlayerID = Sender.ReadString();
            PlayerName = Sender.ReadString();
        }
    }
}
