using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class CreateRoomScriptServer : BaseCreateRoomScriptServer
    {
        private readonly GameServer Owner;
        private readonly GameClientGroup ClientGroupTemplate;

        public CreateRoomScriptServer(GameServer Owner, GameClientGroup ClientGroupTemplate)
            : base(Owner, ClientGroupTemplate)
        {
            this.Owner = Owner;
            this.ClientGroupTemplate = ClientGroupTemplate;
        }

        public override OnlineScript Copy()
        {
            return new CreateRoomScriptServer(Owner, ClientGroupTemplate);
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
                Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetRoomScriptsServer(NewRoom, Owner);
                PVPRoomInformations MissionRoom = (PVPRoomInformations)NewRoom;

                DicNewScript.Add(AskStartGameBattleScriptServer.ScriptName, new AskStartGameBattleScriptServer(MissionRoom, (BattleMapClientGroup)CreatedGroup, Owner));
                DicNewScript.Add(AskChangeRoomExtrasMissionScriptServer.ScriptName, new AskChangeRoomExtrasMissionScriptServer(MissionRoom));

                ActivePlayer.AddOrReplaceScripts(DicNewScript);
            }
        }
    }
}
