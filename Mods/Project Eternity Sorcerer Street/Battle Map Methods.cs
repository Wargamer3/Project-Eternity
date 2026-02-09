using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public partial class SorcererStreetMap : BattleMap
    {
        public override void Init()
        {
            foreach (Player ActivePlayer in ListPlayer)
            {
                if (ActivePlayer.TeamIndex >= 0 && ActivePlayer.TeamIndex < ListMultiplayerColor.Count)
                {
                    ActivePlayer.Color = ListMultiplayerColor[ActivePlayer.TeamIndex];
                }
            }

            GameRule.Init();

            if (IsOnlineClient)
            {
                if (ListAllPlayer[0].OnlinePlayerType == OnlinePlayerBase.PlayerTypePlayer)
                {
                    ListActionMenuChoice.Add(new ActionPanelPlayerDefault(this));
                    ActionPanelDialogPhase.AddIntrodctionIfAvailable(this);
                }
            }
            else if (IsClient && ListPlayer.Count > 0)
            {
                ListActionMenuChoice.Add(new ActionPanelPlayerDefault(this));
                ActionPanelDialogPhase.AddIntrodctionIfAvailable(this);
                GlobalPlayerContext.SetPlayer(ActivePlayerIndex, ListPlayer[ActivePlayerIndex]);
            }

            base.Init();

            OnNewTurn();
        }

        /// <summary>
        /// Move the cursor on the map.
        /// </summary>
        /// <returns>Returns true if the cursor was moved</returns>
        public override bool CursorControl(GameTime gameTime, PlayerInput ActiveInputManager)
        {
            Point GridOffset;
            BattleMap ActiveMap;

            bool CursorMoved = CursorControlGrid(gameTime, ActiveInputManager, out GridOffset, out ActiveMap);

            if (CursorMoved)
            {
                Vector3 NextTerrain = GetNextLayerTile(ActiveMap.CursorTerrain, GridOffset.X, GridOffset.Y, 1f, 15f, out _);
                if (NextTerrain == ActiveMap.CursorTerrain.WorldPosition)//Force movement
                {
                    ActiveMap.CursorPosition.Z = NextTerrain.Z;
                    ActiveMap.CursorPosition.X = Math.Max(0, Math.Min((ActiveMap.MapSize.X - 1) * TileSize.X, NextTerrain.X + GridOffset.X * TileSize.X));
                    ActiveMap.CursorPosition.Y = Math.Max(0, Math.Min((ActiveMap.MapSize.Y - 1) * TileSize.Y, NextTerrain.Y + GridOffset.Y * TileSize.Y));
                }
                else
                {
                    ActiveMap.CursorPosition = NextTerrain;
                }

                foreach (TeleportPoint ActiveTeleport in LayerManager.ListLayer[(int)CursorPosition.Z].ListTeleportPoint)
                {
                    if (ActiveTeleport.Position.X == CursorPosition.X && ActiveTeleport.Position.Y == CursorPosition.Y)
                    {
                        CursorPosition.X = ActiveTeleport.OtherMapEntryPoint.X;
                        CursorPosition.Y = ActiveTeleport.OtherMapEntryPoint.Y;
                        CursorPosition.Z = ActiveTeleport.OtherMapEntryLayer;
                        break;
                    }
                }
            }

            if (ActiveInputManager.InputLButtonPressed())
            {
                if (ListPlayer[ActivePlayerIndex].ListSummonedCreature.Count == 0)
                    return CursorMoved;

                TerrainSorcererStreet ActiveSquad = null;
                int ActiveSquadIndex;

                int UnitIndex = 0;
                if (ActiveSquad != null)
                    UnitIndex = ListPlayer[ActivePlayerIndex].ListSummonedCreature.IndexOf(ActiveSquad);

                int StartIndex = UnitIndex;
                bool UnmovedSquadFound = false;

                do
                {
                    ++UnitIndex;

                    if (UnitIndex >= ListPlayer[ActivePlayerIndex].ListSummonedCreature.Count)
                        UnitIndex = 0;

                    if (!ListPlayer[ActivePlayerIndex].ListSummonedCreature[UnitIndex].DefendingCreature.GamePiece.IsActive && ListPlayer[ActivePlayerIndex].ListSummonedCreature[UnitIndex].DefendingCreature.GamePiece.CanMove)
                    {
                        UnmovedSquadFound = true;
                    }
                }
                while (StartIndex != UnitIndex && !UnmovedSquadFound);

                if (!UnmovedSquadFound)
                {
                    do
                    {
                        if (++UnitIndex >= ListPlayer[ActivePlayerIndex].ListSummonedCreature.Count)
                            UnitIndex = 0;
                    }
                    while (!ListPlayer[ActivePlayerIndex].ListSummonedCreature[UnitIndex].DefendingCreature.GamePiece.IsActive);
                }

                ActiveSquadIndex = UnitIndex;
                CursorPosition = ActiveSquad.WorldPosition;
                CursorPositionVisible = CursorPosition;

                if (ActiveSquad.WorldPosition.X < Camera2DPosition.X || ActiveSquad.WorldPosition.Y < Camera2DPosition.Y ||
                    ActiveSquad.WorldPosition.X >= Camera2DPosition.X + ScreenSize.X || ActiveSquad.WorldPosition.Y >= Camera2DPosition.Y + ScreenSize.Y)
                {
                    PushScreen(new CenterOnSquadCutscene(CenterCamera, this, ActiveSquad.WorldPosition));
                }
            }
            else if (ActiveInputManager.InputRButtonPressed())
            {
                if (ListPlayer[ActivePlayerIndex].ListSummonedCreature.Count == 0)
                    return CursorMoved;

                TerrainSorcererStreet ActiveSquad = null;
                int ActiveSquadIndex;

                int UnitIndex = 0;
                if (ActiveSquad != null)
                    UnitIndex = ListPlayer[ActivePlayerIndex].ListSummonedCreature.IndexOf(ActiveSquad);
                int StartIndex = UnitIndex;
                bool UnmovedSquadFound = false;

                do
                {
                    --UnitIndex;

                    if (UnitIndex < 0)
                        UnitIndex = ListPlayer[ActivePlayerIndex].ListSummonedCreature.Count - 1;

                    if (!ListPlayer[ActivePlayerIndex].ListSummonedCreature[UnitIndex].DefendingCreature.GamePiece.IsActive && ListPlayer[ActivePlayerIndex].ListSummonedCreature[UnitIndex].DefendingCreature.GamePiece.CanMove)
                    {
                        UnmovedSquadFound = true;
                    }
                }
                while (StartIndex != UnitIndex && !UnmovedSquadFound);

                if (!UnmovedSquadFound)
                {
                    do
                    {
                        if (--UnitIndex < 0)
                            UnitIndex = ListPlayer[ActivePlayerIndex].ListSummonedCreature.Count - 1;
                    }
                    while (ListPlayer[ActivePlayerIndex].ListSummonedCreature[UnitIndex].DefendingCreature.GamePiece.IsActive);
                }

                ActiveSquadIndex = UnitIndex;
                CursorPosition = ActiveSquad.WorldPosition;
                CursorPositionVisible = CursorPosition;

                if (ActiveSquad.WorldPosition.X < Camera2DPosition.X || ActiveSquad.WorldPosition.Y < Camera2DPosition.Y ||
                    ActiveSquad.WorldPosition.X >= Camera2DPosition.X + ScreenSize.X || ActiveSquad.WorldPosition.Y >= Camera2DPosition.Y + ScreenSize.Y)
                {
                    PushScreen(new CenterOnSquadCutscene(CenterCamera, this, ActiveSquad.WorldPosition));
                }
            }
            return CursorMoved;
        }
        
        public override Vector3 GetFinalPosition(Vector3 WorldPosition)
        {
            int GridX = (int)WorldPosition.X / TileSize.X;
            int GridY = (int)WorldPosition.Y / TileSize.Y;
            int LayerIndex = (int)WorldPosition.Z / LayerHeight;

            Terrain ActiveTerrain = LayerManager.ListLayer[LayerIndex].ArrayTerrain[GridX, GridY];
            DrawableTile ActiveTile = LayerManager.ListLayer[LayerIndex].ArrayTile[GridX, GridY];

            Vector2 PositionInTile = new Vector2(WorldPosition.X - ActiveTerrain.WorldPosition.X, WorldPosition.Y - ActiveTerrain.WorldPosition.Y);

            return WorldPosition + new Vector3(0, 0, ActiveTile.Terrain3DInfo.GetZOffset(PositionInTile, ActiveTerrain.Height));
        }

        public override Vector3 GetNextPosition(Vector3 WorldPosition, Vector3 Movement)
        {
            return GetFinalPosition(new Vector3(WorldPosition.X + Movement.X, WorldPosition.Y + Movement.Y, GetNextLayerTile(GetTerrain(WorldPosition), (int)(Movement.X / TileSize.X), (int)(Movement.Y / TileSize.Y), 1f, 15f, out _).Z));
        }

        public override Tile3D CreateTile3D(int TilesetIndex, Vector3 WorldPosition, Point Origin, Point TileSize, Point TextureSize, float PositionOffset)
        {
            Vector3 TopFrontLeft = GetFinalPosition(new Vector3(WorldPosition.X, WorldPosition.Y + TileSize.Y, WorldPosition.Z));
            Vector3 TopFrontRight = GetFinalPosition(new Vector3(WorldPosition.X + TileSize.X, WorldPosition.Y + TileSize.Y, WorldPosition.Z));
            Vector3 TopBackLeft = GetFinalPosition(new Vector3(WorldPosition.X, WorldPosition.Y, WorldPosition.Z));
            Vector3 TopBackRight = GetFinalPosition(new Vector3(WorldPosition.X + TileSize.X, WorldPosition.Y, WorldPosition.Z));

            return Terrain3D.CreateTile3D(TilesetIndex, TopFrontLeft, TopFrontRight, TopBackLeft, TopBackRight, TileSize, Origin, TextureSize.X, TextureSize.Y, PositionOffset);
        }

        public override void RemoveUnit(int PlayerIndex, object UnitToRemove)
        {
            /*ListPlayer[ActivePlayerIndex].ListSquad.Remove((SorcererStreetUnit)UnitToRemove);
            ListPlayer[ActivePlayerIndex].UpdateAliveStatus();*/
        }

        public override void AddUnit(int PlayerIndex, object UnitToAdd, Vector3 NewPosition)
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

        public override void SharePlayer(BattleMapPlayer SharedPlayer, bool IsLocal)
        {
            /*Player NewPlayer = new Player(SharedPlayer);
            ListPlayer.Add(NewPlayer);

            if (IsLocal)
            {
                ListLocalPlayerInfo.Add(NewPlayer);
            }*/
        }

        protected override void DoAddLocalPlayer(OnlinePlayerBase NewPlayer)
        {
            Player NewDeahtmatchPlayer = new Player((Player)NewPlayer, SorcererStreetParams);

            AddPlayer(NewDeahtmatchPlayer);
        }

        public override void RemoveOnlinePlayer(string PlayerID, IOnlineConnection ActivePlayer)
        {

        }

        public override byte[] GetSnapshotData()
        {
            ByteWriter BW = new ByteWriter();

            BW.AppendInt32(ListAllPlayer.Count);
            foreach (Player ActivePlayer in ListAllPlayer)
            {
                BW.AppendString(ActivePlayer.ConnectionID);
                BW.AppendString(ActivePlayer.Name);
                BW.AppendInt32(ActivePlayer.TeamIndex);
                BW.AppendBoolean(ActivePlayer.IsPlayerControlled);
                BW.AppendByte(ActivePlayer.Color.R);
                BW.AppendByte(ActivePlayer.Color.G);
                BW.AppendByte(ActivePlayer.Color.B);

                BW.AppendInt32(ActivePlayer.Gold);

                BW.AppendByte((byte)ActivePlayer.ListRemainingCardInDeck.Count);
                foreach (Card ActiveCard in ActivePlayer.ListRemainingCardInDeck)
                {
                    BW.AppendString(ActiveCard.CardType);
                    BW.AppendString(ActiveCard.Path);
                }

                BW.AppendByte((byte)ActivePlayer.ListCardInHand.Count);
                foreach (Card ActiveCard in ActivePlayer.ListCardInHand)
                {
                    BW.AppendString(ActiveCard.CardType);
                    BW.AppendString(ActiveCard.Path);
                }
            }

            BW.AppendString(BattleMapPath);

            byte[] Data = BW.GetBytes();
            BW.ClearWriteBuffer();
            return Data;
        }

        public void AddProjectile(Projectile3D NewProjectile)
        {
        }

        public override List<Vector3> GetCampaignEnemySpawnLocations()
        {
            List<Vector3> ListPossibleSpawnPoint = new List<Vector3>();

            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                ListPossibleSpawnPoint.AddRange(ActivePlatform.Map.GetCampaignEnemySpawnLocations());
            }

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = LayerManager.ListLayer[L];
                for (int S = 0; S < ActiveLayer.ListCampaignSpawns.Count; S++)
                {
                    if (ActiveLayer.ListCampaignSpawns[S].Tag == "E")
                    {
                        ListPossibleSpawnPoint.Add(new Vector3(ActiveLayer.ListMultiplayerSpawns[S].Position.X, ActiveLayer.ListMultiplayerSpawns[S].Position.Y, L));
                    }
                }
            }

            return ListPossibleSpawnPoint;
        }

        public override List<Vector3> GetMultiplayerSpawnLocations(int Team)
        {
            List<Vector3> ListPossibleSpawnPoint = new List<Vector3>();

            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                ListPossibleSpawnPoint.AddRange(ActivePlatform.Map.GetMultiplayerSpawnLocations(Team));
            }

            string PlayerTag = (Team + 1).ToString();
            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = LayerManager.ListLayer[L];
                for (int S = 0; S < ActiveLayer.ListMultiplayerSpawns.Count; S++)
                {
                    if (ActiveLayer.ListMultiplayerSpawns[S].Tag == PlayerTag)
                    {
                        ListPossibleSpawnPoint.Add(new Vector3(ActiveLayer.ListMultiplayerSpawns[S].Position.X, ActiveLayer.ListMultiplayerSpawns[S].Position.Y, L));
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

        public override BattleMap GetNewMap(GameModeInfo GameInfo, string ParamsID)
        {
            SorcererStreetBattleParams Params;
            SorcererStreetMap NewMap;

            if (!SorcererStreetBattleParams.DicParams.TryGetValue(ParamsID, out Params))
            {
                Params = new SorcererStreetBattleParams();
                Params.ActiveParser = new SorcererStreetFormulaParser(Params);
                SorcererStreetBattleParams.DicParams.TryAdd(ParamsID, Params);
                Params.Reload(this.Params, ParamsID);
            }

            NewMap = new SorcererStreetMap(GameInfo, Params);
            Params.Map = NewMap;
            return NewMap;
        }

        public override string GetMapType()
        {
            return "Sorcerer Street";
        }

        public override Dictionary<string, GameModeInfo> GetAvailableGameModes()
        {
            Dictionary<string, GameModeInfo> DicGameType = new Dictionary<string, GameModeInfo>();

            DicGameType.Add(DeathmatchGameInfo.ModeName, new DeathmatchGameInfo(true, null));

            return DicGameType;
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

        public override Dictionary<string, ActionPanel> GetOnlineActionPanel()
        {
            Dictionary<string, ActionPanel> DicActionPanel = new Dictionary<string, ActionPanel>();

            Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath("Mods/Project Eternity Sorcerer Street.dll"));
            Dictionary<string, BattleMapActionPanel> DicActionPanelMap = BattleMapActionPanel.LoadFromAssembly(ActiveAssembly, typeof(ActionPanelSorcererStreet), this);
            foreach (KeyValuePair<string, BattleMapActionPanel> ActiveRequirement in DicActionPanelMap)
            {
                DicActionPanel.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }
            DicActionPanelMap = BattleMapActionPanel.LoadFromAssembly(ActiveAssembly, typeof(ActionPanelCardSelectionPhase), this);
            foreach (KeyValuePair<string, BattleMapActionPanel> ActiveRequirement in DicActionPanelMap)
            {
                DicActionPanel.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            return DicActionPanel;
        }
    }
}
