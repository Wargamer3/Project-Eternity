using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen.Online
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
            Player NewPlayer = new Player(PlayerManager.OnlinePlayerID, PlayerManager.OnlinePlayerName, OnlinePlayerBase.PlayerTypePlayer, true, false, 0, Color.Blue);
            NewPlayer.OnlineClient = Host;
            ByteReader BR = new ByteReader(PlayerInfo);

            NewPlayer.Level = BR.ReadInt32();
            NewPlayer.EXP = BR.ReadInt32();

            NewPlayer.InitRecords(BR);
            NewPlayer.Inventory.Load(BR, GameScreen.ContentFallback, DeathmatchParams.DicParams[""].DicUnitType, DeathmatchParams.DicParams[""].DicRequirement, DeathmatchParams.DicParams[""].DicEffect, DeathmatchParams.DicParams[""].DicAutomaticSkillTarget, DeathmatchParams.DicParams[""].DicManualSkillTarget);

            BR.Clear();

            PlayerManager.ListLocalPlayer[0] = NewPlayer;

            Host.Send(new CheckNewUnlocksScriptClient(PlayerID));
        }

        protected override void Read(OnlineReader Sender)
        {
            PlayerID = Sender.ReadString();
            PlayerInfo = Sender.ReadByteArray();
        }
    }
}
