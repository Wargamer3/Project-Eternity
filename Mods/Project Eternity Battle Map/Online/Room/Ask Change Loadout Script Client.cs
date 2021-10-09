using System;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class AskChangeLoadoutScriptClient : OnlineScript
    {
        private readonly BattleMapPlayer ActivePlayer;

        public AskChangeLoadoutScriptClient(BattleMapPlayer ActivePlayer)
            : base("Ask Change Loadout")
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

            WriteBuffer.AppendInt32(ActivePlayer.ListSquadToSpawn.Count);
            foreach (Squad ActiveSquad in ActivePlayer.ListSquadToSpawn)
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

        protected override void Execute(IOnlineConnection Host)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
