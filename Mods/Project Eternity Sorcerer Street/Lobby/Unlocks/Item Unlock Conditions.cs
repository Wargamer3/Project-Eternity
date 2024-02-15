using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetUnlockConditionsEvaluator : ItemUnlockConditionsEvaluator
    {
        private Player ConditionsOwner;

        public bool IsInit;
        public Task PopulatePlayerItemsTask;
        private object PopulatePlayerItemsTaskLockObject = new object();

        public SorcererStreetUnlockConditionsEvaluator(Player ConditionsOwner)
        {
            this.ConditionsOwner = ConditionsOwner;
        }

        public List<GameScreen> Evaluate(ContentManager Content)
        {
            List<GameScreen> ListPendingUnlocks = new List<GameScreen>();
            bool NewUnlocks = false;

            if (SorcererStreetPlayerUnlockInventory.DatabaseLoaded && ConditionsOwner.UnlockInventory.HasFinishedReadingPlayerShopItems)
            {
                if (!UpdateAvailableItemsIfNeeded())
                {
                    return ListPendingUnlocks;
                }

                for (int U = ConditionsOwner.UnlockInventory.RootBookContainer.ListLockedBook.Count - 1; U >= 0; U--)
                {
                    UnlockableBook ActiveBook = ConditionsOwner.UnlockInventory.RootBookContainer.ListLockedBook[U];
                    if (IsValid(ActiveBook.UnlockConditions, ConditionsOwner))
                    {
                        NewUnlocks = true;

                        foreach (string UnlockMessage in ActiveBook.Unlock(ConditionsOwner))
                        {
                            if (ActiveBook.UnlockConditions.ListUnlockConditions.Count > 0)
                            {
                                ListPendingUnlocks.Add(new PendingUnlockScreen(UnlockMessage));
                            }
                        }
                    }
                }

                for (int U = ConditionsOwner.UnlockInventory.RootCharacterContainer.ListLockedCharacter.Count - 1; U >= 0; U--)
                {
                    UnlockablePlayerCharacter ActiveUnit = ConditionsOwner.UnlockInventory.RootCharacterContainer.ListLockedCharacter[U];
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
            UpdateAvailableBooksTask();
            UpdateAvailableCharactersTask();

            lock (PopulatePlayerItemsTaskLockObject)
            {
                IsInit = true;
            }
        }

        private void UpdateAvailableBooksTask()
        {
            ConditionsOwner.UnlockInventory.RootBookContainer.ListLockedBook = new List<UnlockableBook>(SorcererStreetPlayerUnlockInventory.DicBookDatabase.Values);

            foreach (UnlockableBook NewBook in SorcererStreetPlayerUnlockInventory.DicBookDatabase.Values)
            {
                SorcererStreetPlayerUnlockInventory.BookUnlockContainer CurrentBookContainer = ConditionsOwner.UnlockInventory.RootBookContainer;
                bool IsBookValid = IsValid(NewBook.UnlockConditions, ConditionsOwner);

                if (IsBookValid)
                {
                    if (!CurrentBookContainer.ListUnlockedBook.Contains(NewBook))
                    {
                        CurrentBookContainer.ListUnlockedBook.Add(NewBook);
                    }

                    CurrentBookContainer.ListLockedBook.Remove(NewBook);
                }
                else
                {
                    if (!CurrentBookContainer.ListLockedBook.Contains(NewBook))
                    {
                        CurrentBookContainer.ListLockedBook.Add(NewBook);
                    }

                    CurrentBookContainer.ListUnlockedBook.Remove(NewBook);
                }

                string[] Tags = NewBook.BookToBuy.Tags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveTag in Tags)
                {
                    CurrentBookContainer = ConditionsOwner.UnlockInventory.RootBookContainer;

                    if (IsBookValid)
                    {
                        if (!CurrentBookContainer.ListUnlockedBook.Contains(NewBook))
                        {
                            CurrentBookContainer.ListUnlockedBook.Add(NewBook);
                        }

                        CurrentBookContainer.ListLockedBook.Remove(NewBook);
                    }
                    else
                    {
                        if (!CurrentBookContainer.ListLockedBook.Contains(NewBook))
                        {
                            CurrentBookContainer.ListLockedBook.Add(NewBook);
                        }

                        CurrentBookContainer.ListUnlockedBook.Remove(NewBook);
                    }

                    string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ActiveFolder in SubFolders)
                    {
                        SorcererStreetPlayerUnlockInventory.BookUnlockContainer NewContainer;
                        if (!CurrentBookContainer.DicFolder.TryGetValue(ActiveFolder, out NewContainer))
                        {
                            NewContainer = new SorcererStreetPlayerUnlockInventory.BookUnlockContainer(ActiveFolder);
                            CurrentBookContainer.DicFolder.Add(ActiveFolder, NewContainer);
                            CurrentBookContainer.ListFolder.Add(NewContainer);
                        }

                        CurrentBookContainer = NewContainer;

                        if (IsBookValid)
                        {
                            if (!CurrentBookContainer.ListUnlockedBook.Contains(NewBook))
                            {
                                CurrentBookContainer.ListUnlockedBook.Add(NewBook);
                                CurrentBookContainer.ListLockedBook.Remove(NewBook);
                            }
                        }
                        else
                        {
                            if (!CurrentBookContainer.ListLockedBook.Contains(NewBook))
                            {
                                CurrentBookContainer.ListLockedBook.Add(NewBook);
                                CurrentBookContainer.ListUnlockedBook.Remove(NewBook);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateAvailableCharactersTask()
        {
            ConditionsOwner.UnlockInventory.RootCharacterContainer.ListLockedCharacter = new List<UnlockablePlayerCharacter>(SorcererStreetPlayerUnlockInventory.DicCharacterDatabase.Values);

            foreach (UnlockablePlayerCharacter NewCharacter in SorcererStreetPlayerUnlockInventory.DicCharacterDatabase.Values)
            {
                SorcererStreetPlayerUnlockInventory.CharacterUnlockContainer CurrentCharacterContainer = ConditionsOwner.UnlockInventory.RootCharacterContainer;
                bool IsCharacterValid = IsValid(NewCharacter.UnlockConditions, ConditionsOwner);

                if (IsCharacterValid)
                {
                    if (!CurrentCharacterContainer.ListUnlockedCharacter.Contains(NewCharacter))
                    {
                        CurrentCharacterContainer.ListUnlockedCharacter.Add(NewCharacter);
                    }

                    CurrentCharacterContainer.ListLockedCharacter.Remove(NewCharacter);
                }
                else
                {
                    if (!CurrentCharacterContainer.ListLockedCharacter.Contains(NewCharacter))
                    {
                        CurrentCharacterContainer.ListLockedCharacter.Add(NewCharacter);
                    }

                    CurrentCharacterContainer.ListUnlockedCharacter.Remove(NewCharacter);
                }

                string[] Tags = NewCharacter.CharacterToBuy.Tags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveTag in Tags)
                {
                    CurrentCharacterContainer = ConditionsOwner.UnlockInventory.RootCharacterContainer;

                    if (IsCharacterValid)
                    {
                        if (!CurrentCharacterContainer.ListUnlockedCharacter.Contains(NewCharacter))
                        {
                            CurrentCharacterContainer.ListUnlockedCharacter.Add(NewCharacter);
                        }

                        CurrentCharacterContainer.ListLockedCharacter.Remove(NewCharacter);
                    }
                    else
                    {
                        if (!CurrentCharacterContainer.ListLockedCharacter.Contains(NewCharacter))
                        {
                            CurrentCharacterContainer.ListLockedCharacter.Add(NewCharacter);
                        }

                        CurrentCharacterContainer.ListUnlockedCharacter.Remove(NewCharacter);
                    }

                    string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ActiveFolder in SubFolders)
                    {
                        SorcererStreetPlayerUnlockInventory.CharacterUnlockContainer NewContainer;
                        if (!CurrentCharacterContainer.DicFolder.TryGetValue(ActiveFolder, out NewContainer))
                        {
                            NewContainer = new SorcererStreetPlayerUnlockInventory.CharacterUnlockContainer(ActiveFolder);
                            CurrentCharacterContainer.DicFolder.Add(ActiveFolder, NewContainer);
                            CurrentCharacterContainer.ListFolder.Add(NewContainer);
                        }

                        CurrentCharacterContainer = NewContainer;

                        if (IsCharacterValid)
                        {
                            if (!CurrentCharacterContainer.ListUnlockedCharacter.Contains(NewCharacter))
                            {
                                CurrentCharacterContainer.ListUnlockedCharacter.Add(NewCharacter);
                                CurrentCharacterContainer.ListLockedCharacter.Remove(NewCharacter);
                            }
                        }
                        else
                        {
                            if (!CurrentCharacterContainer.ListLockedCharacter.Contains(NewCharacter))
                            {
                                CurrentCharacterContainer.ListLockedCharacter.Add(NewCharacter);
                                CurrentCharacterContainer.ListUnlockedCharacter.Remove(NewCharacter);
                            }
                        }
                    }
                }
            }
        }

        public static bool IsValid(ItemUnlockConditions ConditionsToCheck, Player ConditionsOwner)
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

        List<GameScreen> ItemUnlockConditionsEvaluator.Evaluate(ContentManager Content)
        {
            throw new NotImplementedException();
        }
    }
}
