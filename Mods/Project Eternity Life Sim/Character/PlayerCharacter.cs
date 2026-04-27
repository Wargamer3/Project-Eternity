using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.Core.Characters.Character;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class QuoteSet
    {
        public List<QuoteSetMap> ListMapQuote;

        public QuoteSet()
        {
            ListMapQuote = new List<QuoteSetMap>();
            ListMapQuote.Add(new QuoteSetMap());//Any Map
        }

        public QuoteSet(BinaryReader BR)
        {
            int ListQuoteCount = BR.ReadByte();
            ListMapQuote = new List<QuoteSetMap>(ListQuoteCount);

            for (int I = 0; I < ListQuoteCount; I++)
            {
                ListMapQuote.Add(new QuoteSetMap(BR));
            }

            if (ListQuoteCount == 0)
            {
                ListMapQuote.Add(new QuoteSetMap());//Any Map
            }
        }

        public void Write(BinaryWriter BW)
        {
            BW.Write((byte)ListMapQuote.Count);
            for (int I = 0; I < ListMapQuote.Count; I++)
            {
                ListMapQuote[I].Write(BW);
            }
        }
    }

    public class QuoteSetMap
    {
        public List<QuoteSetVersus> ListQuoteVersus;

        public QuoteSetMap()
        {
            ListQuoteVersus = new List<QuoteSetVersus>();
            ListQuoteVersus.Add(new QuoteSetVersus());//Any Map
        }

        public QuoteSetMap(BinaryReader BR)
        {
            int ListQuoteCount = BR.ReadByte();
            ListQuoteVersus = new List<QuoteSetVersus>(ListQuoteCount);

            for (int I = 0; I < ListQuoteCount; I++)
            {
                ListQuoteVersus.Add(new QuoteSetVersus(BR));
            }

            if (ListQuoteCount == 0)
            {
                ListQuoteVersus.Add(new QuoteSetVersus());//Any Map
            }
        }

        internal void Write(BinaryWriter BW)
        {
            BW.Write((byte)ListQuoteVersus.Count);
            for (int I = 0; I < ListQuoteVersus.Count; I++)
            {
                ListQuoteVersus[I].Write(BW);
            }
        }
    }

    public class QuoteSetVersus
    {
        public List<string> ListQuote;
        public List<string> ListPortraitPath;

        public QuoteSetVersus()
        {
            ListQuote = new List<string>();
            ListPortraitPath = new List<string>();
        }

        public QuoteSetVersus(BinaryReader BR)
        {
            int ListQuoteCount = BR.ReadByte();
            ListQuote = new List<string>(ListQuoteCount);
            ListPortraitPath = new List<string>(ListQuoteCount);

            for (int I = 0; I < ListQuoteCount; I++)
            {
                ListQuote.Add(BR.ReadString());
                ListPortraitPath.Add(BR.ReadString());
            }
        }

        internal void Write(BinaryWriter BW)
        {
            BW.Write((byte)ListQuote.Count);
            for (int I = 0; I < ListQuote.Count; I++)
            {
                BW.Write(ListQuote[I]);
                BW.Write(ListPortraitPath[I]);
            }
        }
    }

    public class StatsToAsign
    {
        public int PointsRemaning;//Used if Free Boosts
        public PlayerCharacter.CharacterStats[] ArrayStatToChoose;
        public List<PlayerCharacter.CharacterStats> ListStatChoosen;
    }

    public class PlayerCharacter
    {
        public enum CharacterStats { STR, DEX, CON, INT, WIS, CHA, Free }
        public enum CharacterSexes { Male, Female }

        private Dictionary<string, List<AIAction>> DicActionByGoal;
        private Dictionary<string, object> DicObjectiveByName;

        public Dictionary<string, int> DicGoalByPriority;
        public List<string> ListAvailableGoal;

        public string PrimaryGoal;//Fallback in case actions get interrupted
        public List<AIAction> ListFollowingAIAction;
        public ActionPanel ActiveActionPanel;

        public bool CanChangeAIPrimaryGoal;
        public bool AIPrimaryGoalReached => ListFollowingAIAction.Count == 0;

        public List<Weapon> ListWeapon { get; internal set; }

        private Dictionary<string, AIAction> DicActionByKey;

        public string Name;
        public string Description;

        public string SpritPortraitPath;
        public Texture2D SpriteUnit;

        public string SpriteMapPath;
        public Texture2D SpriteMap;
        public UnitMap3D Unit3DSprite;
        public string Model3DPath;
        public AnimatedModel Unit3DModel;

        public CharacterAction[] ArrayCharacterAction;
        public string[] ArrayCharacterActionPath;
        public BaseAutomaticSkill[] ArrayPassiveSkill;
        public BaseAutomaticSkill[] ArrayRelationshipBonus;
        public SkillLevels[] ArrayPilotSkillLevels;
        public string Tags;
        public TagSystem TeamTags;
        public EffectHolder Effects;

        public CharacterAncestry Ancestry;
        public CharacterBackground Background;
        public CharacterClass Class;
        public CharacterDeity Deity;
        public int Age;
        public CharacterSexes CharacterSex;
        public List<Language> ListLanguage;

        public string ID;
        public bool IsInCombat;
        public string CurrentGoal;
        public string NextGoal;//Wait until Current Goal ends
        public int Level;
        public long Exp;
        public int CurrentHP;
        public int MaxHP;
        public int Mana;
        public int Money;
        public int Hunger;
        public int Sleep;
        public int Intelligence;//used for AI

        public int AC;
        public int DC;
        public string DCType;
        public int STR;
        public int CON;
        public int DEX;
        public int INT;
        public int WIS;
        public int CHA;
        public Dictionary<string, ProficiencyLink> DicProficiencyLevelByName;
        public Dictionary<string, int> DicExtraStatByName;
        public Dictionary<string, CharacterSkill> DicSkillByName;

        public Dictionary<string, QuoteSet> DicAttackQuoteSet;

        public QuoteSet[] ArrayBaseQuoteSet = new QuoteSet[6];

        public List<string> ListQuoteSetVersusName;

        private InventoryContainer RootInventoryContainer;
        private KnowledgeContainer RootKnowledgeContainer;
        public NavMapGameManager RootMapContainer;

        public LifeSimParams SharedMapContex;//Context unique to each player
        public MapInfo CurrentMapName;
        public Vector3 WorldPosition;

        public bool AIControlled;

        public PlayerCharacter(MapInfo CurrentMapName, Vector3 WorldPosition)
        {
            this.CurrentMapName = CurrentMapName;
            this.WorldPosition = WorldPosition;

            DicActionByGoal = new Dictionary<string, List<AIAction>>();
            DicObjectiveByName = new Dictionary<string, object>();
            DicExtraStatByName = new Dictionary<string, int>();
            ListFollowingAIAction = new List<AIAction>();

            ArrayRelationshipBonus = new BaseAutomaticSkill[0];

            RootInventoryContainer = new InventoryContainer("Root");
            RootKnowledgeContainer = new KnowledgeContainer("Root");
        }

        public PlayerCharacter(string CharacterPath, ContentManager Content)
        {
            Name = CharacterPath;

            DicActionByGoal = new Dictionary<string, List<AIAction>>();
            DicObjectiveByName = new Dictionary<string, object>();
            DicExtraStatByName = new Dictionary<string, int>();
            ListFollowingAIAction = new List<AIAction>();
            DicProficiencyLevelByName = new Dictionary<string, ProficiencyLink>();

            ArrayRelationshipBonus = new BaseAutomaticSkill[0];

            RootInventoryContainer = new InventoryContainer("Root");
            RootKnowledgeContainer = new KnowledgeContainer("Root");

            FileStream FS = new FileStream("Content/Life Sim/Characters/" + CharacterPath + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();
            ID = string.Empty;

            SpritPortraitPath = BR.ReadString();
            SpriteMapPath = BR.ReadString();
            Model3DPath = BR.ReadString();
            Tags = BR.ReadString();

            Ancestry = new CharacterAncestry(BR.ReadString(), Content);
            Background = new CharacterBackground(BR.ReadString());
            Class = new CharacterClass(BR.ReadString(), Content);
            Deity = new CharacterDeity(BR.ReadString(), Content);
            Age = BR.ReadByte();
            CharacterSex = (CharacterSexes)BR.ReadByte();

            byte ListLanguageCount = BR.ReadByte();
            ListLanguage = new List<Language>(ListLanguageCount);
            for (int L = 0; L < ListLanguageCount; ++L)
            {
                ListLanguage.Add(new Language(BR.ReadString()));
            }

            byte CharacterActionCount = BR.ReadByte();
            ArrayCharacterAction = new CharacterAction[CharacterActionCount];
            ArrayCharacterActionPath = new string[CharacterActionCount];

            for (int S = 0; S < CharacterActionCount; ++S)
            {
                string CharacterActionName = BR.ReadString();
                ArrayCharacterActionPath[S] = CharacterActionName;
                ArrayCharacterAction[S] = new CharacterAction(CharacterActionName, Content);
            }
            
            byte SkillListCount = BR.ReadByte();
            ArrayPassiveSkill = new BaseAutomaticSkill[SkillListCount];
            ArrayPilotSkillLevels = new SkillLevels[SkillListCount];

            for (int S = 0; S < SkillListCount; ++S)
            {
                string RelativePath = BR.ReadString();
            }

            Int32 RelationshipBonusCount = BR.ReadInt32();
            ArrayRelationshipBonus = new BaseAutomaticSkill[RelationshipBonusCount];

            for (int S = 0; S < RelationshipBonusCount; ++S)
            {
                string RelationshipBonusName = BR.ReadString();
            }
            /*
            int ListQuoteSetVersusNameCount = BR.ReadInt32();
            int ListMapQuoteCount = BR.ReadInt32();
            int ListBaseCount = BR.ReadInt32();
            */
            FS.Close();
            BR.Close();

            if (Content != null)
            {
                string FinalSpriteMapPath = "\\Map Sprite\\" + CharacterPath;
                if (!string.IsNullOrEmpty(SpriteMapPath))
                    FinalSpriteMapPath = "\\Map Sprite\\" + SpriteMapPath;

                string FinalSpriteUnitPath = "\\Unit Sprite\\" + CharacterPath;
                if (!string.IsNullOrEmpty(SpritPortraitPath))
                    FinalSpriteUnitPath = "\\Unit Sprite\\" + SpritPortraitPath;

                string UnitDirectory = Path.GetDirectoryName("Content/Life Sim/Units/Normal/");
                string XNADirectory = UnitDirectory.Substring(8);

                if (File.Exists(UnitDirectory + FinalSpriteMapPath + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>(XNADirectory + FinalSpriteMapPath);
                else
                    SpriteMap = Content.Load<Texture2D>("Life Sim/Units/Default");

                if (File.Exists(UnitDirectory + FinalSpriteUnitPath + ".xnb"))
                    SpriteUnit = Content.Load<Texture2D>(XNADirectory + FinalSpriteUnitPath);
                else
                    SpriteUnit = Content.Load<Texture2D>("Life Sim/Units/Default");

                if (!string.IsNullOrEmpty(Model3DPath))
                {
                    string[] ArrayModelFolder = Model3DPath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    string ModelFolder = Model3DPath.Replace(ArrayModelFolder[ArrayModelFolder.Length - 1], "");
                    Unit3DModel = new AnimatedModel("Life Sim/Units/Normal/Unit Models/" + Model3DPath);
                    Unit3DModel.LoadContent(Content);

                    Unit3DModel.AddAnimation("Life Sim/Units/Normal/Unit Models/" + ModelFolder + "Walking", "Walking", Content);
                    Unit3DModel.AddAnimation("Life Sim/Units/Normal/Unit Models/" + ModelFolder + "Idle", "Idle", Content);
                }
            }

            BR.Close();
            FS.Close();
        }

        public void Init()
        {
            SharedMapContex = new LifeSimParams(this);

            Ancestry.Init(SharedMapContex);
            Background.Init(SharedMapContex);
            Class.Init(SharedMapContex);
            Deity.Init(SharedMapContex);

            UpdateStats();
        }

        public void UpdateStats()
        {
            DicProficiencyLevelByName.Clear();
            MaxHP = Ancestry.BaseHP + Class.BaseHP + CON;
            AC = 10 + DEX;
            STR = 0;
            DEX = 0;
            CON = 0;
            INT = 0;
            WIS = 0;
            CHA = 0;

            foreach (StatsToAsign ActiveStatBoost in Ancestry.ListStatBoosts)
            {
                foreach (CharacterStats ActiveStat in ActiveStatBoost.ArrayStatToChoose)
                {
                    switch (ActiveStat)
                    {
                        case CharacterStats.STR:
                            ++STR;
                            break;

                        case CharacterStats.DEX:
                            ++DEX;
                            break;

                        case CharacterStats.CON:
                            ++CON;
                            break;

                        case CharacterStats.INT:
                            ++INT;
                            break;

                        case CharacterStats.WIS:
                            ++WIS;
                            break;

                        case CharacterStats.CHA:
                            ++CHA;
                            break;
                    }
                }
            }

            foreach (StatsToAsign ActiveStatBoost in Background.ListStatBoosts)
            {
                foreach (CharacterStats ActiveStat in ActiveStatBoost.ArrayStatToChoose)
                {
                    switch (ActiveStat)
                    {
                        case CharacterStats.STR:
                            ++STR;
                            break;

                        case CharacterStats.DEX:
                            ++DEX;
                            break;

                        case CharacterStats.CON:
                            ++CON;
                            break;

                        case CharacterStats.INT:
                            ++INT;
                            break;

                        case CharacterStats.WIS:
                            ++WIS;
                            break;

                        case CharacterStats.CHA:
                            ++CHA;
                            break;
                    }
                }
            }

            foreach (StatsToAsign ActiveStatBoost in Class.ListStatBoosts)
            {
                foreach (CharacterStats ActiveStat in ActiveStatBoost.ArrayStatToChoose)
                {
                    switch (ActiveStat)
                    {
                        case CharacterStats.STR:
                            ++STR;
                            break;

                        case CharacterStats.DEX:
                            ++DEX;
                            break;

                        case CharacterStats.CON:
                            ++CON;
                            break;

                        case CharacterStats.INT:
                            ++INT;
                            break;

                        case CharacterStats.WIS:
                            ++WIS;
                            break;

                        case CharacterStats.CHA:
                            ++CHA;
                            break;
                    }
                }
            }

            foreach (KeyValuePair<string, ProficiencyLink> ActiveProficiency in Ancestry.DicProficiencyLevelByName)
            {
                DicProficiencyLevelByName.Add(ActiveProficiency.Key, ActiveProficiency.Value);
            }

            foreach (KeyValuePair<string, ProficiencyLink> ActiveProficiency in Background.DicProficiencyLevelByName)
            {
                DicProficiencyLevelByName.Add(ActiveProficiency.Key, ActiveProficiency.Value);
            }

            foreach (KeyValuePair<string, ProficiencyLink> ActiveProficiency in Class.DicProficiencyLevelByName)
            {
                DicProficiencyLevelByName.Add(ActiveProficiency.Key, ActiveProficiency.Value);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (AIControlled)
            {
                if (AIPrimaryGoalReached)
                {
                    FindNextPrimaryGoal(RootMapContainer);
                }
                else
                {
                    bool ActionHasFinished = ListFollowingAIAction[0].Execute(gameTime, RootMapContainer);

                    if (ActionHasFinished)
                    {
                        ListFollowingAIAction.RemoveAt(0);
                    }
                }
            }
            else
            {
                ActiveActionPanel.Update(gameTime);
            }
        }

        public void AddAction(AIAction NewAction)
        {
            List<AIAction> ListAction;
            if (!DicActionByGoal.TryGetValue(NewAction.AIGoal, out ListAction))
            {
                ListAction = new List<AIAction>();
                DicActionByGoal.Add(NewAction.AIGoal, ListAction);
            }

            ListAction.Add(NewAction);
        }

        public void AddMapKnowledge(MapInfo mapInfo)
        {
            RootMapContainer.AddMapContainer(mapInfo);
        }

        public List<AIAction> GetActionForGoal(string Goal)
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

        private void FindNextPrimaryGoal(NavMapGameManager Map)
        {
            List<AIAction> ListAvailableAction = new List<AIAction>();
            int MaxWeight = 1;

            foreach (KeyValuePair<string, List<AIAction>> ActiveGoal in DicActionByGoal)
            {
                foreach (AIAction ActiveGoalAction in ActiveGoal.Value)
                {
                    ActiveGoalAction.UpdatePrecondition(string.Empty, null);

                    if (ActiveGoalAction.AIWeight >= MaxWeight)
                    {
                        ListAvailableAction.Add(ActiveGoalAction);
                        MaxWeight = ActiveGoalAction.AIWeight;
                    }
                }
            }

            for (int G = ListAvailableAction.Count - 1; G >= 0; G--)
            {
                if (ListAvailableAction[G].AIWeight < MaxWeight)
                {
                    ListAvailableAction.RemoveAt(G);
                }
            }

            int GoalIndex = 0;

            if (ListAvailableAction.Count > 1)
            {
                GoalIndex = RandomHelper.Next(ListAvailableAction.Count);
            }

            CurrentGoal = ListAvailableAction[GoalIndex].AIGoal;
            ListFollowingAIAction = ListAvailableAction[GoalIndex].GetAIExecutionPlan(Map);
        }

        public void GoToLocation(string LocationName, NavMapGameManager Map)
        {
            KnowledgeInfo LocationKnowledge = RootKnowledgeContainer.DicKnowledgeByName[LocationName];
            Vector3 LocationPosition = (Vector3)LocationKnowledge.KnowledgeDetail;

            List<AIAction> ListMovementAction = Map.FindPath(this, WorldPosition, LocationPosition);
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
