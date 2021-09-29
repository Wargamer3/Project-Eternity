using System;
using System.Globalization;

namespace ProjectEternity.Core.Online
{
    public class SendGroupMessageScriptClient : OnlineScript
    {
        private readonly string GroupID;
        private readonly ChatManager.ChatMessage MessageToSend;

        public SendGroupMessageScriptClient(string GroupID, ChatManager.ChatMessage MessageToSend)
            : base("Send Group Message")
        {
            this.GroupID = GroupID;
            this.MessageToSend = MessageToSend;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(GroupID);
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
