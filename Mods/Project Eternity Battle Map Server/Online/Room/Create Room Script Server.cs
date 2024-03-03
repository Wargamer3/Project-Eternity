using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
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

            BattleMapRoomInformations NewRoom = (BattleMapRoomInformations)CreatedGroup.Room;

            foreach (IOnlineConnection ActivePlayer in CreatedGroup.Room.ListUniqueOnlineConnection)
            {
                //Add Game Specific scripts
                Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetRoomScriptsServer(NewRoom, Owner);

                DicNewScript.Add(AskChangeLoadoutScriptServer.ScriptName, new AskChangeLoadoutScriptServer(NewRoom));
                DicNewScript.Add(AskStartGameBattleScriptServer.ScriptName, new AskStartGameBattleScriptServer(NewRoom, (BattleMapClientGroup)CreatedGroup, Owner));
                DicNewScript.Add(AskChangeRoomExtrasMissionScriptServer.ScriptName, new AskChangeRoomExtrasMissionScriptServer(NewRoom));

                ActivePlayer.AddOrReplaceScripts(DicNewScript);
            }
        }
    }
}
