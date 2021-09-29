using System;
using System.Globalization;

namespace ProjectEternity.Core.Online
{
    public class ReceiveGroupMessageScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Receive Group Message";

        private readonly CommunicationClient OnlineCommunicationClient;

        private string GroupID;
        private string Date;
        private string Message;
        private byte MessageColor;

        public ReceiveGroupMessageScriptClient(CommunicationClient OnlineCommunicationClient)
            : base(ScriptName)
        {
            this.OnlineCommunicationClient = OnlineCommunicationClient;
        }

        public override OnlineScript Copy()
        {
            return new ReceiveGroupMessageScriptClient(OnlineCommunicationClient);
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
            ChatManager.ChatMessage NewChatMessage = new ChatManager.ChatMessage(DateTime.Parse(Date, DateTimeFormatInfo.InvariantInfo), Message, (ChatManager.MessageColors)MessageColor);
            OnlineCommunicationClient.Chat.AddMessage(GroupID, new ChatManager.ChatMessage(DateTime.Parse(Date, DateTimeFormatInfo.InvariantInfo), Message, (ChatManager.MessageColors)MessageColor));
            OnlineCommunicationClient.Chat.SaveMessage(GroupID, NewChatMessage);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            GroupID = Sender.ReadString();
            Date = Sender.ReadString();
            Message = Sender.ReadString();
            MessageColor = Sender.ReadByte();
        }
    }
}
