﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskChangeMapScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Map";

        private readonly RoomInformations Owner;
        private readonly GameServer OnlineServer;

        private string MapName;
        private string MapType;
        private string MapPath;
        private string GameMode;
        private byte MinNumberOfPlayer;
        private byte MaxNumberOfPlayer;
        private byte MaxSquadPerPlayer;
        private GameModeInfo GameInfo;
        private List<string> ListMandatoryMutator;
        private List<Color> ListMapTeam;

        public AskChangeMapScriptServer(RoomInformations Owner, GameServer OnlineServer)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.OnlineServer = OnlineServer;
        }

        public override OnlineScript Copy()
        {
            return new AskChangeMapScriptServer(Owner, OnlineServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            Owner.MapName = MapName;
            Owner.MapModName = MapType;
            Owner.MapPath = MapPath;
            Owner.GameMode = GameMode;
            Owner.GameInfo = GameInfo;
            Owner.MinNumberOfPlayer = MinNumberOfPlayer;
            Owner.MaxNumberOfPlayer = MaxNumberOfPlayer;
            Owner.MaxSquadPerPlayer = MaxSquadPerPlayer;
            Owner.ListMandatoryMutator = ListMandatoryMutator;

            for (int P = 0; P < Owner.ListUniqueOnlineConnection.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = Owner.ListUniqueOnlineConnection[P];

                ActiveOnlinePlayer.Send(new ChangeMapScriptServer(MapName, MapType, MapPath, GameMode, MinNumberOfPlayer, MaxNumberOfPlayer, MaxSquadPerPlayer, GameInfo, ListMandatoryMutator, ListMapTeam));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            MapName = Sender.ReadString();
            MapType = Sender.ReadString();
            MapPath = Sender.ReadString();
            GameMode = Sender.ReadString();
            MinNumberOfPlayer = Sender.ReadByte();
            MaxNumberOfPlayer = Sender.ReadByte();
            MaxSquadPerPlayer = Sender.ReadByte();

            string GameInfoName = Sender.ReadString();
            GameInfo = BattleMap.DicBattmeMapType[MapType].GetAvailableGameModes()[GameInfoName].Copy();
            GameInfo.Read(Sender);

            int ListMandatoryMutatorCount = Sender.ReadInt32();
            ListMandatoryMutator = new List<string>(ListMandatoryMutatorCount);
            for (int M = 0; M < ListMandatoryMutatorCount; M++)
            {
                ListMandatoryMutator.Add(Sender.ReadString());
            }

            int ListMapTeamCount = Sender.ReadInt32();
            ListMapTeam = new List<Color>(ListMapTeamCount);
            for (int M = 0; M < ListMapTeamCount; M++)
            {
                ListMapTeam.Add(Color.FromNonPremultiplied(Sender.ReadByte(), Sender.ReadByte(), Sender.ReadByte(), 255));
            }
        }
    }
}
