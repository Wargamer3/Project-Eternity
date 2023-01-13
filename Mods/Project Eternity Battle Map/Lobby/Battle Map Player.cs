using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapPlayer : OnlinePlayerBase
    {
        public BattleMapPlayerInventory Inventory;
        public BattleMapPlayerShopInventory ShopInventory;
        public BattleMapPlayerUnlockInventory UnlockInventory;

        public override string SaveFileFolder => "Battle Map/";

        public BattleMapPlayer()
        {
            Inventory = new BattleMapPlayerInventory();
            ShopInventory = new BattleMapPlayerShopInventory();
            UnlockInventory = new BattleMapPlayerUnlockInventory();
            UnlocksEvaluator = new BattleMapItemUnlockConditionsEvaluator(this);
        }

        public BattleMapPlayer(string ID, string Name, string OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color)
            : base(ID, Name, OnlinePlayerType, IsOnline, Team, IsPlayerControlled, Color)
        {
            Inventory = new BattleMapPlayerInventory();
            ShopInventory = new BattleMapPlayerShopInventory();
            UnlockInventory = new BattleMapPlayerUnlockInventory();
            UnlocksEvaluator = new BattleMapItemUnlockConditionsEvaluator(this);
        }

        public BattleMapPlayer(string ID, string Name, PlayerTypes OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color)
            : base(ID, Name, OnlinePlayerType, IsOnline, Team, IsPlayerControlled, Color)
        {
            Inventory = new BattleMapPlayerInventory();
            ShopInventory = new BattleMapPlayerShopInventory();
            UnlockInventory = new BattleMapPlayerUnlockInventory();
            UnlocksEvaluator = new BattleMapItemUnlockConditionsEvaluator(this);
        }

        public BattleMapPlayer(BattleMapPlayer Clone)
            : base(Clone)
        {
            if (Clone == null)
            {
                return;
            }

            Inventory = Clone.Inventory;
            ShopInventory = Clone.ShopInventory;
            UnlockInventory = Clone.UnlockInventory;
            UnlocksEvaluator = new BattleMapItemUnlockConditionsEvaluator(this);
        }

        protected override void DoLoadLocally(ContentManager Content, BinaryReader BR)
        {
            Inventory.Load(BR, Content, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
            ShopInventory.PopulateUnlockedShopItems(Name);
            UnlockInventory.PopulateUnlockedPlayerItems(Name);
        }

        public override void InitFirstTimeInventory()
        {
            IniFile IniDefaultUnits = IniFile.ReadFromFile("Content/Battle Lobby Default Units.ini");

            foreach (string ActiveKey in IniDefaultUnits.ReadAllKeys())
            {
                string UnitPath = IniDefaultUnits.ReadField(ActiveKey, "Path");
                string PilotPath = IniDefaultUnits.ReadField(ActiveKey, "Pilot");

                Unit NewUnit = Unit.FromFullName(UnitPath, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                Character NewCharacter = new Character(PilotPath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                NewCharacter.Level = 1;
                NewUnit.ArrayCharacterActive = new Character[] { NewCharacter };

                Squad NewSquad = new Squad("Squad", NewUnit);
                NewSquad.IsPlayerControlled = true;

                Inventory.ListOwnedSquad.Add(NewSquad);
                Inventory.ListOwnedCharacter.Add(NewCharacter);
            }

            Inventory.ActiveLoadout.ListSpawnSquad.Add(Inventory.ListOwnedSquad[0]);
        }

        public void FillLoadout(int UnitsInLoadout)
        {
            IniFile IniDefaultUnits = IniFile.ReadFromFile("Content/Battle Lobby Default Units.ini");
            string ActiveKey = IniDefaultUnits.ReadAllKeys()[0];

            string UnitPath = IniDefaultUnits.ReadField(ActiveKey, "Path");
            string Pilot = IniDefaultUnits.ReadField(ActiveKey, "Pilot");

            while (Inventory.ActiveLoadout.ListSpawnSquad.Count < UnitsInLoadout)
            {
                Unit NewUnit = Unit.FromFullName(UnitPath, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                Character NewCharacter = new Character(Pilot, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                NewCharacter.Level = 1;
                NewUnit.ArrayCharacterActive = new Character[] { NewCharacter };

                Squad NewSquad = new Squad("Squad", NewUnit);
                NewSquad.IsPlayerControlled = true;

                Inventory.ActiveLoadout.ListSpawnSquad.Add(NewSquad);
            }

            while (Inventory.ActiveLoadout.ListSpawnSquad.Count > UnitsInLoadout)
            {
                Inventory.ActiveLoadout.ListSpawnSquad.RemoveAt(Inventory.ActiveLoadout.ListSpawnSquad.Count - 1);
            }
        }

        protected override void DoSaveLocally(BinaryWriter BW)
        {
            Inventory.Save(BW);
        }
    }
}
