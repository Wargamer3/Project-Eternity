using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class DeathmatchMap : BattleMap
    {
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
                ListActionMenuChoice.Add(new ActionPanelPhaseChange(this));
            }

            OnNewTurn();
        }

        /// <summary>
        /// Used by Cutscene
        /// </summary>
        public void SpawnSquad(int PlayerIndex, Squad NewSquad, uint ID, Vector2 Position, int LayerIndex)
        {
            while (ListPlayer.Count <= PlayerIndex)
            {
                Player NewPlayer = new Player("Enemy", "CPU", false, false, PlayerIndex, Color.Red);
                ListPlayer.Add(NewPlayer);
            }
            if (Content != null)
            {
                NewSquad.Unit3DSprite = new UnitMap3D(GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), NewSquad.CurrentLeader.SpriteMap, 1);
                Color OutlineColor = ListPlayer[PlayerIndex].Color;
                NewSquad.Unit3DSprite.UnitEffect3D.Parameters["OutlineColor"].SetValue(new Vector4(OutlineColor.R / 255f, OutlineColor.G / 255f, OutlineColor.B / 255f, 1));
                NewSquad.Unit3DSprite.UnitEffect3D.Parameters["World"].SetValue(_World);
            }

            ListPlayer[PlayerIndex].IsAlive = true;

            NewSquad.Init(GlobalBattleParams.GlobalContext);
            ActivateAutomaticSkills(NewSquad, string.Empty);
            NewSquad.ID = ID;
            NewSquad.SetPosition(new Vector3(Position.X, Position.Y, LayerIndex));

            ListPlayer[PlayerIndex].ListSquad.Add(NewSquad);

            NewSquad.CurrentTerrainIndex = UnitStats.TerrainLandIndex;
        }

        public override void RemoveUnit(int PlayerIndex, UnitMapComponent UnitToRemove)
        {
            ListPlayer[ActivePlayerIndex].ListSquad.Remove((Squad)UnitToRemove);
            ListPlayer[ActivePlayerIndex].UpdateAliveStatus();
        }

        public override void AddUnit(int PlayerIndex, UnitMapComponent UnitToAdd, MovementAlgorithmTile NewPosition)
        {
            Squad ActiveSquad = (Squad)UnitToAdd;
            for (int U = 0; U < ActiveSquad.UnitsInSquad; ++U)
            {
                ActiveSquad.At(U).ReinitializeMembers(GlobalBattleParams.DicUnitType[ActiveSquad.At(U).UnitTypeName]);
            }

            ActiveSquad.ReloadSkills(GlobalBattleParams.DicUnitType, GlobalBattleParams.DicRequirement, GlobalBattleParams.DicEffect, GlobalBattleParams.DicAutomaticSkillTarget, GlobalBattleParams.DicManualSkillTarget);
            ListPlayer[PlayerIndex].ListSquad.Add(ActiveSquad);
            ListPlayer[PlayerIndex].UpdateAliveStatus();
            ActiveSquad.SetPosition(new Vector3(NewPosition.InternalPosition.X, NewPosition.InternalPosition.Y, NewPosition.LayerIndex));

            ActiveSquad.Unit3DSprite.UnitEffect3D.Parameters["World"].SetValue(_World);
        }

        public void AddProjectile(Projectile3D NewProjectile)
        {
        }

        public override List<MovementAlgorithmTile> GetSpawnLocations(int Team)
        {
            List<MovementAlgorithmTile> ListPossibleSpawnPoint = new List<MovementAlgorithmTile>();

            foreach(BattleMapPlatform ActivePlatform in ListPlatform)
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

        public override void SaveTemporaryMap()
        {
            FileStream FS = new FileStream("User Data/Saves/TempSave.sav", FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(BattleMapPath);
            BW.Write(typeof(DeathmatchMap).AssemblyQualifiedName);

            BW.Write(DicMapVariables.Count);
            foreach (KeyValuePair<string, double> Variables in DicMapVariables)
            {
                BW.Write(Variables.Key);
                BW.Write(Variables.Value);
            }

            BW.Write(CursorPosition.X);
            BW.Write(CursorPosition.Y);

            BW.Write(CameraPosition.X);
            BW.Write(CameraPosition.Y);

            BW.Write(ActivePlayerIndex);
            BW.Write(GameTurn);
            BW.Write(VictoryCondition);
            BW.Write(LossCondition);
            BW.Write(SkillPoint);

            BW.Write(ListBackground.Count);
            for (int B = 0; B < ListBackground.Count; ++B)
            {
                BW.Write(ListBackground[B].AnimationFullPath);
            }
            BW.Write(ListForeground.Count);
            for (int F = 0; F < ListForeground.Count; ++F)
            {
                BW.Write(ListForeground[F].AnimationFullPath);
            }

            BW.Write(sndBattleThemeName);

            BW.Write(FMODSystem.sndActiveBGMName);
            BW.Write(FMODSystem.GetPosition(FMODSystem.sndActiveBGM));

            BW.Write(ListPlayer.Count);
            foreach (Player ActivePlayer in ListPlayer)
            {
                BW.Write(ActivePlayer.Name);
                BW.Write(ActivePlayer.OnlinePlayerType);
                BW.Write(ActivePlayer.IsPlayerControlled);
                BW.Write(ActivePlayer.Team);
                BW.Write(ActivePlayer.Color.R);
                BW.Write(ActivePlayer.Color.G);
                BW.Write(ActivePlayer.Color.B);

                BW.Write(ActivePlayer.ListSquad.Count);
                foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                {
                    ActiveSquad.QuickSave(BW);
                }
            }

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int S = 0; S < ListPlayer[P].ListSquad.Count; S++)
                {
                    if (!ListPlayer[P].ListSquad[S].IsDead)
                    {
                        for (int U = 0; U < ListPlayer[P].ListSquad[S].UnitsInSquad; ++U)
                        {
                            for (int C = 0; C < ListPlayer[P].ListSquad[S].At(U).ArrayCharacterActive.Length; C++)
                            {
                                Character ActiveCharacter = ListPlayer[P].ListSquad[S].At(U).ArrayCharacterActive[C];
                                ActiveCharacter.Effects.QuickSave(BW);
                            }
                        }
                    }
                }
            }

            BW.Write(ListMapScript.Count);
            for (int S = 0; S < ListMapScript.Count; S++)
            {
                ListMapScript[S].Save(BW);
            }

            DataScreen.SaveProgression(BW, ListPlayer[0], PlayerRoster);

            FS.Close();
            BW.Close();
        }

        public override BattleMap LoadTemporaryMap(BinaryReader BR)
        {
            PlayerRoster = new Roster();
            PlayerRoster.LoadRoster();

            Load();

            //Initialise the ScreenSize based on the map loaded.
            ScreenSize = new Point(Constants.Width / TileSize.X, Constants.Height / TileSize.Y);

            IsInit = true;
            RequireDrawFocus = false;

            TogglePreview(true);

            int DicMapVariablesCount = BR.ReadInt32();
            DicMapVariables = new Dictionary<string, double>(DicMapVariablesCount);
            for (int i = 0; i < DicMapVariablesCount; ++i)
                DicMapVariables.Add(BR.ReadString(), BR.ReadDouble());

            CursorPosition.X = BR.ReadSingle();
            CursorPosition.Y = BR.ReadSingle();
            
            CameraPosition.X = BR.ReadSingle();
            CameraPosition.Y = BR.ReadSingle();

            ActivePlayerIndex = BR.ReadInt32();
            GameTurn = BR.ReadInt32();

            VictoryCondition = BR.ReadString();
            LossCondition = BR.ReadString();
            SkillPoint = BR.ReadString();

            ListBackground.Clear();
            int ListBackgroundCount = BR.ReadInt32();
            for (int B = 0; B < ListBackgroundCount; ++B)
            {
                ListBackground.Add(AnimationBackground.LoadAnimationBackground(BR.ReadString(), Content, GraphicsDevice));
            }

            ListForeground.Clear();
            int ListForegroundCount = BR.ReadInt32();
            for (int F = 0; F < ListForegroundCount; ++F)
            {
                ListForeground.Add(AnimationBackground.LoadAnimationBackground(BR.ReadString(), Content, GraphicsDevice));
            }

            sndBattleThemeName = BR.ReadString();
            if (!string.IsNullOrEmpty(sndBattleThemeName))
            {
                FMODSound NewBattleTheme = new FMODSound(FMODSystem, "Content/Maps/BGM/" + sndBattleThemeName + ".mp3");

                NewBattleTheme.SetLoop(true);
                sndBattleTheme = NewBattleTheme;
            }

            string ThemePath = BR.ReadString();
            uint ThemePosition = BR.ReadUInt32();

            if (!string.IsNullOrEmpty(ThemePath))
            {
                FMODSound NewTheme = new FMODSound(FMODSystem, "Content/Maps/BGM/" + ThemePath + ".mp3");
                NewTheme.SetLoop(true);
                NewTheme.PlayAsBGM();
                FMODSystem.sndActiveBGMName = ThemePath;
                NewTheme.SetPosition(ThemePosition);
            }

            Dictionary<uint, Squad> DicLoadedSquad = new Dictionary<uint, Squad>();

            int ListPlayerCount = BR.ReadInt32();
            ListPlayer = new List<Player>(ListPlayerCount);
            for (int P = 0; P < ListPlayerCount; ++P)
            {
                string ActivePlayerName = BR.ReadString();
                string ActivePlayerType = BR.ReadString();
                bool ActivePlayerIsHuman = BR.ReadBoolean();
                int ActivePlayerTeam = BR.ReadInt32();
                byte ActivePlayerColorRed = BR.ReadByte();
                byte ActivePlayerColorGreen = BR.ReadByte();
                byte ActivePlayerColorBlue = BR.ReadByte();

                Player NewPlayer = new Player(ActivePlayerName, ActivePlayerType, ActivePlayerIsHuman, false, ActivePlayerTeam,
                    Color.FromNonPremultiplied(ActivePlayerColorRed, ActivePlayerColorGreen, ActivePlayerColorBlue, 255));
                
                ListPlayer.Add(NewPlayer);

                int ActivePlayerListSquadCount = BR.ReadInt32();
                for (int S = 0; S < ActivePlayerListSquadCount; ++S)
                {
                    Squad NewSquad;
                    UInt32 ActiveSquadID = BR.ReadUInt32();
                    bool CanMove = BR.ReadBoolean();
                    int ActionsRemaining = BR.ReadInt32();
                    float ActiveSquadPositionX = BR.ReadSingle();
                    float ActiveSquadPositionY = BR.ReadSingle();
                    float ActiveSquadPositionZ = BR.ReadSingle();
                    string ActiveSquadSquadName = BR.ReadString();
                    byte ActiveSquadCurrentMovement = BR.ReadByte();
                    bool ActiveSquadIsUnderTerrain = BR.ReadBoolean();
                    bool ActiveSquadIsPlayerControlled = BR.ReadBoolean();
                    string ActiveSquadSquadAI = BR.ReadString();

                    int ActiveSquadUnitsInSquad = BR.ReadInt32();
                    int CurrentLeaderIndex = BR.ReadInt32();
                    int CurrentWingmanAIndex = BR.ReadInt32();
                    int CurrentWingmanBIndex = BR.ReadInt32();

                    Unit[] ArrayNewUnit = new Unit[ActiveSquadUnitsInSquad];
                    for (int U = 0; U < ActiveSquadUnitsInSquad; ++U)
                    {
                        string UnitTypeName = BR.ReadString();
                        string RelativePath = BR.ReadString();
                        string TeamEventID = BR.ReadString();

                        if (string.IsNullOrEmpty(TeamEventID))
                        {
                            ArrayNewUnit[U] = GlobalBattleParams.DicUnitType[UnitTypeName].FromFile(RelativePath, Content, GlobalBattleParams.DicRequirement, GlobalBattleParams.DicEffect, GlobalBattleParams.DicAutomaticSkillTarget);
                        }
                        else
                        {
                            foreach (Unit ActiveUnit in PlayerRoster.TeamUnits.GetAll())
                            {
                                if (ActiveUnit.ID == TeamEventID)
                                {
                                    ArrayNewUnit[U] = ActiveUnit;
                                    break;
                                }
                            }
                        }

                        ArrayNewUnit[U].QuickLoad(BR, Content, GlobalBattleParams.DicRequirement, GlobalBattleParams.DicEffect, GlobalBattleParams.DicAutomaticSkillTarget, GlobalBattleParams.DicManualSkillTarget);
                    }

                    NewSquad = new Squad(ActiveSquadSquadName, ArrayNewUnit[0],
                                         ArrayNewUnit.Length >= 2 ? ArrayNewUnit[1] : null,
                                         ArrayNewUnit.Length >= 3 ? ArrayNewUnit[2] : null);

                    int ListAttackedTeamCount = BR.ReadInt32();
                    NewSquad.ListAttackedTeam = new List<int>(ListAttackedTeamCount);
                    for (int U = 0; U < ListAttackedTeamCount; ++U)
                        NewSquad.ListAttackedTeam.Add(BR.ReadInt32());

                    NewSquad.SetLeader(CurrentLeaderIndex);
                    NewSquad.SetWingmanA(CurrentWingmanAIndex);
                    NewSquad.SetWingmanB(CurrentWingmanBIndex);

                    if (!CanMove)
                    {
                        NewSquad.EndTurn();
                    }

                    NewSquad.ActionsRemaining = ActionsRemaining;
                    NewSquad.SquadName = ActiveSquadSquadName;
                    NewSquad.ID = ActiveSquadID;

                    DicLoadedSquad.Add(ActiveSquadID, NewSquad);

                    if (NewSquad.CurrentLeader != null)
                    {
                        //Do not spawn squads as it will trigger effect that were already activated
                        if (Content != null)
                        {
                            NewSquad.Unit3DSprite = new UnitMap3D(GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), NewSquad.CurrentLeader.SpriteMap, 1);
                        }

                        if (!string.IsNullOrEmpty(ActiveSquadSquadAI))
                        {
                            NewSquad.SquadAI = new DeathmatchScripAIContainer(new DeathmatchAIInfo(this, NewSquad));
                            NewSquad.SquadAI.Load(ActiveSquadSquadAI);
                        }

                        NewPlayer.IsAlive = true;

                        ActivateAutomaticSkills(NewSquad, string.Empty);
                    }

                    NewSquad.UpdateSquad();

                    //Load the Battle Themes.
                    for (int U = 0; U < NewSquad.UnitsInSquad; ++U)
                    {
                        for (int C = NewSquad.At(U).ArrayCharacterActive.Length - 1; C >= 0; --C)
                            if (!string.IsNullOrEmpty(NewSquad.At(U).ArrayCharacterActive[C].BattleThemeName))
                                if (!Character.DicBattleTheme.ContainsKey(NewSquad.At(U).ArrayCharacterActive[C].BattleThemeName))
                                    Character.DicBattleTheme.Add(NewSquad.At(U).ArrayCharacterActive[C].BattleThemeName, new FMODSound(FMODSystem, "Content/Maps/BGM/" + NewSquad.At(U).ArrayCharacterActive[C].BattleThemeName + ".mp3"));
                    }

                    NewSquad.CurrentTerrainIndex = ActiveSquadCurrentMovement;
                    NewSquad.IsUnderTerrain = ActiveSquadIsUnderTerrain;
                    NewSquad.IsPlayerControlled = ActiveSquadIsPlayerControlled;
                    NewSquad.SetPosition(new Vector3(ActiveSquadPositionX, ActiveSquadPositionY, ActiveSquadPositionZ));
                    NewPlayer.ListSquad.Add(NewSquad);
                }
            }

            GlobalBattleParams.GlobalQuickLoadContext.SetContext(DicLoadedSquad);

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int S = 0; S < ListPlayer[P].ListSquad.Count; S++)
                {
                    if (!ListPlayer[P].ListSquad[S].IsDead)
                    {
                        for (int U = 0; U < ListPlayer[P].ListSquad[S].UnitsInSquad; ++U)
                        {
                            for (int C = 0; C < ListPlayer[P].ListSquad[S].At(U).ArrayCharacterActive.Length; C++)
                            {
                                Character ActiveCharacter = ListPlayer[P].ListSquad[S].At(U).ArrayCharacterActive[C];
                                ActiveCharacter.Effects.QuickLoad(BR, ActiveParser, GlobalBattleParams.DicRequirement, GlobalBattleParams.DicEffect, GlobalBattleParams.DicAutomaticSkillTarget);
                            }
                        }
                    }
                }
            }

            for (int P = 0; P < ListPlayer.Count; ++P)
            {
                for (int S = 0; S < ListPlayer[P].ListSquad.Count; ++S)
                {
                    ListPlayer[P].ListSquad[S].ReloadSkills(GlobalBattleParams.DicUnitType, GlobalBattleParams.DicRequirement, GlobalBattleParams.DicEffect, GlobalBattleParams.DicAutomaticSkillTarget, GlobalBattleParams.DicManualSkillTarget);

                }
            }

            int ListMapScriptCount = BR.ReadInt32();
            if (ListMapScript.Count != ListMapScriptCount)
                throw new Exception("An error occured while loading the map.");

            for (int S = 0; S < ListMapScript.Count; S++)
            {
                ListMapScript[S].Load(BR);
            }

            DataScreen.LoadProgression(BR, ListPlayer[0], PlayerRoster, GlobalBattleParams.DicUnitType, GlobalBattleParams.DicRequirement, GlobalBattleParams.DicEffect, GlobalBattleParams.DicAutomaticSkillTarget, GlobalBattleParams.DicManualSkillTarget);
            ListActionMenuChoice.Add(new ActionPanelPhaseChange(this));

            return this;
        }

        public override GameScreen GetMultiplayerScreen()
        {
            return new MultiplayerScreen();
        }

        public override BattleMap GetNewMap(string GameMode, string ParamsID)
        {
            DeathmatchParams Params;
            DeathmatchMap NewMap;

            if (!DeathmatchParams.DicParams.TryGetValue(ParamsID, out Params))
            {
                Params = new DeathmatchParams();
                Params.ID = ParamsID;
                DeathmatchParams.DicParams.TryAdd(ParamsID, Params);
                Params.Reload(this.Params, ParamsID);
            }

            NewMap = new DeathmatchMap(GameMode, Params);
            Params.Map = NewMap;
            return NewMap;
        }

        public override string GetMapType()
        {
            return MapType;
        }

        public override void AddPlatform(BattleMapPlatform NewPlatform)
        {
            foreach (Player ActivePlayer in ListPlayer)
            {
                NewPlatform.AddLocalPlayer(ActivePlayer);
            }

            ListPlatform.Add(NewPlatform);
        }

        public override void SetWorld(Matrix World)
        {
            this._World = World;
            LayerManager.LayerHolderDrawable.SetWorld(World);

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
                        LayerManager.ListLayer[Z].ArrayTerrain[X, Y].WorldPosition
                            = new Vector3((float)Math.Round(ArrayNewPosition[X + Y * MapSize.X].X / 32), (float)Math.Round(ArrayNewPosition[X + Y * MapSize.X].Z / 32), ArrayNewPosition[X + Y * MapSize.X].Y / 32);
                    }
                }
            }

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                //If the selected unit have the order to move, draw the possible positions it can go to.
                foreach (Squad ActiveSquad in ListPlayer[P].ListSquad)
                {
                    ActiveSquad.Unit3DSprite.UnitEffect3D.Parameters["World"].SetValue(World);
                }
            }
        }

        public override void ReplaceTile(int X, int Y, int LayerIndex, DrawableTile ActiveTile)
        {
            DrawableTile NewTile = new DrawableTile(ActiveTile);

            LayerManager.ListLayer[LayerIndex].LayerGrid.ReplaceTile(X, Y, NewTile);
            LayerManager.LayerHolderDrawable.Reset();
        }

        public override Dictionary<string, ActionPanel> GetOnlineActionPanel()
        {
            Dictionary<string, ActionPanel> DicActionPanel = new Dictionary<string, ActionPanel>();

            Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath("Mods/Project Eternity Deathmatch Map.dll"));
            Dictionary<string, BattleMapActionPanel> DicActionPanelMap = BattleMapActionPanel.LoadFromAssembly(ActiveAssembly, typeof(ActionPanelDeathmatch), this);
            foreach (KeyValuePair<string, BattleMapActionPanel> ActiveRequirement in DicActionPanelMap)
            {
                DicActionPanel.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            Dictionary<string, BattleMapActionPanel> DicActionPanelUnit = BattleMapActionPanel.LoadFromAssemblyFiles(Directory.GetFiles("Units/Deathmatch Map", "*.dll"), typeof(ActionPanelDeathmatch), this);
            foreach (KeyValuePair<string, BattleMapActionPanel> ActiveRequirement in DicActionPanelUnit)
            {
                DicActionPanel.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            return DicActionPanel;
        }
    }
}
