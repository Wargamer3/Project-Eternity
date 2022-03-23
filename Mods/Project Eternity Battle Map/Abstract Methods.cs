﻿using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMap
    {
        public abstract byte[] GetSnapshotData();
        public abstract void Update(double ElapsedSeconds);
        public abstract void RemoveOnlinePlayer(string PlayerID, IOnlineConnection ActivePlayer);
        public abstract void Load(byte[] ArrayGameData);
        public abstract GameScreen GetMultiplayerScreen();

        public abstract void AddPlatform(BattleMapPlatform NewPlatform);

        public void AddLocalPlayer(BattleMapPlayer NewPlayer)
        {
            DoAddLocalPlayer(NewPlayer);

            foreach (BattleMap ActiveSubMap in ListSubMap)
            {
                ActiveSubMap.DoAddLocalPlayer(NewPlayer);
            }
        }

        protected abstract void DoAddLocalPlayer(BattleMapPlayer NewPlayer);

        public abstract void RemoveUnit(int PlayerIndex, UnitMapComponent UnitToRemove);

        public abstract void AddUnit(int PlayerIndex, UnitMapComponent UnitToAdd, MovementAlgorithmTile NewPosition);

        public abstract void Save(string FilePath);

        public abstract void SaveTemporaryMap();

        public abstract BattleMap LoadTemporaryMap(BinaryReader BR);

        public abstract BattleMap GetNewMap(string GameMode);

        public abstract string GetMapType();

        public abstract void SetWorld(Matrix World);

        public abstract bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement);

        public abstract Dictionary<string, ActionPanel> GetOnlineActionPanel();
    }
}
