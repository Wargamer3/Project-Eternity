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

        public SorcererStreetUnlockConditionsEvaluator(Player ConditionsOwner)
        {
            this.ConditionsOwner = ConditionsOwner;
        }

        public List<GameScreen> Evaluate(ContentManager Content)
        {
            List<GameScreen> ListPendingUnlocks = new List<GameScreen>();
            bool NewUnlocks = false;

            if (SorcererStreetPlayerUnlockInventory.AreAssetsAllLoaded && ConditionsOwner.UnlockInventory.HasFinishedReadingPlayerShopItems)
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

                for (int U = ConditionsOwner.UnlockInventory.RootCardContainer.ListLockedCard.Count - 1; U >= 0; U--)
                {
                    UnlockableCard ActiveCard = ConditionsOwner.UnlockInventory.RootCardContainer.ListLockedCard[U];
                    if (IsValid(ActiveCard.UnlockConditions, ConditionsOwner))
                    {
                        NewUnlocks = true;

                        foreach (string UnlockMessage in ActiveCard.Unlock(ConditionsOwner))
                        {
                            if (ActiveCard.UnlockConditions.ListUnlockConditions.Count > 0)
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
            if (!IsInit)
            {
                UpdateAvailableItemsTask();
            }

            return IsInit;
        }

        private void UpdateAvailableItemsTask()
        {
            UpdateAvailableBooksTask();
            UpdateAvailableCardsTask();
            UpdateAvailableCharactersTask();
            UpdateAvailableMissionsTask();

            PopulatePlayerItemsTask = Task.CompletedTask;
            IsInit = true;
        }

        private void UpdateAvailableBooksTask()
        {
            foreach (UnlockableBook ActiveBookUnlock in SorcererStreetPlayerUnlockInventory.BookLoader.Database.Values)
            {
                UnlockableBook PlayerBook = ConditionsOwner.UnlockInventory.FindPlayerBook(ActiveBookUnlock.Path);
                if (PlayerBook == null)
                {
                    PlayerBook = ActiveBookUnlock.Copy();
                }

                bool IsUnlocked = IsValid(PlayerBook.UnlockConditions, ConditionsOwner);
                SorcererStreetPlayerUnlockInventory.BookUnlockContainer Root = ConditionsOwner.UnlockInventory.RootBookContainer;
                if (IsUnlocked)
                {
                    Root.AddToUnlocked(PlayerBook);
                }
                else
                {
                    Root.AddToLocked(PlayerBook);
                }

                string[] Tags = PlayerBook.BookToBuy.Tags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveTag in Tags)
                {
                    SorcererStreetPlayerUnlockInventory.BookUnlockContainer CurrentFolder = Root;
                    string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ActiveFolder in SubFolders)
                    {
                        CurrentFolder = CurrentFolder.GetOrAddFolder(ActiveFolder);
                        if (IsUnlocked)
                        {
                            CurrentFolder.AddToUnlocked(PlayerBook);
                        }
                        else
                        {
                            CurrentFolder.AddToLocked(PlayerBook);
                        }
                    }
                }
            }
        }

        private void UpdateAvailableCardsTask()
        {
            foreach (UnlockableCard ActiveCardUnlock in SorcererStreetPlayerUnlockInventory.CardLoader.Database.Values)
            {
                UnlockableCard PlayerCard = ConditionsOwner.UnlockInventory.FindPlayerCard(ActiveCardUnlock.Path);
                if (PlayerCard == null)
                {
                    PlayerCard = ActiveCardUnlock.Copy();
                }

                bool IsUnlocked = IsValid(PlayerCard.UnlockConditions, ConditionsOwner);
                SorcererStreetPlayerUnlockInventory.CardUnlockContainer Root = ConditionsOwner.UnlockInventory.RootCardContainer;
                if (IsUnlocked)
                {
                    Root.AddToUnlocked(PlayerCard);
                }
                else
                {
                    Root.AddToLocked(PlayerCard);
                }

                string[] Tags = PlayerCard.CardToBuy.Tags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveTag in Tags)
                {
                    SorcererStreetPlayerUnlockInventory.CardUnlockContainer CurrentFolder = Root;
                    string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ActiveFolder in SubFolders)
                    {
                        CurrentFolder = CurrentFolder.GetOrAddFolder(ActiveFolder);
                        if (IsUnlocked)
                        {
                            CurrentFolder.AddToUnlocked(PlayerCard);
                        }
                        else
                        {
                            CurrentFolder.AddToLocked(PlayerCard);
                        }
                    }
                }
            }
        }

        private void UpdateAvailableCharactersTask()
        {
            foreach (UnlockablePlayerCharacter ActiveCharacterUnlock in SorcererStreetPlayerUnlockInventory.CharacterLoader.Database.Values)
            {
                UnlockablePlayerCharacter PlayerCharacter = ConditionsOwner.UnlockInventory.FindPlayerCharacter(ActiveCharacterUnlock.Path);
                if (PlayerCharacter == null)
                {
                    PlayerCharacter = ActiveCharacterUnlock.Copy();
                }

                bool IsUnlocked = IsValid(PlayerCharacter.UnlockConditions, ConditionsOwner);
                SorcererStreetPlayerUnlockInventory.CharacterUnlockContainer Root = ConditionsOwner.UnlockInventory.RootCharacterContainer;
                if (IsUnlocked)
                {
                    Root.AddToUnlocked(PlayerCharacter);
                }
                else
                {
                    Root.AddToLocked(PlayerCharacter);
                }

                string[] Tags = PlayerCharacter.CharacterToBuy.Tags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveTag in Tags)
                {
                    SorcererStreetPlayerUnlockInventory.CharacterUnlockContainer CurrentFolder = Root;
                    string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ActiveFolder in SubFolders)
                    {
                        CurrentFolder = CurrentFolder.GetOrAddFolder(ActiveFolder);
                        if (IsUnlocked)
                        {
                            CurrentFolder.AddToUnlocked(PlayerCharacter);
                        }
                        else
                        {
                            CurrentFolder.AddToLocked(PlayerCharacter);
                        }
                    }
                }
            }
        }

        private void UpdateAvailableMissionsTask()
        {
            foreach (UnlockableMission ActiveMissionUnlock in SorcererStreetPlayerUnlockInventory.MissionLoader.Database.Values)
            {
                bool Valid = IsValid(ActiveMissionUnlock.UnlockConditions, ConditionsOwner);
                if (Valid)
                {
                    if (!ConditionsOwner.UnlockInventory.ListUnlockedMission.Contains(ActiveMissionUnlock))
                    {
                        ConditionsOwner.UnlockInventory.ListUnlockedMission.Add(ActiveMissionUnlock);
                    }

                    ConditionsOwner.UnlockInventory.ListLockedMission.Remove(ActiveMissionUnlock);
                }
                else
                {
                    if (!ConditionsOwner.UnlockInventory.ListLockedMission.Contains(ActiveMissionUnlock))
                    {
                        ConditionsOwner.UnlockInventory.ListLockedMission.Add(ActiveMissionUnlock);
                    }

                    ConditionsOwner.UnlockInventory.ListUnlockedMission.Remove(ActiveMissionUnlock);
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
    }
}
