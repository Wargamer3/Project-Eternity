using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class ShootBulletScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Shoot Bullet";

        private readonly TripleThunderOnlineClient Owner;

        private uint OwnerID;
        private int LayerIndex;
        private string WeaponName;
        private Vector2 GunNozzlePosition;
        private List<Vector2> ListSpeed;

        private readonly List<AttackBox> ListBullet;

        public ShootBulletScriptClient(TripleThunderOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public ShootBulletScriptClient(uint OwnerID, int LayerIndex, string WeaponName, Vector2 GunNozzlePosition, List<AttackBox> ListBullet)
            : base(ScriptName)
        {
            this.OwnerID = OwnerID;
            this.LayerIndex = LayerIndex;
            this.WeaponName = WeaponName;
            this.GunNozzlePosition = GunNozzlePosition;
            this.ListBullet = ListBullet;
        }

        public override OnlineScript Copy()
        {
            return new ShootBulletScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendUInt32(OwnerID);
            WriteBuffer.AppendInt32(LayerIndex);
            WriteBuffer.AppendString(WeaponName);

            WriteBuffer.AppendFloat(GunNozzlePosition.X);
            WriteBuffer.AppendFloat(GunNozzlePosition.Y);

            WriteBuffer.AppendInt32(ListBullet.Count);
            for (int B = 0; B < ListBullet.Count; ++B)
            {
                WriteBuffer.AppendFloat(ListBullet[B].Speed.X);
                WriteBuffer.AppendFloat(ListBullet[B].Speed.Y);
            }
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Layer ActiveLayer = Owner.TripleThunderGame.ListLayer[LayerIndex];
            ActiveLayer.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            RobotAnimation ActivePlayer = Owner.TripleThunderGame.ListLayer[LayerIndex].DicRobot[OwnerID];
            WeaponBase ActiveWeapon = ActivePlayer.PrimaryWeapons.GetWeapon(WeaponName);

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

        protected override void Read(OnlineReader Host)
        {
            OwnerID = Host.ReadUInt32();
            LayerIndex = Host.ReadInt32();
            WeaponName = Host.ReadString();

            GunNozzlePosition = new Vector2(Host.ReadFloat(), Host.ReadFloat());

            int NumberOfBullets = Host.ReadInt32();
            ListSpeed = new List<Vector2>(NumberOfBullets);
            for (int B = 0; B < NumberOfBullets; ++B)
            {
                ListSpeed.Add(new Vector2(Host.ReadFloat(), Host.ReadFloat()));
            }
        }
    }
}
