using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class AskJoinRoomScriptServer : Core.Online.BaseAskJoinRoomScriptServer
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
            List <Player> ListJoiningPlayerInfo = JoinedRoom.GetOnlinePlayer(Sender);

            foreach (IOnlineConnection ActivePlayer in ActiveGroup.Room.ListOnlinePlayer)
            {
                if (ActivePlayer == Sender)
                {
                    continue;
                }

                ActivePlayer.Send(new PlayerJoinedScriptServer(ListJoiningPlayerInfo));
            }

            Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetRoomScriptsServer(JoinedRoom, Owner);

            if (JoinedRoom.RoomType == RoomInformations.RoomTypeMission)
            {
                MissionRoomInformations MissionRoom = (MissionRoomInformations)JoinedRoom;

                DicNewScript.Add(AskStartGameMissionScriptServer.ScriptName, new AskStartGameMissionScriptServer(MissionRoom, (TripleThunderClientGroup)ActiveGroup, Owner));
                DicNewScript.Add(AskChangeRoomExtrasMissionScriptServer.ScriptName, new AskChangeRoomExtrasMissionScriptServer(MissionRoom));
            }
            else if (JoinedRoom.RoomType == RoomInformations.RoomTypeBattle)
            {
                BattleRoomInformations BattleRoom = (BattleRoomInformations)JoinedRoom;

                DicNewScript.Add(AskStartGameMissionScriptServer.ScriptName, new AskStartGameBattleScriptServer(BattleRoom, (TripleThunderClientGroup)ActiveGroup, Owner));
                DicNewScript.Add(AskChangeRoomExtrasBattleScriptServer.ScriptName, new AskChangeRoomExtrasBattleScriptServer(BattleRoom));
            }
            Sender.AddOrReplaceScripts(DicNewScript);

            if (ActiveGroup.CurrentGame != null)
            {
                FightingZone CurrentGame = (FightingZone)ActiveGroup.CurrentGame;

                foreach (Player ActivePlayer in ListJoiningPlayerInfo)
                {
                    CurrentGame.AddLocalCharacter(ActivePlayer);
                    ActivePlayer.OnlineClient = Sender;

                    int LayerIndex;
                    CurrentGame.AddPlayerFromSpawn(ActivePlayer, CurrentGame.NextRobotID + (uint.MaxValue - 100), true, out LayerIndex);

                    //Add Game Specific scripts
                    DicNewScript = OnlineHelper.GetTripleThunderScriptsServer((TripleThunderClientGroup)ActiveGroup, ActivePlayer);
                    Sender.AddOrReplaceScripts(DicNewScript);

                    foreach (IOnlineConnection OtherPlayer in ActiveGroup.Room.ListOnlinePlayer)
                    {
                        if (OtherPlayer == Sender)
                        {
                            continue;
                        }

                        OtherPlayer.Send(new CreatePlayerScriptServer(ActivePlayer, LayerIndex, false));
                    }
                }
            }

            Sender.Send(new JoinRoomLocalScriptServer(RoomID, JoinedRoom.CurrentDifficulty, ListJoiningPlayerInfo, ActiveGroup));
        }
    }
}
