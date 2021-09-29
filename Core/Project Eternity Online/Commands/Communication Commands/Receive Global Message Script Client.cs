using System;
using System.Globalization;

namespace ProjectEternity.Core.Online
{
    public class ReceiveGlobalMessageScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Receive Global Message";

        private readonly CommunicationClient OnlineCommunicationClient;

        private string Date;
        private string Message;
        private byte MessageColor;

        public ReceiveGlobalMessageScriptClient(CommunicationClient OnlineCommunicationClient)
            : base(ScriptName)
        {
            this.OnlineCommunicationClient = OnlineCommunicationClient;
        }

        public override OnlineScript Copy()
        {
            return new ReceiveGlobalMessageScriptClient(OnlineCommunicationClient);
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
            OnlineCommunicationClient.Chat.AddMessage("Global", new ChatManager.ChatMessage(DateTime.Parse(Date, DateTimeFormatInfo.InvariantInfo), Message, (ChatManager.MessageColors)MessageColor));
        }

        protected internal override void Read(OnlineReader Sender)
        {
            Date = Sender.ReadString();
            Message = Sender.ReadString();
            MessageColor = Sender.ReadByte();
        }
    }
}
