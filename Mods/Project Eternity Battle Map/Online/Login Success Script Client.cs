using System;
using System.Collections.Generic;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class LoginSuccessScriptClient : OnlineScript
    {
        private delegate void SafeCallDelegate(IOnlineConnection Host);

        public const string ScriptName = "Login Success";

        private readonly Lobby Owner;
        private string PlayerID;
        private byte[] PlayerInfo;

        public LoginSuccessScriptClient(Lobby Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new LoginSuccessScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            PlayerManager.OnlinePlayerID = PlayerID;

            OnlinePlayer NewPlayer = InitPlayer();
            PlayerManager.ListLocalPlayer.Add(NewPlayer);

            Owner.IdentifyToCommunicationServer(PlayerManager.OnlinePlayerID, PlayerManager.OnlinePlayerName, PlayerInfo);
            Owner.AskForPlayerList();
        }

        private OnlinePlayer InitPlayer()
        {
            ByteReader BR = new ByteReader(PlayerInfo);

            PlayerManager.OnlinePlayerName = BR.ReadString();
            PlayerManager.OnlinePlayerLevel = BR.ReadInt32();

            BR.Clear();

            Unit NewUnit = Unit.FromFullName("Normal/Original/Voltaire", Owner.Content, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
            Character NewCharacter = new Character("Original/Greg", Owner.Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
            NewCharacter.Level = 1;
            NewUnit.ArrayCharacterActive = new Character[] { NewCharacter };

            Squad NewSquad = new Squad("Squad", NewUnit);

            OnlinePlayer NewPlayer = new OnlinePlayer(PlayerManager.OnlinePlayerID, PlayerManager.OnlinePlayerName, OnlinePlayer.PlayerTypes.Online, false, 0);
            NewPlayer.ListSquadToSpawn.Add(NewSquad);

            return NewPlayer;
        }

        protected override void Read(OnlineReader Sender)
        {
            PlayerID = Sender.ReadString();
            PlayerInfo = Sender.ReadByteArray();
        }
    }
}
