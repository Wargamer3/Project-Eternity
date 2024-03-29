﻿using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class CreateRoomTripleThunderScriptServer : BaseCreateRoomScriptServer
    {
        private readonly GameServer Owner;
        private readonly GameClientGroup ClientGroupTemplate;

        public CreateRoomTripleThunderScriptServer(GameServer Owner, GameClientGroup ClientGroupTemplate)
            : base(Owner, ClientGroupTemplate)
        {
            this.Owner = Owner;
            this.ClientGroupTemplate = ClientGroupTemplate;
        }

        public override OnlineScript Copy()
        {
            return new CreateRoomTripleThunderScriptServer(Owner, ClientGroupTemplate);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            base.Execute(Sender);

            RoomInformations NewRoom = (RoomInformations)CreatedGroup.Room;

            foreach (IOnlineConnection ActivePlayer in CreatedGroup.Room.ListUniqueOnlineConnection)
            {
                //Add Game Specific scripts
                Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetRoomScriptsServer(NewRoom, Owner);
                if (NewRoom.GameMode == RoomInformations.RoomTypeMission)
                {
                    MissionRoomInformations MissionRoom = (MissionRoomInformations)NewRoom;

                    DicNewScript.Add(AskStartGameMissionScriptServer.ScriptName, new AskStartGameMissionScriptServer(MissionRoom, (TripleThunderClientGroup)CreatedGroup, Owner));
                    DicNewScript.Add(AskChangeRoomExtrasMissionScriptServer.ScriptName, new AskChangeRoomExtrasMissionScriptServer(MissionRoom));
                }
                else if (NewRoom.GameMode == RoomInformations.RoomTypeBattle)
                {
                    BattleRoomInformations BattleRoom = (BattleRoomInformations)NewRoom;

                    DicNewScript.Add(AskStartGameMissionScriptServer.ScriptName, new AskStartGameBattleScriptServer(BattleRoom, (TripleThunderClientGroup)CreatedGroup, Owner));
                    DicNewScript.Add(AskChangeRoomExtrasBattleScriptServer.ScriptName, new AskChangeRoomExtrasBattleScriptServer(BattleRoom));
                }

                ActivePlayer.AddOrReplaceScripts(DicNewScript);
            }
        }
    }
}
