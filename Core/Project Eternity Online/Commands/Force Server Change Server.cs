using System;

namespace ProjectEternity.Core.Online
{
    public class ForceServerChangeScriptServer : OnlineScript
    {
        private Client Owner;

        public ForceServerChangeScriptServer(Client Owner)
            : base("Redirect")
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new ForceServerChangeScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
