using System;

namespace ProjectEternity.Core.Online
{
    public class AskPlayerInventoryScriptClient : OnlineScript
    {
        private readonly string ID;

        public AskPlayerInventoryScriptClient(string ID)
            : base("Ask Player Inventory")
        {
            this.ID = ID;
        }

        public override OnlineScript Copy()
        {
            return new AskPlayerInventoryScriptClient(ID);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(ID);
        }

        protected internal override void Execute(IOnlineConnection Host)
        {
        }

        protected internal override void Read(OnlineReader Sender)
        {
        }
    }
}
