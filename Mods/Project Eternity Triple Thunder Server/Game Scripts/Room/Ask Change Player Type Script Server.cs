﻿using System;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class AskChangePlayerTypeScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Player Type";

        private readonly RoomInformations Owner;

        private string NewPlayerType;

        public AskChangePlayerTypeScriptServer(RoomInformations Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskChangePlayerTypeScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            foreach (Player ActivePlayer in Owner.ListRoomPlayer)
            {
                if (ActivePlayer.ConnectionID == Sender.ID)
                {
                    ActivePlayer.PlayerType = NewPlayerType;
                }
            }

            for (int P = 0; P < Owner.ListOnlinePlayer.Count; P++)
            {
                Owner.ListOnlinePlayer[P].Send(new ChangePlayerTypeScriptServer(Sender.ID, NewPlayerType));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            NewPlayerType = Sender.ReadString();
        }
    }
}
