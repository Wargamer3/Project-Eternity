using System;
using System.Globalization;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class MessageListGroupScriptServer : OnlineScript
    {
        public const string ScriptName = "Message List Group";
        private readonly string GroupID;
        public readonly List<ChatManager.ChatMessage> ListChatHistory;

        public MessageListGroupScriptServer(string GroupID, List<ChatManager.ChatMessage> ListChatHistory)
            : base(ScriptName)
        {
            this.GroupID = GroupID;
            this.ListChatHistory = ListChatHistory;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(GroupID);
            WriteBuffer.AppendInt32(ListChatHistory.Count);
            foreach (ChatManager.ChatMessage ActiveMessage in ListChatHistory)
            {
                WriteBuffer.AppendString(ActiveMessage.Date.ToString(DateTimeFormatInfo.InvariantInfo));
                WriteBuffer.AppendString(ActiveMessage.Message);
                WriteBuffer.AppendByte((byte)ActiveMessage.MessageColor);
            }
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
