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

            if (BattleMapPlayerUnlockInventory.DatabaseLoaded && ConditionsOwner.UnlockInventory.HasFinishedReadingPlayerShopItems)
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
            bool IsFinished = false;
            lock (PopulatePlayerItemsTaskLockObject)
            {
                if (PopulatePlayerItemsTask == null)
                {
                    PopulatePlayerItemsTask = Task.Run(() => { UpdateAvailableItemsTask(); });
                }

                IsFinished = IsInit;
            }

            return IsFinished;
        }

        private void UpdateAvailableItemsTask()
        {
            ConditionsOwner.UnlockInventory.RootUnitContainer.ListLockedUnit = new List<UnlockableUnit>(BattleMapPlayerUnlockInventory.DicUnitDatabase.Values);

            foreach (UnlockableUnit NewUnit in BattleMapPlayerUnlockInventory.DicUnitDatabase.Values)
            {
                BattleMapPlayerUnlockInventory.UnitUnlockContainer CurrentUnitContainer = ConditionsOwner.UnlockInventory.RootUnitContainer;
                bool IsUnitValid = IsValid(NewUnit.UnlockConditions, ConditionsOwner);

                if (IsUnitValid)
                {
                    if (!CurrentUnitContainer.ListUnlockedUnit.Contains(NewUnit))
                    {
                        CurrentUnitContainer.ListUnlockedUnit.Add(NewUnit);
                    }

                    CurrentUnitContainer.ListLockedUnit.Remove(NewUnit);
                }
                else
                {
                    if (!CurrentUnitContainer.ListLockedUnit.Contains(NewUnit))
                    {
                        CurrentUnitContainer.ListLockedUnit.Add(NewUnit);
                    }

                    CurrentUnitContainer.ListUnlockedUnit.Remove(NewUnit);
                }

                string[] Tags = NewUnit.UnitToBuy.UnitTags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveTag in Tags)
                {
                    CurrentUnitContainer = ConditionsOwner.UnlockInventory.RootUnitContainer;

                    if (IsUnitValid)
                    {
                        if (!CurrentUnitContainer.ListUnlockedUnit.Contains(NewUnit))
                        {
                            CurrentUnitContainer.ListUnlockedUnit.Add(NewUnit);
                        }

                        CurrentUnitContainer.ListLockedUnit.Remove(NewUnit);
                    }
                    else
                    {
                        if (!CurrentUnitContainer.ListLockedUnit.Contains(NewUnit))
                        {
                            CurrentUnitContainer.ListLockedUnit.Add(NewUnit);
                        }

                        CurrentUnitContainer.ListUnlockedUnit.Remove(NewUnit);
                    }

                    string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ActiveFolder in SubFolders)
                    {
                        BattleMapPlayerUnlockInventory.UnitUnlockContainer NewContainer;
                        if (!CurrentUnitContainer.DicFolder.TryGetValue(ActiveFolder, out NewContainer))
                        {
                            NewContainer = new BattleMapPlayerUnlockInventory.UnitUnlockContainer(ActiveFolder);
                            CurrentUnitContainer.DicFolder.Add(ActiveFolder, NewContainer);
                            CurrentUnitContainer.ListFolder.Add(NewContainer);
                        }

                        CurrentUnitContainer = NewContainer;

                        if (IsUnitValid)
                        {
                            if (!CurrentUnitContainer.ListUnlockedUnit.Contains(NewUnit))
                            {
                                CurrentUnitContainer.ListUnlockedUnit.Add(NewUnit);
                                CurrentUnitContainer.ListLockedUnit.Remove(NewUnit);
                            }
                        }
                        else
                        {
                            if (!CurrentUnitContainer.ListLockedUnit.Contains(NewUnit))
                            {
                                CurrentUnitContainer.ListLockedUnit.Add(NewUnit);
                                CurrentUnitContainer.ListUnlockedUnit.Remove(NewUnit);
                            }
                        }
                    }
                }
            }

            lock (PopulatePlayerItemsTaskLockObject)
            {
                IsInit = true;
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
