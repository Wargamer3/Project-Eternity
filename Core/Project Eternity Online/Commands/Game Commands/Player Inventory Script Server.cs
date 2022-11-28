using System;

namespace ProjectEternity.Core.Online
{
    public class PlayerInventoryScriptServer : OnlineScript
    {
        public const string ScriptName = "Player Inventory";

        private readonly PlayerPOCO PlayerInfo;

        public PlayerInventoryScriptServer(PlayerPOCO PlayerInfo)
            : base(ScriptName)
        {
            this.PlayerInfo = PlayerInfo;
        }

        public override OnlineScript Copy()
        {
            return null;
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(PlayerInfo.ID);
            WriteBuffer.AppendByteArray(PlayerInfo.Info);
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
