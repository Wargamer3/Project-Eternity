using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class BuildingSpawn
    {
        public BuildingConquest BuildingToSpawn;

        public string BuildingPath;
        private uint _SpawnID;
        private Point _SpawnPosition;
        private byte _SpawnLayer;
        private byte _SpawnPlayerIndex;
        private bool _IsEventSquad;
        private bool _IsPlayerControlled;
        private int _StartHP;
        private byte _Upgrades;
        private string _AIPath;

        public BuildingSpawn(BinaryReader BR)
        {
            BuildingPath = BR.ReadString();
            _SpawnID = BR.ReadUInt32();
            _SpawnPosition = new Point(BR.ReadInt32(), BR.ReadInt32());
            _SpawnLayer = BR.ReadByte();
            _SpawnPlayerIndex = BR.ReadByte();
            _IsEventSquad = BR.ReadBoolean();
            _IsPlayerControlled = BR.ReadBoolean();
            _StartHP = BR.ReadInt32();
            _Upgrades = BR.ReadByte();
            _AIPath = BR.ReadString();
            BuildingToSpawn = new BuildingConquest(BuildingPath, GameScreens.GameScreen.ContentFallback, null, null, null);
        }

        public BuildingSpawn(BuildingConquest BuildingToSpawn, Point _SpawnPosition, byte SpawnLayer)
        {
            this.BuildingToSpawn = BuildingToSpawn;
            this.BuildingPath = BuildingToSpawn.RelativePath;
            this._SpawnPosition = _SpawnPosition;
            this.SpawnLayer = SpawnLayer;

            _IsEventSquad = false;
            _IsPlayerControlled = true;
            _StartHP = -1;
            _Upgrades = 0;
            _AIPath = string.Empty;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(BuildingPath);
            BW.Write(_SpawnID);
            BW.Write(_SpawnPosition.X);
            BW.Write(_SpawnPosition.Y);
            BW.Write(_SpawnLayer);
            BW.Write(_SpawnPlayerIndex);
            BW.Write(_IsEventSquad);
            BW.Write(_IsPlayerControlled);
            BW.Write(_StartHP);
            BW.Write(_Upgrades);
            BW.Write(_AIPath);
        }

        #region Properties

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public uint SpawnID
        {
            get
            {
                return _SpawnID;
            }
            set
            {
                _SpawnID = value;
            }
        }

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public int SpawnPositionX
        {
            get
            {
                return _SpawnPosition.X;
            }
            set
            {
                _SpawnPosition.X = value;
            }
        }

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public int SpawnPositionY
        {
            get
            {
                return _SpawnPosition.Y;
            }
            set
            {
                _SpawnPosition.Y = value;
            }
        }

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public byte SpawnLayer
        {
            get
            {
                return _SpawnLayer;
            }
            set
            {
                _SpawnLayer = value;
            }
        }

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute("Starts at 1.")]
        public byte SpawnPlayer
        {
            get
            {
                return _SpawnPlayerIndex;
            }
            set
            {
                _SpawnPlayerIndex = value;
            }
        }

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute("Decide if this Building is important and shouldn't be destroyed.")]
        public bool IsEventSquad
        {
            get
            {
                return _IsEventSquad;
            }
            set
            {
                _IsEventSquad = value;
            }
        }

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute("Decide if this Building can be controlled by a player.")]
        public bool IsPlayerControlled
        {
            get
            {
                return _IsPlayerControlled;
            }
            set
            {
                _IsPlayerControlled = value;
            }
        }

        [Editor(typeof(Core.AI.Selectors.AISelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute("The AI path"),
        DefaultValueAttribute(0)]
        public string AIPath
        {
            get
            {
                return _AIPath;
            }
            set
            {
                _AIPath = value;
            }
        }

        #endregion
    }
}
