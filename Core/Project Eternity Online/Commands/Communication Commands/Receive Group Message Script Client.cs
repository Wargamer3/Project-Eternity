using System;

namespace ProjectEternity.Core.Online
{
    public class ReceiveGroupMessageScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Receive Group Message";

        private readonly CommunicationClient OnlineCommunicationClient;

        private string Source;
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
            OnlineCommunicationClient.Chat.AddMessage(Source, Message, (ChatManager.MessageColors)MessageColor);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            Source = Sender.ReadString();
            Message = Sender.ReadString();
            MessageColor = Sender.ReadByte();
        }
    }
}
