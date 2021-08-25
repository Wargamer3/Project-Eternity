using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class SendPlayerRespawnScriptServer : OnlineScript
    {
        public const string ScriptName = "Send Player Respawn";

        private int LayerIndex;
        private uint PlayerID;
        private float PositionX;
        private float PositionY;
        private int PlayerHP;

        public SendPlayerRespawnScriptServer(int LayerIndex, uint PlayerID, float PositionX, float PositionY, int PlayerHP)
            : base(ScriptName)
        {
            this.LayerIndex = LayerIndex;
            this.PlayerID = PlayerID;
            this.PositionX = PositionX;
            this.PositionY = PositionY;
            this.PlayerHP = PlayerHP;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(LayerIndex);
            WriteBuffer.AppendUInt32(PlayerID);

            WriteBuffer.AppendFloat(PositionX);
            WriteBuffer.AppendFloat(PositionY);

            WriteBuffer.AppendInt32(PlayerHP);
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
