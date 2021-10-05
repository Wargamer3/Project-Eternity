using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class AskJoinRoomScriptServer : BaseAskJoinRoomScriptServer
    {
        public AskJoinRoomScriptServer(GameServer Owner)
            : base(Owner)
        {
        }

        public override OnlineScript Copy()
        {
            return new AskJoinRoomScriptServer(Owner);
        }

        protected override void OnJoinRoomLocal(IOnlineConnection Sender, string RoomID, GameClientGroup ActiveGroup)
        {
            RoomInformations JoinedRoom = (RoomInformations)ActiveGroup.Room;
            List<OnlinePlayer> ListJoiningPlayerInfo = JoinedRoom.GetOnlinePlayer(Sender);

            foreach (IOnlineConnection ActivePlayer in ActiveGroup.Room.ListOnlinePlayer)
            {
                if (ActivePlayer == Sender)
                {
                    continue;
                }

                ActivePlayer.Send(new PlayerJoinedScriptServer(ListJoiningPlayerInfo));
            }

            Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetRoomScriptsServer(JoinedRoom, Owner);

            PVPRoomInformations MissionRoom = (PVPRoomInformations)JoinedRoom;

            DicNewScript.Add(AskStartGameBattleScriptServer.ScriptName, new AskStartGameBattleScriptServer(MissionRoom, (BattleMapClientGroup)ActiveGroup, Owner));
            DicNewScript.Add(AskChangeRoomExtrasMissionScriptServer.ScriptName, new AskChangeRoomExtrasMissionScriptServer(MissionRoom));

            Sender.AddOrReplaceScripts(DicNewScript);

            if (ActiveGroup.CurrentGame != null)
            {
            }

            Sender.Send(new JoinRoomLocalScriptServer(RoomID, JoinedRoom.CurrentDifficulty, ListJoiningPlayerInfo, ActiveGroup));
        }
    }
}
