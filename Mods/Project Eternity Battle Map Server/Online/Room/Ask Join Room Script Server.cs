﻿using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
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
            BattleMapRoomInformations JoinedRoom = (BattleMapRoomInformations)ActiveGroup.Room;
            List<OnlinePlayerBase> ListJoiningPlayerInfo = JoinedRoom.GetOnlinePlayer(Sender);

            foreach (IOnlineConnection ActivePlayer in ActiveGroup.Room.ListUniqueOnlineConnection)
            {
                if (ActivePlayer == Sender)
                {
                    continue;
                }

                ActivePlayer.Send(new PlayerJoinedScriptServer(ListJoiningPlayerInfo));
            }

            Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetRoomScriptsServer(JoinedRoom, Owner);

            DicNewScript.Add(AskChangeLoadoutScriptServer.ScriptName, new AskChangeLoadoutScriptServer(JoinedRoom));
            DicNewScript.Add(AskStartGameBattleScriptServer.ScriptName, new AskStartGameBattleScriptServer(JoinedRoom, (Online.BattleMapClientGroup)ActiveGroup, Owner));
            DicNewScript.Add(AskChangeRoomExtrasMissionScriptServer.ScriptName, new AskChangeRoomExtrasMissionScriptServer(JoinedRoom));

            Sender.AddOrReplaceScripts(DicNewScript);

            if (ActiveGroup.CurrentGame != null)
            {
            }

            Sender.Send(new JoinRoomLocalScriptServer(RoomID, JoinedRoom.CurrentDifficulty, ListJoiningPlayerInfo, ActiveGroup));
        }
    }
}
