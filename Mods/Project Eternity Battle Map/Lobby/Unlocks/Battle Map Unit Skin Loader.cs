using System.Collections.Generic;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMapPlayerShopUnlockInventory
    {
        public static class UnitSkinLoader
        {
            public static readonly Dictionary<string, UnlockableUnitSkin> Database = new Dictionary<string, UnlockableUnitSkin>();

            private static readonly List<string> ListPrioritizedUnitSkin = new List<string>();
            private static readonly List<string> ListUnitSkinToLoad = new List<string>();

            public static bool HasPending
            {
                get { return ListPrioritizedUnitSkin.Count > 0 || ListUnitSkinToLoad.Count > 0; }
            }

            public static void ReadUnlockHeader(string ItemPath, Dictionary<string, string> DicHeaderValues)
            {
                string FullPath = ItemPath + DicHeaderValues["SkinPath"];
                if (!Database.ContainsKey(FullPath))
                {
                    Database.Add(FullPath, new UnlockableUnitSkin(ItemPath, DicHeaderValues));
                }
                else
                {
                    Database[FullPath].ReadHeaders(DicHeaderValues);
                }
            }

            public static void EnqueueAll()
            {
                foreach (string ItemPath in Database.Keys)
                {
                    Enqueue(ItemPath, false);
                }
            }

            public static UnlockableUnitSkin GetOrAdd(string UnitPath, string SkinPath)
            {
                string FullPath = UnitPath + SkinPath;
                lock (LockObject)
                {
                    UnlockableUnitSkin Found;
                    if (!Database.TryGetValue(FullPath, out Found))
                    {
                        Found = new UnlockableUnitSkin(UnitPath, SkinPath);
                        Database.Add(FullPath, Found);
                        if (IsContentFileRead)
                        {
                            Enqueue(FullPath, false);
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
                    for (int SkinIndex = 0; SkinIndex < ListPrioritizedUnitSkin.Count; ++SkinIndex)
                    {
                        string Path = ListPrioritizedUnitSkin[SkinIndex];
                        UnlockableUnitSkin FoundSkin;
                        if (Database.TryGetValue(Path, out FoundSkin) && FoundSkin.UnitSkinToBuy != null)
                        {
                            continue;
                        }

                        if (!ListUnitSkinToLoad.Contains(Path))
                        {
                            ListUnitSkinToLoad.Add(Path);
                        }
                    }

                    ListPrioritizedUnitSkin.Clear();
                }
            }

            public static void Prioritize(List<UnlockableUnitSkin> ListSkin)
            {
                for (int SkinIndex = 0; SkinIndex < ListSkin.Count; ++SkinIndex)
                {
                    UnlockableUnitSkin ActiveSkin = ListSkin[SkinIndex];
                    Prioritize(ActiveSkin.Path + ActiveSkin.SkinTypeAndPath);
                }
            }

            public static void Prioritize(string Path)
            {
                lock (LockObject)
                {
                    UnlockableUnitSkin Found;
                    if (!Database.TryGetValue(Path, out Found))
                    {
                        return;
                    }

                    if (Found.UnitSkinToBuy != null)
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
                    if (ListPrioritizedUnitSkin.Count == 0)
                    {
                        return false;
                    }

                    Path = ListPrioritizedUnitSkin[0];
                    ListPrioritizedUnitSkin.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            public static bool LoadNext()
            {
                string Path;
                lock (LockObject)
                {
                    if (ListUnitSkinToLoad.Count == 0)
                    {
                        return false;
                    }

                    Path = ListUnitSkinToLoad[0];
                    ListUnitSkinToLoad.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            private static void Enqueue(string Path, bool Priority)
            {
                UnlockableUnitSkin Found;
                if (Database.TryGetValue(Path, out Found) && Found.UnitSkinToBuy != null)
                {
                    return;
                }

                if (ListPrioritizedUnitSkin.Contains(Path))
                {
                    return;
                }

                if (ListUnitSkinToLoad.Contains(Path))
                {
                    if (Priority)
                    {
                        ListUnitSkinToLoad.Remove(Path);
                        ListPrioritizedUnitSkin.Add(Path);
                    }

                    return;
                }

                if (Priority)
                {
                    ListPrioritizedUnitSkin.Add(Path);
                }
                else
                {
                    ListUnitSkinToLoad.Add(Path);
                }
            }

            private static void Load(string Path)
            {
                UnlockableUnitSkin SkinToLoad;
                lock (LockObject)
                {
                    SkinToLoad = Database[Path];
                }

                Unit LoadedSkin = Unit.FromFullName(SkinToLoad.SkinTypeAndPath, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                SkinToLoad.UnitSkinToBuy = LoadedSkin;
            }
        }
    }
}
