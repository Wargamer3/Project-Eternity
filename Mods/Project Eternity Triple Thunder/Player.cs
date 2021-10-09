using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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

        public PlayerEquipment Equipment;
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

            Equipment = new PlayerEquipment();
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

            Equipment = new PlayerEquipment();
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

            LoadCharacters(BR, Content);
            LoadEquipment(BR, Content);
            LoadItems(BR, Content);
            LoadWeapons(BR, Content);

            Equipment.CharacterType = BR.ReadString();
            Equipment.GrenadeType = BR.ReadString();
            Equipment.ExtraWeaponType = BR.ReadString();

            string EquipedEtc = BR.ReadString();
            string EquipedHead = BR.ReadString();
            string EquipedArmor = BR.ReadString();
            string EquipedWeaponOption = BR.ReadString();
            string EquipedBooster = BR.ReadString();
            string EquipedShoes = BR.ReadString();

            InitArmor(EquipedArmor);

            BR.Close();
            FS.Close();
        }

        private void LoadCharacters(BinaryReader BR, ContentManager Content)
        {
            int ListCharacterCount = BR.ReadInt32();
            for (int C = 0; C < ListCharacterCount; ++C)
            {
                string CharacterName = BR.ReadString();
                switch (CharacterName)
                {
                    case "Soul":
                        Equipment.ListCharacter.Add(new CharacterMenuEquipment("Soul", 100, null, Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Player Soul Portrait")));
                        break;
                }
            }
        }

        private void LoadEquipment(BinaryReader BR, ContentManager Content)
        {
            int ListEquipmentCount = BR.ReadInt32();
            for (int C = 0; C < ListEquipmentCount; ++C)
            {
                string EquipmentName = BR.ReadString();
                switch (EquipmentName)
                {
                    case "Armor 1":
                        Equipment.ListEquipment.Add(new MenuEquipment("Armor 1", EquipmentTypes.Armor, 150, Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Armor 01 Icon"), Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Armor 01 Full")));
                        break;
                }
            }
        }

        private void LoadItems(BinaryReader BR, ContentManager Content)
        {
            int ListItemCount = BR.ReadInt32();
            for (int C = 0; C < ListItemCount; ++C)
            {
                string ItemName = BR.ReadString();
                switch (ItemName)
                {
                }
            }
        }

        private void LoadWeapons(BinaryReader BR, ContentManager Content)
        {
            int ListWeaponCount = BR.ReadInt32();
            for (int C = 0; C < ListWeaponCount; ++C)
            {
                string WeaponName = BR.ReadString();
                switch (WeaponName)
                {
                }
            }
        }

        private void InitArmor(string ArmorToEquip)
        {
            for (int E = 0; E < Equipment.ListEquipment.Count; E++)
            {
                MenuEquipment ActiveEquipment = Equipment.ListEquipment[E];
                if (ActiveEquipment.Name == ArmorToEquip)
                {
                    Equipment.SetArmor(ActiveEquipment);
                }
            }
        }

        public void SaveLocally()
        {
            FileStream FS = new FileStream("Triple Thunder Save File.bin", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(Name);
            BW.Write(GameplayType.ToString());
            BW.Write(Money);

            BW.Write(Equipment.ListCharacter.Count);
            foreach (CharacterMenuEquipment ActiveCharacter in Equipment.ListCharacter)
            {
                BW.Write(ActiveCharacter.Name);
            }

            BW.Write(Equipment.ListEquipment.Count);
            foreach (MenuEquipment ActiveEquipment in Equipment.ListEquipment)
            {
                BW.Write(ActiveEquipment.Name);
            }

            BW.Write(Equipment.ListItem.Count);
            foreach (MenuEquipment ActiveEquipment in Equipment.ListItem)
            {
                BW.Write(ActiveEquipment.Name);
            }

            BW.Write(Equipment.ListWeapon.Count);
            foreach (MenuEquipment ActiveEquipment in Equipment.ListWeapon)
            {
                BW.Write(ActiveEquipment.Name);
            }

            BW.Write(Equipment.CharacterType);
            BW.Write(Equipment.GrenadeType);
            BW.Write(Equipment.ExtraWeaponType);

            if (Equipment.EquipedEtc == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(Equipment.EquipedEtc.Name);
            }

            if (Equipment.EquipedHead == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(Equipment.EquipedHead.Name);
            }

            if (Equipment.EquipedArmor == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(Equipment.EquipedArmor.Name);
            }

            if (Equipment.EquipedWeaponOption == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(Equipment.EquipedWeaponOption.Name);
            }

            if (Equipment.EquipedBooster == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(Equipment.EquipedBooster.Name);
            }

            if (Equipment.EquipedShoes == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(Equipment.EquipedShoes.Name);
            }

            BW.Close();
            FS.Close();
        }

        public bool IsHost()
        {
            return PlayerType == PlayerTypeHost || PlayerType == PlayerTypeOffline;
        }
    }

    public class PlayerEquipment
    {
        public List<CharacterMenuEquipment> ListCharacter;
        public List<MenuEquipment> ListEquipment;
        public List<MenuEquipment> ListItem;
        public List<MenuEquipment> ListWeapon;

        public string CharacterType;
        public string GrenadeType;
        public string ExtraWeaponType;
        public MenuEquipment EquipedEtc;
        public MenuEquipment EquipedHead;
        public MenuEquipment EquipedArmor;
        public MenuEquipment EquipedWeaponOption;
        public MenuEquipment EquipedBooster;
        public MenuEquipment EquipedShoes;

        public PlayerEquipment()
        {
            ListCharacter = new List<CharacterMenuEquipment>();
            ListEquipment = new List<MenuEquipment>();
            ListItem = new List<MenuEquipment>();
            ListWeapon = new List<MenuEquipment>();

            CharacterType = "Jack";
            GrenadeType = string.Empty;
            ExtraWeaponType = string.Empty;
        }

        public void SetEtc(MenuEquipment EtcToEquip)
        {
            if (EquipedEtc != null)
            {
                ListEquipment.Add(EquipedEtc);
            }

            EquipedEtc = EtcToEquip;
            ListEquipment.Remove(EtcToEquip);
        }

        public void SetHead(MenuEquipment HeadToEquip)
        {
            if (EquipedHead != null)
            {
                ListEquipment.Add(EquipedHead);
            }

            EquipedHead = HeadToEquip;
            ListEquipment.Remove(HeadToEquip);
        }

        public void SetArmor(MenuEquipment ArmorToEquip)
        {
            if (EquipedArmor != null)
            {
                ListEquipment.Add(EquipedArmor);
            }

            EquipedArmor = ArmorToEquip;
            ListEquipment.Remove(ArmorToEquip);
        }

        public void SetWeaponOption(MenuEquipment WeaponOptionToEquip)
        {
            if (EquipedWeaponOption != null)
            {
                ListEquipment.Add(EquipedWeaponOption);
            }

            EquipedWeaponOption = WeaponOptionToEquip;
            ListEquipment.Remove(WeaponOptionToEquip);
        }

        public void SetBooster(MenuEquipment BoosterToEquip)
        {
            if (EquipedBooster != null)
            {
                ListEquipment.Add(EquipedBooster);
            }

            EquipedBooster = BoosterToEquip;
            ListEquipment.Remove(BoosterToEquip);
        }

        public void SetShoes(MenuEquipment ShoesToEquip)
        {
            if (EquipedShoes != null)
            {
                ListEquipment.Add(EquipedShoes);
            }

            EquipedShoes = ShoesToEquip;
            ListEquipment.Remove(ShoesToEquip);
        }
    }
}
