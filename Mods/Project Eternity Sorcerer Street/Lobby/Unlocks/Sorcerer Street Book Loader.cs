using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public partial class SorcererStreetPlayerUnlockInventory
    {
        public static class BookLoader
        {
            public static readonly Dictionary<string, UnlockableBook> Database = new Dictionary<string, UnlockableBook>();

            private static readonly List<string> ListPrioritizedBook = new List<string>();
            private static readonly List<string> ListBookToLoad = new List<string>();

            public static bool HasPending
            {
                get { return ListPrioritizedBook.Count > 0 || ListBookToLoad.Count > 0; }
            }

            public static void ReadUnlockHeader(string ItemPath, Dictionary<string, string> DicHeaderValues)
            {
                if (!Database.ContainsKey(ItemPath))
                {
                    Database.Add(ItemPath, new UnlockableBook(ItemPath, DicHeaderValues));
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

            public static UnlockableBook GetOrAdd(string ItemPath)
            {
                lock (LockObject)
                {
                    UnlockableBook Found;
                    if (!Database.TryGetValue(ItemPath, out Found))
                    {
                        Found = new UnlockableBook(ItemPath);
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
                    for (int BookIndex = 0; BookIndex < ListPrioritizedBook.Count; ++BookIndex)
                    {
                        string Path = ListPrioritizedBook[BookIndex];
                        UnlockableBook FoundBook;
                        if (Database.TryGetValue(Path, out FoundBook) && FoundBook.BookToBuy != null)
                        {
                            continue;
                        }

                        if (!ListBookToLoad.Contains(Path))
                        {
                            ListBookToLoad.Add(Path);
                        }
                    }

                    ListPrioritizedBook.Clear();
                }
            }

            public static void Prioritize(List<UnlockableBook> ListBook)
            {
                for (int BookIndex = 0; BookIndex < ListBook.Count; ++BookIndex)
                {
                    Prioritize(ListBook[BookIndex].Path);
                }
            }

            public static void Prioritize(string Path)
            {
                lock (LockObject)
                {
                    UnlockableBook Found;
                    if (!Database.TryGetValue(Path, out Found))
                    {
                        return;
                    }

                    if (Found.BookToBuy != null)
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
                    if (GlobalBook == null || ListPrioritizedBook.Count == 0)
                    {
                        return false;
                    }

                    Path = ListPrioritizedBook[0];
                    ListPrioritizedBook.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            public static bool LoadNext()
            {
                string Path;
                lock (LockObject)
                {
                    if (GlobalBook == null || ListBookToLoad.Count == 0)
                    {
                        return false;
                    }

                    Path = ListBookToLoad[0];
                    ListBookToLoad.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            private static void Enqueue(string Path, bool Priority)
            {
                UnlockableBook Found;
                if (Database.TryGetValue(Path, out Found) && Found.BookToBuy != null)
                {
                    return;
                }

                if (ListPrioritizedBook.Contains(Path))
                {
                    return;
                }

                if (ListBookToLoad.Contains(Path))
                {
                    if (Priority)
                    {
                        ListBookToLoad.Remove(Path);
                        ListPrioritizedBook.Add(Path);
                    }

                    return;
                }

                if (Priority)
                {
                    ListPrioritizedBook.Add(Path);
                }
                else
                {
                    ListBookToLoad.Add(Path);
                }
            }

            private static void Load(string Path)
            {
                UnlockableBook BookToLoad;
                lock (LockObject)
                {
                    BookToLoad = Database[Path];
                }

                CardBook LoadedBook = CardBook.GetCardBook(BookToLoad.Path, GlobalBook, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                BookToLoad.BookToBuy = LoadedBook;
            }
        }
    }
}
