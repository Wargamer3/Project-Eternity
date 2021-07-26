﻿using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class DeathmatchMap : BattleMap
    {
        public override void Init()
        {
            base.Init();
            ListActionMenuChoice.Add(new ActionPanelPhaseChange(this));

            if (GameMode == 0)
            {
                ListPlayer.Clear();

                Player NewPlayer = new Player("Player", "Human", true, false, 0, Color.Blue);
                ListPlayer.Add(NewPlayer);

                if (ListSpawnSquad.Count > 0)
                {
                    int SpawnSquadIndex = 0;
                    for (int S = 0; S < ListSingleplayerSpawns.Count; S++)
                    {
                        if (ListSingleplayerSpawns[S].Tag == "P")
                        {
                            for (int U = 0; U < ListSpawnSquad[SpawnSquadIndex].UnitsInSquad; ++U)
                            {
                                ListSpawnSquad[SpawnSquadIndex].At(U).ReinitializeMembers(DicUnitType[ListSpawnSquad[SpawnSquadIndex].At(U).UnitTypeName]);
                            }
                            ListSpawnSquad[SpawnSquadIndex].ReloadSkills(DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                            SpawnSquad(0, ListSpawnSquad[SpawnSquadIndex], 0, ListSingleplayerSpawns[S].Position);
                            ++SpawnSquadIndex;
                        }
                    }
                }
            }
            else if (GameMode == 1)
            {
                for (int P = 0; P < ListPlayer.Count; P++)
                {
                    ListPlayer[P].Color = ArrayMultiplayerColor[P];

                    for (int S = 0; S < ListPlayer[P].ListSpawnPoint.Count; S++)
                    {
                        if (string.IsNullOrEmpty(ListPlayer[P].ListSpawnPoint[S].LeaderTypeName))
                            continue;

                        Unit NewLeaderUnit = Unit.FromType(ListPlayer[P].ListSpawnPoint[S].LeaderTypeName, ListPlayer[P].ListSpawnPoint[S].LeaderName, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                        Character NewLeaderPilot = new Character(ListPlayer[P].ListSpawnPoint[S].LeaderPilot, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                        NewLeaderPilot.Level = 1;
                        NewLeaderUnit.ArrayCharacterActive = new Character[1] { NewLeaderPilot };

                        Unit NewWingmanAUnit = null;
                        Unit NewWingmanBUnit = null;

                        if (!string.IsNullOrEmpty(ListPlayer[P].ListSpawnPoint[S].WingmanAName))
                        {
                            NewWingmanAUnit = Unit.FromType(ListPlayer[P].ListSpawnPoint[S].WingmanATypeName, ListPlayer[P].ListSpawnPoint[S].WingmanAName, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                            Character NewWingmanAPilot = new Character(ListPlayer[P].ListSpawnPoint[S].WingmanAPilot, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                            NewWingmanAPilot.Level = 1;
                            NewWingmanAUnit.ArrayCharacterActive = new Character[1] { NewWingmanAPilot };
                        }

                        if (!string.IsNullOrEmpty(ListPlayer[P].ListSpawnPoint[S].WingmanBName))
                        {
                            NewWingmanBUnit = Unit.FromType(ListPlayer[P].ListSpawnPoint[S].WingmanBTypeName, ListPlayer[P].ListSpawnPoint[S].WingmanBName, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                            Character NewWingmanBPilot = new Character(ListPlayer[P].ListSpawnPoint[S].WingmanBPilot, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                            NewWingmanBPilot.Level = 1;
                            NewWingmanBUnit.ArrayCharacterActive = new Character[1] { NewWingmanBPilot };
                        }

                        Squad NewSquad = new Squad("", NewLeaderUnit, NewWingmanAUnit, NewWingmanBUnit);

                        if (!ListPlayer[P].IsHuman)
                        {
                            NewSquad.SquadAI = new DeathmatchScripAIContainer(new DeathmatchAIInfo(this, NewSquad));
                            NewSquad.SquadAI.Load("SRWE Enemy AI");
                        }
                        else
                        {
                            NewSquad.IsPlayerControlled = true;
                        }

                        SpawnSquad(P, NewSquad, 0, ListPlayer[P].ListSpawnPoint[S].Position);
                    }
                }
            }
        }

        /// <summary>
        /// Used by Cutscene
        /// </summary>
        public void SpawnSquad(int PlayerIndex, Squad NewSquad, uint ID, Vector3 Position)
        {
            if (Content != null)
            {
                NewSquad.Unit3D = new UnitMap3D(GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), NewSquad.CurrentLeader.SpriteMap, 1);
            }

            while (ListPlayer.Count <= PlayerIndex)
            {
                Player NewPlayer = new Player("Enemy", "CPU", false, false, PlayerIndex, Color.Red);
                ListPlayer.Add(NewPlayer);
            }
            ListPlayer[PlayerIndex].IsAlive = true;

            NewSquad.Init(GlobalBattleContext);
            ActivateAutomaticSkills(NewSquad, string.Empty);
            NewSquad.ID = ID;
            NewSquad.SetPosition(Position);

            ListPlayer[PlayerIndex].ListSquad.Add(NewSquad);

            if (NewSquad.CurrentLeader.ListTerrainChoices.Contains(UnitStats.TerrainAir))
            {
                if (NewSquad.CurrentWingmanA != null)
                {
                    if (NewSquad.CurrentWingmanA.ListTerrainChoices.Contains(UnitStats.TerrainAir))
                    {
                        if (NewSquad.CurrentWingmanB != null)
                        {
                            if (NewSquad.CurrentWingmanB.ListTerrainChoices.Contains(UnitStats.TerrainAir))
                            {
                                NewSquad.IsFlying = true;
                                NewSquad.CurrentMovement = UnitStats.TerrainAir;
                            }
                        }
                        else
                        {
                            NewSquad.IsFlying = true;
                            NewSquad.CurrentMovement = UnitStats.TerrainAir;
                        }
                    }
                }
                else
                {
                    NewSquad.IsFlying = true;
                    NewSquad.CurrentMovement = UnitStats.TerrainAir;
                }
            }
            else
            {
                NewSquad.CurrentMovement = UnitStats.TerrainLand;
            }
        }
        
        public override void SaveTemporaryMap()
        {
            FileStream FS = new FileStream("TempSave.sav", FileMode.Create, FileAccess.Write);
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
            BW.Write(GameMode);
            BW.Write(GameTurn);
            BW.Write(VictoryCondition);
            BW.Write(LossCondition);
            BW.Write(SkillPoint);

            BW.Write(sndBattleThemeName);

            BW.Write(FMODSystem.sndActiveBGMName);
            BW.Write(FMODSystem.GetPosition(FMODSystem.sndActiveBGM));

            BW.Write(ListPlayer.Count);
            foreach (Player ActivePlayer in ListPlayer)
            {
                BW.Write(ActivePlayer.Name);
                BW.Write(ActivePlayer.PlayerType);
                BW.Write(ActivePlayer.IsHuman);
                BW.Write(ActivePlayer.Team);
                BW.Write(ActivePlayer.Color.R);
                BW.Write(ActivePlayer.Color.G);
                BW.Write(ActivePlayer.Color.B);

                BW.Write(ActivePlayer.ListSquad.Count);
                foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                {
                    BW.Write(ActiveSquad.ID);
                    BW.Write(ActiveSquad.CanMove);
                    BW.Write(ActiveSquad.ActionsRemaining);
                    BW.Write(ActiveSquad.X);
                    BW.Write(ActiveSquad.Y);
                    BW.Write(ActiveSquad.Z);
                    BW.Write(ActiveSquad.SquadName);
                    BW.Write(ActiveSquad.CurrentMovement);
                    BW.Write(ActiveSquad.IsFlying);
                    BW.Write(ActiveSquad.IsUnderTerrain);
                    BW.Write(ActiveSquad.IsPlayerControlled);
                    BW.Write(ActiveSquad.LayerIndex);
                    if (ActiveSquad.SquadAI == null || ActiveSquad.SquadAI.Path == null)
                    {
                        BW.Write(string.Empty);
                    }
                    else
                    {
                        BW.Write(ActiveSquad.SquadAI.Path);
                    }

                    int UnitsInSquad = ActiveSquad.UnitsInSquad;

                    BW.Write(UnitsInSquad);
                    BW.Write(ActiveSquad.CurrentLeaderIndex);
                    BW.Write(ActiveSquad.CurrentWingmanAIndex);
                    BW.Write(ActiveSquad.CurrentWingmanBIndex);

                    for (int U = 0; U < UnitsInSquad; ++U)
                        ActiveSquad.At(U).QuickSave(BW);

                    //List of Attacked Teams.
                    BW.Write(ActiveSquad.ListAttackedTeam.Count);
                    for (int U = 0; U < ActiveSquad.ListAttackedTeam.Count; ++U)
                        BW.Write(ActiveSquad.ListAttackedTeam[U]);
                }
            }

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int S = 0; S < ListPlayer[P].ListSquad.Count; S++)
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

            BW.Write(ListMapScript.Count);
            for (int S = 0; S < ListMapScript.Count; S++)
            {
                ListMapScript[S].Save(BW);
            }

            FS.Close();
            BW.Close();
        }

        public override BattleMap LoadTemporaryMap(BinaryReader BR)
        {
            LoadContent();

            ListActionMenuChoice.Add(new ActionPanelPhaseChange(this));
            //Initialise the ScreenSize based on the map loaded.
            ScreenSize = new Point(Constants.Width / TileSize.X, Constants.Height / TileSize.Y);

            IsStarted = true;
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
            GameMode = BR.ReadInt32();
            GameTurn = BR.ReadInt32();

            VictoryCondition = BR.ReadString();
            LossCondition = BR.ReadString();
            SkillPoint = BR.ReadString();

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
                    string ActiveSquadCurrentMovement = BR.ReadString();
                    bool ActiveSquadIsFlying = BR.ReadBoolean();
                    bool ActiveSquadIsUnderTerrain = BR.ReadBoolean();
                    bool ActiveSquadIsPlayerControlled = BR.ReadBoolean();
                    int ActiveSquadLayerIndex = BR.ReadInt32();
                    string ActiveSquadSquadAI = BR.ReadString();

                    int ActiveSquadUnitsInSquad = BR.ReadInt32();
                    int CurrentLeaderIndex = BR.ReadInt32();
                    int CurrentWingmanAIndex = BR.ReadInt32();
                    int CurrentWingmanBIndex = BR.ReadInt32();

                    Unit[] ArrayNewUnit = new Unit[ActiveSquadUnitsInSquad];
                    for (int U = 0; U < ActiveSquadUnitsInSquad; ++U)
                    {
                        string UnitAssemblyQualifiedName = BR.ReadString();
                        string UnitName = BR.ReadString();
                        ArrayNewUnit[U] = DicUnitType[UnitAssemblyQualifiedName].FromFile(UnitName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);

                        ArrayNewUnit[U].QuickLoad(BR, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
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
                            NewSquad.Unit3D = new UnitMap3D(GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), NewSquad.CurrentLeader.SpriteMap, 1);
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

                    NewSquad.CurrentMovement = ActiveSquadCurrentMovement;
                    NewSquad.IsFlying = ActiveSquadIsFlying;
                    NewSquad.IsUnderTerrain = ActiveSquadIsUnderTerrain;
                    NewSquad.IsPlayerControlled = ActiveSquadIsPlayerControlled;
                    NewSquad.SetLayerIndex(ActiveSquadLayerIndex);
                    NewSquad.SetPosition(new Vector3(ActiveSquadPositionX, ActiveSquadPositionY, ActiveSquadPositionZ));
                    NewPlayer.ListSquad.Add(NewSquad);
                }
            }

            GlobalQuickLoadContext.SetContext(DicLoadedSquad);

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int S = 0; S < ListPlayer[P].ListSquad.Count; S++)
                {
                    for (int U = 0; U < ListPlayer[P].ListSquad[S].UnitsInSquad; ++U)
                    {
                        for (int C = 0; C < ListPlayer[P].ListSquad[S].At(U).ArrayCharacterActive.Length; C++)
                        {
                            Character ActiveCharacter = ListPlayer[P].ListSquad[S].At(U).ArrayCharacterActive[C];
                            ActiveCharacter.Effects.QuickLoad(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                        }
                    }
                }
            }

            for (int P = 0; P < ListPlayer.Count; ++P)
            {
                for (int S = 0; S < ListPlayer[P].ListSquad.Count; ++S)
                {
                    ListPlayer[P].ListSquad[S].ReloadSkills(DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

                }
            }

            int ListMapScriptCount = BR.ReadInt32();
            if (ListMapScript.Count != ListMapScriptCount)
                throw new Exception("An error occured while loading the map.");

            for (int S = 0; S < ListMapScript.Count; S++)
            {
                ListMapScript[S].Load(BR);
            }

            return this;
        }

        public override GameScreen GetMultiplayerScreen()
        {
            return new MultiplayerScreen();
        }

        public override BattleMap GetNewMap(string BattleMapPath, int GameMode, List<Squad> ListSpawnSquad)
        {
            return new DeathmatchMap(BattleMapPath, GameMode, ListSpawnSquad);
        }

        public override string GetMapType()
        {
            return "Deathmatch";
        }
    }
}
