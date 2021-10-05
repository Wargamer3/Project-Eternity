﻿using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class AskChangeRoomExtrasBattleScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Room Extras";

        private readonly PVPRoomInformations Owner;

        public int MaxKill;
        public int MaxGameLengthInMinutes;

        public AskChangeRoomExtrasBattleScriptServer(PVPRoomInformations Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskChangeRoomExtrasBattleScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            //TODO: Update database
            Owner.MaxKill = MaxKill;
            Owner.MaxGameLengthInMinutes = MaxGameLengthInMinutes;

            for (int P = 0; P < Owner.ListOnlinePlayer.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = Owner.ListOnlinePlayer[P];

                ActiveOnlinePlayer.Send(new ChangeRoomExtrasBattleScriptServer(MaxKill, MaxGameLengthInMinutes));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            MaxKill = Sender.ReadInt32();
            MaxGameLengthInMinutes = Sender.ReadInt32();
        }
    }
}
