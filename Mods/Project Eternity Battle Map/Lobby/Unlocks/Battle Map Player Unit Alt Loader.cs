using System.Collections.Generic;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMapPlayerShopUnlockInventory
    {
        public static class UnitAltLoader
        {
            public static readonly Dictionary<string, UnlockableUnitAlt> Database = new Dictionary<string, UnlockableUnitAlt>();

            private static readonly List<string> ListPrioritizedUnitAlt = new List<string>();
            private static readonly List<string> ListUnitAltToLoad = new List<string>();

            public static bool HasPending
            {
                get { return ListPrioritizedUnitAlt.Count > 0 || ListUnitAltToLoad.Count > 0; }
            }

            public static void ReadUnlockHeader(string ItemPath, Dictionary<string, string> DicHeaderValues)
            {
                string FullPath = ItemPath + DicHeaderValues["AltPath"];
                if (!Database.ContainsKey(FullPath))
                {
                    Database.Add(FullPath, new UnlockableUnitAlt(ItemPath, DicHeaderValues));
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

            public static UnlockableUnitAlt GetOrAdd(string UnitPath, string AltPath)
            {
                string FullPath = UnitPath + AltPath;
                lock (LockObject)
                {
                    UnlockableUnitAlt Found;
                    if (!Database.TryGetValue(FullPath, out Found))
                    {
                        Found = new UnlockableUnitAlt(UnitPath, AltPath);
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
                    for (int AltIndex = 0; AltIndex < ListPrioritizedUnitAlt.Count; ++AltIndex)
                    {
                        string Path = ListPrioritizedUnitAlt[AltIndex];
                        UnlockableUnitAlt FoundAlt;
                        if (Database.TryGetValue(Path, out FoundAlt) && FoundAlt.UnitAltToBuy != null)
                        {
                            continue;
                        }

                        if (!ListUnitAltToLoad.Contains(Path))
                        {
                            ListUnitAltToLoad.Add(Path);
                        }
                    }

                    ListPrioritizedUnitAlt.Clear();
                }
            }

            public static void Prioritize(List<UnlockableUnitAlt> ListAlt)
            {
                for (int AltIndex = 0; AltIndex < ListAlt.Count; ++AltIndex)
                {
                    UnlockableUnitAlt ActiveAlt = ListAlt[AltIndex];
                    Prioritize(ActiveAlt.Path + ActiveAlt.AltTypeAndPath);
                }
            }

            public static void Prioritize(string Path)
            {
                lock (LockObject)
                {
                    UnlockableUnitAlt Found;
                    if (!Database.TryGetValue(Path, out Found))
                    {
                        return;
                    }

                    if (Found.UnitAltToBuy != null)
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
                    if (ListPrioritizedUnitAlt.Count == 0)
                    {
                        return false;
                    }

                    Path = ListPrioritizedUnitAlt[0];
                    ListPrioritizedUnitAlt.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            public static bool LoadNext()
            {
                string Path;
                lock (LockObject)
                {
                    if (ListUnitAltToLoad.Count == 0)
                    {
                        return false;
                    }

                    Path = ListUnitAltToLoad[0];
                    ListUnitAltToLoad.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            private static void Enqueue(string Path, bool Priority)
            {
                UnlockableUnitAlt Found;
                if (Database.TryGetValue(Path, out Found) && Found.UnitAltToBuy != null)
                {
                    return;
                }

                if (ListPrioritizedUnitAlt.Contains(Path))
                {
                    return;
                }

                if (ListUnitAltToLoad.Contains(Path))
                {
                    if (Priority)
                    {
                        ListUnitAltToLoad.Remove(Path);
                        ListPrioritizedUnitAlt.Add(Path);
                    }

                    return;
                }

                if (Priority)
                {
                    ListPrioritizedUnitAlt.Add(Path);
                }
                else
                {
                    ListUnitAltToLoad.Add(Path);
                }
            }

            private static void Load(string Path)
            {
                UnlockableUnitAlt AltToLoad;
                lock (LockObject)
                {
                    AltToLoad = Database[Path];
                }

                Unit LoadedAlt = Unit.FromFullName(AltToLoad.AltTypeAndPath, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                AltToLoad.UnitAltToBuy = LoadedAlt;
            }
        }
    }
}
