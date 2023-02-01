using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
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

        protected override void Execute(IOnlineConnection Sender)
        {
            PlayerPOCO PlayerInfo = Owner.Database.GetPlayerInventory(ID);

            BattleMapPlayer NewRoomPlayer = new BattleMapPlayer(PlayerInfo.ID, PlayerInfo.Name, true);
            ByteReader BR = new ByteReader(PlayerInfo.Info);
            NewRoomPlayer.InitRecords(BR);
            BR.Clear();
            Sender.ExtraInformation = NewRoomPlayer;
            NewRoomPlayer.OnlineClient = Sender;
            NewRoomPlayer.GameplayType = GameplayTypes.None;

            Sender.Send(new PlayerInventoryScriptServer(PlayerInfo));
        }

        protected override void Read(OnlineReader Sender)
        {
            ID = Sender.ReadString();
        }
    }
}
