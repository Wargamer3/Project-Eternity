using System;
using System.Globalization;

namespace ProjectEternity.Core.Online
{
    public class SendGlobalMessageScriptClient : OnlineScript
    {
        private readonly ChatManager.ChatMessage MessageToSend;

        public SendGlobalMessageScriptClient(ChatManager.ChatMessage MessageToSend)
            : base("Send Global Message")
        {
            this.MessageToSend = MessageToSend;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(MessageToSend.Date.ToString(DateTimeFormatInfo.InvariantInfo));
            WriteBuffer.AppendString(MessageToSend.Message);
            WriteBuffer.AppendByte((byte)MessageToSend.MessageColor);
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
