using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class SendPlayerUpdateScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Send Player Update";

        private readonly TripleThunderOnlineClient Owner;

        private int LayerIndex;
        private uint PlayerID;
        private float PositionX;
        private float PositionY;
        private float SpeedX;
        private float SpeedY;
        private string ActiveMovementStance;

        private int WeaponCount;
        private List<float> ListWeaponAngle;

        private readonly RobotAnimation ActivePlayer;

        public SendPlayerUpdateScriptClient(TripleThunderOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public SendPlayerUpdateScriptClient(RobotAnimation ActivePlayer)
            : base(ScriptName)
        {
            this.ActivePlayer = ActivePlayer;
        }

        public override OnlineScript Copy()
        {
            return new SendPlayerUpdateScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(LayerIndex);
            WriteBuffer.AppendUInt32(ActivePlayer.ID);

            WriteBuffer.AppendFloat(ActivePlayer.Position.X);
            WriteBuffer.AppendFloat(ActivePlayer.Position.Y);
            WriteBuffer.AppendFloat(ActivePlayer.Speed.X);
            WriteBuffer.AppendFloat(ActivePlayer.Speed.Y);
            WriteBuffer.AppendString(ActivePlayer.ActiveMovementStance);

            WriteBuffer.AppendInt32(ActivePlayer.PrimaryWeapons.ActiveWeapons.Count);
            for (int W = 0; W < ActivePlayer.PrimaryWeapons.ActiveWeapons.Count; W++)
            {
                WriteBuffer.AppendFloat(ActivePlayer.PrimaryWeapons.ActiveWeapons[W].WeaponAngle);
            }
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Layer ActiveLayer = Owner.TripleThunderGame.ListLayer[LayerIndex];
            ActiveLayer.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            if (!Owner.TripleThunderGame.IsMainCharacter(PlayerID))
            {
                Layer ActiveLayer = Owner.TripleThunderGame.ListLayer[LayerIndex];
                RobotAnimation ActivePlayer = ActiveLayer.DicRobot[PlayerID];
                ActivePlayer.Position = new Microsoft.Xna.Framework.Vector2(PositionX, PositionY);
                ActivePlayer.Speed = new Microsoft.Xna.Framework.Vector2(SpeedX, SpeedY);

                if (ActivePlayer.ActiveMovementStance != ActiveMovementStance)
                {
                    ActivePlayer.SetRobotAnimation(ActiveMovementStance);
                }

                for (int W = 0; W < WeaponCount; W++)
                {
                    ActivePlayer.PrimaryWeapons.ActiveWeapons[W].WeaponAngle = ListWeaponAngle[W];
                    ActivePlayer.UpdatePrimaryWeaponAngle(ListWeaponAngle[W], W);
                }
            }
        }

        protected override void Read(OnlineReader Host)
        {
            LayerIndex = Host.ReadInt32();
            PlayerID = Host.ReadUInt32();

            PositionX = Host.ReadFloat();
            PositionY = Host.ReadFloat();
            SpeedX = Host.ReadFloat();
            SpeedY = Host.ReadFloat();
            ActiveMovementStance = Host.ReadString();

            WeaponCount = Host.ReadInt32();
            ListWeaponAngle = new List<float>(WeaponCount);
            for (int W = 0; W < WeaponCount; W++)
            {
                ListWeaponAngle.Add(Host.ReadFloat());
            }
        }
    }
}
