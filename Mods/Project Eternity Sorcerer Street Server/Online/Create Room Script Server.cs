using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Server;
using OnlineHelper = ProjectEternity.GameScreens.BattleMapScreen.Server.OnlineHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Server
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

            SorcererStreetRoomInformations NewRoom = (SorcererStreetRoomInformations)CreatedGroup.Room;

            foreach (IOnlineConnection ActivePlayer in CreatedGroup.Room.ListOnlinePlayer)
            {
                //Add Game Specific scripts
                Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetRoomScriptsServer(NewRoom, Owner);

                DicNewScript.Add(AskChangeBookScriptServer.ScriptName, new AskChangeBookScriptServer(NewRoom));
                DicNewScript.Add(AskStartGameBattleScriptServer.ScriptName, new AskStartGameBattleScriptServer(NewRoom, (BattleMapClientGroup)CreatedGroup, Owner));
                DicNewScript.Add(AskChangeRoomExtrasBattleScriptServer.ScriptName, new AskChangeRoomExtrasBattleScriptServer(NewRoom));

                ActivePlayer.AddOrReplaceScripts(DicNewScript);
            }
        }
    }
}
