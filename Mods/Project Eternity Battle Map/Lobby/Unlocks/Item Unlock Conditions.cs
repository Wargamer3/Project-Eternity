using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Characters;
using System;
using System.Collections.Generic;

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
        List<PendingUnlockScreen> Evaluate(ContentManager Content);
    }

    public class BattleMapItemUnlockConditionsEvaluator : ItemUnlockConditionsEvaluator
    {
        private BattleMapPlayer ConditionsOwner;

        public BattleMapItemUnlockConditionsEvaluator(BattleMapPlayer ConditionsOwner)
        {
            this.ConditionsOwner = ConditionsOwner;
        }

        public List<PendingUnlockScreen> Evaluate(ContentManager Content)
        {
            List<PendingUnlockScreen> ListPendingUnlocks = new List<PendingUnlockScreen>();

            if (BattleMapPlayerShopInventory.DatabaseLoaded && ConditionsOwner.ShopInventory.HasFinishedReadingPlayerShopItems)
            {
                if (!ConditionsOwner.ShopInventory.IsInit)
                {
                    ConditionsOwner.ShopInventory.UpdateAvailableItems();
                }

                for (int C = ConditionsOwner.ShopInventory.ListLockedCharacter.Count - 1; C >= 0; C--)
                {
                    ShopItemCharacter ActiveCharacter = ConditionsOwner.ShopInventory.ListLockedCharacter[C];
                    if (IsValid(ActiveCharacter.UnlockConditions))
                    {
                        ActiveCharacter.UnlockConditions.IsUnlocked = true;
                        ConditionsOwner.ShopInventory.ListLockedCharacter.Remove(ActiveCharacter);
                        ConditionsOwner.ShopInventory.ListUnlockedCharacter.Add(ActiveCharacter);
                        ListPendingUnlocks.Add(new PendingUnlockScreen(ActiveCharacter.CharacterToBuy.Name + " is now available in the shop!"));

                        if (ActiveCharacter.UnlockQuantity > 0)
                        {
                            Character NewCharacter = new Character(ActiveCharacter.Path, Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                            ConditionsOwner.Inventory.ListOwnedCharacter.Add(NewCharacter);
                            ListPendingUnlocks.Add(new PendingUnlockScreen("You just received " + ActiveCharacter.UnlockQuantity + "x " + NewCharacter.Name + "!"));
                        }
                    }
                }

                for (int U = ConditionsOwner.ShopInventory.ListLockedUnit.Count - 1; U >= 0; U--)
                {
                    ShopItemUnit ActiveUnit = ConditionsOwner.ShopInventory.ListLockedUnit[U];
                }

                for (int M = ConditionsOwner.ShopInventory.ListLockedMission.Count - 1; M >= 0; M--)
                {
                    ShopItemMission ActiveMission = ConditionsOwner.ShopInventory.ListLockedMission[M];
                }
            }

            if (BattleMapPlayerUnlockInventory.DatabaseLoaded && ConditionsOwner.UnlockInventory.HasFinishedReadingPlayerShopItems)
            {
                if (!ConditionsOwner.UnlockInventory.IsInit)
                {
                    ConditionsOwner.UnlockInventory.UpdateAvailableItems();
                }

                for (int C = ConditionsOwner.UnlockInventory.ListLockedCharacter.Count - 1; C >= 0; C--)
                {
                    UnlockableCharacter ActiveCharacter = ConditionsOwner.UnlockInventory.ListLockedCharacter[C];
                    if (IsValid(ActiveCharacter.UnlockConditions))
                    {
                        ActiveCharacter.UnlockConditions.IsUnlocked = true;
                        ConditionsOwner.UnlockInventory.ListLockedCharacter.Remove(ActiveCharacter);
                        ConditionsOwner.UnlockInventory.ListUnlockedCharacter.Add(ActiveCharacter);
                        Character NewCharacter = new Character(ActiveCharacter.Path, Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                        ConditionsOwner.Inventory.ListOwnedCharacter.Add(NewCharacter);
                        ListPendingUnlocks.Add(new PendingUnlockScreen("You just received " + ActiveCharacter.UnlockQuantity + "x " + NewCharacter.Name + "!"));
                    }
                }

                for (int U = ConditionsOwner.UnlockInventory.ListLockedUnit.Count - 1; U >= 0; U--)
                {
                    UnlockableUnit ActiveUnit = ConditionsOwner.UnlockInventory.ListLockedUnit[U];
                }

                for (int M = ConditionsOwner.UnlockInventory.ListLockedMission.Count - 1; M >= 0; M--)
                {
                    UnlockableMission ActiveMission = ConditionsOwner.UnlockInventory.ListLockedMission[M];
                }
            }

            return ListPendingUnlocks;
        }

        private bool IsValid(ItemUnlockConditions ConditionsToCheck)
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

                            foreach (MultiplayerRecord ActiveMission in ConditionsOwner.Records.ListCampaignLevelInformation)
                            {
                                if (ActiveMission.Name == ActiveCondition.Item2)
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
                            break;
                    }
                }

                if (AllConditionsValid)
                {
                    return true;
                }
            }

            return true;
        }

        private double GetSecondsFromTime(string Time)
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
