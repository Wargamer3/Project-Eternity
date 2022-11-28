using System;

namespace ProjectEternity.Core.Online
{
    public class AskPlayerInventoryScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Player Inventory";

        private readonly GameServer Owner;
        private string ID;

        public AskPlayerInventoryScriptServer(GameServer Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskPlayerInventoryScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
            PlayerPOCO PlayerInfo = Owner.Database.GetPlayerInventory(ID);
            ActivePlayer.Send(new PlayerInventoryScriptServer(PlayerInfo));
        }

        protected internal override void Read(OnlineReader Sender)
        {
            ID = Sender.ReadString();
        }
    }
}
