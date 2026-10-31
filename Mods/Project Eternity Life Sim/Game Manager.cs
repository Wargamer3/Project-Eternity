using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class NavMapGameManager : GameScreen
    {
        //SFX
        public FMODSound sndConfirm;

        public FMODSound sndSelection;
        public FMODSound sndDeny;
        public FMODSound sndCancel;

        public SpriteFont fntMenuText;

        private List<MapInfo> ListIdleMap;//Loaded but not updated
        private List<MapInfo> ListUpdatedMap;//Loaded and updated but invisible

        public List<PlayerOverseer> ListLocalPlayer;

        public const int MaximumMapLoadDistance = 100;
        public const float SimChunkRadius = 384f;
        public const float SimReleaseRadius = 512f;
        public const float DrawChunkRadius = 768f;

        public MapInfo RootMapContainer;
        public List<MapInfo> AllChunks;
        public List<BuildingZone> Zones;
        public readonly HouseholdState Home = new HouseholdState();
        public readonly WorldClock Clock = new WorldClock();
        public MapInfo FarChunk;
        public PlayerOneDevices PlayerOneChoice = PlayerOneDevices.KeyboardAndPad1;
        public bool SecondPlayerOnLeft;

        readonly HashSet<MapInfo> SimChunks = new HashSet<MapInfo>();
        readonly List<MapInfo> LoadedGround = new List<MapInfo>();
        readonly ControllerHelper[] PadHelpers = new ControllerHelper[4];
        readonly PlayerOverseer[] Seats = new PlayerOverseer[4];
        readonly bool[] SeenConnected = new bool[4];
        double CompanionStep;
        PlayerOneDevices SeenChoice = PlayerOneDevices.KeyboardAndPad1;
        bool SeatsReady;

        public IList<MapInfo> UpdatedMaps
        {
            get { return ListUpdatedMap; }
        }

        public NavMapGameManager()
        {
            ListIdleMap = new List<MapInfo>();
            ListUpdatedMap = new List<MapInfo>();
            Zones = new List<BuildingZone>();

            AllChunks = new List<MapInfo>();
            WorldStream.Start(this);

            ListLocalPlayer = new List<PlayerOverseer>();
            for (int i = 0; i < 4; i++)
            {
                PadHelpers[i] = new ControllerHelper((PlayerIndex)i);
            }

            SyncPlayers(true);
            // The life cursor wakes the chunk under it. AwakeMap refuses a second copy.
            AwakeMap(RootMapContainer);
            LifeSync.Home = Home;
        }

        public override void Load()
        {
            LifeSimCharacterParams.Init();
            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial15");
            ActionLogHolder.InitTextParser(Content, fntMenuText, Color.White);

            sndConfirm = new FMODSound(FMODSystem, "Content/SFX/Confirm.mp3");
            sndDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");
            sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
            sndCancel = new FMODSound(FMODSystem, "Content/SFX/Cancel.mp3");
            SalamanderField.Place(this);
        }

        public override void Update(GameTime gameTime)
        {
            EnvironmentManager RootClock = RootEnvironment();
            Clock.Read(RootClock);
            int LastDay = 0;
            if (RootClock != null && RootClock.GlobalZone != null)
            {
                LastDay = Clock.LastDayIndex(RootClock.GlobalZone.ListTimePeriod);
            }

            Clock.Advance(gameTime.ElapsedGameTime.TotalSeconds, LastDay);
            double Now = Clock.Day * Clock.HoursInDay + Clock.Hour;
            Home.Tick(Now);
            TownLife.Deliver(Now);
            LifeSync.Home = Home;
            LifeSync.Dungeon = Home.Dungeon;
            OreField.Tend(this);
            if (Home.Dungeon != null)
            {
                DungeonPay.Sweep(Home.Dungeon, Home);
            }

            CompanionStep -= gameTime.ElapsedGameTime.TotalSeconds;
            if (Home.Dungeon != null && CompanionStep <= 0)
            {
                Home.Dungeon.StepCompanions();
                CompanionStep = 0.35;
            }

            SkyFade.Advance(this, gameTime.ElapsedGameTime.TotalSeconds);
            Climate.Advance(this, (float)gameTime.ElapsedGameTime.TotalSeconds);
            WeatherDraw.Simulate(this, ListLocalPlayer, gameTime.ElapsedGameTime.TotalSeconds);
            PlacedBoxDraw.Update(gameTime);
            float WaterDt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            WaterDraw.AdvanceClock(WaterDt);
            SkyDraw.Advance(WaterDt);
            foreach (MapInfo ActiveMap in ListUpdatedMap)
            {
                if (ActiveMap.Tracks != null)
                {
                    ActiveMap.Tracks.Tick(WaterDt);
                }
            }
            SyncPlayers(false);
            List<PlayerCharacter> Pathing = new List<PlayerCharacter>();
            foreach (MapInfo ActiveMap in ListUpdatedMap)
            {
                if (ActiveMap.People != null)
                {
                    Pathing.AddRange(ActiveMap.People);
                }
            }

            UpdateChunkStreaming(Pathing);
            foreach (MapInfo ActiveMap in ListUpdatedMap)
            {
                if (SimChunks.Contains(ActiveMap))
                {
                    WaterPlace.DriftProps(ActiveMap, WaterDt);
                }
            }

            ForestScatter.MatchDetail();
            if (LifeSimMap.PhysicsManager != null)
            {
                foreach (MapInfo ActiveMap in ListUpdatedMap)
                {
                    if (!SimChunks.Contains(ActiveMap))
                    {
                        continue;
                    }

                    LifeSimMap.PhysicsManager.SyncChunkSolids(ActiveMap, ActiveMap.Furniture, ActiveMap.Buildings);
                }

                LifeSimMap.PhysicsManager.Update();
            }

            Climate.Drench(this, WaterDt);

            foreach (MapInfo ActiveMap in ListUpdatedMap)
            {
                if (!SimChunks.Contains(ActiveMap))
                {
                    continue;
                }

                WaterPlace.DriftPeople(ActiveMap, ActiveMap.People, WaterDt, LifeSimMap.PhysicsManager);
            }

            Pathing.Clear();
            foreach (MapInfo ActiveMap in ListUpdatedMap)
            {
                if (ActiveMap.MapMapContainer == null)
                {
                    continue;
                }

                if (!SimChunks.Contains(ActiveMap))
                {
                    continue;
                }

                LifeSimMap Map = ActiveMap.MapMapContainer.ActiveMap as LifeSimMap;
                if (Map != null)
                {
                    Map.Update(gameTime);
                }

                if (ActiveMap.People != null)
                {
                    Pathing.AddRange(ActiveMap.People);
                }
            }

            TownLife.Advance(this, gameTime.ElapsedGameTime.TotalSeconds * Clock.Multiplier * Clock.Pace);
            double Hour = Clock.Day * Clock.HoursInDay + Clock.Hour;
            for (int i = 0; i < Pathing.Count; i++)
            {
                PlayerCharacter Who = Pathing[i];
                Who.SimHour = Hour;
                Who.Money = Home.Funds;
                Who.DayAttackBonus = Who.Life.AttackNow(Hour);
                Who.DayCraftBonus = Who.Life.CraftNow(Hour);
                Who.Update(gameTime);
                MapInfo Stood = Who.SharedMapContex == null ? null : Who.SharedMapContex.CurrentMapInfo;
                if (Stood == null || Stood.MapMapContainer == null || Stood.MapMapContainer.ActiveMap == null)
                {
                    Who.PoseBody(gameTime);
                }

                if (Who.AIControlled && !Who.Life.Away.Busy && !TownLife.Riding(Who) && (Who.ListCurrentAIObjectiveStep == null || Who.ListCurrentAIObjectiveStep.Count == 0))
                {
                    HouseholdMind.Act(Who, Home, Hour);
                }
            }

            for (int P = 0; P < ListLocalPlayer.Count; P++)
            {
                PlayerOverseer ActivePlayer = ListLocalPlayer[P];
                ActivePlayer.Update(gameTime);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            Viewport Full = g.GraphicsDevice.Viewport;
            int Live = 0;
            for (int i = 0; i < ListLocalPlayer.Count; i++)
            {
                if (ListLocalPlayer[i] != null)
                {
                    Live++;
                }
            }

            Rectangle[] Panes = SplitLayout.Panes(Live, Full.Width, Full.Height, SecondPlayerOnLeft);
            // Ripple, lightning, and ocean wave targets have to be filled before any pane.
            // Switching targets later discards the scene, which is the land and sky flicker.
            WeatherDraw.PrepareFrame(g, this);
            Ocean.Prepare(g.GraphicsDevice);
            g.GraphicsDevice.Viewport = Full;
            int Pane = 0;
            int Seat = 0;
            foreach (PlayerOverseer ActivePlayer in ListLocalPlayer)
            {
                Seat++;
                if (ActivePlayer == null)
                {
                    continue;
                }

                ActivePlayer.SeatIndex = Seat;
                ActivePlayer.Pane = Panes[Pane];
                Pane++;
                // DirectX Clear ignores the viewport and would wipe every pane drawn so far.
                g.GraphicsDevice.Viewport = new Viewport(ActivePlayer.Pane);
                g.Begin();
                ActivePlayer.Draw(g);
                g.End();
            }

            g.GraphicsDevice.Viewport = Full;
            g.Begin();
        }

        public void DrawChunks(CustomSpriteBatch g, Camera3D Camera, Action<LifeSimMap> Prepare, ZoneDraft Draft = null, PlaySession Viewer = null)
        {
            if (Viewer != null && Viewer.InDungeon && Home.Dungeon != null && Camera != null)
            {
                int FloorIndex;
                if (Home.Dungeon.FloorOf.TryGetValue(Viewer.PartyId, out FloorIndex))
                {
                    DungeonFloor Floor = Home.Dungeon.Visit(FloorIndex);
                    int X = 0;
                    int Y = 0;
                    if (Home.Dungeon.CellX.ContainsKey(Viewer.PartyId))
                    {
                        X = Home.Dungeon.CellX[Viewer.PartyId];
                        Y = Home.Dungeon.CellY[Viewer.PartyId];
                    }

                    Home.Dungeon.Settle(Viewer.PartyId);
                    float Px;
                    float Py;
                    Vector3 Focus = DungeonView.Spot(X, Y, 0);
                    if (Home.Dungeon.PosX.TryGetValue(Viewer.PartyId, out Px) && Home.Dungeon.PosY.TryGetValue(Viewer.PartyId, out Py))
                    {
                        Focus = new Vector3(Px, 20f, Py);
                    }

                    Vector3 Eye = Matrix.Invert(Camera.View).Translation;
                    SkyDraw.Light(Clock, this, Eye);
                    PlacedBoxDraw.Dungeon(g.GraphicsDevice, Floor, Camera.View, Camera.Projection, Eye, Focus, Home.Dungeon, FloorIndex);
                    return;
                }
            }

            g.End();
            SceneShade.Begin(g.GraphicsDevice);
            Vector3 Look = Camera != null ? Matrix.Invert(Camera.View).Translation : Vector3.Zero;
            if (Camera != null)
            {
                LampGlow.Gather(ListUpdatedMap, Look, Camera.View, Camera.Projection, g.GraphicsDevice);
            }

            CharacterShadow.Render(g.GraphicsDevice, Viewer, this, Camera);
            Occlusion.Gather(this, Look);
            g.Begin();

            List<Map3DDrawable> Cursors = new List<Map3DDrawable>();
            List<Map3DDrawable> WaterPass = new List<Map3DDrawable>();
            List<MapInfo> Terrain = new List<MapInfo>();
            List<LifeSimMap> Grids = new List<LifeSimMap>();
            LoadedGround.Clear();
            for (int i = 0; i < ListUpdatedMap.Count; i++)
            {
                if (StreamingGround(ListUpdatedMap[i]))
                {
                    LoadedGround.Add(ListUpdatedMap[i]);
                }
            }

            BoundingFrustum Sight = ChunkSight(g, Camera);
            for (int i = 0; i < ListUpdatedMap.Count; i++)
            {
                MapInfo Info = ListUpdatedMap[i];
                if (!ChunkVisible(Info, Sight))
                {
                    continue;
                }

                LifeSimMap Map = Info.MapMapContainer == null ? null : Info.MapMapContainer.ActiveMap as LifeSimMap;
                if (Map == null || Map.LayerManager == null || Map.LayerManager.LayerHolderDrawable == null)
                {
                    Terrain.Add(Info);
                    continue;
                }

                Map.ShowCursor = false;
                if (Prepare != null)
                {
                    Prepare(Map);
                }

                bool WantCursor = Map.ShowCursor;
                Map.ShowCursor = false;
                if (Map.LayerManager.LayerHolderDrawable is Map3DDrawable Drawable)
                {
                    Drawable.SetChunkShift(ChunkOrigin.Shift(Info));
                    Drawable.SetCamera(Camera);
                    WaterPass.Add(Drawable);
                    if (WantCursor)
                    {
                        Cursors.Add(Drawable);
                    }
                }
                else
                {
                    Map.LayerManager.LayerHolderDrawable.SetCamera(Camera);
                }

                Grids.Add(Map);
            }

            DrawHeight(g, Camera, Terrain);
            g.End();
            SkyDraw.Draw(g.GraphicsDevice, Camera, Clock, this);
            g.Begin();
            for (int i = 0; i < Grids.Count; i++)
            {
                Grids[i].ClearOnDraw = false;
                Grids[i].Draw(g);
                Grids[i].ClearOnDraw = true;
            }

            for (int i = 0; i < WaterPass.Count; i++)
            {
                WaterPass[i].DrawWater(g);
            }

            DrawHeightWater(g, Camera, Terrain);

            for (int i = 0; i < Cursors.Count; i++)
            {
                Cursors[i].DrawCursor(g);
            }

            BuildingZoneDraw.All(g.GraphicsDevice, Camera, ListUpdatedMap, Zones, Draft);
            WeatherDraw.All(g, Camera, this);
            g.End();
            SceneShade.Present(g.GraphicsDevice, Camera, this, Terrain);
            g.Begin();
        }

        static bool StreamingGround(MapInfo Info)
        {
            if (Info == null || Info.Heights == null || Info.Heights.DrawVertices == null || Info.Heights.DrawVertices.Length == 0)
            {
                return false;
            }

            LifeSimMap Map = Info.MapMapContainer == null ? null : Info.MapMapContainer.ActiveMap as LifeSimMap;
            return Map == null || Map.LayerManager == null || Map.LayerManager.LayerHolderDrawable == null;
        }

        void DrawHeight(CustomSpriteBatch g, Camera3D Camera, List<MapInfo> Terrain)
        {
            if (g == null || Camera == null)
            {
                return;
            }

            Viewport Pane = g.GraphicsDevice.Viewport;
            if (Pane.Height <= 0)
            {
                return;
            }

            float Aspect = Pane.Width / (float)Pane.Height;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, 10000f);
            Matrix View = Camera.View;
            bool Watching = Vector3.DistanceSquared(Camera.LookAtTarget, Camera.CameraPosition3D) > 4f;
            Vector3 Focus = Watching ? Camera.LookAtTarget : Vector3.Zero;
            GraphicsDevice Device = g.GraphicsDevice;
            HeightDraw.DrawLoaded(Device, LoadedGround, View, Projection);
            if (Terrain == null || Terrain.Count == 0)
            {
                return;
            }

            ForestScatter.Draw(Device, Terrain, View, Projection, false);
            ForestScatter.Draw(Device, Terrain, View, Projection, true);

            GrassDraw.All(Device, Terrain, View, Projection, Camera.CameraPosition3D);

            for (int i = 0; i < Terrain.Count; i++)
            {
                LandmarkDraw.Draw(Device, Terrain[i], View, Projection);
                MeshChunk.Draw(Device, Terrain[i], View, Projection);
            }

            for (int i = 0; i < Terrain.Count; i++)
            {
                MapInfo Info = Terrain[i];
                PlacedBoxDraw.All(Device, Info.Furniture, Info.Buildings, View, Projection, 0, false, ChunkOrigin.Shift(Info), Info.Heights, Focus, Watching);
            }

            for (int i = 0; i < Terrain.Count; i++)
            {
                MapInfo Info = Terrain[i];
                if (Info.People == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Info);
                for (int p = 0; p < Info.People.Count; p++)
                {
                    PlayerCharacter Who = Info.People[p];
                    if (Who == null || Who.HiddenFromBattle)
                    {
                        continue;
                    }

                    Who.Draw3DOnMap(Device, View, Projection, Shift);
                }
            }
        }

        void DrawHeightWater(CustomSpriteBatch g, Camera3D Camera, List<MapInfo> Terrain)
        {
            if (g == null || Camera == null || Terrain == null || Terrain.Count == 0)
            {
                return;
            }

            Viewport Pane = g.GraphicsDevice.Viewport;
            if (Pane.Height <= 0)
            {
                return;
            }

            float Aspect = Pane.Width / (float)Pane.Height;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, 10000f);
            Matrix View = Camera.View;
            Vector3 Eye = Matrix.Invert(View).Translation;
            GraphicsDevice Device = g.GraphicsDevice;
            for (int i = 0; i < Terrain.Count; i++)
            {
                MapInfo Info = Terrain[i];
                WaterDraw.All(Device, Content, Info.Water, null, Info.Tracks, View, Projection, ChunkOrigin.Shift(Info), Eye, Info.Heights, Info);
            }

            Ocean.Draw(Device, View, Projection, Terrain);
        }

        public void BeginOverlay(CustomSpriteBatch g)
        {
            g.End();
            GraphicsDevice Device = g.GraphicsDevice;
            Device.DepthStencilState = DepthStencilState.None;
            Device.Clear(ClearOptions.DepthBuffer, Color.Black, 1f, 0);
            g.Begin();
        }

        EnvironmentManager RootEnvironment()
        {
            if (RootMapContainer == null || RootMapContainer.MapMapContainer == null)
            {
                return null;
            }

            LifeSimMap Root = RootMapContainer.MapMapContainer.ActiveMap as LifeSimMap;
            return Root == null ? null : Root.MapEnvironment;
        }

        public List<AIAction> FindPath(MapInfo StartMap, Vector3 StartPosition, string Destination)
        {
            return null;
        }

        public List<AIAction> FindPath(PlayerCharacter ActiveCharacter, Vector3 Start, Vector3 End)
        {
            return null;
        }

        public void AddMapContainer(MapInfo ItemToAdd)
        {
            RootMapContainer.AddNestedMap(ItemToAdd);
        }

        public void AccessNewMap(string MapPath)
        {
        }

        public void UpdateChunkStreaming(IList<PlayerCharacter> Characters)
        {
            List<MapInfo> Cameras = new List<MapInfo>();
            foreach (PlayerOverseer Player in ListLocalPlayer)
            {
                if (Player.InvisibleCharacterAsCursor != null && Player.InvisibleCharacterAsCursor.SharedMapContex != null)
                {
                    Cameras.Add(Player.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo);
                }
            }

            HashSet<MapInfo> Keep = ChunkStreaming.Needed(Cameras, Characters, MaximumMapLoadDistance);
            List<Vector3> Eyes = new List<Vector3>();
            if (Characters != null)
            {
                for (int i = 0; i < Characters.Count; i++)
                {
                    PlayerCharacter Who = Characters[i];
                    if (Who == null || Who.SharedMapContex == null || Who.SharedMapContex.CurrentMapInfo == null)
                    {
                        continue;
                    }

                    Vector3 Shift = ChunkOrigin.Shift(Who.SharedMapContex.CurrentMapInfo);
                    Eyes.Add(new Vector3(Who.Position.X + Shift.X, Who.Position.Z, Who.Position.Y + Shift.Z));
                }
            }

            foreach (PlayerOverseer Player in ListLocalPlayer)
            {
                PlayerCharacter Cursor = Player.InvisibleCharacterAsCursor;
                if (Cursor == null || Cursor.SharedMapContex == null || Cursor.SharedMapContex.CurrentMapInfo == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Cursor.SharedMapContex.CurrentMapInfo);
                Eyes.Add(new Vector3(Cursor.Position.X + Shift.X, Cursor.Position.Y, Cursor.Position.Z + Shift.Z));
            }

            TownLife.AddEyes(Eyes);
            if (Eyes.Count == 0 && RootMapContainer != null && RootMapContainer.MapMapContainer != null)
            {
                Eyes.Add(new Vector3(RootMapContainer.MapMapContainer.OffsetX + 32f, 8f, RootMapContainer.MapMapContainer.OffsetY + 32f));
            }

            WorldStream.Gather(this, Keep, Eyes);
            if (Characters != null)
            {
                for (int i = 0; i < Characters.Count; i++)
                {
                    PlayerCharacter Who = Characters[i];
                    if (Who == null || Who.SharedMapContex == null)
                    {
                        continue;
                    }

                    Vector3 Shift = ChunkOrigin.Shift(Who.SharedMapContex.CurrentMapInfo);
                    ChunkStreaming.KeepAround(Keep, AllChunks, Who.Position.X + Shift.X, Who.Position.Y + Shift.Z, DrawChunkRadius * PictureOptions.Reach, FarChunk);
                }
            }

            foreach (PlayerOverseer Player in ListLocalPlayer)
            {
                PlayerCharacter Cursor = Player.InvisibleCharacterAsCursor;
                if (Cursor == null || Cursor.SharedMapContex == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Cursor.SharedMapContex.CurrentMapInfo);
                ChunkStreaming.KeepEdge(Keep, Cursor.SharedMapContex.CurrentMapInfo, Cursor.Position.X, Cursor.Position.Z, MaximumMapLoadDistance);
                ChunkStreaming.KeepAround(Keep, AllChunks, Cursor.Position.X + Shift.X, Cursor.Position.Z + Shift.Z, DrawChunkRadius * PictureOptions.Reach, FarChunk);
            }
            List<MapInfo> Awake = new List<MapInfo>(ListUpdatedMap);
            for (int i = 0; i < Awake.Count; i++)
            {
                if (!Keep.Contains(Awake[i]))
                {
                    SleepMap(Awake[i]);
                }
            }

            foreach (MapInfo Map in Keep)
            {
                AwakeMap(Map);
            }

            RefreshSimulation(Eyes);
        }

        public void AwakeMap(MapInfo MapToWake)
        {
            if (MapToWake == null || MapToWake == FarChunk)
            {
                return;
            }

            EnsureLoaded(MapToWake);
            if (ListUpdatedMap.Contains(MapToWake))
            {
                TownLife.Wake(this, MapToWake);
                return;
            }

            ListIdleMap.Remove(MapToWake);
            ListUpdatedMap.Add(MapToWake);
            TownLife.Wake(this, MapToWake);
        }

        public void SleepMap(MapInfo MapToSleep)
        {
            if (MapToSleep == null || MapToSleep == RootMapContainer || !ListUpdatedMap.Contains(MapToSleep))
            {
                return;
            }

            TownLife.Stow(this, MapToSleep);
            ListUpdatedMap.Remove(MapToSleep);
            if (!ListIdleMap.Contains(MapToSleep))
            {
                ListIdleMap.Add(MapToSleep);
            }

            if (LifeSimMap.PhysicsManager != null)
            {
                LifeSimMap.PhysicsManager.DetachChunk(MapToSleep);
            }
        }

        void EnsureLoaded(MapInfo Map)
        {
            if (Map == null || Map.MapMapContainer == null || Map.ViewReady)
            {
                return;
            }

            WorldStream.OpenView(Map);
        }

        public void ReloadWorld()
        {
            TownLife.Capture();
            List<PlayerCharacter> People = new List<PlayerCharacter>();
            foreach (MapInfo Active in ListUpdatedMap)
            {
                if (Active.People == null)
                {
                    continue;
                }

                for (int i = 0; i < Active.People.Count; i++)
                {
                    if (Active.People[i] != null && !People.Contains(Active.People[i]))
                    {
                        People.Add(Active.People[i]);
                    }
                }
            }

            for (int i = 0; i < AllChunks.Count; i++)
            {
                if (LifeSimMap.PhysicsManager != null)
                {
                    LifeSimMap.PhysicsManager.DetachChunk(AllChunks[i]);
                }
            }

            ListUpdatedMap.Clear();
            ListIdleMap.Clear();
            WorldStream.Start(this);
            LifeSimMap Root = RootMapContainer.MapMapContainer.ActiveMap as LifeSimMap;
            if (Root != null && WorldStream.Slot != null)
            {
                WorldStream.Slot.ApplyClock(Root);
            }

            float LocalX = WorldStream.Slot.ChunkSize * 0.5f;
            float LocalY = LocalX;
            float Feet = RootMapContainer != null && RootMapContainer.Heights != null ? HeightField.Stand(RootMapContainer, LocalX, LocalY) : 8f;
            Vector3 Spawn = new Vector3(LocalX, LocalY, Feet);
            for (int i = 0; i < People.Count; i++)
            {
                PlayerCharacter Who = People[i];
                if (TownLife.PlaceSaved(this, Who))
                {
                    continue;
                }

                if (Who.SharedMapContex != null)
                {
                    Who.SharedMapContex.CurrentMapInfo = RootMapContainer;
                }

                if (RootMapContainer != null)
                {
                    RootMapContainer.Adopt(Who);
                }

                Who.SetPosition(Spawn);
            }

            foreach (PlayerOverseer Player in ListLocalPlayer)
            {
                PlayerCharacter Cursor = Player.InvisibleCharacterAsCursor;
                if (Cursor == null)
                {
                    continue;
                }

                if (Cursor.SharedMapContex != null)
                {
                    Cursor.SharedMapContex.CurrentMapInfo = RootMapContainer;
                }

                Cursor.SetPosition(new Vector3(LocalX, HeightField.View(RootMapContainer, LocalX, LocalY, 0), LocalY));
            }

            AwakeMap(RootMapContainer);
        }

        public void SetPlayerOne(PlayerOneDevices Choice)
        {
            PlayerOneChoice = Choice;
            SyncPlayers(true);
        }

        void SyncPlayers(bool Force)
        {
            bool[] Connected = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                PadHelpers[i].Update();
                Connected[i] = PadHelpers[i].IsConnected;
            }

            bool Same = SeatsReady && PlayerOneChoice == SeenChoice;
            if (Same)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Connected[i] != SeenConnected[i])
                    {
                        Same = false;
                    }
                }
            }

            if (Same && !Force)
            {
                return;
            }

            SeatsReady = true;
            SeenChoice = PlayerOneChoice;
            for (int i = 0; i < 4; i++)
            {
                SeenConnected[i] = Connected[i];
            }

            List<List<LocalDevice>> Resolved = LocalSeats.Resolve(PlayerOneChoice, Connected[0], Connected[1], Connected[2], Connected[3]);
            ListLocalPlayer.Clear();
            for (int i = 0; i < 4; i++)
            {
                if (Resolved[i].Count == 0)
                {
                    if (Seats[i] != null && Seats[i].Session != null)
                    {
                        PartyMember Left = Home.Party.Leave(Seats[i].Session.Seat);
                        if (Left != null && Left.Body != null)
                        {
                            Left.Body.AIControlled = true;
                            Left.Body.RemoveControl();
                        }
                    }

                    Seats[i] = null;
                    continue;
                }

                bool Fresh = Seats[i] == null;
                if (Fresh)
                {
                    Seats[i] = new PlayerOverseer(this, RootMapContainer);
                }

                Seats[i].Session.Home = Home;
                Seats[i].Session.Seat = i;
                if (Fresh)
                {
                    Seats[i].OfferJoin();
                }

                Seats[i].Bind(Resolved[i], PadHelpers);
                ListLocalPlayer.Add(Seats[i]);
            }

            Home.Party.Note(ListLocalPlayer.Count);
        }

        public void UnloadMap(MapInfo MapToUnload)
        {
        }

        public void CreateNewCharacter(string CharacterName, Vector3 Position)
        {
        }

        void RefreshSimulation(List<Vector3> Eyes)
        {
            SimChunks.Clear();
            for (int i = 0; i < ListUpdatedMap.Count; i++)
            {
                MapInfo Info = ListUpdatedMap[i];
                float Distance = ChunkDistance(Info, Eyes);
                bool People = ChunkHasPeople(Info);
                if (People || Distance <= SimChunkRadius)
                {
                    SimChunks.Add(Info);
                }

                if (LifeSimMap.PhysicsManager == null)
                {
                    continue;
                }

                if (People || Distance <= SimReleaseRadius)
                {
                    if (!LifeSimMap.PhysicsManager.HoldsChunk(Info))
                    {
                        EnsureChunkPhysics(Info);
                    }
                }
                else
                {
                    LifeSimMap.PhysicsManager.DetachChunk(Info);
                }
            }
        }

        static void EnsureChunkPhysics(MapInfo Info)
        {
            LifeSimMap Map = Info.MapMapContainer == null ? null : Info.MapMapContainer.ActiveMap as LifeSimMap;
            if (Map != null && Map.LayerManager != null && Map.LayerManager.LayerHolderDrawable is Map3DDrawable Drawable)
            {
                Drawable.SubmitChunkPhysics();
                return;
            }

            WorldStream.Submit(Info);
        }

        static bool ChunkHasPeople(MapInfo Info)
        {
            return Info != null && Info.People != null && Info.People.Count > 0;
        }

        static float ChunkDistance(MapInfo Info, IList<Vector3> Eyes)
        {
            if (Info == null || Info.MapMapContainer == null || Eyes == null || Eyes.Count == 0)
            {
                return 0f;
            }

            float MinX = Info.MapMapContainer.OffsetX;
            float MinZ = Info.MapMapContainer.OffsetY;
            float SpanX = Info.MapMapContainer.SpanX > 1f ? Info.MapMapContainer.SpanX : 256f;
            float SpanZ = Info.MapMapContainer.SpanY > 1f ? Info.MapMapContainer.SpanY : 256f;
            float Best = float.MaxValue;
            for (int i = 0; i < Eyes.Count; i++)
            {
                float X = Eyes[i].X;
                float Z = Eyes[i].Z;
                float NearX = X < MinX ? MinX : X > MinX + SpanX ? MinX + SpanX : X;
                float NearZ = Z < MinZ ? MinZ : Z > MinZ + SpanZ ? MinZ + SpanZ : Z;
                float DX = X - NearX;
                float DZ = Z - NearZ;
                float Distance = (float)Math.Sqrt(DX * DX + DZ * DZ);
                if (Distance < Best)
                {
                    Best = Distance;
                }
            }

            return Best;
        }

        static BoundingFrustum ChunkSight(CustomSpriteBatch g, Camera3D Camera)
        {
            Viewport Pane = g.GraphicsDevice.Viewport;
            float Aspect = Pane.Height > 0 ? Pane.Width / (float)Pane.Height : 1.6f;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, DrawChunkRadius * PictureOptions.Reach + 512f);
            return new BoundingFrustum(Camera.View * Projection);
        }

        static bool ChunkVisible(MapInfo Info, BoundingFrustum Sight)
        {
            if (Info == null || Info.MapMapContainer == null || Sight == null)
            {
                return true;
            }

            float MinX = Info.MapMapContainer.OffsetX;
            float MinZ = Info.MapMapContainer.OffsetY;
            float SpanX = Info.MapMapContainer.SpanX > 1f ? Info.MapMapContainer.SpanX : 256f;
            float SpanZ = Info.MapMapContainer.SpanY > 1f ? Info.MapMapContainer.SpanY : 256f;
            BoundingBox Box = new BoundingBox(new Vector3(MinX, -160f, MinZ), new Vector3(MinX + SpanX, 320f, MinZ + SpanZ));
            return Sight.Intersects(Box);
        }
    }
}
