using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Vehicle;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using ProjectEternity.Core.Attacks;

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
        public RenderTarget2D MapRenderTarget;

        public FMODSound sndBattleTheme;
        public string sndBattleThemePath;
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
        public List<GameModeInfoHolder> ListGameType;
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

        public Vector3 CursorPosition;//Z is layer index
        public Vector3 CursorPositionVisible;
        public abstract MovementAlgorithmTile CursorTerrain { get; }
        protected float CursorHoldTime;

        public List<Texture2D> ListTileSet;//Picture of the tilesets used for the map.
        public List<Texture2D> ListTemporaryTileSet;//Picture of the tilesets used for the map.
        public List<TilesetPreset> ListTilesetPreset;
        public List<DestructibleTilesetPreset> ListTemporaryTilesetPreset;
        public List<string> ListBattleBackgroundAnimationPath;
        public List<string> ListTemporaryBattleBackgroundAnimationPath;

        public bool IsInit = false;
        public string MapTerrainType;//Grid, Spots, Free roam
        public Point TileSize;
        public int LayerHeight = 32;
        public Point MapSize;
        public int ShowLayerIndex;
        public EnvironmentManager MapEnvironment;
        public bool IsEditor;
        public bool ShowGrid;
        public bool ShowUnits;
        public bool ShowTerrainType;
        public bool ShowTerrainHeight;
        public bool Show3DObjects;
        public List<string> ListBackgroundsPath;
        public List<AnimationBackground> ListBackground;
        public List<string> ListForegroundsPath;
        public List<AnimationBackground> ListForeground;
        public List<BattleMapLight> ListLight;

        public Point ScreenSize;//Size in tiles of the maximum amonth of tiles shown by the camera.
        public string CameraType;
        public Vector3 Camera2DPosition;//Position in the grid
        public Camera3D Camera3D;
        public Camera3D Camera3DOverride;//Used by vehicles.
        public float Camera3DDistance = 255;
        public float Camera3DYawAngle = 11.5f;
        public float Camera3DPitchAngle = 45;

        public uint OrderNumber;
        public byte PlayersMin;
        public byte PlayersMax;
        public byte MaxSquadsPerPlayer;
        public string Description;
        public List<string> ListMandatoryMutator;

        public List<Color> ListMultiplayerColor;
        public List<BattleMap> ListSubMap;
        public List<BattleMapPlatform> ListPlatform;
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
        public Dictionary<string, CutsceneScript> DicCutsceneScript;
        public Dictionary<string, InteractiveProp> DicInteractiveProp = new Dictionary<string, InteractiveProp>();

        public BattleParams Params;

        //Classic P2P online
        public OnlineConfiguration OnlinePlayers;

        //Server based online
        //Client
        public BattleMapOnlineClient OnlineClient;
        public CommunicationClient OnlineCommunicationClient;
        public RoomInformations Room;
        public bool IsChatOpen;
        //Server
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
            MapSize = new Point(20, 20);
            TileSize = new Point(32, 32);
            ShowLayerIndex = -1;

            IsEditor = false;
            IsFrozen = false;
            OnlinePlayers = new OnlineConfiguration();
            ListGameType = new List<GameModeInfoHolder>();

            GameTurn = 0;
            ListTileSet = new List<Texture2D>();
            ListTemporaryTileSet = new List<Texture2D>();
            ListBattleBackgroundAnimationPath = new List<string>();
            ListTemporaryBattleBackgroundAnimationPath = new List<string>();
            ListBackground = new List<AnimationBackground>();
            ListBackgroundsPath = new List<string>();
            ListForeground = new List<AnimationBackground>();
            ListForegroundsPath = new List<string>();
            ListLight = new List<BattleMapLight>();
            ListMAPAttackTarget = new Stack<Tuple<int, int>>();
            ListMandatoryMutator = new List<string>();
            ListTemporaryTilesetPreset = new List<DestructibleTilesetPreset>();

            CameraType = "2D";
            Description = "";
            VictoryCondition = "";
            LossCondition = "";
            SkillPoint = "";
            sndBattleThemePath = "";
            _World = Matrix.Identity;

            DicMapVariables = new Dictionary<string, double>();
            MovementAnimation = new MovementAnimations();

            ListMapScript = new List<MapScript>();
            ListMapEvent = new List<MapEvent>();
            DicCutsceneScript = new Dictionary<string, CutsceneScript>();
            DicInteractiveProp = new Dictionary<string, InteractiveProp>();
            ListSubMap = new List<BattleMap>();
            ListPlatform = new List<BattleMapPlatform>();
            ListVehicle = new List<Vehicle>();
            ListMultiplayerColor = new List<Color>();

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
            //Prioritise displyable informations to avoid reading unnecessary information
            BW.Write(OrderNumber);

            BW.Write(MapSize.X);
            BW.Write(MapSize.Y);

            BW.Write(PlayersMin);
            BW.Write(PlayersMax);
            BW.Write(MaxSquadsPerPlayer);

            BW.Write(Description);
            BW.Write(sndBattleThemePath);

            BW.Write((byte)ListMandatoryMutator.Count);
            for (int M = 0; M < ListMandatoryMutator.Count; M++)
            {
                BW.Write(ListMandatoryMutator[M]);
            }

            BW.Write((byte)ListMultiplayerColor.Count);
            for (int D = 0; D < ListMultiplayerColor.Count; D++)
            {
                BW.Write(ListMultiplayerColor[D].R);
                BW.Write(ListMultiplayerColor[D].G);
                BW.Write(ListMultiplayerColor[D].B);
            }

            BW.Write((byte)ListGameType.Count);
            for (int G = 0; G < ListGameType.Count; G++)
            {
                BW.Write(ListGameType[G].Name);
                ListGameType[G].GameMode.Save(BW);
            }

            //The rest
            BW.Write(TileSize.X);
            BW.Write(TileSize.Y);

            BW.Write(CameraType);

            BW.Write((int)Math.Max(0, Camera2DPosition.X));
            BW.Write((int)Math.Max(0, Camera2DPosition.Y));

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
        }

        protected void SaveTilesets(BinaryWriter BW)
        {
            int RealTilesetCount = 0;
            for (int T = 0; T < ListTilesetPreset.Count; T++)
            {
                if (ListTilesetPreset[T].TilesetType == TilesetPreset.TilesetTypes.Slave)
                {
                    continue;
                }

                ++RealTilesetCount;
            }

            BW.Write(RealTilesetCount);
            for (int T = 0; T < ListTilesetPreset.Count; T++)
            {
                if (ListTilesetPreset[T].TilesetType == TilesetPreset.TilesetTypes.Slave)
                {
                    continue;
                }

                BW.Write(ListTilesetPreset[T].RelativePath);
                BW.Write(ListTilesetPreset[T].TilesetType != TilesetPreset.TilesetTypes.Regular);
            }

            BW.Write(ListBattleBackgroundAnimationPath.Count);
            foreach (string BattleBackgroundAnimationPath in ListBattleBackgroundAnimationPath)
            {
                BW.Write(BattleBackgroundAnimationPath);
            }

            SaveDestructibleTerrainTilesets(BW);
        }

        private void SaveDestructibleTerrainTilesets(BinaryWriter BW)
        {
            int RealTilesetCount = 0;
            for (int T = 0; T < ListTemporaryTilesetPreset.Count; T++)
            {
                if (ListTemporaryTilesetPreset[T].TilesetType == DestructibleTilesAttackAttributes.DestructibleTypes.Slave)
                {
                    continue;
                }

                ++RealTilesetCount;
            }

            BW.Write(RealTilesetCount);
            for (int T = 0; T < ListTemporaryTilesetPreset.Count; T++)
            {
                if (ListTemporaryTilesetPreset[T].TilesetType == DestructibleTilesAttackAttributes.DestructibleTypes.Slave)
                {
                    continue;
                }

                BW.Write(ListTemporaryTilesetPreset[T].RelativePath);
            }

            BW.Write(ListTemporaryBattleBackgroundAnimationPath.Count);
            foreach (string BattleBackgroundAnimationPath in ListTemporaryBattleBackgroundAnimationPath)
            {
                BW.Write(BattleBackgroundAnimationPath);
            }
        }

        public override void Load()
        {
            if (!IsEditor)
            {
                Show3DObjects = true;
            }
            ListSubMap.Add(this);
            if (!IsServer)
            {
                MapRenderTarget = new RenderTarget2D(GraphicsDevice, Constants.Width, Constants.Height, false,
                    GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

                Camera3D = new DefaultCamera(GraphicsDevice);
                AttackPicker = new AttacksMenu(Params.ActiveParser);

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

                fntPhaseNumber = Content.Load<SpriteFont>("Deathmatch/Battle Menu/Phase/Number");
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
                sprCursor = Content.Load<Texture2D>("Editors/Reticle");
                sprCursorPath = Content.Load<Texture2D>("Editors/Cursor Path");
                sprUnitHover = Content.Load<Texture2D>("Deathmatch/Units/Unit Hover");

                sprPhaseBackground = Content.Load<Texture2D>("Deathmatch/Battle Menu/Phase/Background");
                sprPhasePlayer = Content.Load<Texture2D>("Deathmatch/Battle Menu/Phase/Player");
                sprPhaseEnemy = Content.Load<Texture2D>("Deathmatch/Battle Menu/Phase/Enemy");
                sprPhaseTurn = Content.Load<Texture2D>("Deathmatch/Battle Menu/Phase/Turn");

                sprCursorTerrainSelection = Content.Load<Texture2D>("Deathmatch/Battle Menu/Cursor/Terrain Selection");

                #region Bars

                sprBarSmallBackground = Content.Load<Texture2D>("Deathmatch/Battle Menu/Bars/Small Bar");
                sprBarSmallEN = Content.Load<Texture2D>("Deathmatch/Battle Menu/Bars/Small Energy");
                sprBarSmallHP = Content.Load<Texture2D>("Deathmatch/Battle Menu/Bars/Small Health");
                sprBarLargeBackground = Content.Load<Texture2D>("Deathmatch/Battle Menu/Bars/Large Bar");
                sprBarLargeEN = Content.Load<Texture2D>("Deathmatch/Battle Menu/Bars/Large Energy");
                sprBarLargeHP = Content.Load<Texture2D>("Deathmatch/Battle Menu/Bars/Large Health");
                sprBarExtraLargeBackground = Content.Load<Texture2D>("Deathmatch/Battle Menu/Bars/Extra Long Bar");
                sprBarExtraLargeEN = Content.Load<Texture2D>("Deathmatch/Battle Menu/Bars/Extra Long Energy");
                sprBarExtraLargeHP = Content.Load<Texture2D>("Deathmatch/Battle Menu/Bars/Extra Long Health");

                #endregion
            }

            LoadMapScripts();
            LoadCutsceneScripts();
            LoadInteractiveProps();
        }

        protected void LoadMapScripts()
        {
            DicMapEvent = new Dictionary<string, MapEvent>();
            DicMapEvent.Add(EventTypeGame, new BattleEvent(EventTypeGame, new string[] { "Game Start" }));
            DicMapEvent.Add(EventTypePhase, new BattleEvent(EventTypePhase, new string[] { "Phase Start" }));
            DicMapEvent.Add(EventTypeTurn, new BattleEvent(EventTypeTurn, new string[] { "Turn Start" }));
            DicMapEvent.Add(EventTypeUnitMoved, new BattleEvent(EventTypeUnitMoved, new string[] { "Unit Moved" }));
            DicMapEvent.Add(EventTypeOnBattle, new BattleEvent(EventTypeOnBattle, new string[] { "Battle Start", "Battle End" }));
            DicMapEvent.Add(EventTypeGameOver, new BattleEvent(EventTypeGameOver, new string[] { "Game Over" }));
            DicMapEvent.Add(WeaponPickedUpMap, new WeaponPickedUpMapEvent(this));

            DicMapCondition = MapScript.LoadConditions<BattleCondition>(this);
            DicMapTrigger = MapScript.LoadTriggers<BattleTrigger>(this);
        }

        protected void LoadProperties(BinaryReader BR)
        {
            OrderNumber = BR.ReadUInt32();
            
            MapSize.X = BR.ReadInt32();
            MapSize.Y = BR.ReadInt32();

            PlayersMin = BR.ReadByte();
            PlayersMax = BR.ReadByte();
            MaxSquadsPerPlayer = BR.ReadByte();

            Description = BR.ReadString();
            sndBattleThemePath = BR.ReadString();
            if (!string.IsNullOrEmpty(sndBattleThemePath) && File.Exists("Content/Maps/BGM/" + sndBattleThemePath))
            {
                FMODSound NewBattleTheme = new FMODSound(FMODSystem, "Content/Maps/BGM/" + sndBattleThemePath);

                NewBattleTheme.SetLoop(true);
                sndBattleTheme = NewBattleTheme;
            }

            int ListMandatoryMutatorCount = BR.ReadByte();
            for (int M = 0; M < ListMandatoryMutatorCount; M++)
            {
                ListMandatoryMutator.Add(BR.ReadString());
            }

            int ArrayMultiplayerColorLength = BR.ReadByte();
            ListMultiplayerColor = new List<Color>(ArrayMultiplayerColorLength);

            for (int D = 0; D < ArrayMultiplayerColorLength; D++)
            {
                ListMultiplayerColor.Add(Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), 255));
            }

            Dictionary<string, GameModeInfo> DicAvailableGameType = GetAvailableGameModes();

            int ListGameTypeCount = BR.ReadByte();
            for (int G = 0; G < ListGameTypeCount; G++)
            {
                string GameTypeCategoryName = BR.ReadString();
                string GameTypeName = BR.ReadString();
                GameModeInfo LoadedGameType = DicAvailableGameType[GameTypeName].Copy();
                LoadedGameType.Load(BR);
                ListGameType.Add(new GameModeInfoHolder(GameTypeCategoryName, LoadedGameType));
            }

            TileSize.X = BR.ReadInt32();
            TileSize.Y = BR.ReadInt32();

            CameraType = BR.ReadString();

            Camera2DPosition.X = BR.ReadInt32();
            Camera2DPosition.Y = BR.ReadInt32();
            CursorPosition = Camera2DPosition;
            CursorPositionVisible = CursorPosition;

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
        }

        protected virtual TilesetPreset ReadTileset(string TilesetPresetPath, bool IsAutotile, int Index)
        {
            return TilesetPreset.FromFile("/Tilesets presets/" + TilesetPresetPath + ".pet", TilesetPresetPath, Index);
        }

        protected virtual DestructibleTilesetPreset ReadDestructibleTilesetPreset(BinaryReader BR, int Index)
        {
            return new DestructibleTilesetPreset(BR, TileSize.X, TileSize.Y, Index, false);
        }

        protected void LoadTilesets(BinaryReader BR)
        {
            //Tile sets
            int Tiles = BR.ReadInt32();
            for (int T = 0; T < Tiles; T++)
            {
                TilesetPreset LoadedTileset = ReadTileset(BR.ReadString(), BR.ReadBoolean(), T);
                ListTilesetPreset.Add(LoadedTileset);

                #region Load Tilesets

                string SpritePath = ListTilesetPreset[T].ArrayTilesetInformation[0].TilesetName;

                if (Content != null)
                {
                    if (ListTilesetPreset[T].TilesetType == TilesetPreset.TilesetTypes.Regular)
                    {
                        if (File.Exists("Content/Assets/Tilesets/" + SpritePath + ".xnb"))
                            ListTileSet.Add(Content.Load<Texture2D>("Assets/Tilesets/" + SpritePath));
                        else
                            ListTileSet.Add(Content.Load<Texture2D>("Assets/Tilesets/Default"));
                    }
                    else
                    {
                        if (File.Exists("Content/Assets/Autotiles/" + SpritePath + ".xnb"))
                            ListTileSet.Add(Content.Load<Texture2D>("Assets/Autotiles/" + SpritePath));
                        else
                            ListTileSet.Add(Content.Load<Texture2D>("Assets/Autotiles/Default"));
                    }
                }

                #endregion


                for (int i = 1; i < LoadedTileset.ArrayTilesetInformation.Length; i++)
                {
                    SpritePath = ListTilesetPreset[T].ArrayTilesetInformation[i].TilesetName;
                    TilesetPreset ExtraTileset = LoadedTileset.CreateSlave(i);
                    ListTilesetPreset.Add(ExtraTileset);

                    if (File.Exists("Content/Assets/Autotiles/" + SpritePath + ".xnb"))
                        ListTileSet.Add(Content.Load<Texture2D>("Assets/Autotiles/" + SpritePath));
                    else
                        ListTileSet.Add(null);
                }
            }

            int ListBattleBackgroundAnimationPathCount = BR.ReadInt32();
            ListBattleBackgroundAnimationPath = new List<string>(ListBattleBackgroundAnimationPathCount);
            for (int B = 0; B < ListBattleBackgroundAnimationPathCount; B++)
            {
                ListBattleBackgroundAnimationPath.Add(BR.ReadString());
            }
            
            LoadDestructibleTerrainTilesets(BR);
        }

        private void LoadDestructibleTerrainTilesets(BinaryReader BR)
        {
            //Tile sets
            int Tiles = BR.ReadInt32();
            for (int T = 0; T < Tiles; T++)
            {
                DestructibleTilesetPreset LoadedTileset = ReadDestructibleTilesetPreset(BR, T);
                ListTemporaryTilesetPreset.Add(LoadedTileset);

                #region Load Tilesets

                string SpritePath = ListTemporaryTilesetPreset[T].ArrayTilesetInformation[0].TilesetName;

                if (Content != null)
                {
                    if (ListTemporaryTilesetPreset[T].TilesetType == DestructibleTilesAttackAttributes.DestructibleTypes.Regular)
                    {
                        if (File.Exists("Content/Maps/Tilesets/" + SpritePath + ".xnb"))
                            ListTemporaryTileSet.Add(Content.Load<Texture2D>("Maps/Tilesets/" + SpritePath));
                        else
                            ListTemporaryTileSet.Add(Content.Load<Texture2D>("Maps/Tilesets/Default"));
                    }
                    else
                    {
                        if (File.Exists("Content/Maps/Autotiles/" + SpritePath + ".xnb"))
                            ListTemporaryTileSet.Add(Content.Load<Texture2D>("Maps/Autotiles/" + SpritePath));
                        else
                            ListTemporaryTileSet.Add(Content.Load<Texture2D>("Maps/Autotiles/Default"));
                    }
                }

                #endregion

                for (int i = 1; i < LoadedTileset.ArrayTilesetInformation.Length; i++)
                {
                    SpritePath = ListTemporaryTilesetPreset[T].ArrayTilesetInformation[i].TilesetName;
                    DestructibleTilesetPreset ExtraTileset = LoadedTileset.CreateSlave(i);
                    ListTemporaryTilesetPreset.Add(ExtraTileset);

                    if (File.Exists("Content/Maps/Autotiles/" + SpritePath + ".xnb"))
                        ListTemporaryTileSet.Add(Content.Load<Texture2D>("Maps/Autotiles/" + SpritePath));
                    else
                        ListTemporaryTileSet.Add(null);
                }
            }

            int ListBattleBackgroundAnimationPathCount = BR.ReadInt32();
            ListTemporaryBattleBackgroundAnimationPath = new List<string>(ListBattleBackgroundAnimationPathCount);
            for (int B = 0; B < ListBattleBackgroundAnimationPathCount; B++)
            {
                ListTemporaryBattleBackgroundAnimationPath.Add(BR.ReadString());
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

            if (!IsServer)
            {
                for (int B = 0; B < ListBackgroundsPath.Count; B++)
                {
                    ListBackground.Add(AnimationBackground.LoadAnimationBackground(ListBackgroundsPath[B], Content, GraphicsDevice));
                }

                for (int F = 0; F < ListForegroundsPath.Count; F++)
                {
                    ListForeground.Add(AnimationBackground.LoadAnimationBackground(ListForegroundsPath[F], Content, GraphicsDevice));
                }
            }

            #endregion
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
                ActiveProp.LoadPreset(Content);
                DicInteractiveProp.Add(ActiveProp.PropName, ActiveProp);
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

        public virtual void InitOnlineClient(BattleMapOnlineClient OnlineClient, CommunicationClient OnlineCommunicationClient, RoomInformations Room)
        {
            this.OnlineClient = OnlineClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.Room = Room;
        }

        public void UpdateCursorVisiblePosition(GameTime gameTime)
        {
            if (CursorPositionVisible.X < CursorPosition.X)
            {
                float CursorSpeed = TileSize.X * gameTime.ElapsedGameTime.Milliseconds * 0.05f;
                if (KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.S))
                    CursorSpeed *= 2;
                CursorPositionVisible.X += CursorSpeed;
                if (CursorPositionVisible.X > CursorPosition.X)
                    CursorPositionVisible.X = CursorPosition.X;
            }
            else if (CursorPositionVisible.X > CursorPosition.X)
            {
                float CursorSpeed = TileSize.X * gameTime.ElapsedGameTime.Milliseconds * 0.05f;
                if (KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.S))
                    CursorSpeed *= 2;
                CursorPositionVisible.X -= CursorSpeed;
                if (CursorPositionVisible.X < CursorPosition.X)
                    CursorPositionVisible.X = CursorPosition.X;
            }
            if (CursorPositionVisible.Y < CursorPosition.Y)
            {
                float CursorSpeed = TileSize.Y * gameTime.ElapsedGameTime.Milliseconds * 0.05f;
                if (KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.S))
                    CursorSpeed *= 2;
                CursorPositionVisible.Y += CursorSpeed;
                if (CursorPositionVisible.Y > CursorPosition.Y)
                    CursorPositionVisible.Y = CursorPosition.Y;
            }
            else if (CursorPositionVisible.Y > CursorPosition.Y)
            {
                float CursorSpeed = TileSize.Y * gameTime.ElapsedGameTime.Milliseconds * 0.05f;
                if (KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.S))
                    CursorSpeed *= 2;

                CursorPositionVisible.Y -= CursorSpeed;
                if (CursorPositionVisible.Y < CursorPosition.Y)
                    CursorPositionVisible.Y = CursorPosition.Y;
            }
        }

        public bool IsInsideMap(Vector3 WorldPosition)
        {
            return WorldPosition.X < MapSize.X * TileSize.X && WorldPosition.X >= 0 && WorldPosition.Y >= 0 && WorldPosition.Y < MapSize.Y * TileSize.Y;
        }

        public Vector3 ConvertToGridPosition(Vector3 WorldPosition)
        {
            return new Vector3((float)Math.Floor(WorldPosition.X / TileSize.X), (float)Math.Floor(WorldPosition.Y / TileSize.Y), (float)Math.Floor(WorldPosition.Z / LayerHeight));
        }

        public void GetEmptyPosition(Vector3 TargetPosition, out Vector3 FinalPosition)
        {
            if (!IsInsideMap(TargetPosition))
            {
                FinalPosition = TargetPosition;
                return;
            }

            int MaxValue = TileSize.X;
            bool ObstacleFound = true;
            int X = 0, Y = 0;
            FinalPosition = new Vector3(0, 0, TargetPosition.Z);
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
                            IsInsideMap(new Vector3(TargetPosition.X - X, TargetPosition.Y + Y, TargetPosition.Z)))
                        {
                            ObstacleFound = CheckForObstacleAtPosition(TargetPosition, new Vector3(-X, Y, 0));

                            if (!ObstacleFound)
                            {
                                FinalPosition.X = TargetPosition.X - X;
                                FinalPosition.Y = TargetPosition.Y + Y;
                            }
                        }

                        if (ObstacleFound &&
                            IsInsideMap(new Vector3(TargetPosition.X + X, TargetPosition.Y + Y, TargetPosition.Z)))
                        {
                            ObstacleFound = CheckForObstacleAtPosition(TargetPosition, new Vector3(X, Y, 0));

                            if (!ObstacleFound)
                            {
                                FinalPosition.X = TargetPosition.X + X;
                                FinalPosition.Y = TargetPosition.Y + Y;
                            }
                        }

                        if (ObstacleFound &&
                            IsInsideMap(new Vector3(TargetPosition.X - X, TargetPosition.Y - Y, TargetPosition.Z)))
                        {
                            ObstacleFound = CheckForObstacleAtPosition(TargetPosition, new Vector3(-X, -Y, 0));

                            if (!ObstacleFound)
                            {
                                FinalPosition.X = TargetPosition.X - X;
                                FinalPosition.Y = TargetPosition.Y - Y;
                            }
                        }

                        if (ObstacleFound &&
                            IsInsideMap(new Vector3(TargetPosition.X + X, TargetPosition.Y - Y, TargetPosition.Z)))
                        {
                            ObstacleFound = CheckForObstacleAtPosition(TargetPosition, new Vector3(X, -Y, 0));

                            if (!ObstacleFound)
                            {
                                FinalPosition.X = TargetPosition.X + X;
                                FinalPosition.Y = TargetPosition.Y - Y;
                            }
                        }
                        Y += TileSize.Y;//Proceed vertically.
                    }
                    X += TileSize.X;//Proceed horizontally.
                }
                if (ObstacleFound)
                    ++TileSize.X;
            }
            while (ObstacleFound);
        }

        /// <summary>
        /// Move the cursor on the map.
        /// </summary>
        /// <returns>Returns true if the cursor was moved</returns>
        public bool CursorControlGrid(PlayerInput ActiveInputManager, out Point GridOffset, out BattleMap ActiveMap)
        {
            GridOffset = Point.Zero;
            bool CursorMoved = false;

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

            ActiveMap = this;
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

            float CameraSpeed = 23;
            //X
            if (IsMovingLeft && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.X - Camera2DPosition.X - 3 * TileSize.X < 0 && Camera2DPosition.X > -3 * TileSize.X)
                    Camera2DPosition.X -= CameraSpeed;

                GridOffset.X -= (CursorPosition.X > 0) ? 1 : 0;
                CursorMoved = true;
            }
            else if (IsMovingRight && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.X - Camera2DPosition.X + 3 * TileSize.X >= ScreenSize.X * TileSize.X && Camera2DPosition.X + ScreenSize.X * TileSize.X < (MapSize.X + 3) * TileSize.X)
                    Camera2DPosition.X += CameraSpeed;

                GridOffset.X += (CursorPosition.X < (MapSize.X - 1) * TileSize.X) ? 1 : 0;
                CursorMoved = true;
            }
            //Y
            if (IsMovingUp && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.Y - Camera2DPosition.Y - 3 * TileSize.Y * TileSize.Y < 0 && Camera2DPosition.Y > -3 * TileSize.Y)
                    Camera2DPosition.Y -= CameraSpeed;

                GridOffset.Y -= (CursorPosition.Y > 0) ? 1 : 0;
                CursorMoved = true;
            }
            else if (IsMovingDown && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.Y - Camera2DPosition.Y + 3 * TileSize.Y >= ScreenSize.Y * TileSize.Y && Camera2DPosition.Y + ScreenSize.Y * TileSize.Y < (MapSize.Y + 3) * TileSize.Y)
                    Camera2DPosition.Y += CameraSpeed;

                GridOffset.Y += (CursorPosition.Y < (MapSize.Y - 1) * TileSize.Y) ? 1 : 0;
                CursorMoved = true;
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
