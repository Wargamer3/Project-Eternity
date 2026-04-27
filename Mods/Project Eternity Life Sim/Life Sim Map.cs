using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public partial class LifeSimMap : BattleMap, IProjectile3DSandbox
    {
        public LifeSimFormulaParser ActiveParser;
        public LifeSimBattleContext BattleContext;
        public MovementAlgorithm Pathfinder;
        public LayerHolderLifeSim LayerManager;
        public Dictionary<Vector3, Terrain> DicTemporaryTerrain;//Temporary obstacles
        public LifeSimParams SorcererStreetParams;

        public override MovementAlgorithmTile CursorTerrain => throw new NotImplementedException();

        public LifeSimMap()
        {
            RequireDrawFocus = false;
            ListActionMenuChoice = new ActionPanelHolderLifeSim(this);
            Pathfinder = new MovementAlgorithmSorcererStreet(this);
            BattleContext = new LifeSimBattleContext();

            ListTilesetPreset = new List<TilesetPreset>();
            LayerManager = new LayerHolderLifeSim(this);
            MapEnvironment = new EnvironmentManagerSorcererStreet(this);
            ActiveParser = new LifeSimFormulaParser(this, BattleContext);

            CursorPosition = new Vector3(0, 0, 0);
            CursorPositionVisible = CursorPosition;

            ListTileSet = new List<Texture2D>();
            this.Camera2DPosition = Vector3.Zero;
        }

        public LifeSimMap(GameModeInfo GameInfo)
            : this()
        {
            CursorPosition = new Vector3(9, 13, 0);
            CursorPositionVisible = CursorPosition;

            ListTileSet = new List<Texture2D>();
            ListTilesetPreset = new List<TilesetPreset>();
            Camera2DPosition = Vector3.Zero;

            if (GameInfo == null)
            {
                GameRule = new DeathmatchGameRule(this, new DeathmatchGameInfo(true, null));
            }
            else
            {
                GameRule = GameInfo.GetRule(this);
                if (GameRule == null)
                {
                    GameRule = new DeathmatchGameRule(this, new DeathmatchGameInfo(true, null));
                }
            }
        }

        public LifeSimMap(string BattleMapPath, GameModeInfo GameInfo)
            : this(GameInfo)
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
            base.Load();

            if (!IsServer)
            {

            }

            LoadMap();
            LoadMapAssets();

            Dictionary<string, CutsceneScript> ConquestScripts = CutsceneScriptHolder.LoadAllScripts(typeof(SorcererStreetMapCutsceneScriptHolder), this);
            foreach (CutsceneScript ActiveListScript in ConquestScripts.Values)
            {
                DicCutsceneScript.Add(ActiveListScript.Name, ActiveListScript);
            }
        }

        public override void Load(byte[] ArrayGameData)
        {
            ByteReader BR = new ByteReader(ArrayGameData);

            BR.Clear();
        }

        public void LoadMap(bool BackgroundOnly = false)
        {
            //Clear everything.
            ListTileSet = new List<Texture2D>();
            FileStream FS = new FileStream("Content/Life Sim/Maps/" + BattleMapPath + ".pem", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            //Map parameters.
            MapName = Path.GetFileNameWithoutExtension(BattleMapPath);

            LoadProperties(BR);

            ListMapScript = MapScript.LoadMapScripts(BR, DicMapEvent, DicMapCondition, DicMapTrigger, out ListMapEvent);

            LoadTilesets(BR);

            LayerManager = new LayerHolderLifeSim(this, BR);

            MapEnvironment = new EnvironmentManagerSorcererStreet(BR, this);

            BR.Close();
            FS.Close();

            TogglePreview(BackgroundOnly);
        }

        protected override TilesetPreset ReadTileset(string TilesetPresetPath, bool IsAutotile, int Index)
        {
            if (IsAutotile)
            {
                return TilesetPreset.FromFile("Life Sim/Autotiles Presets/" + TilesetPresetPath + ".peat", TilesetPresetPath, Index);
            }
            else
            {
                return TilesetPreset.FromFile("Life Sim/Tilesets Presets/" + TilesetPresetPath + ".pet", TilesetPresetPath, Index);
            }
        }

        protected override DestructibleTilesetPreset ReadDestructibleTilesetPreset(string TilesetPresetPath, int Index)
        {
            return DestructibleTilesetPreset.FromFile("Life Sim/Destroyable Tiles Presets/" + TilesetPresetPath + ".pedt", TilesetPresetPath, Index);
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

            if (!IsServer)
            {
                ListBackground.Clear();
                ListForeground.Clear();

                for (int B = 0; B < ListBackgroundsPath.Count; B++)
                {
                    ListBackground.Add(AnimationBackground.LoadAnimationBackground(ListBackgroundsPath[B], Content, GraphicsDevice));
                }

                for (int F = 0; F < ListForegroundsPath.Count; F++)
                {
                    ListForeground.Add(AnimationBackground.LoadAnimationBackground(ListForegroundsPath[F], Content, GraphicsDevice));
                }
            }

            LayerManager.TogglePreview(UsePreview);
            MapEnvironment.Reset();
        }

        public void Reset()
        {
            LayerManager.LayerHolderDrawable.Reset();
            MapEnvironment.Reset();
        }

        public override bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement)
        {
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            if (OnlineCommunicationClient != null)
            {
                OnlineCommunicationClient.ExecuteDelayedScripts();
            }

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
                    MovementAnimation.MoveSquad(gameTime, this, 2);
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

            if (!GameGroup.IsGameReady)
            {
                return;
            }

            if (!IsServer)
            {
                LayerManager.Update(UpdateTime);
            }

            ListActionMenuChoice.Last().Update(UpdateTime);
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.SetRenderTarget(null);
            g.BeginUnscaled(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

            LayerManager.BeginDraw(g);

            g.End();

            if (IsOnTop)
            {
                if (ListActionMenuChoice.HasMainPanel)
                {
                    ListActionMenuChoice.Last().BeginDraw(g);
                }
            }

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

            if (ListBackground.Count > 0 && Show3DObjects)
            {
                g.End();
                g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                g.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                g.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                for (int B = 0; B < ListBackground.Count; B++)
                {
                    ListBackground[B].Draw(g, Constants.Width, Constants.Height);
                    ListBackground[B].Draw3D(Camera3D, Matrix.CreateTranslation(new Vector3(0, 0, 0)));
                }
                g.Begin();
            }

            LayerManager.Draw(g);

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

        public Terrain GetTerrain(Vector3 GridPosition)
        {
            GridPosition = ConvertToGridPosition(GridPosition);

            if (GridPosition.X < 0 || GridPosition.X >= MapSize.X || GridPosition.Y < 0 || GridPosition.Y >= MapSize.Y || GridPosition.Z < 0 || GridPosition.Z >= LayerManager.ListLayer.Count)
            {
                return null;
            }

            Terrain TemporaryTerrain;
            if (DicTemporaryTerrain.TryGetValue(GridPosition, out TemporaryTerrain))
            {
                return TemporaryTerrain;
            }

            return LayerManager.ListLayer[(int)GridPosition.Z].ArrayTerrain[(int)GridPosition.X, (int)GridPosition.Y];
        }

        public Vector3 GetNextLayerTile(MovementAlgorithmTile StartingPosition, int GridOffsetX, int GridOffsetY, float MaxClearance, float ClimbValue, out List<MovementAlgorithmTile> ListLayerPossibility)
        {
            ListLayerPossibility = new List<MovementAlgorithmTile>();
            int NextX = StartingPosition.GridPosition.X + GridOffsetX;
            int NextY = StartingPosition.GridPosition.Y + GridOffsetY;

            if (NextX < 0 || NextX >= MapSize.X || NextY < 0 || NextY >= MapSize.Y)
            {
                return StartingPosition.WorldPosition;
            }

            byte CurrentTerrainIndex = StartingPosition.TerrainTypeIndex;

            float CurrentZ = StartingPosition.WorldPosition.Z;

            MovementAlgorithmTile ClosestLayerIndexDown = null;
            MovementAlgorithmTile ClosestLayerIndexUp = StartingPosition;
            float ClosestTerrainDistanceDown = float.MaxValue;
            float ClosestTerrainDistanceUp = float.MinValue;

            bool IsOnUsableTerrain = CurrentTerrainIndex > 0;

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                MovementAlgorithmTile NextTerrain = GetTerrainIncludingPlatforms(StartingPosition, GridOffsetX, GridOffsetY, L);
                byte NextTerrainIndex = NextTerrain.TerrainTypeIndex;
                bool IsNextTerrainnUsable = NextTerrainIndex > 0;

                Terrain PreviousTerrain = GetTerrain(new Vector3(StartingPosition.WorldPosition.X, StartingPosition.WorldPosition.Y, L));
                bool IsPreviousTerrainnUsable = PreviousTerrain.TerrainTypeIndex > 0;

                if (L > StartingPosition.LayerIndex && PreviousTerrain.TerrainTypeIndex == 0)//Void, abort
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

        public MovementAlgorithmTile GetTerrainIncludingPlatforms(MovementAlgorithmTile StartingPosition, int OffsetX, int OffsetY, int NextLayerIndex)
        {
            if (!IsAPlatform)
            {
                foreach (BattleMapPlatform ActivePlatform in ListPlatform)
                {
                    MovementAlgorithmTile FoundTile = ((LifeSimMap)ActivePlatform.Map).LayerManager.ListLayer[NextLayerIndex].ArrayTerrain[StartingPosition.GridPosition.X + OffsetX, StartingPosition.GridPosition.Y + OffsetY];

                    if (FoundTile != null)
                    {
                        return FoundTile;
                    }
                }
            }

            return GetTerrain(new Vector3(StartingPosition.WorldPosition.X + OffsetX, (int)StartingPosition.WorldPosition.Y + OffsetY, NextLayerIndex));
        }

        private bool HasEnoughClearance(float CurrentZ, int NextX, int NextY, int StartLayer, float MaxClearance)
        {
            for (int L = StartLayer + 1; L < LayerManager.ListLayer.Count; L++)
            {
                Terrain ActiveTerrain = GetTerrain(new Vector3(NextX, NextY, L));

                byte NextTerrainType = ActiveTerrain.TerrainTypeIndex;
                float NextTerrainZ = ActiveTerrain.WorldPosition.Z;

                float ZDiff = NextTerrainZ - CurrentZ;

                if (NextTerrainType > 0 && ZDiff < MaxClearance)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
