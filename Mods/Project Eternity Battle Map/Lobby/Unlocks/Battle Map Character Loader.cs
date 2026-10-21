using System.Collections.Generic;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMapPlayerShopUnlockInventory
    {
        public static class CharacterLoader
        {
            public static readonly Dictionary<string, UnlockableCharacter> Database = new Dictionary<string, UnlockableCharacter>();

            private static readonly List<string> ListPrioritizedCharacter = new List<string>();
            private static readonly List<string> ListCharacterToLoad = new List<string>();

            public static bool HasPending
            {
                get { return ListPrioritizedCharacter.Count > 0 || ListCharacterToLoad.Count > 0; }
            }

            public static void ReadUnlockHeader(string ItemPath, Dictionary<string, string> DicHeaderValues)
            {
                if (!Database.ContainsKey(ItemPath))
                {
                    Database.Add(ItemPath, new UnlockableCharacter(ItemPath, DicHeaderValues));
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

            public static UnlockableCharacter GetOrAdd(string ItemPath)
            {
                lock (LockObject)
                {
                    UnlockableCharacter Found;
                    if (!Database.TryGetValue(ItemPath, out Found))
                    {
                        Found = new UnlockableCharacter(ItemPath);
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
                    for (int CharacterIndex = 0; CharacterIndex < ListPrioritizedCharacter.Count; ++CharacterIndex)
                    {
                        string Path = ListPrioritizedCharacter[CharacterIndex];
                        UnlockableCharacter FoundCharacter;
                        if (Database.TryGetValue(Path, out FoundCharacter) && FoundCharacter.CharacterToBuy != null)
                        {
                            continue;
                        }

                        if (!ListCharacterToLoad.Contains(Path))
                        {
                            ListCharacterToLoad.Add(Path);
                        }
                    }

                    ListPrioritizedCharacter.Clear();
                }
            }

            public static void Prioritize(List<UnlockableCharacter> ListCharacter)
            {
                for (int CharacterIndex = 0; CharacterIndex < ListCharacter.Count; ++CharacterIndex)
                {
                    Prioritize(ListCharacter[CharacterIndex].Path);
                }
            }

            public static void Prioritize(string Path)
            {
                lock (LockObject)
                {
                    UnlockableCharacter Found;
                    if (!Database.TryGetValue(Path, out Found))
                    {
                        return;
                    }

                    if (Found.CharacterToBuy != null)
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
                    if (ListPrioritizedCharacter.Count == 0)
                    {
                        return false;
                    }

                    Path = ListPrioritizedCharacter[0];
                    ListPrioritizedCharacter.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            public static bool LoadNext()
            {
                string Path;
                lock (LockObject)
                {
                    if (ListCharacterToLoad.Count == 0)
                    {
                        return false;
                    }

                    Path = ListCharacterToLoad[0];
                    ListCharacterToLoad.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            private static void Enqueue(string Path, bool Priority)
            {
                UnlockableCharacter Found;
                if (Database.TryGetValue(Path, out Found) && Found.CharacterToBuy != null)
                {
                    return;
                }

                if (ListPrioritizedCharacter.Contains(Path))
                {
                    return;
                }

                if (ListCharacterToLoad.Contains(Path))
                {
                    if (Priority)
                    {
                        ListCharacterToLoad.Remove(Path);
                        ListPrioritizedCharacter.Add(Path);
                    }

                    return;
                }

                if (Priority)
                {
                    ListPrioritizedCharacter.Add(Path);
                }
                else
                {
                    ListCharacterToLoad.Add(Path);
                }
            }

            private static void Load(string Path)
            {
                UnlockableCharacter CharacterToLoad;
                lock (LockObject)
                {
                    CharacterToLoad = Database[Path];
                }

                Character LoadedCharacter = new Character(CharacterToLoad.Path, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                LoadedCharacter.Level = 1;
                CharacterToLoad.CharacterToBuy = LoadedCharacter;
            }
        }
    }
}
