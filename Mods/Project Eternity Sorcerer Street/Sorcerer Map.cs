using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public partial class SorcererStreetMap : BattleMap
    {
        public Texture2D sprArrowUp;
        public Texture2D sprEndTurn;

        public override MovementAlgorithmTile CursorTerrain { get { return ListLayer[(int)CursorPosition.Z].ArrayTerrain[(int)CursorPosition.X, (int)CursorPosition.Y]; } }

        public readonly List<MapLayer> ListLayer;
        public readonly Vector3 LastPosition;
        public readonly List<Player> ListPlayer;
        public readonly MovementAlgorithm Pathfinder;
        public readonly SorcererStreetBattleContext GlobalSorcererStreetBattleContext;

        public SorcererStreetMap()
        {
            RequireDrawFocus = false;
            ListActionMenuChoice = new ActionPanelHolder();
            Pathfinder = new MovementAlgorithmSorcererStreet(this);
            ListPlayer = new List<Player>();
            GlobalSorcererStreetBattleContext = new SorcererStreetBattleContext();
            ListLayer = new List<MapLayer>();
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

        public SorcererStreetMap(string GameMode)
            : this()
        {
        }

        public SorcererStreetMap(string BattleMapPath, string GameMode)
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

            for (int L = 0; L < LayerCount; ++L)
            {
                if (L == 0)
                {
                    ListLayer.Add(new MapLayer(this, ListBackground, ListForeground, BR, L));
                }
                else
                {
                    ListLayer.Add(new MapLayer(this, null, null, BR, L));
                }
            }
        }


        public override void RemoveUnit(int PlayerIndex, UnitMapComponent UnitToRemove)
        {
            /*ListPlayer[ActivePlayerIndex].ListSquad.Remove((SorcererStreetUnit)UnitToRemove);
            ListPlayer[ActivePlayerIndex].UpdateAliveStatus();*/
        }

        public override void AddUnit(int PlayerIndex, UnitMapComponent UnitToAdd, MovementAlgorithmTile NewPosition)
        {
            /*SorcererStreetUnit ActiveSquad = (SorcererStreetUnit)UnitToAdd;
            for (int U = 0; U < ActiveSquad.UnitsInSquad; ++U)
            {
                ActiveSquad.At(U).ReinitializeMembers(DicUnitType[ActiveSquad.At(U).UnitTypeName]);
            }

            ActiveSquad.ReloadSkills(DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            ListPlayer[PlayerIndex].ListSquad.Add(ActiveSquad);
            ListPlayer[PlayerIndex].UpdateAliveStatus();
            ActiveSquad.SetPosition(new Vector3(NewPosition.WorldPosition.X, NewPosition.WorldPosition.Y, NewPosition.LayerIndex));*/
        }

        public override void ReplaceTile(int X, int Y, int LayerIndex, DrawableTile ActiveTile)
        {
            DrawableTile NewTile = new DrawableTile(ActiveTile);

            /*LayerManager.ListLayer[LayerIndex].LayerGrid.ReplaceTile(X, Y, NewTile);
            LayerManager.LayerHolderDrawable.Reset();*/
        }

        protected override void DoAddLocalPlayer(BattleMapPlayer NewPlayer)
        {
            //Player NewDeahtmatchPlayer = new Player(NewPlayer);
            //ListPlayer.Add(NewDeahtmatchPlayer);
            //ListLocalPlayerInfo.Add(NewDeahtmatchPlayer);
        }

        public override void AddPlatform(BattleMapPlatform NewPlatform)
        {
            /*foreach (Player ActivePlayer in ListPlayer)
            {
                NewPlatform.AddLocalPlayer(ActivePlayer);
            }
            */
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
                        ArrayNewPosition[X + Y * MapSize.X] = new Vector3(X * 32, (LayerManager.ListLayer[Z].ArrayTerrain[X, Y].Height + Z) * 32, Y * 32);
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
            ShowUnits = !ShowUnits;

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
                MovementAnimation.MoveSquad(this);
            }
            else if (ListPlayer.Count > 0)
            {
                if (!ListActionMenuChoice.HasMainPanel)
                {
                    if (ListPlayer[ActivePlayerIndex].IsHuman)
                    {
                        ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerDefault(this, ActivePlayerIndex));
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
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Deferred, null);

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
            return GetTerrain((int)ActiveUnit.X, (int)ActiveUnit.Y, (int)ActiveUnit.Z);
        }
        public string GetTerrainType(float PosX, float PosY, int LayerIndex)
        {
            return GetTerrainType(GetTerrain((int)PosX, (int)PosY, LayerIndex));
        }

        public string GetTerrainType(MovementAlgorithmTile ActiveTerrain)
        {
            return ListTerrainType[ActiveTerrain.TerrainTypeIndex];
        }

        public override string GetTerrainType(int TerrainTypeIndex)
        {
            return ListTerrainType[TerrainTypeIndex];
        }

        public override MovementAlgorithmTile GetNextLayerIndex(MovementAlgorithmTile StartingPosition, int NextX, int NextY, float MaxClearance, float ClimbValue, out List<MovementAlgorithmTile> ListLayerPossibility)
        {
            ListLayerPossibility = new List<MovementAlgorithmTile>();

            string CurrentTerrainType = GetTerrainType(StartingPosition.WorldPosition.X, StartingPosition.WorldPosition.Y, (int)StartingPosition.LayerIndex);
            float CurrentZ = StartingPosition.WorldPosition.Z;

            int ClosestLayerIndexDown = -1;
            int ClosestLayerIndexUp = 0;
            float ClosestTerrainDistanceDown = float.MaxValue;
            float ClosestTerrainDistanceUp = float.MinValue;

            for (int L = 0; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                Terrain NextTerrain = ActiveLayer.ArrayTerrain[NextX, NextY];

                string NextTerrainType = GetTerrainType(NextX, NextY, L);
                float NextTerrainZ = NextTerrain.WorldPosition.Z;

                //Check lower or higher neighbors if on solid ground
                if (CurrentTerrainType != UnitStats.TerrainAir && CurrentTerrainType != UnitStats.TerrainVoid)
                {
                    if (NextTerrainType != UnitStats.TerrainAir && NextTerrainType != UnitStats.TerrainVoid)
                    {
                        //Prioritize going downward
                        if (NextTerrainZ <= CurrentZ)
                        {
                            float ZDiff = CurrentZ - NextTerrainZ;
                            if (ZDiff <= ClosestTerrainDistanceDown && HasEnoughClearance(NextTerrainZ, NextX, NextY, L, MaxClearance))
                            {
                                ClosestTerrainDistanceDown = ZDiff;
                                ClosestLayerIndexDown = L;
                                ListLayerPossibility.Add(NextTerrain);
                            }
                        }
                        else
                        {
                            float ZDiff = NextTerrainZ - CurrentZ;
                            if (ZDiff >= ClosestTerrainDistanceUp && ZDiff <= ClimbValue)
                            {
                                ClosestTerrainDistanceUp = ZDiff;
                                ClosestLayerIndexUp = L;
                                ListLayerPossibility.Add(NextTerrain);
                            }
                        }
                    }
                }
                //Already in void, check for any neighbors
                else
                {
                    if (NextTerrainZ == StartingPosition.LayerIndex && NextTerrainType == CurrentTerrainType)
                    {
                        return NextTerrain;
                    }
                    //Prioritize going upward
                    else if (NextTerrainZ > StartingPosition.LayerIndex)
                    {
                        float ZDiff = NextTerrainZ - CurrentZ;
                        if (ZDiff < ClosestTerrainDistanceUp && ZDiff <= ClimbValue)
                        {
                            ClosestTerrainDistanceUp = ZDiff;
                            ClosestLayerIndexUp = L;
                            ListLayerPossibility.Add(NextTerrain);
                        }
                    }
                }
            }

            if (ClosestLayerIndexDown >= 0)
            {
                return ListLayer[ClosestLayerIndexDown].ArrayTerrain[NextX, NextY];
            }
            else
            {
                return ListLayer[ClosestLayerIndexUp].ArrayTerrain[NextX, NextY];
            }
        }

        public override MovementAlgorithmTile GetMovementTile(int X, int Y, int LayerIndex)
        {
            if (X < 0 || Y >= MapSize.X || Y < 0 || Y >= MapSize.Y || LayerIndex < 0 || LayerIndex >= ListLayer.Count)
            {
                return null;
            }

            return ListLayer[LayerIndex].ArrayTerrain[X, Y];
        }

        private bool HasEnoughClearance(float CurrentZ, int NextX, int NextY, int StartLayer, float MaxClearance)
        {
            for (int L = StartLayer + 1; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                Terrain ActiveTerrain = ActiveLayer.ArrayTerrain[NextX, NextY];

                string NextTerrainType = GetTerrainType(NextX, NextX, L);
                float NextTerrainZ = ActiveTerrain.WorldPosition.Z;

                float ZDiff = NextTerrainZ - CurrentZ;

                if (NextTerrainType != UnitStats.TerrainAir && NextTerrainType != UnitStats.TerrainVoid && ZDiff < MaxClearance)
                {
                    return false;
                }
            }

            return true;
        }

        public override List<MovementAlgorithmTile> GetSpawnLocations(int Team)
        {
            List<MovementAlgorithmTile> ListPossibleSpawnPoint = new List<MovementAlgorithmTile>();

            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                ListPossibleSpawnPoint.AddRange(ActivePlatform.GetSpawnLocations(Team));
            }

            string PlayerTag = (Team + 1).ToString();
            for (int L = 0; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                for (int S = 0; S < ActiveLayer.ListMultiplayerSpawns.Count; S++)
                {
                    if (ActiveLayer.ListMultiplayerSpawns[S].Tag == PlayerTag)
                    {
                        ListPossibleSpawnPoint.Add(ActiveLayer.ArrayTerrain[(int)ActiveLayer.ListMultiplayerSpawns[S].Position.X, (int)ActiveLayer.ListMultiplayerSpawns[S].Position.Y]);
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

        public override BattleMap GetNewMap(string GameMode, string ParamsID)
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

        public override Dictionary<string, ActionPanel> GetOnlineActionPanel()
        {
            Dictionary<string, ActionPanel> DicActionPanel = new Dictionary<string, ActionPanel>();

            Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath("Mods/Project Eternity Sorcerer Street.dll"));
            Dictionary<string, BattleMapActionPanel> DicActionPanelMap = BattleMapActionPanel.LoadFromAssembly(ActiveAssembly, typeof(ActionPanelSorcererStreet), this);
            foreach (KeyValuePair<string, BattleMapActionPanel> ActiveRequirement in DicActionPanelMap)
            {
                DicActionPanel.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            return DicActionPanel;
        }
    }
}
