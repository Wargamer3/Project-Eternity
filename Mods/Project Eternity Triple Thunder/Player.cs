using System;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class Player
    {
        public const string PlayerTypeOffline = "Offline";
        public const string PlayerTypeOnline = "Online";
        public const string PlayerTypeHost = "Host";
        public const string PlayerTypePlayer = "Player";
        public const string PlayerTypeReady = "Ready";
        public const string PlayerTypeSpectator = "Spectator";

        public enum PlayerTypes { Offline, Online, Host, Player, Ready, Spectator }

        public string ConnectionID;
        public string Name;
        public string PlayerType;
        public string Guild;
        public byte License;
        public byte Ranking;
        public int Team;
        public int Level;
        public uint Money;
        public GameplayTypes GameplayType;
        public RobotAnimation CharacterPreview;

        public PlayerInventory Equipment;
        public RobotAnimation InGameRobot;
        public IOnlineConnection OnlineClient;//Used by the server

        public Player(string ID, string Name, string PlayerType, int Team)
        {
            this.ConnectionID = ID;
            this.Name = Name;
            this.PlayerType = PlayerType;
            this.Team = Team;

            Guild = string.Empty;

            GameplayType = GameplayTypes.MouseAndKeyboard;

            Equipment = new PlayerInventory();
        }

        public Player(string ID, string Name, PlayerTypes PlayerType, int Team)
        {
            this.ConnectionID = ID;
            this.Name = Name;
            this.Team = Team;

            Guild = string.Empty;

            if (PlayerType == PlayerTypes.Offline)
            {
                this.PlayerType = PlayerTypeOffline;
            }
            else if (PlayerType == PlayerTypes.Online)
            {
                this.PlayerType = PlayerTypeOnline;
            }
            else if (PlayerType == PlayerTypes.Host)
            {
                this.PlayerType = PlayerTypeHost;
            }
            else if (PlayerType == PlayerTypes.Player)
            {
                this.PlayerType = PlayerTypePlayer;
            }
            else if (PlayerType == PlayerTypes.Ready)
            {
                this.PlayerType = PlayerTypeReady;
            }
            else
            {
                this.PlayerType = PlayerTypeSpectator;
            }

            GameplayType = GameplayTypes.MouseAndKeyboard;

            Equipment = new PlayerInventory();
        }

        public void LoadLocally(ContentManager Content)
        {
            string ProfileFile = ProfileFilePath();
            if (!File.Exists(ProfileFile))
            {
                SaveLocally();
            }

            FileStream FS = new FileStream(ProfileFile, FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            ReadProfile(BR, Content);
            BR.Close();
            FS.Close();
        }

        public void SaveLocally()
        {
            string ProfileFile = ProfileFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(ProfileFile));
            FileStream FS = new FileStream(ProfileFile, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);
            WriteProfile(BW);
            BW.Close();
            FS.Close();
        }

        public byte[] SaveToBytes()
        {
            MemoryStream Stream = new MemoryStream();
            BinaryWriter Writer = new BinaryWriter(Stream, Encoding.UTF8, true);
            WriteProfile(Writer);
            Writer.Flush();
            return Stream.ToArray();
        }

        public void Load(byte[] Bytes, ContentManager Content)
        {
            BinaryReader Reader = new BinaryReader(new MemoryStream(Bytes), Encoding.UTF8);
            ReadProfile(Reader, Content);
            Reader.Close();
        }

        private void WriteProfile(BinaryWriter BW)
        {
            BW.Write(Name);
            BW.Write(GameplayType.ToString());
            BW.Write(Money);
            Equipment.SaveLocally(BW);
        }

        private void ReadProfile(BinaryReader BR, ContentManager Content)
        {
            Name = BR.ReadString();
            Enum.TryParse(BR.ReadString(), out GameplayType);
            Money = BR.ReadUInt32();
            Equipment.Load(BR, Content);
        }

        private string ProfileFilePath()
        {
            return Path.Combine("User data", "Profiles", "Triple Thunder", Name + ".bin");
        }

        public bool IsHost()
        {
            return PlayerType == PlayerTypeHost || PlayerType == PlayerTypeOffline;
        }
    }
}
