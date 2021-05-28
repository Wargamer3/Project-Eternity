using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class SendPlayerDamageScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Send Player Damage";

        private readonly TripleThunderOnlineClient Owner;

        private int LayerIndex;
        private uint ActiveRobotID;
        private uint TargetRobotID;
        private Vector2 DamagePosition;
        private int Damage;
        private int FinalHP;
        private bool IsPlayerControlled;

        public SendPlayerDamageScriptClient(TripleThunderOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new SendPlayerDamageScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Layer ActiveLayer = Owner.TripleThunderGame.ListLayer[LayerIndex];
            ActiveLayer.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            RobotAnimation ActiveRobot;
            RobotAnimation TargetRobot;

            if (LayerIndex < Owner.TripleThunderGame.ListLayer.Count)
            {
                Layer ActiveLayer = Owner.TripleThunderGame.ListLayer[LayerIndex];
                bool HasActiveRobot = ActiveLayer.DicRobot.TryGetValue(ActiveRobotID, out ActiveRobot);
                bool HasTargetRobot = ActiveLayer.DicRobot.TryGetValue(TargetRobotID, out TargetRobot);

                if (HasActiveRobot && HasTargetRobot)
                {
                    TargetRobot.HP = FinalHP;
                    ActiveLayer.OnDamageRobot(ActiveRobot, TargetRobot, Damage, DamagePosition, IsPlayerControlled);
                }
            }
        }

        protected override void Read(OnlineReader Host)
        {
            LayerIndex = Host.ReadInt32();
            ActiveRobotID = Host.ReadUInt32();
            TargetRobotID = Host.ReadUInt32();
            DamagePosition = new Vector2(Host.ReadFloat(), Host.ReadFloat());
            Damage = Host.ReadInt32();
            FinalHP = Host.ReadInt32();
            IsPlayerControlled = Host.ReadBoolean();
        }
    }
}
