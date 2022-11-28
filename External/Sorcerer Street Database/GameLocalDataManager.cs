using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace Database.SorcererStreet
{
    public class GameLocalDataManager : IGameDataManager
    {
        private DateTimeOffset LastTimeChecked;
        private uint RoomIDCount;
        private SortedList<DateTimeOffset, List<RoomInformations>> ListRoom;
        private uint PlayerIDCount;
        private List<LocalPlayer> ListPlayer;

        public GameLocalDataManager()
        {
            LastTimeChecked = DateTime.MinValue;
            RoomIDCount = 1;
            ListRoom = new SortedList<DateTimeOffset, List<RoomInformations>>();
            ListPlayer = new List<LocalPlayer>();
        }

        public List<IRoomInformations> GetAllRoomUpdatesSinceLastTimeChecked(string ServerVersion)
        {
            DateTimeOffset CurrentTime = DateTimeOffset.Now;
            List<IRoomInformations> ListRoomFound = new List<IRoomInformations>();

            bool CheckTime = true;
            IList<DateTimeOffset> RoomKeys = ListRoom.Keys;

            for (int R = 0; R < RoomKeys.Count; ++R)
            {
                if ((CheckTime && RoomKeys[R] >= LastTimeChecked) || !CheckTime)
                {
                    CheckTime = false;
                    foreach (RoomInformations ActiveRoom in ListRoom[RoomKeys[R]])
                    {
                        ListRoomFound.Add(ActiveRoom);
                    }
                }
            }

            LastTimeChecked = CurrentTime;

            return ListRoomFound;
        }

        public void HandleOldData(string OwnerServerIP, int OwnerServerPort)
        {
        }

        public IRoomInformations GenerateNewRoom(string RoomName, string RoomType, string RoomSubtype, string Password, string OwnerServerIP, int OwnerServerPort, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
        {
            DateTimeOffset CurrentTime = DateTimeOffset.Now;

            RoomInformations NewRoom = new BattleMapRoomInformations(RoomIDCount++.ToString(), RoomName, RoomType, RoomSubtype, Password, OwnerServerIP, OwnerServerPort, MinNumberOfPlayer, MaxNumberOfPlayer);

            if (ListRoom.ContainsKey(CurrentTime))
            {
                ListRoom[CurrentTime].Add(NewRoom);
            }
            else
            {
                ListRoom.Add(CurrentTime, new List<RoomInformations>() { NewRoom });
            }

            return NewRoom;
        }

        public void RemoveRoom(string RoomID)
        {
            IList<DateTimeOffset> RoomKeys = ListRoom.Keys;

            for (int R = 0; R < RoomKeys.Count; ++R)
            {
                for (int i = 0; i < ListRoom[RoomKeys[R]].Count; i++)
                {
                    IRoomInformations ActiveRoom = ListRoom[RoomKeys[R]][i];
                    if (ActiveRoom.RoomID == RoomID)
                    {
                        ListRoom[RoomKeys[R]].RemoveAt(i);
                        break;
                    }
                }

                if (ListRoom[RoomKeys[R]].Count == 0)
                {
                    ListRoom.Remove(RoomKeys[R]);
                }
            }
        }

        public IRoomInformations TransferRoom(string RoomID, string OwnerServerIP)
        {
            throw new NotImplementedException();
        }

        public void UpdatePlayerCountInRoom(string RoomID, byte CurrentPlayerCount)
        {
            foreach (List<RoomInformations> ListRoomInfo in ListRoom.Values)
            {
                for (int i = 0; i < ListRoomInfo.Count; i++)
                {
                    RoomInformations ActiveRoom = ListRoomInfo[i];
                    if (ActiveRoom.RoomID == RoomID)
                    {
                        ActiveRoom.CurrentPlayerCount = CurrentPlayerCount;
                        return;
                    }
                }
            }
        }

        public void RemovePlayer(IOnlineConnection PlayerToRemove)
        {
            UpdatePlayerIsLoggedIn(PlayerToRemove.ID, "", 0);
        }

        public PlayerPOCO LogInPlayer(string Login, string Password, string GameServerIP, int GameServerPort)
        {
            LocalPlayer FoundPlayer = FindPlayer(Login, Password);

            if (FoundPlayer == null && FindPlayer(Login, null) == null)
            {
                LocalPlayer NewPlayer = new LocalPlayer();
                NewPlayer.ID = PlayerIDCount++.ToString();
                NewPlayer.Login = Login;
                NewPlayer.Name = Login;
                NewPlayer.Password = Password;
                NewPlayer.LoggedIn = true;
                NewPlayer.Level = 1;
                NewPlayer.NumberOfFailedConnection = 0;

                FoundPlayer = NewPlayer;
                ListPlayer.Add(NewPlayer);
            }

            if (FoundPlayer == null)
            {
                return LogInPlayer(Login + "1", Password, GameServerIP, GameServerPort);
            }
            else
            {
                ByteWriter BW = new ByteWriter();
                BW.AppendString(FoundPlayer.Name);
                BW.AppendInt32(FoundPlayer.Level);
                FoundPlayer.Info = BW.GetBytes();
                BW.ClearWriteBuffer();

                UpdatePlayerIsLoggedIn(FoundPlayer.ID, GameServerIP, GameServerPort);

                return FoundPlayer;
            }
        }

        public PlayerPOCO GetPlayerInventory(string ID)
        {
            throw new NotImplementedException();
        }

        private LocalPlayer FindPlayer(string Login, string Password)
        {
            foreach (LocalPlayer ActivePlayer in ListPlayer)
            {
                if (ActivePlayer.Login == Login)
                {
                    if (Password == null)
                    {
                        return ActivePlayer;
                    }
                    else if (ActivePlayer.Password == Password)
                    {
                        return ActivePlayer;
                    }
                }
            }

            return null;
        }

        private void UpdatePlayerIsLoggedIn(string ID, string GameServerIP, int GameServerPort)
        {
            if (ID == null)
            {
                return;
            }

            foreach (LocalPlayer ActivePlayer in ListPlayer)
            {
                if (ActivePlayer.ID == ID)
                {
                    ActivePlayer.LoggedIn = GameServerIP != null;
                    break;
                }
            }

        }

        private class LocalPlayer : PlayerPOCO
        {
            public int NumberOfFailedConnection;

            public string Login;
            public string Password;
            public bool LoggedIn;
            public int Level;
        }
    }
}
