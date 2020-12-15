using System;

namespace ProjectEternity.Core.Online
{
    public class AskLoginScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Login";

        private readonly Server Owner;

        private string Login;
        private string Password;

        public AskLoginScriptServer(Server Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskLoginScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
            PlayerPOCO PlayerInfo = Owner.Database.LogInPlayer(Login, Password, Owner.IP, Owner.Port);
            ActivePlayer.ID = PlayerInfo.ID;
            ActivePlayer.Name = PlayerInfo.Name;
            ActivePlayer.Send(new LoginSuccessScriptServer(PlayerInfo));
        }

        protected internal override void Read(OnlineReader Sender)
        {
            Login = Sender.ReadString();
            Password = Sender.ReadString();
        }
    }
}
