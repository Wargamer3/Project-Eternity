using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapPlayer : OnlinePlayerBase
    {
        public BattleMapPlayerInventory Inventory;
        public BattleMapPlayerUnlockInventory UnlockInventory;

        public override string SaveFileFolder => "Battle Map/";

        public BattleMapPlayer()
        {
            Inventory = new BattleMapPlayerInventory();
            UnlockInventory = new BattleMapPlayerUnlockInventory();
            UnlocksEvaluator = new BattleMapItemUnlockConditionsEvaluator(this);
        }

        public BattleMapPlayer(string ID, string Name, bool IsOnline)
            : base(ID, Name, IsOnline)
        {
            Inventory = new BattleMapPlayerInventory();
            UnlockInventory = new BattleMapPlayerUnlockInventory();
            UnlocksEvaluator = new BattleMapItemUnlockConditionsEvaluator(this);
        }

        public BattleMapPlayer(string ID, string Name, string OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color)
            : base(ID, Name, OnlinePlayerType, IsOnline, Team, IsPlayerControlled, Color)
        {
            Inventory = new BattleMapPlayerInventory();
            UnlockInventory = new BattleMapPlayerUnlockInventory();
            UnlocksEvaluator = new BattleMapItemUnlockConditionsEvaluator(this);
        }

        public BattleMapPlayer(string ID, string Name, PlayerTypes OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color)
            : base(ID, Name, OnlinePlayerType, IsOnline, Team, IsPlayerControlled, Color)
        {
            Inventory = new BattleMapPlayerInventory();
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
            UnlockInventory = Clone.UnlockInventory;
            UnlocksEvaluator = new BattleMapItemUnlockConditionsEvaluator(this);
        }

        public void InitRecords(ByteReader BR)
        {
            UnlockInventory.LoadOnlineData(BR);
            Records.Load(BR);
        }

        protected override void DoLoadLocally(ContentManager Content, BinaryReader BR)
        {
            Inventory.Load(BR, Content, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
            UnlockInventory.LoadPlayerUnlocks(Name);
        }

        public override void InitFirstTimeInventory()
        {
            IniFile IniDefaultUnits = IniFile.ReadFromFile("Content/Battle Lobby Default Units.ini");

            Squad FirstSquad = null;
            foreach (string ActiveKey in IniDefaultUnits.ReadAllKeys())
            {
                string UnitPath = IniDefaultUnits.ReadField(ActiveKey, "Path");
                string PilotPath = IniDefaultUnits.ReadField(ActiveKey, "Pilot");

                Unit NewUnit = Unit.FromFullName(UnitPath, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                NewUnit.ID = NewUnit.ItemName;
                if (!string.IsNullOrEmpty(PilotPath))
                {
                    Character NewCharacter = new Character(PilotPath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                    NewCharacter.Level = 1;
                    NewCharacter.ID = NewCharacter.Name;
                    NewUnit.ArrayCharacterActive = new Character[] { NewCharacter };
                    Inventory.DicOwnedCharacter.Add(PilotPath, new CharacterInfo(NewCharacter, 1));
                }

                Inventory.DicOwnedSquad.Add(UnitPath, new UnitInfo(NewUnit, 1));

                if (FirstSquad == null)
                {
                    Squad NewSquad = new Squad("Squad", NewUnit);
                    NewSquad.IsPlayerControlled = true;

                    FirstSquad = NewSquad;
                }
            }

            Inventory.ActiveLoadout.ListSpawnSquad.Add(FirstSquad);
            UnlockInventory.LoadPlayerUnlocks(Name);
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
                NewUnit.ID = NewUnit.ItemName;
                Character NewCharacter = new Character(Pilot, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                NewCharacter.Level = 1;
                NewCharacter.ID = NewCharacter.Name;
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
            UnlockInventory.SaveLocally(Name);
        }

        public override List<MissionInfo> GetUnlockedMissions()
        {
            return new List<MissionInfo>(Inventory.DicOwnedMission.Values);
        }
    }
}
