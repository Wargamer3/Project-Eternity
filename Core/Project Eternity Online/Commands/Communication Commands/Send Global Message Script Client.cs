using System;

namespace ProjectEternity.Core.Online
{
    public class SendGlobalMessageScriptClient : OnlineScript
    {
        private readonly string Message;
        private readonly ChatManager.MessageColors MessageColor;

        public SendGlobalMessageScriptClient(string Message, ChatManager.MessageColors MessageColor)
            : base("Send Global Message")
        {
            this.Message = Message;
            this.MessageColor = MessageColor;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(Message);
            WriteBuffer.AppendByte((byte)MessageColor);
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
