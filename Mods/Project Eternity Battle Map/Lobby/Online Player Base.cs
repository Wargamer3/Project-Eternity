using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class OnlinePlayerBase
    {
        public const string PlayerTypeNA = "N/A";
        public const string PlayerTypeBot = "Bot";
        public const string PlayerTypePlayer = "Player";
        public const string PlayerTypeSpectator = "Spectator";

        public enum PlayerTypes { Offline, Bot, Player, Spectator }

        public string ConnectionID;
        public string Name;
        public string Guild;
        public byte License;
        public byte Ranking;
        public bool IsOnline;
        public int TeamIndex;
        public int EXP;
        public int Level;
        public bool IsPlayerControlled;//Controlled by a player or a bot
        public Color Color;
        public GameplayTypes GameplayType;
        public PlayerInput InputManager;
        public byte LocalPlayerIndex;//Index in PlayerManager.ListLocalPlayer

        public string OnlinePlayerType;
        public IOnlineConnection OnlineClient;//Used by the server

        public ItemUnlockConditionsEvaluator UnlocksEvaluator;
        public readonly PlayerRecords Records;

        public abstract string SaveFileFolder { get; }

        public OnlinePlayerBase()
        {
            Records = new PlayerRecords();
            InputManager = new KeyboardInput();
        }

        public OnlinePlayerBase(string ID, string Name, bool IsOnline)
        {
            this.ConnectionID = ID;
            this.Name = Name;
            this.IsOnline = IsOnline;

            Level = 1;
            Guild = string.Empty;
            Records = new PlayerRecords();

            GameplayType = GameplayTypes.MouseAndKeyboard;
            InputManager = new KeyboardInput();
            OnlineClient = new OnlineConnectionDummy();
        }

        public OnlinePlayerBase(string ID, string Name, string OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color)
        {
            this.ConnectionID = ID;
            this.Name = Name;
            this.OnlinePlayerType = OnlinePlayerType;
            this.IsOnline = IsOnline;
            this.TeamIndex = Team;
            this.IsPlayerControlled = IsPlayerControlled;
            this.Color = Color;

            Level = 1;
            Guild = string.Empty;
            Records = new PlayerRecords();

            GameplayType = GameplayTypes.MouseAndKeyboard;
            InputManager = new KeyboardInput();
            OnlineClient = new OnlineConnectionDummy();
        }

        public OnlinePlayerBase(string ID, string Name, PlayerTypes OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color)
        {
            this.ConnectionID = ID;
            this.Name = Name;
            this.IsOnline = IsOnline;
            this.TeamIndex = Team;
            this.IsPlayerControlled = IsPlayerControlled;
            this.Color = Color;

            Level = 1;
            Guild = string.Empty;
            Records = new PlayerRecords();

            if (OnlinePlayerType == PlayerTypes.Player)
            {
                this.OnlinePlayerType = PlayerTypePlayer;
            }
            else if(OnlinePlayerType == PlayerTypes.Bot)
            {
                this.OnlinePlayerType = PlayerTypeBot;
            }
            else
            {
                this.OnlinePlayerType = PlayerTypeSpectator;
            }

            GameplayType = GameplayTypes.MouseAndKeyboard;
            InputManager = new KeyboardInput();
            OnlineClient = new OnlineConnectionDummy();
        }

        public OnlinePlayerBase(OnlinePlayerBase Clone)
        {
            if (Clone == null)
            {
                TeamIndex = -1;
                OnlinePlayerType = PlayerTypeNA;
                Records = new PlayerRecords();
                return;
            }

            Records = Clone.Records;
            ConnectionID = Clone.ConnectionID;
            Name = Clone.Name;
            Guild = Clone.Guild;
            License = Clone.License;
            Ranking = Clone.Ranking;
            IsOnline = Clone.IsOnline;
            TeamIndex = Clone.TeamIndex;
            Level = Clone.Level;
            IsPlayerControlled = Clone.IsPlayerControlled;
            Color = Clone.Color;
            GameplayType = Clone.GameplayType;
            InputManager = Clone.InputManager;
            LocalPlayerIndex = Clone.LocalPlayerIndex;

            OnlinePlayerType = Clone.OnlinePlayerType;
            OnlineClient = Clone.OnlineClient;
        }

        public abstract void InitFirstTimeInventory();

        public void LoadLocally(ContentManager Content)
        {
            if (!File.Exists("User data/Profiles/" + SaveFileFolder + "Last Selected Profile.txt"))
            {
                Name = "Player 1";
                InitFirstTimeInventory();
                SaveLocally();
                File.WriteAllText("User data/Profiles/" + SaveFileFolder + "Last Selected Profile.txt", "Player 1", Encoding.UTF8);
                return;
            }

            if (Name == null)
            {
                Name = File.ReadAllText("User data/Profiles/" + SaveFileFolder + "Last Selected Profile.txt", Encoding.UTF8);
            }
            FileStream FS = new FileStream("User data/Profiles/" + SaveFileFolder + Name + ".bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            ReadProfileAndRecords(BR);

            DoLoadLocally(Content, BR);

            BR.Close();
            FS.Close();
        }

        protected abstract void DoLoadLocally(ContentManager Content, BinaryReader BR);

        private void ReadProfileAndRecords(BinaryReader BR)
        {
            Enum.TryParse(BR.ReadString(), out GameplayType);
            Level = BR.ReadInt32();
            EXP = BR.ReadInt32();
            Records.Load(BR);
        }

        protected void OpenPackedInventory(byte[] Packed, out BinaryReader ProfileReader, out BinaryReader UnlockReader)
        {
            BinaryReader Reader = new BinaryReader(new MemoryStream(Packed), Encoding.UTF8);
            byte[] ProfileBytes = Reader.ReadBytes(Reader.ReadInt32());
            byte[] UnlockBytes = Reader.ReadBytes(Reader.ReadInt32());
            ProfileReader = new BinaryReader(new MemoryStream(ProfileBytes), Encoding.UTF8);
            UnlockReader = new BinaryReader(new MemoryStream(UnlockBytes), Encoding.UTF8);
            ReadProfileAndRecords(ProfileReader);
        }

        public void SaveLocally()
        {
            Directory.CreateDirectory("User data/Profiles/" + SaveFileFolder);
            FileStream FS = new FileStream("User data/Profiles/" + SaveFileFolder + Name + ".bin", FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(GameplayType.ToString());
            BW.Write(Level);
            BW.Write(EXP);

            Records.Save(BW);

            DoSaveLocally(BW);

            BW.Close();
            FS.Close();
        }

        public byte[] SaveToBytes()
        {
            MemoryStream Stream = new MemoryStream();
            BinaryWriter Writer = new BinaryWriter(Stream, Encoding.UTF8, true);
            Writer.Write(GameplayType.ToString());
            Writer.Write(Level);
            Writer.Write(EXP);
            Records.Save(Writer);
            DoSaveLocally(Writer);
            Writer.Flush();
            return Stream.ToArray();
        }

        protected abstract void DoSaveLocally(BinaryWriter BW);

        public abstract List<MissionInfo> GetUnlockedMissions();

        public List<string> GetProfileNames()
        {
            List<string> ListProfileName = new List<string>();

            DirectoryInfo MapDirectory = new DirectoryInfo("User data/Profiles/" + SaveFileFolder);

            FileInfo[] ArrayMapFile = MapDirectory.GetFiles("*.bin");
            foreach (FileInfo ActiveFile in ArrayMapFile)
            {
                string FileName = ActiveFile.Name.Remove(ActiveFile.Name.Length - 4);
                ListProfileName.Add(FileName);
            }
            return ListProfileName;
        }

        public bool IsHost()
        {
            return OnlineClient.Roles.IsRoomHost;
        }

        public bool IsReady()
        {
            return OnlineClient.Roles.IsRoomReady;
        }
    }
}
