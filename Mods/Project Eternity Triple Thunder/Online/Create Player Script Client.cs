using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class CreatePlayerScriptClient : OnlineScript
    {
        public const string ScriptName = "Create Player";

        public class GamePlayer
        {
            public readonly uint PlayerID;
            public readonly IOnlineConnection ActiveOnlinePlayer;
            public readonly int LayerIndex;
            public readonly Vector2 PlayerPosition;
            public readonly Player PlayerInfo;

            public GamePlayer(uint PlayerID, IOnlineConnection ActiveOnlinePlayer, int LayerIndex, Vector2 PlayerPosition, Player PlayerInfo)
            {
                this.PlayerID = PlayerID;
                this.ActiveOnlinePlayer = ActiveOnlinePlayer;
                this.LayerIndex = LayerIndex;
                this.PlayerPosition = PlayerPosition;
                this.PlayerInfo = PlayerInfo;
            }
        }

        private readonly TripleThunderOnlineClient Owner;

        private bool IsPlayerControlled;
        private GamePlayer GamePlayerToCreate;

        public CreatePlayerScriptClient(TripleThunderOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new CreatePlayerScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            Owner.TripleThunderGame.AddPlayer(GamePlayerToCreate.PlayerInfo, GamePlayerToCreate.LayerIndex, GamePlayerToCreate.PlayerPosition, GamePlayerToCreate.PlayerID);
            
            if (IsPlayerControlled)
            {
                Owner.TripleThunderGame.AddLocalCharacter(GamePlayerToCreate.PlayerInfo);
            }
        }

        public static GamePlayer ReadGamePlayer(OnlineReader Sender)
        {
            uint GamePlayerID = Sender.ReadUInt32();
            int LayerIndex = Sender.ReadInt32();
            Vector2 PlayerPosition = new Vector2(Sender.ReadFloat(), Sender.ReadFloat());

            string PlayerID = Sender.ReadString();
            string PlayerName = Sender.ReadString();
            string PlayerType = Sender.ReadString();
            int PlayerTeam = Sender.ReadInt32();

            Player PlayerInfo = new Player(PlayerID, PlayerName, PlayerType, PlayerTeam);

            PlayerInfo.Equipment.CharacterType = Sender.ReadString();
            /*PlayerInfo.Equipment.EquipedBooster =*/Sender.ReadString();
            PlayerInfo.Equipment.GrenadeType = Sender.ReadString();
            PlayerInfo.Equipment.ExtraWeaponType = Sender.ReadString();
            /*PlayerInfo.Equipment.EquipedWeaponOption =*/ Sender.ReadString();
            /*PlayerInfo.Equipment.EquipedArmor =*/Sender.ReadString();

            return new GamePlayer(GamePlayerID, null, LayerIndex, PlayerPosition, PlayerInfo);
        }

        protected override void Read(OnlineReader Sender)
        {
            IsPlayerControlled = Sender.ReadBoolean();
            GamePlayerToCreate = ReadGamePlayer(Sender);
        }
    }
}
