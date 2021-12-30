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
            GameRule.Init();

            if (IsClient)
            {
                ListActionMenuChoice.Add(new ActionPanelPhaseChange(this));
            }

            OnNewTurn();
        }

        /// <summary>
        /// Used by Cutscene
        /// </summary>
        public void SpawnSquad(int PlayerIndex, Squad NewSquad, uint ID, Vector3 Position, int LayerIndex)
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
            FileStream FS = new FileStream("User Data/Saves/TempSave.sav", FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(BattleMapPath);
            BW.Write(typeof(DeathmatchMap).AssemblyQualifiedName);

            DataScreen.SaveProgression(BW, PlayerRoster);

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

            FS.Close();
            BW.Close();
        }

        public override BattleMap LoadTemporaryMap(BinaryReader BR)
        {
            PlayerRoster = new Roster();
            PlayerRoster.LoadRoster();

            Load();
            DataScreen.LoadProgression(BR, PlayerRoster, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

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
                        string UnitTypeName = BR.ReadString();
                        string RelativePath = BR.ReadString();
                        string TeamEventID = BR.ReadString();

                        if (string.IsNullOrEmpty(TeamEventID))
                        {
                            ArrayNewUnit[U] = DicUnitType[UnitTypeName].FromFile(RelativePath, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                        }
                        else
                        {
                            foreach (Unit ActiveUnit in PlayerRoster.TeamUnits.GetAll())
                            {
                                if (ActiveUnit.TeamEventID == TeamEventID)
                                {
                                    ArrayNewUnit[U] = ActiveUnit;
                                    break;
                                }
                            }
                        }

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
                    if (!ListPlayer[P].ListSquad[S].IsDead)
                    {
                        for (int U = 0; U < ListPlayer[P].ListSquad[S].UnitsInSquad; ++U)
                        {
                            for (int C = 0; C < ListPlayer[P].ListSquad[S].At(U).ArrayCharacterActive.Length; C++)
                            {
                                Character ActiveCharacter = ListPlayer[P].ListSquad[S].At(U).ArrayCharacterActive[C];
                                ActiveCharacter.Effects.QuickLoad(BR, ActiveParser, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                            }
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

            ListActionMenuChoice.Add(new ActionPanelPhaseChange(this));

            return this;
        }

        public override GameScreen GetMultiplayerScreen()
        {
            return new MultiplayerScreen();
        }

        public override BattleMap GetNewMap(string GameMode)
        {
            return new DeathmatchMap(GameMode);
        }

        public override string GetMapType()
        {
            return "Deathmatch";
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
