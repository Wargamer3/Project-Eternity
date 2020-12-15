using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class CreateRoomTripleThunderScriptServer : CreateRoomScriptServer
    {
        private readonly Server Owner;
        private readonly ClientGroup ClientGroupTemplate;

        public CreateRoomTripleThunderScriptServer(Server Owner, ClientGroup ClientGroupTemplate)
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

            foreach (IOnlineConnection ActivePlayer in CreatedGroup.Room.ListOnlinePlayer)
            {
                //Add Game Specific scripts
                Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();
                if (NewRoom.RoomType == RoomInformations.RoomTypeMission)
                {
                    MissionRoomInformations MissionRoom = (MissionRoomInformations)NewRoom;

                    DicNewScript.Add(AskStartGameMissionScriptServer.ScriptName, new AskStartGameMissionScriptServer(MissionRoom, (TripleThunderClientGroup)CreatedGroup, Owner));
                    DicNewScript.Add(AskChangeRoomExtrasMissionScriptServer.ScriptName, new AskChangeRoomExtrasMissionScriptServer(MissionRoom));
                }
                else if (NewRoom.RoomType == RoomInformations.RoomTypeBattle)
                {
                    BattleRoomInformations BattleRoom = (BattleRoomInformations)NewRoom;

                    DicNewScript.Add(AskStartGameMissionScriptServer.ScriptName, new AskStartGameBattleScriptServer(BattleRoom, (TripleThunderClientGroup)CreatedGroup, Owner));
                    DicNewScript.Add(AskChangeRoomExtrasBattleScriptServer.ScriptName, new AskChangeRoomExtrasBattleScriptServer(BattleRoom));
                }

                DicNewScript.Add(AskChangeCharacterScriptServer.ScriptName, new AskChangeCharacterScriptServer(NewRoom));
                DicNewScript.Add(AskChangePlayerTypeScriptServer.ScriptName, new AskChangePlayerTypeScriptServer(NewRoom));
                DicNewScript.Add(AskChangeTeamScriptServer.ScriptName, new AskChangeTeamScriptServer(NewRoom));
                DicNewScript.Add(AskChangeMapScriptServer.ScriptName, new AskChangeMapScriptServer(NewRoom, Owner));
                DicNewScript.Add(AskChangeRoomSubtypeScriptServer.ScriptName, new AskChangeRoomSubtypeScriptServer(NewRoom));
                DicNewScript.Add(LeaveRoomScriptServer.ScriptName, new LeaveRoomScriptServer(NewRoom, Owner));
                ActivePlayer.AddOrReplaceScripts(DicNewScript);
            }
        }
    }
}
