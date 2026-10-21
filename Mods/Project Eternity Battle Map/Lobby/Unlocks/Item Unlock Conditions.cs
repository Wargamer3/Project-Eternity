using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ItemUnlockConditions
    {
        public enum UnlockConditions { TimePlayed, GamePlayed, CampaignProgression, UnitKilled, UnitKilledBeforeMissionX, PlayerLevelReached, TimeSinceFirstPlayed }

        public readonly List<List<Tuple<string, string>>> ListUnlockConditions;//Top list for OR checks, nested lists for AND checks
        public bool IsUnlocked;

        public ItemUnlockConditions(Dictionary<string, string> ActiveHeaderValues)
        {
            ListUnlockConditions = new List<List<Tuple<string, string>>>();

            int UnlockNumber = 1;
            while (true)
            {
                string ActiveConditionLine;

                if (ActiveHeaderValues.TryGetValue("UnlockType" + UnlockNumber, out ActiveConditionLine))
                {
                    List<Tuple<string, string>> ListCurrentCondition = new List<Tuple<string, string>>();
                    string[] ArrayUnlockCondition = ActiveConditionLine.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                    for (int U = 0; U < ArrayUnlockCondition.Length; ++U)
                    {
                        string[] ArrayUnlock = ArrayUnlockCondition[U].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                        ListCurrentCondition.Add(new Tuple<string, string>(ArrayUnlock[0], ArrayUnlock[1]));
                    }

                    ListUnlockConditions.Add(ListCurrentCondition);
                    ++UnlockNumber;
                }
                else
                {
                    break;
                }
            }

            IsUnlocked = ListUnlockConditions.Count == 0;
        }

        public ItemUnlockConditions Copy()
        {
            ItemUnlockConditions Copied = (ItemUnlockConditions)MemberwiseClone();
            Copied.IsUnlocked = IsUnlocked;
            return Copied;
        }
    }

    public interface ItemUnlockConditionsEvaluator
    {
        List<GameScreen> Evaluate(ContentManager Content);
    }

    public class BattleMapItemUnlockConditionsEvaluator : ItemUnlockConditionsEvaluator
    {
        private BattleMapPlayer ConditionsOwner;

        public bool IsInit;
        public Task PopulatePlayerItemsTask;
        private object PopulatePlayerItemsTaskLockObject = new object();

        public BattleMapItemUnlockConditionsEvaluator(BattleMapPlayer ConditionsOwner)
        {
            this.ConditionsOwner = ConditionsOwner;
        }

        public List<GameScreen> Evaluate(ContentManager Content)
        {
            List<GameScreen> ListPendingUnlocks = new List<GameScreen>();
            bool NewUnlocks = false;

            if (BattleMapPlayerShopUnlockInventory.AreAssetsAllLoaded && ConditionsOwner.UnlockInventory.HasFinishedReadingPlayerShopItems)
            {
                if (!UpdateAvailableItemsIfNeeded())
                {
                    return ListPendingUnlocks;
                }

                for (int C = ConditionsOwner.UnlockInventory.ListLockedCharacter.Count - 1; C >= 0; C--)
                {
                    UnlockableCharacter ActiveCharacter = ConditionsOwner.UnlockInventory.ListLockedCharacter[C];
                    if (IsValid(ActiveCharacter.UnlockConditions, ConditionsOwner))
                    {
                        NewUnlocks = true;

                        foreach (string UnlockMessage in ActiveCharacter.Unlock(ConditionsOwner))
                        {
                            if (ActiveCharacter.UnlockConditions.ListUnlockConditions.Count > 0)
                            {
                                ListPendingUnlocks.Add(new PendingUnlockScreen(UnlockMessage));
                            }
                        }
                    }
                }

                for (int U = ConditionsOwner.UnlockInventory.RootUnitContainer.ListLockedUnit.Count - 1; U >= 0; U--)
                {
                    UnlockableUnit ActiveUnit = ConditionsOwner.UnlockInventory.RootUnitContainer.ListLockedUnit[U];
                    if (IsValid(ActiveUnit.UnlockConditions, ConditionsOwner))
                    {
                        NewUnlocks = true;

                        foreach (string UnlockMessage in ActiveUnit.Unlock(ConditionsOwner))
                        {
                            if (ActiveUnit.UnlockConditions.ListUnlockConditions.Count > 0)
                            {
                                ListPendingUnlocks.Add(new PendingUnlockScreen(UnlockMessage));
                            }
                        }
                    }
                }


                List<UnlockableUnit> PlayerUnits = new List<UnlockableUnit>();
                PlayerUnits.AddRange(ConditionsOwner.UnlockInventory.RootUnitContainer.ListUnlockedUnit);
                PlayerUnits.AddRange(ConditionsOwner.UnlockInventory.RootUnitContainer.ListLockedUnit);
                foreach (UnlockableUnit ActiveUnit in PlayerUnits)
                {
                    for (int S = ActiveUnit.ListLockedSkin.Count - 1; S >= 0; --S)
                    {
                        UnlockableUnitSkin ActiveSkin = ActiveUnit.ListLockedSkin[S];

                        if (IsValid(ActiveSkin.UnlockConditions, ConditionsOwner))
                        {
                            NewUnlocks = true;

                            foreach (string UnlockMessage in ActiveSkin.Unlock(ConditionsOwner))
                            {
                                if (ActiveSkin.UnlockConditions.ListUnlockConditions.Count > 0)
                                {
                                    ListPendingUnlocks.Add(new PendingUnlockScreen(UnlockMessage));
                                }
                            }
                        }
                    }

                    for (int S = ActiveUnit.ListLockedAlt.Count - 1; S >= 0; --S)
                    {
                        UnlockableUnitAlt ActiveAlt = ActiveUnit.ListLockedAlt[S];

                        if (IsValid(ActiveAlt.UnlockConditions, ConditionsOwner))
                        {
                            NewUnlocks = true;

                            foreach (string UnlockMessage in ActiveAlt.Unlock(ConditionsOwner))
                            {
                                if (ActiveAlt.UnlockConditions.ListUnlockConditions.Count > 0)
                                {
                                    ListPendingUnlocks.Add(new PendingUnlockScreen(UnlockMessage));
                                }
                            }
                        }
                    }
                }

                for (int M = ConditionsOwner.UnlockInventory.ListLockedMission.Count - 1; M >= 0; M--)
                {
                    UnlockableMission ActiveMission = ConditionsOwner.UnlockInventory.ListLockedMission[M];
                    if (IsValid(ActiveMission.UnlockConditions, ConditionsOwner))
                    {
                        NewUnlocks = true;

                        foreach (string UnlockMessage in ActiveMission.Unlock(ConditionsOwner))
                        {
                            if (ActiveMission.UnlockConditions.ListUnlockConditions.Count > 0)
                            {
                                ListPendingUnlocks.Add(new PendingUnlockScreen(UnlockMessage));
                            }
                        }
                    }
                }
            }

            if (NewUnlocks)
            {
                ConditionsOwner.SaveLocally();
            }

            return ListPendingUnlocks;
        }

        public bool UpdateAvailableItemsIfNeeded()
        {
            if (!IsInit)
            {
                UpdateAvailableItemsTask();
            }

            return IsInit;
        }

        private void UpdateCharacterUnlockStatus(UnlockableCharacter ShopCharacter)
        {
            bool Valid = IsValid(ShopCharacter.UnlockConditions, ConditionsOwner);
            if (Valid)
            {
                if (!ConditionsOwner.UnlockInventory.ListUnlockedCharacter.Contains(ShopCharacter))
                {
                    ConditionsOwner.UnlockInventory.ListUnlockedCharacter.Add(ShopCharacter);
                }

                ConditionsOwner.UnlockInventory.ListLockedCharacter.Remove(ShopCharacter);
            }
            else
            {
                if (!ConditionsOwner.UnlockInventory.ListLockedCharacter.Contains(ShopCharacter))
                {
                    ConditionsOwner.UnlockInventory.ListLockedCharacter.Add(ShopCharacter);
                }

                ConditionsOwner.UnlockInventory.ListUnlockedCharacter.Remove(ShopCharacter);
            }
        }

        private void UpdateMissionUnlockStatus(UnlockableMission ShopMission)
        {
            bool Valid = IsValid(ShopMission.UnlockConditions, ConditionsOwner);
            if (Valid)
            {
                if (!ConditionsOwner.UnlockInventory.ListUnlockedMission.Contains(ShopMission))
                {
                    ConditionsOwner.UnlockInventory.ListUnlockedMission.Add(ShopMission);
                }

                ConditionsOwner.UnlockInventory.ListLockedMission.Remove(ShopMission);
            }
            else
            {
                if (!ConditionsOwner.UnlockInventory.ListLockedMission.Contains(ShopMission))
                {
                    ConditionsOwner.UnlockInventory.ListLockedMission.Add(ShopMission);
                }

                ConditionsOwner.UnlockInventory.ListUnlockedMission.Remove(ShopMission);
            }
        }

        private void UpdateAvailableItemsTask()
        {
            foreach (UnlockableCharacter ActiveCharacter in BattleMapPlayerShopUnlockInventory.CharacterLoader.Database.Values)
            {
                UpdateCharacterUnlockStatus(ActiveCharacter);
            }

            foreach (UnlockableMission ActiveMission in BattleMapPlayerShopUnlockInventory.MissionLoader.Database.Values)
            {
                UpdateMissionUnlockStatus(ActiveMission);
            }

            foreach (UnlockableUnit ActiveUnit in BattleMapPlayerShopUnlockInventory.UnitLoader.Database.Values)
            {
                UnlockableUnit PlayerUnit = ConditionsOwner.UnlockInventory.FindPlayerUnit(ActiveUnit.Path);
                if (PlayerUnit == null)
                {
                    PlayerUnit = ActiveUnit.Copy();
                }

                bool IsUnlocked = IsValid(PlayerUnit.UnlockConditions, ConditionsOwner);
                BattleMapPlayerShopUnlockInventory.UnitUnlockContainer Root = ConditionsOwner.UnlockInventory.RootUnitContainer;
                if (IsUnlocked)
                {
                    Root.AddToUnlocked(PlayerUnit);
                }
                else
                {
                    Root.AddToLocked(PlayerUnit);
                }

                UpdateAvailableUnitSkinsTask(PlayerUnit);

                string[] Tags = PlayerUnit.UnitToBuy.UnitTags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveTag in Tags)
                {
                    BattleMapPlayerShopUnlockInventory.UnitUnlockContainer CurrentFolder = Root;
                    string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ActiveFolder in SubFolders)
                    {
                        CurrentFolder = CurrentFolder.GetOrAddFolder(ActiveFolder);
                        if (IsUnlocked)
                        {
                            CurrentFolder.AddToUnlocked(PlayerUnit);
                        }
                        else
                        {
                            CurrentFolder.AddToLocked(PlayerUnit);
                        }
                    }
                }
            }

            lock (PopulatePlayerItemsTaskLockObject)
            {
                IsInit = true;
            }
        }

        private void UpdateAvailableUnitSkinsTask(UnlockableUnit NewUnit)
        {
            foreach (UnlockableUnitSkin NewSkin in BattleMapPlayerShopUnlockInventory.UnitSkinLoader.Database.Values)
            {
                if (NewUnit.Path != NewSkin.Path)
                    continue;

                bool IsUnitValid = IsValid(NewSkin.UnlockConditions, ConditionsOwner);

                if (IsUnitValid)
                {
                    if (!NewUnit.ListUnlockedSkin.Contains(NewSkin))
                    {
                        NewUnit.ListUnlockedSkin.Add(NewSkin);
                    }

                    NewUnit.ListLockedSkin.Remove(NewSkin);
                }
                else
                {
                    if (!NewUnit.ListLockedSkin.Contains(NewSkin))
                    {
                        NewUnit.ListLockedSkin.Add(NewSkin);
                    }

                    NewUnit.ListUnlockedSkin.Remove(NewSkin);
                }
            }

            foreach (UnlockableUnitAlt NewAlt in BattleMapPlayerShopUnlockInventory.UnitAltLoader.Database.Values)
            {
                if (NewUnit.Path != NewAlt.Path)
                    continue;

                bool IsUnitValid = IsValid(NewAlt.UnlockConditions, ConditionsOwner);

                if (IsUnitValid)
                {
                    if (!NewUnit.ListUnlockedAlt.Contains(NewAlt))
                    {
                        NewUnit.ListUnlockedAlt.Add(NewAlt);
                    }

                    NewUnit.ListLockedAlt.Remove(NewAlt);
                }
                else
                {
                    if (!NewUnit.ListLockedAlt.Contains(NewAlt))
                    {
                        NewUnit.ListLockedAlt.Add(NewAlt);
                    }

                    NewUnit.ListUnlockedAlt.Remove(NewAlt);
                }
            }
        }

        public static bool IsValid(ItemUnlockConditions ConditionsToCheck, BattleMapPlayer ConditionsOwner)
        {
            foreach (List<Tuple<string, string>> ActiveRootCondition in ConditionsToCheck.ListUnlockConditions)
            {
                bool AllConditionsValid = true;

                foreach (Tuple<string, string> ActiveCondition in ActiveRootCondition)
                {
                    switch (ActiveCondition.Item1)
                    {
                        case "Campaign Progression":

                            bool CampaignLevelFound = false;

                            foreach (KeyValuePair<string, CampaignRecord> ActiveMission in ConditionsOwner.Records.DicCampaignLevelInformation)
                            {
                                if (ActiveMission.Value.Name == ActiveCondition.Item2)
                                {
                                    CampaignLevelFound = true;
                                    break;
                                }
                            }

                            if (!CampaignLevelFound)
                            {
                                AllConditionsValid = false;
                            }
                            break;

                        case "Play Time":
                            double SecondsToReach = GetSecondsFromTime(ActiveCondition.Item2);
                            if (ConditionsOwner.Records.TotalSecondsPlayed < SecondsToReach)
                            {
                                AllConditionsValid = false;
                            }
                            break;

                        case "Game Played":

                            break;

                        case "Player Level":
                            if (ConditionsOwner.Level < int.Parse(ActiveCondition.Item2))
                            {
                                AllConditionsValid = false;
                            }
                            break;

                        case "Total Kills":
                            if (ConditionsOwner.Records.TotalKills < int.Parse(ActiveCondition.Item2))
                            {
                                AllConditionsValid = false;
                            }
                            break;
                    }
                }

                if (AllConditionsValid)
                {
                    return true;
                }
            }

            return ConditionsToCheck.ListUnlockConditions.Count == 0;
        }

        private static double GetSecondsFromTime(string Time)
        {
            string[] ArrayTimeSection = Time.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            double TotalTime = 0;

            for (int T = 0; T < ArrayTimeSection.Length; T += 2)
            {
                double TimeValue = double.Parse(ArrayTimeSection[T]);
                double TimeMultiplier = 1;
                switch (ArrayTimeSection[T + 1])
                {
                    case "minute":
                    case "minutes":
                        TimeMultiplier = 60f;
                        break;

                    case "hour":
                    case "hours":
                        TimeMultiplier = 3600f;
                        break;
                }

                TotalTime += TimeValue * TimeMultiplier;
            }

            return TotalTime;
        }
    }
}
