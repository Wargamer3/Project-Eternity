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
            NewRoomPlayer.LoadOnlineInventory(PlayerInfo.Info, null, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
            Sender.ExtraInformation = NewRoomPlayer;
            NewRoomPlayer.OnlineClient = Sender;

            Sender.Send(new PlayerInventoryScriptServer(PlayerInfo));
        }

        protected override void Read(OnlineReader Sender)
        {
            ID = Sender.ReadString();
        }
    }
}
