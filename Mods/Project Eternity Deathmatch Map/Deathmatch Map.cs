using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Roslyn;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.ControlHelper;
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
        
        private SpriteFont fntArial16;

        private List<GameScreen> ListNextAnimationScreen;
        public List<Player> ListPlayer;
        public List<DelayedAttack> ListDelayedAttack;
        public MovementAlgorithm Pathfinder;
        public List<MapLayer> ListLayer;

        public NonDemoScreen NonDemoScreen;
        public SpiritMenu SpiritMenu;
        public MapMenu BattleMapMenu;
        public UnitDeploymentScreen UnitDeploymentScreen;
        public DeathmatchContext GlobalDeathmatchContext;

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
            : this(new DeathmatchContext())
        {
        }

        public DeathmatchMap(DeathmatchContext GlobalBattleContext)
            : base()
        {
            this.GlobalBattleContext = GlobalDeathmatchContext = GlobalBattleContext;
            GlobalDeathmatchContext.Map = this;

            FormulaParser.ActiveParser = new DeathmatchFormulaParser(this);
            ActivePlayerIndex = 0;
            ListPlayer = new List<Player>();
            RequireFocus = false;
            RequireDrawFocus = true;
            Pathfinder = new MovementAlgorithmDeathmatch(this);
            ListDelayedAttack = new List<DelayedAttack>();
            ListLayer = new List<MapLayer>();
            ListTerrainType = new List<string>();
            ListTerrainType.Add(UnitStats.TerrainAir);
            ListTerrainType.Add(UnitStats.TerrainLand);
            ListTerrainType.Add(UnitStats.TerrainSea);
            ListTerrainType.Add(UnitStats.TerrainSpace);
        }

        public DeathmatchMap(string BattleMapPath)
            : this(BattleMapPath, 0, new List<Squad>())
        {
            base.Init();
            ListActionMenuChoice.Add(new ActionPanelPhaseChange(this));
        }

        public DeathmatchMap(string BattleMapPath, int GameMode, List<Squad> ListSpawnSquad)
            : this()
        {
            this.BattleMapPath = BattleMapPath;
            this.GameMode = GameMode;
            this.ListSpawnSquad = ListSpawnSquad;

            CursorPosition = new Vector3(9, 13, 0);
            CursorPositionVisible = CursorPosition;

            ListNextAnimationScreen = new List<GameScreen>();

            ListTileSet = new List<Texture2D>();
            ListTilesetPreset = new List<Terrain.TilesetPreset>();
            ListSingleplayerSpawns = new List<EventPoint>();
            ListMultiplayerSpawns = new List<EventPoint>();
            ListMapSwitchPoint = new List<MapSwitchPoint>();
            CameraPosition = Vector3.Zero;
            ActiveSquadIndex = -1;
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
            if (PlayerRoster == null)
            {
                PlayerRoster = new Roster();
                PlayerRoster.LoadRoster();
            }

            base.Load();
            LoadPreBattleMenu();
            LoadMap();
            LoadMapAssets();
            LoadDeathmatchAIScripts();

            Dictionary<string, CutsceneScript> DeathmatchMapScripts = CutsceneScriptHolder.LoadAllScripts(typeof(DeathmatchMapCutsceneScriptHolder), this);
            foreach (CutsceneScript ActiveListScript in DeathmatchMapScripts.Values)
            {
                DicCutsceneScript.Add(ActiveListScript.Name, ActiveListScript);
            }

            SpiritMenu = new SpiritMenu(this);
            SpiritMenu.Load();

            BattleMapMenu = new MapMenu(this);
            BattleMapMenu.Load(Content, FMODSystem);

            UnitDeploymentScreen = new UnitDeploymentScreen(PlayerRoster);
            UnitDeploymentScreen.Load(Content);

            NonDemoScreen = new NonDemoScreen(this);
            NonDemoScreen.Load();

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntArial16 = Content.Load<SpriteFont>("Fonts/Arial16");
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

            LoadSpawns(BR);

            ListMapScript = MapScript.LoadMapScripts(BR, DicMapEvent, DicMapCondition, DicMapTrigger, out ListMapEvent);

            LoadTilesets(BR);

            LoadMapGrid(BR);

            BR.Close();
            FS.Close();
        }
        
        protected void LoadMapGrid(BinaryReader BR)
        {
            int LayerCount = BR.ReadInt32();

            for (int i = 0; i < LayerCount; ++i)
            {
                ListLayer.Add(new MapLayer(this, BR));
            }
        }

        public override void LoadEffects()
        {
            base.LoadEffects();

            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(DeathmatchEffect), new DeathmatchParams(GlobalDeathmatchContext)))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(DeathmatchEffect), new DeathmatchParams(GlobalDeathmatchContext)))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
        }

        public override void LoadSkillRequirements()
        {
            base.LoadSkillRequirements();

            Dictionary<string, BaseSkillRequirement> DicRequirementCore = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(UnitSkillRequirement), GlobalDeathmatchContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCore)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementCoreAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(UnitSkillRequirement), GlobalDeathmatchContext);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCoreAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }
        }

        public override void LoadAutomaticSkillActivation()
        {
            base.LoadAutomaticSkillActivation();

            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(AutomaticSkillTargetType), GlobalDeathmatchContext))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(AutomaticSkillTargetType), GlobalDeathmatchContext))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }
        }

        protected override void LoadManualSkillActivation()
        {
            base.LoadManualSkillActivation();

            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), GlobalDeathmatchContext))
            {
                DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssembly(ActiveAssembly, GlobalDeathmatchContext))
                {
                    DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }
        }

        protected override void LoadUnits()
        {
            base.LoadUnits();
            
            foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssemblyFiles(Directory.GetFiles("Units/Deathmatch Map", "*.dll"), this))
            {
                DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
            }

            var ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Units/Deathmatch Map", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
                }
            }
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

        public override void TogglePreview(bool UsePreview)
        {
            foreach (MapLayer ActiveMapLayer in ListLayer)
            {
                ActiveMapLayer.LayerGrid.TogglePreview(UsePreview);
            }

            if (!UsePreview)
            {
                //Reset game
                if (IsInit)
                {
                    Init();
                }
            }
        }

        public Terrain GetTerrain(float X, float Y, int LayerIndex)
        {
            return ListLayer[LayerIndex].ArrayTerrain[(int)X, (int)Y];
        }

        public DrawableTile GetTile(float X, float Y, int LayerIndex)
        {
            return ListLayer[LayerIndex].OriginalLayerGrid.GetTile((int)X, (int)Y);
        }

        public Terrain GetTerrain(UnitMapComponent ActiveUnit)
        {
            return GetTerrain(ActiveUnit.X, ActiveUnit.Y, ActiveUnit.LayerIndex);
        }

        public DrawableTile GetTile(UnitMapComponent ActiveUnit)
        {
            return GetTile(ActiveUnit.X, ActiveUnit.Y, ActiveUnit.LayerIndex);
        }

        public override void Update(GameTime gameTime)
        {
            if (GameMode == 1)
            {
                OnlinePlayers.Update();
            }

            if (!IsFrozen)
            {
                for (int B = 0; B < ListBackground.Count; ++B)
                {
                    ListBackground[B].Update(gameTime);
                }

                for (int F = 0; F < ListForeground.Count; ++F)
                {
                    ListForeground[F].Update(gameTime);
                }

                foreach (MapLayer ActiveMapLayer in ListLayer)
                {
                    ActiveMapLayer.Update(gameTime);
                }

                if (!IsOnTop)
                {
                    return;
                }

                if ((KeyboardHelper.KeyHold(Keys.LeftControl) || KeyboardHelper.KeyHold(Keys.RightControl)) && KeyboardHelper.KeyPressed(Keys.K))
                {
                    ListLayer[0].LayerGrid = new Map3D(this, GraphicsDevice);
                }
                if ((KeyboardHelper.KeyHold(Keys.LeftControl) || KeyboardHelper.KeyHold(Keys.RightControl)) && KeyboardHelper.KeyPressed(Keys.L))
                {
                    ListLayer[0].LayerGrid = new CubeMap3D(this, GraphicsDevice);
                }
                if ((KeyboardHelper.KeyHold(Keys.LeftControl) || KeyboardHelper.KeyHold(Keys.RightControl)) && KeyboardHelper.KeyPressed(Keys.O))
                {
                    ListLayer[0].LayerGrid = new SphericalMap3D(this, GraphicsDevice);
                }
                if ((KeyboardHelper.KeyHold(Keys.LeftControl) || KeyboardHelper.KeyHold(Keys.RightControl)) && KeyboardHelper.KeyPressed(Keys.U))
                {
                    ListLayer[0].LayerGrid = ListLayer[0].OriginalLayerGrid;
                }

                if (ListNextAnimationScreen.Count > 0)
                {
                    PushScreen(ListNextAnimationScreen[0]);
                    ListNextAnimationScreen.Remove(ListNextAnimationScreen[0]);
                }
                else
                {
                    if (!IsInit)
                    {
                        Init();
                    }
                    else if (MovementAnimation.Count > 0)
                    {
                        MoveSquad();
                    }
                    else if (GameMode == 0 || (GameMode == 1 && !ListPlayer[ActivePlayerIndex].IsOnline))
                    {
                        ListActionMenuChoice.Last().Update(gameTime);
                    }

                    UpdateCursorVisiblePosition(gameTime);
                }
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.BeginUnscaled(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

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

            g.End();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (!IsInit)
            {
                return;
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

            if (ListForeground.Count > 0)
            {
                g.End();
                for (int F = 0; F < ListForeground.Count; F++)
                {
                    ListForeground[F].Draw(g, Constants.Width, Constants.Height);
                }
                g.Begin();
            }

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
                if (ListPlayer[P].Team == UnitTeam)//Don't check your team.
                    continue;

                for (int U = 0; U < ListPlayer[P].ListSquad.Count; U++)
                {
                    if (ListPlayer[P].ListSquad[U].CurrentLeader == null)
                        continue;

                    CurrentUnit.UpdateAllAttacks(StartPosition, ListPlayer[P].ListSquad[U].Position,
                            ListPlayer[P].ListSquad[U].ArrayMapSize, ListPlayer[P].ListSquad[U].CurrentMovement, CanMove);
                }
            }
        }
        
        public int GetSquadMaxMovement(Squad ActiveSquad)
        {
            if (ActiveSquad.CurrentMovement == UnitStats.TerrainAir)
            {
                int StartingMV = Math.Min(ActiveSquad.CurrentLeader.MaxMovement, ActiveSquad.CurrentLeader.EN);//Maximum distance you can reach.

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

        public List<Vector3> GetMVChoice(Squad CurrentSquad)
        {
            int StartingMV = GetSquadMaxMovement(CurrentSquad);//Maximum distance you can reach.

            StartingMV += CurrentSquad.CurrentLeader.Boosts.MovementModifier;
            
            for (int X = -StartingMV; X <= StartingMV; X++)
            {
                for (int Y = -StartingMV; Y <= StartingMV; Y++)
                {
                    if (CurrentSquad.X + X < 0 || CurrentSquad.X + X >= MapSize.X || CurrentSquad.Y + Y < 0 || CurrentSquad.Y + Y >= MapSize.Y)
                        continue;

                    GetTerrain(CurrentSquad.X + X, CurrentSquad.Y + Y, CurrentSquad.LayerIndex).Parent = null;

                    bool UnitFound = false;

                    for (int P = 0; P < ListPlayer.Count && !UnitFound; P++)
                    {//Only check for enemies, can move through allies, can't move through ennemies.
                        if (ListPlayer[P].Team == ListPlayer[ActivePlayerIndex].Team)
                            continue;

                        //Check if there's a Unit.
                        //If a Unit was found.
                        if (CheckForObstacleAtPosition(P, CurrentSquad.Position, new Vector3(X, Y, 0)))
                            UnitFound = true;
                    }
                    //If there is an enemy Unit.
                    if (UnitFound)
                        GetTerrain(CurrentSquad.X + X, CurrentSquad.Y + Y, CurrentSquad.LayerIndex).MovementCost = -1;//Make it impossible to go there.
                    else
                        GetTerrain(CurrentSquad.X + X, CurrentSquad.Y + Y, CurrentSquad.LayerIndex).MovementCost = 0;
                }
            }

            //Init A star.
            List<MovementAlgorithmTile> ListAllNode = Pathfinder.FindPath(GetTerrain(CurrentSquad.X, CurrentSquad.Y, CurrentSquad.LayerIndex), CurrentSquad, CurrentSquad.CurrentLeader.UnitStat, StartingMV);

            List<Vector3> MovementChoice = new List<Vector3>();

            for (int i = 0; i < ListAllNode.Count; i++)
            {
                bool UnitFound = false;
                for (int P = 0; P < ListPlayer.Count && !UnitFound; P++)
                {
                    int SquadIndex = CheckForSquadAtPosition(P, ListAllNode[i].Position, Vector3.Zero);
                    if (SquadIndex >= 0)
                        UnitFound = true;
                }
                //If there is no Unit.
                if (!UnitFound)
                    MovementChoice.Add(ListAllNode[i].Position);
            }
            
            return MovementChoice;
        }

        public void FinalizeMovement(Squad CurrentSquad, int UsedMovement)
        {
            if (CurrentSquad.CurrentMovement != UnitStats.TerrainAir && GetTerrainType(CurrentSquad.X, CurrentSquad.Y, CurrentSquad.LayerIndex) != UnitStats.TerrainAir)
            {
                CurrentSquad.CurrentMovement = GetTerrainType(CurrentSquad.X, CurrentSquad.Y, CurrentSquad.LayerIndex);
            }
            
            if (UsedMovement > 0)
            {
                if (CurrentSquad.CurrentMovement == UnitStats.TerrainAir)
                {
                    CurrentSquad.CurrentLeader.ConsumeEN((int)GetTerrain(CurrentSquad).MovementCost);
                    if (CurrentSquad.CurrentWingmanA != null)
                        CurrentSquad.CurrentWingmanA.ConsumeEN((int)GetTerrain(CurrentSquad).MovementCost);
                    if (CurrentSquad.CurrentWingmanB != null)
                        CurrentSquad.CurrentWingmanB.ConsumeEN((int)GetTerrain(CurrentSquad).MovementCost);
                }
            }
            ActivateAutomaticSkills(CurrentSquad, CurrentSquad.CurrentLeader, BaseSkillRequirement.AfterMovingRequirementName);
            UpdateMapEvent(EventTypeUnitMoved, 0);
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

        public override void RemoveOnlinePlayer(string PlayerID, IOnlineConnection activePlayer)
        {

        }

        public override string ToString()
        {
            return "Deathmatch Mode";
        }
    }
}
