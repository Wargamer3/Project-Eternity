using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class RoomInformations : IRoomInformations
    {
        public const string RoomTypeCoop = "Coop";
        public const string RoomTypePVP = "PVP";

        public string RoomID { get; }//Only contains a value for locally create Rooms, if the Room is on another server the ID should be null.
        public string RoomName { get; }
        public string RoomType { get; set; }
        public string RoomSubtype { get; set; }
        public string MapName { get; set; }
        public string MapType { get; set; }
        public string MapPath { get; set; }
        public bool IsPlaying { get; set; }
        public string Password { get; set; }
        public bool UseTeams { get; set; }
        public List<IOnlineConnection> ListOnlinePlayer { get; }
        public byte CurrentPlayerCount { get; set; }
        public byte MinNumberOfPlayer { get; set; }
        public byte MaxNumberOfPlayer { get; set; }
        public string OwnerServerIP { get; }
        public int OwnerServerPort { get; }
        public bool IsDead { get; set; }//Used when the DataManager need to tell that a Room is deleted.

        public string CurrentDifficulty;

        public readonly List<BattleMapPlayer> ListRoomPlayer;

        private readonly List<string> ListLocalPlayerID;

        public RoomInformations(string RoomID, bool IsDead)
        {
            this.RoomID = RoomID;
            this.IsDead = IsDead;

            ListOnlinePlayer = new List<IOnlineConnection>();
            ListRoomPlayer = new List<BattleMapPlayer>();
            ListLocalPlayerID = new List<string>();
            RoomName = string.Empty;
            RoomType = string.Empty;
            RoomSubtype = string.Empty;
            CurrentDifficulty = "Easy";
            UseTeams = true;
            OwnerServerIP = null;
            OwnerServerPort = 0;
            CurrentPlayerCount = 0;
            MaxNumberOfPlayer = 0;
        }

        public RoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, bool IsPlaying, byte MinPlayer, byte MaxPlayer, byte CurrentClientCount)
        {
            this.RoomID = RoomID;
            this.RoomName = RoomName;
            this.RoomType = RoomType;
            this.RoomSubtype = RoomSubtype;
            this.MinNumberOfPlayer = MinPlayer;
            this.MaxNumberOfPlayer = MaxPlayer;
            this.CurrentPlayerCount = CurrentClientCount;

            ListOnlinePlayer = new List<IOnlineConnection>();
            ListRoomPlayer = new List<BattleMapPlayer>();
            ListLocalPlayerID = new List<string>();
            CurrentDifficulty = "Easy";
            UseTeams = true;
            IsDead = false;
            OwnerServerIP = null;
            OwnerServerPort = 0;
        }

        public RoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string CurrentDifficulty, string MapName, List<string> ListLocalPlayerID)
        {
            ListOnlinePlayer = new List<IOnlineConnection>();
            this.RoomID = RoomID;
            this.RoomName = RoomName;
            this.RoomType = RoomType;
            this.RoomSubtype = RoomSubtype;
            this.CurrentDifficulty = CurrentDifficulty;
            this.MapPath = MapName;
            this.ListRoomPlayer = new List<BattleMapPlayer>();
            this.ListLocalPlayerID = ListLocalPlayerID;
            UseTeams = true;
        }

        public RoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, bool IsPlaying, string Password, string OwnerServerIP, int OwnerServerPort,
            byte CurrentPlayerCount, byte MinNumberOfPlayer, byte MaxNumberOfPlayer, bool IsDead)
        {
            this.RoomID = RoomID;
            this.RoomName = RoomName;
            this.RoomType = RoomType;
            this.RoomSubtype = RoomSubtype;
            this.IsPlaying = IsPlaying;
            this.Password = Password;
            this.OwnerServerIP = OwnerServerIP;
            this.OwnerServerPort = OwnerServerPort;
            this.CurrentPlayerCount = CurrentPlayerCount;
            this.MinNumberOfPlayer = MinNumberOfPlayer;
            this.MaxNumberOfPlayer = MaxNumberOfPlayer;
            this.IsDead = IsDead;

            ListOnlinePlayer = new List<IOnlineConnection>();
            ListRoomPlayer = new List<BattleMapPlayer>();
            ListLocalPlayerID = new List<string>();
            CurrentDifficulty = "Easy";
            UseTeams = true;
        }

        public void AddLocalPlayer(BattleMapPlayer NewPlayer)
        {
            ListRoomPlayer.Add(NewPlayer);
            ListLocalPlayerID.Add(NewPlayer.ConnectionID);
            CurrentPlayerCount = (byte)ListRoomPlayer.Count;
        }

        public void AddOnlinePlayer(IOnlineConnection NewPlayer, string PlayerType)
        {
            ListOnlinePlayer.Add(NewPlayer);
            BattleMapPlayer NewRoomPlayer = new BattleMapPlayer(NewPlayer.ID, NewPlayer.Name, PlayerType, true, 0, true, Color.Blue);
            NewRoomPlayer.OnlineClient = NewPlayer;
            NewRoomPlayer.GameplayType = GameplayTypes.None;
            ListRoomPlayer.Add(NewRoomPlayer);
            CurrentPlayerCount = (byte)ListRoomPlayer.Count;
        }

        public void RemovePlayer(IOnlineConnection OnlinePlayerToRemove)
        {
            for (int P = 0; P < ListOnlinePlayer.Count; ++P)
            {
                if (ListOnlinePlayer[P] == OnlinePlayerToRemove)
                {
                    ListOnlinePlayer.RemoveAt(P);
                    ListRoomPlayer.RemoveAt(P);

                    if (ListOnlinePlayer.Count == 0)
                    {
                        IsDead = true;
                    }
                }
            }

            CurrentPlayerCount = (byte)ListRoomPlayer.Count;
            HandleHostChange();
        }

        public void RemoveOnlinePlayer(int Index)
        {
            ListOnlinePlayer.RemoveAt(Index);
            ListRoomPlayer.RemoveAt(Index);

            if (ListOnlinePlayer.Count == 0)
            {
                IsDead = true;
            }

            CurrentPlayerCount = (byte)ListRoomPlayer.Count;
            HandleHostChange();
        }

        public BattleMapPlayer GetLocalPlayer()
        {
            foreach (BattleMapPlayer LocalPlayer in ListRoomPlayer)
            {
                if (ListLocalPlayerID.Contains(LocalPlayer.ConnectionID))
                {
                    return LocalPlayer;
                }
            }

            return null;
        }

        public List<BattleMapPlayer> GetLocalPlayers()
        {
            List<BattleMapPlayer> ListLocalPlayer = new List<BattleMapPlayer>();

            foreach (BattleMapPlayer LocalPlayer in ListRoomPlayer)
            {
                if (ListLocalPlayerID.Contains(LocalPlayer.ConnectionID))
                {
                    ListLocalPlayer.Add(LocalPlayer);
                }
            }

            return ListLocalPlayer;
        }

        public List<BattleMapPlayer> GetOnlinePlayer(IOnlineConnection Sender)
        {
            List<BattleMapPlayer> ListPlayerInfo = new List<BattleMapPlayer>();

            foreach (BattleMapPlayer ActiveOnlinePlayer in ListRoomPlayer)
            {
                if (ActiveOnlinePlayer.ConnectionID == Sender.ID)
                {
                    ListPlayerInfo.Add(ActiveOnlinePlayer);
                }
            }

            return ListPlayerInfo;
        }

        private void HandleHostChange()
        {
            if (ListOnlinePlayer.Count == 0)
            {
                return;
            }

            bool HasHost = false;

            foreach (BattleMapPlayer ActiveOnlinePlayer in ListRoomPlayer)
            {
                if (ActiveOnlinePlayer.OnlinePlayerType == BattleMapPlayer.PlayerTypeHost)
                {
                    HasHost = true;
                }
            }

            if (!HasHost)
            {
                int RandomNewHostIndex = RandomHelper.Next(ListRoomPlayer.Count);
                ListRoomPlayer[RandomNewHostIndex].OnlinePlayerType = BattleMapPlayer.PlayerTypeHost;

                for (int P = 0; P < ListOnlinePlayer.Count; ++P)
                {
                    ListOnlinePlayer[P].Send(new ChangePlayerTypeScriptServer(ListRoomPlayer[RandomNewHostIndex].ConnectionID, BattleMapPlayer.PlayerTypeHost));
                }
            }
        }

        public virtual byte[] GetRoomInfo()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                using (BinaryWriter BW = new BinaryWriter(MS))
                {
                    BW.Write(ListRoomPlayer.Count);
                    for (int P = 0; P < ListRoomPlayer.Count; P++)
                    {
                        BW.Write(ListRoomPlayer[P].ConnectionID);
                        BW.Write(ListRoomPlayer[P].Name);
                        BW.Write(ListRoomPlayer[P].OnlinePlayerType);
                        BW.Write(ListRoomPlayer[P].Team);
                    }

                    return MS.ToArray();
                }
            }
        }
    }
}
