﻿using System;

namespace ProjectEternity.Core.Online
{
    public class SendGlobalMessageScriptServer : OnlineScript
    {
        public const string ScriptName = "Send Global Message";

        private readonly CommunicationServer OnlineServer;

        private string Message;
        private byte MessageColor;

        public SendGlobalMessageScriptServer(CommunicationServer OnlineServer)
            : base(ScriptName)
        {
            this.OnlineServer = OnlineServer;
        }

        public override OnlineScript Copy()
        {
            return new SendGlobalMessageScriptServer(OnlineServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            string FinalMessage = Sender.Name + " : " + Message;
            OnlineServer.SendGlobalMessage(FinalMessage, (ChatManager.MessageColors)MessageColor);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            Message = Sender.ReadString();
            MessageColor = Sender.ReadByte();
        }
    }
}
