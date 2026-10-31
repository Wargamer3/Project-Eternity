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
    public class StatsToAsign
    {
        public int PointsRemaning;//Used if Free Boosts
        public PlayerCharacter.CharacterStats[] ArrayStatToChoose;
        public List<PlayerCharacter.CharacterStats> ListStatChoosen;
    }

    public partial class PlayerCharacter : Object3D
    {
        public enum CharacterStats { STR, DEX, CON, INT, WIS, CHA, Free }
        public enum CharacterSexes { Male, Female }

        private Dictionary<string, List<AIAction>> DicActionByGoal;
        private Dictionary<string, object> DicObjectiveByName;

        public Dictionary<string, int> DicGoalByPriority;
        public List<string> ListAvailableGoal;

        public string PrimaryGoal;//Fallback in case actions get interrupted
        public ActionPanelHolderLifeSim ListActionMenuChoice;
        public List<AutomatedAction> ListCurrentAIObjectiveStep;

        public bool CanChangeAIPrimaryGoal;
        public bool AIControlled;
        public bool HiddenFromBattle;
        public bool AIPrimaryGoalReached => ListAIObjective.Count == 0 && ListCurrentAIObjectiveStep.Count == 0;
        private double AITimer;

        public List<AIObjective> ListAIObjective;
        public List<Personality> ListPersonality;
        public List<AIAction> ListGrantedAutonomousAction;
        public CharacterBelief Belief;
        public bool CopyLinksFromWorld;
        public float PerceptionRadius;
        public List<ChunkHop> ListPlannedHop;
        public int PlannedHopIndex;
        public bool PassageAttemptFailed;
        public bool TurnBased;
        public int ActionPointsRemaining;
        public bool HasAffordableWindow;
        public double AffordableSeconds;

        public const double IdleRetrySeconds = 60;
        public const int ShortTermUrgencyMargin = 10;
        public const int HighPrecisionUrgencyMargin = 20;
        public const int CriticalHunger = 80;
        public const int CriticalSleep = 80;

        public List<Weapon> ListWeapon { get; internal set; }

        private Dictionary<string, AIAction> DicActionByKey;

        #region Stats

        public string RelativePath;
        public string Name;
        public string Description;

        public string SpritPortraitPath;
        public Texture2D SpriteUnit;

        public string SpriteMapPath;
        public Texture2D SpriteMap;
        public UnitMap3D Unit3DSprite;
        public string Model3DPath;
        public AnimatedModel Unit3DModel;
        public GltfModel BodyModel;
        public BodySprings BodySprings;
        public float BodyScale = 1f;

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
        public CharacterHeritage Heritage;
        public Armor WornArmor;
        public int Age;
        public CharacterSexes CharacterSex;
        public List<Language> ListLanguage;

        public string ID;
        public bool IsInCombat;
        public bool InPractice;
        public string CurrentGoal;
        public string NextGoal;//Wait until Current Goal ends
        public int Level;
        public long Exp;
        public int CurrentHP;
        public int MaxHP;
        public int BonusHP;
        public int Mana;
        public int Money;
        public int Hunger;
        public int Hygiene;
        public int Sleep;
        public int Social;
        public int Comfort;
        public int Bladder;
        public int Fun;
        public float NeedRemainder;
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

        #endregion

        public Dictionary<string, QuoteSet> DicAttackQuoteSet;

        public QuoteSet[] ArrayBaseQuoteSet = new QuoteSet[6];

        public List<string> ListQuoteSetVersusName;

        private InventoryContainer RootInventoryContainer;
        private KnowledgeContainer RootKnowledgeContainer;
        public ActionLogHolder Logs;

        public LifeSimCharacterParams SharedMapContex;//Context unique to each player

        public Vector3 MovementSpeed = Vector3.Zero;
        public Vector3 ExternalSpeed = Vector3.Zero;//Wind, etc

        public float BaseMoveSpeed = 30f;
        public float MovePace = 1f;
        public bool IgnoreTownSolids;
        public bool DungeonPose;
        public float SwimPace = 1f;
        public float WetLine;
        public float WetAmount;
        public bool WasInWater;
        public float WallRunPace = 1f;
        public TraversalCapabilities Body;
        public Pose CurrentPose;
        public NoisePrint Noise;

        public enum SurfaceType { Normal, Ice, Mud }
        public SurfaceType CurrentSurface = SurfaceType.Normal;

        public Camera3D Camera;

        public Color Color;

        public PlayerCharacter(NavMapGameManager MapManager, MapInfo CurrentMapInfo, Vector3 WorldPosition)
        {
            SharedMapContex = new LifeSimCharacterParams(this, MapManager, CurrentMapInfo);
            SetPosition(WorldPosition);

            DicActionByGoal = new Dictionary<string, List<AIAction>>();
            DicObjectiveByName = new Dictionary<string, object>();
            DicExtraStatByName = new Dictionary<string, int>();
            ListCurrentAIObjectiveStep = new List<AutomatedAction>();
            ListAIObjective = new List<AIObjective>();
            ListPersonality = new List<Personality>();
            ListGrantedAutonomousAction = new List<AIAction>();
            Belief = new CharacterBelief();
            PerceptionRadius = 64;
            ListPlannedHop = new List<ChunkHop>();
            Body = new TraversalCapabilities();
            CurrentPose = Pose.Stand;
            ListActionMenuChoice = new ActionPanelHolderLifeSim(MapManager);
            DicProficiencyLevelByName = new Dictionary<string, ProficiencyLink>();

            ArrayRelationshipBonus = new BaseAutomaticSkill[0];

            RootInventoryContainer = new InventoryContainer("Root");
            RootKnowledgeContainer = new KnowledgeContainer("Root");
            Logs = new ActionLogHolder();

            Ancestry = new CharacterAncestry();
            Background = new CharacterBackground();
            Class = new CharacterClass();
            Deity = new CharacterDeity();
        }

        public PlayerCharacter(string CharacterPath, ContentManager Content, NavMapGameManager MapManager = null, MapInfo CurrentMapInfo = null)
        {
            RelativePath = CharacterPath;
            SharedMapContex = new LifeSimCharacterParams(this, MapManager, CurrentMapInfo);

            DicActionByGoal = new Dictionary<string, List<AIAction>>();
            DicObjectiveByName = new Dictionary<string, object>();
            DicExtraStatByName = new Dictionary<string, int>();
            ListCurrentAIObjectiveStep = new List<AutomatedAction>();
            ListAIObjective = new List<AIObjective>();
            ListPersonality = new List<Personality>();
            ListGrantedAutonomousAction = new List<AIAction>();
            Belief = new CharacterBelief();
            PerceptionRadius = 64;
            ListPlannedHop = new List<ChunkHop>();
            Body = new TraversalCapabilities();
            CurrentPose = Pose.Stand;
            ListActionMenuChoice = new ActionPanelHolderLifeSim(MapManager);
            DicProficiencyLevelByName = new Dictionary<string, ProficiencyLink>();

            ArrayRelationshipBonus = new BaseAutomaticSkill[0];

            RootInventoryContainer = new InventoryContainer("Root");
            RootKnowledgeContainer = new KnowledgeContainer("Root");
            Logs = new ActionLogHolder();

            FileStream FS = new FileStream("Content/Life Sim/Characters/" + CharacterPath + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();
            ID = string.Empty;

            SpritPortraitPath = BR.ReadString();
            SpriteMapPath = BR.ReadString();
            Model3DPath = BR.ReadString();
            Tags = BR.ReadString();

            string AncestryPath = BR.ReadString();
            string BackgroundPath = BR.ReadString();
            string ClassPath = BR.ReadString();
            string DeityPath = BR.ReadString();

            if (string.IsNullOrEmpty(AncestryPath))
            {
                Ancestry = new CharacterAncestry();
            }
            else
            {
                Ancestry = new CharacterAncestry(AncestryPath, Content);
            }
            if (string.IsNullOrEmpty(AncestryPath))
            {
                Background = new CharacterBackground();
            }
            else
            {
                Background = new CharacterBackground(BackgroundPath);
            }
            if (string.IsNullOrEmpty(AncestryPath))
            {
                Class = new CharacterClass();
            }
            else
            {
                Class = new CharacterClass(ClassPath, Content);
            }
            if (string.IsNullOrEmpty(AncestryPath))
            {
                Deity = new CharacterDeity();
            }
            else
            {
                Deity = new CharacterDeity(DeityPath, Content);
            }

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
                string FinalSpriteMapPath = "\\Map Sprites\\" + CharacterPath;
                if (!string.IsNullOrEmpty(SpriteMapPath))
                    FinalSpriteMapPath = "\\Map Sprites\\" + SpriteMapPath;

                string FinalSpriteUnitPath = "\\Unit Sprites\\" + CharacterPath;
                if (!string.IsNullOrEmpty(SpritPortraitPath))
                    FinalSpriteUnitPath = "\\Unit Sprites\\" + SpritPortraitPath;

                string UnitDirectory = Path.GetDirectoryName("Content/Life Sim/Characters/");
                string XNADirectory = UnitDirectory.Substring(8);

                if (File.Exists(UnitDirectory + FinalSpriteMapPath + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>(XNADirectory + FinalSpriteMapPath);
                else
                    SpriteMap = Content.Load<Texture2D>("Life Sim/Characters/Default");

                if (File.Exists(UnitDirectory + FinalSpriteUnitPath + ".xnb"))
                    SpriteUnit = Content.Load<Texture2D>(XNADirectory + FinalSpriteUnitPath);
                else
                    SpriteUnit = Content.Load<Texture2D>("Life Sim/Characters/Default");

                string Models = Path.Combine("Content", "Life Sim", "Characters", "Models");
                string Forest = Path.Combine(Models, "Forest Girl.glb");
                string Racoon = Path.Combine(Models, "Racoon Girl.glb");
                Model3DPath = File.Exists(Racoon) ? Forest : File.Exists(Forest) ? Forest : null;
                if (Model3DPath != null && GameScreen.GraphicsDevice != null)
                {
                    WearBody(GameScreen.GraphicsDevice, Model3DPath);
                    RotationMatrix = Matrix.CreateFromAxisAngle(Vector3.UnitY, 0);
                }
            }

            BR.Close();
            FS.Close();
        }

        public void WearBody(GraphicsDevice Device, string ModelPath)
        {
            if (BodyModel != null)
            {
                BodyModel.Dispose();
                BodyModel = null;
            }

            BodySprings = null;
            string Folder = Path.Combine("Content", "Life Sim", "Characters", "Animations");
            string[] Clips = Directory.Exists(Folder) ? Directory.GetFiles(Folder, "*.fbx") : Array.Empty<string>();
            Array.Sort(Clips, StringComparer.OrdinalIgnoreCase);
            BodyModel = new GltfModel(Device, ModelPath, false, Clips);
            BodyModel.CharacterDraw = true;
            BodyModel.Light = ModelLight.CharacterSelection;
            BodySprings = new BodySprings();
            BodySprings.SourcePath = ModelPath;
        }

        public void Init()
        {
            RotationMatrix = Matrix.CreateFromAxisAngle(Vector3.UnitY, 0f);

            Ancestry.Init(SharedMapContex);
            Background.Init(SharedMapContex);
            Class.Init(SharedMapContex);
            Deity.Init(SharedMapContex);

            RootInventoryContainer.Init(SharedMapContex);

            if (ArrayCharacterAction == null)
            {
                ArrayCharacterAction = new CharacterAction[0];
            }

            foreach (CharacterAction ActiveAction in ArrayCharacterAction)
            {
                ActiveAction.Init(SharedMapContex);
            }

            UpdateStats();
            if (CurrentHP == 0 && MaxHP > 0)
            {
                CurrentHP = MaxHP;
            }

            InitAI();
        }

        private void InitAI()
        {
            foreach (CharacterAction ActiveCharacterAction in ArrayCharacterAction)
            {
                foreach (AICharacterAction ActiveCharacterAIAction in ActiveCharacterAction.ListAIAction)
                {
                    foreach (LifeSimAIAction LoadedAIAction in ActiveCharacterAIAction.ListAIAction)
                    {
                        LifeSimAIAction ActiveAIAction = (LifeSimAIAction)LoadedAIAction.Copy();

                        if (!DicActionByGoal.ContainsKey(ActiveAIAction.AIGoal))
                        {
                            DicActionByGoal.Add(ActiveAIAction.AIGoal, new List<AIAction>());
                        }

                        ActiveAIAction.Init(SharedMapContex);

                        DicActionByGoal[ActiveAIAction.AIGoal].Add(ActiveAIAction);
                    }
                }
            }

            DefaultAI.ApplyIfEmpty(this);
        }

        public void UpdateStats()
        {
            if (DicProficiencyLevelByName == null)
            {
                DicProficiencyLevelByName = new Dictionary<string, ProficiencyLink>();
            }

            DicProficiencyLevelByName.Clear();
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

            MaxHP = Ancestry.BaseHP + Class.BaseHP + CON;
            AC = 10 + DEX;

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

            foreach (ProficiencyLink ActiveProficiency in DicProficiencyLevelByName.Values)
            {
                ActiveProficiency.ComputeStatBonuses(this);
            }
        }

        public int GetInitiative(string ProficiencyType)
        {
            int FinalInitiative = 0;

            ProficiencyLink FoundProficiency;
            if (DicProficiencyLevelByName.TryGetValue(ProficiencyType, out FoundProficiency))
            {
                FinalInitiative = FoundProficiency.GetValue();
            }

            return FinalInitiative;
        }

        public void PoseBody(GameTime gameTime)
        {
            if (gameTime == null || DungeonPose || SharedMapContex == null)
            {
                return;
            }

            Vector3 Shift = ChunkOrigin.Shift(SharedMapContex.CurrentMapInfo);
            if (BodyModel != null)
            {
                if (BodySprings != null)
                {
                    BodySprings.World = BodyDrawWorld(Shift);
                    BodySprings.Seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                BodyModel.Update(gameTime);
                FootPlant.Apply(this, Shift);
                if (BodySprings != null)
                {
                    BodySprings.Pose(BodyModel);
                }
            }

            if (Unit3DModel != null)
            {
                Unit3DModel.Update(gameTime);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime != null)
            {
                DungeonCombat.Tick(this, gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (Life.Away.Busy)
            {
                return;
            }

            if (ListActionMenuChoice.HasMainPanel)
            {
                ListActionMenuChoice.Last().Update(gameTime);
                return;
            }

            if (!AIControlled)
            {
                return;
            }

            WatchCommittedPlan();
            DoorYield.Consider(this);
            AdvanceNeeds(gameTime);

            AIObjective Top = TopObjective();
            if (Top != null && Top.ListRemainingStep.Count > 0)
            {
                ListCurrentAIObjectiveStep = Top.ListRemainingStep;
                int Guard = 0;
                while (Guard < 8 && Top.ListRemainingStep.Count > 0)
                {
                    Guard++;
                    ListCurrentAIObjectiveStep = Top.ListRemainingStep;
                    if (ListCurrentAIObjectiveStep[0].Update(gameTime))
                    {
                        break;
                    }

                    if (PassageAttemptFailed)
                    {
                        PassageAttemptFailed = false;
                        RefillSteps(Top);
                        if (Top.ListRemainingStep.Count == 0)
                        {
                            WaitAction Pause = new WaitAction(this, 1);
                            Top.ListRemainingStep.Add(Pause);
                            ListCurrentAIObjectiveStep = Top.ListRemainingStep;
                            Pause.OnStarted();
                        }

                        break;
                    }

                    ListCurrentAIObjectiveStep.RemoveAt(0);
                    if (ListCurrentAIObjectiveStep.Count == 0)
                    {
                        break;
                    }

                    ListCurrentAIObjectiveStep[0].OnStarted();
                    if (ListCurrentAIObjectiveStep[0] is FollowPath || ListCurrentAIObjectiveStep[0] is DriveLegAction)
                    {
                        break;
                    }
                }
            }
            else if (Top != null)
            {
                if (Top.IsStandingOrder && ListAIObjective.Count == 1)
                {
                    RefillStandingOrder(Top);
                }
                else
                {
                    PopTopObjective();
                }
            }

            if (ListAIObjective.Count == 0)
            {
                TryPickIdleGoal(gameTime);
            }

            Top = TopObjective();
            if (!DungeonPose && (Top == null || !IsLocomoting(Top.ListRemainingStep)))
            {
                PlayClip("Idle");
            }
        }

        static bool IsLocomoting(List<AutomatedAction> Steps)
        {
            if (Steps == null || Steps.Count == 0)
            {
                return false;
            }

            return Steps[0] is FollowPath || Steps[0] is DriveLegAction;
        }

        public void RemoveControl()
        {
            if (LifeSimMap.PhysicsManager != null)
            {
                LifeSimMap.PhysicsManager.Release(this);
            }

            SharedMapContex.User = null;
            AIControlled = true;

            AIObjective Top = TopObjective();
            if (Top != null)
            {
                ListCurrentAIObjectiveStep = Top.ListRemainingStep;
            }
        }

        public void TakeControl(PlayerOverseer NewUser)
        {
            SharedMapContex.User = NewUser;
            AIControlled = false;
            ListCurrentAIObjectiveStep = new List<AutomatedAction>();
            if (LifeSimMap.PhysicsManager != null)
            {
                LifeSimMap.PhysicsManager.Adopt(this);
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

        public void Equip(Weapon Weapon)
        {
            if (ListWeapon == null)
            {
                ListWeapon = new List<Weapon>();
            }

            ListWeapon.Clear();
            if (Weapon != null)
            {
                ListWeapon.Add(Weapon);
            }
        }

        public void Hold(Weapon Weapon)
        {
            if (ListWeapon == null)
            {
                ListWeapon = new List<Weapon>();
            }

            if (Weapon != null)
            {
                ListWeapon.Add(Weapon);
            }
        }

        public Weapon EquippedWeapon()
        {
            if (ListWeapon == null || ListWeapon.Count == 0)
            {
                return null;
            }

            return ListWeapon[0];
        }

        public bool HasAnyScript()
        {
            foreach (List<AIAction> ListAction in DicActionByGoal.Values)
            {
                if (ListAction.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasScript(string ActionName)
        {
            return ScriptCount(ActionName) > 0;
        }

        public int ScriptCount(string ActionName)
        {
            int Count = 0;
            foreach (List<AIAction> ListAction in DicActionByGoal.Values)
            {
                foreach (AIAction Action in ListAction)
                {
                    if (Action.AIActionName == ActionName)
                    {
                        Count++;
                    }
                }
            }

            return Count;
        }

        public void AddMapKnowledge(MapInfo mapInfo)
        {
            Belief.LearnArea(mapInfo);
            foreach (ChunkHop Link in mapInfo.ListLink)
            {
                Belief.LearnLink(Link);
            }
        }

        public void SeedBelief()
        {
            MapInfo Here = SharedMapContex.CurrentMapInfo;
            if (Here == null)
            {
                return;
            }

            Belief.LearnArea(Here);
            if (!CopyLinksFromWorld)
            {
                return;
            }

            foreach (ChunkHop Link in Here.ListLink)
            {
                Belief.LearnLink(Link);
            }
        }

        public void Perceive()
        {
            MapInfo Here = SharedMapContex.CurrentMapInfo;
            if (Here == null)
            {
                return;
            }

            Belief.Perceive(Here, Position, PerceptionRadius);
            AccountForWhatWasSeen();
        }

        public bool IsCarrying(string Name)
        {
            return RootInventoryContainer.ContainsTagged(Name);
        }

        public void ConsumeCarried(string Name)
        {
            ItemInfo Found;
            if (RootInventoryContainer.TryFindTagged(Name, out Found))
            {
                RootInventoryContainer.ConsumeItem(Found);
            }
        }

        public List<ChunkHop> GoToLocation(string LocationName)
        {
            MapInfo Start = SharedMapContex.CurrentMapInfo;
            if (Start == null || Belief == null)
            {
                return new List<ChunkHop>();
            }

            string DestinationName = LocationName;
            BelievedObject KnownObject = Belief.FindObject(LocationName);
            if (KnownObject != null && KnownObject.Chunk != null && KnownObject.Chunk.MapMapContainer != null)
            {
                DestinationName = KnownObject.Chunk.MapMapContainer.AreaName;
            }

            return new InterMapMovementAlgorithm().FindHops(Start, DestinationName, Belief);
        }

        public List<AutomatedAction> GetActionForGoal(string Goal)
        {
            List<AutomatedAction> ListAction = new List<AutomatedAction>();
            foreach (AIAction ActiveAIAction in DicActionByGoal[Goal])
            {
                ActiveAIAction.UpdatePrecondition(null, SharedMapContex.RootMapContainer);
                if (ActiveAIAction.AIWeight > 0)
                {
                    ListAction.AddRange(ActiveAIAction.GetAIExecutionPlan(SharedMapContex.RootMapContainer));
                }
            }

            return ListAction;
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

        public void GoToLocation(string LocationName, NavMapGameManager Map)
        {
            GoToLocation(LocationName);
        }

        public void AddItem(ItemInfo ItemToAdd)
        {
            InventoryContainer CurrentUnitContainer = RootInventoryContainer;

            CurrentUnitContainer.AddItem(ItemToAdd);

            foreach (string ActiveTag in ItemToAdd.OwnedItem.ArrayTags)
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
            if (Belief != null && Belief.HasCategory(KnowledgeCategory))
            {
                return true;
            }

            return RootKnowledgeContainer.ContainsCategory(KnowledgeCategory);
        }

        public void Move(Vector3 TargetDirection, double Strength, double DeltaTime)
        {
            Move(TargetDirection, Strength, DeltaTime, null);
        }

        public void Move(Vector3 TargetDirection, double Strength, double DeltaTime, float? FaceYaw)
        {
            float Acceleration = 15;
            float StopSpeed = 18;
            float Friction = 6;
            float TurnSpeed = 12;

            switch (CurrentSurface)
            {
                case SurfaceType.Ice:
                    Acceleration = 3.0f;
                    StopSpeed = 1.2f;
                    Friction = 1.5f;
                    TurnSpeed = 5f;
                    break;

                case SurfaceType.Mud:
                    Acceleration = 6.0f;
                    StopSpeed = 20.0f;
                    Friction = 12.0f;
                    break;
            }

            Vector3 Movement = new Vector3(TargetDirection.X, TargetDirection.Y, 0f) * BaseMoveSpeed * (float)Strength * MovePace * SpeedStripBoost();
            bool Stepping = Strength > 0 && Movement != Vector3.Zero;

            float? Aim = FaceYaw;
            if (!Aim.HasValue && Stepping)
            {
                Aim = (float)Math.Atan2(Movement.X, Movement.Y);
            }

            if (Aim.HasValue)
            {
                float CurrentAngle = ThirdPersonRig.YawOf(RotationMatrix);
                float Delta = MathHelper.WrapAngle(Aim.Value - CurrentAngle);
                float SmoothedTurn = 1.0f - (float)Math.Exp(-TurnSpeed * (float)DeltaTime);
                RotationMatrix = Matrix.CreateFromAxisAngle(Vector3.UnitY, CurrentAngle + Delta * SmoothedTurn);
            }

            if (!IgnoreTownSolids && LifeSimMap.PhysicsManager != null && LifeSimMap.PhysicsManager.Drives(this))
            {
                float Speed = Movement.Length();
                Vector3 Direction = Speed > 0.001f ? Movement / Speed : Vector3.Zero;
                LifeSimMap.PhysicsManager.Steer(this, Direction, Speed);
                PlayClip(Stepping ? "Walking" : "Idle");
                float Sliding = LifeSimMap.PhysicsManager.HorizontalSpeed(this);
                if (Sliding > Speed)
                {
                    Speed = Sliding;
                }

                ProjectEternity.GameScreens.LifeSimScreen.Noise.Stamp(this, Speed);
                return;
            }

            Vector3 Before = Position;
            if (Stepping)
            {
                float SmoothedAcceleration = 1.0f - (float)Math.Exp(-Acceleration * DeltaTime);
                MovementSpeed = Vector3.Lerp(MovementSpeed, Movement, SmoothedAcceleration);
            }
            else
            {
                float SmoothedAcceleration = 1.0f - (float)Math.Exp(-StopSpeed * DeltaTime);
                MovementSpeed = Vector3.Lerp(MovementSpeed, Movement, SmoothedAcceleration);
            }

            MovementSpeed.Z = 0f;

            float SmoothedFriction = 1.0f - (float)Math.Exp(-Friction * DeltaTime);
            Vector2 Gust = Climate.Roof(this) ? Vector2.Zero : Climate.Now.Wind * 0.12f;
            ExternalSpeed = Vector3.Lerp(ExternalSpeed, new Vector3(Gust.X, Gust.Y, 0f), SmoothedFriction);

            Vector3 MovementFinal = MovementSpeed + ExternalSpeed;
            StepBody(MovementFinal * (float)DeltaTime);
            Vector3 Advanced = Position - Before;
            Advanced.Z = 0f;
            bool Travelled = Stepping && Advanced.LengthSquared() > 0.0001f;
            PlayClip(Travelled ? "Walking" : "Idle");
            float FlatSpeed = (float)Math.Sqrt(MovementFinal.X * MovementFinal.X + MovementFinal.Y * MovementFinal.Y);
            ProjectEternity.GameScreens.LifeSimScreen.Noise.Stamp(this, FlatSpeed);
        }

        float SpeedStripBoost()
        {
            if (Belief == null)
            {
                return 1f;
            }

            foreach (BelievedObject Object in Belief.ListObject)
            {
                if (Object.Tag != "Speed Strip")
                {
                    continue;
                }

                float DX = Position.X - Object.LocalPosition.X;
                float DY = Position.Y - Object.LocalPosition.Y;
                if (DX * DX + DY * DY <= 16f * 16f)
                {
                    return 1.5f;
                }
            }

            return 1f;
        }

        public bool TryJump()
        {
            if (Body == null || !Body.CanJump)
            {
                return false;
            }

            if (LifeSimMap.PhysicsManager != null && LifeSimMap.PhysicsManager.Drives(this))
            {
                if (!LifeSimMap.PhysicsManager.Jump(this))
                {
                    return false;
                }

                CurrentPose = Pose.MidAir;
                return true;
            }

            Position = new Vector3(Position.X, Position.Y, Position.Z + 8f);
            CurrentPose = Pose.MidAir;
            return true;
        }

        public void StepBody(Vector3 Displacement)
        {
            Vector3 Next = Position + Displacement;
            bool FlatMove = Displacement.X * Displacement.X + Displacement.Y * Displacement.Y > 0.0000001f;
            if (BoxBlocks(Position, Next))
            {
                if (FlatMove)
                {
                    SolidStoppedMove = true;
                }

                Next = new Vector3(Position.X, Position.Y, Position.Z + Displacement.Z);
                if (BoxBlocks(Position, Next))
                {
                    return;
                }
            }

            Next = new Vector3(Next.X, Next.Y, StairHeight(Next));
            Position = Next;
            WorldMatrix = RotationMatrix * Matrix.CreateTranslation(Position);
        }

        bool BoxBlocks(Vector3 From, Vector3 To)
        {
            if (IgnoreTownSolids)
            {
                return false;
            }

            MapInfo Here = SharedMapContex == null ? null : SharedMapContex.CurrentMapInfo;
            if (Here == null || Here.Furniture == null || Here.Buildings == null)
            {
                return false;
            }

            for (int i = 0; i < Here.Furniture.Count; i++)
            {
                if (Here.Furniture[i] != null && Here.Furniture[i].HitsMove(From, To))
                {
                    return true;
                }
            }

            for (int i = 0; i < Here.Buildings.Count; i++)
            {
                BuildingItem Box = Here.Buildings[i];
                if (Box == null)
                {
                    continue;
                }

                if (Box.HitsMove(From, To))
                {
                    return true;
                }

                BoxVolume Hull;
                if (Box.TryHull(out Hull) && Hull.Hits(From, To))
                {
                    return true;
                }
            }

            return ForestScatter.Hits(Here, From, To);
        }

        public GroundContact TownGround(float MapX, float MapY)
        {
            float Height = Position.Z;
            float SlopeX = 0f;
            float SlopeY = 0f;
            MapInfo Here = SharedMapContex == null ? null : SharedMapContex.CurrentMapInfo;
            if (Here != null && Here.Heights != null)
            {
                float Ground;
                float GradeX;
                float GradeY;
                if (Here.Heights.Sample(MapX, MapY, out Ground, out GradeX, out GradeY))
                {
                    Height = Ground;
                    SlopeX = GradeX;
                    SlopeY = GradeY;
                }
            }

            if (Here == null || Here.Buildings == null)
            {
                return new GroundContact(Height, SlopeX, SlopeY);
            }

            for (int i = 0; i < Here.Buildings.Count; i++)
            {
                BuildingItem Box = Here.Buildings[i];
                if (Box == null || Box.Kind != BuildingKind.Stairs || !Box.FootprintContains(MapX, MapY))
                {
                    continue;
                }

                if (Position.Z < Box.Position.Z - 2f || Position.Z > Box.Position.Z + Box.Height + 2f)
                {
                    continue;
                }

                Height = Box.Tread(new Vector3(MapX, MapY, Position.Z));
                bool AlongX = Box.Width >= Box.Length;
                float Span = AlongX ? Box.Width : Box.Length;
                float Grade = Span > 0.01f ? Box.Height / Span : 0f;
                SlopeX = AlongX ? Grade : 0f;
                SlopeY = AlongX ? 0f : Grade;
                return new GroundContact(Height, SlopeX, SlopeY);
            }

            return new GroundContact(Height, SlopeX, SlopeY);
        }

        float StairHeight(Vector3 Point)
        {
            if (IgnoreTownSolids)
            {
                return Point.Z;
            }

            MapInfo Here = SharedMapContex == null ? null : SharedMapContex.CurrentMapInfo;
            if (Here == null || Here.Buildings == null)
            {
                return Point.Z;
            }

            for (int i = 0; i < Here.Buildings.Count; i++)
            {
                BuildingItem Box = Here.Buildings[i];
                if (Box == null || Box.Kind != BuildingKind.Stairs || !Box.FootprintContains(Point.X, Point.Y))
                {
                    continue;
                }

                if (Point.Z < Box.Position.Z - 2f || Point.Z > Box.Position.Z + Box.Height + 2f)
                {
                    continue;
                }

                return Box.Tread(Point);
            }

            return Point.Z;
        }

        static string ClipNamed(GltfModel Model, string ClipName)
        {
            if (Model == null || string.IsNullOrEmpty(ClipName))
            {
                return null;
            }

            string Loose = null;
            for (int Index = 0; Index < Model.AnimationNames.Count; Index++)
            {
                string Name = Model.AnimationNames[Index];
                if (string.Equals(Name, ClipName, StringComparison.OrdinalIgnoreCase))
                {
                    return Name;
                }

                if (Loose == null && SameMotion(ClipName, Name))
                {
                    Loose = Name;
                }
            }

            return Loose;
        }

        static bool SameMotion(string Wanted, string Name)
        {
            if (string.IsNullOrEmpty(Wanted) || string.IsNullOrEmpty(Name))
            {
                return false;
            }

            if (Wanted.Equals("Idle", StringComparison.OrdinalIgnoreCase))
            {
                return Name.Contains("Idle", StringComparison.OrdinalIgnoreCase) && !Name.Contains("Walk", StringComparison.OrdinalIgnoreCase);
            }

            if (Wanted.Equals("Walking", StringComparison.OrdinalIgnoreCase) || Wanted.Equals("Walk", StringComparison.OrdinalIgnoreCase))
            {
                return Name.Contains("Walk", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public void PlayClip(string ClipName)
        {
            if (BodyModel != null)
            {
                string Clip = ClipNamed(BodyModel, ClipName);
                if (Clip == null)
                {
                    int Index = MovementClips.IndexFor(ClipName, BodyModel.AnimationNames.Count);
                    if (Index < 0)
                    {
                        return;
                    }

                    Clip = BodyModel.AnimationNames[Index];
                }

                if (BodyModel.ActiveAnimationName != Clip)
                {
                    BodyModel.PlayAnimation(Clip);
                }

                return;
            }

            if (Unit3DModel == null || string.IsNullOrEmpty(ClipName))
            {
                return;
            }

            if (!Unit3DModel.HasAnimation(ClipName))
            {
                ClipName = "Idle";
            }

            if (!Unit3DModel.HasAnimation(ClipName))
            {
                return;
            }

            Unit3DModel.PlayAnimation(ClipName);
        }

        public void SetPosition(Vector3 Position)
        {
            this.Position = Position;
            if (LifeSimMap.PhysicsManager != null)
            {
                LifeSimMap.PhysicsManager.Teleport(this);
            }
        }

        public void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor)
        {
            g.Draw(SpriteMap, new Rectangle((int)Position.X, (int)Position.Y, SizeX, SizeY), null, UnitColor, 0f, new Vector2(SpriteMap.Width / 2, SpriteMap.Height / 2), SpriteEffects.None, 1 / Position.Y);
        }

        public Matrix BodyDrawWorld(Vector3 ChunkShift)
        {
            float DrawnScale = BodyScale > 0.01f ? BodyScale : 1f;
            ScaleMatrix = Matrix.CreateScale(DrawnScale);
            Vector3 Place = ChunkOrigin.ModelWorld(Position, ChunkShift);
            // Physics keeps the cylinder StandClearance above the mesh so it does not snag.
            // The skeleton is already at full length, so the feet cannot reach down across that gap.
            Place.Y -= GroundSkin();
            return ScaleMatrix * RotationMatrix * Matrix.CreateTranslation(Place);
        }

        // How far the posed body is being held above the ground it is standing on.
        // A jump stays airborne: only the standing skin is removed, and never more than that skin.
        float GroundSkin()
        {
            if (WasInWater || CurrentPose == Pose.MidAir)
            {
                return 0f;
            }

            float Above = Position.Z - TownGround(Position.X, Position.Y).Height;
            if (Above <= 0f)
            {
                return 0f;
            }

            return Above < HeightField.StandClearance ? Above : HeightField.StandClearance;
        }

        public void Draw3DOnMap(GraphicsDevice GraphicsDevice, Matrix View, Matrix Projection, Vector3 ChunkShift)
        {
            if (Life.Away.Busy)
            {
                return;
            }

            if (BodyModel == null && Unit3DSprite == null && Unit3DModel == null)
            {
                return;
            }

            WorldMatrix = BodyDrawWorld(ChunkShift);
            float WetWorld = WorldMatrix.Translation.Y + WetLine;
            if (BodyModel != null)
            {
                BodyModel.Light = SkyDraw.CharacterLight(WorldMatrix);
                BodyModel.SetWet(WetWorld, WetAmount);
                BodyModel.Draw(View, Projection, WorldMatrix);
                return;
            }

            if (Unit3DSprite != null)
            {
                Unit3DSprite.SetViewMatrix(View);
                Unit3DSprite.Draw(GraphicsDevice);
                return;
            }

            Unit3DModel.Draw(View, Projection, WorldMatrix);
        }

        public override void Draw(CustomSpriteBatch g, Matrix View)
        {
            throw new NotImplementedException();
        }
    }

    public struct InventoryContainer
    {
        internal Dictionary<string, InventoryContainer> DicFolderByName;
        internal List<InventoryContainer> ListFolder;//Share the same folders as the dictionnary
        internal Dictionary<string, ItemInfo> DicItemByName;

        public string Name;
        public ActionLogHolder Logs;

        public InventoryContainer(string Name)
        {
            this.Name = Name;

            DicFolderByName = new Dictionary<string, InventoryContainer>();
            ListFolder = new List<InventoryContainer>();
            DicItemByName = new Dictionary<string, ItemInfo>();
            Logs = new ActionLogHolder();
        }

        public void Init(LifeSimCharacterParams SharedMapContex)
        {
        }

        public bool HasItemInCategory(string Category)
        {
            return DicFolderByName.ContainsKey(Category);
        }

        public bool HasItem(string ItemName)
        {
            return DicItemByName.ContainsKey(ItemName);
        }

        public bool ContainsTagged(string Name)
        {
            ItemInfo Found;
            return TryFindTagged(Name, out Found);
        }

        public bool TryFindTagged(string Name, out ItemInfo Found)
        {
            foreach (ItemInfo Info in DicItemByName.Values)
            {
                if (ItemMatches(Info, Name))
                {
                    Found = Info;
                    return true;
                }
            }

            foreach (InventoryContainer Folder in ListFolder)
            {
                if (Folder.TryFindTagged(Name, out Found))
                {
                    return true;
                }
            }

            Found = new ItemInfo();
            return false;
        }

        private static bool ItemMatches(ItemInfo Info, string Name)
        {
            if (Info.OwnedItem == null)
            {
                return false;
            }

            if (Info.OwnedItem.Name == Name)
            {
                return true;
            }

            if (Info.OwnedItem.ArrayTags == null)
            {
                return false;
            }

            for (int i = 0; i < Info.OwnedItem.ArrayTags.Length; i++)
            {
                if (Info.OwnedItem.ArrayTags[i] == Name)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddItem(ItemInfo ItemToAdd)
        {
            if (!DicItemByName.ContainsKey(ItemToAdd.OwnedItem.RelativePath))
            {
                DicItemByName.Add(ItemToAdd.OwnedItem.RelativePath, ItemToAdd);
                Logs.AddLog("Added " + ItemToAdd.OwnedItem.RelativePath);
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
            DicItemByName.Remove(ItemToRemove.OwnedItem.RelativePath);

            foreach (InventoryContainer ActiveFolder in ListFolder)
            {
                ActiveFolder.RemoveItemFromInventory(ItemToRemove);
                Logs.AddLog("Removed " + ItemToRemove.OwnedItem.RelativePath);
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

        public bool ContainsCategory(string Category)
        {
            if (Name == Category)
            {
                return true;
            }

            foreach (KnowledgeInfo Info in DicKnowledgeByName.Values)
            {
                if (Info.ArrayTags == null)
                {
                    continue;
                }

                foreach (string Tag in Info.ArrayTags)
                {
                    if (Tag == Category)
                    {
                        return true;
                    }
                }
            }

            foreach (KnowledgeContainer Child in ListFolder)
            {
                if (Child.ContainsCategory(Category))
                {
                    return true;
                }
            }

            return false;
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
        public Item OwnedItem;
        public byte QuantityOwned;

        public ItemInfo(Item OwnedItem, byte QuantityOwned)
        {
            this.OwnedItem = OwnedItem;
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
