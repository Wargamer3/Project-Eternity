using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public partial class WorldMap : BattleMap
    {
        public class MapZoneInfo
        {
            public List<Vector3> ListZoneTile;
            public List<int> ListZoneBonus;
            public int NumberOfUnitsRequired;
            public Player TileOwner;
            public string Name;
            public System.Drawing.Color Color;

            public MapZoneInfo()
            {
                ListZoneTile = new List<Vector3>();
            }

            public List<UnitMap> UpdateZoneForPlayer(Player ActivePlayer)
            {
                List<UnitMap> ListReturnUnit = new List<UnitMap>();
                for (int U = ActivePlayer.ListUnit.Count - 1; U >= 0; --U)
                {
                    if (ListZoneTile.Contains(ActivePlayer.ListUnit[U].Position))
                    {
                        ListReturnUnit.Add(ActivePlayer.ListUnit[U]);
                    }
                }

                return ListReturnUnit;
            }
        }
        
        #region Variables
        
        public Texture2D sprWaypoint;
        
        public List<string> ListTileSetPath;

        public List<Player> ListPlayer;
        public List<MapZoneInfo> ListZone;
        public List<MapConsumable> ListConsumable;
        public List<MapLayer> ListLayer;
        protected DrawableGrid MapGrid { get { return ListLayer[0].LayerGrid; } set { ListLayer[0].LayerGrid = value; } }

        #endregion

        public WorldMap()
            : base()
        {
        }

        public WorldMap(string GameMode)
            : base()
        {
            RequireDrawFocus = false;

            ListActionMenuChoice = new ActionPanelHolder();
            CursorPosition = new Vector3(9, 13, 0);
            ListTerrainType = new List<string>();
            ListTerrainType.Add("Air");
            ListTerrainType.Add("Land");
            ListTerrainType.Add("Sea");
            ListTerrainType.Add("Space");

            ListTileSet = new List<Texture2D>();
            ListTileSetPath = new List<string>();
            ListTilesetPreset = new List<Terrain.TilesetPreset>();
            ListSingleplayerSpawns = new List<EventPoint>();
            ListMultiplayerSpawns = new List<EventPoint>();
            ListMapSwitchPoint = new List<MapSwitchPoint>();
            ListZone = new List<MapZoneInfo>();
            ListConsumable = new List<MapConsumable>();
            ListLayer = new List<MapLayer>();
            this.CameraPosition = Vector3.Zero;

            this.ListPlayer = new List<Player>();
        }

        public WorldMap(string BattleMapPath, string GameMode, Dictionary<string, List<Squad>> DicSpawnSquadByPlayer)
            : this(GameMode)
        {
            this.BattleMapPath = BattleMapPath;
            this.DicSpawnSquadByPlayer = DicSpawnSquadByPlayer;
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

            BW.Write(ListZone.Count);
            for (int Z = 0; Z < ListZone.Count; Z++)
            {
                MapZoneInfo ActiveZone = ListZone[Z];
                BW.Write(ActiveZone.Name);
                BW.Write(ActiveZone.NumberOfUnitsRequired);
                BW.Write(ActiveZone.Color.R);
                BW.Write(ActiveZone.Color.G);
                BW.Write(ActiveZone.Color.B);

                BW.Write(ActiveZone.ListZoneTile.Count);
                for (int T = 0; T < ActiveZone.ListZoneTile.Count; T++)
                {
                    BW.Write(ActiveZone.ListZoneTile[T].X);
                    BW.Write(ActiveZone.ListZoneTile[T].Y);
                    BW.Write(ActiveZone.ListZoneTile[T].Z);
                }
            }

            FS.Close();
            BW.Close();
        }

        public override void Load()
        {
            base.Load();
            LoadMap();
            LoadMapAssets();

            MapGrid = new WorldMap2D(this);

            sprWaypoint = Content.Load<Texture2D>("Maps/World Maps/Constructions/Humans/Unit Factory/Waypoint");
            Player NewPlayer = new Player("Player", "Human", true, 0, Factions.Humans, Color.Blue);
            LoadFactionPlayerHumans(NewPlayer);
            ListPlayer.Add(NewPlayer);

            TogglePreview(true);
        }

        public override void Load(byte[] ArrayGameData)
        {
        }

        private void LoadMap(bool BackgroundOnly = false)
        {
            ListTileSet = new List<Texture2D>();
            FileStream FS = new FileStream("Content/Maps/World Maps/" + BattleMapPath + ".pem", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            //Map parameters.
            MapName = Path.GetFileNameWithoutExtension(BattleMapPath);

            LoadProperties(BR);

            LoadSpawns(BR);

            ListMapScript = MapScript.LoadMapScripts(BR, DicMapEvent, DicMapCondition, DicMapTrigger, out ListMapEvent);

            LoadTilesets(BR);

            LoadMapGrid(BR);

            int MapZoneCount = BR.ReadInt32();
            for (int Z = 0; Z < MapZoneCount; Z++)
            {
                MapZoneInfo ActiveZone = new MapZoneInfo();
                ListZone.Add(ActiveZone);

                ActiveZone.Name = BR.ReadString();
                ActiveZone.NumberOfUnitsRequired = BR.ReadInt32();
                byte ColorRed = BR.ReadByte();
                byte ColorGreen = BR.ReadByte();
                byte ColorBlue = BR.ReadByte();
                ActiveZone.Color = System.Drawing.Color.FromArgb(255, ColorRed, ColorGreen, ColorBlue);

                int ListZoneTileCount = BR.ReadInt32();
                for (int T = 0; T < ListZoneTileCount; T++)
                {
                    float PointX = BR.ReadSingle();
                    float PointY = BR.ReadSingle();
                    float PointZ = BR.ReadSingle();
                    ActiveZone.ListZoneTile.Add(new Vector3(PointX, PointY, PointZ));
                }
            }

            FS.Close();
            BR.Close();
        }

        protected void LoadMapGrid(BinaryReader BR)
        {
            int LayerCount = BR.ReadInt32();

            for (int i = 0; i < LayerCount; ++i)
            {
                ListLayer.Add(new MapLayer(this, BR));
            }
        }

        public override void AddLocalPlayer(BattleMapPlayer NewPlayer)
        {
            /*Player NewDeahtmatchPlayer = new Player(NewPlayer);
            ListPlayer.Add(NewDeahtmatchPlayer);
            ListLocalPlayerInfo.Add(NewDeahtmatchPlayer);*/
        }

        public override void TogglePreview(bool UsePreview)
        {
            ShowUnits = !ShowUnits;

            if (!UsePreview)
            {
                //Reset game
                if (IsInit)
                {
                    Init();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsInit)
            {
                Init();
            }
            else if (MovementAnimation.Count > 0)
            {
                MoveSquad();
            }
            else
            {
                if (!ListActionMenuChoice.HasMainPanel)
                {
                    if (ListPlayer[ActivePlayerIndex].IsHuman)
                    {
                        ListActionMenuChoice.Add(new ActionPanelPlayerHumanStep(this));
                    }
                    else
                    {
                    }
                }

                ListActionMenuChoice.Last().Update(gameTime);
            }

            UpdateCursorVisiblePosition(gameTime);

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int U = 0; U < ListPlayer[P].ListConstruction.Count; U++)
                {
                    ListPlayer[P].ListConstruction[U].SpriteMap.Update(gameTime);
                    if (ListPlayer[P].ListConstruction[U].SpriteMap.AnimationEnded)
                    {
                        ListPlayer[P].ListConstruction[U].SpriteMap.RestartAnimation();
                    }
                }
            }
        }
        
        public void OnNewPhase()
        {
            for (int C = ListPlayer[ActivePlayerIndex].ListConstructionInProgress.Count - 1; C >= 0; --C)
            {
                ListPlayer[ActivePlayerIndex].ListConstructionInProgress[C].BuildingTimeRemaining--;
                if (ListPlayer[ActivePlayerIndex].ListConstructionInProgress[C].BuildingTimeRemaining == 0)
                    ListPlayer[ActivePlayerIndex].ListConstructionInProgress.RemoveAt(C++);
            }

            for (int Z = ListZone.Count - 1; Z >= 0; --Z)
            {
                if (ListZone[Z].TileOwner == null || ListZone[Z].UpdateZoneForPlayer(ListZone[Z].TileOwner).Count < ListZone.Count)
                {
                    ListZone[Z].TileOwner = GetPlayerWithHighestUnitCountInZone(ListZone[Z]);
                }
            }

            for (int Z = ListZone.Count - 1; Z >= 0; --Z)
            {
                ListZone[Z].TileOwner.EnergyReserve += 100;
            }

            ++ActivePlayerIndex;
            if (ActivePlayerIndex >= ListPlayer.Count)
            {
                ActivePlayerIndex = 0;
            }
        }

        private Player GetPlayerWithHighestUnitCountInZone(MapZoneInfo ActiveZone)
        {
            List<UnitMap> ListReturnUnit = new List<UnitMap>();
            Player FinalPlayer = null;
            for (int P = ListPlayer.Count - 1; P >= 0; --P)
            {
                List<UnitMap> ListCurrentPlayerUnitInZone = ActiveZone.UpdateZoneForPlayer(ListPlayer[P]);
                if (ListCurrentPlayerUnitInZone.Count > ListReturnUnit.Count)
                {
                    ListReturnUnit = ListCurrentPlayerUnitInZone;
                    FinalPlayer = ListPlayer[P];
                }
            }

            return FinalPlayer;
        }

        public void OnNewTurn()
        {
            for (int C = ListPlayer[ActivePlayerIndex].ListConstructionInProgress.Count - 1; C >= 0; --C)
            {
                ListPlayer[ActivePlayerIndex].ListConstructionInProgress[C].BuildingTimeRemaining--;
                if (ListPlayer[ActivePlayerIndex].ListConstructionInProgress[C].BuildingTimeRemaining == 0)
                    ListPlayer[ActivePlayerIndex].ListConstructionInProgress.RemoveAt(C++);
            }
        }

        public void MoveCursor()
        {
            if (InputHelper.InputLeftPressed())
            {
                //Update the camera if needed.
                if (CursorPosition.X - CameraPosition.X - 3 < 0 && CameraPosition.X > 0)
                    --CameraPosition.X;

                CursorPosition.X -= (CursorPosition.X > 0) ? 1 : 0;
            }
            else if (InputHelper.InputRightPressed())
            {
                //Update the camera if needed.
                if (CursorPosition.X - CameraPosition.X + 3 >= ScreenSize.X && CameraPosition.X + ScreenSize.X < MapSize.X)
                    ++CameraPosition.X;

                CursorPosition.X += (CursorPosition.X < MapSize.X - 1) ? 1 : 0;
            }
            //Y
            if (InputHelper.InputUpPressed())
            {
                //Update the camera if needed.
                if (CursorPosition.Y - CameraPosition.Y - 3 < 0 && CameraPosition.Y > 0)
                    --CameraPosition.Y;

                CursorPosition.Y -= (CursorPosition.Y > 0) ? 1 : 0;
            }
            else if (InputHelper.InputDownPressed())
            {
                //Update the camera if needed.
                if (CursorPosition.Y - CameraPosition.Y + 3 >= ScreenSize.Y && CameraPosition.Y + ScreenSize.Y < MapSize.Y)
                    ++CameraPosition.Y;

                CursorPosition.Y += (CursorPosition.Y < MapSize.Y - 1) ? 1 : 0;
            }
        }

        public void SpawnUnit(int PlayerIndex, UnitMap NewUnit, Vector3 SpawnPosition, Vector3 SpawnDestination)
        {
            NewUnit.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), NewUnit.ActiveUnit.SpriteMap, 1);
            NewUnit.ActiveUnit.Init();
            ListPlayer[PlayerIndex].ListUnit.Add(NewUnit);

            NewUnit.SetPosition(SpawnPosition);

            Vector3 FinalPosition;
            GetEmptyPosition(SpawnDestination, out FinalPosition);

            MovementAnimation.Add(NewUnit.X, NewUnit.Y, NewUnit);

            NewUnit.SetPosition(FinalPosition);
        }

        public void SpawnConstruction(int PlayerIndex, Construction NewConstruction, Vector3 SpawnPosition)
        {
            NewConstruction.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), NewConstruction.SpriteMap.ActiveSprite, 1);

            NewConstruction.SetPosition(SpawnPosition);

            if (NewConstruction.BuildingTimeRemaining > 0)
                ListPlayer[PlayerIndex].ListConstructionInProgress.Add(NewConstruction);

            ListPlayer[PlayerIndex].ListConstruction.Add(NewConstruction);
        }

        public void UpdateWeapons(int TargetPlayerIndex, Unit CurrentUnit, Vector3 Position, int UnitTeam, bool HasMoved, int UnitMovement = 0)
        {
            for (int C = 0; C < ListPlayer[TargetPlayerIndex].ListConstruction.Count; C++)
            {
                if (ListPlayer[TargetPlayerIndex].Team == UnitTeam)//Don't check your team.
                    continue;
                if (ListPlayer[TargetPlayerIndex].ListConstruction[C].HP <= 0)
                    continue;

                for (int W = 0; W < CurrentUnit.ListAttack.Count; W++)
                {
                    CurrentUnit.ListAttack[W].UpdateAttack(CurrentUnit, Position, ListPlayer[TargetPlayerIndex].ListConstruction[C].Position,
                        ListPlayer[TargetPlayerIndex].ListConstruction[C].ArrayMapSize, ListPlayer[TargetPlayerIndex].ListConstruction[C].CurrentMovement, HasMoved);
                }
            }
        }

        public int CheckForSquadAtPosition(int PlayerIndex, Vector3 Position, Vector3 Displacement)
        {
            if (ListPlayer[PlayerIndex].ListUnit.Count == 0)
                return -1;

            Vector3 FinalPosition = Position + Displacement;

            if (FinalPosition.X < 0 || FinalPosition.X > MapSize.X || FinalPosition.Y < 0 || FinalPosition.Y > MapSize.Y)
                return -1;

            int S = 0;
            bool SquadFound = false;
            //Check if there's a Construction.
            while (S < ListPlayer[PlayerIndex].ListUnit.Count && !SquadFound)
            {
                if (ListPlayer[PlayerIndex].ListUnit[S].ActiveUnit == null)
                {
                    ++S;
                    continue;
                }
                if (ListPlayer[PlayerIndex].ListUnit[S].ActiveUnit.UnitStat.IsUnitAtPosition(ListPlayer[PlayerIndex].ListUnit[S].Position, FinalPosition))
                    SquadFound = true;
                else
                    ++S;
            }
            //If a Unit was founded.
            if (SquadFound)
                return S;

            return -1;
        }

        public int CheckForConstructionAtPosition(int PlayerIndex, Vector3 Position, Vector3 Displacement)
        {
            if (ListPlayer[PlayerIndex].ListConstruction.Count == 0)
                return -1;

            Vector3 FinalPosition = Position + Displacement;

            int C = 0;
            bool ConstructionFound = false;
            //Check if there's a Construction.
            while (C < ListPlayer[PlayerIndex].ListConstruction.Count && ListPlayer[PlayerIndex].ListConstruction[C].HP > 0 && !ConstructionFound)
            {
                for (int SizeX = 0; SizeX < ListPlayer[PlayerIndex].ListConstruction[C].MapSize.X && !ConstructionFound; SizeX++)
                {
                    for (int SizeY = 0; SizeY < ListPlayer[PlayerIndex].ListConstruction[C].MapSize.Y && !ConstructionFound; SizeY++)
                    {
                        if (FinalPosition.X == ListPlayer[PlayerIndex].ListConstruction[C].X + SizeX && FinalPosition.Y == ListPlayer[PlayerIndex].ListConstruction[C].Y + SizeY)
                            ConstructionFound = true;
                    }
                }
                if (!ConstructionFound)
                    ++C;
            }
            //If a Unit was founded.
            if (ConstructionFound)
                return C;

            return -1;
        }

        public override bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement)
        {
            bool ObstacleFound = false;

            for (int P = 0; P < ListPlayer.Count && !ObstacleFound; P++)
            {
                ObstacleFound = (CheckForSquadAtPosition(P, Position, Displacement) >= 0) || (CheckForConstructionAtPosition(P, Position, Displacement) >= 0);
            }

            return ObstacleFound;
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

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.BeginUnscaled(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

            if (ShowLayerIndex == -1)
            {
                for (int i = 0; i < ListLayer.Count; ++i)
                {
                    ListLayer[i].BeginDraw(g);
                }
            }
            else
            {
                ListLayer[ShowLayerIndex].BeginDraw(g);
            }

            g.End();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Deferred, null);
            MapGrid.Draw(g, 0, ListLayer[0].ArrayTerrain);
            int Y = Constants.Height - 30;
            DrawBox(g, new Vector2(0, Y), 400, 30, Color.White);
            TextHelper.DrawText(g, "Ressources", new Vector2(5, Y + 5), Color.White);
            TextHelper.DrawTextRightAligned(g, ListPlayer[ActivePlayerIndex].EnergyReserve.ToString(), new Vector2(150, Y + 5), Color.White);
            TextHelper.DrawText(g, "Unit", new Vector2(155, Y + 5), Color.White);
            TextHelper.DrawTextRightAligned(g, ListPlayer[ActivePlayerIndex].ListUnit.Count.ToString(), new Vector2(220, Y + 5), Color.White);
            TextHelper.DrawText(g, "Constructions", new Vector2(225, Y + 5), Color.White);
            TextHelper.DrawTextRightAligned(g, ListPlayer[ActivePlayerIndex].ListConstruction.Count.ToString(), new Vector2(360, Y + 5), Color.White);

            if (ListActionMenuChoice.HasMainPanel)
            {
                ListActionMenuChoice.Last().Draw(g);
            }
        }

        public void DrawConstructionMenuInfo(CustomSpriteBatch g)
        {
            g.Draw(sprPixel, new Rectangle(Constants.Width - 100, 0, Constants.Width, Constants.Height), Color.Black);
            g.Draw(sprPixel, new Rectangle(Constants.Width - 98, 2, 96, 20), Color.Gray);
            g.Draw(sprPixel, new Rectangle(Constants.Width - 98, 24, 96, 100), Color.Gray);
            g.Draw(sprPixel, new Rectangle(Constants.Width - 98, Constants.Height - 200, 96, 198), Color.Gray);
        }

        public override string ToString()
        {
            return "World Map Mode";
        }
    }
}
