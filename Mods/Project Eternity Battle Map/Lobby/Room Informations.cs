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
        public List<IOnlineConnection> ListOnlinePlayer { get; }
        public List<IOnlineConnection> ListUniqueOnlineConnection { get; }//Ignore local players
        public byte CurrentPlayerCount { get; set; }
        public byte MinNumberOfPlayer { get; set; }
        public byte MaxNumberOfPlayer { get; set; }
        public byte MaxSquadPerPlayer { get; set; }
        public string OwnerServerIP { get; }
        public int OwnerServerPort { get; }
        public bool IsDead { get; set; }//Used when the DataManager need to tell that a Room is deleted.

        public bool UseTeams;
        public int MaxNumberOfBots;
        public int MaxSquadsPerBot;

        public List<string> ListMandatoryMutator;
        public List<Mutator> ListMutator;

        public string CurrentDifficulty;

        public readonly List<OnlinePlayerBase> ListRoomPlayer;

        public readonly List<OnlinePlayerBase> ListRoomBot;

        private readonly List<string> ListLocalPlayerID;

        public RoomInformations(string RoomID, bool IsDead)
        {
            this.RoomID = RoomID;
            this.IsDead = IsDead;

            ListOnlinePlayer = new List<IOnlineConnection>();
            ListUniqueOnlineConnection = new List<IOnlineConnection>();
            ListRoomPlayer = new List<OnlinePlayerBase>();
            ListRoomBot = new List<OnlinePlayerBase>();
            ListLocalPlayerID = new List<string>();
            ListMandatoryMutator = new List<string>();
            ListMutator = new List<Mutator>();
            RoomName = string.Empty;
            RoomType = string.Empty;
            RoomSubtype = string.Empty;
            CurrentDifficulty = "Easy";
            UseTeams = true;
            OwnerServerIP = null;
            OwnerServerPort = 0;
            CurrentPlayerCount = 0;
            MaxNumberOfPlayer = 0;
            MaxSquadsPerBot = 1;
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
            ListUniqueOnlineConnection = new List<IOnlineConnection>();
            ListRoomPlayer = new List<OnlinePlayerBase>();
            ListRoomBot = new List<OnlinePlayerBase>();
            ListLocalPlayerID = new List<string>();
            ListMandatoryMutator = new List<string>();
            ListMutator = new List<Mutator>();
            CurrentDifficulty = "Easy";
            UseTeams = true;
            IsDead = false;
            OwnerServerIP = null;
            OwnerServerPort = 0;
            MaxSquadsPerBot = 1;
        }

        public RoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string CurrentDifficulty, string MapName, List<string> ListLocalPlayerID)
        {
            ListOnlinePlayer = new List<IOnlineConnection>();
            ListUniqueOnlineConnection = new List<IOnlineConnection>();
            this.RoomID = RoomID;
            this.RoomName = RoomName;
            this.RoomType = RoomType;
            this.RoomSubtype = RoomSubtype;
            this.CurrentDifficulty = CurrentDifficulty;
            this.MapPath = MapName;
            this.ListRoomPlayer = new List<OnlinePlayerBase>();
            ListRoomBot = new List<OnlinePlayerBase>();
            this.ListLocalPlayerID = ListLocalPlayerID;
            ListMandatoryMutator = new List<string>();
            ListMutator = new List<Mutator>();
            UseTeams = true;
            MaxSquadsPerBot = 1;
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
            ListUniqueOnlineConnection = new List<IOnlineConnection>();
            ListRoomPlayer = new List<OnlinePlayerBase>();
            ListRoomBot = new List<OnlinePlayerBase>();
            ListLocalPlayerID = new List<string>();
            ListMandatoryMutator = new List<string>();
            ListMutator = new List<Mutator>();
            CurrentDifficulty = "Easy";
            UseTeams = true;
            MaxSquadsPerBot = 1;
        }

        public void AddLocalPlayer(OnlinePlayerBase NewPlayer)
        {
            ListRoomPlayer.Add(NewPlayer);
            ListOnlinePlayer.Add(NewPlayer.OnlineClient);
            ListLocalPlayerID.Add(NewPlayer.ConnectionID);
            CurrentPlayerCount = (byte)ListRoomPlayer.Count;
        }

        public abstract void AddOnlinePlayerServer(IOnlineConnection NewPlayer, string PlayerType);

        public void RemovePlayer(IOnlineConnection OnlinePlayerToRemove)
        {
            for (int P = 0; P < ListOnlinePlayer.Count; ++P)
            {
                if (ListOnlinePlayer[P] == OnlinePlayerToRemove)
                {
                    ListUniqueOnlineConnection.Remove(OnlinePlayerToRemove);
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
            ListUniqueOnlineConnection.Remove(ListOnlinePlayer[Index]);
            ListOnlinePlayer.RemoveAt(Index);
            ListRoomPlayer.RemoveAt(Index);

            if (ListOnlinePlayer.Count == 0)
            {
                IsDead = true;
            }

            CurrentPlayerCount = (byte)ListRoomPlayer.Count;
            HandleHostChange();
        }

        public OnlinePlayerBase GetLocalPlayer()
        {
            foreach (OnlinePlayerBase LocalPlayer in ListRoomPlayer)
            {
                if (ListLocalPlayerID.Contains(LocalPlayer.ConnectionID))
                {
                    return LocalPlayer;
                }
            }

            return null;
        }

        public List<OnlinePlayerBase> GetLocalPlayers()
        {
            List<OnlinePlayerBase> ListLocalPlayer = new List<OnlinePlayerBase>();

            foreach (OnlinePlayerBase LocalPlayer in ListRoomPlayer)
            {
                if (ListLocalPlayerID.Contains(LocalPlayer.ConnectionID))
                {
                    ListLocalPlayer.Add(LocalPlayer);
                }
            }

            return ListLocalPlayer;
        }

        public List<OnlinePlayerBase> GetOnlinePlayer(IOnlineConnection Sender)
        {
            List<OnlinePlayerBase> ListPlayerInfo = new List<OnlinePlayerBase>();

            foreach (OnlinePlayerBase ActiveOnlinePlayer in ListRoomPlayer)
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

            foreach (OnlinePlayerBase ActiveOnlinePlayer in ListRoomPlayer)
            {
                if (ActiveOnlinePlayer.IsHost())
                {
                    HasHost = true;
                }
            }

            if (!HasHost)
            {
                int RandomNewHostIndex = RandomHelper.Next(ListRoomPlayer.Count);
                ListRoomPlayer[RandomNewHostIndex].OnlineClient.Roles.AddRole(RoleManager.Host);
                ListRoomPlayer[RandomNewHostIndex].OnlineClient.Send(new SendRolesScriptServer(ListRoomPlayer[RandomNewHostIndex].OnlineClient.Roles.ListActiveRole));

                for (int P = 0; P < ListUniqueOnlineConnection.Count; ++P)
                {
                    ListUniqueOnlineConnection[P].Send(new ChangePlayerRolesScriptServer(ListRoomPlayer[RandomNewHostIndex].ConnectionID, ListRoomPlayer[RandomNewHostIndex].OnlineClient.Roles.ListActiveRole));
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
