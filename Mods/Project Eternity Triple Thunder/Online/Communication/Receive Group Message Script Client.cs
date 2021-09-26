using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class ReceiveGroupMessageScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Receive Group Message";

        private readonly IMissionSelect MissionSelectScreen;
        private readonly CommunicationClient OnlineCommunicationClient;

        private string Message;
        private byte ColorR;
        private byte ColorG;
        private byte ColorB;

        public ReceiveGroupMessageScriptClient(CommunicationClient OnlineCommunicationClient, IMissionSelect MissionSelectScreen)
            : base(ScriptName)
        {
            this.MissionSelectScreen = MissionSelectScreen;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
        }

        public override OnlineScript Copy()
        {
            return new ReceiveGroupMessageScriptClient(OnlineCommunicationClient, MissionSelectScreen);
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
            MissionSelectScreen.AddMessage(Message, MessageColor);
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
