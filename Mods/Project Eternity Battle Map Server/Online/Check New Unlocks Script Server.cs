using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class CheckNewUnlocksScriptServer : OnlineScript
    {
        public const string ScriptName = "Check New Unlocks";

        private readonly GameServer Owner;
        private string ID;

        public CheckNewUnlocksScriptServer(GameServer Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new CheckNewUnlocksScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            BattleMapPlayer ConditionsOwner = (BattleMapPlayer)Sender.ExtraInformation;
            UpdateUnlockedItems(ConditionsOwner);

            List<UnlockableItem> ListUnlockedItem = new List<UnlockableItem>();

            for (int C = ConditionsOwner.UnlockInventory.ListLockedCharacter.Count - 1; C >= 0; C--)
            {
                UnlockableCharacter ActiveCharacter = ConditionsOwner.UnlockInventory.ListLockedCharacter[C];
                if (BattleMapItemUnlockConditionsEvaluator.IsValid(ActiveCharacter.UnlockConditions, ConditionsOwner))
                {
                    ActiveCharacter.UnlockConditions.IsUnlocked = true;
                    ConditionsOwner.UnlockInventory.ListLockedCharacter.Remove(ActiveCharacter);
                    ConditionsOwner.UnlockInventory.ListUnlockedCharacter.Add(ActiveCharacter);
                    ListUnlockedItem.Add(ActiveCharacter);
                    if (ActiveCharacter.UnlockQuantity > 0)
                    {
                        ConditionsOwner.Inventory.AddPathCharacter(ActiveCharacter.Path, ActiveCharacter.UnlockQuantity);
                    }
                }
            }

            for (int U = ConditionsOwner.UnlockInventory.RootUnitContainer.ListLockedUnit.Count - 1; U >= 0; U--)
            {
                UnlockableUnit ActiveUnit = ConditionsOwner.UnlockInventory.RootUnitContainer.ListLockedUnit[U];
                if (BattleMapItemUnlockConditionsEvaluator.IsValid(ActiveUnit.UnlockConditions, ConditionsOwner))
                {
                    ActiveUnit.UnlockConditions.IsUnlocked = true;
                    ConditionsOwner.UnlockInventory.RootUnitContainer.ListLockedUnit.Remove(ActiveUnit);
                    ConditionsOwner.UnlockInventory.RootUnitContainer.ListUnlockedUnit.Add(ActiveUnit);
                    ListUnlockedItem.Add(ActiveUnit);
                    if (ActiveUnit.UnlockQuantity > 0 && !ConditionsOwner.Inventory.DicOwnedUnit.ContainsKey(ActiveUnit.Path))
                    {
                        ConditionsOwner.Inventory.DicOwnedUnit.Add(ActiveUnit.Path, new UnitInfo(new ServerFillerUnit(ActiveUnit.Path), ActiveUnit.UnlockQuantity));
                    }
                }
            }

            for (int M = ConditionsOwner.UnlockInventory.ListLockedMission.Count - 1; M >= 0; M--)
            {
                UnlockableMission ActiveMission = ConditionsOwner.UnlockInventory.ListLockedMission[M];
                if (BattleMapItemUnlockConditionsEvaluator.IsValid(ActiveMission.UnlockConditions, ConditionsOwner))
                {
                    ActiveMission.UnlockConditions.IsUnlocked = true;
                    ConditionsOwner.UnlockInventory.ListLockedMission.Remove(ActiveMission);
                    ConditionsOwner.UnlockInventory.ListUnlockedMission.Add(ActiveMission);
                    ListUnlockedItem.Add(ActiveMission);
                    if (!ConditionsOwner.Inventory.DicOwnedMission.ContainsKey(ActiveMission.Path))
                    {
                        ConditionsOwner.Inventory.DicOwnedMission.Add(ActiveMission.Path, new MissionInfo(ActiveMission.Path, ActiveMission.UnlockQuantity));
                    }
                }
            }

            if (ListUnlockedItem.Count > 0)
            {
                Owner.Database.SavePlayerInventory(ID, ConditionsOwner);
            }

            Sender.Send(new NewUnlocksScriptServer(ListUnlockedItem));
        }

        protected override void Read(OnlineReader Sender)
        {
            ID = Sender.ReadString();
        }

        private static bool AlreadyExist(List<UnlockableCharacter> ListItem, string Path)
        {
            foreach (UnlockableCharacter ActiveItem in ListItem)
            {
                if (ActiveItem.Path == Path)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool AlreadyExist(List<UnlockableUnit> ListItem, string Path)
        {
            foreach (UnlockableUnit ActiveItem in ListItem)
            {
                if (ActiveItem.Path == Path)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool AlreadyExist(List<UnlockableMission> ListItem, string Path)
        {
            foreach (UnlockableMission ActiveItem in ListItem)
            {
                if (ActiveItem.Path == Path)
                {
                    return true;
                }
            }

            return false;
        }

        private static void UpdateUnlockedItems(BattleMapPlayer ConditionsOwner)
        {
            foreach (UnlockableCharacter ActiveCharacterUnlock in BattleMapPlayerShopUnlockInventory.CharacterLoader.Database.Values)
            {
                if (AlreadyExist(ConditionsOwner.UnlockInventory.ListUnlockedCharacter, ActiveCharacterUnlock.Path) || AlreadyExist(ConditionsOwner.UnlockInventory.ListLockedCharacter, ActiveCharacterUnlock.Path))
                {
                    continue;
                }

                if (BattleMapItemUnlockConditionsEvaluator.IsValid(ActiveCharacterUnlock.UnlockConditions, ConditionsOwner))
                {
                    ConditionsOwner.UnlockInventory.ListUnlockedCharacter.Add(ActiveCharacterUnlock);
                }
                else
                {
                    ConditionsOwner.UnlockInventory.ListLockedCharacter.Add(ActiveCharacterUnlock);
                }
            }

            foreach (UnlockableUnit ActiveUnitUnlock in BattleMapPlayerShopUnlockInventory.UnitLoader.Database.Values)
            {
                UnlockableUnit PlayerUnit = ConditionsOwner.UnlockInventory.FindPlayerUnit(ActiveUnitUnlock.Path);
                if (PlayerUnit == null)
                {
                    PlayerUnit = ActiveUnitUnlock.Copy();
                }

                if (BattleMapItemUnlockConditionsEvaluator.IsValid(PlayerUnit.UnlockConditions, ConditionsOwner))
                {
                    ConditionsOwner.UnlockInventory.RootUnitContainer.AddToUnlocked(PlayerUnit);
                }
                else
                {
                    ConditionsOwner.UnlockInventory.RootUnitContainer.AddToLocked(PlayerUnit);
                }
            }

            foreach (UnlockableMission ActiveMissionUnlock in BattleMapPlayerShopUnlockInventory.MissionLoader.Database.Values)
            {
                if (AlreadyExist(ConditionsOwner.UnlockInventory.ListUnlockedMission, ActiveMissionUnlock.Path) || AlreadyExist(ConditionsOwner.UnlockInventory.ListLockedMission, ActiveMissionUnlock.Path))
                {
                    continue;
                }

                if (BattleMapItemUnlockConditionsEvaluator.IsValid(ActiveMissionUnlock.UnlockConditions, ConditionsOwner))
                {
                    ConditionsOwner.UnlockInventory.ListUnlockedMission.Add(ActiveMissionUnlock);
                }
                else
                {
                    ConditionsOwner.UnlockInventory.ListLockedMission.Add(ActiveMissionUnlock);
                }
            }
        }

        // Server filler. Remembers which unit was unlocked by keeping UnitTypeName and RelativePath in DicOwnedUnit. It is not loaded and it is not spawned.
        private sealed class ServerFillerUnit : Unit
        {
            public override string UnitTypeName { get; }

            public ServerFillerUnit(string FullPath)
                : base(FullPath.Substring(FullPath.IndexOf('/') + 1))
            {
                UnitTypeName = FullPath.Substring(0, FullPath.IndexOf('/'));
                UnitTags = string.Empty;
                ID = RelativePath;
            }

            public override Unit FromFile(string RelativePath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            {
                throw new NotImplementedException();
            }

            protected override void DoQuickSave(BinaryWriter BW)
            {
                throw new NotImplementedException();
            }

            protected override void DoQuickLoad(BinaryReader BR, ContentManager Content)
            {
                throw new NotImplementedException();
            }

            public override void DoInit()
            {
                throw new NotImplementedException();
            }

            public override void ReinitializeMembers(Unit InitializedUnitBase)
            {
                throw new NotImplementedException();
            }

            public override GameScreen GetCustomizeScreen(List<Unit> ListPresentUnit, int SelectedUnitIndex, FormulaParser ActiveParser)
            {
                throw new NotImplementedException();
            }
        }
    }
}
