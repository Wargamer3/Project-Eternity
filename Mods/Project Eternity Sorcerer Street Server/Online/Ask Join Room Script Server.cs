using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Server;
using OnlineHelper = ProjectEternity.GameScreens.BattleMapScreen.Server.OnlineHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Server
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
            SorcererStreetRoomInformations JoinedRoom = (SorcererStreetRoomInformations)ActiveGroup.Room;
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

            DicNewScript.Add(AskChangeBookScriptServer.ScriptName, new AskChangeBookScriptServer(JoinedRoom));
            DicNewScript.Add(AskStartGameBattleScriptServer.ScriptName, new AskStartGameBattleScriptServer(JoinedRoom, (BattleMapClientGroup)ActiveGroup, Owner));
            DicNewScript.Add(AskChangeRoomExtrasBattleScriptServer.ScriptName, new AskChangeRoomExtrasBattleScriptServer(JoinedRoom));

            Sender.AddOrReplaceScripts(DicNewScript);

            if (ActiveGroup.CurrentGame != null)
            {
            }

            Sender.Send(new JoinRoomLocalScriptServer(RoomID, JoinedRoom.CurrentDifficulty, ListJoiningPlayerInfo, ActiveGroup));
        }
    }
}
