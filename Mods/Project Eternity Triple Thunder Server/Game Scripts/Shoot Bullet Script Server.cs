using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class ShootBulletScriptServer : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Shoot Bullet";

        private readonly TripleThunderClientGroup ActiveGroup;
        private FightingZone ActiveGame { get { return ActiveGroup.TripleThunderGame; } }
        private readonly Player Owner;

        private uint OwnerID;
        private int LayerIndex;
        private string WeaponName;
        private Vector2 GunNozzlePosition;
        private List<Vector2> ListSpeed;

        public ShootBulletScriptServer(TripleThunderClientGroup ActiveGroup, Player Owner)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
            this.Owner = Owner;
        }

        public ShootBulletScriptServer(uint OwnerID, int LayerIndex, string WeaponName, Vector2 GunNozzlePosition, List<Vector2> ListSpeed)
            : base(ScriptName)
        {
            this.OwnerID = OwnerID;
            this.LayerIndex = LayerIndex;
            this.WeaponName = WeaponName;
            this.GunNozzlePosition = GunNozzlePosition;
            this.ListSpeed = ListSpeed;
        }

        public override OnlineScript Copy()
        {
            return new ShootBulletScriptServer(ActiveGroup, Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendUInt32(OwnerID);
            WriteBuffer.AppendInt32(LayerIndex);
            WriteBuffer.AppendString(WeaponName);

            WriteBuffer.AppendFloat(GunNozzlePosition.X);
            WriteBuffer.AppendFloat(GunNozzlePosition.Y);

            WriteBuffer.AppendInt32(ListSpeed.Count);
            for (int B = 0; B < ListSpeed.Count; ++B)
            {
                WriteBuffer.AppendFloat(ListSpeed[B].X);
                WriteBuffer.AppendFloat(ListSpeed[B].Y);
            }
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            if (Owner.InGameRobot.ID == OwnerID)
            {
                Layer ActiveLayer = ActiveGame.ListLayer[LayerIndex];
                ActiveLayer.DelayOnlineScript(this);

                foreach (IOnlineConnection ActiveOnlinePlayer in ActiveGroup.Room.ListOnlinePlayer)
                {
                    if (ActiveOnlinePlayer != Sender)
                    {
                        ActiveOnlinePlayer.Send(new ShootBulletScriptServer(OwnerID, LayerIndex, WeaponName, GunNozzlePosition, ListSpeed));
                    }
                }
            }
        }

        public void ExecuteOnMainThread()
        {
            Layer ActiveLayer = ActiveGame.ListLayer[LayerIndex];
            RobotAnimation ActivePlayer = ActiveLayer.DicRobot[OwnerID];
            Weapon ActiveWeapon = ActivePlayer.Weapons.GetWeapon(WeaponName);

            foreach (Vector2 ActiveSpeed in ListSpeed)
            {
                float Angle = (float)Math.Atan2(ActiveSpeed.Y, ActiveSpeed.X);
                ActivePlayer.SetRobotContext(ActiveWeapon, Angle, GunNozzlePosition);

                if (ActiveWeapon.HasSkills)
                {
                    ActiveWeapon.UpdateSkills("Shoot");
                }
                else
                {
                    ActiveWeapon.Shoot(ActivePlayer, GunNozzlePosition, Angle, new List<BaseAutomaticSkill>());
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            OwnerID = Sender.ReadUInt32();
            LayerIndex = Sender.ReadInt32();
            WeaponName = Sender.ReadString();

            GunNozzlePosition = new Vector2(Sender.ReadFloat(), Sender.ReadFloat());

            int NumberOfBullets = Sender.ReadInt32();
            ListSpeed = new List<Vector2>(NumberOfBullets);
            for (int B = 0; B < NumberOfBullets; ++B)
            {
                ListSpeed.Add(new Vector2(Sender.ReadFloat(), Sender.ReadFloat()));
            }
        }
    }
}
