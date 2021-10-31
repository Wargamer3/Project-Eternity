using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class SendPlayerUpdateScriptServer : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Send Player Update";

        private int LayerIndex;
        private readonly TripleThunderClientGroup ActiveGroup;
        private FightingZone ActiveGame { get { return ActiveGroup.TripleThunderGame; } }
        private readonly Player ActivePlayer;
        private readonly RobotAnimation ActiveRobot;

        private uint PlayerID;
        private float PositionX;
        private float PositionY;
        private float SpeedX;
        private float SpeedY;
        private string ActiveMovementStance;

        private int WeaponCount;
        private List<float> ListWeaponAngle;

        /// <summary>
        /// Used to receive messages
        /// </summary>
        /// <param name="Owner"></param>
        /// <param name="ActivePlayer"></param>
        public SendPlayerUpdateScriptServer(TripleThunderClientGroup Owner, Player ActivePlayer)
            : base(ScriptName)
        {
            this.ActiveGroup = Owner;
            this.ActivePlayer = ActivePlayer;
        }

        /// <summary>
        /// Used to send messages
        /// </summary>
        /// <param name="LayerIndex"></param>
        /// <param name="ActiveRobot"></param>
        public SendPlayerUpdateScriptServer(int LayerIndex, RobotAnimation ActiveRobot)
            : base(ScriptName)
        {
            this.LayerIndex = LayerIndex;
            this.ActiveRobot = ActiveRobot;
        }

        public override OnlineScript Copy()
        {
            return new SendPlayerUpdateScriptServer(ActiveGroup, ActivePlayer);//Copy the receiver
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(LayerIndex);
            WriteBuffer.AppendUInt32(ActiveRobot.ID);

            WriteBuffer.AppendFloat(ActiveRobot.Position.X);
            WriteBuffer.AppendFloat(ActiveRobot.Position.Y);
            WriteBuffer.AppendFloat(ActiveRobot.Speed.X);
            WriteBuffer.AppendFloat(ActiveRobot.Speed.Y);
            WriteBuffer.AppendString(ActiveRobot.ActiveMovementStance);

            WriteBuffer.AppendInt32(ActiveRobot.PrimaryWeapons.ActiveWeapons.Count);
            for (int W = 0; W < ActiveRobot.PrimaryWeapons.ActiveWeapons.Count; W++)
            {
                WriteBuffer.AppendFloat(ActiveRobot.PrimaryWeapons.ActiveWeapons[W].WeaponAngle);
            }
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            if (Sender.IsGameReady)
            {
                Layer ActiveLayer = ActiveGame.ListLayer[LayerIndex];
                ActiveLayer.DelayOnlineScript(this);
            }
        }

        public void ExecuteOnMainThread()
        {
            if (ActivePlayer.InGameRobot.ID == PlayerID)
            {
                Microsoft.Xna.Framework.Vector2 Movement = new Microsoft.Xna.Framework.Vector2(PositionX, PositionY) - ActivePlayer.InGameRobot.Position;
                ActivePlayer.InGameRobot.Move(Movement);
                ActivePlayer.InGameRobot.ActiveMovementStance = ActiveMovementStance;

                ActivePlayer.InGameRobot.Speed = new Microsoft.Xna.Framework.Vector2(SpeedX, SpeedY);

                for (int W = 0; W < WeaponCount; W++)
                {
                    ActivePlayer.InGameRobot.PrimaryWeapons.ActiveWeapons[W].WeaponAngle = ListWeaponAngle[W];
                    ActivePlayer.InGameRobot.UpdatePrimaryWeaponAngle(ListWeaponAngle[W], W);
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            LayerIndex = Sender.ReadInt32();
            PlayerID = Sender.ReadUInt32();

            PositionX = Sender.ReadFloat();
            PositionY = Sender.ReadFloat();
            SpeedX = Sender.ReadFloat();
            SpeedY = Sender.ReadFloat();
            ActiveMovementStance = Sender.ReadString();

            WeaponCount = Sender.ReadInt32();
            ListWeaponAngle = new List<float>(WeaponCount);
            for (int W = 0; W < WeaponCount; W++)
            {
                ListWeaponAngle.Add(Sender.ReadFloat());
            }
        }
    }
}
