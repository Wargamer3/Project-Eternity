using System;

namespace ProjectEternity.Core.Online
{
    public class AskChangePlayerTypeScriptClient : OnlineScript
    {
        private readonly string PlayerType;

        public AskChangePlayerTypeScriptClient(string PlayerType)
            : base("Ask Change Player Type")
        {
            this.PlayerType = PlayerType;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(PlayerType);
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
