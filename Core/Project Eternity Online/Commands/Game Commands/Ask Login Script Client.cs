using System;

namespace ProjectEternity.Core.Online
{
    public class AskLoginScriptClient : OnlineScript
    {
        private readonly string Login;
        private readonly string Password;

        public AskLoginScriptClient(string Login, string Password)
            : base("Ask Login")
        {
            this.Login = Login;
            this.Password = Password;
        }

        public override OnlineScript Copy()
        {
            return new AskLoginScriptClient(Login, Password);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(Login);
            WriteBuffer.AppendString(Password);
        }

        protected internal override void Execute(IOnlineConnection Host)
        {
        }

        protected internal override void Read(OnlineReader Sender)
        {
        }
    }
}
