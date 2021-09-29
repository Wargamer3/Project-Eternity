using System;
using System.Globalization;

namespace ProjectEternity.Core.Online
{
    public class ReceiveGlobalMessageScriptServer : OnlineScript
    {
        private readonly ChatManager.ChatMessage NewMessage;

        public ReceiveGlobalMessageScriptServer(ChatManager.ChatMessage NewMessage)
            : base("Receive Global Message")
        {
            this.NewMessage = NewMessage;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
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
