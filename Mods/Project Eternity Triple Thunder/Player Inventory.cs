using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class PlayerInventory
    {
        public List<CharacterMenuEquipment> ListCharacter;
        public List<MenuEquipment> ListEquipment;
        public List<MenuEquipment> ListItem;
        public List<WeaponMenuEquipment> ListWeapon;

        public string CharacterType;
        public string GrenadeType;

        public MenuEquipment EquipedEtc;
        public MenuEquipment EquipedHead;
        public MenuEquipment EquipedArmor;
        public MenuEquipment EquipedWeaponOption;
        public MenuEquipment EquipedBooster;
        public MenuEquipment EquipedShoes;

        public WeaponMenuEquipment[] ArrayEquipedPrimaryWeapon;
        public string EquipedPrimaryWeapon;
        public WeaponMenuEquipment EquipedSecondaryWeapon;

        public PlayerInventory()
        {
            ListCharacter = new List<CharacterMenuEquipment>();
            ListEquipment = new List<MenuEquipment>();
            ListItem = new List<MenuEquipment>();
            ListWeapon = new List<WeaponMenuEquipment>();

            CharacterType = "Jack";
            GrenadeType = string.Empty;

            ArrayEquipedPrimaryWeapon = new WeaponMenuEquipment[2];
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

        public void SetPrimaryWeapon(WeaponMenuEquipment WeaponToEquip, int Index)
        {
            if (ArrayEquipedPrimaryWeapon[Index] != null)
            {
                ListWeapon.Add(ArrayEquipedPrimaryWeapon[Index]);
            }

            ArrayEquipedPrimaryWeapon[Index] = WeaponToEquip;
            ListWeapon.Remove(WeaponToEquip);
        }

        public void Load(BinaryReader BR, ContentManager Content)
        {
            LoadCharacters(BR, Content);
            LoadEquipment(BR, Content);
            LoadItems(BR, Content);
            LoadWeapons(BR, Content);

            CharacterType = BR.ReadString();
            GrenadeType = BR.ReadString();
            LoadPrimaryWeapons(BR, Content);
            EquipSecondary(BR.ReadString(), Content);

            EquipedEtc = LoadNamedEquipment(BR.ReadString(), EquipmentTypes.Etc, Content);
            EquipedHead = LoadNamedEquipment(BR.ReadString(), EquipmentTypes.Head, Content);
            EquipedArmor = LoadNamedEquipment(BR.ReadString(), EquipmentTypes.Armor, Content);
            EquipedWeaponOption = LoadNamedEquipment(BR.ReadString(), EquipmentTypes.WeaponOption, Content);
            EquipedBooster = LoadNamedEquipment(BR.ReadString(), EquipmentTypes.Booster, Content);
            EquipedShoes = LoadNamedEquipment(BR.ReadString(), EquipmentTypes.Shoes, Content);
        }

        private void LoadCharacters(BinaryReader BR, ContentManager Content)
        {
            int ListCharacterCount = BR.ReadInt32();
            for (int C = 0; C < ListCharacterCount; ++C)
            {
                string CharacterName = BR.ReadString();
                if (CharacterName != "")
                {
                    ListCharacter.Add(LoadCharacter(CharacterName, Content));
                }
            }
        }

        private void LoadEquipment(BinaryReader BR, ContentManager Content)
        {
            int ListEquipmentCount = BR.ReadInt32();
            for (int C = 0; C < ListEquipmentCount; ++C)
            {
                MenuEquipment LoadedEquipment = LoadNamedEquipment(BR.ReadString(), EquipmentTypes.Etc, Content);
                if (LoadedEquipment != null)
                {
                    ListEquipment.Add(LoadedEquipment);
                }
            }
        }

        private void LoadItems(BinaryReader BR, ContentManager Content)
        {
            int ListItemCount = BR.ReadInt32();
            for (int C = 0; C < ListItemCount; ++C)
            {
                MenuEquipment LoadedItem = LoadNamedEquipment(BR.ReadString(), EquipmentTypes.Etc, Content);
                if (LoadedItem != null)
                {
                    ListItem.Add(LoadedItem);
                }
            }
        }

        private void LoadWeapons(BinaryReader BR, ContentManager Content)
        {
            int ListWeaponCount = BR.ReadInt32();
            for (int C = 0; C < ListWeaponCount; ++C)
            {
                string WeaponName = BR.ReadString();
                string WeaponCategory = BR.ReadString();
                int WeaponLevel = BR.ReadInt32();
                ListWeapon.Add(LoadWeapon(WeaponName, WeaponCategory, WeaponLevel, Content));
            }
        }

        private void LoadPrimaryWeapons(BinaryReader BR, ContentManager Content)
        {
            int ListWeaponCount = BR.ReadInt32();
            for (int C = 0; C < ListWeaponCount; ++C)
            {
                string WeaponName = BR.ReadString();
                string WeaponCategory = BR.ReadString();
                int WeaponLevel = BR.ReadInt32();

                if (WeaponName != "")
                {
                    ArrayEquipedPrimaryWeapon[C] = LoadWeapon(WeaponName, WeaponCategory, WeaponLevel, Content);
                }
            }
        }

        private void EquipSecondary(string SecondaryWeaponName, ContentManager Content)
        {
            if (SecondaryWeaponName == "")
            {
                return;
            }

            for (int W = 0; W < ListWeapon.Count; W++)
            {
                if (ListWeapon[W].Name == SecondaryWeaponName)
                {
                    EquipedSecondaryWeapon = ListWeapon[W];
                    ListWeapon.RemoveAt(W);
                    return;
                }
            }

            EquipedSecondaryWeapon = LoadWeapon(SecondaryWeaponName, string.Empty, 0, Content);
        }

        private CharacterMenuEquipment LoadCharacter(string CharacterName, ContentManager Content)
        {
            if (CharacterName == "Soul")
            {
                return new CharacterMenuEquipment("Soul", 100, null, Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Player Soul Portrait"));
            }

            return new CharacterMenuEquipment(CharacterName, 100, null, Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/" + CharacterName + " Portrait"));
        }

        private MenuEquipment LoadNamedEquipment(string EquipmentName, EquipmentTypes SlotType, ContentManager Content)
        {
            if (EquipmentName == "")
            {
                return null;
            }

            if (EquipmentName == "Armor 1")
            {
                return new MenuEquipment("Armor 1", EquipmentTypes.Armor, 150, Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Armor 01 Icon"), Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Armor 01 Full"));
            }

            return new MenuEquipment(EquipmentName, SlotType, 150, Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/" + EquipmentName + " Icon"), Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/" + EquipmentName + " Full"));
        }

        private WeaponMenuEquipment LoadWeapon(string WeaponName, string WeaponCategory, int WeaponLevel, ContentManager Content)
        {
            return new WeaponMenuEquipment(WeaponName, WeaponCategory, WeaponLevel, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/" + WeaponName + " Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/" + WeaponName + " Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/" + WeaponName + " Full"));
        }

        public void SaveLocally(BinaryWriter BW)
        {
            BW.Write(ListCharacter.Count);
            foreach (CharacterMenuEquipment ActiveCharacter in ListCharacter)
            {
                BW.Write(ActiveCharacter.Name);
            }

            BW.Write(ListEquipment.Count);
            foreach (MenuEquipment ActiveEquipment in ListEquipment)
            {
                BW.Write(ActiveEquipment.Name);
            }

            BW.Write(ListItem.Count);
            foreach (MenuEquipment ActiveEquipment in ListItem)
            {
                BW.Write(ActiveEquipment.Name);
            }

            BW.Write(ListWeapon.Count);
            foreach (WeaponMenuEquipment ActiveEquipment in ListWeapon)
            {
                BW.Write(ActiveEquipment.Name);
                BW.Write(ActiveEquipment.Category);
                BW.Write(ActiveEquipment.MinLevel);
            }

            BW.Write(CharacterType);
            BW.Write(GrenadeType);

            BW.Write(ArrayEquipedPrimaryWeapon.Length);
            foreach (WeaponMenuEquipment ActiveWeapon in ArrayEquipedPrimaryWeapon)
            {
                if (ActiveWeapon == null)
                {
                    BW.Write("");
                    BW.Write("");
                    BW.Write(0);
                }
                else
                {
                    BW.Write(ActiveWeapon.Name);
                    BW.Write(ActiveWeapon.Category);
                    BW.Write(ActiveWeapon.MinLevel);
                }
            }

            if (EquipedSecondaryWeapon == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(EquipedSecondaryWeapon.Name);
            }

            if (EquipedEtc == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(EquipedEtc.Name);
            }

            if (EquipedHead == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(EquipedHead.Name);
            }

            if (EquipedArmor == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(EquipedArmor.Name);
            }

            if (EquipedWeaponOption == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(EquipedWeaponOption.Name);
            }

            if (EquipedBooster == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(EquipedBooster.Name);
            }

            if (EquipedShoes == null)
            {
                BW.Write("");
            }
            else
            {
                BW.Write(EquipedShoes.Name);
            }
        }
    }
}
