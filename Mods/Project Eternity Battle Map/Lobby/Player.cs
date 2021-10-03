using System;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public enum GameplayTypes { None, MouseAndKeyboard, Controller1, Controller2, Controller3, Controller4 }

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
        public bool IsOnline;
        public int Team;
        public int Level;
        public uint Money;
        public GameplayTypes GameplayType;

        public IOnlineConnection OnlineClient;//Used by the server

        public Player(string ID, string Name, string PlayerType, bool IsOnline, int Team)
        {
            this.ConnectionID = ID;
            this.Name = Name;
            this.PlayerType = PlayerType;
            this.IsOnline = IsOnline;
            this.Team = Team;

            Guild = string.Empty;

            GameplayType = GameplayTypes.MouseAndKeyboard;
        }

        public Player(string ID, string Name, PlayerTypes PlayerType, bool IsOnline, int Team)
        {
            this.ConnectionID = ID;
            this.Name = Name;
            this.IsOnline = IsOnline;
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
        }

        public void LoadLocally(ContentManager Content)
        {
            if (!File.Exists("Triple Thunder Save File.bin"))
            {
                SaveLocally();
            }

            FileStream FS = new FileStream("Triple Thunder Save File.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Enum.TryParse(BR.ReadString(), out GameplayType);
            Money = BR.ReadUInt32();

            BR.Close();
            FS.Close();
        }

        public void SaveLocally()
        {
            FileStream FS = new FileStream("Triple Thunder Save File.bin", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(Name);
            BW.Write(GameplayType.ToString());
            BW.Write(Money);

            BW.Close();
            FS.Close();
        }

        public bool IsHost()
        {
            return PlayerType == PlayerTypeHost || PlayerType == PlayerTypeOffline;
        }
    }
}
