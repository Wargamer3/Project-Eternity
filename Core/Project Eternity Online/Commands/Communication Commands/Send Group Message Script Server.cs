using System;

namespace ProjectEternity.Core.Online
{
    public class SendGroupMessageScriptServer : OnlineScript
    {
        public const string ScriptName = "Send Group Message";

        private readonly CommunicationServer OnlineServer;

        private string Source;
        private string Message;
        private byte MessageColor;

        public SendGroupMessageScriptServer(CommunicationServer OnlineServer)
            : base(ScriptName)
        {
            this.OnlineServer = OnlineServer;
        }

        public override OnlineScript Copy()
        {
            return new SendGroupMessageScriptServer(OnlineServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            string FinalMessage = Sender.Name + " : " + Message;

            CommunicationGroup SourceOwner;
            if (OnlineServer.DicCommunicationGroup.TryGetValue(Source, out SourceOwner))
            {
                foreach (IOnlineConnection ActiveOnlinePlayer in SourceOwner.ListGroupMember)
                {
                    ActiveOnlinePlayer.Send(new ReceiveGroupMessageScriptServer(Source, FinalMessage, (ChatManager.MessageColors)MessageColor));
                }

                if (SourceOwner.SaveLogs)
                {
                    OnlineServer.Database.SaveGroupMessage(DateTime.UtcNow, Source, FinalMessage, MessageColor);
                }
            }
            else
            {
                //cross server
            }
        }

        protected internal override void Read(OnlineReader Sender)
        {
            Source = Sender.ReadString();
            Message = Sender.ReadString();
            MessageColor = Sender.ReadByte();
        }
    }
}
