using System;

namespace ProjectEternity.Core.Online
{
    public class SendGroupMessageScriptServer : OnlineScript
    {
        public const string ScriptName = "Send Group Message";

        private readonly CommunicationServer OnlineServer;
        private readonly CommunicationGroup Owner;

        private string Message;
        private byte ColorR;
        private byte ColorG;
        private byte ColorB;

        public SendGroupMessageScriptServer(CommunicationServer OnlineServer, CommunicationGroup Owner)
            : base(ScriptName)
        {
            this.OnlineServer = OnlineServer;
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new SendGroupMessageScriptServer(OnlineServer, Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            string FinalMessage = Sender.Name + " : " + Message;
            foreach (IOnlineConnection ActiveOnlinePlayer in Owner.ListGroupMember)
            {
                ActiveOnlinePlayer.Send(new ReceiveGroupMessageScriptServer(FinalMessage, ColorR, ColorG, ColorB));
            }
        }

        protected internal override void Read(OnlineReader Sender)
        {
            Message = Sender.ReadString();
            ColorR = Sender.ReadByte();
            ColorG = Sender.ReadByte();
            ColorB = Sender.ReadByte();
        }
    }
}
