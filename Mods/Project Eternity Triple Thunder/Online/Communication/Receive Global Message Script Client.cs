using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class ReceiveGlobalMessageScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Receive Global Message";

        private readonly CommunicationClient OnlineCommunicationClient;

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

        protected override void Execute(IOnlineConnection Sender)
        {
            OnlineCommunicationClient.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            OnlineCommunicationClient.Chat.AddMessage("Global", Message, (ChatManager.MessageColors)MessageColor);
        }

        protected override void Read(OnlineReader Sender)
        {
            Message = Sender.ReadString();
            MessageColor = Sender.ReadByte();
        }
    }
}
