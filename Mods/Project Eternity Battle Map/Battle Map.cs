using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using FMOD;
using Roslyn;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using static ProjectEternity.GameScreens.BattleMapScreen.MovementAlgorithmTile;
using ProjectEternity.Core.Vehicle;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract partial class BattleMap : GameScreen, IOnlineGame
    {
        #region Enum definition

        /// <summary>
        /// Focused will make every Unit attack the leader. Spread will make each Unit attack to opposite Unit. ALL is only used to show the leader is using an ALL attack and can't be toggled.
        /// </summary>
        public enum FormationChoices { Focused = 0, Spread = 1, ALL = 2 };
        
        #endregion

        #region Ressources

        protected RenderTarget2D ShakingRenderTraget;

        public FMODSound sndBattleTheme;
        public string sndBattleThemeName;
        //SFX
        public FMODSound sndConfirm;

        public FMODSound sndSelection;
        public FMODSound sndDeny;
        public FMODSound sndCancel;

        public Effect fxOutline;
        public Effect fxGrayscale;

        #region Fonts

        public SpriteFont fntPhaseNumber;
        public SpriteFont fntUnitAttack;
        public SpriteFont fntArial12;
        protected SpriteFont fntArial10;
        public SpriteFont fntArial9;
        public SpriteFont fntArial8;
        protected SpriteFont fntNumbers;
        protected SpriteFont fntAccuracyNormal;//1 pt outline
        protected SpriteFont fntAccuracySmall;//2pt outline Color is #00c6ff for Blue. #ff0000 for Red
        protected SpriteFont fntBattleMenuText;
        public SpriteFont fntNonDemoDamage;
        public SpriteFont fntBattleNumberSmall;
        public SpriteFont fntFinlanderFont;

        #endregion

        public Texture2D sprCursorTerrainSelection;

        protected Texture2D sprEllipse;
        public Texture2D sprCursor;
        public Texture2D sprCursorPath;

        public Texture2D sprUnitHover;

        public Texture2D sprPhaseBackground;
        public Texture2D sprPhasePlayer;
        public Texture2D sprPhaseEnemy;

        public Texture2D sprPhaseTurn;

        public StatusMenuScreen StatusMenu;

        public AttacksMenu AttackPicker;

        #region Bars

        public Texture2D sprBarSmallBackground;
        public Texture2D sprBarSmallEN;
        public Texture2D sprBarSmallHP;
        private Texture2D sprBarLargeBackground;
        private Texture2D sprBarLargeEN;
        private Texture2D sprBarLargeHP;
        protected Texture2D sprBarExtraLargeBackground;
        protected Texture2D sprBarExtraLargeEN;
        protected Texture2D sprBarExtraLargeHP;

        #endregion

        #endregion

        #region Variable definition

        public static char[] Grades = new char[6] { '-', 'S', 'A', 'B', 'C', 'D' };

        public static float WingmanDamageModifier = 0.75f;
        public string MapName;
        public string BattleMapPath;
        public IGameRule GameRule;
        public bool IsFrozen;
        public static string NextMapType = string.Empty;
        public static string NextMapPath = string.Empty;
        public static int ClearedStages = 0;
        public string VictoryCondition;
        public string LossCondition;
        public string SkillPoint;
        public Dictionary<string, double> DicMapVariables;
        public static Dictionary<string, string> DicGlobalVariables;
        public static Dictionary<string, int> DicRouteChoices;
        public static Dictionary<string, BattleMap> DicBattmeMapType = new Dictionary<string, BattleMap>();
        public FormulaParser ActiveParser;

        public Vector3 CursorPosition;//Z is layer index
        public Vector3 CursorPositionVisible;
        public abstract MovementAlgorithmTile CursorTerrain { get; }
        protected float CursorHoldTime;

        public List<Terrain.TilesetPreset> ListTilesetPreset;
        public List<string> ListTerrainType;//Used to store the types of the terrain used.

        public bool IsInit = false;
        public Point TileSize;
        public Point MapSize;
        public int ShowLayerIndex;
        public BattleMapOverlay MapOverlay;
        public bool IsEditor;
        public bool ShowGrid;
        public bool ShowUnits;
        public bool ShowTerrainType;
        public bool ShowTerrainHeight;
        public List<Texture2D> ListTileSet;//Picture of the tilesets used for the map.
        public List<string> ListBackgroundsPath;
        public List<AnimationBackground> ListBackground;
        public List<string> ListForegroundsPath;
        public List<AnimationBackground> ListForeground;

        public Point ScreenSize;//Size in tiles of the maximum amonth of tiles shown by the camera.
        public string CameraType;
        public Vector3 CameraPosition;
        public Camera3D Camera;
        public Camera3D CameraOverride;//Used by vehicles.
        public byte PlayersMin;
        public byte PlayersMax;
        public string Description;

        public List<Color> ListMultiplayerColor;
        public List<BattleMap> ListSubMap;
        protected List<BattleMapPlatform> ListPlatform;
        public List<Vehicle> ListVehicle;
        protected Matrix _World;
        public Matrix World => _World;
        public bool IsAPlatform;//Everything should be handled by the main map.
        public bool IsPlatformActive => _IsPlatformActive;//Tell if the platform has focus.
        protected bool _IsPlatformActive;
        public BattleMapPlatform ActivePlatform => _ActivePlatform;//The platform in which the cursor is.
        public BattleMapPlatform _ActivePlatform;

        public ActionPanelHolder ListActionMenuChoice;
        public Stack<Tuple<int, int>> ListMAPAttackTarget;//Player index, Squad index.
        
        public int ActivePlayerIndex;//Tell which team is playing.
        public int GameTurn;
        public MovementAnimations MovementAnimation;

        public Roster PlayerRoster;
        public List<MapScript> ListMapScript;
        public List<MapEvent> ListMapEvent;
        public Dictionary<string, Unit> DicUnitType = new Dictionary<string, Unit>();
        public Dictionary<string, CutsceneScript> DicCutsceneScript;
        public Dictionary<string, InteractiveProp> DicInteractiveProp = new Dictionary<string, InteractiveProp>();
        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;
        public Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;
        public Dictionary<string, ManualSkillTarget> DicManualSkillTarget;

        public BattleContext GlobalBattleContext;
        protected UnitQuickLoadEffectContext GlobalQuickLoadContext;

        //Classic P2P online
        public OnlineConfiguration OnlinePlayers;

        //Server based online
        public BattleMapOnlineClient OnlineClient;
        public GameServer OnlineServer;
        public BattleMapClientGroup GameGroup;
        public bool IsOfflineOrServer { get { return OnlineClient == null; } }
        public bool IsOnlineClient { get { return OnlineClient != null; } }
        public bool IsServer { get { return GameScreen.GraphicsDevice == null; } }
        public bool IsClient { get { return GameScreen.GraphicsDevice != null; } }

        #region Screen shaking

        public bool IsShaking;
        public bool IsShakingEnded;
        public float ShakeRadiusMax;
        protected float ShakeCounter;
        protected float ShakeAngle;
        protected float ShakeOffsetX;
        protected float ShakeOffsetY;
        protected Vector2 ShakeAngleVariation;

        #endregion

        #region Fade to black

        public bool FadeIsActive;
        public float FadeAlpha;

        #endregion

        #endregion

        /// <summary>
        /// Create a sample
        /// </summary>
        protected BattleMap()
            : base()
        {
            MapSize = new Point(10, 10);
            TileSize = new Point(32, 32);
            ShowLayerIndex = -1;

            IsEditor = false;
            IsFrozen = false;
            OnlinePlayers = new OnlineConfiguration();

            GameTurn = 0;
            MapOverlay = new DayNightCycleColorOnly();
            ListTileSet = new List<Texture2D>();
            ListBackground = new List<AnimationBackground>();
            ListBackgroundsPath = new List<string>();
            ListForeground = new List<AnimationBackground>();
            ListForegroundsPath = new List<string>();
            ListMAPAttackTarget = new Stack<Tuple<int, int>>();

            CameraType = "2D";
            Description = "";
            VictoryCondition = "";
            LossCondition = "";
            SkillPoint = "";
            sndBattleThemeName = "";
            _World = Matrix.Identity;

            DicMapVariables = new Dictionary<string, double>();
            MovementAnimation = new MovementAnimations();

            ListMapScript = new List<MapScript>();
            ListMapEvent = new List<MapEvent>();
            DicCutsceneScript = new Dictionary<string, CutsceneScript>();
            DicInteractiveProp = new Dictionary<string, InteractiveProp>();
            DicRequirement = new Dictionary<string, BaseSkillRequirement>();
            DicEffect = new Dictionary<string, BaseEffect>();
            DicAutomaticSkillTarget = new Dictionary<string, AutomaticSkillTargetType>();
            DicManualSkillTarget = new Dictionary<string, ManualSkillTarget>();
            ListSubMap = new List<BattleMap>();
            ListPlatform = new List<BattleMapPlatform>();
            ListVehicle = new List<Vehicle>();
            ListMultiplayerColor = new List<Color>();

            GlobalBattleContext = new BattleContext();
            GlobalQuickLoadContext = new UnitQuickLoadEffectContext();

            #region Screen shaking

            ShakeCounter = 0;
            ShakeRadiusMax = 3;
            ShakeOffsetX = 0;
            ShakeOffsetY = 0;
            ShakeAngle = RandomHelper.Next(360);
            ShakeAngleVariation = new Vector2(1, 0);

            #endregion
        }

        protected void SaveProperties(BinaryWriter BW)
        {
            BW.Write(MapSize.X);
            BW.Write(MapSize.Y);

            BW.Write(TileSize.X);
            BW.Write(TileSize.Y);

            BW.Write(CameraType);

            BW.Write((int)Math.Max(0, CameraPosition.X));
            BW.Write((int)Math.Max(0, CameraPosition.Y));

            BW.Write(PlayersMin);
            BW.Write(PlayersMax);

            BW.Write(Description);

            BW.Write(ListBackgroundsPath.Count);
            for (int B = 0; B < ListBackgroundsPath.Count; B++)
            {
                BW.Write(ListBackgroundsPath[B]);
            }
            BW.Write(ListForegroundsPath.Count);
            for (int F = 0; F < ListForegroundsPath.Count; F++)
            {
                BW.Write(ListForegroundsPath[F]);
            }

            BW.Write(ListMultiplayerColor.Count);
            //Deathmatch colors
            for (int D = 0; D < ListMultiplayerColor.Count; D++)
            {
                BW.Write(ListMultiplayerColor[D].R);
                BW.Write(ListMultiplayerColor[D].G);
                BW.Write(ListMultiplayerColor[D].B);
            }
        }

        protected void SaveTilesets(BinaryWriter BW)
        {
            //Tile sets
            BW.Write(ListTilesetPreset.Count);
            for (int T = 0; T < ListTilesetPreset.Count; T++)
            {
                Terrain.TilesetPreset.SaveTerrainPreset(BW, ListTilesetPreset[T].ArrayTerrain, ListTilesetPreset[T].TilesetName, ListTilesetPreset[T].ListBattleBackgroundAnimationPath);
            }
        }

        public override void Load()
        {
            if (!IsServer)
            {
                Camera = new DefaultCamera(GraphicsDevice);
                AttackPicker = new AttacksMenu(ActiveParser);

                sndConfirm = new FMODSound(FMODSystem, "Content/SFX/Confirm.mp3");
                sndDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");
                sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
                sndCancel = new FMODSound(FMODSystem, "Content/SFX/Cancel.mp3");

                #region Init outline shader.

                fxOutline = Content.Load<Effect>("Shaders/Outline");
                Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 0, -1f);
                Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                Matrix projectionMatrix = HalfPixelOffset * Projection;

                fxOutline.Parameters["World"].SetValue(Matrix.Identity);
                fxOutline.Parameters["View"].SetValue(Matrix.Identity);
                fxOutline.Parameters["Projection"].SetValue(projectionMatrix);

                fxOutline.Parameters["TextureOffset"].SetValue(new Vector2(1, 1));

                #endregion

                #region Fonts

                fntPhaseNumber = Content.Load<SpriteFont>("Battle/Phase/Number");
                fntPhaseNumber.Spacing = -5;
                fntUnitAttack = Content.Load<SpriteFont>("Fonts/Arial16");
                fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
                fntArial10 = Content.Load<SpriteFont>("Fonts/Arial10");
                fntArial9 = Content.Load<SpriteFont>("Fonts/Arial9");
                fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
                fntNumbers = Content.Load<SpriteFont>("Fonts/VFfont");
                fntBattleMenuText = Content.Load<SpriteFont>("Fonts/Battle Menu Text");
                fntAccuracyNormal = Content.Load<SpriteFont>("Fonts/Accuracy Normal");
                fntAccuracySmall = Content.Load<SpriteFont>("Fonts/Accuracy Small");
                fntNonDemoDamage = Content.Load<SpriteFont>("Fonts/Battle Damage");
                fntNonDemoDamage.Spacing = -5;
                fntBattleNumberSmall = Content.Load<SpriteFont>("Fonts/Battle Numbers Small");
                fntBattleNumberSmall.Spacing = -3;

                #endregion

                StatusMenu = new StatusMenuScreen(this);
                AttackPicker.Load();
                fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

                fxGrayscale = Content.Load<Effect>("Shaders/Grayscale");

                sprEllipse = Content.Load<Texture2D>("Ellipse");
                sprCursor = Content.Load<Texture2D>("Cursors/Reticle");
                sprCursorPath = Content.Load<Texture2D>("Cursors/Cursor Path");
                sprUnitHover = Content.Load<Texture2D>("Units/Unit Hover");

                sprPhaseBackground = Content.Load<Texture2D>("Battle/Phase/Background");
                sprPhasePlayer = Content.Load<Texture2D>("Battle/Phase/Player");
                sprPhaseEnemy = Content.Load<Texture2D>("Battle/Phase/Enemy");
                sprPhaseTurn = Content.Load<Texture2D>("Battle/Phase/Turn");

                sprCursorTerrainSelection = Content.Load<Texture2D>("Battle/Cursor/Terrain Selection");

                #region Bars

                sprBarSmallBackground = Content.Load<Texture2D>("Battle/Bars/Small Bar");
                sprBarSmallEN = Content.Load<Texture2D>("Battle/Bars/Small Energy");
                sprBarSmallHP = Content.Load<Texture2D>("Battle/Bars/Small Health");
                sprBarLargeBackground = Content.Load<Texture2D>("Battle/Bars/Large Bar");
                sprBarLargeEN = Content.Load<Texture2D>("Battle/Bars/Large Energy");
                sprBarLargeHP = Content.Load<Texture2D>("Battle/Bars/Large Health");
                sprBarExtraLargeBackground = Content.Load<Texture2D>("Battle/Bars/Extra Long Bar");
                sprBarExtraLargeEN = Content.Load<Texture2D>("Battle/Bars/Extra Long Energy");
                sprBarExtraLargeHP = Content.Load<Texture2D>("Battle/Bars/Extra Long Health");

                #endregion
            }

            LoadMapScripts();
            LoadEffects();
            LoadAutomaticSkillActivation();
            LoadManualSkillActivation();
            LoadSkillRequirements();
            LoadCutsceneScripts();
            LoadInteractiveProps();
            LoadUnits();
        }

        protected void LoadMapScripts()
        {
            DicMapEvent = new Dictionary<string, MapEvent>();
            DicMapEvent.Add(EventTypeGame, new BattleEvent(EventTypeGame, new string[] { "Game Start" }));
            DicMapEvent.Add(EventTypePhase, new BattleEvent(EventTypePhase, new string[] { "Phase Start" }));
            DicMapEvent.Add(EventTypeTurn, new BattleEvent(EventTypeTurn, new string[] { "Turn Start" }));
            DicMapEvent.Add(EventTypeUnitMoved, new BattleEvent(EventTypeUnitMoved, new string[] { "Unit Moved" }));
            DicMapEvent.Add(EventTypeOnBattle, new BattleEvent(EventTypeOnBattle, new string[] { "Battle Start", "Battle End" }));

            DicMapCondition = MapScript.LoadConditions<BattleCondition>(this);
            DicMapTrigger = MapScript.LoadTriggers<BattleTrigger>(this);
        }

        protected void LoadProperties(BinaryReader BR)
        {
            MapSize.X = BR.ReadInt32();
            MapSize.Y = BR.ReadInt32();

            TileSize.X = BR.ReadInt32();
            TileSize.Y = BR.ReadInt32();

            CameraType = BR.ReadString();

            CameraPosition.X = BR.ReadInt32();
            CameraPosition.Y = BR.ReadInt32();
            CursorPosition = CameraPosition;
            CursorPositionVisible = CursorPosition;

            PlayersMin = BR.ReadByte();
            PlayersMax = BR.ReadByte();

            Description = BR.ReadString();

            int ListBackgroundsPathCount = BR.ReadInt32();
            for (int B = 0; B < ListBackgroundsPathCount; B++)
            {
                ListBackgroundsPath.Add(BR.ReadString());
            }
            int ListForegroundsPathCount = BR.ReadInt32();
            for (int F = 0; F < ListForegroundsPathCount; F++)
            {
                ListForegroundsPath.Add(BR.ReadString());
            }

            int ArrayMultiplayerColorLength = BR.ReadInt32();
            ListMultiplayerColor = new List<Color>(ArrayMultiplayerColorLength);

            for (int D = 0; D < ArrayMultiplayerColorLength; D++)
            {
                ListMultiplayerColor.Add(Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), 255));
            }
        }

        protected void LoadTilesets(BinaryReader BR)
        {
            //Tile sets
            int Tiles = BR.ReadInt32();
            for (int T = 0; T < Tiles; T++)
            {
                ListTilesetPreset.Add(new Terrain.TilesetPreset(BR, TileSize.X, TileSize.Y, T));

                #region Load Tilesets

                string SpritePath = ListTilesetPreset[T].TilesetName;

                if (Content != null)
                {
                    if (File.Exists("Content/Maps/Tilesets/" + SpritePath + ".xnb"))
                        ListTileSet.Add(Content.Load<Texture2D>("Maps/Tilesets/" + SpritePath));
                    else
                        ListTileSet.Add(Content.Load<Texture2D>("Maps/Tilesets/Default"));
                }

                #endregion
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UsePreview">True for preview, false for real time.</param>
        public abstract void TogglePreview(bool UsePreview);

        protected void LoadMapAssets()
        {
            for (int S = ListMapScript.Count - 1; S >= 0; --S)
            {
                switch (ListMapScript[S].MapScriptType)
                {
                    case MapScriptTypes.Trigger:
                        MapTrigger ActiveTrigger = (MapTrigger)ListMapScript[S];
                        ActiveTrigger.Preload();
                        break;
                }
            }

            #region Load Backgrounds and Foregrounds

            for (int B = 0; B < ListBackgroundsPath.Count; B++)
            {
                ListBackground.Add(AnimationBackground.LoadAnimationBackground(ListBackgroundsPath[B], Content, GraphicsDevice));
            }

            for (int F = 0; F < ListForegroundsPath.Count; F++)
            {
                ListForeground.Add(AnimationBackground.LoadAnimationBackground(ListForegroundsPath[F], Content, GraphicsDevice));
            }

            #endregion
        }

        public virtual void LoadEffects()
        {
            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(SkillEffect), new UnitEffectParams(GlobalBattleContext, GlobalQuickLoadContext)))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }
            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll"), typeof(SkillEffect), new UnitEffectParams(GlobalBattleContext, GlobalQuickLoadContext)))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(SkillEffect), new UnitEffectParams(GlobalBattleContext, GlobalQuickLoadContext)))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(SkillEffect), new UnitEffectParams(GlobalBattleContext, GlobalQuickLoadContext)))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
        }

        public virtual void LoadSkillRequirements()
        {
            DicRequirement.Add(BaseSkillRequirement.OnCreatedRequirementName, new OnCreatedRequirement());

            Dictionary<string, BaseSkillRequirement> DicRequirementCore = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(UnitSkillRequirement), GlobalBattleContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCore)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }
            Dictionary<string, BaseSkillRequirement> DicRequirementBattleMap = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll"), typeof(UnitSkillRequirement), GlobalBattleContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementBattleMap)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementCoreAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(UnitSkillRequirement), GlobalBattleContext);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCoreAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementBattleMapAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(UnitSkillRequirement), GlobalBattleContext);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementBattleMapAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }
        }

        public virtual void LoadAutomaticSkillActivation()
        {
            DicAutomaticSkillTarget.Clear();
            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(AutomaticSkillTargetType)))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }
            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll"), typeof(AutomaticSkillTargetType), GlobalBattleContext))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(AutomaticSkillTargetType)))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }
            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(AutomaticSkillTargetType), GlobalBattleContext))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }
        }

        protected virtual void LoadManualSkillActivation()
        {
            DicManualSkillTarget.Clear();
            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll")))
            {
                DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
            }
            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll")))
            {
                DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssembly(ActiveAssembly))
                {
                    DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }
        }

        private void LoadCutsceneScripts()
        {
            ScriptingScriptHolder CoreScripts = new ScriptingScriptHolder();
            KeyValuePair<string, List<CutsceneScript>> NewScripts = CoreScripts.GetNameAndContent();
            foreach (CutsceneScript ActiveListScript in NewScripts.Value)
            {
                DicCutsceneScript.Add(ActiveListScript.Name, ActiveListScript);
            }
            VisualNovelCutsceneScriptHolder VNScripts = new VisualNovelCutsceneScriptHolder();
            KeyValuePair<string, List<CutsceneScript>> NewVNScripts = VNScripts.GetNameAndContent();
            foreach (CutsceneScript ActiveListScript in NewVNScripts.Value)
            {
                DicCutsceneScript.Add(ActiveListScript.Name, ActiveListScript);
            }

            Dictionary<string, CutsceneScript> BattleMapScripts = CutsceneScriptHolder.LoadAllScripts(typeof(BattleMapCutsceneScriptHolder), this);
            foreach (CutsceneScript ActiveListScript in BattleMapScripts.Values)
            {
                DicCutsceneScript.Add(ActiveListScript.Name, ActiveListScript);
            }
        }

        protected virtual void LoadInteractiveProps()
        {
            Dictionary<string, InteractiveProp> BattleMapInteractiveProp = InteractiveProp.LoadProps(this);
            foreach (InteractiveProp ActiveProp in BattleMapInteractiveProp.Values)
            {
                ActiveProp.Load(Content);
                DicInteractiveProp.Add(ActiveProp.PropName, ActiveProp);
            }
        }

        protected virtual void LoadUnits()
        {
            foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssemblyFiles(Directory.GetFiles("Units", "*.dll")))
            {
                DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
            }

            var ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Units", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in RoslynWrapper.GetCompiledAssembliesFromFolder("Units", "*.csx", SearchOption.TopDirectoryOnly))
            {
                foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssembly(ActiveAssembly))
                {
                    DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
                }
            }

            foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssemblyFiles(Directory.GetFiles("Units/Battle Map", "*.dll"), this))
            {
                DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
            }

            foreach (Assembly ActiveAssembly in RoslynWrapper.GetCompiledAssembliesFromFolder("Units/Battle Map", " *.csx", SearchOption.TopDirectoryOnly))
            {
                foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
                }
            }
        }
        
        public static void LoadMapTypes()
        {
            string[] Files;
            bool InstanceIsBaseObject;
            Type ObjectType;

            Files = Directory.GetFiles("Mods", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ass = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                //Get every classes in it.
                Type[] types = ass.GetTypes();
                for (int t = 0; t < types.Length; t++)
                {
                    //Look if the class inherit from Unit somewhere.
                    ObjectType = types[t].BaseType;
                    InstanceIsBaseObject = ObjectType == typeof(BattleMap);
                    while (ObjectType != null && ObjectType != typeof(BattleMap))
                    {
                        ObjectType = ObjectType.BaseType;
                        if (ObjectType == null)
                            InstanceIsBaseObject = false;
                    }
                    //If this class is from BaseEditor, load it.
                    if (InstanceIsBaseObject)
                    {
                        BattleMap instance = Activator.CreateInstance(types[t]) as BattleMap;
                        BattleMap.DicBattmeMapType.Add(instance.GetMapType(), instance);
                    }
                }
            }
        }

        public static BattleMap LoadTemporaryMap(List<GameScreen> ListGameScreen)
        {
            FileStream FS = new FileStream("User Data/Saves/TempSave.sav", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            string BattleMapPath = BR.ReadString();
            string MapAssembly = BR.ReadString();
            Type MapType = Type.GetType(MapAssembly);
            BattleMap NewMap = (BattleMap)Activator.CreateInstance(MapType, 0);
            NewMap.BattleMapPath = BattleMapPath;
            NewMap.ListGameScreen = ListGameScreen;
            NewMap.LoadTemporaryMap(BR);
            NewMap.TogglePreview(true);

            FS.Close();
            BR.Close();

            return NewMap;
        }

        /// <summary>
        /// Automatically called once this Game Screen gain focus.
        /// </summary>
        public virtual void Init()
        {
            //Initialise the ScreenSize based on the map loaded.
            ScreenSize = new Point(Constants.Width / TileSize.X, Constants.Height / TileSize.Y);

            UpdateMapEvent(EventTypeGame, 0);
            IsInit = true;
            RequireDrawFocus = false;
        }

        public void InitOnlineServer(GameServer OnlineServer, BattleMapClientGroup GameGroup)
        {
            this.OnlineServer = OnlineServer;
            this.GameGroup = GameGroup;
        }

        public void InitOnlineClient(BattleMapOnlineClient OnlineClient)
        {
            this.OnlineClient = OnlineClient;
        }

        public void UpdateCursorVisiblePosition(GameTime gameTime)
        {
            if (CursorPositionVisible.X < CursorPosition.X)
            {
                float CursorSpeed = TileSize.X * gameTime.ElapsedGameTime.Milliseconds * 0.0005f;
                if (KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.S))
                    CursorSpeed *= 2;
                CursorPositionVisible.X += CursorSpeed;
                if (CursorPositionVisible.X > CursorPosition.X)
                    CursorPositionVisible.X = CursorPosition.X;
            }
            else if (CursorPositionVisible.X > CursorPosition.X)
            {
                float CursorSpeed = TileSize.X * gameTime.ElapsedGameTime.Milliseconds * 0.0005f;
                if (KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.S))
                    CursorSpeed *= 2;
                CursorPositionVisible.X -= CursorSpeed;
                if (CursorPositionVisible.X < CursorPosition.X)
                    CursorPositionVisible.X = CursorPosition.X;
            }
            if (CursorPositionVisible.Y < CursorPosition.Y)
            {
                float CursorSpeed = TileSize.Y * gameTime.ElapsedGameTime.Milliseconds * 0.0005f;
                if (KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.S))
                    CursorSpeed *= 2;
                CursorPositionVisible.Y += CursorSpeed;
                if (CursorPositionVisible.Y > CursorPosition.Y)
                    CursorPositionVisible.Y = CursorPosition.Y;
            }
            else if (CursorPositionVisible.Y > CursorPosition.Y)
            {
                float CursorSpeed = TileSize.Y * gameTime.ElapsedGameTime.Milliseconds * 0.0005f;
                if (KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.S))
                    CursorSpeed *= 2;

                CursorPositionVisible.Y -= CursorSpeed;
                if (CursorPositionVisible.Y < CursorPosition.Y)
                    CursorPositionVisible.Y = CursorPosition.Y;
            }
        }

        public void GetEmptyPosition(Vector3 TargetPosition, out Vector3 FinalPosition)
        {
            if (TargetPosition.X < 0 || TargetPosition.Y < 0
                || TargetPosition.X >= MapSize.X || TargetPosition.Y >= MapSize.Y)
            {
                FinalPosition = TargetPosition;
                return;
            }

            int MaxValue = 1;
            bool ObstacleFound = true;
            int X = 0, Y = 0;
            FinalPosition = Vector3.Zero;
            //Check if there isn'T a unit at spawn location and move the spawn point if needed.
            do
            {
                X = 0;
                //As long as not out of map or out of range.
                while (X <= MaxValue && ObstacleFound)
                {
                    Y = 0;
                    //As long as not out of map or out of range.
                    while (TargetPosition.Y + Y >= 0 && (X + Y) <= MaxValue && ObstacleFound)
                    {
                        if (ObstacleFound &&
                            TargetPosition.X - X >= 0 && TargetPosition.X - X < MapSize.X &&
                            TargetPosition.Y + Y >= 0 && TargetPosition.Y + Y < MapSize.Y)
                        {
                            ObstacleFound = CheckForObstacleAtPosition(TargetPosition, new Vector3(-X, Y, 0));

                            if (!ObstacleFound)
                            {
                                FinalPosition.X = TargetPosition.X - X;
                                FinalPosition.Y = TargetPosition.Y + Y;
                            }
                        }

                        if (ObstacleFound &&
                            TargetPosition.X + X >= 0 && TargetPosition.X + X < MapSize.X &&
                            TargetPosition.Y + Y >= 0 && TargetPosition.Y + Y < MapSize.Y)
                        {
                            ObstacleFound = CheckForObstacleAtPosition(TargetPosition, new Vector3(X, Y, 0));

                            if (!ObstacleFound)
                            {
                                FinalPosition.X = TargetPosition.X + X;
                                FinalPosition.Y = TargetPosition.Y + Y;
                            }
                        }

                        if (ObstacleFound &&
                            TargetPosition.X - X >= 0 && TargetPosition.X - X < MapSize.X &&
                            TargetPosition.Y - Y >= 0 && TargetPosition.Y - Y < MapSize.Y)
                        {
                            ObstacleFound = CheckForObstacleAtPosition(TargetPosition, new Vector3(-X, -Y, 0));

                            if (!ObstacleFound)
                            {
                                FinalPosition.X = TargetPosition.X - X;
                                FinalPosition.Y = TargetPosition.Y - Y;
                            }
                        }

                        if (ObstacleFound &&
                            TargetPosition.X + X >= 0 && TargetPosition.X + X < MapSize.X &&
                            TargetPosition.Y - Y >= 0 && TargetPosition.Y - Y < MapSize.Y)
                        {
                            ObstacleFound = CheckForObstacleAtPosition(TargetPosition, new Vector3(X, -Y, 0));

                            if (!ObstacleFound)
                            {
                                FinalPosition.X = TargetPosition.X + X;
                                FinalPosition.Y = TargetPosition.Y - Y;
                            }
                        }
                        Y++;//Proceed vertically.
                    }
                    X++;//Proceed horizontally.
                }
                if (ObstacleFound)
                    ++MaxValue;
            }
            while (ObstacleFound);
        }

        /// <summary>
        /// Move the cursor on the map.
        /// </summary>
        /// <returns>Returns true if the cursor was moved</returns>
        public bool CursorControl(PlayerInput ActiveInputManager)
        {
            Point Offset = Point.Zero;
            bool CursorMoved = false;

            /*if (MouseHelper.MouseMoved())
            {
                float NewX = MouseHelper.MouseStateCurrent.X / TileSize.X;
                float NewY = MouseHelper.MouseStateCurrent.Y / TileSize.Y;
                
                NewX += CameraPosition.X;
                NewY += CameraPosition.Y;

                if (NewX < 0)
                    NewX = 0;
                else if (NewX >= MapSize.X)
                    NewX = MapSize.X - 1;
                if (NewY < 0)
                    NewY = 0;
                else if (NewY >= MapSize.Y)
                    NewY = MapSize.Y - 1;

                if (NewX != CursorPosition.X || NewY != CursorPosition.Y)
                {
                    //Update the camera if needed.
                    if (CursorPosition.X - CameraPosition.X - 3 < 0 && CameraPosition.X > -3)
                        --CameraPosition.X;
                    else if (CursorPosition.X - CameraPosition.X + 3 >= ScreenSize.X && CameraPosition.X + ScreenSize.X < MapSize.X + 3)
                        ++CameraPosition.X;

                    if (CursorPosition.Y - CameraPosition.Y - 3 < 0 && CameraPosition.Y > -3)
                        --CameraPosition.Y;
                    else if (CursorPosition.Y - CameraPosition.Y + 3 >= ScreenSize.Y && CameraPosition.Y + ScreenSize.Y < MapSize.Y + 3)
                        ++CameraPosition.Y;

                    CursorPosition.X = NewX;
                    CursorPosition.Y = NewY;
                    CursorMoved = true;
                }
            }
            */
            bool CanKeyboardMove = false;
            if (ActiveInputManager.InputLeftHold() || ActiveInputManager.InputRightHold() || ActiveInputManager.InputUpHold() || ActiveInputManager.InputDownHold())
            {
                if (CursorHoldTime < 0)
                {
                    CanKeyboardMove = true;
                    CursorHoldTime = 0f;
                }
                else
                {
                    CursorHoldTime += 1f;

                    if (CursorHoldTime >= 28)
                    {
                        CanKeyboardMove = true;
                        CursorHoldTime -= 5;
                    }
                    else if (CursorHoldTime == 18)
                    {
                        CanKeyboardMove = true;
                    }
                }
            }
            else
            {
                CursorHoldTime = -1;
            }

            BattleMap ActiveMap = this;
            bool IsMovingLeft = ActiveInputManager.InputLeftHold();
            bool IsMovingRight = ActiveInputManager.InputRightHold();
            bool IsMovingUp = ActiveInputManager.InputUpHold();
            bool IsMovingDown = ActiveInputManager.InputDownHold();

            if (ActivePlatform != null)
            {
                ActiveMap = ActivePlatform.Map;

                float PlatformAngle = MathHelper.ToDegrees(ActivePlatform.Yaw);

                if (PlatformAngle >= 315 || PlatformAngle < 45)
                {
                    //do nothing
                }
                else if (PlatformAngle >= 45 && PlatformAngle < 135)
                {
                    IsMovingLeft = ActiveInputManager.InputDownHold();
                    IsMovingRight = ActiveInputManager.InputUpHold();
                    IsMovingUp = ActiveInputManager.InputLeftHold();
                    IsMovingDown = ActiveInputManager.InputRightHold();
                }
                else if (PlatformAngle >= 135 && PlatformAngle < 225)
                {
                }
                else if (PlatformAngle >= 225 && PlatformAngle < 315)
                {
                }
            }

            //X
            if (IsMovingLeft && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.X - CameraPosition.X - 3 < 0 && CameraPosition.X > -3)
                    --CameraPosition.X;

                Offset.X -= (CursorPosition.X > 0) ? 1 : 0;
                CursorMoved = true;
            }
            else if (IsMovingRight && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.X - CameraPosition.X + 3 >= ScreenSize.X && CameraPosition.X + ScreenSize.X < MapSize.X + 3)
                    ++CameraPosition.X;

                Offset.X += (CursorPosition.X < MapSize.X - 1) ? 1 : 0;
                CursorMoved = true;
            }
            //Y
            if (IsMovingUp && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.Y - CameraPosition.Y - 3 < 0 && CameraPosition.Y > -3)
                    --CameraPosition.Y;

                Offset.Y -= (CursorPosition.Y > 0) ? 1 : 0;
                CursorMoved = true;
            }
            else if (IsMovingDown && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.Y - CameraPosition.Y + 3 >= ScreenSize.Y && CameraPosition.Y + ScreenSize.Y < MapSize.Y + 3)
                    ++CameraPosition.Y;

                Offset.Y += (CursorPosition.Y < MapSize.Y - 1) ? 1 : 0;
                CursorMoved = true;
            }

            if (CursorMoved)
            {
                MovementAlgorithmTile NextTerrain = GetNextLayerIndex(ActiveMap.CursorTerrain, Offset.X, Offset.Y, 1f, 15f, out _);
                ActiveMap.CursorPosition.Z = NextTerrain.LayerIndex;
                ActiveMap.CursorPosition.X = NextTerrain.InternalPosition.X;
                ActiveMap.CursorPosition.Y = NextTerrain.InternalPosition.Y;
            }

            return CursorMoved;
        }

        public MovementAlgorithmTile GetTerrainUnderCursor()
        {
            if (ActivePlatform != null)
            {
                return ActivePlatform.Map.CursorTerrain;
            }

            return CursorTerrain;
        }

        public BattleMapPlatform GetPlatform(BattleMap PlatformMapToSelect)
        {
            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                if (ActivePlatform.Map == PlatformMapToSelect)
                {
                    return ActivePlatform;
                }
            }

            return null;
        }

        public void SelectPlatform(BattleMapPlatform PlatformToSelect)
        {
            if (_ActivePlatform != null)
            {
                _ActivePlatform.Map._IsPlatformActive = false;
            }

            _ActivePlatform = PlatformToSelect;

            if (PlatformToSelect != null)
            {
                PlatformToSelect.Map._IsPlatformActive = true;
            }
        }
    }
}
