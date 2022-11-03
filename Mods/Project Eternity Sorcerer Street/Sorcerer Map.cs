﻿using System;
using System.IO;
using System.Linq;
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
        public enum Checkpoints { North, South, West, East }
        public override MovementAlgorithmTile CursorTerrain { get { return LayerManager.ListLayer[(int)CursorPosition.Z].ArrayTerrain[(int)CursorPosition.X, (int)CursorPosition.Y]; } }

        #region Ressources

        public Texture2D sprCardBack;

        public Texture2D sprPlayerBackground;
        public Texture2D sprArrowUp;

        public Texture2D sprTerritory;
        public Texture2D sprMap;
        public Texture2D sprInfo;
        public Texture2D sprOptions;
        public Texture2D sprHelp;
        public Texture2D sprEndTurn;

        public Texture2D sprElementAir;
        public Texture2D sprElementEarth;
        public Texture2D sprElementFire;
        public Texture2D sprElementWater;
        public Texture2D sprElementNeutral;
        public Texture2D sprElementMulti;

        public Texture2D sprMenuG;
        public Texture2D sprMenuTG;
        public Texture2D sprMenuST;
        public Texture2D sprMenuHP;
        public Texture2D sprMenuMHP;

        public Texture2D sprMenuHand;
        public Texture2D sprMenuCursor;
        public Texture2D sprVS;

        public Texture2D sprDirectionNorth;
        public Texture2D sprDirectionEast;
        public Texture2D sprDirectionWest;
        public Texture2D sprDirectionSouth;
        public Texture2D sprDirectionNorthFilled;
        public Texture2D sprDirectionEastFilled;
        public Texture2D sprDirectionWestFilled;
        public Texture2D sprDirectionSouthFilled;

        public Texture2D sprRarityE;
        public Texture2D sprRarityN;
        public Texture2D sprRarityR;
        public Texture2D sprRarityS;

        public Texture2D sprTileBorderEmpty;
        public Texture2D sprTileBorderColor;

        #endregion

        public readonly Vector3 LastPosition;
        private List<Player> ListLocalPlayerInfo;
        public List<Player> ListPlayer;
        public List<Player> ListLocalPlayer { get { return ListLocalPlayerInfo; } }
        public List<Player> ListAllPlayer { get { return ListPlayer; } }
        public int MagicAtStart;
        public int MagicGainPerLap;
        public int TowerMagicGain;
        public int MagicGoal;
        public int HighestDieRoll;
        public List<Checkpoints> ListCheckpoint;
        public readonly MovementAlgorithm Pathfinder;
        public readonly SorcererStreetBattleContext GlobalSorcererStreetBattleContext;
        public readonly SorcererStreetBattleParams SorcererStreetParams;
        public readonly List<string> ListTerrainType = new List<string>();
        public LayerHolderSorcererStreet LayerManager;

        public SorcererStreetMap()
        {
            RequireDrawFocus = false;
            ListActionMenuChoice = new ActionPanelHolder();
            Pathfinder = new MovementAlgorithmSorcererStreet(this);
            ListPlayer = new List<Player>();
            ListLocalPlayerInfo = new List<Player>();

            ListTilesetPreset = new List<Terrain.TilesetPreset>();
            LayerManager = new LayerHolderSorcererStreet(this);
            MapEnvironment = new EnvironmentManagerSorcererStreet(this);
            ActiveParser = new SorcererStreetFormulaParser(this);
            ListCheckpoint = new List<Checkpoints>();

            CursorPosition = new Vector3(0, 0, 0);
            CursorPositionVisible = CursorPosition;
            ListTerrainType = new List<string>();

            ListTerrainType.Add("Not Assigned");
            ListTerrainType.Add(TerrainSorcererStreet.Castle);
            ListTerrainType.Add(TerrainSorcererStreet.FireElement);
            ListTerrainType.Add(TerrainSorcererStreet.WaterElement);
            ListTerrainType.Add(TerrainSorcererStreet.EarthElement);
            ListTerrainType.Add(TerrainSorcererStreet.AirElement);
            ListTerrainType.Add(TerrainSorcererStreet.NeutralElement);
            ListTerrainType.Add(TerrainSorcererStreet.MorphElement);
            ListTerrainType.Add(TerrainSorcererStreet.MultiElement);
            ListTerrainType.Add(TerrainSorcererStreet.EastTower);
            ListTerrainType.Add(TerrainSorcererStreet.WestTower);
            ListTerrainType.Add(TerrainSorcererStreet.SouthTower);
            ListTerrainType.Add(TerrainSorcererStreet.NorthTower);
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

            GameRule = new SinglePlayerGameRule(this);
        }

        public SorcererStreetMap(string GameMode)
            : this()
        {
            Params = SorcererStreetParams = new SorcererStreetBattleParams();
            GlobalSorcererStreetBattleContext = SorcererStreetParams.GlobalContext;
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

            BW.Write(MagicAtStart);
            BW.Write(MagicGainPerLap);
            BW.Write(TowerMagicGain);
            BW.Write(MagicGoal);
            BW.Write(HighestDieRoll);

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

            sprCardBack = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Back");

            sprArrowUp = Content.Load<Texture2D>("Sorcerer Street/Ressources/Arrow Up");

            sprTerritory = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Territory");
            sprMap = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Map");
            sprInfo = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Info");
            sprOptions = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Options");
            sprHelp = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Help");
            sprEndTurn = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/End");


            sprElementAir = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Air");
            sprElementEarth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Earth");
            sprElementFire = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Fire");
            sprElementWater = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Water");
            sprElementMulti = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Multi");
            sprElementNeutral = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Neutral");

            sprPlayerBackground = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Player Background");

            sprMenuG = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/G");
            sprMenuTG = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/TG");
            sprMenuST = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/ST");
            sprMenuHP = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/HP");
            sprMenuMHP = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/MHP");

            sprMenuHand = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Hand");
            sprMenuCursor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cursor");
            sprVS = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/VS");

            sprDirectionNorth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/South");
            sprDirectionWest = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/West");
            sprDirectionEast = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/East");
            sprDirectionSouth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/South");
            sprDirectionNorthFilled = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/South");
            sprDirectionWestFilled = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/West");
            sprDirectionEastFilled = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/East");
            sprDirectionSouthFilled = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/South Filled");

            sprRarityE = Content.Load<Texture2D>("Sorcerer Street/Ressources/Rarity/Rarity E");
            sprRarityN = Content.Load<Texture2D>("Sorcerer Street/Ressources/Rarity/Rarity N");
            sprRarityR = Content.Load<Texture2D>("Sorcerer Street/Ressources/Rarity/Rarity R");
            sprRarityS = Content.Load<Texture2D>("Sorcerer Street/Ressources/Rarity/Rarity S");

            sprTileBorderEmpty = Content.Load<Texture2D>("Sorcerer Street/Ressources/Tile Border Empty");
            sprTileBorderColor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Tile Border");

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

            MagicAtStart = BR.ReadInt32();
            MagicGainPerLap = BR.ReadInt32();
            TowerMagicGain = BR.ReadInt32();
            MagicGoal = BR.ReadInt32();
            HighestDieRoll = BR.ReadInt32();

            ListMapScript = MapScript.LoadMapScripts(BR, DicMapEvent, DicMapCondition, DicMapTrigger, out ListMapEvent);

            LoadTilesets(BR);

            LayerManager = new LayerHolderSorcererStreet(this, BR);

            MapEnvironment = new EnvironmentManagerSorcererStreet(BR, this);

            BR.Close();
            FS.Close();

            TogglePreview(BackgroundOnly);
        }

        public override void Init()
        {
            base.Init();
            foreach (Player ActivePlayer in ListPlayer)
            {
                if (ActivePlayer.Team >= 0 && ActivePlayer.Team < ListMultiplayerColor.Count)
                {
                    ActivePlayer.Color = ListMultiplayerColor[ActivePlayer.Team];
                }
            }

            GameRule.Init();

            if (IsClient && ListPlayer.Count > 0)
            {
                ListActionMenuChoice.Add(new ActionPanelPlayerDefault(this, ActivePlayerIndex));
            }

            OnNewTurn();
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

        public override void SharePlayer(BattleMapPlayer SharedPlayer, bool IsLocal)
        {
            /*Player NewPlayer = new Player(SharedPlayer);
            ListPlayer.Add(NewPlayer);

            if (IsLocal)
            {
                ListLocalPlayerInfo.Add(NewPlayer);
            }*/
        }

        protected override void DoAddLocalPlayer(BattleMapPlayer NewPlayer)
        {
            /*Player NewDeahtmatchPlayer = new Player(NewPlayer);

            ListPlayer.Add(NewDeahtmatchPlayer);
            ListLocalPlayerInfo.Add(NewDeahtmatchPlayer);*/
        }

        public void AddPlayer(Player NewPlayer)
        {
            if (Content != null)
            {
                NewPlayer.GamePiece.SpriteMap = Content.Load<Texture2D>("Units/Default");
                NewPlayer.GamePiece.Unit3DSprite = new UnitMap3D(GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), NewPlayer.GamePiece.SpriteMap, 1);
                NewPlayer.GamePiece.Unit3DModel = new AnimatedModel("Units/Normal/Models/Bomberman/Default");
                NewPlayer.GamePiece.Unit3DModel.LoadContent(Content);
                NewPlayer.GamePiece.Unit3DModel.AddAnimation("Units/Normal/Models/Bomberman/Waving", "Idle", Content);
                NewPlayer.GamePiece.Unit3DModel.AddAnimation("Units/Normal/Models/Bomberman/Walking", "Walking", Content);
                NewPlayer.GamePiece.Unit3DModel.PlayAnimation("Walking");
            }

            ListPlayer.Add(NewPlayer);
            NewPlayer.TotalMagic = NewPlayer.Magic = MagicAtStart;
            UpdatePlayersRank();
        }

        public override void SetMutators(List<Mutator> ListMutator)
        {
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
            ListActionMenuChoice.RemoveAllActionPanels();
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

            ListActionMenuChoice.Add(new ActionPanelPlayerDefault(this, ActivePlayerIndex));
        }

        public void UpdateTolls(Player ActivePlayer)
        {
            ActivePlayer.TotalMagic = ActivePlayer.Magic;
            for (int X = MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = MapSize.Y - 1; Y >= 0; --Y)
                {
                    TerrainSorcererStreet ActiveTerrain = GetTerrain(X, Y, 0);
                    if (ActiveTerrain.PlayerOwner == ActivePlayer && ActiveTerrain.DefendingCreature != null)
                    {
                        ActiveTerrain.UpdateValue(ActivePlayer.DicChainLevelByTerrainTypeIndex[ActiveTerrain.TerrainTypeIndex], ActiveTerrain.DefendingCreature);
                        ActivePlayer.TotalMagic += ActiveTerrain.CurrentValue;
                    }
                }
            }

            UpdatePlayersRank();
        }

        public void UpdatePlayersRank()
        {
            List<Player> SortedList = ListPlayer.OrderBy(P => P.TotalMagic).ThenBy(P => ListPlayer.IndexOf(P)).ToList();

            for (int P = 0; P < SortedList.Count; ++P)
            {
                SortedList[P].Rank = P + 1;
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

        public override bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement)
        {
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            SorcererStreetParams.Map = this;

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

            g.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1, 0);

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

        public TerrainSorcererStreet GetTerrain(Vector3 Position)
        {
            return LayerManager.ListLayer[(int)Position.Z].ArrayTerrain[(int)Position.X, (int)Position.Y];
        }

        public TerrainSorcererStreet GetTerrain(int X, int Y, int LayerIndex)
        {
            return LayerManager.ListLayer[LayerIndex].ArrayTerrain[X, Y];
        }

        public TerrainSorcererStreet GetTerrain(UnitMapComponent ActiveUnit)
        {
            return GetTerrain((int)ActiveUnit.X, (int)ActiveUnit.Y, (int)ActiveUnit.Z);
        }

        public override MovementAlgorithmTile GetMovementTile(int X, int Y, int LayerIndex)
        {
            if (X < 0 || X >= MapSize.X || Y < 0 || Y >= MapSize.Y || LayerIndex < 0 || LayerIndex >= LayerManager.ListLayer.Count)
            {
                return null;
            }

            return GetTerrain(new Vector3(X, Y, LayerIndex));
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

            byte CurrentTerrainIndex = StartingPosition.TerrainTypeIndex;
            TerrainType CurrentTerrainType = TerrainRestrictions.ListTerrainType[CurrentTerrainIndex];

            float CurrentZ = StartingPosition.WorldPosition.Z;

            MovementAlgorithmTile ClosestLayerIndexDown = null;
            MovementAlgorithmTile ClosestLayerIndexUp = StartingPosition;
            float ClosestTerrainDistanceDown = float.MaxValue;
            float ClosestTerrainDistanceUp = float.MinValue;

            bool IsOnUsableTerrain = CurrentTerrainType.ListRestriction.Count > 0;

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                MovementAlgorithmTile NextTerrain = GetTerrainIncludingPlatforms(StartingPosition, OffsetX, OffsetY, L);
                byte NextTerrainIndex = NextTerrain.TerrainTypeIndex;
                TerrainType NextTerrainType = TerrainRestrictions.ListTerrainType[NextTerrainIndex];
                bool IsNextTerrainnUsable = NextTerrainType.ListRestriction.Count > 0 && NextTerrainType.ActivationName == CurrentTerrainType.ActivationName;

                Terrain PreviousTerrain = GetTerrain(new Vector3(StartingPosition.WorldPosition.X, StartingPosition.WorldPosition.Y, L));
                TerrainType PreviousTerrainType = TerrainRestrictions.ListTerrainType[PreviousTerrain.TerrainTypeIndex];
                bool IsPreviousTerrainnUsable = PreviousTerrainType.ListRestriction.Count > 0 && PreviousTerrainType.ActivationName == CurrentTerrainType.ActivationName;

                if (L > StartingPosition.LayerIndex && PreviousTerrainType.ListRestriction.Count == 0)
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

                if (TerrainRestrictions.ListTerrainType[NextTerrainType].ListRestriction.Count > 0 && ZDiff < MaxClearance)
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
            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = LayerManager.ListLayer[L];
                for (int S = 0; S < ActiveLayer.ListMultiplayerSpawns.Count; S++)
                {
                    if (ActiveLayer.ListMultiplayerSpawns[S].Tag == PlayerTag)
                    {
                        ListPossibleSpawnPoint.Add(GetTerrain(new Vector3(ActiveLayer.ListMultiplayerSpawns[S].Position.X, ActiveLayer.ListMultiplayerSpawns[S].Position.Y, L)));
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
