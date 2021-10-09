using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class TerrainSorcererStreet : Terrain
    {
        public const string FireElement = "Fire";
        public const string WaterElement = "Water";
        public const string EarthElement = "Earth";
        public const string AirElement = "Air";
        public const string EastGate = "East Gate";
        public const string WestGate = "Weast Gate";
        public const string SouthGate = "South Gate";
        public const string NorthGate = "North Gate";

        public CreatureCard DefendingCreature;
        public Player Owner;
        public int TerrainLevel;

        public TerrainSorcererStreet(Terrain Other)
            : base(Other)
        {
            TerrainTypeIndex = Other.TerrainTypeIndex;
            TerrainLevel = 0;
        }

        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public TerrainSorcererStreet(int XPos, int YPos)
            : base(XPos, YPos)
        {
            TerrainTypeIndex = 0;
            MVMoveCost = 1;
            TerrainLevel = 0;
        }
        
        public TerrainSorcererStreet(int XPos, int YPos, int TerrainTypeIndex)
            : base(XPos, YPos)
        {
            this.TerrainTypeIndex = TerrainTypeIndex;
            TerrainLevel = 0;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(TerrainTypeIndex);
        }

        public virtual void OnSelect(SorcererStreetMap Map, Player ActivePlayer)
        {

        }
    }

    public abstract class SorcererStreetMapCutsceneScriptHolder : CutsceneScriptHolder
    {
        public abstract class SorcererStreetMapScript : CutsceneActionScript
        {
            protected SorcererStreetMap Map;

            protected SorcererStreetMapScript(SorcererStreetMap Map, int ScriptWidth, int ScriptHeight, string Name, string[] NameTriggers, string[] NameEvents)
                : base(ScriptWidth, ScriptHeight, Name, NameTriggers, NameEvents)
            {
                this.Map = Map;
            }
        }

        public abstract class SorcererStreetDataContainer : CutsceneDataContainer
        {
            protected SorcererStreetMap Map;

            protected SorcererStreetDataContainer(SorcererStreetMap Map, int ScriptWidth, int ScriptHeight, string Name)
                : base(ScriptWidth, ScriptHeight, Name)
            {
                this.Map = Map;
            }
        }
    }

    public partial class SorcererStreetMap : BattleMap
    {
        public Texture2D sprArrowUp;
        public Texture2D sprEndTurn;

        public readonly List<MapLayer> ListLayer;
        public readonly Vector3 LastPosition;
        public readonly List<Player> ListPlayer;
        public readonly MovementAlgorithm Pathfinder;
        public readonly SorcererStreetBattleContext GlobalSorcererStreetBattleContext;

        public SorcererStreetMap()
        {
            RequireDrawFocus = false;
            Pathfinder = new MovementAlgorithmSorcererStreet(this);
            ListPlayer = new List<Player>();
            GlobalSorcererStreetBattleContext = new SorcererStreetBattleContext();
            ListLayer = new List<MapLayer>();
            ListSingleplayerSpawns = new List<EventPoint>();
            ListMultiplayerSpawns = new List<EventPoint>();
            ListMapSwitchPoint = new List<MapSwitchPoint>();
            ListTilesetPreset = new List<Terrain.TilesetPreset>();

            CursorPosition = new Vector3(0, 0, 0);
            CursorPositionVisible = CursorPosition;
            ListTerrainType = new List<string>();

            ListTerrainType.Add("Not Assigned");
            ListTerrainType.Add("Morph");
            ListTerrainType.Add("Multi-Element");
            ListTerrainType.Add(TerrainSorcererStreet.FireElement);
            ListTerrainType.Add(TerrainSorcererStreet.WaterElement);
            ListTerrainType.Add(TerrainSorcererStreet.EarthElement);
            ListTerrainType.Add(TerrainSorcererStreet.AirElement);
            ListTerrainType.Add(TerrainSorcererStreet.EastGate);
            ListTerrainType.Add(TerrainSorcererStreet.WestGate);
            ListTerrainType.Add(TerrainSorcererStreet.SouthGate);
            ListTerrainType.Add(TerrainSorcererStreet.NorthGate);
            ListTerrainType.Add("Warp");
            ListTerrainType.Add("Bridge");
            ListTerrainType.Add("Fortune Teller");
            ListTerrainType.Add("Spell Circle");
            ListTerrainType.Add("Path Switch");
            ListTerrainType.Add("Card Shop");
            ListTerrainType.Add("Magic Trap");
            ListTerrainType.Add("Siege Tower");
            ListTerrainType.Add("Gem Store");

            ListTileSet = new List<Texture2D>();
            this.CameraPosition = Vector3.Zero;

            this.ListPlayer = new List<Player>();
        }

        public SorcererStreetMap(int GameMode)
            : this()
        {
            this.GameMode = GameMode;
        }

        public SorcererStreetMap(string BattleMapPath, int GameMode)
            : this(GameMode)
        {
            this.BattleMapPath = BattleMapPath;
        }
        
        public override void Save(string FilePath)
        {
            //Create the Part file.
            FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            SaveProperties(BW);

            SaveSpawns(BW);

            MapScript.SaveMapScripts(BW, ListMapScript);

            SaveTilesets(BW);
            
            BW.Write(ListLayer.Count);
            foreach (MapLayer ActiveLayer in ListLayer)
            {
                ActiveLayer.Save(BW);
            }

            FS.Close();
            BW.Close();
        }

        public override void Load()
        {
            base.Load();
            sprArrowUp = Content.Load<Texture2D>("Sorcerer Street/Ressources/Arrow Up");
            sprEndTurn = Content.Load<Texture2D>("Sorcerer Street/Ressources/End Turn");

            LoadMap();
            LoadMapAssets();

            Dictionary<string, CutsceneScript> ConquestScripts = CutsceneScriptHolder.LoadAllScripts(typeof(SorcererStreetMapCutsceneScriptHolder), this);
            foreach (CutsceneScript ActiveListScript in ConquestScripts.Values)
            {
                DicCutsceneScript.Add(ActiveListScript.Name, ActiveListScript);
            }

            OnNewTurn();
        }

        public override void Load(byte[] ArrayGameData)
        {
        }

        public void LoadMap(bool BackgroundOnly = false)
        {
            //Clear everything.
            ListTileSet = new List<Texture2D>();
            FileStream FS = new FileStream("Content/Maps/Sorcerer Street/" + BattleMapPath + ".pem", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            //Map parameters.
            MapName = Path.GetFileNameWithoutExtension(BattleMapPath);

            LoadProperties(BR);

            LoadSpawns(BR);

            ListMapScript = MapScript.LoadMapScripts(BR, DicMapEvent, DicMapCondition, DicMapTrigger, out ListMapEvent);

            LoadTilesets(BR);

            LoadMapGrid(BR);

            BR.Close();
            FS.Close();

            TogglePreview(BackgroundOnly);
        }

        protected void LoadMapGrid(BinaryReader BR)
        {
            int LayerCount = BR.ReadInt32();

            for (int i = 0; i < LayerCount; ++i)
            {
                if (i == 0)
                {
                    ListLayer.Add(new MapLayer(this, ListBackground, ListForeground, BR));
                }
                else
                {
                    ListLayer.Add(new MapLayer(this, null, null, BR));
                }
            }
        }
        public override void AddLocalPlayer(BattleMapPlayer NewPlayer)
        {
            //Player NewDeahtmatchPlayer = new Player(NewPlayer);
            //ListPlayer.Add(NewDeahtmatchPlayer);
            //ListLocalPlayerInfo.Add(NewDeahtmatchPlayer);
        }

        public void EndPlayerPhase()
        {
            //Reset the cursor.
            if (FMODSystem.sndActiveBGMName != sndBattleThemeName && !string.IsNullOrEmpty(sndBattleThemeName))
            {
                sndBattleTheme.Stop();
                sndBattleTheme.SetLoop(true);
                sndBattleTheme.PlayAsBGM();
                FMODSystem.sndActiveBGMName = sndBattleThemeName;
            }
            
            ActivePlayerIndex++;

            if (ActivePlayerIndex >= ListPlayer.Count)
            {
                OnNewTurn();
            }
        }

        protected void OnNewTurn()
        {
            ActivePlayerIndex = 0;
            GameTurn++;

            UpdateMapEvent(EventTypeTurn, 0);
        }

        public override void TogglePreview(bool UsePreview)
        {
            for (int i = 0; i < ListLayer.Count; ++i)
            {
                if (UsePreview)
                {
                    //Recreate the 3D Map to match the updates done on the 2D grid.
                    ListLayer[i].LayerGrid = new Map3D(this, ListLayer[i], ListLayer[i].OriginalLayerGrid, GameScreen.GraphicsDevice);
                }
                else
                {
                    ListLayer[i].LayerGrid = ListLayer[i].OriginalLayerGrid;
                }

                ListLayer[i].LayerGrid.TogglePreview(UsePreview);
            }
        }

        public override bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement)
        {
            return false;
        }

        public void AddPlayer(Player NewPlayer)
        {
            NewPlayer.GamePiece.SpriteMap = Content.Load<Texture2D>("Units/Default");
            NewPlayer.GamePiece.Unit3D = new UnitMap3D(GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), NewPlayer.GamePiece.SpriteMap, 1);

            ListPlayer.Add(NewPlayer);
        }
        
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < ListLayer.Count; ++i)
            {
                ListLayer[i].Update(gameTime);
            }

            if (!IsInit)
            {
                Init();
            }
            else if (MovementAnimation.Count > 0)
            {
                MoveSquad();
            }
            else if (GameMode == 1)
            {
                if (!ListPlayer[ActivePlayerIndex].IsOnline)
                {

                }
            }
            else if (ListPlayer.Count > 0)
            {
                if (!ListActionMenuChoice.HasMainPanel)
                {
                    if (ListPlayer[ActivePlayerIndex].IsHuman)
                    {
                        ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerDefault(this, ListPlayer[ActivePlayerIndex]));
                    }
                    else
                    {
                    }
                }

                ListActionMenuChoice.Last().Update(gameTime);
            }

            UpdateCursorVisiblePosition(gameTime);
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            if (ShowAllLayers)
            {
                for (int i = 0; i < ListLayer.Count; ++i)
                {
                    ListLayer[i].BeginDraw(g);
                }
            }
            else
            {
                ListLayer[ActiveLayerIndex].BeginDraw(g);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Deferred, null);
            if (ShowAllLayers)
            {
                for (int i = 0; i < ListLayer.Count; ++i)
                {
                    ListLayer[i].Draw(g);
                }
            }
            else
            {
                ListLayer[ActiveLayerIndex].Draw(g);
            }

            if (IsOnTop)
            {
                if (ListActionMenuChoice.HasMainPanel)
                {
                    ListActionMenuChoice.Last().Draw(g);
                }
            }
        }
        
        public TerrainSorcererStreet GetTerrain(int X, int Y, int LayerIndex)
        {
            return ListLayer[LayerIndex].ArrayTerrain[X, Y];
        }

        public TerrainSorcererStreet GetTerrain(UnitMapComponent ActiveUnit)
        {
            return GetTerrain((int)ActiveUnit.X, (int)ActiveUnit.Y, ActiveUnit.LayerIndex);
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

        public override BattleMap GetNewMap(int GameMode)
        {
            return new SorcererStreetMap(GameMode);
        }

        public override string GetMapType()
        {
            return "Sorcerer Street";
        }

        public override byte[] GetSnapshotData()
        {
            return new byte[0];
        }

        public override void Update(double ElapsedSeconds)
        {
            GameTime UpdateTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(ElapsedSeconds));
            for (int L = 0; L < ListLayer.Count; L++)
            {
                ListLayer[L].Update(UpdateTime);
            }
        }

        public override void RemoveOnlinePlayer(string PlayerID, IOnlineConnection ActivePlayer)
        {

        }
    }
}
