using System;
using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using System.Drawing.Design;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public abstract class SpawnPoint
    {
        public enum SpawnPointTypes { Player, Vehicle }

        public bool IsUsed;
        private int _Team;

        private SpawnPointTypes SpawnPointType;

        protected SpawnPoint(SpawnPointTypes SpawnPointType)
        {
            this.SpawnPointType = SpawnPointType;
        }

        internal void Save(BinaryWriter BW)
        {
            BW.Write((byte)SpawnPointType);
            BW.Write(SpawnLocation.X);
            BW.Write(SpawnLocation.Y);
            BW.Write(Team);

            DoSave(BW);
        }

        public abstract void DoSave(BinaryWriter BW);

        public abstract RobotAnimation SpawnPlayer(Player NewPlayer, Layer Owner, ISFXGenerator PlayerSFXGenerator, Rectangle CameraBounds);

        public static SpawnPoint Load(BinaryReader BR)
        {
            SpawnPointTypes SpawnPointType = (SpawnPointTypes)BR.ReadByte();

            switch (SpawnPointType)
            {
                case SpawnPointTypes.Player:
                    return new PlayerSpawnPoint(BR);
                case SpawnPointTypes.Vehicle:
                    return new VehicleSpawnPoint(BR);
            }

            return null;
        }

        [CategoryAttribute("Spawn attributes"),
        DescriptionAttribute("The position of the spawn"),
        DefaultValueAttribute("")]
        public Vector2 SpawnLocation { get; set; }

        [CategoryAttribute("Spawn attributes"),
        DescriptionAttribute("The team of the spawned item"),
        DefaultValueAttribute("")]
        public int Team { get; set; }
    }

    public class PlayerSpawnPoint : SpawnPoint
    {
        public PlayerSpawnPoint(BinaryReader BR)
            : base(SpawnPointTypes.Player)
        {
            SpawnLocation = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            Team = BR.ReadInt32();
            IsUsed = false;
        }

        public PlayerSpawnPoint(int X, int Y, int Team)
            : base(SpawnPointTypes.Player)
        {
            SpawnLocation = new Vector2(X, Y);
            this.Team = Team;
            IsUsed = false;
        }

        public override void DoSave(BinaryWriter BW)
        {
        }

        public override RobotAnimation SpawnPlayer(Player NewPlayer, Layer Owner, ISFXGenerator PlayerSFXGenerator, Rectangle CameraBounds)
        {
            RobotAnimation NewRobot = new RobotAnimation("Characters/" + NewPlayer.Equipment.CharacterType, Owner, SpawnLocation, Team, NewPlayer.Equipment, PlayerSFXGenerator, new List<Weapon>());
            NewRobot.InputManagerHelper = new PlayerRobotInputManager();

            NewRobot.UpdateControls(NewPlayer.GameplayType, CameraBounds);

            return NewRobot;
        }
    }

    public class VehicleSpawnPoint : SpawnPoint
    {
        private string _VehiclePath;
        private List<string> _ListWeapons;

        public VehicleSpawnPoint(BinaryReader BR)
            : base(SpawnPointTypes.Vehicle)
        {
            SpawnLocation = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            Team = BR.ReadInt32();
            _VehiclePath = BR.ReadString();

            int ListWeaponsCount = BR.ReadInt32();
            _ListWeapons = new List<string>(ListWeaponsCount);
            for (int W = 0; W < ListWeaponsCount; ++W)
                _ListWeapons.Add(BR.ReadString());

            IsUsed = false;
        }

        public VehicleSpawnPoint(int X, int Y, int Team)
            : base(SpawnPointTypes.Vehicle)
        {
            SpawnLocation = new Vector2(X, Y);
            this.Team = Team;
            _VehiclePath = string.Empty;
            IsUsed = false;
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(_VehiclePath);

            BW.Write(_ListWeapons.Count);
            for (int W = 0; W < _ListWeapons.Count; ++W)
                BW.Write(_ListWeapons[W]);
        }

        public override RobotAnimation SpawnPlayer(Player NewPlayer, Layer Owner, ISFXGenerator PlayerSFXGenerator, Rectangle CameraBounds)
        {
            List<Weapon> ListExtraWeapon = new List<Weapon>();
            for (int W = 0; W < _ListWeapons.Count; ++W)
            {
                ListExtraWeapon.Add(new Weapon(_ListWeapons[W], Owner.DicRequirement, Owner.DicEffect, Owner.DicAutomaticSkillTarget));
            }

            Vehicle NewVehicle = new Vehicle(_VehiclePath, Owner, SpawnLocation, Team, new PlayerInventory(), Owner.PlayerSFXGenerator, ListExtraWeapon);
            NewVehicle.InputManagerHelper = new VehicleInputManager();

            NewVehicle.UpdateControls(NewPlayer.GameplayType, CameraBounds);

            return NewVehicle;
        }

        [Editor(typeof(VehicleSelector), typeof(UITypeEditor)),
        CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The vehicle path"),
        DefaultValueAttribute("")]
        public string VehiclePath { get { return _VehiclePath; } set { _VehiclePath = value; } }

        [Editor(typeof(WeaponSelector), typeof(UITypeEditor)),
        CategoryAttribute("Prop attributes"),
        DescriptionAttribute("All the Weapons used by the vehicle"),
        DefaultValueAttribute("")]
        public List<string> ListWeapons { get { return _ListWeapons; } set { _ListWeapons = value; } }
    }
}
