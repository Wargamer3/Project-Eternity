using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public partial class ConquestMap : BattleMap, IProjectile3DSandbox
    {
        public static readonly string MapType = "Conquest";

        public Texture2D sprTileBorderRed;
        public Texture2D sprTileBorderBlue;

        public override MovementAlgorithmTile CursorTerrain { get { return GetTerrain(CursorPosition); } }

        public List<Dictionary<string, int>> ListUnitMovementCost;//Terrain Type Index, Movement type, how much it cost to move.
        public Dictionary<string, List<string>> DicWeapon1EffectiveAgainst;//Unit Name, Target Name.
        public Dictionary<string, List<string>> DicWeapon2EffectiveAgainst;//Unit Name, Target Name.
        public Dictionary<string, Dictionary<string, int>> DicUnitDamageWeapon1;//Unit Name, <Target Name, Damage>.
        public Dictionary<string, Dictionary<string, int>> DicUnitDamageWeapon2;//Unit Name, <Target Name, Damage>.
        public Vector3 LastPosition;

        private List<Player> ListLocalPlayerInfo;
        public List<Player> ListPlayer;
        public List<Player> ListLocalPlayer { get { return ListLocalPlayerInfo; } }
        public List<Player> ListAllPlayer { get { return ListPlayer; } }

        public List<BuildingConquest> ListBuilding;

        public List<DelayedAttack> ListDelayedAttack;
        public List<PERAttack> ListPERAttack;
        public MovementAlgorithm Pathfinder;
        public LayerHolderConquest LayerManager;

        public ConquestParams GlobalBattleParams;
        public List<ConquestMutator> ListMutator;
        public Dictionary<Vector3, DestructibleTerrain> DicTemporaryTerrain;//Temporary obstacles
        public ConquestTerrainHolder TerrainHolder;

        public int ActiveUnitIndex
        {
            get
            {
                return _ActiveUnitIndex;
            }
            set
            {
                if (value >= 0)
                {
                    _ActiveUnitIndex = value;
                    _ActiveUnit = ListPlayer[ActivePlayerIndex].ListUnit[value];
                }
                else
                {
                    _ActiveUnitIndex = -1;
                    _ActiveUnit = null;
                }
            }
        }

        private int _ActiveUnitIndex;//Unit selected by the active player.
        public UnitConquest ActiveUnit { get { return _ActiveUnit; } }

        private UnitConquest _ActiveUnit;

        public int TargetSquadIndex
        {
            get
            {
                return _TargetSquadIndex;
            }
            set
            {
                if (value >= 0)
                {
                    _TargetSquadIndex = value;
                    _TargetSquad = ListPlayer[TargetPlayerIndex].ListUnit[value];
                }
                else
                {
                    _TargetSquadIndex = -1;
                    _TargetSquad = null;
                }
            }
        }

        private int _TargetSquadIndex;//Unit targetted by the active player.
        public UnitConquest TargetSquad { get { return _TargetSquad; } }

        private UnitConquest _TargetSquad;
        public int TargetPlayerIndex;//Player of controling TargetUnit.

        public ConquestMap()
            : this(new ConquestParams(new BattleContext()))
        {
            ConquestParams.DicParams.TryAdd(string.Empty, GlobalBattleParams);
        }

        public ConquestMap(ConquestParams Params)
            : base()
        {
            this.Params = GlobalBattleParams = Params;
            Params.AttackParams.SharedParams.Content = Content;

            GameRule = new SinglePlayerGameRule(this);
            LayerManager = new LayerHolderConquest(this);
            MapEnvironment = new EnvironmentManagerConquest(this);
            ListActionMenuChoice = new ActionPanelHolderConquest(this);
            Params.ActiveParser = new ConquestFormulaParser(this);
            ActivePlayerIndex = 0;
            ListPlayer = new List<Player>();
            ListLocalPlayerInfo = new List<Player>();
            RequireFocus = false;
            RequireDrawFocus = true;
            Pathfinder = new MovementAlgorithmConquest(this);
            ListDelayedAttack = new List<DelayedAttack>();
            ListPERAttack = new List<PERAttack>();
            ListMutator = new List<ConquestMutator>();
            DicTemporaryTerrain = new Dictionary<Vector3, DestructibleTerrain>();
            TerrainHolder = new ConquestTerrainHolder();
            ListBuilding = new List<BuildingConquest>();
        }

        public ConquestMap(GameModeInfo GameInfo, ConquestParams Params)
            : this(Params)
        {
            ListTileSet = new List<Texture2D>();
            ListTilesetPreset = new List<TilesetPreset>();
            ListTemporaryTilesetPreset = new List<DestructibleTilesetPreset>();
            Camera2DPosition = Vector3.Zero;
            ActiveUnitIndex = -1;

            if (GameInfo == null)
            {
                GameRule = new SinglePlayerGameRule(this);
            }
            else
            {
                GameRule = GameInfo.GetRule(this);
                if (GameRule == null)
                {
                    GameRule = new SinglePlayerGameRule(this);
                }
            }
        }

        public ConquestMap(string BattleMapPath, GameModeInfo GameInfo, ConquestParams Params)
            : this(GameInfo, Params)
        {
            this.BattleMapPath = BattleMapPath;
        }

        public override void Save(string FilePath)
        {
            //Create the Part file.
            FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            SaveProperties(BW);

            MapScript.SaveMapScripts(BW, ListMapScript);

            SaveTilesets(BW);

            SaveTemporaryTerrain(BW);

            LayerManager.Save(BW);

            MapEnvironment.Save(BW);

            FS.Close();
            BW.Close();
        }

        private void SaveTemporaryTerrain(BinaryWriter BW)
        {
            BW.Write(DicTemporaryTerrain.Count);

            foreach (KeyValuePair<Vector3, DestructibleTerrain> ActiveTemporaryTerrain in DicTemporaryTerrain)
            {
                BW.Write((int)ActiveTemporaryTerrain.Key.X);
                BW.Write((int)ActiveTemporaryTerrain.Key.Y);
                BW.Write((int)ActiveTemporaryTerrain.Key.Z);
                ActiveTemporaryTerrain.Value.ReplacementTerrain.Save(BW);
                ActiveTemporaryTerrain.Value.ReplacementTile.Save(BW);
            }
        }

        public override void Load()
        {
            base.Load();
            LoadMap();
            LoadMapAssets();
            LoadConquestAIScripts();

            var ConquestScripts = CutsceneScriptHolder.LoadAllScripts(typeof(ConquestMapCutsceneScriptHolder), this);
            foreach (CutsceneScript ActiveListScript in ConquestScripts.Values)
            {
                DicCutsceneScript.Add(ActiveListScript.Name, ActiveListScript);
            }

            TerrainHolder.LoadData();

            Player NewPlayer = new Player("Human", "Human", true, false, 0, Color.Red);
            ListPlayer.Add(NewPlayer);

            PopulateUnitMovementCost();
            PopulateUnitDamageWeapon1();
            PopulateUnitDamageWeapon2();

            if (!IsServer)
            {
                fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
                sprTileBorderRed = Content.Load<Texture2D>("Sorcerer Street/Ressources/Tile Border Red Tile");
                sprTileBorderBlue = Content.Load<Texture2D>("Sorcerer Street/Ressources/Tile Border Blue Tile");
            }
        }

        public override void Load(byte[] ArrayGameData)
        {
        }

        public void LoadMap(bool BackgroundOnly = false)
        {
            //Clear everything.
            ListTileSet = new List<Texture2D>();
            FileStream FS = new FileStream("Content/Conquest/Maps/" + BattleMapPath + ".pem", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            MapName = Path.GetFileNameWithoutExtension(BattleMapPath);

            LoadProperties(BR);

            ListMapScript = MapScript.LoadMapScripts(BR, DicMapEvent, DicMapCondition, DicMapTrigger, out ListMapEvent);

            LoadTilesets(BR);

            LoadTemporaryTerrain(BR);

            LayerManager = new LayerHolderConquest(this, BR);

            MapEnvironment = new EnvironmentManagerConquest(BR, this);

            BR.Close();
            FS.Close();
        }

        protected override TilesetPreset ReadTileset(string TilesetPresetPath, int Index)
        {
            return TilesetPreset.FromFile("Conquest", TilesetPresetPath, Index);
        }

        protected override DestructibleTilesetPreset ReadDestructibleTilesetPreset(BinaryReader BR, int Index)
        {
            return new DestructibleTilesetPreset(BR, TileSize.X, TileSize.Y, Index, false);
        }

        private void LoadTemporaryTerrain(BinaryReader BR)
        {
            int DicTemporaryTerrainCount = BR.ReadInt32();

            for (int T = 0; T < DicTemporaryTerrainCount; ++T)
            {
                int GridX = BR.ReadInt32();
                int GridY = BR.ReadInt32();
                int LayerIndex = BR.ReadInt32();
                TerrainConquest ReplacementTerrain = new TerrainConquest(BR, GridX, GridY, TileSize.X, TileSize.Y, LayerIndex, LayerHeight, LayerIndex);
                DrawableTile ReplacementTile = new DrawableTile(BR, TileSize.X, TileSize.Y);

                var NewTerrain = new DestructibleTerrain();
                NewTerrain.ReplacementTerrain = ReplacementTerrain;
                NewTerrain.ReplacementTile = ReplacementTile;
                NewTerrain.RemainingHP = ListTemporaryTileSet[ReplacementTile.TilesetIndex].Height / TileSize.Y;

                DicTemporaryTerrain.Add(new Vector3(GridX, GridY, LayerIndex), NewTerrain);
            }
        }

        public void LoadConquestAIScripts()
        {
            AIScriptHolder.DicAIScripts.Clear();

            foreach (KeyValuePair<string, List<AIScript>> ActiveListScript in ReflectionHelper.GetContentByName<AIScript>(typeof(CoreAI), Directory.GetFiles("AI", "*.dll")))
            {
                for (int S = ActiveListScript.Value.Count - 1; S >= 0; --S)
                    AIScriptHolder.DicAIScripts.Add(ActiveListScript.Value[S].Name, ActiveListScript.Value[S]);
            }
            foreach (KeyValuePair<string, List<AIScript>> ActiveListScript in ReflectionHelper.GetContentByName<AIScript>(typeof(ConquestAIScriptHolder), Directory.GetFiles("AI", "*.dll")))
            {
                for (int S = ActiveListScript.Value.Count - 1; S >= 0; --S)
                    AIScriptHolder.DicAIScripts.Add(ActiveListScript.Value[S].Name, ActiveListScript.Value[S]);
            }
        }

        public override void Init()
        {
            base.Init();

            GameRule.Init();

            if (IsClient && ListPlayer.Count > 0)
            {
                ListActionMenuChoice.Add(new ActionPanelPhaseChange(this));
            }

            ActionPanelPhaseChange.OnNewTurn(this);
        }

        public override void TogglePreview(bool UsePreview)
        {
            ShowUnits = UsePreview;

            if (!UsePreview)
            {
                //Reset game
                if (IsInit)
                {
                    Init();
                }
                if (sndBattleTheme != null)
                {
                    sndBattleTheme.Stop();
                }
            }
            else
            {
                if (sndBattleTheme != null)
                {
                    sndBattleTheme.PlayAsBGM();
                }
            }

            LayerManager.TogglePreview(UsePreview);
        }

        public void PopulateUnitMovementCost()
        {
            ListUnitMovementCost = new List<Dictionary<string, int>>(13);

            #region Terrains (0 to 12)

            #region Plains

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[0].Add("Infantry", 1);
            ListUnitMovementCost[0].Add("Bazooka", 1);
            ListUnitMovementCost[0].Add("TireA", 2);
            ListUnitMovementCost[0].Add("TireB", 1);
            ListUnitMovementCost[0].Add("Tank", 1);
            ListUnitMovementCost[0].Add("Air", 1);
            ListUnitMovementCost[0].Add("Ship", -1);
            ListUnitMovementCost[0].Add("Transport", -1);

            #endregion

            #region Road

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[1].Add("Infantry", 1);
            ListUnitMovementCost[1].Add("Bazooka", 1);
            ListUnitMovementCost[1].Add("TireA", 1);
            ListUnitMovementCost[1].Add("TireB", 1);
            ListUnitMovementCost[1].Add("Tank", 1);
            ListUnitMovementCost[1].Add("Air", 1);
            ListUnitMovementCost[1].Add("Ship", -1);
            ListUnitMovementCost[1].Add("Transport", -1);

            #endregion

            #region Wood

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[2].Add("Infantry", 1);
            ListUnitMovementCost[2].Add("Bazooka", 1);
            ListUnitMovementCost[2].Add("TireA", 3);
            ListUnitMovementCost[2].Add("TireB", 3);
            ListUnitMovementCost[2].Add("Tank", 2);
            ListUnitMovementCost[2].Add("Air", 1);
            ListUnitMovementCost[2].Add("Ship", -1);
            ListUnitMovementCost[2].Add("Transport", -1);

            #endregion

            #region Mountain

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[3].Add("Infantry", 2);
            ListUnitMovementCost[3].Add("Bazooka", 1);
            ListUnitMovementCost[3].Add("TireA", -1);
            ListUnitMovementCost[3].Add("TireB", -1);
            ListUnitMovementCost[3].Add("Tank", -1);
            ListUnitMovementCost[3].Add("Air", 1);
            ListUnitMovementCost[3].Add("Ship", -1);
            ListUnitMovementCost[3].Add("Transport", -1);

            #endregion

            #region Wasteland

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[4].Add("Infantry", 1);
            ListUnitMovementCost[4].Add("Bazooka", 1);
            ListUnitMovementCost[4].Add("TireA", 3);
            ListUnitMovementCost[4].Add("TireB", 3);
            ListUnitMovementCost[4].Add("Tank", 2);
            ListUnitMovementCost[4].Add("Air", 1);
            ListUnitMovementCost[4].Add("Ship", -1);
            ListUnitMovementCost[4].Add("Transport", -1);

            #endregion

            #region Ruins

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[5].Add("Infantry", 1);
            ListUnitMovementCost[5].Add("Bazooka", 1);
            ListUnitMovementCost[5].Add("TireA", 2);
            ListUnitMovementCost[5].Add("TireB", 1);
            ListUnitMovementCost[5].Add("Tank", 1);
            ListUnitMovementCost[5].Add("Air", 1);
            ListUnitMovementCost[5].Add("Ship", -1);
            ListUnitMovementCost[5].Add("Transport", -1);

            #endregion

            #region Sea

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[6].Add("Infantry", -1);
            ListUnitMovementCost[6].Add("Bazooka", -1);
            ListUnitMovementCost[6].Add("TireA", -1);
            ListUnitMovementCost[6].Add("TireB", -1);
            ListUnitMovementCost[6].Add("Tank", -1);
            ListUnitMovementCost[6].Add("Air", 1);
            ListUnitMovementCost[6].Add("Ship", 1);
            ListUnitMovementCost[6].Add("Transport", 1);

            #endregion

            #region Bridge

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[7].Add("Infantry", 1);
            ListUnitMovementCost[7].Add("Bazooka", 1);
            ListUnitMovementCost[7].Add("TireA", 1);
            ListUnitMovementCost[7].Add("TireB", 1);
            ListUnitMovementCost[7].Add("Tank", 1);
            ListUnitMovementCost[7].Add("Air", 1);
            ListUnitMovementCost[7].Add("Ship", 1);
            ListUnitMovementCost[7].Add("Transport", 1);

            #endregion

            #region River

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[8].Add("Infantry", 2);
            ListUnitMovementCost[8].Add("Bazooka", 1);
            ListUnitMovementCost[8].Add("TireA", -1);
            ListUnitMovementCost[8].Add("TireB", -1);
            ListUnitMovementCost[8].Add("Tank", -1);
            ListUnitMovementCost[8].Add("Air", 1);
            ListUnitMovementCost[8].Add("Ship", -1);
            ListUnitMovementCost[8].Add("Transport", -1);

            #endregion

            #region Beach

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[9].Add("Infantry", 1);
            ListUnitMovementCost[9].Add("Bazooka", 1);
            ListUnitMovementCost[9].Add("TireA", 2);
            ListUnitMovementCost[9].Add("TireB", 2);
            ListUnitMovementCost[9].Add("Tank", 1);
            ListUnitMovementCost[9].Add("Air", 1);
            ListUnitMovementCost[9].Add("Ship", -1);
            ListUnitMovementCost[9].Add("Transport", 1);

            #endregion

            #region Rough Sea

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[10].Add("Infantry", -1);
            ListUnitMovementCost[10].Add("Bazooka", -1);
            ListUnitMovementCost[10].Add("TireA", -1);
            ListUnitMovementCost[10].Add("TireB", -1);
            ListUnitMovementCost[10].Add("Tank", -1);
            ListUnitMovementCost[10].Add("Air", 1);
            ListUnitMovementCost[10].Add("Ship", 2);
            ListUnitMovementCost[10].Add("Transport", 2);

            #endregion

            #region Mist

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[11].Add("Infantry", -1);
            ListUnitMovementCost[11].Add("Bazooka", -1);
            ListUnitMovementCost[11].Add("TireA", -1);
            ListUnitMovementCost[11].Add("TireB", -1);
            ListUnitMovementCost[11].Add("Tank", -1);
            ListUnitMovementCost[11].Add("Air", 1);
            ListUnitMovementCost[11].Add("Ship", 1);
            ListUnitMovementCost[11].Add("Transport", 1);

            #endregion

            #region Reef

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[12].Add("Infantry", -1);
            ListUnitMovementCost[12].Add("Bazooka", -1);
            ListUnitMovementCost[12].Add("TireA", -1);
            ListUnitMovementCost[12].Add("TireB", -1);
            ListUnitMovementCost[12].Add("Tank", -1);
            ListUnitMovementCost[12].Add("Air", 1);
            ListUnitMovementCost[12].Add("Ship", 2);
            ListUnitMovementCost[12].Add("Transport", 2);

            #endregion

            #endregion

            #region Properties (13 to 22)

            #region HQ

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[13].Add("Infantry", 1);
            ListUnitMovementCost[13].Add("Bazooka", 1);
            ListUnitMovementCost[13].Add("TireA", 1);
            ListUnitMovementCost[13].Add("TireB", 1);
            ListUnitMovementCost[13].Add("Tank", 1);
            ListUnitMovementCost[13].Add("Air", 1);
            ListUnitMovementCost[13].Add("Ship", -1);
            ListUnitMovementCost[13].Add("Transport", -1);

            #endregion

            #region City

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[14].Add("Infantry", 1);
            ListUnitMovementCost[14].Add("Bazooka", 1);
            ListUnitMovementCost[14].Add("TireA", 1);
            ListUnitMovementCost[14].Add("TireB", 1);
            ListUnitMovementCost[14].Add("Tank", 1);
            ListUnitMovementCost[14].Add("Air", 1);
            ListUnitMovementCost[14].Add("Ship", -1);
            ListUnitMovementCost[14].Add("Transport", -1);

            #endregion

            #region Factory

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[15].Add("Infantry", 1);
            ListUnitMovementCost[15].Add("Bazooka", 1);
            ListUnitMovementCost[15].Add("TireA", 1);
            ListUnitMovementCost[15].Add("TireB", 1);
            ListUnitMovementCost[15].Add("Tank", 1);
            ListUnitMovementCost[15].Add("Air", 1);
            ListUnitMovementCost[15].Add("Ship", -1);
            ListUnitMovementCost[15].Add("Transport", -1);

            #endregion

            #region Airport

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[16].Add("Infantry", 1);
            ListUnitMovementCost[16].Add("Bazooka", 1);
            ListUnitMovementCost[16].Add("TireA", 1);
            ListUnitMovementCost[16].Add("TireB", 1);
            ListUnitMovementCost[16].Add("Tank", 1);
            ListUnitMovementCost[16].Add("Air", 1);
            ListUnitMovementCost[16].Add("Ship", -1);
            ListUnitMovementCost[16].Add("Transport", -1);

            #endregion

            #region Port

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[17].Add("Infantry", 1);
            ListUnitMovementCost[17].Add("Bazooka", 1);
            ListUnitMovementCost[17].Add("TireA", 1);
            ListUnitMovementCost[17].Add("TireB", 1);
            ListUnitMovementCost[17].Add("Tank", 1);
            ListUnitMovementCost[17].Add("Air", 1);
            ListUnitMovementCost[17].Add("Ship", 1);
            ListUnitMovementCost[17].Add("Transport", 1);

            #endregion

            #region Com Tower

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[18].Add("Infantry", 1);
            ListUnitMovementCost[18].Add("Bazooka", 1);
            ListUnitMovementCost[18].Add("TireA", 1);
            ListUnitMovementCost[18].Add("TireB", 1);
            ListUnitMovementCost[18].Add("Tank", 1);
            ListUnitMovementCost[18].Add("Air", 1);
            ListUnitMovementCost[18].Add("Ship", -1);
            ListUnitMovementCost[18].Add("Transport", -1);

            #endregion

            #region Radar

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[19].Add("Infantry", 1);
            ListUnitMovementCost[19].Add("Bazooka", 1);
            ListUnitMovementCost[19].Add("TireA", 1);
            ListUnitMovementCost[19].Add("TireB", 1);
            ListUnitMovementCost[19].Add("Tank", 1);
            ListUnitMovementCost[19].Add("Air", 1);
            ListUnitMovementCost[19].Add("Ship", -1);
            ListUnitMovementCost[19].Add("Transport", -1);

            #endregion

            #region Temp Airport

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[20].Add("Infantry", 1);
            ListUnitMovementCost[20].Add("Bazooka", 1);
            ListUnitMovementCost[20].Add("TireA", 1);
            ListUnitMovementCost[20].Add("TireB", 1);
            ListUnitMovementCost[20].Add("Tank", 1);
            ListUnitMovementCost[20].Add("Air", 1);
            ListUnitMovementCost[20].Add("Ship", -1);
            ListUnitMovementCost[20].Add("Transport", -1);

            #endregion

            #region Temp Port

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[21].Add("Infantry", 1);
            ListUnitMovementCost[21].Add("Bazooka", 1);
            ListUnitMovementCost[21].Add("TireA", 1);
            ListUnitMovementCost[21].Add("TireB", 1);
            ListUnitMovementCost[21].Add("Tank", 1);
            ListUnitMovementCost[21].Add("Air", 1);
            ListUnitMovementCost[21].Add("Ship", 1);
            ListUnitMovementCost[21].Add("Transport", 1);

            #endregion

            #region Missile Silo

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[22].Add("Infantry", 1);
            ListUnitMovementCost[22].Add("Bazooka", 1);
            ListUnitMovementCost[22].Add("TireA", 1);
            ListUnitMovementCost[22].Add("TireB", 1);
            ListUnitMovementCost[22].Add("Tank", 1);
            ListUnitMovementCost[22].Add("Air", 1);
            ListUnitMovementCost[22].Add("Ship", -1);
            ListUnitMovementCost[22].Add("Transport", -1);

            #endregion

            #endregion
        }

        public void PopulateUnitDamageWeapon1()
        {
            DicUnitDamageWeapon1 = new Dictionary<string, Dictionary<string, int>>();
            DicUnitDamageWeapon1.Add("Infantry", new Dictionary<string, int>());

            #region Mech

            DicUnitDamageWeapon1.Add("Mech", new Dictionary<string, int>());//Bazooka
            DicUnitDamageWeapon1["Mech"].Add("Infantry", -1);
            DicUnitDamageWeapon1["Mech"].Add("Mech", -1);
            DicUnitDamageWeapon1["Mech"].Add("Bike", -1);
            DicUnitDamageWeapon1["Mech"].Add("Recon", 85);
            DicUnitDamageWeapon1["Mech"].Add("Flare", 80);
            DicUnitDamageWeapon1["Mech"].Add("Anti-Air", 55);
            DicUnitDamageWeapon1["Mech"].Add("Tank", 55);
            DicUnitDamageWeapon1["Mech"].Add("Md Tank", 25);
            DicUnitDamageWeapon1["Mech"].Add("War Tank", 15);
            DicUnitDamageWeapon1["Mech"].Add("Artillery", 70);
            DicUnitDamageWeapon1["Mech"].Add("Anti-Tank", 55);
            DicUnitDamageWeapon1["Mech"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Mech"].Add("Missile", 85);
            DicUnitDamageWeapon1["Mech"].Add("Rig", 75);

            #endregion

            DicUnitDamageWeapon1.Add("Bike", new Dictionary<string, int>());
            DicUnitDamageWeapon1.Add("Recon", new Dictionary<string, int>());
            DicUnitDamageWeapon1.Add("Flare", new Dictionary<string, int>());

            #region Anti-Air

            DicUnitDamageWeapon1.Add("Anti-Air", new Dictionary<string, int>());//Vulcan Cannon
            DicUnitDamageWeapon1["Anti-Air"].Add("Infantry", 105);
            DicUnitDamageWeapon1["Anti-Air"].Add("Mech", 105);
            DicUnitDamageWeapon1["Anti-Air"].Add("Bike", 105);
            DicUnitDamageWeapon1["Anti-Air"].Add("Recon", 60);
            DicUnitDamageWeapon1["Anti-Air"].Add("Flare", 50);
            DicUnitDamageWeapon1["Anti-Air"].Add("Anti-Air", 45);
            DicUnitDamageWeapon1["Anti-Air"].Add("Tank", 15);
            DicUnitDamageWeapon1["Anti-Air"].Add("Md Tank", 10);
            DicUnitDamageWeapon1["Anti-Air"].Add("War Tank", 5);
            DicUnitDamageWeapon1["Anti-Air"].Add("Artillery", 50);
            DicUnitDamageWeapon1["Anti-Air"].Add("Anti-Tank", 25);
            DicUnitDamageWeapon1["Anti-Air"].Add("Rocket", 55);
            DicUnitDamageWeapon1["Anti-Air"].Add("Missile", 55);
            DicUnitDamageWeapon1["Anti-Air"].Add("Rig", 50);

            #endregion

            #region Tank

            DicUnitDamageWeapon1.Add("Tank", new Dictionary<string, int>());//Tank Gun
            DicUnitDamageWeapon1["Tank"].Add("Infantry", -1);
            DicUnitDamageWeapon1["Tank"].Add("Mech", -1);
            DicUnitDamageWeapon1["Tank"].Add("Bike", -1);
            DicUnitDamageWeapon1["Tank"].Add("Recon", 85);
            DicUnitDamageWeapon1["Tank"].Add("Flare", 80);
            DicUnitDamageWeapon1["Tank"].Add("Anti-Air", 75);
            DicUnitDamageWeapon1["Tank"].Add("Tank", 55);
            DicUnitDamageWeapon1["Tank"].Add("Md Tank", 35);
            DicUnitDamageWeapon1["Tank"].Add("War Tank", 20);
            DicUnitDamageWeapon1["Tank"].Add("Artillery", 70);
            DicUnitDamageWeapon1["Tank"].Add("Anti-Tank", 30);
            DicUnitDamageWeapon1["Tank"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Tank"].Add("Missile", 85);
            DicUnitDamageWeapon1["Tank"].Add("Rig", 75);

            #endregion

            #region Medium Tank

            DicUnitDamageWeapon1.Add("Medium Tank", new Dictionary<string, int>());//Heavy Tank Gun
            DicUnitDamageWeapon1["Medium Tank"].Add("Infantry", -1);
            DicUnitDamageWeapon1["Medium Tank"].Add("Mech", -1);
            DicUnitDamageWeapon1["Medium Tank"].Add("Bike", -1);
            DicUnitDamageWeapon1["Medium Tank"].Add("Recon", 95);
            DicUnitDamageWeapon1["Medium Tank"].Add("Flare", 90);
            DicUnitDamageWeapon1["Medium Tank"].Add("Anti-Air", 90);
            DicUnitDamageWeapon1["Medium Tank"].Add("Tank", 70);
            DicUnitDamageWeapon1["Medium Tank"].Add("Md Tank", 55);
            DicUnitDamageWeapon1["Medium Tank"].Add("War Tank", 35);
            DicUnitDamageWeapon1["Medium Tank"].Add("Artillery", 85);
            DicUnitDamageWeapon1["Medium Tank"].Add("Anti-Tank", 35);
            DicUnitDamageWeapon1["Medium Tank"].Add("Rocket", 90);
            DicUnitDamageWeapon1["Medium Tank"].Add("Missile", 90);
            DicUnitDamageWeapon1["Medium Tank"].Add("Rig", 90);

            #endregion

            #region War Tank

            DicUnitDamageWeapon1.Add("War Tank", new Dictionary<string, int>());//Mega Gun
            DicUnitDamageWeapon1["War Tank"].Add("Infantry", -1);
            DicUnitDamageWeapon1["War Tank"].Add("Mech", -1);
            DicUnitDamageWeapon1["War Tank"].Add("Bike", -1);
            DicUnitDamageWeapon1["War Tank"].Add("Recon", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Flare", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Anti-Air", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Tank", 85);
            DicUnitDamageWeapon1["War Tank"].Add("Md Tank", 75);
            DicUnitDamageWeapon1["War Tank"].Add("War Tank", 50);
            DicUnitDamageWeapon1["War Tank"].Add("Artillery", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Anti-Tank", 40);
            DicUnitDamageWeapon1["War Tank"].Add("Rocket", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Missile", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Rig", 105);

            #endregion

            #region Artillery

            DicUnitDamageWeapon1.Add("Artillery", new Dictionary<string, int>());//Cannon
            DicUnitDamageWeapon1["Artillery"].Add("Infantry", 90);
            DicUnitDamageWeapon1["Artillery"].Add("Mech", 85);
            DicUnitDamageWeapon1["Artillery"].Add("Bike", 85);
            DicUnitDamageWeapon1["Artillery"].Add("Recon", 80);
            DicUnitDamageWeapon1["Artillery"].Add("Flare", 75);
            DicUnitDamageWeapon1["Artillery"].Add("Anti-Air", 65);
            DicUnitDamageWeapon1["Artillery"].Add("Tank", 60);
            DicUnitDamageWeapon1["Artillery"].Add("Md Tank", 45);
            DicUnitDamageWeapon1["Artillery"].Add("War Tank", 35);
            DicUnitDamageWeapon1["Artillery"].Add("Artillery", 75);
            DicUnitDamageWeapon1["Artillery"].Add("Anti-Tank", 55);
            DicUnitDamageWeapon1["Artillery"].Add("Rocket", 80);
            DicUnitDamageWeapon1["Artillery"].Add("Missile", 80);
            DicUnitDamageWeapon1["Artillery"].Add("Rig", 70);

            #endregion

            #region Anti-Tank

            DicUnitDamageWeapon1.Add("Anti-Tank", new Dictionary<string, int>());//Cannon
            DicUnitDamageWeapon1["Anti-Tank"].Add("Infantry", 75);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Mech", 65);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Bike", 65);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Recon", 75);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Flare", 75);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Anti-Air", 75);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Tank", 75);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Md Tank", 65);
            DicUnitDamageWeapon1["Anti-Tank"].Add("War Tank", 55);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Artillery", 65);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Anti-Tank", 55);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Rocket", 70);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Missile", 70);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Rig", 65);

            #endregion

            #region Rockets

            DicUnitDamageWeapon1.Add("Rockets", new Dictionary<string, int>());//Rockets
            DicUnitDamageWeapon1["Rockets"].Add("Infantry", 95);
            DicUnitDamageWeapon1["Rockets"].Add("Mech", 90);
            DicUnitDamageWeapon1["Rockets"].Add("Bike", 90);
            DicUnitDamageWeapon1["Rockets"].Add("Recon", 90);
            DicUnitDamageWeapon1["Rockets"].Add("Flare", 85);
            DicUnitDamageWeapon1["Rockets"].Add("Anti-Air", 75);
            DicUnitDamageWeapon1["Rockets"].Add("Tank", 70);
            DicUnitDamageWeapon1["Rockets"].Add("Md Tank", 55);
            DicUnitDamageWeapon1["Rockets"].Add("War Tank", 45);
            DicUnitDamageWeapon1["Rockets"].Add("Artillery", 80);
            DicUnitDamageWeapon1["Rockets"].Add("Anti-Tank", 65);
            DicUnitDamageWeapon1["Rockets"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Rockets"].Add("Missile", 85);
            DicUnitDamageWeapon1["Rockets"].Add("Rig", 80);

            #endregion

            #region Missiles

            DicUnitDamageWeapon1.Add("Missiles", new Dictionary<string, int>());//Anti-Air Missiles
            DicUnitDamageWeapon1["Missiles"].Add("Infantry", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Mech", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Bike", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Recon", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Flare", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Anti-Air", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Tank", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Md Tank", -1);
            DicUnitDamageWeapon1["Missiles"].Add("War Tank", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Artillery", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Anti-Tank", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Rocket", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Missile", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Rig", -1);

            #endregion

            #region Rig

            DicUnitDamageWeapon1.Add("Rig", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Rig"].Add("Infantry", -1);
            DicUnitDamageWeapon1["Rig"].Add("Mech", -1);
            DicUnitDamageWeapon1["Rig"].Add("Bike", -1);
            DicUnitDamageWeapon1["Rig"].Add("Recon", -1);
            DicUnitDamageWeapon1["Rig"].Add("Flare", -1);
            DicUnitDamageWeapon1["Rig"].Add("Anti-Air", -1);
            DicUnitDamageWeapon1["Rig"].Add("Tank", -1);
            DicUnitDamageWeapon1["Rig"].Add("Md Tank", -1);
            DicUnitDamageWeapon1["Rig"].Add("War Tank", -1);
            DicUnitDamageWeapon1["Rig"].Add("Artillery", -1);
            DicUnitDamageWeapon1["Rig"].Add("Anti-Tank", -1);
            DicUnitDamageWeapon1["Rig"].Add("Rocket", -1);
            DicUnitDamageWeapon1["Rig"].Add("Missile", -1);
            DicUnitDamageWeapon1["Rig"].Add("Rig", -1);

            #endregion
        }

        public void PopulateUnitDamageWeapon2()
        {
            DicUnitDamageWeapon2 = new Dictionary<string, Dictionary<string, int>>();

            #region Infantry

            DicUnitDamageWeapon2.Add("Infantry", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Infantry"].Add("Infantry", 55);
            DicUnitDamageWeapon2["Infantry"].Add("Mech", 45);
            DicUnitDamageWeapon2["Infantry"].Add("Bike", 45);
            DicUnitDamageWeapon2["Infantry"].Add("Recon", 12);
            DicUnitDamageWeapon2["Infantry"].Add("Flare", 10);
            DicUnitDamageWeapon2["Infantry"].Add("Anti-Air", 3);
            DicUnitDamageWeapon2["Infantry"].Add("Tank", 5);
            DicUnitDamageWeapon2["Infantry"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Infantry"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Infantry"].Add("Artillery", 10);
            DicUnitDamageWeapon2["Infantry"].Add("Anti-Tank", 30);
            DicUnitDamageWeapon2["Infantry"].Add("Rocket", 20);
            DicUnitDamageWeapon2["Infantry"].Add("Missile", 20);
            DicUnitDamageWeapon2["Infantry"].Add("Rig", 14);

            #endregion

            #region Mech

            DicUnitDamageWeapon2.Add("Mech", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Mech"].Add("Infantry", 65);
            DicUnitDamageWeapon2["Mech"].Add("Mech", 55);
            DicUnitDamageWeapon2["Mech"].Add("Bike", 55);
            DicUnitDamageWeapon2["Mech"].Add("Recon", 18);
            DicUnitDamageWeapon2["Mech"].Add("Flare", 15);
            DicUnitDamageWeapon2["Mech"].Add("Anti-Air", 5);
            DicUnitDamageWeapon2["Mech"].Add("Tank", 8);
            DicUnitDamageWeapon2["Mech"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Mech"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Mech"].Add("Artillery", 15);
            DicUnitDamageWeapon2["Mech"].Add("Anti-Tank", 35);
            DicUnitDamageWeapon2["Mech"].Add("Rocket", 35);
            DicUnitDamageWeapon2["Mech"].Add("Missile", 35);
            DicUnitDamageWeapon2["Mech"].Add("Rig", 20);

            #endregion

            #region Bike

            DicUnitDamageWeapon2.Add("Bike", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Bike"].Add("Infantry", 65);
            DicUnitDamageWeapon2["Bike"].Add("Mech", 55);
            DicUnitDamageWeapon2["Bike"].Add("Bike", 55);
            DicUnitDamageWeapon2["Bike"].Add("Recon", 18);
            DicUnitDamageWeapon2["Bike"].Add("Flare", 15);
            DicUnitDamageWeapon2["Bike"].Add("Anti-Air", 5);
            DicUnitDamageWeapon2["Bike"].Add("Tank", 8);
            DicUnitDamageWeapon2["Bike"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Bike"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Bike"].Add("Artillery", 15);
            DicUnitDamageWeapon2["Bike"].Add("Anti-Tank", 35);
            DicUnitDamageWeapon2["Bike"].Add("Rocket", 35);
            DicUnitDamageWeapon2["Bike"].Add("Missile", 35);
            DicUnitDamageWeapon2["Bike"].Add("Rig", 20);

            #endregion

            #region Recon

            DicUnitDamageWeapon2.Add("Recon", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Recon"].Add("Infantry", 75);
            DicUnitDamageWeapon2["Recon"].Add("Mech", 65);
            DicUnitDamageWeapon2["Recon"].Add("Bike", 65);
            DicUnitDamageWeapon2["Recon"].Add("Recon", 35);
            DicUnitDamageWeapon2["Recon"].Add("Flare", 30);
            DicUnitDamageWeapon2["Recon"].Add("Anti-Air", 8);
            DicUnitDamageWeapon2["Recon"].Add("Tank", 8);
            DicUnitDamageWeapon2["Recon"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Recon"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Recon"].Add("Artillery", 45);
            DicUnitDamageWeapon2["Recon"].Add("Anti-Tank", 25);
            DicUnitDamageWeapon2["Recon"].Add("Rocket", 55);
            DicUnitDamageWeapon2["Recon"].Add("Missile", 55);
            DicUnitDamageWeapon2["Recon"].Add("Rig", 45);

            #endregion

            #region Flare

            DicUnitDamageWeapon2.Add("Flare", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Flare"].Add("Infantry", 80);
            DicUnitDamageWeapon2["Flare"].Add("Mech", 70);
            DicUnitDamageWeapon2["Flare"].Add("Bike", 70);
            DicUnitDamageWeapon2["Flare"].Add("Recon", 60);
            DicUnitDamageWeapon2["Flare"].Add("Flare", 50);
            DicUnitDamageWeapon2["Flare"].Add("Anti-Air", 45);
            DicUnitDamageWeapon2["Flare"].Add("Tank", 10);
            DicUnitDamageWeapon2["Flare"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Flare"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Flare"].Add("Artillery", 45);
            DicUnitDamageWeapon2["Flare"].Add("Anti-Tank", 25);
            DicUnitDamageWeapon2["Flare"].Add("Rocket", 55);
            DicUnitDamageWeapon2["Flare"].Add("Missile", 55);
            DicUnitDamageWeapon2["Flare"].Add("Rig", 45);

            #endregion

            DicUnitDamageWeapon2.Add("Anti-Air", new Dictionary<string, int>());

            #region Tank

            DicUnitDamageWeapon2.Add("Tank", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Tank"].Add("Infantry", 75);
            DicUnitDamageWeapon2["Tank"].Add("Mech", 70);
            DicUnitDamageWeapon2["Tank"].Add("Bike", 70);
            DicUnitDamageWeapon2["Tank"].Add("Recon", 40);
            DicUnitDamageWeapon2["Tank"].Add("Flare", 35);
            DicUnitDamageWeapon2["Tank"].Add("Anti-Air", 8);
            DicUnitDamageWeapon2["Tank"].Add("Tank", 8);
            DicUnitDamageWeapon2["Tank"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Tank"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Tank"].Add("Artillery", 45);
            DicUnitDamageWeapon2["Tank"].Add("Anti-Tank", 1);
            DicUnitDamageWeapon2["Tank"].Add("Rocket", 55);
            DicUnitDamageWeapon2["Tank"].Add("Missile", 55);
            DicUnitDamageWeapon2["Tank"].Add("Rig", 45);

            #endregion

            #region Medium Tank

            DicUnitDamageWeapon2.Add("Medium Tank", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Medium Tank"].Add("Infantry", 90);
            DicUnitDamageWeapon2["Medium Tank"].Add("Mech", 80);
            DicUnitDamageWeapon2["Medium Tank"].Add("Bike", 80);
            DicUnitDamageWeapon2["Medium Tank"].Add("Recon", 40);
            DicUnitDamageWeapon2["Medium Tank"].Add("Flare", 35);
            DicUnitDamageWeapon2["Medium Tank"].Add("Anti-Air", 8);
            DicUnitDamageWeapon2["Medium Tank"].Add("Tank", 8);
            DicUnitDamageWeapon2["Medium Tank"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Medium Tank"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Medium Tank"].Add("Artillery", 45);
            DicUnitDamageWeapon2["Medium Tank"].Add("Anti-Tank", 1);
            DicUnitDamageWeapon2["Medium Tank"].Add("Rocket", 60);
            DicUnitDamageWeapon2["Medium Tank"].Add("Missile", 60);
            DicUnitDamageWeapon2["Medium Tank"].Add("Rig", 45);

            #endregion

            #region War Tank

            DicUnitDamageWeapon2.Add("War Tank", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["War Tank"].Add("Infantry", 90);
            DicUnitDamageWeapon2["War Tank"].Add("Mech", 80);
            DicUnitDamageWeapon2["War Tank"].Add("Bike", 80);
            DicUnitDamageWeapon2["War Tank"].Add("Recon", 40);
            DicUnitDamageWeapon2["War Tank"].Add("Flare", 35);
            DicUnitDamageWeapon2["War Tank"].Add("Anti-Air", 8);
            DicUnitDamageWeapon2["War Tank"].Add("Tank", 8);
            DicUnitDamageWeapon2["War Tank"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["War Tank"].Add("War Tank", 1);
            DicUnitDamageWeapon2["War Tank"].Add("Artillery", 45);
            DicUnitDamageWeapon2["War Tank"].Add("Anti-Tank", 1);
            DicUnitDamageWeapon2["War Tank"].Add("Rocket", 60);
            DicUnitDamageWeapon2["War Tank"].Add("Missile", 60);
            DicUnitDamageWeapon2["War Tank"].Add("Rig", 45);

            #endregion

            DicUnitDamageWeapon2.Add("Artillery", new Dictionary<string, int>());
            DicUnitDamageWeapon2.Add("Anti-Tank", new Dictionary<string, int>());
            DicUnitDamageWeapon2.Add("Rockets", new Dictionary<string, int>());
            DicUnitDamageWeapon2.Add("Missiles", new Dictionary<string, int>());
            DicUnitDamageWeapon2.Add("Rig", new Dictionary<string, int>());
        }

        public void Reset()
        {
            LayerManager.LayerHolderDrawable.Reset();
            MapEnvironment.Reset();
        }

        public override Tile3D CreateTile3D(int TilesetIndex, Vector3 WorldPosition, Point Origin, Point TileSize, Point TextureSize, float PositionOffset)
        {
            Vector3 TopFrontLeft = GetFinalPosition(new Vector3(WorldPosition.X, WorldPosition.Y + TileSize.Y, WorldPosition.Z));
            Vector3 TopFrontRight = GetFinalPosition(new Vector3(WorldPosition.X + TileSize.X, WorldPosition.Y + TileSize.Y, WorldPosition.Z));
            Vector3 TopBackLeft = GetFinalPosition(new Vector3(WorldPosition.X, WorldPosition.Y, WorldPosition.Z));
            Vector3 TopBackRight = GetFinalPosition(new Vector3(WorldPosition.X + TileSize.X, WorldPosition.Y, WorldPosition.Z));

            return Terrain3D.CreateTile3D(TilesetIndex, TopFrontLeft, TopFrontRight, TopBackLeft, TopBackRight, TileSize, Origin, TextureSize.X, TextureSize.Y, PositionOffset);
        }

        public override void RemoveUnit(int PlayerIndex, object UnitToRemove)
        {
            ListPlayer[ActivePlayerIndex].ListUnit.Remove((UnitConquest)UnitToRemove);
            ListPlayer[ActivePlayerIndex].UpdateAliveStatus();
        }

        public override void AddUnit(int PlayerIndex, object UnitToAdd, Vector3 NewPosition)
        {
            UnitConquest ActiveUnit = (UnitConquest)UnitToAdd;
            ActiveUnit.ReinitializeMembers(GlobalBattleParams.DicUnitType[ActiveUnit.UnitTypeName]);

            ActiveUnit.ReloadSkills(GlobalBattleParams.DicUnitType[ActiveUnit.UnitTypeName], GlobalBattleParams.DicRequirement, GlobalBattleParams.DicEffect, GlobalBattleParams.DicAutomaticSkillTarget, GlobalBattleParams.DicManualSkillTarget);
            ListPlayer[PlayerIndex].ListUnit.Add(ActiveUnit);
            ListPlayer[PlayerIndex].UpdateAliveStatus();
            ActiveUnit.SetPosition(new Vector3(NewPosition.X, NewPosition.Y, NewPosition.Z));

            ActiveUnit.Unit3DSprite.UnitEffect3D.Parameters["World"].SetValue(_World);
        }

        public override void SharePlayer(BattleMapPlayer SharedPlayer, bool IsLocal)
        {
            /*Player NewPlayer = new Player(SharedPlayer);
            ListPlayer.Add(NewPlayer);

            if (IsLocal)
            {
                ListLocalPlayerInfo.Add(NewPlayer);
            }*/
        }

        protected override void DoAddLocalPlayer(OnlinePlayerBase NewPlayer)
        {
            /*Player NewDeahtmatchPlayer = new Player(NewPlayer);

            ListPlayer.Add(NewDeahtmatchPlayer);
            ListLocalPlayerInfo.Add(NewDeahtmatchPlayer);*/
        }

        public override void SetMutators(List<Mutator> ListMutator)
        {
        }

        public override void AddPlatform(BattleMapPlatform NewPlatform)
        {
            /*foreach (Player ActivePlayer in ListPlayer)
            {
                NewPlatform.AddLocalPlayer(ActivePlayer);
            }*/

            ListPlatform.Add(NewPlatform);
        }

        public override void SetWorld(Matrix World)
        {
            /*LayerManager.LayerHolderDrawable.SetWorld(World);

            for (int Z = 0; Z < LayerManager.ListLayer.Count; ++Z)
            {
                Vector3[] ArrayNewPosition = new Vector3[MapSize.X * MapSize.Y];
                for (int X = 0; X < MapSize.X; ++X)
                {
                    for (int Y = 0; Y < MapSize.Y; ++Y)
                    {
                        ArrayNewPosition[X + Y * MapSize.X] = new Vector3(X * 32, (LayerManager.ListLayer[Z].ArrayTerrain[X, Y].Height + Z) * Map.LayerHeight, Y * 32);
                    }
                }

                Vector3.Transform(ArrayNewPosition, ref World, ArrayNewPosition);

                for (int X = 0; X < MapSize.X; ++X)
                {
                    for (int Y = 0; Y < MapSize.Y; ++Y)
                    {
                        LayerManager.ListLayer[Z].ArrayTerrain[X, Y].Position
                            = new Vector3((float)Math.Round(ArrayNewPosition[X + Y * MapSize.X].X / 32), (float)Math.Round(ArrayNewPosition[X + Y * MapSize.X].Z / 32), ArrayNewPosition[X + Y * MapSize.X].Y / 32);
                    }
                }
            }*/
        }

        public bool CheckForObstacleAtPosition(int PlayerIndex, Vector3 WorldPosition, Vector3 Displacement)
        {
            return CheckForUnitAtPosition(PlayerIndex, WorldPosition, Displacement) >= 0;
        }

        public override bool CheckForObstacleAtPosition(Vector3 WorldPosition, Vector3 Displacement)
        {
            bool ObstacleFound = false;

            for (int P = 0; P < ListPlayer.Count && !ObstacleFound; P++)
                ObstacleFound = CheckForObstacleAtPosition(P, WorldPosition, Displacement);

            return ObstacleFound;
        }

        public int CheckForUnitAtPosition(int PlayerIndex, Vector3 WorldPosition, Vector3 Displacement)
        {
            if (ListPlayer[PlayerIndex].ListUnit.Count == 0)
                return -1;

            Vector3 FinalPosition = WorldPosition + Displacement;

            if (!IsInsideMap(FinalPosition))
                return -1;

            int S = 0;
            bool SquadFound = false;
            //Check if there's a Construction.
            while (S < ListPlayer[PlayerIndex].ListUnit.Count && !SquadFound)
            {
                if (ListPlayer[PlayerIndex].ListUnit[S] == null)
                {
                    ++S;
                    continue;
                }
                if (ListPlayer[PlayerIndex].ListUnit[S].IsUnitAtPosition(FinalPosition, TileSize))
                    SquadFound = true;
                else
                    ++S;
            }
            //If a Unit was founded.
            if (SquadFound)
                return S;

            return -1;
        }

        public int CheckForBuildingPosition(Vector3 WorldPosition)
        {
            Vector3 FinalPosition = WorldPosition;

            if (!IsInsideMap(FinalPosition))
                return -1;

            Vector3 GridPosition = ConvertToGridPosition(FinalPosition);

            for (int B = 0; B < ListBuilding.Count; B++)
            {
                Vector3 BuildingGridPosition = ConvertToGridPosition(ListBuilding[B].Position);

                if (BuildingGridPosition == GridPosition)
                {
                    return B;
                }
            }

            return -1;
        }

        public List<MovementAlgorithmTile> GetMVChoice(UnitConquest CurrentUnit, ConquestMap ActiveMap)
        {
            int StartingMV = GetSquadMaxMovement(CurrentUnit);//Maximum distance you can reach.

            //Init A star.
            List<MovementAlgorithmTile> ListAllNode = Pathfinder.FindPath(GetAllTerrain(CurrentUnit.Components, ActiveMap), CurrentUnit.Components, CurrentUnit.UnitStat, StartingMV, false);

            List<MovementAlgorithmTile> MovementChoice = new List<MovementAlgorithmTile>();

            for (int i = 0; i < ListAllNode.Count; i++)
            {
                ListAllNode[i].ParentTemp = null;//Unset parents
                ListAllNode[i].MovementCost = 0;

                if (ListAllNode[i].TerrainTypeIndex == UnitStats.TerrainWallIndex || ListAllNode[i].TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    continue;
                }
                bool UnitFound = false;
                for (int P = 0; P < ListPlayer.Count && !UnitFound; P++)
                {
                    int SquadIndex = CheckForSquadAtPosition(P, ListAllNode[i].WorldPosition, Vector3.Zero);
                    if (SquadIndex >= 0)
                        UnitFound = true;
                }
                //If there is no Unit.
                if (!UnitFound)
                    MovementChoice.Add(ListAllNode[i]);
            }

            return MovementChoice;
        }

        public int GetSquadMaxMovement(UnitConquest ActiveUnit)
        {
            if (ActiveUnit.CurrentMovement == UnitStats.TerrainAirIndex)
            {
                int StartingMV = Math.Min(ActiveUnit.MaxMovement, ActiveUnit.EN);//Maximum distance you can reach.

                return StartingMV;
            }
            else
            {
                int StartingMV = ActiveUnit.MaxMovement;//Maximum distance you can reach.

                return StartingMV;
            }
        }

        public void FinalizeMovement(UnitConquest ActiveUnit, int UsedMovement, List<Vector3> ListMVHoverPoints)
        {
            Params.GlobalContext.ListAttackPickedUp.Clear();
            Params.GlobalContext.ListMVPoints.Clear();

            Params.GlobalContext.ListMVPoints.AddRange(ListMVHoverPoints);

            TerrainConquest ActiveTerrain = GetTerrain(ActiveUnit.Components);

            ActiveUnit.CurrentMovement = ActiveTerrain.TerrainTypeIndex;

            HashSet<int> ListLayerIndex = new HashSet<int>();

            if (ListMVHoverPoints.Count > 0)
            {
                float TotalENCost = 0;
                foreach (Vector3 TerrainCrossed in ListMVHoverPoints)
                {
                    TotalENCost += 0;
                    ListLayerIndex.Add((int)TerrainCrossed.Z);
                }
                if (TotalENCost > 0)
                {
                    ActiveUnit.ConsumeEN((int)TotalENCost);
                }

                foreach (int ActiveLayerIndex in ListLayerIndex)
                {
                    BaseMapLayer ActiveLayer = LayerManager[ActiveLayerIndex];

                    for (int P = ActiveLayer.ListProp.Count - 1; P >= 0; P--)
                    {
                        ActiveLayer.ListProp[P].FinishMoving(ActiveUnit, ActiveUnit.Components, ListMVHoverPoints);
                    }

                    for (int A = ActiveLayer.ListAttackPickup.Count - 1; A >= 0; A--)
                    {
                        Core.Attacks.TemporaryAttackPickup ActiveAttack = ActiveLayer.ListAttackPickup[A];
                        if (ListMVHoverPoints.Contains(ActiveAttack.Position))
                        {
                            ActiveUnit.AddTemporaryAttack(ActiveAttack, Content, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget);
                            Params.GlobalContext.ListAttackPickedUp.Add(ActiveAttack.AttackName);
                            ActiveLayer.ListAttackPickup.RemoveAt(A);
                        }
                    }

                    for (int I = ActiveLayer.ListHoldableItem.Count - 1; I >= 0; I--)
                    {
                        HoldableItem ActiveItem = ActiveLayer.ListHoldableItem[I];
                        foreach (Vector3 MovedOverPoint in ListMVHoverPoints)
                        {
                            ActiveItem.OnMovedOverBeforeStop(ActiveUnit.Components, MovedOverPoint, ActiveUnit.Position);
                        }

                        ActiveItem.OnUnitStop(ActiveUnit.Components);
                    }
                }
            }

            ActivateAutomaticSkills(null, ActiveUnit, BaseSkillRequirement.AfterMovingRequirementName, null, ActiveUnit);
            UpdateMapEvent(EventTypeUnitMoved, 0);
            UpdateMapEvent(WeaponPickedUpMap, 0);
            LayerManager.UnitMoved(ActivePlayerIndex);
        }

        public override void Update(GameTime gameTime)
        {
            GlobalBattleParams.Map = this;

            if (!IsFrozen)
            {
                if (ShowUnits)
                {
                    MapEnvironment.Update(gameTime);
                }

                if (Show3DObjects)
                {
                    for (int B = 0; B < ListBackground.Count; ++B)
                    {
                        ListBackground[B].Update(gameTime);
                    }

                    for (int F = 0; F < ListForeground.Count; ++F)
                    {
                        ListForeground[F].Update(gameTime);
                    }
                }

                LayerManager.Update(gameTime);

                UpdateCursorVisiblePosition(gameTime);

                if (!IsOnTop || IsAPlatform)//Everything should be handled by the main map.
                {
                    return;
                }

                if (!IsInit)
                {
                    Init();
                }
                if (MovementAnimation.Count > 0)
                {
                    MovementAnimation.MoveSquad(gameTime, this, 100);
                }
                if (!MovementAnimation.IsBlocking || MovementAnimation.Count == 0)
                {
                    GameRule.Update(gameTime);
                }

                foreach (BattleMapPlatform ActivePlatform in ListPlatform)
                {
                    ActivePlatform.Update(gameTime);
                }
            }
        }

        public override void Update(double ElapsedSeconds)
        {
            GameTime UpdateTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(ElapsedSeconds));

            if (!IsInit)
            {
                if (ListGameScreen.Count == 0)
                {
                    Load();
                    Init();
                    TogglePreview(true);

                    if (ListGameScreen.Count == 0)
                    {
                    }
                    else
                    {
                        IsInit = false;
                    }
                }
                else
                {
                    ListGameScreen[0].Update(UpdateTime);
                    if (!ListGameScreen[0].Alive)
                    {
                        ListGameScreen.RemoveAt(0);
                    }

                    if (ListGameScreen.Count == 0)
                    {
                        IsInit = true;
                    }
                }
            }

            LayerManager.Update(UpdateTime);

            if (!ListPlayer[ActivePlayerIndex].IsPlayerControlled && ListActionMenuChoice.HasMainPanel)
            {
                ListActionMenuChoice.Last().Update(UpdateTime);
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.BeginUnscaled(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

            LayerManager.BeginDraw(g);

            g.End();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            LayerManager.Draw(g);

            if (IsOnTop && !IsEditor)
            {
                if (ListActionMenuChoice.HasMainPanel)
                {
                    ListActionMenuChoice.Last().Draw(g);
                }
            }
        }

        public TerrainConquest GetTerrain(UnitMapComponent ActiveUnit)
        {
            return GetTerrain(ActiveUnit.Position);
        }

        public TerrainConquest GetTerrain(Vector3 WorldPosition)
        {
            Vector3 GridPosition = ConvertToGridPosition(WorldPosition);

            if (GridPosition.X < 0 || GridPosition.X >= MapSize.X || GridPosition.Y < 0 || GridPosition.Y >= MapSize.Y || GridPosition.Z < 0 || GridPosition.Z >= LayerManager.ListLayer.Count)
            {
                return null;
            }

            DestructibleTerrain TemporaryTerrain;
            if (DicTemporaryTerrain.TryGetValue(GridPosition, out TemporaryTerrain))
            {
                return TemporaryTerrain.ReplacementTerrain;
            }

            return LayerManager.ListLayer[(int)GridPosition.Z].ArrayTerrain[(int)GridPosition.X, (int)GridPosition.Y];
        }

        public Vector3 GetNextLayerTile(MovementAlgorithmTile StartingPosition, float WorldffsetX, float WorldOffsetY, float MaxClearance, float ClimbValue, out List<MovementAlgorithmTile> ListLayerPossibility)
        {
            ListLayerPossibility = new List<MovementAlgorithmTile>();
            float NextX = StartingPosition.WorldPosition.X + WorldffsetX;
            float NextY = StartingPosition.WorldPosition.Y + WorldOffsetY;

            if (!IsInsideMap(new Vector3(NextX, NextY, 0)))
            {
                return StartingPosition.WorldPosition;
            }

            byte CurrentTerrainIndex = StartingPosition.TerrainTypeIndex;
            ConquestTerrainTypeAttributes CurrentTerrainType = TerrainHolder.ListConquestTerrainType[CurrentTerrainIndex];

            bool IsOnUsableTerrain = CurrentTerrainType.DicMovementCostByMoveType.Count > 0;

            float CurrentZ = StartingPosition.WorldPosition.Z;

            MovementAlgorithmTile ClosestLayerIndexDown = null;
            MovementAlgorithmTile ClosestLayerIndexUp = StartingPosition;
            float ClosestTerrainDistanceDown = float.MaxValue;
            float ClosestTerrainDistanceUp = float.MinValue;

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                MovementAlgorithmTile NextTerrain = GetTerrainIncludingPlatforms(new Vector3(StartingPosition.WorldPosition.X + WorldffsetX, StartingPosition.WorldPosition.Y + WorldOffsetY, L * LayerHeight));
                byte NextTerrainIndex = NextTerrain.TerrainTypeIndex;
                ConquestTerrainTypeAttributes NextTerrainType = TerrainHolder.ListConquestTerrainType[NextTerrainIndex];
                bool IsNextTerrainnUsable = NextTerrainType.DicMovementCostByMoveType.Count > 0;

                Terrain PreviousTerrain = GetTerrain(new Vector3(StartingPosition.WorldPosition.X, StartingPosition.WorldPosition.Y, L));
                ConquestTerrainTypeAttributes PreviousTerrainType = TerrainHolder.ListConquestTerrainType[PreviousTerrain.TerrainTypeIndex];
                bool IsPreviousTerrainnUsable = PreviousTerrainType.DicMovementCostByMoveType.Count > 0;

                if (L > StartingPosition.LayerIndex && PreviousTerrainType.DicMovementCostByMoveType.Count == 0)
                {
                    break;
                }

                float NextTerrainZ = NextTerrain.WorldPosition.Z;

                //Check lower or higher neighbors if on solid ground
                if (IsOnUsableTerrain)
                {
                    if (IsNextTerrainnUsable)
                    {
                        //Prioritize going downward
                        if (NextTerrainZ <= CurrentZ)
                        {
                            float ZDiff = CurrentZ - NextTerrainZ;
                            if (ZDiff <= ClosestTerrainDistanceDown && HasEnoughClearance(new Vector3(NextX, NextY, NextTerrainZ), L, MaxClearance))
                            {
                                ClosestTerrainDistanceDown = ZDiff;
                                ClosestLayerIndexDown = NextTerrain;
                                ListLayerPossibility.Add(NextTerrain);
                            }
                        }
                        else
                        {
                            float ZDiff = NextTerrainZ - CurrentZ;
                            if (ZDiff >= ClosestTerrainDistanceUp && ZDiff <= ClimbValue)
                            {
                                if (IsPreviousTerrainnUsable)
                                {
                                    ClosestTerrainDistanceUp = ZDiff;
                                    ClosestLayerIndexUp = NextTerrain;
                                    ListLayerPossibility.Add(NextTerrain);
                                }
                            }
                        }
                    }
                }
                //Already in void, check for any neighbors
                else
                {
                    if (NextTerrainZ == StartingPosition.LayerIndex && NextTerrainIndex == CurrentTerrainIndex)
                    {
                        return NextTerrain.WorldPosition;
                    }
                    //Prioritize going upward
                    else if (NextTerrainZ > StartingPosition.LayerIndex)
                    {
                        float ZDiff = NextTerrainZ - CurrentZ;
                        if (ZDiff < ClosestTerrainDistanceUp && ZDiff <= ClimbValue)
                        {
                            ClosestTerrainDistanceUp = ZDiff;
                            ClosestLayerIndexUp = NextTerrain;
                            ListLayerPossibility.Add(NextTerrain);
                        }
                    }
                }
            }

            if (ClosestLayerIndexDown != null)
            {
                return ClosestLayerIndexDown.WorldPosition;
            }
            else
            {
                return ClosestLayerIndexUp.WorldPosition;
            }
        }

        public MovementAlgorithmTile GetTerrainIncludingPlatforms(Vector3 WorldPosition)
        {
            if (!IsAPlatform)
            {
                foreach (BattleMapPlatform ActivePlatform in ListPlatform)
                {
                    MovementAlgorithmTile FoundTile = ((ConquestMap)ActivePlatform.Map).GetTerrain(WorldPosition);

                    if (FoundTile != null)
                    {
                        return FoundTile;
                    }
                }
            }

            return GetTerrain(WorldPosition);
        }


        private bool HasEnoughClearance(Vector3 WorldPosition, int StartLayer, float MaxClearance)
        {
            for (int L = StartLayer + 1; L < LayerManager.ListLayer.Count; L++)
            {
                Terrain ActiveTerrain = GetTerrain(new Vector3(WorldPosition.X, WorldPosition.Y, L * LayerHeight));

                var NextTerrainType = TerrainHolder.ListConquestTerrainType[ActiveTerrain.TerrainTypeIndex];
                float NextTerrainZ = ActiveTerrain.WorldPosition.Z;

                float ZDiff = NextTerrainZ - WorldPosition.Z;

                if (NextTerrainType.DicMovementCostByMoveType.Count > 0 && ZDiff != 0 && ZDiff < MaxClearance)
                {
                    return false;
                }
            }

            return true;
        }

        public float GetMVCost(UnitMapComponent Unit, UnitStats Stats, byte TerrainTypeIndex)
        {
            if (Unit.CurrentTerrainIndex != TerrainTypeIndex)
            {
                //return ListConquestTerrainType[TerrainTypeIndex].GetEntryCost(Unit, Stats);
            }

            return TerrainHolder.ListConquestTerrainType[TerrainTypeIndex].DicMovementCostByMoveType[Stats.UnitTypeIndex];
        }

        public MovementAlgorithmTile GetMovementTile(int X, int Y, int LayerIndex)
        {
            if (X < 0 || Y >= MapSize.X || Y < 0 || Y >= MapSize.Y || LayerIndex < 0 || LayerIndex >= LayerManager.ListLayer.Count)
            {
                return null;
            }

            return LayerManager.ListLayer[LayerIndex].ArrayTerrain[X, Y];
        }

        public void ReplaceTile(int X, int Y, int LayerIndex, DrawableTile ActiveTile)
        {
            DrawableTile NewTile = new DrawableTile(ActiveTile);

            LayerManager.ListLayer[LayerIndex].ArrayTile[X, Y] = NewTile;
            LayerManager.LayerHolderDrawable.Reset();
        }

        public override List<Vector3> GetCampaignEnemySpawnLocations()
        {
            List<Vector3> ListPossibleSpawnPoint = new List<Vector3>();

            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                ListPossibleSpawnPoint.AddRange(ActivePlatform.Map.GetCampaignEnemySpawnLocations());
            }

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = LayerManager.ListLayer[L];
                for (int S = 0; S < ActiveLayer.ListCampaignSpawns.Count; S++)
                {
                    if (ActiveLayer.ListCampaignSpawns[S].Tag == "E")
                    {
                        ListPossibleSpawnPoint.Add(ActiveLayer.ListCampaignSpawns[S].Position);
                    }
                }
            }

            return ListPossibleSpawnPoint;
        }

        public override List<Vector3> GetMultiplayerSpawnLocations(int Team)
        {
            List<Vector3> ListPossibleSpawnPoint = new List<Vector3>();

            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                ListPossibleSpawnPoint.AddRange(ActivePlatform.Map.GetMultiplayerSpawnLocations(Team));
            }

            string PlayerTag = (Team + 1).ToString();
            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = LayerManager.ListLayer[L];
                for (int S = 0; S < ActiveLayer.ListMultiplayerSpawns.Count; S++)
                {
                    if (ActiveLayer.ListMultiplayerSpawns[S].Tag == PlayerTag)
                    {
                        ListPossibleSpawnPoint.Add(ActiveLayer.ListMultiplayerSpawns[S].Position);
                    }
                }
            }

            return ListPossibleSpawnPoint;
        }

        public override BattleMap LoadTemporaryMap(BinaryReader BR)
        {
            throw new NotImplementedException();
        }

        public override void SaveTemporaryMap()
        {
            throw new NotImplementedException();
        }

        public override GameScreen GetMultiplayerScreen()
        {
            throw new NotImplementedException();
        }

        public void CenterCamera()
        {
            if (ActiveUnit == null)
                return;

            if (ActiveUnit.X < Camera2DPosition.X || ActiveUnit.Y < Camera2DPosition.Y ||
                ActiveUnit.X >= Camera2DPosition.X + ScreenSize.X || ActiveUnit.Y >= Camera2DPosition.Y + ScreenSize.Y)
            {
                PushScreen(new CenterOnSquadCutscene(null, this, ActiveUnit.Position));
            }
        }

        public override BattleMap GetNewMap(GameModeInfo GameInfo, string ParamsID)
        {
            ConquestParams Params;
            ConquestMap NewMap;

            if (!ConquestParams.DicParams.TryGetValue(ParamsID, out Params))
            {
                Params = new ConquestParams();
                Params.ID = ParamsID;
                ConquestParams.DicParams.TryAdd(ParamsID, Params);
                Params.Reload(this.Params, ParamsID);
            }

            NewMap = new ConquestMap(GameInfo, Params);
            Params.Map = NewMap;
            return NewMap;
        }

        public override string GetMapType()
        {
            return MapType;
        }

        public override Dictionary<string, GameModeInfo> GetAvailableGameModes()
        {
            Dictionary<string, GameModeInfo> DicGameType = new Dictionary<string, GameModeInfo>();

            return DicGameType;
        }

        public void SpawnUnit(int PlayerIndex, UnitConquest NewUnit, uint ID, Vector3 Position)
        {
            Position = Position + new Vector3(TileSize.X / 2, TileSize.Y / 2, 0);

            NewUnit.InitStat();

            while (ListPlayer.Count <= PlayerIndex)
            {
                Player NewPlayer = new Player("Enemy", "CPU", false, false, PlayerIndex, Color.Red);
                ListPlayer.Add(NewPlayer);
            }
            if (Content != null)
            {
                NewUnit.Unit3DSprite = new UnitMap3D(GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), NewUnit.SpriteMap, 1);
                Color OutlineColor = ListPlayer[PlayerIndex].Color;
                NewUnit.Unit3DSprite.UnitEffect3D.Parameters["OutlineColor"].SetValue(new Vector4(OutlineColor.R / 255f, OutlineColor.G / 255f, OutlineColor.B / 255f, 1));
                NewUnit.Unit3DSprite.UnitEffect3D.Parameters["World"].SetValue(_World);
            }

            ListPlayer[PlayerIndex].IsAlive = true;

            GlobalBattleParams.GlobalContext.SetContext(null, NewUnit, null, null, null, null, null);
            NewUnit.Init();
            NewUnit.StartTurn();
            ActivateAutomaticSkills(null, NewUnit, string.Empty, null, NewUnit);
            NewUnit.SpawnID = ID;
            NewUnit.SetPosition(Position);

            ListPlayer[PlayerIndex].ListUnit.Add(NewUnit);

            NewUnit.Components.CurrentTerrainIndex = UnitStats.TerrainLandIndex;
        }

        public void SpawnBuilding(int PlayerIndex, BuildingConquest NewBuilding, uint ID, Vector3 Position)
        {
            Position = Position + new Vector3(TileSize.X / 2, TileSize.Y / 2, 0);

            NewBuilding.InitStats();

            while (ListPlayer.Count <= PlayerIndex)
            {
                Player NewPlayer = new Player("Enemy", "CPU", false, false, PlayerIndex, Color.Red);
                ListPlayer.Add(NewPlayer);
            }
            if (Content != null)
            {
                NewBuilding.Unit3DSprite = new UnitMap3D(GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), NewBuilding.SpriteMap.ActiveSprite, 1);
                Color OutlineColor = ListPlayer[PlayerIndex].Color;
                NewBuilding.Unit3DSprite.UnitEffect3D.Parameters["OutlineColor"].SetValue(new Vector4(OutlineColor.R / 255f, OutlineColor.G / 255f, OutlineColor.B / 255f, 1));
                NewBuilding.Unit3DSprite.UnitEffect3D.Parameters["World"].SetValue(_World);
            }

            ListPlayer[PlayerIndex].IsAlive = true;

            NewBuilding.SpawnID = ID;
            NewBuilding.SetPosition(Position);

            if (NewBuilding.RelativePath.StartsWith("Red"))
            {
                NewBuilding.CapturedTeamIndex = 0;
            }

            ListBuilding.Add(NewBuilding);
        }

        public uint GetNextUnusedUnitID()
        {
            uint NewUnitID = 0;
            bool SameIDFound = false;
            do
            {
                SameIDFound = false;
                for (int P = 0; P < ListPlayer.Count; ++P)
                {
                    for (int S = 0; S < ListPlayer[P].ListUnit.Count; S++)
                    {
                        if (ListPlayer[P].ListUnit[S].SpawnID == NewUnitID)
                        {
                            NewUnitID++;
                            SameIDFound = true;
                        }
                    }
                }
            }
            while (SameIDFound);

            return NewUnitID;
        }

        public override byte[] GetSnapshotData()
        {
            return new byte[0];
        }

        public override void RemoveOnlinePlayer(string PlayerID, IOnlineConnection ActivePlayer)
        {

        }

        public override Dictionary<string, ActionPanel> GetOnlineActionPanel()
        {
            Dictionary<string, ActionPanel> DicActionPanel = new Dictionary<string, ActionPanel>();

            Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath("Mods/Project Eternity Conquest Map.dll"));
            Dictionary<string, BattleMapActionPanel> DicActionPanelMap = BattleMapActionPanel.LoadFromAssembly(ActiveAssembly, typeof(ActionPanelConquest), this);
            foreach (KeyValuePair<string, BattleMapActionPanel> ActiveRequirement in DicActionPanelMap)
            {
                DicActionPanel.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            return DicActionPanel;
        }

        public void AddProjectile(Projectile3D NewProjectile)
        {
            throw new NotImplementedException();
        }
    }
}
