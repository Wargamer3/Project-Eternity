using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Online
{
    public class PlayerInventoryScriptClient : OnlineScript
    {
        public const string ScriptName = "Player Inventory";

        private string PlayerID;
        private byte[] PlayerInfo;

        public PlayerInventoryScriptClient()
            : base(ScriptName)
        {
        }

        public override OnlineScript Copy()
        {
            return new PlayerInventoryScriptClient();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Player NewPlayer = new Player(PlayerManager.OnlinePlayerID, PlayerManager.OnlinePlayerName, OnlinePlayerBase.PlayerTypes.Player, false, 0, true, Color.Blue);

            ByteReader BR = new ByteReader(PlayerInfo);

            NewPlayer.Inventory.Load(BR, GameScreen.ContentFallback, SorcererStreetBattleParams.DicParams[""].DicRequirement, SorcererStreetBattleParams.DicParams[""].DicEffect, SorcererStreetBattleParams.DicParams[""].DicAutomaticSkillTarget, SorcererStreetBattleParams.DicParams[""].DicManualSkillTarget);

            BR.Clear();

            PlayerManager.ListLocalPlayer[0] = NewPlayer;
        }

        protected override void Read(OnlineReader Sender)
        {
            PlayerID = Sender.ReadString();
            PlayerInfo = Sender.ReadByteArray();
        }
    }
}
