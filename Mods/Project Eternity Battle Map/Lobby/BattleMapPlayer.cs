﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapPlayer
    {
        public const string PlayerTypeNA = "N/A";
        public const string PlayerTypeOffline = "Offline";
        public const string PlayerTypeOnline = "Online";
        public const string PlayerTypeHost = "Host";
        public const string PlayerTypePlayer = "Player";
        public const string PlayerTypeReady = "Ready";
        public const string PlayerTypeSpectator = "Spectator";

        public enum PlayerTypes { Offline, Online, Host, Player, Ready, Spectator }

        public string ConnectionID;
        public string Name;
        public string Guild;
        public byte License;
        public byte Ranking;
        public bool IsOnline;
        public int Team;
        public int Level;
        public uint Money;
        public bool IsPlayerControlled;
        public Color Color;
        public GameplayTypes GameplayType;
        public PlayerInput InputManager;
        public byte LocalPlayerIndex;

        public string OnlinePlayerType;
        public IOnlineConnection OnlineClient;//Used by the server

        public PlayerInventory Inventory;

        public BattleMapPlayer(string ID, string Name, string OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color)
        {
            this.ConnectionID = ID;
            this.Name = Name;
            this.OnlinePlayerType = OnlinePlayerType;
            this.IsOnline = IsOnline;
            this.Team = Team;
            this.IsPlayerControlled = IsPlayerControlled;
            this.Color = Color;

            Guild = string.Empty;
            Inventory = new PlayerInventory();

            GameplayType = GameplayTypes.MouseAndKeyboard;
            InputManager = new KeyboardInput();
        }

        public BattleMapPlayer(string ID, string Name, PlayerTypes OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color)
        {
            this.ConnectionID = ID;
            this.Name = Name;
            this.IsOnline = IsOnline;
            this.Team = Team;
            this.IsPlayerControlled = IsPlayerControlled;
            this.Color = Color;

            Guild = string.Empty;
            Inventory = new PlayerInventory();

            if (OnlinePlayerType == PlayerTypes.Offline)
            {
                this.OnlinePlayerType = PlayerTypeOffline;
            }
            else if (OnlinePlayerType == PlayerTypes.Online)
            {
                this.OnlinePlayerType = PlayerTypeOnline;
            }
            else if (OnlinePlayerType == PlayerTypes.Host)
            {
                this.OnlinePlayerType = PlayerTypeHost;
            }
            else if (OnlinePlayerType == PlayerTypes.Player)
            {
                this.OnlinePlayerType = PlayerTypePlayer;
            }
            else if (OnlinePlayerType == PlayerTypes.Ready)
            {
                this.OnlinePlayerType = PlayerTypeReady;
            }
            else
            {
                this.OnlinePlayerType = PlayerTypeSpectator;
            }

            GameplayType = GameplayTypes.MouseAndKeyboard;
            InputManager = new KeyboardInput();
        }

        public BattleMapPlayer(BattleMapPlayer Clone)
        {
            if (Clone == null)
            {
                Team = -1;
                OnlinePlayerType = PlayerTypeNA;
                return;
            }

            ConnectionID = Clone.ConnectionID;
            Name = Clone.Name;
            Guild = Clone.Guild;
            License = Clone.License;
            Ranking = Clone.Ranking;
            IsOnline = Clone.IsOnline;
            Team = Clone.Team;
            Level = Clone.Level;
            Money = Clone.Money;
            IsPlayerControlled = Clone.IsPlayerControlled;
            Color = Clone.Color;
            GameplayType = Clone.GameplayType;
            InputManager = Clone.InputManager;
            LocalPlayerIndex = Clone.LocalPlayerIndex;

            OnlinePlayerType = Clone.OnlinePlayerType;
            OnlineClient = Clone.OnlineClient;

            Inventory = Clone.Inventory;
        }

        public void LoadLocally(ContentManager Content)
        {
            if (!File.Exists("User data/Profiles/Battle Map/Last Selected Profile.txt"))
            {
                File.WriteAllText("User data/Profiles/Battle Map/Last Selected Profile.txt", "Player 1", Encoding.UTF8);
                Name = "Player 1";
                InitFirstTimeInventory();
                SaveLocally();
                return;
            }

            Name = File.ReadAllText("User data/Profiles/Battle Map/Last Selected Profile.txt", Encoding.UTF8);
            FileStream FS = new FileStream("User data/Profiles/Battle Map/" + Name + ".bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Enum.TryParse(BR.ReadString(), out GameplayType);
            Money = BR.ReadUInt32();

            InitFirstTimeInventory();
            BR.Close();
            FS.Close();
        }

        private void InitFirstTimeInventory()
        {
            IniFile IniDefaultUnits = IniFile.ReadFromFile("Content/Battle Lobby Default Units.ini");

            foreach (string ActiveKey in IniDefaultUnits.ReadAllKeys())
            {
                string UnitPath = IniDefaultUnits.ReadField(ActiveKey, "Path");
                string Pilot = IniDefaultUnits.ReadField(ActiveKey, "Pilot");

                Unit NewUnit = Unit.FromFullName(UnitPath, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                Character NewCharacter = new Character(Pilot, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                NewCharacter.Level = 1;
                NewUnit.ArrayCharacterActive = new Character[] { NewCharacter };

                Squad NewSquad = new Squad("Squad", NewUnit);
                NewSquad.IsPlayerControlled = true;

                Inventory.ListOwnedSquad.Add(NewSquad);
                Inventory.ListOwnedCharacter.Add(NewCharacter);
            }

            Inventory.ActiveLoadout.ListSquad.Add(Inventory.ListOwnedSquad[0]);
        }

        public void SaveLocally()
        {
            FileStream FS = new FileStream("User data/Profiles/Battle Map/" + Name + ".bin", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(GameplayType.ToString());
            BW.Write(Money);

            BW.Close();
            FS.Close();
        }

        public static List<string> GetProfileNames()
        {
            List<string> ListProfileName = new List<string>();

            DirectoryInfo MapDirectory = new DirectoryInfo("User data/Profiles/Battle Map");

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
            return OnlinePlayerType == PlayerTypeHost || OnlinePlayerType == PlayerTypeOffline;
        }
    }
}
