using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Magic;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;
using FMOD;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public partial class FightingZone : GameScreen, IOnlineGame
    {
        public uint NextRobotID { get { return _NextRobotID++; } }
        private uint _NextRobotID = 0;
        public uint NextDroppedWeaponID { get { return _NextDroppedWeaponID++; } }
        private uint _NextDroppedWeaponID = 0;

        #region Ressources

        private SpriteFont fntArial9;

        private Texture2D sprHUDBackground;
        private Texture2D sprHUDBackgroundGlow;
        private Texture2D sprHUDForeground;
        private Texture2D sprHUDGrenadeForground;
        private Texture2D sprHUDRespawnBackground;
        private Texture2D sprHUDRespawnGauge;

        private AnimatedSprite sprMeFront;
        private AnimatedSprite sprMeBack;
        private AnimatedSprite sprTeamCursor;
        private AnimatedSprite sprVehicleInIcon;

        public SimpleAnimation sprDamageAnimation;
        private Texture2D sprMessageKilledBy;
        private Texture2D sprMessageYouKilledBlue;
        private Texture2D sprMessageYouKilledRed;
        private Texture2D sprStageStart;
        private Texture2D sprStageClear;

        private Texture2D sprGaugeBoost;
        private Texture2D sprGaugeDelay;
        private Texture2D sprGaugeGrenade;
        private Texture2D sprGaugeHUDHP;
        private Texture2D sprGaugeHP;
        private Texture2D sprGaugeHPBackground;
        private Texture2D sprGaugeReload;

        private Texture2D sprGrenadeCluster;
        private Texture2D sprGrenadeFire;
        private Texture2D sprGrenadeMagnetic;
        private Texture2D sprGrenadeMine;
        private Texture2D sprGrenadeNormal;
        private Texture2D sprGrenadePower;
        private Texture2D sprGrenadeSmoke;

        private SpriteFont fntNumberBulletSmall;
        private SpriteFont fntNumberBullet;
        private SpriteFont fntNumberDamage;

        public AnimatedSprite sprExplosionSplinter;
        public AnimatedSprite sprEnemyExplosionSplinter;

        #endregion

        public Dictionary<string, double> DicMapVariables;

        private List<Player> ListLocalPlayerInfo;
        private List<Player> ListAllPlayerInfo;
        public List<Player> ListLocalPlayer { get { return ListLocalPlayerInfo; } }
        public List<Player> ListAllPlayer { get { return ListAllPlayerInfo; } }
        private readonly List<Player> ListTransferPlayer;
        private List<DamageNumber> ListDamageNumber;
        private List<KillMessage> ListKillMessage;
        private ScrollingTextOverlayBase ScrollingText;
        public ISFXGenerator PlayerSFXGenerator;
        public GameRules Rules;

        private string FightingZonePath;
        private bool UseTeams;
        public bool HasLoaded;
        public string Description;
        public string BGMPath;
        public List<Layer> ListLayer;
        public Rectangle CameraBounds;
        public static Vector2 GravityVector = new Vector2(0, 1);
        public static double GravityAngle = Math.Atan2(1, 0);
        public static double GroundMinAngle;
        public static double GroundMaxAngle;
        public float RespawnTime = 2f;
        public AnimationBackground Background;
        public bool UsePreview;
        private string BackgroundName;
        public static bool ShowCollisionBoxes = false;
        public static bool ShowHPBars = true;
        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;
        public Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;
        public Dictionary<string, MapEvent> DicMapEvent = new Dictionary<string, MapEvent>();
        public Dictionary<string, MapCondition> DicMapCondition = new Dictionary<string, MapCondition>();
        public Dictionary<string, MapTrigger> DicMapTrigger = new Dictionary<string, MapTrigger>();
        public List<MapScript> ListMapScript;
        public List<MapEvent> ListMapEvent;

        public readonly TripleThunderRobotContext GlobalRobotContext;
        public readonly TripleThunderAttackContext GlobalAttackContext;
        public readonly TripleThunderAttackParams AttackParams;
        public bool IsServer { get { return GameScreen.GraphicsDevice == null; } }
        public bool IsOfflineOrServer { get { return OnlineClient == null; } }

        public readonly TripleThunderOnlineClient OnlineClient;
        private int OnlinePlayerUpdateTimer;
        public readonly GameServer OnlineServer;
        public readonly TripleThunderClientGroup GameGroup;

        public FightingZone()
            : this(new TripleThunderRobotContext(), new TripleThunderAttackContext())
        {
        }

        public FightingZone(TripleThunderRobotContext GlobalRobotContext, TripleThunderAttackContext GlobalAttackContext)
        {
            this.GlobalRobotContext = GlobalRobotContext;
            this.GlobalAttackContext = GlobalAttackContext;

            DicMapVariables = new Dictionary<string, double>();
            FormulaParser.ActiveParser = new FightingZoneFormulaParser(this);
            AttackParams = new TripleThunderAttackParams(GlobalAttackContext);
            AttackParams.SharedParams.Content = Content;
            ListLayer = new List<Layer>();
            ListAllPlayerInfo = new List<Player>();
            ListLocalPlayerInfo = new List<Player>();
            ListDamageNumber = new List<DamageNumber>();
            ListKillMessage = new List<KillMessage>();
            PlayerSFXGenerator = new MuteSFXGenerator();

            UseTeams = true;
            HasLoaded = false;
            CameraBounds = new Rectangle(0, 0, Constants.Width, Constants.Height);
            Description = string.Empty;
            BGMPath = string.Empty;
            Background = null;
            BackgroundName = string.Empty;
            GroundMinAngle = GravityAngle - MathHelper.PiOver4;
            GroundMaxAngle = GravityAngle + MathHelper.PiOver4;

            DicEffect = new Dictionary<string, BaseEffect>();
            DicRequirement = new Dictionary<string, BaseSkillRequirement>();
            DicAutomaticSkillTarget = new Dictionary<string, AutomaticSkillTargetType>();
            ListMapScript = new List<MapScript>();
            ListMapEvent = new List<MapEvent>();
        }
        
        /// <summary>
        /// Local game constructor
        /// </summary>
        /// <param name="MapName"></param>
        /// <param name="ListLocalPlayerInfo"></param>
        /// <param name="ListTransferPlayer"></param>
        public FightingZone(string MapName, bool UseTeams, List<Player> ListLocalPlayerInfo = null, List<Player> ListTransferPlayer = null)
            : this()
        {
            FightingZonePath = MapName;
            this.UseTeams = UseTeams;
            this.ListLocalPlayerInfo = ListLocalPlayerInfo;
            this.ListAllPlayerInfo = ListLocalPlayerInfo;
            this.ListTransferPlayer = ListTransferPlayer;
            UsePreview = false;
        }

        /// <summary>
        /// Local game constructor
        /// </summary>
        /// <param name="MapName"></param>
        /// <param name="ListLocalPlayerInfo"></param>
        public FightingZone(string MapName, bool UseTeams, List<Player> ListLocalPlayerInfo)
            : this()
        {
            FightingZonePath = MapName;
            this.UseTeams = UseTeams;
            this.ListLocalPlayerInfo = ListLocalPlayerInfo;
            this.ListAllPlayerInfo = ListLocalPlayerInfo;
            UsePreview = false;
        }

        public FightingZone(string MapName, bool UseTeams, GameServer OnlineServer, TripleThunderClientGroup GameGroup, List<Player> ListLocalPlayer = null, List<Player> ListAllPlayer = null)
            : this()
        {
            FightingZonePath = MapName;
            this.UseTeams = UseTeams;
            this.OnlineServer = OnlineServer;
            this.GameGroup = GameGroup;
            if (ListLocalPlayer != null)
            {
                this.ListLocalPlayerInfo = ListLocalPlayer;
            }
            if (ListAllPlayer != null)
            {
                this.ListAllPlayerInfo = ListAllPlayer;
            }
            UsePreview = false;
        }

        public FightingZone(TripleThunderOnlineClient OnlineClient)
            : this()
        {
            this.OnlineClient = OnlineClient;
            UsePreview = false;
        }

        public override void Load()
        {
            if (HasLoaded)
                return;

            if (File.Exists("Triple Thunder Params.ini"))
            {
                IniFile ParamFile = IniFile.ReadFromFile("Triple Thunder Params.ini");

                JetpackBase.JetpackTrust = float.Parse(ParamFile.ReadField("JetpackParams", "Trust"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                JetpackBase.MaxJetpackTrust = float.Parse(ParamFile.ReadField("JetpackParams", "MaxTrust"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

                BaseShoes.DashCounterStartValue = double.Parse(ParamFile.ReadField("ShoesParams", "DashCounterStartValue"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                BaseShoes.MaxJumpTime = double.Parse(ParamFile.ReadField("ShoesParams", "MaxJumpTime"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                BaseShoes.LongJumpMultiplier = float.Parse(ParamFile.ReadField("ShoesParams", "LongJumpMultiplier"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

                Layer.Gravity = float.Parse(ParamFile.ReadField("PlayerParams", "Gravity"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                Layer.GravityMax = float.Parse(ParamFile.ReadField("PlayerParams", "GravityMax"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                Layer.Friction = float.Parse(ParamFile.ReadField("PlayerParams", "Friction"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            }

            AIScriptHolder.DicAIScripts.Clear();

            LoadMapScripts();
            LoadTripleThunderAIScripts();
            LoadTripleThunderEffects();
            LoadTripleThunderRequirements();
            LoadTripleThunderTargetTypes();

            if (Rules != null)
            {
                Rules.Init();
            }

            LoadMap();

            if (!IsServer && !UsePreview && !string.IsNullOrEmpty(BGMPath))
            {
                FMODSound NewTheme = new FMODSound(GameScreen.FMODSystem, "Content/Maps/BGM/" + BGMPath + ".mp3");
                NewTheme.PlayAsBGM();
                GameScreen.FMODSystem.ChangeBGMVolume(0.5f);
                NewTheme.SetLoop(true);
            }

            if (IsOfflineOrServer || IsServer)
            {
                UpdateMapEvent(FightingZoneEvent.EventTypeGame, 0);
                HasLoaded = true;
            }
            
            if (ListLocalPlayerInfo != null)
            {
                foreach (Player PlayerInfo in ListLocalPlayerInfo)
                {
                    PlayerInfo.Equipment.EquipedPrimaryWeapon = null;

                    AddPlayerFromSpawn(PlayerInfo, NextRobotID + (uint.MaxValue - 100), false, out _);

                    if (ListLocalPlayerInfo.Count == 2)
                    {
                        PlayerInfo.InGameRobot.Camera = new Rectangle(0, 0, Constants.Width, Constants.Height / 2);
                    }
                    else if (ListLocalPlayerInfo.Count > 2)
                    {
                        PlayerInfo.InGameRobot.Camera = new Rectangle(0, 0, Constants.Width / 2, Constants.Height / 2);
                    }
                }
            }

            if (ListTransferPlayer != null)
            {
                foreach (Player ActivePlayer in ListTransferPlayer)
                {
                    bool PlayerAdded = false;

                    for (int L = 0; L < ListLayer.Count && !PlayerAdded; ++L)
                    {
                        foreach (SpawnPoint ActiveSpawn in ListLayer[L].ListSpawnPointTeam)
                        {
                            if (!ActiveSpawn.IsUsed)
                            {
                                ActiveSpawn.IsUsed = true;
                                ActivePlayer.InGameRobot.ChangeLayer(ListLayer[L]);
                                ListLayer[L].DicRobot.Add(ActivePlayer.InGameRobot.ID, ActivePlayer.InGameRobot);
                                ActivePlayer.InGameRobot.SetIdle();
                                ActivePlayer.InGameRobot.ChangeMap(CameraBounds);
                                ActivePlayer.InGameRobot.Move(-ActivePlayer.InGameRobot.Position + ActiveSpawn.SpawnLocation);
                                PlayerAdded = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (!IsServer)
            {
                LoadRessources();

                Propulsor.Load(Content, GraphicsDevice, Constants.Width, Constants.Height);
            }
        }

        internal void Load(byte[] ArrayGameData, List<uint> ListLocalCharacterID, Dictionary<uint, Player> DicAllPlayer)
        {
            using (MemoryStream MS = new MemoryStream(ArrayGameData))
            {
                using (BinaryReader BR = new BinaryReader(MS))
                {
                    int LocalPlayerIndex = 0;

                    FightingZonePath = BR.ReadString();

                    Load();

                    int ListLayerCount = BR.ReadInt32();
                    for (int L = 0; L < ListLayerCount; ++L)
                    {
                        Layer ActiveLayer = ListLayer[L];
                        int EnemyCount = BR.ReadInt32();

                        for (int E = 0; E < EnemyCount; ++E)
                        {
                            uint PlayerID = BR.ReadUInt32();
                            int PlayerTeam = BR.ReadInt32();
                            string PlayerAIPath = BR.ReadString();
                            string PlayerName = BR.ReadString();
                            Vector2 PlayerPosition = new Vector2(BR.ReadSingle(), BR.ReadSingle());

                            int ListWeaponCount = BR.ReadInt32();
                            List<string> ListEnememyWeapon = new List<string>(ListWeaponCount);
                            for (int W = 0; W < ListWeaponCount; ++W)
                            {
                                ListEnememyWeapon.Add(BR.ReadString());
                            }

                            List<WeaponBase> ListExtraWeapon = new List<WeaponBase>();
                            for (int W = 0; W < ListEnememyWeapon.Count; ++W)
                            {
                                ListExtraWeapon.Add(WeaponBase.CreateFromFile(PlayerName, ListEnememyWeapon[W], true, ActiveLayer.DicRequirement, ActiveLayer.DicEffect, ActiveLayer.DicAutomaticSkillTarget));
                            }

                            //TODO: support vehicles
                            RobotAnimation NewRobot = new RobotAnimation(PlayerName, ActiveLayer, PlayerPosition, PlayerTeam, new PlayerInventory(), PlayerSFXGenerator, ListExtraWeapon);
                            if (ListLocalCharacterID.Contains(PlayerID))
                            {
                                Player NewPlayer = PlayerManager.ListLocalPlayer[LocalPlayerIndex++];
                                NewPlayer.InGameRobot = NewRobot;
                                AddLocalCharacter(NewPlayer);
                                NewRobot.InputManagerHelper = new PlayerRobotInputManager();
                                NewRobot.UpdateControls(GameplayTypes.MouseAndKeyboard, CameraBounds);
                            }
                            else if (DicAllPlayer.ContainsKey(PlayerID))
                            {
                                Player NewPlayer = DicAllPlayer[PlayerID];
                                NewPlayer.InGameRobot = NewRobot;
                                ListAllPlayerInfo.Add(NewPlayer);
                            }

                            if (NewRobot.Content != null)
                            {
                                if (!string.IsNullOrEmpty(PlayerAIPath))
                                {
                                    NewRobot.RobotAI = new TripleThunderScripAIContainer(new TripleThunderAIInfo(NewRobot, ActiveLayer, this));
                                    NewRobot.RobotAI.Load(PlayerAIPath);
                                }

                                ActiveLayer.SpawnRobot(PlayerID, NewRobot);
                            }
                        }
                    }
                }
            }

            HasLoaded = true;
        }

        private void LoadMap()
        {
            FileStream FS = new FileStream("Content/Maps/Triple Thunder/" + FightingZonePath + ".ttm", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            string BackgroundName = BR.ReadString();
            if (!IsServer)
            {
                LoadBackground(BackgroundName);
            }

            CameraBounds = new Rectangle(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32());
            BGMPath = BR.ReadString();
            Description = BR.ReadString();

            Dictionary<string, Prop> DicPropsByName = Prop.GetAllPropsByName();

            int ListLayerCount = BR.ReadInt32();
            ListLayer = new List<Layer>(ListLayerCount);
            for (int L = 0; L < ListLayerCount; L++)
            {
                Layer NewLayer = new Layer(this, BR);

                ListLayer.Add(NewLayer);
            }

            ListMapScript = MapScript.LoadMapScripts(BR, DicMapEvent, DicMapCondition, DicMapTrigger, out ListMapEvent);

            BR.Close();
            FS.Close();
        }

        private void LoadRessources()
        {
            if (Rules != null)
            {
                Rules.Load(Content);
            }

            SFXGenerator NewSFXGenerator = new SFXGenerator();
            NewSFXGenerator.LoadAllSounds();
            PlayerSFXGenerator = NewSFXGenerator;

            fntArial9 = Content.Load<SpriteFont>("Fonts/Arial9");

            sprHUDBackground = Content.Load<Texture2D>("Triple Thunder/HUD/HUD Background");
            sprHUDBackgroundGlow = Content.Load<Texture2D>("Triple Thunder/HUD/HUD Background Glow");
            sprHUDForeground = Content.Load<Texture2D>("Triple Thunder/HUD/HUD Foreground");
            sprHUDGrenadeForground = Content.Load<Texture2D>("Triple Thunder/HUD/HUD Grenade Foreground");
            sprHUDRespawnBackground = Content.Load<Texture2D>("Triple Thunder/HUD/Respawn Background");
            sprHUDRespawnGauge = Content.Load<Texture2D>("Triple Thunder/HUD/Respawn Gauge");

            sprDamageAnimation = new SimpleAnimation("Triple Thunder/Effects/Damage_strip6", Content);
            sprDamageAnimation.IsLooped = false;
            sprMeFront = new AnimatedSprite(Content, "Triple Thunder/HUD/Me Front Red_strip14_2", Vector2.Zero);
            sprMeBack = new AnimatedSprite(Content, "Triple Thunder/HUD/Me Back Red_strip14_2", Vector2.Zero);
            sprTeamCursor = new AnimatedSprite(Content, "Triple Thunder/HUD/My Team Red_strip12", Vector2.Zero);
            sprVehicleInIcon = new AnimatedSprite(Content, "Triple Thunder/HUD/Vehicle In Icon_strip2", Vector2.Zero, 3);

            sprMessageKilledBy = Content.Load<Texture2D>("Triple Thunder/HUD/Message Killed By");
            sprMessageYouKilledBlue = Content.Load<Texture2D>("Triple Thunder/HUD/Message You Killed Blue");
            sprMessageYouKilledRed = Content.Load<Texture2D>("Triple Thunder/HUD/Message You Killed Red");
            sprStageStart = Content.Load<Texture2D>("Triple Thunder/HUD/Text Start Game");
            sprStageClear = Content.Load<Texture2D>("Triple Thunder/HUD/Text Stage Clear");

            sprGaugeBoost = Content.Load<Texture2D>("Triple Thunder/HUD/Gauge Boost");
            sprGaugeDelay = Content.Load<Texture2D>("Triple Thunder/HUD/Gauge Delay");
            sprGaugeGrenade = Content.Load<Texture2D>("Triple Thunder/HUD/Gauge Grenade");
            sprGaugeHUDHP = Content.Load<Texture2D>("Triple Thunder/HUD/Gauge HP");
            sprGaugeHP = Content.Load<Texture2D>("Triple Thunder/HUD/GUI_Hp_Dot");
            sprGaugeHPBackground = Content.Load<Texture2D>("Triple Thunder/HUD/GUI_Hp_bar");
            sprGaugeReload = Content.Load<Texture2D>("Triple Thunder/HUD/Gauge Reload");

            sprGrenadeCluster = Content.Load<Texture2D>("Triple Thunder/HUD/Grenade Cluster");
            sprGrenadeFire = Content.Load<Texture2D>("Triple Thunder/HUD/Grenade Fire");
            sprGrenadeMagnetic = Content.Load<Texture2D>("Triple Thunder/HUD/Grenade Magnetic");
            sprGrenadeMine = Content.Load<Texture2D>("Triple Thunder/HUD/Grenade Mine");
            sprGrenadeNormal = Content.Load<Texture2D>("Triple Thunder/HUD/Grenade Normal");
            sprGrenadePower = Content.Load<Texture2D>("Triple Thunder/HUD/Grenade Power");
            sprGrenadeSmoke = Content.Load<Texture2D>("Triple Thunder/HUD/Grenade Smoke");

            fntNumberBulletSmall = Content.Load<SpriteFont>("Triple Thunder/HUD/Number Bullet Small");
            fntNumberBullet = Content.Load<SpriteFont>("Triple Thunder/HUD/Number Bullets");
            fntNumberDamage = Content.Load<SpriteFont>("Triple Thunder/HUD/Number Damage");

            sprExplosionSplinter = new AnimatedSprite(Content, "Animations/Sprites/gob_SplinterStone_strip6", Vector2.Zero, 0);
            sprEnemyExplosionSplinter = new AnimatedSprite(Content, "Animations/Sprites/gob_SplinterEnemy_strip8", Vector2.Zero, 0);

            ScrollingText = new ScrollingTextOverlay(sprStageStart);
        }

        protected void LoadMapScripts()
        {
            DicMapEvent = new Dictionary<string, MapEvent>();
            DicMapEvent.Add(FightingZoneEvent.EventTypeGame, new FightingZoneEvent(FightingZoneEvent.EventTypeGame, new string[] { "Game Start" }));
            DicMapEvent.Add(FightingZoneEvent.EventTypeAllEnemiesDefeated, new FightingZoneEvent(FightingZoneEvent.EventTypeAllEnemiesDefeated, new string[] { "All Enemies Defeated" }));
            DicMapEvent.Add(FightingZoneEvent.EventOnStep, new FightingZoneEvent(FightingZoneEvent.EventOnStep, new string[] { FightingZoneEvent.EventOnStep }));

            DicMapCondition = MapScript.LoadConditions<FightingZoneCondition>(this);
            DicMapTrigger = MapScript.LoadTriggers<FightingZoneTrigger>(this);
        }

        public void LoadTripleThunderEffects()
        {
            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in Projectile.GetCoreProjectileEffects(AttackParams))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Triple Thunder", "*.dll"), typeof(TripleThunderAttackEffect), AttackParams))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Triple Thunder", "*.dll"), typeof(TripleThunderRobotEffect), new TripleThunderRobotParams(GlobalRobotContext)))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }
        }

        public void LoadTripleThunderRequirements()
        {
            Dictionary<string, BaseSkillRequirement> DicRequirementCore = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(BaseSkillRequirement));
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCore)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            Dictionary<string, BaseSkillRequirement> DicRequirementAttack = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Triple Thunder", "*.dll"), typeof(TripleThunderAttackRequirement), GlobalAttackContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementAttack)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            Dictionary<string, BaseSkillRequirement> DicRequirementRobot = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Triple Thunder", "*.dll"), typeof(TripleThunderRobotRequirement), GlobalRobotContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementRobot)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }
        }

        public void LoadTripleThunderTargetTypes()
        {
            DicAutomaticSkillTarget.Clear();
            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Triple Thunder", "*.dll"), typeof(AttackTargetType), GlobalAttackContext))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Triple Thunder", "*.dll"), typeof(RobotTargetType), GlobalRobotContext))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }
        }

        private void LoadTripleThunderAIScripts()
        {
            foreach (KeyValuePair<string, List<AIScript>> ActiveListScript in ReflectionHelper.GetContentByName<AIScript>(typeof(CoreAI), Directory.GetFiles("AI", "*.dll")))
            {
                for (int S = ActiveListScript.Value.Count - 1; S >= 0; --S)
                    AIScriptHolder.DicAIScripts.Add(ActiveListScript.Value[S].Name, ActiveListScript.Value[S]);
            }

            foreach (KeyValuePair<string, List<AIScript>> ActiveListScript in ReflectionHelper.GetContentByName<AIScript>(typeof(TripleThunderScriptHolder), Directory.GetFiles("AI", "*.dll")))
            {
                for (int S = ActiveListScript.Value.Count - 1; S >= 0; --S)
                    AIScriptHolder.DicAIScripts.Add(ActiveListScript.Value[S].Name, ActiveListScript.Value[S]);
            }
        }

        public void LoadBackground(string BackgroundName)
        {
            this.BackgroundName = BackgroundName;
            if (string.IsNullOrEmpty(BackgroundName))
            {
                return;
            }

            Background = AnimationBackground.LoadAnimationBackground(BackgroundName, Content, GraphicsDevice);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(BackgroundName);
            BW.Write(CameraBounds.X);
            BW.Write(CameraBounds.Y);
            BW.Write(CameraBounds.Width);
            BW.Write(CameraBounds.Height);
            BW.Write(BGMPath);
            BW.Write(Description);

            BW.Write(ListLayer.Count);
            for (int L = 0; L < ListLayer.Count; L++)
            {
                ListLayer[L].Save(BW);
            }

            MapScript.SaveMapScripts(BW, ListMapScript);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateLevelChange(gameTime);

            Propulsor.ParticleSystem.Update(gameTime.ElapsedGameTime.TotalSeconds);

            sprVehicleInIcon.Update(gameTime);
            if (sprVehicleInIcon.AnimationEnded)
            {
                sprVehicleInIcon.LoopAnimation();
            }
            sprMeFront.Update(gameTime);
            if (sprMeFront.AnimationEnded)
            {
                sprMeFront.LoopAnimation();
            }
            sprMeBack.Update(gameTime);
            if (sprMeBack.AnimationEnded)
            {
                sprMeBack.LoopAnimation();
            }
            sprTeamCursor.Update(gameTime);
            if (sprTeamCursor.AnimationEnded)
            {
                sprTeamCursor.LoopAnimation();
            }

            Vector3 CameraOldPosition = new Vector3(ListLocalPlayerInfo[0].InGameRobot.Camera.X, ListLocalPlayerInfo[0].InGameRobot.Camera.Y, 0f);

            UpdateMainCharacter(gameTime);
            HandleRespawn(gameTime);

            for (int L = 0; L < ListLayer.Count; L++)
            {
                ListLayer[L].Update(gameTime);
            }

            Rules.Update(gameTime);

            PlayerSFXGenerator.PlayExplosionSounds(ListLocalPlayerInfo);

            if (Background != null)
            {
                Background.MoveSpeed = new Vector3(ListLocalPlayerInfo[0].InGameRobot.Camera.X, ListLocalPlayerInfo[0].InGameRobot.Camera.Y, 0f) - CameraOldPosition;
                Background.Update(gameTime);
            }

            for (int D = 0; D < ListDamageNumber.Count; D++)
            {
                if (!ListDamageNumber[D].DamageAnimation.HasEnded)
                {
                    ListDamageNumber[D].DamageAnimation.Update(gameTime);
                }
                ListDamageNumber[D].LifetimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;

                if (ListDamageNumber[D].LifetimeRemaining <= 0)
                {
                    ListDamageNumber.RemoveAt(D--);
                }
            }

            for (int D = 0; D < ListKillMessage.Count; D++)
            {
                ListKillMessage[D].LifetimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;

                if (ListKillMessage[D].LifetimeRemaining <= 0)
                {
                    ListKillMessage.RemoveAt(D--);
                }
            }

            UpdateMapEvent(FightingZoneEvent.EventOnStep, 0);
        }

        public void Update(double ElapsedSeconds)
        {
            if (!HasLoaded)
            {
                Load();

                foreach (IOnlineConnection ActivePlayer in GameGroup.Room.ListOnlinePlayer)
                {
                    ActivePlayer.Send(new ServerIsReadyScriptServer());
                }
            }

            GameTime UpdateTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(ElapsedSeconds));
            for (int L = 0; L < ListLayer.Count; L++)
            {
                ListLayer[L].Update(UpdateTime);
            }

            Rules.Update(UpdateTime);

            HandleRespawn(UpdateTime);

            if (--OnlinePlayerUpdateTimer <= 0)
            {
                OnlinePlayerUpdateTimer = 2;
                OnlineServer.SharedWriteBuffer.ClearWriteBuffer();

                for (int L = 0; L < ListLayer.Count; L++)
                {
                    foreach (KeyValuePair<uint, RobotAnimation> ActiveRobot in ListLayer[L].DicRobot)
                    {
                        OnlineServer.SharedWriteBuffer.WriteScript(new SendPlayerUpdateScriptServer(L, ActiveRobot.Value));
                    }
                }

                foreach (IOnlineConnection ActivePlayer in GameGroup.Room.ListOnlinePlayer)
                {
                    if (ActivePlayer.IsGameReady)
                    {
                        ActivePlayer.SendWriteBuffer();
                    }
                }
            }
        }

        public void RemoveOnlinePlayer(string PlayerID, IOnlineConnection PlayerToRemove)
        {
            for (int P = ListLocalPlayer.Count - 1; P >= 0; P--)
            { 
                if (ListLocalPlayer[P].OnlineClient == PlayerToRemove)
                {
                    foreach (IOnlineConnection ActivePlayer in GameGroup.Room.ListOnlinePlayer)
                    {
                        ActivePlayer.Send(new PlayerLeftScriptServer(PlayerID, ListLocalPlayer[P].InGameRobot.ID));
                    }

                    ListLocalPlayer.RemoveAt(P);
                    break;
                }
            }
        }

        public void UpdateLevelChange(GameTime gameTime)
        {
            if (ScrollingText != null)
            {
                ScrollingText.Update(gameTime);

                if (ScrollingText.IsFinished)
                {
                    if (ScrollingText.NextLevelPath != null)
                    {
                        foreach (Layer ActiveLayer in ListLayer)
                        {
                            foreach (RobotAnimation ActiveRobot in ActiveLayer.DicRobot.Values)
                            {
                                if (ActiveRobot.Team == 0)
                                {
                                    ActiveRobot.ChangeLayer(null);
                                }
                            }

                            ActiveLayer.DicRobot.Clear();
                        }

                        ListLayer.Clear();

                        FightingZone NextZone;
                        if (IsOfflineOrServer)
                        {
                            NextZone = new FightingZone(ScrollingText.NextLevelPath, UseTeams, ListLocalPlayerInfo, null);
                        }
                        else
                        {
                            NextZone = new FightingZone(OnlineClient);
                        }

                        NextZone.Rules = Rules;
                        PushScreen(new LoadingScreen(NextZone, OnlineClient));
                        RemoveScreen(this);

                        return;
                    }

                    ScrollingText = null;
                }
            }
        }

        public void AddPlayerFromSpawn(Player NewPlayer, uint ID, bool AllowReuse, out int LayerIndex)
        {
            for (LayerIndex = 0; LayerIndex < ListLayer.Count; ++LayerIndex)
            {
                foreach (SpawnPoint ActiveSpawn in ListLayer[LayerIndex].ListSpawnPointTeam)
                {
                    if ((AllowReuse || !ActiveSpawn.IsUsed) && ActiveSpawn.Team == NewPlayer.Team)
                    {
                        ActiveSpawn.IsUsed = true;
                        RobotAnimation SpawnedPlayer = ActiveSpawn.SpawnPlayer(NewPlayer, ListLayer[LayerIndex], PlayerSFXGenerator, CameraBounds);

                        string WeaponName = SpawnedPlayer.SecondaryWeapons.GetWeaponName(0);
                        SpawnedPlayer.SecondaryWeapons.UseWeapon(WeaponName);

                        ListLayer[LayerIndex].SpawnRobot(ID, SpawnedPlayer);
                        SpawnedPlayer.SetIdle();

                        NewPlayer.InGameRobot = SpawnedPlayer;

                        return;
                    }
                }
            }
        }

        public void AddPlayer(Player NewPlayer, int LayerIndex, Vector2 Position, uint ID)
        {
            RobotAnimation SpawnedPlayer = new RobotAnimation("Characters/" + NewPlayer.Equipment.CharacterType, ListLayer[LayerIndex], Position, 0, NewPlayer.Equipment, PlayerSFXGenerator, new List<WeaponBase>());

            string WeaponName = SpawnedPlayer.SecondaryWeapons.GetWeaponName(0);
            SpawnedPlayer.SecondaryWeapons.UseWeapon(WeaponName);

            ListLayer[LayerIndex].SpawnRobot(ID, SpawnedPlayer);
            SpawnedPlayer.SetIdle();

            NewPlayer.InGameRobot = SpawnedPlayer;
            ListAllPlayerInfo.Add(NewPlayer);
        }

        public void AddLocalCharacter(Player NewLocalCharacter)
        {
            ListAllPlayerInfo.Add(NewLocalCharacter);
            ListLocalPlayerInfo.Add(NewLocalCharacter);
        }

        internal bool IsMainCharacter(uint PlayerID)
        {
            foreach (Player MainCharacter in ListLocalPlayerInfo)
            {
                if (MainCharacter.InGameRobot.ID == PlayerID)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateMainCharacter(GameTime gameTime)
        {
            if (KeyboardHelper.KeyPressed(Keys.M))
            {
                PushScreen(new MagicEditor(ListLocalPlayerInfo[0].InGameRobot.ListMagicSpell[0], GlobalAttackContext, AttackParams.SharedParams));
            }

            if (!IsOfflineOrServer)
            {
                foreach (Player MainCharacter in ListLocalPlayerInfo)
                {
                    OnlineClient.Host.Send(new SendPlayerUpdateScriptClient(MainCharacter.InGameRobot));
                }
            }
        }

        private void HandleRespawn(GameTime gameTime)
        {
            foreach (Player MainCharacter in ListLocalPlayerInfo)
            {
                if (MainCharacter.InGameRobot.RespawnTimer > 0)
                {
                    MainCharacter.InGameRobot.RespawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (IsOfflineOrServer && MainCharacter.InGameRobot.RespawnTimer <= 0)
                    {
                        RespawnRobot(MainCharacter.InGameRobot);
                    }
                }
            }
        }

        private void RespawnRobot(RobotAnimation RobotToRespawn)
        {
            List<Tuple<SpawnPoint, int>> ListPossibleSpawn = new List<Tuple<SpawnPoint, int>> ();
            for (int LayerIndex = 0; LayerIndex < ListLayer.Count; ++LayerIndex)
            {
                if (UseTeams)
                {
                    foreach (SpawnPoint ActiveSpawn in ListLayer[LayerIndex].ListSpawnPointTeam)
                    {
                        if (ActiveSpawn.Team == RobotToRespawn.Team)
                        {
                            ListPossibleSpawn.Add(new Tuple<SpawnPoint, int>(ActiveSpawn, LayerIndex));
                        }
                    }
                }
                else
                {
                    foreach (SpawnPoint ActiveSpawn in ListLayer[LayerIndex].ListSpawnPointNoTeam)
                    {
                        ListPossibleSpawn.Add(new Tuple<SpawnPoint, int>(ActiveSpawn, LayerIndex));
                    }
                }
            }

            Tuple<SpawnPoint, int> FinalSpawn = ListPossibleSpawn[RandomHelper.Next(ListPossibleSpawn.Count)];
            RespawnRobot(FinalSpawn.Item2, RobotToRespawn.ID, FinalSpawn.Item1.SpawnLocation.X, FinalSpawn.Item1.SpawnLocation.Y, RobotToRespawn.MaxHP);

            if (IsServer)
            {
                foreach (IOnlineConnection ActivePlayer in GameGroup.Room.ListOnlinePlayer)
                {
                    ActivePlayer.Send(new SendPlayerRespawnScriptServer(FinalSpawn.Item2, RobotToRespawn.ID,
                    RobotToRespawn.Position.X, RobotToRespawn.Position.Y, RobotToRespawn.HP));
                }
            }
        }

        public void RespawnRobot(int LayerIndex, uint PlayerID, float PositionX, float PositionY, int PlayerHP)
        {
            foreach (Player ActivePlayer in ListLocalPlayerInfo)
            {
                if (ActivePlayer.InGameRobot.ID == PlayerID)
                {
                    Vector2 Movement = new Vector2(PositionX - ActivePlayer.InGameRobot.Position.X, PositionY - ActivePlayer.InGameRobot.Position.Y);
                    ActivePlayer.InGameRobot.ChangeLayer(ListLayer[LayerIndex]);
                    ActivePlayer.InGameRobot.Move(Movement);
                    ActivePlayer.InGameRobot.HP = PlayerHP;
                    if (ActivePlayer.Equipment.EquipedPrimaryWeapon != null && ActivePlayer.InGameRobot.PrimaryWeapons.HasWeapons)
                    {
                        ActivePlayer.InGameRobot.ReplacePrimaryWeapon(ActivePlayer.Equipment.EquipedPrimaryWeapon);
                    }
                    return;
                }
            }

            foreach (Layer ActiveLayer in ListLayer)
            {
                foreach (RobotAnimation ActiveRobot in ActiveLayer.DicRobot.Values)
                {
                    if (ActiveRobot.ID == PlayerID)
                    {
                        Vector2 Movement = new Vector2(PositionX - ActiveRobot.Position.X, PositionY - ActiveRobot.Position.Y);
                        ActiveRobot.ChangeLayer(ListLayer[LayerIndex]);
                        ActiveRobot.Move(Movement);
                        ActiveRobot.HP = PlayerHP;
                    }
                }
            }
        }

        public void ChangeRobotLayer(RobotAnimation RobotToChange, int NewLayerIndex)
        {
            ListLayer[NewLayerIndex].DicRobot.Add(RobotToChange.ID, RobotToChange);
            RobotToChange.ChangeLayer(ListLayer[NewLayerIndex]);
        }

        public void AddDamageNumber(DamageNumber NewDamageNumber)
        {
            NewDamageNumber.DamageAnimation = sprDamageAnimation.Copy();
            NewDamageNumber.DamageAnimation.Position = NewDamageNumber.Position;
            ListDamageNumber.Add(NewDamageNumber);
        }

        public void AddKillMessage(bool Red, bool KilledBy, string Message)
        {
            if (KilledBy)
            {
                ListKillMessage.Add(new KillMessage(Message, 1000, sprMessageKilledBy));
            }
            else if (Red)
            {
                ListKillMessage.Add(new KillMessage(Message, 1000, sprMessageYouKilledRed));
            }
            else
            {
                ListKillMessage.Add(new KillMessage(Message, 1000, sprMessageYouKilledBlue));
            }
        }

        public void CheckIfAllEnemiesAreDead()
        {
            foreach (Layer ActiveLayer in ListLayer)
            {
                foreach (KeyValuePair<uint, RobotAnimation> ActiveRobot in ActiveLayer.DicRobot)
                {
                    if (ActiveRobot.Value.Team > 0 && ActiveRobot.Value.HP > 0)
                    {
                        return;
                    }
                }
            }

            UpdateMapEvent(FightingZoneEvent.EventTypeAllEnemiesDefeated, 0);
        }

        public bool IsInsideCamera(Vector2 Position, Vector2 Size)
        {
            foreach (Player ActiveRobot in ListLocalPlayerInfo)
            {
                if (Position.X + Size.X > ActiveRobot.InGameRobot.Camera.X && Position.X - Size.X < ActiveRobot.InGameRobot.Camera.Right
                    && Position.Y + Size.Y > ActiveRobot.InGameRobot.Camera.Y && Position.Y - Size.Y < ActiveRobot.InGameRobot.Camera.Bottom)
                {
                    return true;
                }
            }

            return false;
        }

        public void PrepareNextLevel(string NextLevelPath)
        {
            if (IsServer)
            {
                FightingZone NextLevel = new FightingZone(NextLevelPath, UseTeams, OnlineServer, GameGroup, ListLocalPlayerInfo, ListAllPlayerInfo);
                NextLevel.Rules = Rules;
                GameGroup.SetGame(NextLevel);

                foreach (IOnlineConnection ActivePlayer in GameGroup.Room.ListOnlinePlayer)
                {
                    ActivePlayer.IsGameReady = false;
                    ActivePlayer.Send(new GoToNextMapScriptServer(NextLevelPath));
                }
            }
            else
            {
                ScrollingText = new ScrollingTextOverlay(sprStageClear, NextLevelPath);
            }
        }

        public void SetScrollingTextOverlay(ScrollingTextOverlayBase NewScrollingTextOverlay)
        {
            ScrollingText = NewScrollingTextOverlay;
        }

        public void UpdateMapEvent(string EventType, int Index)
        {
            for (int E = ListMapEvent.Count - 1; E >= 0; --E)
            {
                if (ListMapEvent[E].Name != EventType)
                    continue;

                ExecuteFollowingScripts(ListMapEvent[E], Index);
            }
        }

        public void ExecuteFollowingScripts(MapScript InputScript, int Index)
        {
            for (int E = 0; E < InputScript.ArrayEvents[Index].Count; E++)
            {
                if (ListMapScript[InputScript.ArrayEvents[Index][E].LinkedScriptIndex].MapScriptType == MapScriptTypes.Condition ||
                    ListMapScript[InputScript.ArrayEvents[Index][E].LinkedScriptIndex].MapScriptType == MapScriptTypes.Trigger)
                {
                    ListMapScript[InputScript.ArrayEvents[Index][E].LinkedScriptIndex].Update(InputScript.ArrayEvents[Index][E].LinkedScriptTriggerIndex);
                }
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.BeginUnscaled(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

            foreach (Layer ActiveLayer in ListLayer)
            {
                foreach (RobotAnimation ActiveRobot in ActiveLayer.DicRobot.Values)
                    ActiveRobot.BeginDraw(g);
            }

            g.End();
        }

        public override void EndDraw(CustomSpriteBatch g)
        {
            base.EndDraw(g);
            foreach (Layer ActiveLayer in ListLayer)
            {
                foreach (RobotAnimation ActiveRobot in ActiveLayer.DicRobot.Values)
                    ActiveRobot.EndDraw(g);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            foreach (Player MainCharacter in ListLocalPlayerInfo)
            {
                Draw(g, MainCharacter.InGameRobot, MainCharacter);
                if (ListLocalPlayerInfo.Count > 1)
                {
                    g.End();

                    g.Begin();
                }
            }
        }

        public void Draw(CustomSpriteBatch g, RobotAnimation MainCharacter, Player PlayerInfo)
        {
            if (ListLocalPlayerInfo.Count == 2)
            {
                if (MainCharacter == ListLocalPlayerInfo[0].InGameRobot)
                {
                    g.GraphicsDevice.Viewport = new Viewport(0, 0, MainCharacter.Camera.Width, MainCharacter.Camera.Height);
                }
                if (MainCharacter == ListLocalPlayerInfo[1].InGameRobot)
                {
                    g.GraphicsDevice.Viewport = new Viewport(0, MainCharacter.Camera.Height, MainCharacter.Camera.Width, MainCharacter.Camera.Height);
                }
            }

            Matrix TransformationMatrix = Matrix.CreateTranslation(-MainCharacter.Camera.X, -MainCharacter.Camera.Y, 0);

            g.End();

            if (Background != null)
            {
                Background.Draw(g, Constants.Width, Constants.Height);
            }

            Propulsor.ParticleSystem.Draw(GameScreen.GraphicsDevice, new Vector2(MainCharacter.Camera.X, MainCharacter.Camera.Y));

            #region Layers

            foreach (Layer ActiveLayer in ListLayer)
            {
                g.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, TransformationMatrix);

                if (ActiveLayer.DicRobot.ContainsKey(MainCharacter.ID))
                {
                    sprMeBack.Draw(g, MainCharacter.Position + new Vector2(0, 20), Color.White);
                }

                ActiveLayer.Draw(g);

                g.End();
                g.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, null, null, null, null, TransformationMatrix);

                ActiveLayer.DrawAdditive(g);

                g.End();
                g.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, TransformationMatrix);


                foreach (RobotAnimation ActiveRobot in ActiveLayer.DicRobot.Values)
                {
                    if (!ActiveRobot.IsUpdated)
                        continue;

                    int BarWidth = sprGaugeHPBackground.Width;

                    ActiveRobot.Draw(g, Vector2.Zero);
                    Vector2 ActiveRobotRealPosition = ActiveRobot.Position - new Vector2(BarWidth / 2, 57);

                    if (ShowHPBars && ActiveRobot != MainCharacter && ActiveRobot.IsDynamic)
                    {
                        g.Draw(sprGaugeHPBackground, new Vector2((int)ActiveRobotRealPosition.X, (int)ActiveRobotRealPosition.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        g.Draw(sprGaugeHP,
                            new Rectangle((int)ActiveRobotRealPosition.X, (int)ActiveRobotRealPosition.Y,
                            (int)(ActiveRobot.HP / (float)ActiveRobot.MaxHP * BarWidth), sprGaugeHP.Height),
                            new Rectangle(0, 0, sprGaugeHP.Width, sprGaugeHP.Height),
                            Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    }

                    if (ShowCollisionBoxes)
                    {
                        //Collisions Boxes
                        foreach (Polygon ActivePolygon in ActiveRobot.Collision.ListCollisionPolygon)
                        {
                            g.Draw(sprPixel, new Rectangle((int)ActivePolygon.Center.X - 2,
                                                           (int)ActivePolygon.Center.Y - 2, 5, 5), Color.Red);

                            for (int V = 0; V < ActivePolygon.ArrayVertex.Length; V++)
                            {
                                Vector2 EndPoint;
                                if (V < ActivePolygon.ArrayVertex.Length - 1)
                                {
                                    EndPoint = ActivePolygon.ArrayVertex[V + 1];
                                }
                                else
                                {
                                    EndPoint = ActivePolygon.ArrayVertex[0];
                                }

                                DrawLine(g, ActivePolygon.ArrayVertex[V], EndPoint, Color.Black);

                                if (V < ActivePolygon.ArrayAxis.Length)
                                {
                                    Vector2 StartPoint = ActivePolygon.ArrayVertex[V];
                                    StartPoint += (EndPoint - ActivePolygon.ArrayVertex[V]) / 2;
                                    DrawLine(g, StartPoint, StartPoint + ActivePolygon.ArrayAxis[V] * 20, Color.Red);
                                }
                            }
                        }
                    }

                    foreach (Vehicle ActiveVehicle in ActiveLayer.ListVehicle)
                    {
                        if (ActiveVehicle.CanGetIn(MainCharacter))
                        {
                            sprVehicleInIcon.Draw(g, new Vector2(ActiveVehicle.Position.X, ActiveVehicle.Position.Y - 50), Color.White);
                        }
                    }
                }

                g.End();
            }

            #endregion

            g.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, TransformationMatrix);

            sprMeFront.Draw(g, MainCharacter.Position + new Vector2(0, 20), Color.White);
            sprTeamCursor.Draw(g, MainCharacter.Position - new Vector2(0, 50), Color.White);

            foreach (DamageNumber ActiveDamage in ListDamageNumber)
            {
                if (!ActiveDamage.DamageAnimation.HasEnded)
                {
                    ActiveDamage.DamageAnimation.Draw(g);
                }

                float LifetimeScaling = ActiveDamage.LifetimeRemaining / ActiveDamage.MaxLifetime;
                g.DrawString(fntNumberDamage, ActiveDamage.Damage.ToString(), new Vector2(ActiveDamage.Position.X, ActiveDamage.Position.Y - LifetimeScaling * 20), Color.FromNonPremultiplied(255, 255, 255, (int)(LifetimeScaling * 255)));
            }

            if (ShowCollisionBoxes)
            {
                for (int L = 0; L < ListLayer.Count; L++)
                {
                    foreach (WorldPolygon ActivePolygon in ListLayer[L].ListWorldCollisionPolygon)
                    {
                        for (int V = 0; V < ActivePolygon.Collision.ListCollisionPolygon[0].ArrayVertex.Length; V++)
                        {
                            Vector2 EndPoint;
                            if (V < ActivePolygon.Collision.ListCollisionPolygon[0].ArrayVertex.Length - 1)
                            {
                                EndPoint = ActivePolygon.Collision.ListCollisionPolygon[0].ArrayVertex[V + 1];
                            }
                            else
                            {
                                EndPoint = ActivePolygon.Collision.ListCollisionPolygon[0].ArrayVertex[0];
                            }

                            DrawLine(g, ActivePolygon.Collision.ListCollisionPolygon[0].ArrayVertex[V], EndPoint, Color.Black);
                            if (V < ActivePolygon.Collision.ListCollisionPolygon[0].ArrayAxis.Length)
                            {
                                Vector2 StartPoint = ActivePolygon.Collision.ListCollisionPolygon[0].ArrayVertex[V];
                                StartPoint -= (ActivePolygon.Collision.ListCollisionPolygon[0].ArrayVertex[V] - EndPoint) / 2;
                                DrawLine(g, StartPoint, StartPoint + ActivePolygon.Collision.ListCollisionPolygon[0].ArrayAxis[V] * 20, Color.Red);
                            }
                        }
                    }

                    for (int V = 0; V < ListLayer[L].GroundLevelCollision.ArrayVertex.Length - 1; V++)
                    {
                        Polygon ActiveGroundPolygon = ListLayer[L].GroundLevelCollision;

                        DrawLine(g, ActiveGroundPolygon.ArrayVertex[V], ActiveGroundPolygon.ArrayVertex[V + 1], Color.Red);
                    }
                }
            }

            g.End();

            g.Begin();

            foreach (KillMessage ActiveMessage in ListKillMessage)
            {
                float LifetimeScaling = ActiveMessage.LifetimeRemaining / ActiveMessage.MaxLifetime;
                float X = MainCharacter.Camera.Width / 2;
                float Y = MainCharacter.Camera.Height * 0.60f + 60 * LifetimeScaling;

                g.Draw(ActiveMessage.sprMessagePrefix, new Vector2(X - ActiveMessage.sprMessagePrefix.Width, Y), Color.FromNonPremultiplied(255, 255, 255, (int)(LifetimeScaling * 255)));
                g.DrawString(fntArial9, ActiveMessage.Message, new Vector2(X + 5, Y + 1), Color.FromNonPremultiplied(0, 0, 0, (int)(LifetimeScaling * 255)));
            }

            #region Main Character UI

            if (MainCharacter != null)
            {
                g.Draw(sprHUDBackground, new Vector2(30, MainCharacter.Camera.Height - 48), Color.White);

                g.Draw(sprGaugeHUDHP, new Rectangle(32, MainCharacter.Camera.Height - 44, (int)(MainCharacter.HP / (float)MainCharacter.MaxHP * sprGaugeHUDHP.Width), sprGaugeHUDHP.Height), Color.White);
                g.Draw(sprGaugeBoost, new Rectangle(59, MainCharacter.Camera.Height - 19, (int)(MainCharacter.Equipment.EquipedBooster.JetpackFuel / (float)MainCharacter.Equipment.EquipedBooster.JetpackFuelMax * sprGaugeBoost.Width), sprGaugeBoost.Height), Color.White);

                g.Draw(sprGaugeGrenade, new Rectangle(395, MainCharacter.Camera.Height - 44 - MainCharacter.SecondaryWeapons.Charge + sprGaugeGrenade.Height, sprGaugeGrenade.Width, MainCharacter.SecondaryWeapons.Charge), Color.White);

                if (MainCharacter.PrimaryWeapons.ActiveWeapons[0].AmmoPerMagazine > 0)
                {
                    if (MainCharacter.PrimaryWeapons.ActiveWeapons[0].IsShooting && MainCharacter.PrimaryWeapons.ActiveWeapons[0].CurrentAnimation != null)
                    {
                        AnimationClass ShootingAnimation = MainCharacter.PrimaryWeapons.ActiveWeapons[0].CurrentAnimation;
                        int RecoilValue = (int)(ShootingAnimation.ActiveKeyFrame / (float)ShootingAnimation.LoopEnd * sprGaugeDelay.Height);
                        g.Draw(sprGaugeDelay, new Rectangle(352, MainCharacter.Camera.Height - 42 - RecoilValue + sprGaugeDelay.Height, sprGaugeDelay.Width, RecoilValue), Color.White);
                    }
                    else
                    {
                        g.Draw(sprGaugeDelay, new Rectangle(352, MainCharacter.Camera.Height - 42, sprGaugeDelay.Width, sprGaugeDelay.Height), Color.White);
                    }

                    if (MainCharacter.PrimaryWeapons.ActiveWeapons[0].IsReloading && MainCharacter.PrimaryWeapons.ActiveWeapons[0].CurrentAnimation != null)
                    {
                        AnimationClass ReloadAnimation = MainCharacter.PrimaryWeapons.ActiveWeapons[0].CurrentAnimation;
                        int ReloadValue = (int)(ReloadAnimation.ActiveKeyFrame / (float)ReloadAnimation.LoopEnd * sprGaugeReload.Height);
                        g.Draw(sprGaugeReload, new Rectangle(191, MainCharacter.Camera.Height - 42 - ReloadValue + sprGaugeReload.Height, sprGaugeReload.Width, ReloadValue), Color.White);
                    }
                    else
                    {
                        int RemainingAmmoValue = (int)(MainCharacter.PrimaryWeapons.ActiveWeapons[0].AmmoCurrent / MainCharacter.PrimaryWeapons.ActiveWeapons[0].AmmoPerMagazine * sprGaugeReload.Height);
                        g.Draw(sprGaugeReload, new Rectangle(191, MainCharacter.Camera.Height - 42 - RemainingAmmoValue + sprGaugeReload.Height, sprGaugeReload.Width, RemainingAmmoValue), Color.White);
                    }

                    g.DrawString(fntNumberBullet, MainCharacter.PrimaryWeapons.ActiveWeapons[0].AmmoCurrent.ToString(), new Vector2(216, MainCharacter.Camera.Height - 39), Color.White);
                    g.DrawString(fntNumberBulletSmall, MainCharacter.PrimaryWeapons.ActiveWeapons[0].AmmoPerMagazine.ToString(), new Vector2(231, MainCharacter.Camera.Height - 25), Color.White);
                }

                g.Draw(sprHUDGrenadeForground, new Vector2(407, MainCharacter.Camera.Height - 56), Color.White);
                g.Draw(sprHUDForeground, new Vector2(0, MainCharacter.Camera.Height - 56), Color.White);
                g.Draw(sprHUDBackgroundGlow, new Vector2(214, MainCharacter.Camera.Height - 44), Color.White);

                if (MainCharacter.RespawnTimer > 0)
                {
                    g.Draw(sprHUDRespawnBackground, new Vector2(MainCharacter.Camera.Width / 2, 0), Color.White);
                    g.Draw(sprHUDRespawnGauge, new Rectangle(MainCharacter.Camera.Width / 2 + 8, 9,
                        (int)(MainCharacter.RespawnTimer / RespawnTime * sprHUDRespawnGauge.Width), sprHUDRespawnGauge.Height), Color.White);
                }
            }

            #endregion

            Rules.Draw(g, PlayerInfo);

            if (ScrollingText != null)
            {
                ScrollingText.Draw(g);
            }

            DrawLine(g, new Vector2(MouseHelper.MouseStateCurrent.X - 10, MouseHelper.MouseStateCurrent.Y),
                        new Vector2(MouseHelper.MouseStateCurrent.X + 10, MouseHelper.MouseStateCurrent.Y), Color.Black);
            DrawLine(g, new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y - 10),
                        new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y + 10), Color.Black);
        }

        public byte[] GetSnapshotData()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                using (BinaryWriter BW = new BinaryWriter(MS))
                {
                    BW.Write(FightingZonePath);

                    //Send enemies
                    BW.Write(ListLayer.Count);

                    for (int L = 0; L < ListLayer.Count; ++L)
                    {
                        BW.Write(ListLayer[L].DicRobot.Count);

                        foreach (RobotAnimation ActiveEnemy in ListLayer[L].DicRobot.Values)
                        {
                            BW.Write(ActiveEnemy.ID);
                            BW.Write(ActiveEnemy.Team);
                            if (ActiveEnemy.RobotAI == null)
                            {
                                BW.Write("");
                            }
                            else
                            {
                                BW.Write(ActiveEnemy.RobotAI.Path);
                            }
                            BW.Write(ActiveEnemy.Name);
                            BW.Write(ActiveEnemy.Position.X);
                            BW.Write(ActiveEnemy.Position.Y);

                            int ExtraWeaponCount = 0;
                            for (int W = 0; W < ActiveEnemy.PrimaryWeapons.ActiveWeapons.Count; ++W)
                            {
                                if (ActiveEnemy.PrimaryWeapons.ActiveWeapons[W].IsExtra)
                                {
                                    ++ExtraWeaponCount;
                                }
                            }

                            BW.Write(ExtraWeaponCount);
                            for (int W = 0; W < ActiveEnemy.PrimaryWeapons.ActiveWeapons.Count; ++W)
                            {
                                if (ActiveEnemy.PrimaryWeapons.ActiveWeapons[W].IsExtra)
                                {
                                    BW.Write(ActiveEnemy.PrimaryWeapons.ActiveWeapons[W].WeaponPath);
                                }
                            }
                        }
                    }

                    return MS.ToArray();
                }
            }
        }
    }
}
