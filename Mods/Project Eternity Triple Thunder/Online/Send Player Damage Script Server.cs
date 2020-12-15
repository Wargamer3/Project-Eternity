using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class SendPlayerDamageScriptServer : OnlineScript
    {
        public const string ScriptName = "Send Player Damage";

        private readonly int LayerIndex;
        private readonly uint ActiveRobotID;
        private readonly uint TargetRobotID;
        private readonly Vector2 DamagePosition;
        private readonly int Damage;
        private readonly int FinalHP;
        private readonly bool IsPlayerControlled;

        public SendPlayerDamageScriptServer(int LayerIndex, uint ActiveRobotID, uint TargetRobotID, Vector2 DamagePosition, int Damage, int FinalHP, bool IsPlayerControlled)
            : base(ScriptName)
        {
            this.LayerIndex = LayerIndex;
            this.ActiveRobotID = ActiveRobotID;
            this.TargetRobotID = TargetRobotID;
            this.DamagePosition = DamagePosition;
            this.Damage = Damage;
            this.FinalHP = FinalHP;
            this.IsPlayerControlled = IsPlayerControlled;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(LayerIndex);
            WriteBuffer.AppendUInt32(ActiveRobotID);
            WriteBuffer.AppendUInt32(TargetRobotID);
            WriteBuffer.AppendFloat(DamagePosition.X);
            WriteBuffer.AppendFloat(DamagePosition.Y);
            WriteBuffer.AppendInt32(Damage);
            WriteBuffer.AppendInt32(FinalHP);
            WriteBuffer.AppendBoolean(IsPlayerControlled);
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
