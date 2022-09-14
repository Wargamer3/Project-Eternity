using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchMapCutsceneScriptHolder : CutsceneScriptHolder
    {
        public abstract class DeathmatchMapScript : CutsceneActionScript
        {
            protected DeathmatchMap Map;

            protected DeathmatchMapScript(DeathmatchMap Map, int ScriptWidth, int ScriptHeight, string Name, string[] NameTriggers, string[] NameEvents)
                : base(ScriptWidth, ScriptHeight, Name, NameTriggers, NameEvents)
            {
                this.Map = Map;
            }
        }

        public abstract class DeathmatchMapDataContainer : CutsceneDataContainer
        {
            protected DeathmatchMap Map;

            protected DeathmatchMapDataContainer(DeathmatchMap Map, int ScriptWidth, int ScriptHeight, string Name)
                : base(ScriptWidth, ScriptHeight, Name)
            {
                this.Map = Map;
            }
        }
    }

    public partial class DeathmatchMap : BattleMap
    {
        #region Variables

        public static readonly string MapType = "Deathmatch";
        
        private SpriteFont fntArial16;

        public override MovementAlgorithmTile CursorTerrain { get { return LayerManager.ListLayer[(int)CursorPosition.Z].ArrayTerrain[(int)CursorPosition.X, (int)CursorPosition.Y]; } }

        private List<Player> ListLocalPlayerInfo;
        public List<Player> ListPlayer;
        public List<Player> ListLocalPlayer { get { return ListLocalPlayerInfo; } }
        public List<Player> ListAllPlayer { get { return ListPlayer; } }

        public List<DelayedAttack> ListDelayedAttack;
        public List<PERAttack> ListPERAttack;
        public MovementAlgorithm Pathfinder;
        public LayerHolderDeathmatch LayerManager;

        public NonDemoScreen NonDemoScreen;
        public SpiritMenu SpiritMenu;
        public MapMenu BattleMapMenu;
        public UnitDeploymentScreen UnitDeploymentScreen;
        public DeathmatchParams GlobalBattleParams;
        public List<DeathmatchMutator> ListMutator;

        public int ActiveSquadIndex
        {
            get
            {
                return _ActiveSquadIndex;
            }
            set
            {
                if (value >= 0)
                {
                    _ActiveSquadIndex = value;
                    _ActiveSquad = ListPlayer[ActivePlayerIndex].ListSquad[value];
                }
                else
                {
                    _ActiveSquadIndex = -1;
                    _ActiveSquad = null;
                }
            }
        }

        private int _ActiveSquadIndex;//Unit selected by the active player.
        public Squad ActiveSquad { get { return _ActiveSquad; } }

        private Squad _ActiveSquad;

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
                    _TargetSquad = ListPlayer[TargetPlayerIndex].ListSquad[value];
                }
                else
                {
                    _TargetSquadIndex = -1;
                    _TargetSquad = null;
                }
            }
        }

        private int _TargetSquadIndex;//Unit targetted by the active player.
        public Squad TargetSquad { get { return _TargetSquad; } }

        private Squad _TargetSquad;
        public int TargetPlayerIndex;//Player of controling TargetUnit.

        #endregion

        public DeathmatchMap()
            : this(new DeathmatchParams(new BattleContext()))
        {
            DeathmatchParams.DicParams.TryAdd(string.Empty, GlobalBattleParams);
        }

        public DeathmatchMap(DeathmatchParams Params)
            : base()
        {
            this.Params = GlobalBattleParams = Params;

            GameRule = new SinglePlayerGameRule(this);
            LayerManager = new LayerHolderDeathmatch(this);
            MapEnvironment = new EnvironmentManagerDeathmatch(this);
            ListActionMenuChoice = new ActionPanelHolderDeathmatch(this);
            ActiveParser = new DeathmatchFormulaParser(this);
            ActivePlayerIndex = 0;
            ListPlayer = new List<Player>();
            ListLocalPlayerInfo = new List<Player>();
            RequireFocus = false;
            RequireDrawFocus = true;
            Pathfinder = new MovementAlgorithmDeathmatch(this);
            ListDelayedAttack = new List<DelayedAttack>();
            ListPERAttack = new List<PERAttack>();
            ListMutator = new List<DeathmatchMutator>();

            ListTerrainType = new List<string>();
            ListTerrainType.Add(UnitStats.TerrainAir);
            ListTerrainType.Add(UnitStats.TerrainLand);
            ListTerrainType.Add(UnitStats.TerrainSea);
            ListTerrainType.Add(UnitStats.TerrainSpace);
            ListTerrainType.Add(UnitStats.TerrainWall);
            ListTerrainType.Add(UnitStats.TerrainVoid);
        }

        public DeathmatchMap(string GameMode, DeathmatchParams Params)
            : this(Params)
        {
            CursorPosition = new Vector3(9, 13, 0);
            CursorPositionVisible = CursorPosition;

            ListTileSet = new List<Texture2D>();
            ListTilesetPreset = new List<Terrain.TilesetPreset>();
            CameraPosition = Vector3.Zero;
            ActiveSquadIndex = -1;

            switch (GameMode)
            {
                case "":
                    GameRule = new SinglePlayerGameRule(this);
                    break;

                case "Classic":
                    GameRule = new ClassicMPGameRule(this);
                    break;

                case "Campaign":
                    GameRule = new CampaignGameRule(this);
                    break;

                case "Horde":
                    GameRule = new HordeGameRule(this);
                    break;

                case "Deathmatch":
                    GameRule = new DeathmatchGameRule(this);
                    break;

                case "Capture The Flag":
                    GameRule = new CaptureTheFlagGameRule(this);
                    break;

                case "Titan":
                    GameRule = new TitanGameRule(this);
                    break;
            }
        }

        public DeathmatchMap(string BattleMapPath, string GameMode, DeathmatchParams Params)
            : this(GameMode, Params)
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

            LayerManager.Save(BW);

            MapEnvironment.Save(BW);

            FS.Close();
            BW.Close();
        }
        
        public override void Load()
        {
            if (PlayerRoster == null)
            {
                PlayerRoster = new Roster();
                PlayerRoster.LoadRoster();
            }

            base.Load();
            if (!IsServer)
            {
                LoadPreBattleMenu();
            }
            LoadMap();
            LoadMapAssets();
            LoadDeathmatchAIScripts();

            Dictionary<string, CutsceneScript> DeathmatchMapScripts = CutsceneScriptHolder.LoadAllScripts(typeof(DeathmatchMapCutsceneScriptHolder), this);
            foreach (CutsceneScript ActiveListScript in DeathmatchMapScripts.Values)
            {
                DicCutsceneScript.Add(ActiveListScript.Name, ActiveListScript);
            }

            SpiritMenu = new SpiritMenu(this);
            if (!IsServer)
            {
                SpiritMenu.Load();
            }

            BattleMapMenu = new MapMenu(this);
            if (!IsServer)
            {
                BattleMapMenu.Load(Content, FMODSystem);
            }

            UnitDeploymentScreen = new UnitDeploymentScreen(PlayerRoster);
            if (!IsServer)
            {
                UnitDeploymentScreen.Load(Content);
            }

            NonDemoScreen = new NonDemoScreen(this);
            if (!IsServer)
            {
                NonDemoScreen.Load();

                fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
                fntArial16 = Content.Load<SpriteFont>("Fonts/Arial16");
            }
        }

        public override void Load(byte[] ArrayGameData)
        {
            ByteReader BR = new ByteReader(ArrayGameData);

            int ListCharacterCount = BR.ReadInt32();
            for (int P = 0; P < ListCharacterCount; ++P)
            {
                string PlayerID = BR.ReadString();
                string PlayerName = BR.ReadString();
                int PlayerTeam = BR.ReadInt32();
                bool IsPlayerControlled = BR.ReadBoolean();
                Color PlayerColor = Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), 255);

                byte LocalPlayerIndex = BR.ReadByte();

                Player NewPlayer;
                if (PlayerManager.OnlinePlayerID == PlayerID)
                {
                    NewPlayer = new Player(PlayerManager.ListLocalPlayer[LocalPlayerIndex]);
                    NewPlayer.Team = PlayerTeam;
                    NewPlayer.Color = PlayerColor;
                    AddLocalCharacter(NewPlayer);
                    //NewPlayer.InputManagerHelper = new PlayerRobotInputManager();
                    //NewPlayer.UpdateControls(GameplayTypes.MouseAndKeyboard);
                }
                else
                {
                    NewPlayer = new Player(PlayerName, "Online", true, true, PlayerTeam, PlayerColor);
                    NewPlayer.LocalPlayerIndex = LocalPlayerIndex;
                    ListAllPlayer.Add(NewPlayer);
                }

                NewPlayer.IsPlayerControlled = IsPlayerControlled;
                NewPlayer.Inventory.ActiveLoadout.ListSpawnSquad.Clear();
                int ArraySquadLength = BR.ReadInt32();
                for (int S = 0; S < ArraySquadLength; ++S)
                {
                    float SquadX = BR.ReadFloat();
                    float SquadY = BR.ReadFloat();
                    float SquadZ = BR.ReadFloat();
                    bool SquadIsPlayerControlled = BR.ReadBoolean();

                    int UnitsInSquad = BR.ReadInt32();
                    Unit[] ArrayNewUnit = new Unit[UnitsInSquad];
                    for (int U = 0; U < UnitsInSquad; ++U)
                    {
                        string UnitTypeName = BR.ReadString();
                        string RelativePath = BR.ReadString();

                        ArrayNewUnit[U] = PlayerManager.DicUnitType[UnitTypeName].FromFile(RelativePath, Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);

                        int ArrayCharacterLength = BR.ReadInt32();
                        ArrayNewUnit[U] .ArrayCharacterActive = new Character[UnitsInSquad];
                        for (int C = 0; C < ArrayCharacterLength; ++C)
                        {
                            string CharacterPath = BR.ReadString();
                            ArrayNewUnit[U].ArrayCharacterActive[C] = new Character(CharacterPath, Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                            ArrayNewUnit[U].ArrayCharacterActive[C].Level = 1;
                        }
                    }

                    Unit Leader = ArrayNewUnit[0];

                    Unit WingmanA = null;
                    if (UnitsInSquad > 1)
                    {
                        WingmanA = ArrayNewUnit[1];

                    }

                    Unit WingmanB = null;
                    if (UnitsInSquad > 2)
                    {
                        WingmanB = ArrayNewUnit[2];

                    }
                    Squad NewSquad = new Squad("", Leader, WingmanA, WingmanB);
                    NewSquad.SetPosition(new Vector3(SquadX, SquadY, SquadZ));
                    NewSquad.IsPlayerControlled = SquadIsPlayerControlled;
                    NewPlayer.Inventory.ActiveLoadout.ListSpawnSquad.Add(NewSquad);
                }
            }

            BattleMapPath = BR.ReadString();

            Load();
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                Player ActivePlayer = ListPlayer[P];
                foreach (Squad ActiveSquad in ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad)
                {
                    ActiveSquad.ReloadSkills(GlobalBattleParams.DicUnitType, GlobalBattleParams.DicRequirement, GlobalBattleParams.DicEffect, GlobalBattleParams.DicAutomaticSkillTarget, GlobalBattleParams.DicManualSkillTarget);
                    SpawnSquad(P, ActiveSquad, 0, new Vector2(ActiveSquad.Position.X, ActiveSquad.Position.Y), (int)ActiveSquad.Position.Z);
                }
            }
            Init();
            TogglePreview(true);

            BR.Clear();
        }

        private void LoadMap()
        {
            //Clear everything.
            ListBackgroundsPath = new List<string>();
            ListForegroundsPath = new List<string>();
            FileStream FS = new FileStream("Content/Maps/Deathmatch/" + BattleMapPath + ".pem", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            //Map parameters.
            MapName = Path.GetFileNameWithoutExtension(BattleMapPath);

            LoadProperties(BR);

            ListMapScript = MapScript.LoadMapScripts(BR, DicMapEvent, DicMapCondition, DicMapTrigger, out ListMapEvent);

            LoadTilesets(BR);

            LayerManager = new LayerHolderDeathmatch(this, BR);

            MapEnvironment = new EnvironmentManagerDeathmatch(BR, this);

            BR.Close();
            FS.Close();
        }

        public void LoadDeathmatchAIScripts()
        {
            AIScriptHolder.DicAIScripts.Clear();

            foreach (KeyValuePair<string, List<AIScript>> ActiveListScript in ReflectionHelper.GetContentByName<AIScript>(typeof(CoreAI), Directory.GetFiles("AI", "*.dll")))
            {
                for (int S = ActiveListScript.Value.Count - 1; S >= 0; --S)
                    AIScriptHolder.DicAIScripts.Add(ActiveListScript.Value[S].Name, ActiveListScript.Value[S]);
            }
            foreach (KeyValuePair<string, List<AIScript>> ActiveListScript in ReflectionHelper.GetContentByName<AIScript>(typeof(DeathmatchAIScriptHolder), Directory.GetFiles("AI", "*.dll")))
            {
                for (int S = ActiveListScript.Value.Count - 1; S >= 0; --S)
                    AIScriptHolder.DicAIScripts.Add(ActiveListScript.Value[S].Name, ActiveListScript.Value[S]);
            }
        }

        protected override void LoadInteractiveProps()
        {
            base.LoadInteractiveProps();

            foreach (KeyValuePair<string, InteractiveProp> ActiveProp in InteractiveProp.LoadFromAssemblyFiles(Directory.GetFiles("Props/Deathmatch Map", "*.dll"), this))
            {
                ActiveProp.Value.Load(Content);
                DicInteractiveProp.Add(ActiveProp.Value.PropName, ActiveProp.Value);
            }
        }

        protected override void DoAddLocalPlayer(BattleMapPlayer NewPlayer)
        {
            Player NewDeahtmatchPlayer = new Player(NewPlayer);

            ListPlayer.Add(NewDeahtmatchPlayer);
            ListLocalPlayerInfo.Add(NewDeahtmatchPlayer);
        }

        public void AddLocalCharacter(Player NewLocalCharacter)
        {
            ListPlayer.Add(NewLocalCharacter);
            ListLocalPlayerInfo.Add(NewLocalCharacter);
        }

        public override void SetMutators(List<Mutator> ListMutator)
        {
            foreach (DeathmatchMutator ActiveMutator in ListMutator)
            {
                this.ListMutator.Add(ActiveMutator);
            }
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
            }

            LayerManager.TogglePreview(UsePreview);
        }

        public void Reset()
        {
            LayerManager.LayerHolderDrawable.Reset();
            MapEnvironment.Reset();
        }

        public Terrain GetTerrain(float X, float Y, int LayerIndex)
        {
            return LayerManager.ListLayer[LayerIndex].ArrayTerrain[(int)X, (int)Y];
        }

        public Terrain GetTerrain(UnitMapComponent ActiveUnit)
        {
            return LayerManager.ListLayer[(int)ActiveUnit.Z].ArrayTerrain[(int)ActiveUnit.X, (int)ActiveUnit.Y];
        }

        public DrawableTile GetTile(UnitMapComponent ActiveUnit)
        {
            return LayerManager.GetTile((int)ActiveUnit.X, (int)ActiveUnit.Y, (int)ActiveUnit.Z);
        }

        public List<MovementAlgorithmTile> GetAllTerrain(UnitMapComponent ActiveUnit, BattleMap ActiveMap)
        {
            List<MovementAlgorithmTile> ListTerrainFound = new List<MovementAlgorithmTile>();
            for (int X = 0; X < ActiveUnit.ArrayMapSize.GetLength(0); ++X)
            {
                for (int Y = 0; Y < ActiveUnit.ArrayMapSize.GetLength(1); ++Y)
                {
                    if (ActiveUnit.ArrayMapSize[X, Y])
                    {
                        ListTerrainFound.Add(ActiveMap.GetMovementTile((int)ActiveUnit.X + X, (int)ActiveUnit.Y + Y, (int)ActiveUnit.Z));
                    }
                }
            }

            return ListTerrainFound;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsFrozen)
            {
                if (ShowUnits)
                {
                    MapEnvironment.Update(gameTime);
                }

                for (int B = 0; B < ListBackground.Count; ++B)
                {
                    ListBackground[B].Update(gameTime);
                }

                for (int F = 0; F < ListForeground.Count; ++F)
                {
                    ListForeground[F].Update(gameTime);
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
                    MovementAnimation.MoveSquad(this);
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
                        foreach (IOnlineConnection ActivePlayer in GameGroup.Room.ListOnlinePlayer)
                        {
                            ActivePlayer.Send(new ServerIsReadyScriptServer());
                        }
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
                        foreach (IOnlineConnection ActivePlayer in GameGroup.Room.ListOnlinePlayer)
                        {
                            ActivePlayer.Send(new ServerIsReadyScriptServer());
                        }

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
            g.GraphicsDevice.SetRenderTarget(null);
            g.BeginUnscaled(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

            if (ShowUnits)
            {
                BeginDrawNightOverlay(g);
            }

            LayerManager.BeginDraw(g);

            g.End();

            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                ActivePlatform.BeginDraw(g);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (!IsInit)
            {
                return;
            }

            if (!IsAPlatform)
            {
                g.GraphicsDevice.Clear(Color.Black);
            }

            //Handle screen shaking.
            if (IsShaking)
            {
                g.End();

                //Run during initialization
                ShakingRenderTraget = new RenderTarget2D(GraphicsDevice, Constants.Width, Constants.Height, false,
                    GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

                GraphicsDevice.SetRenderTarget(ShakingRenderTraget);

                g.Begin();
            }

            if (ListBackground.Count > 0)
            {
                g.End();
                for (int B = 0; B < ListBackground.Count; B++)
                {
                    ListBackground[B].Draw(g, Constants.Width, Constants.Height);
                }
                g.Begin();
            }

            LayerManager.Draw(g);

            if (ShowUnits)
            {
                DrawNightOverlay(g);
            }

            if (ListForeground.Count > 0)
            {
                g.End();
                for (int F = 0; F < ListForeground.Count; F++)
                {
                    ListForeground[F].Draw(g, Constants.Width, Constants.Height);
                }
                g.Begin();
            }

            GameRule.Draw(g);

            if (IsOnTop)
            {
                if (ListActionMenuChoice.HasMainPanel)
                {
                    ListActionMenuChoice.Last().Draw(g);
                }
            }

            #region Handle screen shaking.

            if (IsShaking)
            {
                g.End();

                //Switches rendertarget back to backbuffer
                GraphicsDevice.SetRenderTarget(null);

                GraphicsDevice.Clear(Color.Black);

                g.Begin();

                // counter is a float initially set to zero
                ShakeCounter += 0.45f;
                Vector2 Translation = new Vector2(ShakeOffsetX + ShakeAngleVariation.X * (float)Math.Sin(ShakeCounter),
                                                                                                    ShakeOffsetY + ShakeAngleVariation.Y * (float)Math.Sin(ShakeCounter));
                //Reached the peak of the sin function.
                if (ShakeCounter >= MathHelper.PiOver2)
                {
                    //Remember where and how the shake ended.
                    ShakeOffsetX = ShakeOffsetX + ShakeAngleVariation.X;
                    ShakeOffsetY = ShakeOffsetY + ShakeAngleVariation.Y;

                    //Calculate new shake angle.
                    ShakeAngle = (ShakeAngle + 150 + RandomHelper.Next(60)) % 360;
                    float Angle = MathHelper.ToRadians(ShakeAngle);
                    ShakeAngleVariation.X = (float)Math.Cos(Angle);
                    ShakeAngleVariation.Y = (float)Math.Sin(Angle);

                    float DestinationX = ShakeRadiusMax * ShakeAngleVariation.X;
                    float DestinationY = ShakeRadiusMax * ShakeAngleVariation.Y;

                    ShakeAngleVariation.X = DestinationX - ShakeOffsetX;
                    ShakeAngleVariation.Y = DestinationY - ShakeOffsetY;
                    ShakeCounter = 0;

                    if (IsShakingEnded)
                        IsShaking = false;
                }

                g.Draw(ShakingRenderTraget, Translation, null, Color.White,
                    0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.9f);
            }

            #endregion

            #region Handle fade to black

            if (FadeIsActive)
            {
                g.Draw(sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.FromNonPremultiplied(0, 0, 0, (int)FadeAlpha));
            }

            #endregion
        }

        private void BeginDrawNightOverlay(CustomSpriteBatch g)
        {
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int S = 0; S < ListPlayer[P].ListSquad.Count; S++)
                {
                    if (ListPlayer[P].ListSquad[S].CurrentLeader != null)
                    {
                        ListPlayer[P].ListSquad[S].DrawTimeOfDayOverlayOnMap(g, ListPlayer[P].ListSquad[S].Position, 24);
                    }
                }
            }
        }

        private void DrawNightOverlay(CustomSpriteBatch g)
        {
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int S = 0; S < ListPlayer[P].ListSquad.Count; S++)
                {
                    if (!ListPlayer[P].ListSquad[S].IsDead)
                    {
                        ListPlayer[P].ListSquad[S].DrawOverlayOnMap(g, ListPlayer[P].ListSquad[S].Position);
                    }
                }
            }
        }

        public void CenterCamera()
        {
            if (ActiveSquad == null)
                return;

            if (ActiveSquad.X < CameraPosition.X || ActiveSquad.Y < CameraPosition.Y ||
                ActiveSquad.X >= CameraPosition.X + ScreenSize.X || ActiveSquad.Y >= CameraPosition.Y + ScreenSize.Y)
            {
                PushScreen(new CenterOnSquadCutscene(null, this, ActiveSquad.Position));
            }
        }
        
        public void UpdateAllAttacks(Unit CurrentUnit, Vector3 StartPosition, int UnitTeam, bool CanMove)
        {
            CurrentUnit.DisableAllAttacks();

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int U = 0; U < ListPlayer[P].ListSquad.Count; U++)
                {
                    if (ListPlayer[P].ListSquad[U].CurrentLeader == null)
                        continue;

                    CurrentUnit.UpdateAllAttacks(StartPosition, UnitTeam, ListPlayer[P].ListSquad[U].Position, ListPlayer[P].Team,
                            ListPlayer[P].ListSquad[U].ArrayMapSize, ListPlayer[P].ListSquad[U].CurrentMovement, CanMove);
                }
            }
        }
        
        public int GetSquadMaxMovement(Squad ActiveSquad)
        {
            if (ActiveSquad.CurrentMovement == UnitStats.TerrainAir)
            {
                int StartingMV = Math.Min(ActiveSquad.CurrentLeader.MaxMovement, ActiveSquad.CurrentLeader.EN);//Maximum distance you can reach.
                StartingMV += ActiveSquad.CurrentLeader.Boosts.MovementModifier;

                if (ActiveSquad.CurrentWingmanA != null)
                {
                    if (ActiveSquad.CurrentWingmanA.EN < StartingMV)
                        StartingMV = ActiveSquad.CurrentWingmanA.EN;
                }
                if (ActiveSquad.CurrentWingmanB != null)
                {
                    if (ActiveSquad.CurrentWingmanB.EN < StartingMV)
                        StartingMV = ActiveSquad.CurrentWingmanB.EN;
                }
                return StartingMV;
            }
            else
            {
                int StartingMV = ActiveSquad.CurrentLeader.MaxMovement;//Maximum distance you can reach.
                StartingMV += ActiveSquad.CurrentLeader.Boosts.MovementModifier;

                if (ActiveSquad.CurrentWingmanA != null)
                {
                    StartingMV += ActiveSquad.CurrentWingmanA.MaxMovement;
                    if (ActiveSquad.CurrentWingmanB != null)
                    {
                        StartingMV += ActiveSquad.CurrentWingmanA.MaxMovement;
                        StartingMV = (int)Math.Ceiling((double)(StartingMV / 3));
                    }
                    else
                        StartingMV = (int)Math.Ceiling((double)(StartingMV / 2));
                }
                return StartingMV;
            }
        }

        public void UpdateSquadCurrentMovement(Squad ActiveSquad)
        {
            //Unit can't stay on this terrain.
            if (!ActiveSquad.CurrentLeader.ListTerrainChoices.Contains(ActiveSquad.CurrentMovement))
            {
                //Can be in air.
                if (ActiveSquad.CurrentLeader.ListTerrainChoices.Contains(UnitStats.TerrainAir))
                {
                    ActiveSquad.CurrentMovement = UnitStats.TerrainAir;
                    ActiveSquad.IsFlying = true;
                }
                //Can be on land.
                else if (ActiveSquad.CurrentLeader.ListTerrainChoices.Contains(UnitStats.TerrainLand))
                {
                    ActiveSquad.CurrentMovement = UnitStats.TerrainLand;
                    ActiveSquad.IsFlying = false;
                }
            }
        }

        public List<MovementAlgorithmTile> GetMVChoice(Squad ActiveSquad, BattleMap ActiveMap)
        {
            int StartingMV = GetSquadMaxMovement(ActiveSquad);//Maximum distance you can reach.

            //Init A star.
            List<MovementAlgorithmTile> ListAllNode = Pathfinder.FindPath(GetAllTerrain(ActiveSquad, ActiveMap), ActiveSquad, ActiveSquad.CurrentLeader.UnitStat, StartingMV);

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

        public List<MovementAlgorithmTile> GetAttackChoice(Squad ActiveSquad, int RangeMaximum)
        {
            BattleMap ActiveMap = this;
            if (ActivePlatform != null)
            {
                ActiveMap = ActivePlatform.Map;
            }

            //Init A star.
            List<MovementAlgorithmTile> ListAllNode = Pathfinder.FindPath(GetAllTerrain(ActiveSquad, ActiveMap), ActiveSquad, ActiveSquad.CurrentLeader.UnitStat, RangeMaximum);

            List<MovementAlgorithmTile> MovementChoice = new List<MovementAlgorithmTile>();

            for (int i = 0; i < ListAllNode.Count; i++)
            {
                ListAllNode[i].ParentTemp = null;//Unset parents
                ListAllNode[i].MovementCost = 0;

                if (ListAllNode[i].TerrainTypeIndex == UnitStats.TerrainWallIndex || ListAllNode[i].TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    continue;
                }

                MovementChoice.Add(ListAllNode[i]);
            }

            return MovementChoice;
        }

        public List<MovementAlgorithmTile> GetMVChoicesTowardPoint(Squad ActiveSquad, Vector3 Destination, bool IgnoreObstacles)
        {
            int MaxMovement = GetSquadMaxMovement(ActiveSquad);//Maximum distance you can reach.

            BattleMap ActiveMap = this;
            if (ActivePlatform != null)
            {
                ActiveMap = ActivePlatform.Map;
            }

            //Init A star.
            List<MovementAlgorithmTile> ListAllNode = Pathfinder.FindPath(GetAllTerrain(ActiveSquad, ActiveMap), ActiveSquad, ActiveSquad.CurrentLeader.UnitStat, MaxMovement, Destination, IgnoreObstacles);

            MovementAlgorithmTile ActiveNode = ListAllNode[ListAllNode.Count - 1];

            if (Destination.X != ActiveNode.WorldPosition.X || Destination.Y != ActiveNode.WorldPosition.Y || Destination.Z != ActiveNode.LayerIndex)
            {
                bool EmptyNodeFound = false;
                int Offset = 1;

                while (!EmptyNodeFound && Offset < 5)
                {
                    for (int i = ListAllNode.Count - 1; i >= 0 && !EmptyNodeFound; i--)
                    {
                        for (int X = -Offset; X <= Offset && !EmptyNodeFound; ++X)
                        {
                            for (int Y = -Offset; Y <= Offset && !EmptyNodeFound; ++Y)
                            {
                                if (ListAllNode[i].WorldPosition.X == Destination.X + X && ListAllNode[i].WorldPosition.Y == Destination.Y + Y && ListAllNode[i].LayerIndex == Destination.Z)
                                {
                                    ActiveNode = ListAllNode[i];
                                    EmptyNodeFound = true;
                                }
                            }
                        }
                    }

                    ++Offset;
                }

                if (!EmptyNodeFound)
                {
                    for (int i = ListAllNode.Count - 1; i >= 0; i--)
                    {
                        ListAllNode[i].ParentTemp = null;//Unset parents
                        ListAllNode[i].MovementCost = 0;
                    }

                    return ListAllNode;
                }
            }

            List<MovementAlgorithmTile> ListPathNode = new List<MovementAlgorithmTile>();

            while (ActiveNode != null)
            {
                if (ListPathNode.Contains(ActiveNode.ParentReal))
                {
                    break;
                }
                if (ActiveSquad.Position.X == ActiveNode.WorldPosition.X && ActiveSquad.Position.Y == ActiveNode.WorldPosition.Y && ActiveSquad.Position.Z == ActiveNode.LayerIndex)
                {
                    ListPathNode.Add(ActiveNode);
                    break;
                }
                if (ActiveNode.MovementCost <= MaxMovement)
                {
                    ListPathNode.Add(ActiveNode);
                }

                ActiveNode = ActiveNode.ParentReal;
            }

            for (int i = ListAllNode.Count - 1; i >= 0; i--)
            {
                ListAllNode[i].ParentTemp = null;//Unset parents
                ListAllNode[i].MovementCost = 0;
            }

            ListPathNode.Reverse();

            return ListPathNode;
        }

        public override MovementAlgorithmTile GetNextLayerIndex(MovementAlgorithmTile StartingPosition, int OffsetX, int OffsetY, float MaxClearance, float ClimbValue, out List<MovementAlgorithmTile> ListLayerPossibility)
        {
            ListLayerPossibility = new List<MovementAlgorithmTile>();
            int NextX = StartingPosition.InternalPosition.X + OffsetX;
            int NextY = StartingPosition.InternalPosition.Y + OffsetY;

            if (NextX < 0 || NextX >= MapSize.X || NextY < 0 || NextY >= MapSize.Y)
            {
                return null;
            }
            
            int CurrentTerrainType = StartingPosition.TerrainTypeIndex;
            float CurrentZ = StartingPosition.WorldPosition.Z;

            MovementAlgorithmTile ClosestLayerIndexDown = null;
            MovementAlgorithmTile ClosestLayerIndexUp = StartingPosition;
            float ClosestTerrainDistanceDown = float.MaxValue;
            float ClosestTerrainDistanceUp = float.MinValue;

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                MovementAlgorithmTile NextTerrain = GetTerrainIncludingPlatforms(StartingPosition, OffsetX, OffsetY, L);
                Terrain PreviousTerrain = LayerManager.ListLayer[L].ArrayTerrain[(int)StartingPosition.WorldPosition.X, (int)StartingPosition.WorldPosition.Y];

                if (L > StartingPosition.LayerIndex && PreviousTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
                {
                    break;
                }

                int NextTerrainType = NextTerrain.TerrainTypeIndex;
                float NextTerrainZ = NextTerrain.WorldPosition.Z;

                //Check lower or higher neighbors if on solid ground
                if (CurrentTerrainType != UnitStats.TerrainAirIndex && CurrentTerrainType != UnitStats.TerrainVoidIndex && CurrentTerrainType != UnitStats.TerrainWallIndex)
                {
                    if (NextTerrainType != UnitStats.TerrainAirIndex && NextTerrainType != UnitStats.TerrainVoidIndex && NextTerrainType != UnitStats.TerrainWallIndex)
                    {
                        //Prioritize going downward
                        if (NextTerrainZ <= CurrentZ)
                        {
                            float ZDiff = CurrentZ - NextTerrainZ;
                            if (ZDiff <= ClosestTerrainDistanceDown && HasEnoughClearance(NextTerrainZ, NextX, NextY, L, MaxClearance))
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
                                if (PreviousTerrain.TerrainTypeIndex == UnitStats.TerrainLandIndex)
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
                            ClosestLayerIndexUp = NextTerrain;
                            ListLayerPossibility.Add(NextTerrain);
                        }
                    }
                }
            }

            if (ClosestLayerIndexDown != null)
            {
                return ClosestLayerIndexDown;
            }
            else
            {
                return ClosestLayerIndexUp;
            }
        }

        public override MovementAlgorithmTile GetMovementTile(int X, int Y, int LayerIndex)
        {
            if (X < 0 || X >= MapSize.X || Y < 0 || Y >= MapSize.Y || LayerIndex < 0 || LayerIndex >= LayerManager.ListLayer.Count)
            {
                return null;
            }

            return LayerManager.ListLayer[LayerIndex].ArrayTerrain[X, Y];
        }

        public MovementAlgorithmTile GetTerrainIncludingPlatforms(MovementAlgorithmTile StartingPosition, int OffsetX, int OffsetY, int NextLayerIndex)
        {
            if (!IsAPlatform)
            {
                foreach (BattleMapPlatform ActivePlatform in ListPlatform)
                {
                    MovementAlgorithmTile FoundTile = ActivePlatform.FindTileFromLocalPosition(StartingPosition.InternalPosition.X + OffsetX, StartingPosition.InternalPosition.Y + OffsetY, NextLayerIndex);

                    if (FoundTile != null)
                    {
                        return FoundTile;
                    }
                }
            }

            return LayerManager.ListLayer[NextLayerIndex].ArrayTerrain[(int)StartingPosition.WorldPosition.X + OffsetX, (int)StartingPosition.WorldPosition.Y + OffsetY];
        }

        private bool HasEnoughClearance(float CurrentZ, int NextX, int NextY, int StartLayer, float MaxClearance)
        {
            for (int L = StartLayer + 1; L < LayerManager.ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = LayerManager.ListLayer[L];
                Terrain ActiveTerrain = ActiveLayer.ArrayTerrain[NextX, NextY];

                string NextTerrainType = GetTerrainType(NextX, NextY, L);
                float NextTerrainZ = ActiveTerrain.WorldPosition.Z;

                float ZDiff = NextTerrainZ - CurrentZ;

                if (NextTerrainType != UnitStats.TerrainAir && NextTerrainType != UnitStats.TerrainVoid && ZDiff < MaxClearance)
                {
                    return false;
                }
            }

            return true;
        }

        public void FinalizeMovement(Squad ActiveSquad, int UsedMovement, List<Vector3> ListMVHoverPoints)
        {
            if (ActiveSquad.CurrentMovement != UnitStats.TerrainAir && GetTerrainType(ActiveSquad.X, ActiveSquad.Y, (int)ActiveSquad.Position.Z) != UnitStats.TerrainAir)
            {
                ActiveSquad.CurrentMovement = GetTerrainType(ActiveSquad.X, ActiveSquad.Y, (int)ActiveSquad.Position.Z);
            }
            
            if (UsedMovement > 0)
            {
                if (ActiveSquad.CurrentMovement == UnitStats.TerrainAir)
                {
                    ActiveSquad.CurrentLeader.ConsumeEN(1);
                    if (ActiveSquad.CurrentWingmanA != null)
                        ActiveSquad.CurrentWingmanA.ConsumeEN(1);
                    if (ActiveSquad.CurrentWingmanB != null)
                        ActiveSquad.CurrentWingmanB.ConsumeEN(1);
                }
            }

            if (ListMVHoverPoints.Count > 0)
            {
                BaseMapLayer ActiveLayer = LayerManager[(int)ActiveSquad.Position.Z];

                foreach (InteractiveProp ActiveProp in ActiveLayer.ListProp)
                {
                    ActiveProp.FinishMoving(ActiveSquad, ListMVHoverPoints);
                }

                foreach (Core.Attacks.TemporaryAttackPickup ActiveAttack in ActiveLayer.ListAttackPickup)
                {
                    if (ListMVHoverPoints.Contains(ActiveAttack.Position))
                    {
                        ActiveSquad.CurrentLeader.AddTemporaryAttack(ActiveAttack, Content, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget);
                    }
                }

                for (int I = ActiveLayer.ListHoldableItem.Count - 1; I >= 0; I--)
                {
                    HoldableItem ActiveItem = ActiveLayer.ListHoldableItem[I];
                    foreach (Vector3 MovedOverPoint in ListMVHoverPoints)
                    {
                        ActiveItem.OnMovedOverBeforeStop(ActiveSquad, MovedOverPoint, ActiveSquad.Position);
                    }

                    ActiveItem.OnUnitStop(ActiveSquad);
                }
            }

            ActivateAutomaticSkills(ActiveSquad, ActiveSquad.CurrentLeader, BaseSkillRequirement.AfterMovingRequirementName, ActiveSquad, ActiveSquad.CurrentLeader);
            UpdateMapEvent(EventTypeUnitMoved, 0);
        }

        public override byte[] GetSnapshotData()
        {
            ByteWriter BW = new ByteWriter();

            BW.AppendInt32(ListAllPlayer.Count);
            foreach (Player ActivePlayer in ListAllPlayer)
            {
                BW.AppendString(ActivePlayer.ConnectionID);
                BW.AppendString(ActivePlayer.Name);
                BW.AppendInt32(ActivePlayer.Team);
                BW.AppendBoolean(ActivePlayer.IsPlayerControlled);
                BW.AppendByte(ActivePlayer.Color.R);
                BW.AppendByte(ActivePlayer.Color.G);
                BW.AppendByte(ActivePlayer.Color.B);

                BW.AppendByte(ActivePlayer.LocalPlayerIndex);

                BW.AppendInt32(ActivePlayer.ListSquad.Count);
                foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                {
                    BW.AppendFloat(ActiveSquad.X);
                    BW.AppendFloat(ActiveSquad.Y);
                    BW.AppendFloat(ActiveSquad.Z);
                    BW.AppendBoolean(ActiveSquad.IsPlayerControlled);

                    BW.AppendInt32(ActiveSquad.UnitsInSquad);
                    for (int U = 0; U < ActiveSquad.UnitsInSquad; ++U)
                    {
                        Unit ActiveUnit = ActiveSquad.At(U);
                        BW.AppendString(ActiveUnit.UnitTypeName);
                        BW.AppendString(ActiveUnit.RelativePath);

                        BW.AppendInt32(ActiveUnit.ArrayCharacterActive.Length);
                        for (int C = 0; C < ActiveUnit.ArrayCharacterActive.Length; ++C)
                        {
                            BW.AppendString(ActiveUnit.ArrayCharacterActive[C].FullName);
                        }
                    }
                }
            }

            BW.AppendString(BattleMapPath);

            byte[] Data = BW.GetBytes();
            BW.ClearWriteBuffer();
            return Data;
        }

        public override void RemoveOnlinePlayer(string PlayerID, IOnlineConnection activePlayer)
        {

        }

        public bool IsActivePlayerLocal(int PlayerIndex)
        {
            return ListLocalPlayerInfo.Contains(ListPlayer[PlayerIndex]);
        }

        public override string ToString()
        {
            return "Deathmatch Mode";
        }
    }
}
