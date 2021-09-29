using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class MessageListGroupScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Message List Group";

        private readonly CommunicationClient OnlineCommunicationClient;

        private string Source;
        private Dictionary<string, ChatManager.MessageColors> DicChatHistory;

        public MessageListGroupScriptClient(CommunicationClient OnlineCommunicationClient)
            : base(ScriptName)
        {
            this.OnlineCommunicationClient = OnlineCommunicationClient;

            DicChatHistory = new Dictionary<string, ChatManager.MessageColors>();
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
            OnlineCommunicationClient.Chat.InsertMessages(Source, DicChatHistory);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            Source = Sender.ReadString();
            int MessageCount = Sender.ReadInt32();
            for (int M = 0; M < MessageCount; ++M)
            {
                string Message = Sender.ReadString();
                byte MessageColor = Sender.ReadByte();

                DicChatHistory.Add(Message, (ChatManager.MessageColors)MessageColor);
            }
        }
    }
}
