using System;
using ProjectEternity.Core.Online;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class LoginSuccessScriptClient : OnlineScript
    {
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

            BattleMapPlayer NewPlayer = InitPlayer();
            PlayerManager.ListLocalPlayer.Add(NewPlayer);

            Owner.IdentifyToCommunicationServer(PlayerManager.OnlinePlayerID, PlayerManager.OnlinePlayerName, PlayerInfo);
            Owner.AskForPlayerList();
            Owner.AskForPlayerInventory();
        }

        private BattleMapPlayer InitPlayer()
        {
            ByteReader BR = new ByteReader(PlayerInfo);

            PlayerManager.OnlinePlayerName = BR.ReadString();
            PlayerManager.OnlinePlayerLevel = BR.ReadInt32();

            BR.Clear();

            BattleMapPlayer NewPlayer = new BattleMapPlayer(PlayerManager.OnlinePlayerID, PlayerManager.OnlinePlayerName, OnlinePlayerBase.PlayerTypes.Player, false, 0, true, Color.Blue);

            return NewPlayer;
        }

        protected override void Read(OnlineReader Sender)
        {
            PlayerID = Sender.ReadString();
            PlayerInfo = Sender.ReadByteArray();
        }
    }
}
