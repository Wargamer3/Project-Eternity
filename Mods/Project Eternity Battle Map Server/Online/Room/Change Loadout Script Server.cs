using System;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class ChangeLoadoutScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Loadout";

        private readonly BattleMapPlayer ActivePlayer;

        public ChangeLoadoutScriptServer(BattleMapPlayer ActivePlayer)
            : base(ScriptName)
        {
            this.ActivePlayer = ActivePlayer;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendByte(ActivePlayer.LocalPlayerIndex);

            WriteBuffer.AppendInt32(ActivePlayer.Inventory.ActiveLoadout.ListSquad.Count);
            foreach (Squad ActiveSquad in ActivePlayer.Inventory.ActiveLoadout.ListSquad)
            {
                WriteBuffer.AppendInt32(ActiveSquad.UnitsInSquad);
                for (int U = 0; U < ActiveSquad.UnitsInSquad; ++U)
                {
                    Unit ActiveUnit = ActiveSquad.At(U);
                    WriteBuffer.AppendString(ActiveUnit.UnitTypeName);
                    WriteBuffer.AppendString(ActiveUnit.RelativePath);

                    WriteBuffer.AppendInt32(ActiveUnit.ArrayCharacterActive.Length);
                    for (int C = 0; C < ActiveUnit.ArrayCharacterActive.Length; ++C)
                    {
                        WriteBuffer.AppendString(ActiveUnit.ArrayCharacterActive[C].FullName);
                    }
                }
            }
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
