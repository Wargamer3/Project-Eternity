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

            MapName = Path.GetFileNameWithoutExtension(BattleMapPath);
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

            LoadProperties(BR);

            ListMapScript = MapScript.LoadMapScripts(BR, DicMapEvent, DicMapCondition, DicMapTrigger, out ListMapEvent);

            LoadTilesets(BR);

            LoadTemporaryTerrain(BR);

            LayerManager = new LayerHolderConquest(this, BR);

            MapEnvironment = new EnvironmentManagerConquest(BR, this);

            BR.Close();
            FS.Close();
        }

        protected override TilesetPreset ReadTileset(string TilesetPresetPath, bool IsAutotile, int Index)
        {
            if (IsAutotile)
            {
                return TilesetPreset.FromFile("Conquest/Autotiles Presets/" + TilesetPresetPath + ".peat", TilesetPresetPath, Index);
            }
            else
            {
                return TilesetPreset.FromFile("Conquest/Tilesets Presets/" + TilesetPresetPath + ".pet", TilesetPresetPath, Index);
            }
        }

        protected override DestructibleTilesetPreset ReadDestructibleTilesetPreset(string TilesetPresetPath, int Index)
        {
            return ConquestDestructibleTilesetPreset.FromFile(this, "Conquest/Destroyable Tiles Presets/" + TilesetPresetPath + ".pedt", TilesetPresetPath, Index);
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
                ReplacementTerrain.Owner = this;
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
            DicUnitDamageWeapon1.Add("APC", new Dictionary<string, int>());
            DicUnitDamageWeapon1["APC"].Add("APC", 0);
            DicUnitDamageWeapon1["APC"].Add("Anti-Air", 0);
            DicUnitDamageWeapon1["APC"].Add("Artillery", 0);
            DicUnitDamageWeapon1["APC"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["APC"].Add("Battleship", 0);
            DicUnitDamageWeapon1["APC"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["APC"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["APC"].Add("Bomber", 0);
            DicUnitDamageWeapon1["APC"].Add("Carrier", 0);
            DicUnitDamageWeapon1["APC"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["APC"].Add("Fighter", 0);
            DicUnitDamageWeapon1["APC"].Add("Infantry", 0);
            DicUnitDamageWeapon1["APC"].Add("Lander", 0);
            DicUnitDamageWeapon1["APC"].Add("Mech", 0);
            DicUnitDamageWeapon1["APC"].Add("Medium Tank", 0);
            DicUnitDamageWeapon1["APC"].Add("Mega Tank", 0);
            DicUnitDamageWeapon1["APC"].Add("Missile", 0);
            DicUnitDamageWeapon1["APC"].Add("Neotank", 0);
            DicUnitDamageWeapon1["APC"].Add("Piperunner", 0);
            DicUnitDamageWeapon1["APC"].Add("Recon", 0);
            DicUnitDamageWeapon1["APC"].Add("Rocket", 0);
            DicUnitDamageWeapon1["APC"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["APC"].Add("Stealth", 0);
            DicUnitDamageWeapon1["APC"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["APC"].Add("Sub", 0);
            DicUnitDamageWeapon1["APC"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["APC"].Add("Tank", 0);
            DicUnitDamageWeapon1.Add("Anti-Air", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Anti-Air"].Add("APC", 50);
            DicUnitDamageWeapon1["Anti-Air"].Add("Anti-Air", 45);
            DicUnitDamageWeapon1["Anti-Air"].Add("Artillery", 50);
            DicUnitDamageWeapon1["Anti-Air"].Add("B-Copter", 120);
            DicUnitDamageWeapon1["Anti-Air"].Add("Battleship", 0);
            DicUnitDamageWeapon1["Anti-Air"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["Anti-Air"].Add("Black Bomb", 120);
            DicUnitDamageWeapon1["Anti-Air"].Add("Bomber", 75);
            DicUnitDamageWeapon1["Anti-Air"].Add("Carrier", 0);
            DicUnitDamageWeapon1["Anti-Air"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["Anti-Air"].Add("Fighter", 65);
            DicUnitDamageWeapon1["Anti-Air"].Add("Infantry", 105);
            DicUnitDamageWeapon1["Anti-Air"].Add("Lander", 0);
            DicUnitDamageWeapon1["Anti-Air"].Add("Mech", 105);
            DicUnitDamageWeapon1["Anti-Air"].Add("Medium Tank", 10);
            DicUnitDamageWeapon1["Anti-Air"].Add("Mega Tank", 1);
            DicUnitDamageWeapon1["Anti-Air"].Add("Missile", 55);
            DicUnitDamageWeapon1["Anti-Air"].Add("Neotank", 5);
            DicUnitDamageWeapon1["Anti-Air"].Add("Piperunner", 25);
            DicUnitDamageWeapon1["Anti-Air"].Add("Recon", 60);
            DicUnitDamageWeapon1["Anti-Air"].Add("Rocket", 55);
            DicUnitDamageWeapon1["Anti-Air"].Add("Stealth - Hidden", 75);
            DicUnitDamageWeapon1["Anti-Air"].Add("Stealth", 75);
            DicUnitDamageWeapon1["Anti-Air"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["Anti-Air"].Add("Sub", 0);
            DicUnitDamageWeapon1["Anti-Air"].Add("T-Copter", 120);
            DicUnitDamageWeapon1["Anti-Air"].Add("Tank", 25);
            DicUnitDamageWeapon1.Add("Artillery", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Artillery"].Add("APC", 70);
            DicUnitDamageWeapon1["Artillery"].Add("Anti-Air", 75);
            DicUnitDamageWeapon1["Artillery"].Add("Artillery", 75);
            DicUnitDamageWeapon1["Artillery"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Artillery"].Add("Battleship", 40);
            DicUnitDamageWeapon1["Artillery"].Add("Black Boat", 55);
            DicUnitDamageWeapon1["Artillery"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Artillery"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Artillery"].Add("Carrier", 45);
            DicUnitDamageWeapon1["Artillery"].Add("Cruiser", 65);
            DicUnitDamageWeapon1["Artillery"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Artillery"].Add("Infantry", 90);
            DicUnitDamageWeapon1["Artillery"].Add("Lander", 55);
            DicUnitDamageWeapon1["Artillery"].Add("Mech", 85);
            DicUnitDamageWeapon1["Artillery"].Add("Medium Tank", 45);
            DicUnitDamageWeapon1["Artillery"].Add("Mega Tank", 15);
            DicUnitDamageWeapon1["Artillery"].Add("Missile", 80);
            DicUnitDamageWeapon1["Artillery"].Add("Neotank", 40);
            DicUnitDamageWeapon1["Artillery"].Add("Piperunner", 70);
            DicUnitDamageWeapon1["Artillery"].Add("Recon", 80);
            DicUnitDamageWeapon1["Artillery"].Add("Rocket", 80);
            DicUnitDamageWeapon1["Artillery"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Artillery"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Artillery"].Add("Sub - Submerged", 60);
            DicUnitDamageWeapon1["Artillery"].Add("Sub", 60);
            DicUnitDamageWeapon1["Artillery"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Artillery"].Add("Tank", 70);
            DicUnitDamageWeapon1.Add("B-Copter", new Dictionary<string, int>());
            DicUnitDamageWeapon1["B-Copter"].Add("APC", 60);
            DicUnitDamageWeapon1["B-Copter"].Add("Anti-Air", 25);
            DicUnitDamageWeapon1["B-Copter"].Add("Artillery", 65);
            DicUnitDamageWeapon1["B-Copter"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["B-Copter"].Add("Battleship", 25);
            DicUnitDamageWeapon1["B-Copter"].Add("Black Boat", 25);
            DicUnitDamageWeapon1["B-Copter"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["B-Copter"].Add("Bomber", 0);
            DicUnitDamageWeapon1["B-Copter"].Add("Carrier", 25);
            DicUnitDamageWeapon1["B-Copter"].Add("Cruiser", 55);
            DicUnitDamageWeapon1["B-Copter"].Add("Fighter", 0);
            DicUnitDamageWeapon1["B-Copter"].Add("Infantry", 0);
            DicUnitDamageWeapon1["B-Copter"].Add("Lander", 25);
            DicUnitDamageWeapon1["B-Copter"].Add("Mech", 0);
            DicUnitDamageWeapon1["B-Copter"].Add("Medium Tank", 25);
            DicUnitDamageWeapon1["B-Copter"].Add("Mega Tank", 10);
            DicUnitDamageWeapon1["B-Copter"].Add("Missile", 65);
            DicUnitDamageWeapon1["B-Copter"].Add("Neotank", 20);
            DicUnitDamageWeapon1["B-Copter"].Add("Piperunner", 55);
            DicUnitDamageWeapon1["B-Copter"].Add("Recon", 55);
            DicUnitDamageWeapon1["B-Copter"].Add("Rocket", 65);
            DicUnitDamageWeapon1["B-Copter"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["B-Copter"].Add("Stealth", 0);
            DicUnitDamageWeapon1["B-Copter"].Add("Sub - Submerged", 25);
            DicUnitDamageWeapon1["B-Copter"].Add("Sub", 25);
            DicUnitDamageWeapon1["B-Copter"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["B-Copter"].Add("Tank", 55);
            DicUnitDamageWeapon1.Add("Battleship", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Battleship"].Add("APC", 80);
            DicUnitDamageWeapon1["Battleship"].Add("Anti-Air", 85);
            DicUnitDamageWeapon1["Battleship"].Add("Artillery", 80);
            DicUnitDamageWeapon1["Battleship"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Battleship"].Add("Battleship", 50);
            DicUnitDamageWeapon1["Battleship"].Add("Black Boat", 95);
            DicUnitDamageWeapon1["Battleship"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Battleship"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Battleship"].Add("Carrier", 60);
            DicUnitDamageWeapon1["Battleship"].Add("Cruiser", 95);
            DicUnitDamageWeapon1["Battleship"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Battleship"].Add("Infantry", 95);
            DicUnitDamageWeapon1["Battleship"].Add("Lander", 95);
            DicUnitDamageWeapon1["Battleship"].Add("Mech", 90);
            DicUnitDamageWeapon1["Battleship"].Add("Medium Tank", 55);
            DicUnitDamageWeapon1["Battleship"].Add("Mega Tank", 25);
            DicUnitDamageWeapon1["Battleship"].Add("Missile", 90);
            DicUnitDamageWeapon1["Battleship"].Add("Neotank", 50);
            DicUnitDamageWeapon1["Battleship"].Add("Piperunner", 80);
            DicUnitDamageWeapon1["Battleship"].Add("Recon", 90);
            DicUnitDamageWeapon1["Battleship"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Battleship"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Battleship"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Battleship"].Add("Sub - Submerged", 95);
            DicUnitDamageWeapon1["Battleship"].Add("Sub", 95);
            DicUnitDamageWeapon1["Battleship"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Battleship"].Add("Tank", 80);
            DicUnitDamageWeapon1.Add("Black Boat", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Black Boat"].Add("APC", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Anti-Air", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Artillery", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Battleship", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Carrier", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Lander", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Mech", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Medium Tank", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Mega Tank", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Missile", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Neotank", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Piperunner", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Recon", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Rocket", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Sub", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Black Boat"].Add("Tank", 0);
            DicUnitDamageWeapon1.Add("Black Bomb", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Black Bomb"].Add("APC", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Anti-Air", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Artillery", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Battleship", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Carrier", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Lander", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Mech", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Medium Tank", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Mega Tank", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Missile", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Neotank", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Piperunner", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Recon", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Rocket", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Sub", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Black Bomb"].Add("Tank", 0);
            DicUnitDamageWeapon1.Add("Bomber", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Bomber"].Add("APC", 105);
            DicUnitDamageWeapon1["Bomber"].Add("Anti-Air", 95);
            DicUnitDamageWeapon1["Bomber"].Add("Artillery", 105);
            DicUnitDamageWeapon1["Bomber"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Bomber"].Add("Battleship", 75);
            DicUnitDamageWeapon1["Bomber"].Add("Black Boat", 95);
            DicUnitDamageWeapon1["Bomber"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Bomber"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Bomber"].Add("Carrier", 75);
            DicUnitDamageWeapon1["Bomber"].Add("Cruiser", 85);
            DicUnitDamageWeapon1["Bomber"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Bomber"].Add("Infantry", 110);
            DicUnitDamageWeapon1["Bomber"].Add("Lander", 95);
            DicUnitDamageWeapon1["Bomber"].Add("Mech", 110);
            DicUnitDamageWeapon1["Bomber"].Add("Medium Tank", 95);
            DicUnitDamageWeapon1["Bomber"].Add("Mega Tank", 35);
            DicUnitDamageWeapon1["Bomber"].Add("Missile", 105);
            DicUnitDamageWeapon1["Bomber"].Add("Neotank", 90);
            DicUnitDamageWeapon1["Bomber"].Add("Piperunner", 105);
            DicUnitDamageWeapon1["Bomber"].Add("Recon", 105);
            DicUnitDamageWeapon1["Bomber"].Add("Rocket", 105);
            DicUnitDamageWeapon1["Bomber"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Bomber"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Bomber"].Add("Sub - Submerged", 95);
            DicUnitDamageWeapon1["Bomber"].Add("Sub", 95);
            DicUnitDamageWeapon1["Bomber"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Bomber"].Add("Tank", 105);
            DicUnitDamageWeapon1.Add("Carrier", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Carrier"].Add("APC", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Anti-Air", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Artillery", 0);
            DicUnitDamageWeapon1["Carrier"].Add("B-Copter", 115);
            DicUnitDamageWeapon1["Carrier"].Add("Battleship", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Black Bomb", 120);
            DicUnitDamageWeapon1["Carrier"].Add("Bomber", 100);
            DicUnitDamageWeapon1["Carrier"].Add("Carrier", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Fighter", 100);
            DicUnitDamageWeapon1["Carrier"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Lander", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Mech", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Medium Tank", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Mega Tank", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Missile", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Neotank", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Piperunner", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Recon", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Rocket", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Stealth - Hidden", 100);
            DicUnitDamageWeapon1["Carrier"].Add("Stealth", 100);
            DicUnitDamageWeapon1["Carrier"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["Carrier"].Add("Sub", 0);
            DicUnitDamageWeapon1["Carrier"].Add("T-Copter", 115);
            DicUnitDamageWeapon1["Carrier"].Add("Tank", 0);
            DicUnitDamageWeapon1.Add("Cruiser", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Cruiser"].Add("APC", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Anti-Air", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Artillery", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Battleship", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Black Boat", 25);
            DicUnitDamageWeapon1["Cruiser"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Carrier", 5);
            DicUnitDamageWeapon1["Cruiser"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Lander", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Mech", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Medium Tank", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Mega Tank", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Missile", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Neotank", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Piperunner", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Recon", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Rocket", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Sub - Submerged", 90);
            DicUnitDamageWeapon1["Cruiser"].Add("Sub", 90);
            DicUnitDamageWeapon1["Cruiser"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Cruiser"].Add("Tank", 0);
            DicUnitDamageWeapon1.Add("Fighter", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Fighter"].Add("APC", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Anti-Air", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Artillery", 0);
            DicUnitDamageWeapon1["Fighter"].Add("B-Copter", 100);
            DicUnitDamageWeapon1["Fighter"].Add("Battleship", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Black Bomb", 120);
            DicUnitDamageWeapon1["Fighter"].Add("Bomber", 100);
            DicUnitDamageWeapon1["Fighter"].Add("Carrier", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Fighter", 55);
            DicUnitDamageWeapon1["Fighter"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Lander", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Mech", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Medium Tank", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Mega Tank", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Missile", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Neotank", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Piperunner", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Recon", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Rocket", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Stealth - Hidden", 85);
            DicUnitDamageWeapon1["Fighter"].Add("Stealth", 85);
            DicUnitDamageWeapon1["Fighter"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["Fighter"].Add("Sub", 0);
            DicUnitDamageWeapon1["Fighter"].Add("T-Copter", 100);
            DicUnitDamageWeapon1["Fighter"].Add("Tank", 0);
            DicUnitDamageWeapon1.Add("Infantry", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Infantry"].Add("APC", 12);
            DicUnitDamageWeapon1["Infantry"].Add("Anti-Air", 5);
            DicUnitDamageWeapon1["Infantry"].Add("Artillery", 15);
            DicUnitDamageWeapon1["Infantry"].Add("B-Copter", 7);
            DicUnitDamageWeapon1["Infantry"].Add("Battleship", 0);
            DicUnitDamageWeapon1["Infantry"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["Infantry"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Infantry"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Infantry"].Add("Carrier", 0);
            DicUnitDamageWeapon1["Infantry"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["Infantry"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Infantry"].Add("Infantry", 55);
            DicUnitDamageWeapon1["Infantry"].Add("Lander", 0);
            DicUnitDamageWeapon1["Infantry"].Add("Mech", 45);
            DicUnitDamageWeapon1["Infantry"].Add("Medium Tank", 1);
            DicUnitDamageWeapon1["Infantry"].Add("Mega Tank", 1);
            DicUnitDamageWeapon1["Infantry"].Add("Missile", 25);
            DicUnitDamageWeapon1["Infantry"].Add("Neotank", 1);
            DicUnitDamageWeapon1["Infantry"].Add("Piperunner", 5);
            DicUnitDamageWeapon1["Infantry"].Add("Recon", 12);
            DicUnitDamageWeapon1["Infantry"].Add("Rocket", 25);
            DicUnitDamageWeapon1["Infantry"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Infantry"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Infantry"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["Infantry"].Add("Sub", 0);
            DicUnitDamageWeapon1["Infantry"].Add("T-Copter", 30);
            DicUnitDamageWeapon1["Infantry"].Add("Tank", 5);
            DicUnitDamageWeapon1.Add("Mech", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Mech"].Add("APC", 75);
            DicUnitDamageWeapon1["Mech"].Add("Anti-Air", 65);
            DicUnitDamageWeapon1["Mech"].Add("Artillery", 70);
            DicUnitDamageWeapon1["Mech"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Mech"].Add("Battleship", 0);
            DicUnitDamageWeapon1["Mech"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["Mech"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Mech"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Mech"].Add("Carrier", 0);
            DicUnitDamageWeapon1["Mech"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["Mech"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Mech"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Mech"].Add("Lander", 0);
            DicUnitDamageWeapon1["Mech"].Add("Mech", 0);
            DicUnitDamageWeapon1["Mech"].Add("Medium Tank", 15);
            DicUnitDamageWeapon1["Mech"].Add("Mega Tank", 5);
            DicUnitDamageWeapon1["Mech"].Add("Missile", 85);
            DicUnitDamageWeapon1["Mech"].Add("Neotank", 15);
            DicUnitDamageWeapon1["Mech"].Add("Piperunner", 55);
            DicUnitDamageWeapon1["Mech"].Add("Recon", 85);
            DicUnitDamageWeapon1["Mech"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Mech"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Mech"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Mech"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["Mech"].Add("Sub", 0);
            DicUnitDamageWeapon1["Mech"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Mech"].Add("Tank", 55);
            DicUnitDamageWeapon1.Add("Medium Tank", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Medium Tank"].Add("APC", 105);
            DicUnitDamageWeapon1["Medium Tank"].Add("Anti-Air", 105);
            DicUnitDamageWeapon1["Medium Tank"].Add("Artillery", 105);
            DicUnitDamageWeapon1["Medium Tank"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Medium Tank"].Add("Battleship", 10);
            DicUnitDamageWeapon1["Medium Tank"].Add("Black Boat", 35);
            DicUnitDamageWeapon1["Medium Tank"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Medium Tank"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Medium Tank"].Add("Carrier", 10);
            DicUnitDamageWeapon1["Medium Tank"].Add("Cruiser", 45);
            DicUnitDamageWeapon1["Medium Tank"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Medium Tank"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Medium Tank"].Add("Lander", 35);
            DicUnitDamageWeapon1["Medium Tank"].Add("Mech", 0);
            DicUnitDamageWeapon1["Medium Tank"].Add("Medium Tank", 55);
            DicUnitDamageWeapon1["Medium Tank"].Add("Mega Tank", 25);
            DicUnitDamageWeapon1["Medium Tank"].Add("Missile", 105);
            DicUnitDamageWeapon1["Medium Tank"].Add("Neotank", 45);
            DicUnitDamageWeapon1["Medium Tank"].Add("Piperunner", 85);
            DicUnitDamageWeapon1["Medium Tank"].Add("Recon", 105);
            DicUnitDamageWeapon1["Medium Tank"].Add("Rocket", 105);
            DicUnitDamageWeapon1["Medium Tank"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Medium Tank"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Medium Tank"].Add("Sub - Submerged", 10);
            DicUnitDamageWeapon1["Medium Tank"].Add("Sub", 10);
            DicUnitDamageWeapon1["Medium Tank"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Medium Tank"].Add("Tank", 85);
            DicUnitDamageWeapon1.Add("Mega Tank", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Mega Tank"].Add("APC", 195);
            DicUnitDamageWeapon1["Mega Tank"].Add("Anti-Air", 195);
            DicUnitDamageWeapon1["Mega Tank"].Add("Artillery", 195);
            DicUnitDamageWeapon1["Mega Tank"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Mega Tank"].Add("Battleship", 45);
            DicUnitDamageWeapon1["Mega Tank"].Add("Black Boat", 105);
            DicUnitDamageWeapon1["Mega Tank"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Mega Tank"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Mega Tank"].Add("Carrier", 45);
            DicUnitDamageWeapon1["Mega Tank"].Add("Cruiser", 65);
            DicUnitDamageWeapon1["Mega Tank"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Mega Tank"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Mega Tank"].Add("Lander", 75);
            DicUnitDamageWeapon1["Mega Tank"].Add("Mech", 0);
            DicUnitDamageWeapon1["Mega Tank"].Add("Medium Tank", 125);
            DicUnitDamageWeapon1["Mega Tank"].Add("Mega Tank", 65);
            DicUnitDamageWeapon1["Mega Tank"].Add("Missile", 195);
            DicUnitDamageWeapon1["Mega Tank"].Add("Neotank", 115);
            DicUnitDamageWeapon1["Mega Tank"].Add("Piperunner", 180);
            DicUnitDamageWeapon1["Mega Tank"].Add("Recon", 195);
            DicUnitDamageWeapon1["Mega Tank"].Add("Rocket", 195);
            DicUnitDamageWeapon1["Mega Tank"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Mega Tank"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Mega Tank"].Add("Sub - Submerged", 45);
            DicUnitDamageWeapon1["Mega Tank"].Add("Sub", 45);
            DicUnitDamageWeapon1["Mega Tank"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Mega Tank"].Add("Tank", 180);
            DicUnitDamageWeapon1.Add("Missile", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Missile"].Add("APC", 0);
            DicUnitDamageWeapon1["Missile"].Add("Anti-Air", 0);
            DicUnitDamageWeapon1["Missile"].Add("Artillery", 0);
            DicUnitDamageWeapon1["Missile"].Add("B-Copter", 120);
            DicUnitDamageWeapon1["Missile"].Add("Battleship", 0);
            DicUnitDamageWeapon1["Missile"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["Missile"].Add("Black Bomb", 120);
            DicUnitDamageWeapon1["Missile"].Add("Bomber", 100);
            DicUnitDamageWeapon1["Missile"].Add("Carrier", 0);
            DicUnitDamageWeapon1["Missile"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["Missile"].Add("Fighter", 100);
            DicUnitDamageWeapon1["Missile"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Missile"].Add("Lander", 0);
            DicUnitDamageWeapon1["Missile"].Add("Mech", 0);
            DicUnitDamageWeapon1["Missile"].Add("Medium Tank", 0);
            DicUnitDamageWeapon1["Missile"].Add("Mega Tank", 0);
            DicUnitDamageWeapon1["Missile"].Add("Missile", 0);
            DicUnitDamageWeapon1["Missile"].Add("Neotank", 0);
            DicUnitDamageWeapon1["Missile"].Add("Piperunner", 0);
            DicUnitDamageWeapon1["Missile"].Add("Recon", 0);
            DicUnitDamageWeapon1["Missile"].Add("Rocket", 0);
            DicUnitDamageWeapon1["Missile"].Add("Stealth - Hidden", 100);
            DicUnitDamageWeapon1["Missile"].Add("Stealth", 100);
            DicUnitDamageWeapon1["Missile"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["Missile"].Add("Sub", 0);
            DicUnitDamageWeapon1["Missile"].Add("T-Copter", 120);
            DicUnitDamageWeapon1["Missile"].Add("Tank", 0);
            DicUnitDamageWeapon1.Add("Neotank", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Neotank"].Add("APC", 125);
            DicUnitDamageWeapon1["Neotank"].Add("Anti-Air", 115);
            DicUnitDamageWeapon1["Neotank"].Add("Artillery", 115);
            DicUnitDamageWeapon1["Neotank"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Neotank"].Add("Battleship", 15);
            DicUnitDamageWeapon1["Neotank"].Add("Black Boat", 40);
            DicUnitDamageWeapon1["Neotank"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Neotank"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Neotank"].Add("Carrier", 15);
            DicUnitDamageWeapon1["Neotank"].Add("Cruiser", 50);
            DicUnitDamageWeapon1["Neotank"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Neotank"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Neotank"].Add("Lander", 40);
            DicUnitDamageWeapon1["Neotank"].Add("Mech", 0);
            DicUnitDamageWeapon1["Neotank"].Add("Medium Tank", 75);
            DicUnitDamageWeapon1["Neotank"].Add("Mega Tank", 35);
            DicUnitDamageWeapon1["Neotank"].Add("Missile", 125);
            DicUnitDamageWeapon1["Neotank"].Add("Neotank", 55);
            DicUnitDamageWeapon1["Neotank"].Add("Piperunner", 105);
            DicUnitDamageWeapon1["Neotank"].Add("Recon", 125);
            DicUnitDamageWeapon1["Neotank"].Add("Rocket", 125);
            DicUnitDamageWeapon1["Neotank"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Neotank"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Neotank"].Add("Sub - Submerged", 15);
            DicUnitDamageWeapon1["Neotank"].Add("Sub", 15);
            DicUnitDamageWeapon1["Neotank"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Neotank"].Add("Tank", 105);
            DicUnitDamageWeapon1.Add("Piperunner", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Piperunner"].Add("APC", 80);
            DicUnitDamageWeapon1["Piperunner"].Add("Anti-Air", 85);
            DicUnitDamageWeapon1["Piperunner"].Add("Artillery", 80);
            DicUnitDamageWeapon1["Piperunner"].Add("B-Copter", 105);
            DicUnitDamageWeapon1["Piperunner"].Add("Battleship", 55);
            DicUnitDamageWeapon1["Piperunner"].Add("Black Boat", 60);
            DicUnitDamageWeapon1["Piperunner"].Add("Black Bomb", 120);
            DicUnitDamageWeapon1["Piperunner"].Add("Bomber", 75);
            DicUnitDamageWeapon1["Piperunner"].Add("Carrier", 60);
            DicUnitDamageWeapon1["Piperunner"].Add("Cruiser", 60);
            DicUnitDamageWeapon1["Piperunner"].Add("Fighter", 65);
            DicUnitDamageWeapon1["Piperunner"].Add("Infantry", 95);
            DicUnitDamageWeapon1["Piperunner"].Add("Lander", 60);
            DicUnitDamageWeapon1["Piperunner"].Add("Mech", 90);
            DicUnitDamageWeapon1["Piperunner"].Add("Medium Tank", 55);
            DicUnitDamageWeapon1["Piperunner"].Add("Mega Tank", 25);
            DicUnitDamageWeapon1["Piperunner"].Add("Missile", 90);
            DicUnitDamageWeapon1["Piperunner"].Add("Neotank", 50);
            DicUnitDamageWeapon1["Piperunner"].Add("Piperunner", 80);
            DicUnitDamageWeapon1["Piperunner"].Add("Recon", 90);
            DicUnitDamageWeapon1["Piperunner"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Piperunner"].Add("Stealth - Hidden", 75);
            DicUnitDamageWeapon1["Piperunner"].Add("Stealth", 75);
            DicUnitDamageWeapon1["Piperunner"].Add("Sub - Submerged", 85);
            DicUnitDamageWeapon1["Piperunner"].Add("Sub", 85);
            DicUnitDamageWeapon1["Piperunner"].Add("T-Copter", 105);
            DicUnitDamageWeapon1["Piperunner"].Add("Tank", 80);
            DicUnitDamageWeapon1.Add("Recon", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Recon"].Add("APC", 45);
            DicUnitDamageWeapon1["Recon"].Add("Anti-Air", 4);
            DicUnitDamageWeapon1["Recon"].Add("Artillery", 45);
            DicUnitDamageWeapon1["Recon"].Add("B-Copter", 10);
            DicUnitDamageWeapon1["Recon"].Add("Battleship", 0);
            DicUnitDamageWeapon1["Recon"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["Recon"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Recon"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Recon"].Add("Carrier", 0);
            DicUnitDamageWeapon1["Recon"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["Recon"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Recon"].Add("Infantry", 70);
            DicUnitDamageWeapon1["Recon"].Add("Lander", 0);
            DicUnitDamageWeapon1["Recon"].Add("Mech", 65);
            DicUnitDamageWeapon1["Recon"].Add("Medium Tank", 1);
            DicUnitDamageWeapon1["Recon"].Add("Mega Tank", 1);
            DicUnitDamageWeapon1["Recon"].Add("Missile", 28);
            DicUnitDamageWeapon1["Recon"].Add("Neotank", 1);
            DicUnitDamageWeapon1["Recon"].Add("Piperunner", 6);
            DicUnitDamageWeapon1["Recon"].Add("Recon", 35);
            DicUnitDamageWeapon1["Recon"].Add("Rocket", 55);
            DicUnitDamageWeapon1["Recon"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Recon"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Recon"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["Recon"].Add("Sub", 0);
            DicUnitDamageWeapon1["Recon"].Add("T-Copter", 35);
            DicUnitDamageWeapon1["Recon"].Add("Tank", 6);
            DicUnitDamageWeapon1.Add("Rocket", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Rocket"].Add("APC", 80);
            DicUnitDamageWeapon1["Rocket"].Add("Anti-Air", 85);
            DicUnitDamageWeapon1["Rocket"].Add("Artillery", 80);
            DicUnitDamageWeapon1["Rocket"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Rocket"].Add("Battleship", 55);
            DicUnitDamageWeapon1["Rocket"].Add("Black Boat", 60);
            DicUnitDamageWeapon1["Rocket"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Rocket"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Rocket"].Add("Carrier", 60);
            DicUnitDamageWeapon1["Rocket"].Add("Cruiser", 85);
            DicUnitDamageWeapon1["Rocket"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Rocket"].Add("Infantry", 95);
            DicUnitDamageWeapon1["Rocket"].Add("Lander", 60);
            DicUnitDamageWeapon1["Rocket"].Add("Mech", 90);
            DicUnitDamageWeapon1["Rocket"].Add("Medium Tank", 55);
            DicUnitDamageWeapon1["Rocket"].Add("Mega Tank", 25);
            DicUnitDamageWeapon1["Rocket"].Add("Missile", 90);
            DicUnitDamageWeapon1["Rocket"].Add("Neotank", 50);
            DicUnitDamageWeapon1["Rocket"].Add("Piperunner", 80);
            DicUnitDamageWeapon1["Rocket"].Add("Recon", 90);
            DicUnitDamageWeapon1["Rocket"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Rocket"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Rocket"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Rocket"].Add("Sub - Submerged", 85);
            DicUnitDamageWeapon1["Rocket"].Add("Sub", 85);
            DicUnitDamageWeapon1["Rocket"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Rocket"].Add("Tank", 80);
            DicUnitDamageWeapon1.Add("Stealth - Hidden", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("APC", 85);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Anti-Air", 50);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Artillery", 75);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("B-Copter", 85);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Battleship", 45);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Black Boat", 65);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Black Bomb", 120);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Bomber", 70);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Carrier", 45);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Cruiser", 35);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Fighter", 45);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Infantry", 90);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Lander", 65);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Mech", 90);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Medium Tank", 70);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Mega Tank", 15);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Missile", 85);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Neotank", 60);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Piperunner", 80);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Recon", 85);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Stealth - Hidden", 55);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Stealth", 55);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Sub - Submerged", 55);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Sub", 55);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("T-Copter", 95);
            DicUnitDamageWeapon1["Stealth - Hidden"].Add("Tank", 75);
            DicUnitDamageWeapon1.Add("Sub - Submerged", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Sub - Submerged"].Add("APC", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Anti-Air", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Artillery", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Battleship", 55);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Black Boat", 95);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Carrier", 75);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Cruiser", 25);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Lander", 95);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Mech", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Medium Tank", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Mega Tank", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Missile", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Neotank", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Piperunner", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Recon", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Rocket", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Sub - Submerged", 55);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Sub", 55);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Sub - Submerged"].Add("Tank", 0);
            DicUnitDamageWeapon1.Add("Sub", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Sub"].Add("APC", 0);
            DicUnitDamageWeapon1["Sub"].Add("Anti-Air", 0);
            DicUnitDamageWeapon1["Sub"].Add("Artillery", 0);
            DicUnitDamageWeapon1["Sub"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Sub"].Add("Battleship", 55);
            DicUnitDamageWeapon1["Sub"].Add("Black Boat", 95);
            DicUnitDamageWeapon1["Sub"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Sub"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Sub"].Add("Carrier", 75);
            DicUnitDamageWeapon1["Sub"].Add("Cruiser", 25);
            DicUnitDamageWeapon1["Sub"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Sub"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Sub"].Add("Lander", 95);
            DicUnitDamageWeapon1["Sub"].Add("Mech", 0);
            DicUnitDamageWeapon1["Sub"].Add("Medium Tank", 0);
            DicUnitDamageWeapon1["Sub"].Add("Mega Tank", 0);
            DicUnitDamageWeapon1["Sub"].Add("Missile", 0);
            DicUnitDamageWeapon1["Sub"].Add("Neotank", 0);
            DicUnitDamageWeapon1["Sub"].Add("Piperunner", 0);
            DicUnitDamageWeapon1["Sub"].Add("Recon", 0);
            DicUnitDamageWeapon1["Sub"].Add("Rocket", 0);
            DicUnitDamageWeapon1["Sub"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Sub"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Sub"].Add("Sub - Submerged", 55);
            DicUnitDamageWeapon1["Sub"].Add("Sub", 55);
            DicUnitDamageWeapon1["Sub"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Sub"].Add("Tank", 0);
            DicUnitDamageWeapon1.Add("T-Copter", new Dictionary<string, int>());
            DicUnitDamageWeapon1["T-Copter"].Add("APC", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Anti-Air", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Artillery", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Battleship", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Bomber", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Carrier", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Fighter", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Infantry", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Lander", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Mech", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Medium Tank", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Mega Tank", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Missile", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Neotank", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Piperunner", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Recon", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Rocket", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Stealth", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Sub", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["T-Copter"].Add("Tank", 0);
            DicUnitDamageWeapon1.Add("Tank", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Tank"].Add("APC", 75);
            DicUnitDamageWeapon1["Tank"].Add("Anti-Air", 65);
            DicUnitDamageWeapon1["Tank"].Add("Artillery", 70);
            DicUnitDamageWeapon1["Tank"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Tank"].Add("Battleship", 1);
            DicUnitDamageWeapon1["Tank"].Add("Black Boat", 10);
            DicUnitDamageWeapon1["Tank"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Tank"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Tank"].Add("Carrier", 1);
            DicUnitDamageWeapon1["Tank"].Add("Cruiser", 5);
            DicUnitDamageWeapon1["Tank"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Tank"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Tank"].Add("Lander", 10);
            DicUnitDamageWeapon1["Tank"].Add("Mech", 0);
            DicUnitDamageWeapon1["Tank"].Add("Medium Tank", 15);
            DicUnitDamageWeapon1["Tank"].Add("Mega Tank", 10);
            DicUnitDamageWeapon1["Tank"].Add("Missile", 85);
            DicUnitDamageWeapon1["Tank"].Add("Neotank", 15);
            DicUnitDamageWeapon1["Tank"].Add("Piperunner", 55);
            DicUnitDamageWeapon1["Tank"].Add("Recon", 85);
            DicUnitDamageWeapon1["Tank"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Tank"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Tank"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Tank"].Add("Sub - Submerged", 1);
            DicUnitDamageWeapon1["Tank"].Add("Sub", 1);
            DicUnitDamageWeapon1["Tank"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Tank"].Add("Tank", 55);
            DicUnitDamageWeapon1.Add("Lander", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Lander"].Add("APC", 0);
            DicUnitDamageWeapon1["Lander"].Add("Anti-Air", 0);
            DicUnitDamageWeapon1["Lander"].Add("Artillery", 0);
            DicUnitDamageWeapon1["Lander"].Add("B-Copter", 0);
            DicUnitDamageWeapon1["Lander"].Add("Battleship", 0);
            DicUnitDamageWeapon1["Lander"].Add("Black Boat", 0);
            DicUnitDamageWeapon1["Lander"].Add("Black Bomb", 0);
            DicUnitDamageWeapon1["Lander"].Add("Bomber", 0);
            DicUnitDamageWeapon1["Lander"].Add("Carrier", 0);
            DicUnitDamageWeapon1["Lander"].Add("Cruiser", 0);
            DicUnitDamageWeapon1["Lander"].Add("Fighter", 0);
            DicUnitDamageWeapon1["Lander"].Add("Infantry", 0);
            DicUnitDamageWeapon1["Lander"].Add("Lander", 0);
            DicUnitDamageWeapon1["Lander"].Add("Mech", 0);
            DicUnitDamageWeapon1["Lander"].Add("Medium Tank", 0);
            DicUnitDamageWeapon1["Lander"].Add("Mega Tank", 0);
            DicUnitDamageWeapon1["Lander"].Add("Missile", 0);
            DicUnitDamageWeapon1["Lander"].Add("Neotank", 0);
            DicUnitDamageWeapon1["Lander"].Add("Piperunner", 0);
            DicUnitDamageWeapon1["Lander"].Add("Recon", 0);
            DicUnitDamageWeapon1["Lander"].Add("Rocket", 0);
            DicUnitDamageWeapon1["Lander"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon1["Lander"].Add("Stealth", 0);
            DicUnitDamageWeapon1["Lander"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon1["Lander"].Add("Sub", 0);
            DicUnitDamageWeapon1["Lander"].Add("T-Copter", 0);
            DicUnitDamageWeapon1["Lander"].Add("Tank", 0);
            DicUnitDamageWeapon1.Add("Stealth", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Stealth"].Add("APC", 85);
            DicUnitDamageWeapon1["Stealth"].Add("Anti-Air", 50);
            DicUnitDamageWeapon1["Stealth"].Add("Artillery", 75);
            DicUnitDamageWeapon1["Stealth"].Add("B-Copter", 85);
            DicUnitDamageWeapon1["Stealth"].Add("Battleship", 45);
            DicUnitDamageWeapon1["Stealth"].Add("Black Boat", 65);
            DicUnitDamageWeapon1["Stealth"].Add("Black Bomb", 120);
            DicUnitDamageWeapon1["Stealth"].Add("Bomber", 70);
            DicUnitDamageWeapon1["Stealth"].Add("Carrier", 45);
            DicUnitDamageWeapon1["Stealth"].Add("Cruiser", 35);
            DicUnitDamageWeapon1["Stealth"].Add("Fighter", 45);
            DicUnitDamageWeapon1["Stealth"].Add("Infantry", 90);
            DicUnitDamageWeapon1["Stealth"].Add("Lander", 65);
            DicUnitDamageWeapon1["Stealth"].Add("Mech", 90);
            DicUnitDamageWeapon1["Stealth"].Add("Medium Tank", 70);
            DicUnitDamageWeapon1["Stealth"].Add("Mega Tank", 15);
            DicUnitDamageWeapon1["Stealth"].Add("Missile", 85);
            DicUnitDamageWeapon1["Stealth"].Add("Neotank", 60);
            DicUnitDamageWeapon1["Stealth"].Add("Piperunner", 80);
            DicUnitDamageWeapon1["Stealth"].Add("Recon", 85);
            DicUnitDamageWeapon1["Stealth"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Stealth"].Add("Stealth - Hidden", 55);
            DicUnitDamageWeapon1["Stealth"].Add("Stealth", 55);
            DicUnitDamageWeapon1["Stealth"].Add("Sub - Submerged", 55);
            DicUnitDamageWeapon1["Stealth"].Add("Sub", 55);
            DicUnitDamageWeapon1["Stealth"].Add("T-Copter", 95);
            DicUnitDamageWeapon1["Stealth"].Add("Tank", 75);
        }

        public void PopulateUnitDamageWeapon2()
        {
            DicUnitDamageWeapon2 = new Dictionary<string, Dictionary<string, int>>();

            DicUnitDamageWeapon2.Add("APC", new Dictionary<string, int>());
            DicUnitDamageWeapon2["APC"].Add("APC", 0);
            DicUnitDamageWeapon2["APC"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["APC"].Add("Artillery", 0);
            DicUnitDamageWeapon2["APC"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["APC"].Add("Battleship", 0);
            DicUnitDamageWeapon2["APC"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["APC"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["APC"].Add("Bomber", 0);
            DicUnitDamageWeapon2["APC"].Add("Carrier", 0);
            DicUnitDamageWeapon2["APC"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["APC"].Add("Fighter", 0);
            DicUnitDamageWeapon2["APC"].Add("Infantry", 0);
            DicUnitDamageWeapon2["APC"].Add("Lander", 0);
            DicUnitDamageWeapon2["APC"].Add("Mech", 0);
            DicUnitDamageWeapon2["APC"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["APC"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["APC"].Add("Missile", 0);
            DicUnitDamageWeapon2["APC"].Add("Neotank", 0);
            DicUnitDamageWeapon2["APC"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["APC"].Add("Recon", 0);
            DicUnitDamageWeapon2["APC"].Add("Rocket", 0);
            DicUnitDamageWeapon2["APC"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["APC"].Add("Stealth", 0);
            DicUnitDamageWeapon2["APC"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["APC"].Add("Sub", 0);
            DicUnitDamageWeapon2["APC"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["APC"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Anti-Air", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Anti-Air"].Add("APC", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Lander", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Mech", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Missile", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Recon", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Sub", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Anti-Air"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Artillery", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Artillery"].Add("APC", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Artillery"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Lander", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Mech", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Missile", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Recon", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Sub", 0);
            DicUnitDamageWeapon2["Artillery"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Artillery"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("B-Copter", new Dictionary<string, int>());
            DicUnitDamageWeapon2["B-Copter"].Add("APC", 20);
            DicUnitDamageWeapon2["B-Copter"].Add("Anti-Air", 6);
            DicUnitDamageWeapon2["B-Copter"].Add("Artillery", 25);
            DicUnitDamageWeapon2["B-Copter"].Add("B-Copter", 65);
            DicUnitDamageWeapon2["B-Copter"].Add("Battleship", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("Bomber", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("Carrier", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("Fighter", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("Infantry", 75);
            DicUnitDamageWeapon2["B-Copter"].Add("Lander", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("Mech", 75);
            DicUnitDamageWeapon2["B-Copter"].Add("Medium Tank", 1);
            DicUnitDamageWeapon2["B-Copter"].Add("Mega Tank", 1);
            DicUnitDamageWeapon2["B-Copter"].Add("Missile", 35);
            DicUnitDamageWeapon2["B-Copter"].Add("Neotank", 1);
            DicUnitDamageWeapon2["B-Copter"].Add("Piperunner", 6);
            DicUnitDamageWeapon2["B-Copter"].Add("Recon", 30);
            DicUnitDamageWeapon2["B-Copter"].Add("Rocket", 35);
            DicUnitDamageWeapon2["B-Copter"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("Stealth", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("Sub", 0);
            DicUnitDamageWeapon2["B-Copter"].Add("T-Copter", 95);
            DicUnitDamageWeapon2["B-Copter"].Add("Tank", 6);
            DicUnitDamageWeapon2.Add("Battleship", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Battleship"].Add("APC", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Battleship"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Lander", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Mech", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Missile", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Recon", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Sub", 0);
            DicUnitDamageWeapon2["Battleship"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Battleship"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Black Boat", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Black Boat"].Add("APC", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Lander", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Mech", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Missile", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Recon", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Sub", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Black Boat"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Black Bomb", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Black Bomb"].Add("APC", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Lander", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Mech", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Missile", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Recon", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Sub", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Black Bomb"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Bomber", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Bomber"].Add("APC", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Bomber"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Lander", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Mech", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Missile", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Recon", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Sub", 0);
            DicUnitDamageWeapon2["Bomber"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Bomber"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Carrier", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Carrier"].Add("APC", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Carrier"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Lander", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Mech", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Missile", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Recon", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Sub", 0);
            DicUnitDamageWeapon2["Carrier"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Carrier"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Cruiser", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Cruiser"].Add("APC", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("B-Copter", 115);
            DicUnitDamageWeapon2["Cruiser"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Black Bomb", 120);
            DicUnitDamageWeapon2["Cruiser"].Add("Bomber", 65);
            DicUnitDamageWeapon2["Cruiser"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Fighter", 55);
            DicUnitDamageWeapon2["Cruiser"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Lander", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Mech", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Missile", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Recon", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Stealth - Hidden", 100);
            DicUnitDamageWeapon2["Cruiser"].Add("Stealth", 100);
            DicUnitDamageWeapon2["Cruiser"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("Sub", 0);
            DicUnitDamageWeapon2["Cruiser"].Add("T-Copter", 115);
            DicUnitDamageWeapon2["Cruiser"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Fighter", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Fighter"].Add("APC", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Fighter"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Lander", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Mech", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Missile", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Recon", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Sub", 0);
            DicUnitDamageWeapon2["Fighter"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Fighter"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Infantry", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Infantry"].Add("APC", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Infantry"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Lander", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Mech", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Missile", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Recon", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Sub", 0);
            DicUnitDamageWeapon2["Infantry"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Infantry"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Mech", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Mech"].Add("APC", 20);
            DicUnitDamageWeapon2["Mech"].Add("Anti-Air", 6);
            DicUnitDamageWeapon2["Mech"].Add("Artillery", 32);
            DicUnitDamageWeapon2["Mech"].Add("B-Copter", 9);
            DicUnitDamageWeapon2["Mech"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Mech"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Mech"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Mech"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Mech"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Mech"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Mech"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Mech"].Add("Infantry", 65);
            DicUnitDamageWeapon2["Mech"].Add("Lander", 0);
            DicUnitDamageWeapon2["Mech"].Add("Mech", 55);
            DicUnitDamageWeapon2["Mech"].Add("Medium Tank", 1);
            DicUnitDamageWeapon2["Mech"].Add("Mega Tank", 1);
            DicUnitDamageWeapon2["Mech"].Add("Missile", 35);
            DicUnitDamageWeapon2["Mech"].Add("Neotank", 1);
            DicUnitDamageWeapon2["Mech"].Add("Piperunner", 6);
            DicUnitDamageWeapon2["Mech"].Add("Recon", 18);
            DicUnitDamageWeapon2["Mech"].Add("Rocket", 35);
            DicUnitDamageWeapon2["Mech"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Mech"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Mech"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Mech"].Add("Sub", 0);
            DicUnitDamageWeapon2["Mech"].Add("T-Copter", 35);
            DicUnitDamageWeapon2["Mech"].Add("Tank", 6);
            DicUnitDamageWeapon2.Add("Medium Tank", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Medium Tank"].Add("APC", 45);
            DicUnitDamageWeapon2["Medium Tank"].Add("Anti-Air", 7);
            DicUnitDamageWeapon2["Medium Tank"].Add("Artillery", 45);
            DicUnitDamageWeapon2["Medium Tank"].Add("B-Copter", 12);
            DicUnitDamageWeapon2["Medium Tank"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("Infantry", 105);
            DicUnitDamageWeapon2["Medium Tank"].Add("Lander", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("Mech", 95);
            DicUnitDamageWeapon2["Medium Tank"].Add("Medium Tank", 1);
            DicUnitDamageWeapon2["Medium Tank"].Add("Mega Tank", 1);
            DicUnitDamageWeapon2["Medium Tank"].Add("Missile", 35);
            DicUnitDamageWeapon2["Medium Tank"].Add("Neotank", 1);
            DicUnitDamageWeapon2["Medium Tank"].Add("Piperunner", 8);
            DicUnitDamageWeapon2["Medium Tank"].Add("Recon", 45);
            DicUnitDamageWeapon2["Medium Tank"].Add("Rocket", 45);
            DicUnitDamageWeapon2["Medium Tank"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("Sub", 0);
            DicUnitDamageWeapon2["Medium Tank"].Add("T-Copter", 45);
            DicUnitDamageWeapon2["Medium Tank"].Add("Tank", 8);
            DicUnitDamageWeapon2.Add("Mega Tank", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Mega Tank"].Add("APC", 65);
            DicUnitDamageWeapon2["Mega Tank"].Add("Anti-Air", 17);
            DicUnitDamageWeapon2["Mega Tank"].Add("Artillery", 65);
            DicUnitDamageWeapon2["Mega Tank"].Add("B-Copter", 22);
            DicUnitDamageWeapon2["Mega Tank"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("Infantry", 135);
            DicUnitDamageWeapon2["Mega Tank"].Add("Lander", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("Mech", 125);
            DicUnitDamageWeapon2["Mega Tank"].Add("Medium Tank", 1);
            DicUnitDamageWeapon2["Mega Tank"].Add("Mega Tank", 1);
            DicUnitDamageWeapon2["Mega Tank"].Add("Missile", 55);
            DicUnitDamageWeapon2["Mega Tank"].Add("Neotank", 1);
            DicUnitDamageWeapon2["Mega Tank"].Add("Piperunner", 10);
            DicUnitDamageWeapon2["Mega Tank"].Add("Recon", 65);
            DicUnitDamageWeapon2["Mega Tank"].Add("Rocket", 75);
            DicUnitDamageWeapon2["Mega Tank"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("Sub", 0);
            DicUnitDamageWeapon2["Mega Tank"].Add("T-Copter", 55);
            DicUnitDamageWeapon2["Mega Tank"].Add("Tank", 10);
            DicUnitDamageWeapon2.Add("Missile", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Missile"].Add("APC", 0);
            DicUnitDamageWeapon2["Missile"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Missile"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Missile"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Missile"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Missile"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Missile"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Missile"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Missile"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Missile"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Missile"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Missile"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Missile"].Add("Lander", 0);
            DicUnitDamageWeapon2["Missile"].Add("Mech", 0);
            DicUnitDamageWeapon2["Missile"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Missile"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Missile"].Add("Missile", 0);
            DicUnitDamageWeapon2["Missile"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Missile"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Missile"].Add("Recon", 0);
            DicUnitDamageWeapon2["Missile"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Missile"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Missile"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Missile"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Missile"].Add("Sub", 0);
            DicUnitDamageWeapon2["Missile"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Missile"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Neotank", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Neotank"].Add("APC", 65);
            DicUnitDamageWeapon2["Neotank"].Add("Anti-Air", 17);
            DicUnitDamageWeapon2["Neotank"].Add("Artillery", 65);
            DicUnitDamageWeapon2["Neotank"].Add("B-Copter", 22);
            DicUnitDamageWeapon2["Neotank"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Neotank"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Neotank"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Neotank"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Neotank"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Neotank"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Neotank"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Neotank"].Add("Infantry", 125);
            DicUnitDamageWeapon2["Neotank"].Add("Lander", 0);
            DicUnitDamageWeapon2["Neotank"].Add("Mech", 115);
            DicUnitDamageWeapon2["Neotank"].Add("Medium Tank", 1);
            DicUnitDamageWeapon2["Neotank"].Add("Mega Tank", 1);
            DicUnitDamageWeapon2["Neotank"].Add("Missile", 55);
            DicUnitDamageWeapon2["Neotank"].Add("Neotank", 1);
            DicUnitDamageWeapon2["Neotank"].Add("Piperunner", 10);
            DicUnitDamageWeapon2["Neotank"].Add("Recon", 65);
            DicUnitDamageWeapon2["Neotank"].Add("Rocket", 75);
            DicUnitDamageWeapon2["Neotank"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Neotank"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Neotank"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Neotank"].Add("Sub", 0);
            DicUnitDamageWeapon2["Neotank"].Add("T-Copter", 55);
            DicUnitDamageWeapon2["Neotank"].Add("Tank", 10);
            DicUnitDamageWeapon2.Add("Piperunner", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Piperunner"].Add("APC", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Lander", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Mech", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Missile", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Recon", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Sub", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Piperunner"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Recon", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Recon"].Add("APC", 0);
            DicUnitDamageWeapon2["Recon"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Recon"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Recon"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Recon"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Recon"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Recon"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Recon"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Recon"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Recon"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Recon"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Recon"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Recon"].Add("Lander", 0);
            DicUnitDamageWeapon2["Recon"].Add("Mech", 0);
            DicUnitDamageWeapon2["Recon"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Recon"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Recon"].Add("Missile", 0);
            DicUnitDamageWeapon2["Recon"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Recon"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Recon"].Add("Recon", 0);
            DicUnitDamageWeapon2["Recon"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Recon"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Recon"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Recon"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Recon"].Add("Sub", 0);
            DicUnitDamageWeapon2["Recon"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Recon"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Rocket", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Rocket"].Add("APC", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Rocket"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Lander", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Mech", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Missile", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Recon", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Sub", 0);
            DicUnitDamageWeapon2["Rocket"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Rocket"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Stealth - Hidden", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("APC", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Lander", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Mech", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Missile", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Recon", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Sub", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Stealth - Hidden"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Sub - Submerged", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Sub - Submerged"].Add("APC", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Lander", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Mech", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Missile", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Recon", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Sub", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Sub - Submerged"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Sub", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Sub"].Add("APC", 0);
            DicUnitDamageWeapon2["Sub"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Sub"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Sub"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Sub"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Sub"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Sub"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Sub"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Sub"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Sub"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Sub"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Sub"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Sub"].Add("Lander", 0);
            DicUnitDamageWeapon2["Sub"].Add("Mech", 0);
            DicUnitDamageWeapon2["Sub"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Sub"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Sub"].Add("Missile", 0);
            DicUnitDamageWeapon2["Sub"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Sub"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Sub"].Add("Recon", 0);
            DicUnitDamageWeapon2["Sub"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Sub"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Sub"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Sub"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Sub"].Add("Sub", 0);
            DicUnitDamageWeapon2["Sub"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Sub"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("T-Copter", new Dictionary<string, int>());
            DicUnitDamageWeapon2["T-Copter"].Add("APC", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Artillery", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Battleship", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Bomber", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Carrier", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Fighter", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Infantry", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Lander", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Mech", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Missile", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Neotank", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Recon", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Rocket", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Stealth", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Sub", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["T-Copter"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Tank", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Tank"].Add("APC", 54);
            DicUnitDamageWeapon2["Tank"].Add("Anti-Air", 5);
            DicUnitDamageWeapon2["Tank"].Add("Artillery", 45);
            DicUnitDamageWeapon2["Tank"].Add("B-Copter", 10);
            DicUnitDamageWeapon2["Tank"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Tank"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Tank"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Tank"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Tank"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Tank"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Tank"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Tank"].Add("Infantry", 75);
            DicUnitDamageWeapon2["Tank"].Add("Lander", 0);
            DicUnitDamageWeapon2["Tank"].Add("Mech", 70);
            DicUnitDamageWeapon2["Tank"].Add("Medium Tank", 1);
            DicUnitDamageWeapon2["Tank"].Add("Mega Tank", 1);
            DicUnitDamageWeapon2["Tank"].Add("Missile", 30);
            DicUnitDamageWeapon2["Tank"].Add("Neotank", 1);
            DicUnitDamageWeapon2["Tank"].Add("Piperunner", 6);
            DicUnitDamageWeapon2["Tank"].Add("Recon", 40);
            DicUnitDamageWeapon2["Tank"].Add("Rocket", 55);
            DicUnitDamageWeapon2["Tank"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Tank"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Tank"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Tank"].Add("Sub", 0);
            DicUnitDamageWeapon2["Tank"].Add("T-Copter", 40);
            DicUnitDamageWeapon2["Tank"].Add("Tank", 6);
            DicUnitDamageWeapon2.Add("Lander", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Lander"].Add("APC", 0);
            DicUnitDamageWeapon2["Lander"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Lander"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Lander"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Lander"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Lander"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Lander"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Lander"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Lander"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Lander"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Lander"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Lander"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Lander"].Add("Lander", 0);
            DicUnitDamageWeapon2["Lander"].Add("Mech", 0);
            DicUnitDamageWeapon2["Lander"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Lander"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Lander"].Add("Missile", 0);
            DicUnitDamageWeapon2["Lander"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Lander"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Lander"].Add("Recon", 0);
            DicUnitDamageWeapon2["Lander"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Lander"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Lander"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Lander"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Lander"].Add("Sub", 0);
            DicUnitDamageWeapon2["Lander"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Lander"].Add("Tank", 0);
            DicUnitDamageWeapon2.Add("Stealth", new Dictionary<string, int>());
            DicUnitDamageWeapon2["Stealth"].Add("APC", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Anti-Air", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Artillery", 0);
            DicUnitDamageWeapon2["Stealth"].Add("B-Copter", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Battleship", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Black Boat", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Black Bomb", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Bomber", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Carrier", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Cruiser", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Fighter", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Infantry", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Lander", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Mech", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Medium Tank", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Mega Tank", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Missile", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Neotank", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Piperunner", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Recon", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Rocket", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Stealth - Hidden", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Stealth", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Sub - Submerged", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Sub", 0);
            DicUnitDamageWeapon2["Stealth"].Add("T-Copter", 0);
            DicUnitDamageWeapon2["Stealth"].Add("Tank", 0);
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
            if (!ActiveUnit.Components.IsOnGround)
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

            DicGameType.Add(CampaignGameInfo.ModeName, new CampaignGameInfo(true, null));

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

            NewBuilding.SpriteMap.Origin = new Vector2(NewBuilding.SpriteMap.SpriteWidth / 2, NewBuilding.SpriteMap.SpriteHeight - TileSize.Y / 2);

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
