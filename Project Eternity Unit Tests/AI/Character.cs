using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.UnitTests.AI
{
    public class Character
    {
        private Dictionary<string, List<Action>> DicActionByGoal;
        private Dictionary<string, object> DicObjectiveByName;

        public Dictionary<string, int> DicGoalByPriority;
        public List<string> ListAvailableGoal;

        public string PrimaryGoal;//Fallback in case actions get interrupted
        public List<Action> ListPrimaryGoalAction;

        public bool CanChangePrimaryGoal;
        public bool PrimaryGoalReached => ListPrimaryGoalAction.Count == 0;

        public bool IsInCombat;
        public string CurrentGoal;
        public string NextGoal;//Wait until Current Goal ends
        public int HP;
        public int Mana;
        public int Money;
        public int Hunger;
        public int Sleep;
        public int Intelligence;
        public Dictionary<string, int> DicExtraStatByName;

        private InventoryContainer RootInventoryContainer;
        private KnowledgeContainer RootKnowledgeContainer;
        public NavMap RootMapContainer;

        public MapInfo CurrentMapName;
        public Vector3 WorldPosition;

        public Character(MapInfo CurrentMapName, Vector3 WorldPosition)
        {
            this.CurrentMapName = CurrentMapName;
            this.WorldPosition = WorldPosition;

            DicActionByGoal = new Dictionary<string, List<Action>>();
            DicObjectiveByName = new Dictionary<string, object>();
            DicExtraStatByName = new Dictionary<string, int>();
            ListPrimaryGoalAction = new List<Action>();

            RootInventoryContainer = new InventoryContainer("Root");
            RootKnowledgeContainer = new KnowledgeContainer("Root");
            RootMapContainer = new NavMap();
        }

        public void AddAction(Action NewAction)
        {
            List<Action> ListAction;
            if (!DicActionByGoal.TryGetValue(NewAction.Goal, out ListAction))
            {
                ListAction = new List<Action>();
                DicActionByGoal.Add(NewAction.Goal, ListAction);
            }

            ListAction.Add(NewAction);
        }

        public void AddMapKnowledge(MapInfo mapInfo)
        {
            RootMapContainer.AddMapContainer(mapInfo);
        }

        public List<Action> GetActionForGoal(string Goal)
        {
            return DicActionByGoal[Goal];
        }

        public void SetObjective(string ObjectiveName, object ObjectiveValue)
        {
            if (!DicObjectiveByName.ContainsKey(ObjectiveName))
            {
                DicObjectiveByName.Add(ObjectiveName, ObjectiveValue);
            }
            else
            {
                DicObjectiveByName[ObjectiveName] = ObjectiveValue;
            }
        }

        public void ClearObjective(string ObjectiveName)
        {
            DicObjectiveByName.Remove(ObjectiveName);
        }

        public object GetObjective(string ObjectiveName)
        {
            object ReturnValue;

            if (DicObjectiveByName.TryGetValue(ObjectiveName, out ReturnValue))
            {
                return ReturnValue;
            }
            else
            {
                return null;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (PrimaryGoalReached)
            {
                FindNextPrimaryGoal(RootMapContainer);
            }
            else
            {
                bool ActionHasFinished = ListPrimaryGoalAction[0].Execute(gameTime, RootMapContainer);

                if (ActionHasFinished)
                {
                    ListPrimaryGoalAction.RemoveAt(0);
                }
            }
        }

        private void FindNextPrimaryGoal(NavMap Map)
        {
            List<Action> ListAvailableAction = new List<Action>();
            int MaxWeight = 1;

            foreach (KeyValuePair<string, List<Action>> ActiveGoal in DicActionByGoal)
            {
                foreach (Action ActiveGoalAction in ActiveGoal.Value)
                {
                    ActiveGoalAction.UpdatePrecondition(string.Empty, null);

                    if (ActiveGoalAction.Weight >= MaxWeight)
                    {
                        ListAvailableAction.Add(ActiveGoalAction);
                        MaxWeight = ActiveGoalAction.Weight;
                    }
                }
            }

            for (int G = ListAvailableAction.Count - 1; G >= 0; G--)
            {
                if (ListAvailableAction[G].Weight < MaxWeight)
                {
                    ListAvailableAction.RemoveAt(G);
                }
            }

            int GoalIndex = 0;

            if (ListAvailableAction.Count > 1)
            {
                GoalIndex = RandomHelper.Next(ListAvailableAction.Count);
            }

            CurrentGoal = ListAvailableAction[GoalIndex].Goal;
            ListPrimaryGoalAction = ListAvailableAction[GoalIndex].GetExecutionPlan(Map);
        }

        public void GoToLocation(string LocationName, NavMap Map)
        {
            KnowledgeInfo LocationKnowledge = RootKnowledgeContainer.DicKnowledgeByName[LocationName];
            Vector3 LocationPosition = (Vector3)LocationKnowledge.KnowledgeDetail;

            List<Action> ListMovementAction = Map.FindPath(this, WorldPosition, LocationPosition);
        }

        public void AddItem(ItemInfo ItemToAdd)
        {
            InventoryContainer CurrentUnitContainer = RootInventoryContainer;

            CurrentUnitContainer.AddItem(ItemToAdd);

            foreach (string ActiveTag in ItemToAdd.ArrayTags)
            {
                CurrentUnitContainer = RootInventoryContainer;

                CurrentUnitContainer.AddItem(ItemToAdd);

                string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveFolder in SubFolders)
                {
                    InventoryContainer NewContainer;
                    if (!CurrentUnitContainer.DicFolderByName.TryGetValue(ActiveFolder, out NewContainer))
                    {
                        NewContainer = new InventoryContainer(ActiveFolder);
                        CurrentUnitContainer.DicFolderByName.Add(ActiveFolder, NewContainer);
                        CurrentUnitContainer.ListFolder.Add(NewContainer);
                    }

                    CurrentUnitContainer = NewContainer;

                    CurrentUnitContainer.AddItem(ItemToAdd);
                }
            }
        }

        public void AddKnowledge(KnowledgeInfo KnowledgeToAdd)
        {
            KnowledgeContainer CurrentUnitContainer = RootKnowledgeContainer;

            CurrentUnitContainer.AddKnowledge(KnowledgeToAdd);

            foreach (string ActiveTag in KnowledgeToAdd.ArrayTags)
            {
                CurrentUnitContainer = RootKnowledgeContainer;

                CurrentUnitContainer.AddKnowledge(KnowledgeToAdd);

                string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveFolder in SubFolders)
                {
                    KnowledgeContainer NewContainer;
                    if (!CurrentUnitContainer.DicFolderByName.TryGetValue(ActiveFolder, out NewContainer))
                    {
                        NewContainer = new KnowledgeContainer(ActiveFolder);
                        CurrentUnitContainer.DicFolderByName.Add(ActiveFolder, NewContainer);
                        CurrentUnitContainer.ListFolder.Add(NewContainer);
                    }

                    CurrentUnitContainer = NewContainer;

                    CurrentUnitContainer.AddKnowledge(KnowledgeToAdd);
                }
            }
        }

        public bool HasItem(string ItemName)
        {
            return RootInventoryContainer.HasItem(ItemName);
        }

        public bool HasItemInCategory(string ItemCategory)
        {
            return RootInventoryContainer.HasItemInCategory(ItemCategory);
        }

        public bool HasKnowledge(string KnowledgeName)
        {
            return RootKnowledgeContainer.HasItem(KnowledgeName);
        }

        public bool HasKnowledgeInCategory(string KnowledgeCategory)
        {
            return RootKnowledgeContainer.HasItemInCategory(KnowledgeCategory);
        }
    }

    public struct InventoryContainer
    {
        internal Dictionary<string, InventoryContainer> DicFolderByName;
        internal List<InventoryContainer> ListFolder;//Share the same folders as the dictionnary
        internal Dictionary<string, ItemInfo> DicItemByName;

        public string Name;

        public InventoryContainer(string Name)
        {
            this.Name = Name;

            DicFolderByName = new Dictionary<string, InventoryContainer>();
            ListFolder = new List<InventoryContainer>();
            DicItemByName = new Dictionary<string, ItemInfo>();
        }

        public bool HasItemInCategory(string Category)
        {
            return DicFolderByName.ContainsKey(Category);
        }

        public bool HasItem(string ItemName)
        {
            return DicItemByName.ContainsKey(ItemName);
        }

        public void AddItem(ItemInfo ItemToAdd)
        {
            if (!DicItemByName.ContainsKey(ItemToAdd.ItemName))
            {
                DicItemByName.Add(ItemToAdd.ItemName, ItemToAdd);
            }
        }

        public void ConsumeItem(ItemInfo ItemToConsume)
        {
            --ItemToConsume.QuantityOwned;

            if (ItemToConsume.QuantityOwned <= 0)
            {
                RemoveItemFromInventory(ItemToConsume);
            }
        }

        private void RemoveItemFromInventory(ItemInfo ItemToRemove)
        {
            DicItemByName.Remove(ItemToRemove.ItemName);

            foreach (InventoryContainer ActiveFolder in ListFolder)
            {
                ActiveFolder.RemoveItemFromInventory(ItemToRemove);
            }
        }
    }

    public struct KnowledgeContainer
    {
        internal Dictionary<string, KnowledgeContainer> DicFolderByName;
        internal List<KnowledgeContainer> ListFolder;//Share the same folders as the dictionnary
        internal Dictionary<string, KnowledgeInfo> DicKnowledgeByName;

        public string Name;

        public KnowledgeContainer(string Name)
        {
            this.Name = Name;

            DicFolderByName = new Dictionary<string, KnowledgeContainer>();
            ListFolder = new List<KnowledgeContainer>();
            DicKnowledgeByName = new Dictionary<string, KnowledgeInfo>();
        }

        public bool HasItemInCategory(string Category)
        {
            return DicFolderByName.ContainsKey(Category);
        }

        public bool HasItem(string ItemName)
        {
            return DicKnowledgeByName.ContainsKey(ItemName);
        }

        public void AddKnowledge(KnowledgeInfo KnowledgeToAdd)
        {
            if (!DicKnowledgeByName.ContainsKey(KnowledgeToAdd.KnowledgeName))
            {
                DicKnowledgeByName.Add(KnowledgeToAdd.KnowledgeName, KnowledgeToAdd);
            }
        }
    }

    public struct ItemInfo
    {
        public string ItemName;
        public string[] ArrayTags;
        public byte QuantityOwned;

        public ItemInfo(string Item, string[] ArrayTags, byte QuantityOwned)
        {
            this.ItemName = Item;
            this.ArrayTags = ArrayTags;
            this.QuantityOwned = QuantityOwned;
        }
    }

    public struct KnowledgeInfo
    {
        public string KnowledgeName;
        public string[] ArrayTags;
        public object KnowledgeDetail;

        public KnowledgeInfo(string KnowledgeName, string[] ArrayTags, object KnowledgeDetail)
        {
            this.KnowledgeName = KnowledgeName;
            this.ArrayTags = ArrayTags;
            this.KnowledgeDetail = KnowledgeDetail;
        }
    }

    public struct BuildingKnowledgeInfo
    {
        public Vector3[] ArrayDoorLocation;
        public Vector3[] ArrayWindowLocation;
        public string MapAreaName;
    }

    public struct MonsterKnowledgeInfo
    {
        public string[] ArrayWeakness;
        public string[] ArrayResistence;
        public string MonsterName;
    }
}
