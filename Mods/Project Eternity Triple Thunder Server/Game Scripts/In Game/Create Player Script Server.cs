using System;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;
using static ProjectEternity.GameScreens.TripleThunderScreen.Online.CreatePlayerScriptClient;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class CreatePlayerScriptServer : OnlineScript
    {
        public const string ScriptName = "Create Player";

        private readonly bool IsPlayerControlled;
        private readonly int LayerIndex;
        private Player GamePlayerToCreate;

        public CreatePlayerScriptServer(Player GamePlayerToCreate, int LayerIndex, bool IsPlayerControlled)
            : base(ScriptName)
        {
            this.GamePlayerToCreate = GamePlayerToCreate;
            this.LayerIndex = LayerIndex;
            this.IsPlayerControlled = IsPlayerControlled;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        public static void WriteGamePlayer(OnlineWriter WriteBuffer, Player GamePlayerToWrite, int LayerIndex)
        {
            WriteBuffer.AppendUInt32(GamePlayerToWrite.InGameRobot.ID);
            WriteBuffer.AppendInt32(LayerIndex);
            WriteBuffer.AppendFloat(GamePlayerToWrite.InGameRobot.Position.X);
            WriteBuffer.AppendFloat(GamePlayerToWrite.InGameRobot.Position.Y);

            WriteBuffer.AppendString(GamePlayerToWrite.ConnectionID);
            WriteBuffer.AppendString(GamePlayerToWrite.Name);
            WriteBuffer.AppendString(GamePlayerToWrite.PlayerType);
            WriteBuffer.AppendInt32(GamePlayerToWrite.Team);

            WriteBuffer.AppendString(GamePlayerToWrite.Equipment.CharacterType);
            if (GamePlayerToWrite.Equipment.EquipedBooster != null)
            {
                WriteBuffer.AppendString(GamePlayerToWrite.Equipment.EquipedBooster.Name);
            }
            else
            {
                WriteBuffer.AppendString("");
            }

            WriteBuffer.AppendString(GamePlayerToWrite.Equipment.GrenadeType);

            if (GamePlayerToWrite.Equipment.EquipedSecondaryWeapon != null)
            {
                WriteBuffer.AppendString(GamePlayerToWrite.Equipment.EquipedSecondaryWeapon.Name);
            }
            else
            {
                WriteBuffer.AppendString("");
            }

            if (GamePlayerToWrite.Equipment.EquipedWeaponOption != null)
            {
                WriteBuffer.AppendString(GamePlayerToWrite.Equipment.EquipedWeaponOption.Name);
            }
            else
            {
                WriteBuffer.AppendString("");
            }

            if (GamePlayerToWrite.Equipment.EquipedArmor != null)
            {
                WriteBuffer.AppendString(GamePlayerToWrite.Equipment.EquipedArmor.Name);
            }
            else
            {
                WriteBuffer.AppendString("");
            }
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendBoolean(IsPlayerControlled);
            WriteGamePlayer(WriteBuffer, GamePlayerToCreate, LayerIndex);
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
