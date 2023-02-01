using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskShopInventoryScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Shop Inventory";

        private readonly GameServer Owner;
        private string ID;

        public AskShopInventoryScriptServer(GameServer Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskShopInventoryScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            BattleMapPlayer NewRoomPlayer = (BattleMapPlayer)Sender.ExtraInformation;

        }

        protected override void Read(OnlineReader Sender)
        {
            ID = Sender.ReadString();
        }
    }
}
