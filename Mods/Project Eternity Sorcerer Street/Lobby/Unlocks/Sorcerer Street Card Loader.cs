using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public partial class SorcererStreetPlayerUnlockInventory
    {
        public static class CardLoader
        {
            public static readonly Dictionary<string, UnlockableCard> Database = new Dictionary<string, UnlockableCard>();

            private static readonly List<string> ListPrioritizedCard = new List<string>();
            private static readonly List<string> ListCardToLoad = new List<string>();

            public static bool HasPending
            {
                get { return ListPrioritizedCard.Count > 0 || ListCardToLoad.Count > 0; }
            }

            public static void ReadUnlockHeader(string ItemPath, Dictionary<string, string> DicHeaderValues)
            {
                if (!Database.ContainsKey(ItemPath))
                {
                    Database.Add(ItemPath, new UnlockableCard(ItemPath, DicHeaderValues));
                }
                else
                {
                    Database[ItemPath].ReadHeaders(DicHeaderValues);
                }
            }

            public static void EnqueueAll()
            {
                foreach (string ItemPath in Database.Keys)
                {
                    Enqueue(ItemPath, false);
                }
            }

            public static UnlockableCard GetOrAdd(string ItemPath)
            {
                lock (LockObject)
                {
                    UnlockableCard Found;
                    if (!Database.TryGetValue(ItemPath, out Found))
                    {
                        Found = new UnlockableCard(ItemPath);
                        Database.Add(ItemPath, Found);
                        if (IsContentFileRead)
                        {
                            Enqueue(ItemPath, false);
                            AreAssetsAllLoaded = false;
                        }
                    }

                    return Found;
                }
            }

            public static void ClearPriority()
            {
                lock (LockObject)
                {
                    for (int CardIndex = 0; CardIndex < ListPrioritizedCard.Count; ++CardIndex)
                    {
                        string Path = ListPrioritizedCard[CardIndex];
                        UnlockableCard FoundCard;
                        if (Database.TryGetValue(Path, out FoundCard) && FoundCard.CardToBuy != null)
                        {
                            continue;
                        }

                        if (!ListCardToLoad.Contains(Path))
                        {
                            ListCardToLoad.Add(Path);
                        }
                    }

                    ListPrioritizedCard.Clear();
                }
            }

            public static void Prioritize(List<UnlockableCard> ListCard)
            {
                for (int CardIndex = 0; CardIndex < ListCard.Count; ++CardIndex)
                {
                    Prioritize(ListCard[CardIndex].Path);
                }
            }

            public static void Prioritize(string Path)
            {
                lock (LockObject)
                {
                    UnlockableCard Found;
                    if (!Database.TryGetValue(Path, out Found))
                    {
                        return;
                    }

                    if (Found.CardToBuy != null)
                    {
                        return;
                    }

                    Enqueue(Path, true);
                }
            }

            public static bool LoadNextPriority()
            {
                string Path;
                lock (LockObject)
                {
                    if (ListPrioritizedCard.Count == 0)
                    {
                        return false;
                    }

                    Path = ListPrioritizedCard[0];
                    ListPrioritizedCard.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            public static bool LoadNext()
            {
                string Path;
                lock (LockObject)
                {
                    if (ListCardToLoad.Count == 0)
                    {
                        return false;
                    }

                    Path = ListCardToLoad[0];
                    ListCardToLoad.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            private static void Enqueue(string Path, bool Priority)
            {
                UnlockableCard Found;
                if (Database.TryGetValue(Path, out Found) && Found.CardToBuy != null)
                {
                    return;
                }

                if (ListPrioritizedCard.Contains(Path))
                {
                    return;
                }

                if (ListCardToLoad.Contains(Path))
                {
                    if (Priority)
                    {
                        ListCardToLoad.Remove(Path);
                        ListPrioritizedCard.Add(Path);
                    }

                    return;
                }

                if (Priority)
                {
                    ListPrioritizedCard.Add(Path);
                }
                else
                {
                    ListCardToLoad.Add(Path);
                }
            }

            private static void Load(string Path)
            {
                UnlockableCard CardToLoad;
                lock (LockObject)
                {
                    CardToLoad = Database[Path];
                }

                Card LoadedCard = Card.LoadCard(CardToLoad.Path, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                CardToLoad.CardToBuy = LoadedCard;
            }
        }
    }
}
