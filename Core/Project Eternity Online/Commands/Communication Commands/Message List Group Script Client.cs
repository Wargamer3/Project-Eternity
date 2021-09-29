using System;
using System.Collections.Generic;
using System.Globalization;

namespace ProjectEternity.Core.Online
{
    public class MessageListGroupScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Message List Group";

        private readonly CommunicationClient OnlineCommunicationClient;

        private string GroupID;
        public readonly List<ChatManager.ChatMessage> ListChatHistory;

        public MessageListGroupScriptClient(CommunicationClient OnlineCommunicationClient)
            : base(ScriptName)
        {
            this.OnlineCommunicationClient = OnlineCommunicationClient;

            ListChatHistory = new List<ChatManager.ChatMessage>();
        }

        public override OnlineScript Copy()
        {
            return new MessageListGroupScriptClient(OnlineCommunicationClient);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            OnlineCommunicationClient.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            OnlineCommunicationClient.Chat.InsertMessages(GroupID, ListChatHistory);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            GroupID = Sender.ReadString();
            int MessageCount = Sender.ReadInt32();
            for (int M = 0; M < MessageCount; ++M)
            {
                string Date = Sender.ReadString();
                string Message = Sender.ReadString();
                byte MessageColor = Sender.ReadByte();

                ListChatHistory.Add(new ChatManager.ChatMessage(DateTime.Parse(Date, DateTimeFormatInfo.InvariantInfo), Message, (ChatManager.MessageColors)MessageColor));
            }
        }
    }
}
