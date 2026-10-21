using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.SorcererStreetScreen;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class InventoryTests
    {
        [TestMethod]
        public void BattleMapUnlockCopiesKeepSeparateSkins()
        {
            string UnitPath = "InventoryTest/Shared Unit";
            string SkinPath = "InventoryTest/Skin A";
            try
            {
                UnlockableUnit ShopUnit = BattleMapPlayerShopUnlockInventory.GetOrAddUnit(UnitPath);
                BattleMapPlayerShopUnlockInventory First = new BattleMapPlayerShopUnlockInventory();
                BattleMapPlayerShopUnlockInventory Second = new BattleMapPlayerShopUnlockInventory();
                UnlockableUnit FirstPlayerUnit = ShopUnit.Copy();
                First.RootUnitContainer.ListUnlockedUnit.Add(FirstPlayerUnit);
                UnlockableUnit SecondPlayerUnit = ShopUnit.Copy();
                Second.RootUnitContainer.ListUnlockedUnit.Add(SecondPlayerUnit);
                UnlockableUnitSkin Skin = BattleMapPlayerShopUnlockInventory.GetOrAddSkin(UnitPath, SkinPath);
                FirstPlayerUnit.ListUnlockedSkin.Add(Skin);

                Assert.AreSame(ShopUnit, BattleMapPlayerShopUnlockInventory.UnitLoader.Database[UnitPath]);
                Assert.AreNotSame(FirstPlayerUnit, ShopUnit);
                Assert.AreNotSame(FirstPlayerUnit, SecondPlayerUnit);
                Assert.AreNotSame(FirstPlayerUnit.ListUnlockedSkin, SecondPlayerUnit.ListUnlockedSkin);
                Assert.AreEqual(0, ShopUnit.ListUnlockedSkin.Count);
                Assert.AreEqual(1, FirstPlayerUnit.ListUnlockedSkin.Count);
                Assert.AreSame(Skin, FirstPlayerUnit.ListUnlockedSkin[0]);
                Assert.AreEqual(0, SecondPlayerUnit.ListUnlockedSkin.Count);
            }
            finally
            {
                BattleMapPlayerShopUnlockInventory.UnitLoader.Database.Remove(UnitPath);
                BattleMapPlayerShopUnlockInventory.UnitSkinLoader.Database.Remove(UnitPath + SkinPath);
            }
        }

        [TestMethod]
        public void SorcererStreetUnlockSaveKeepsBookCardCharacterAndMissionPaths()
        {
            string BookPath = "InventoryTest/Book";
            string CardPath = "InventoryTest/Card";
            string CharacterPath = "InventoryTest/Character";
            string MissionPath = "InventoryTest/Mission";
            try
            {
                SorcererStreetPlayerUnlockInventory Source = new SorcererStreetPlayerUnlockInventory();
                Source.RootBookContainer.ListUnlockedBook.Add(SorcererStreetPlayerUnlockInventory.GetOrAddBook(BookPath).Copy());
                Source.RootCardContainer.ListUnlockedCard.Add(SorcererStreetPlayerUnlockInventory.GetOrAddCard(CardPath).Copy());
                Source.RootCharacterContainer.ListUnlockedCharacter.Add(SorcererStreetPlayerUnlockInventory.GetOrAddCharacter(CharacterPath).Copy());
                Source.ListUnlockedMission.Add(SorcererStreetPlayerUnlockInventory.GetOrAddMission(MissionPath));

                MemoryStream Stream = new MemoryStream();
                BinaryWriter Writer = new BinaryWriter(Stream, Encoding.UTF8, true);
                Source.WriteUnlocks(Writer);
                Writer.Flush();
                Stream.Position = 0;

                SorcererStreetPlayerUnlockInventory Loaded = new SorcererStreetPlayerUnlockInventory();
                Loaded.ReadUnlocks(new BinaryReader(Stream, Encoding.UTF8));

                Assert.AreEqual(BookPath, Loaded.RootBookContainer.ListUnlockedBook[0].Path);
                Assert.AreEqual(CardPath, Loaded.RootCardContainer.ListUnlockedCard[0].Path);
                Assert.AreEqual(CharacterPath, Loaded.RootCharacterContainer.ListUnlockedCharacter[0].Path);
                Assert.AreEqual(MissionPath, Loaded.ListUnlockedMission[0].Path);
            }
            finally
            {
                SorcererStreetPlayerUnlockInventory.BookLoader.Database.Remove(BookPath);
                SorcererStreetPlayerUnlockInventory.CardLoader.Database.Remove(CardPath);
                SorcererStreetPlayerUnlockInventory.CharacterLoader.Database.Remove(CharacterPath);
                SorcererStreetPlayerUnlockInventory.MissionLoader.Database.Remove(MissionPath);
            }
        }

        [TestMethod]
        public void TripleThunderSaveRestoresCharacterTypeAndEquippedSlots()
        {
            ProjectEternity.GameScreens.TripleThunderScreen.Player Source = new ProjectEternity.GameScreens.TripleThunderScreen.Player("id", "Player 1", ProjectEternity.GameScreens.TripleThunderScreen.Player.PlayerTypes.Player, 0);
            Source.Money = 40;
            Source.GameplayType = ProjectEternity.GameScreens.TripleThunderScreen.GameplayTypes.Controller1;
            Source.Equipment.CharacterType = "Soul";
            Source.Equipment.GrenadeType = "Frag";
            Source.Equipment.ListCharacter.Add(new CharacterMenuEquipment("Soul", 100, null, null));
            Source.Equipment.ListWeapon.Add(new WeaponMenuEquipment("Colt Python", "Pistol", 1, 150, null, null, null));
            Source.Equipment.ArrayEquipedPrimaryWeapon[0] = new WeaponMenuEquipment("AWP", "Sniper Rifle", 1, 150, null, null, null);
            Source.Equipment.ArrayEquipedPrimaryWeapon[1] = new WeaponMenuEquipment("M3", "Shotgun", 2, 150, null, null, null);
            Source.Equipment.EquipedSecondaryWeapon = new WeaponMenuEquipment("Bubble Gun", "Machine Gun", 1, 150, null, null, null);
            Source.Equipment.EquipedEtc = new MenuEquipment("Visor", EquipmentTypes.Etc, 10, null, null);
            Source.Equipment.EquipedHead = new MenuEquipment("Cap", EquipmentTypes.Head, 10, null, null);
            Source.Equipment.EquipedArmor = new MenuEquipment("Armor 1", EquipmentTypes.Armor, 150, null, null);
            Source.Equipment.EquipedWeaponOption = new MenuEquipment("Scope", EquipmentTypes.WeaponOption, 10, null, null);
            Source.Equipment.EquipedBooster = new MenuEquipment("Jump Pack", EquipmentTypes.Booster, 10, null, null);
            Source.Equipment.EquipedShoes = new MenuEquipment("Runners", EquipmentTypes.Shoes, 10, null, null);

            byte[] Saved = Source.SaveToBytes();
            DataManager Manager = new DataManager();
            Manager.SavePlayerInventory("player-1", Source);
            CollectionAssert.AreEqual(Saved, Manager.GetPlayerInventory("player-1").Info);

            ProjectEternity.GameScreens.TripleThunderScreen.Player Loaded = new ProjectEternity.GameScreens.TripleThunderScreen.Player("id", "Other", ProjectEternity.GameScreens.TripleThunderScreen.Player.PlayerTypes.Player, 1);
            Loaded.Load(Saved, new TexturelessContent());

            Assert.AreEqual("Player 1", Loaded.Name);
            Assert.AreEqual(ProjectEternity.GameScreens.TripleThunderScreen.GameplayTypes.Controller1, Loaded.GameplayType);
            Assert.AreEqual<uint>(40, Loaded.Money);
            Assert.AreEqual("Soul", Loaded.Equipment.CharacterType);
            Assert.AreEqual("Frag", Loaded.Equipment.GrenadeType);
            Assert.AreEqual("Soul", Loaded.Equipment.ListCharacter[0].Name);
            Assert.AreEqual("Colt Python", Loaded.Equipment.ListWeapon[0].Name);
            Assert.AreEqual("Pistol", Loaded.Equipment.ListWeapon[0].Category);
            Assert.AreEqual(1, Loaded.Equipment.ListWeapon[0].MinLevel);
            Assert.AreEqual("AWP", Loaded.Equipment.ArrayEquipedPrimaryWeapon[0].Name);
            Assert.AreEqual("Sniper Rifle", Loaded.Equipment.ArrayEquipedPrimaryWeapon[0].Category);
            Assert.AreEqual(1, Loaded.Equipment.ArrayEquipedPrimaryWeapon[0].MinLevel);
            Assert.AreEqual("M3", Loaded.Equipment.ArrayEquipedPrimaryWeapon[1].Name);
            Assert.AreEqual("Shotgun", Loaded.Equipment.ArrayEquipedPrimaryWeapon[1].Category);
            Assert.AreEqual(2, Loaded.Equipment.ArrayEquipedPrimaryWeapon[1].MinLevel);
            Assert.AreEqual("Bubble Gun", Loaded.Equipment.EquipedSecondaryWeapon.Name);
            Assert.AreEqual("Visor", Loaded.Equipment.EquipedEtc.Name);
            Assert.AreEqual(EquipmentTypes.Etc, Loaded.Equipment.EquipedEtc.EquipmentType);
            Assert.AreEqual("Cap", Loaded.Equipment.EquipedHead.Name);
            Assert.AreEqual(EquipmentTypes.Head, Loaded.Equipment.EquipedHead.EquipmentType);
            Assert.AreEqual("Armor 1", Loaded.Equipment.EquipedArmor.Name);
            Assert.AreEqual(EquipmentTypes.Armor, Loaded.Equipment.EquipedArmor.EquipmentType);
            Assert.AreEqual("Scope", Loaded.Equipment.EquipedWeaponOption.Name);
            Assert.AreEqual(EquipmentTypes.WeaponOption, Loaded.Equipment.EquipedWeaponOption.EquipmentType);
            Assert.AreEqual("Jump Pack", Loaded.Equipment.EquipedBooster.Name);
            Assert.AreEqual(EquipmentTypes.Booster, Loaded.Equipment.EquipedBooster.EquipmentType);
            Assert.AreEqual("Runners", Loaded.Equipment.EquipedShoes.Name);
            Assert.AreEqual(EquipmentTypes.Shoes, Loaded.Equipment.EquipedShoes.EquipmentType);
        }

        private sealed class EmptyServices : IServiceProvider
        {
            public object GetService(Type ServiceType)
            {
                return null;
            }
        }

        private sealed class TexturelessContent : ContentManager
        {
            public TexturelessContent()
                : base(new EmptyServices(), "Content")
            {
            }

            public override T Load<T>(string AssetName)
            {
                return default(T);
            }
        }
    }
}
