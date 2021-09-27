using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class ReceiveGlobalMessageScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Receive Global Message";

        private readonly Lobby Owner;
        private readonly CommunicationClient OnlineCommunicationClient;

        private string Message;
        private byte ColorR;
        private byte ColorG;
        private byte ColorB;

        public ReceiveGlobalMessageScriptClient(CommunicationClient OnlineCommunicationClient, Lobby Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
        }

        public override OnlineScript Copy()
        {
            return new ReceiveGlobalMessageScriptClient(OnlineCommunicationClient, Owner);
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
            Color MessageColor = Color.FromNonPremultiplied(ColorR, ColorG, ColorB, 255);
            Owner.AddMessage(Message, MessageColor);
        }

        protected override void Read(OnlineReader Sender)
        {
            Message = Sender.ReadString();
            ColorR = Sender.ReadByte();
            ColorG = Sender.ReadByte();
            ColorB = Sender.ReadByte();
        }
    }
}
