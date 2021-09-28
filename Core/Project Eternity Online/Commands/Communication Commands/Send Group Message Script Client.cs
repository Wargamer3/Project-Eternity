using System;

namespace ProjectEternity.Core.Online
{
    public class SendGroupMessageScriptClient : OnlineScript
    {
        private readonly string Source;
        private readonly string Message;
        private readonly ChatManager.MessageColors MessageColor;

        public SendGroupMessageScriptClient(string Source, string Message, ChatManager.MessageColors MessageColor)
            : base("Send Group Message")
        {
            this.Source = Source;
            this.Message = Message;
            this.MessageColor = MessageColor;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(Source);
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
