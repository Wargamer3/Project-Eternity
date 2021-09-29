using System;
using System.Globalization;

namespace ProjectEternity.Core.Online
{
    public class ReceiveGroupMessageScriptServer : OnlineScript
    {
        private readonly string Source;
        private readonly ChatManager.ChatMessage NewMessage;

        public ReceiveGroupMessageScriptServer(string Source, ChatManager.ChatMessage NewMessage)
            : base("Receive Group Message")
        {
            this.Source = Source;
            this.NewMessage = NewMessage;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(Source);
            WriteBuffer.AppendString(NewMessage.Date.ToString(DateTimeFormatInfo.InvariantInfo));
            WriteBuffer.AppendString(NewMessage.Message);
            WriteBuffer.AppendByte((byte)NewMessage.MessageColor);
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
