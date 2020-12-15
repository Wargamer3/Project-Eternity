using System;
using System.Net;

namespace ProjectEternity.Core.Online
{
    public class RedirectScriptClient : OnlineScript
    {
        public const string ScriptName = "Redirect";

        private readonly Client Owner;
        private string ServerIP;
        private int ServerPort;
        private string CommunicationServerIP;
        private int CommunicationServerPort;

        public RedirectScriptClient(Client Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new RedirectScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
            Owner.ChangeHost(IPAddress.Parse(ServerIP), ServerPort);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            ServerIP = Sender.ReadString();
            ServerPort = Sender.ReadInt32();
            CommunicationServerIP = Sender.ReadString();
            CommunicationServerPort = Sender.ReadInt32();
        }
    }
}
