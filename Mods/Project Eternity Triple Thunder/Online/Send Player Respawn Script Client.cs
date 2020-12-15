using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class SendPlayerRespawnScriptClient : OnlineScript
    {
        public const string ScriptName = "Send Player Respawn";

        private readonly TripleThunderOnlineClient Owner;

        private int LayerIndex;
        private uint PlayerID;
        private float PositionX;
        private float PositionY;
        private int PlayerHP;

        public SendPlayerRespawnScriptClient(TripleThunderOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new SendPlayerRespawnScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Owner.TripleThunderGame.RespawnRobot(LayerIndex, PlayerID, PositionX, PositionY, PlayerHP);
        }

        protected override void Read(OnlineReader Host)
        {
            LayerIndex = Host.ReadInt32();
            PlayerID = Host.ReadUInt32();

            PositionX = Host.ReadFloat();
            PositionY = Host.ReadFloat();

            PlayerHP = Host.ReadInt32();
        }
    }
}
