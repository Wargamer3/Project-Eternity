using System;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class MoveUnitScriptClient : OnlineScript
    {
        public const string ScriptName = "Move Unit";

        private readonly BattleMapOnlineClient Owner;

        private Squad ActiveSquad;
        private Vector3 StartPosition;
        private Vector3 FinalPosition;

        public MoveUnitScriptClient(BattleMapOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new MoveUnitScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
        }

        protected override void Execute(IOnlineConnection Sender)
        {

            DeathmatchMap Map = (DeathmatchMap)Owner.BattleMapGame;

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                int ActiveSquadIndex = Map.CheckForSquadAtPosition(P, StartPosition, Vector3.Zero);

                if (ActiveSquadIndex >= 0)
                {
                    ActiveSquad = Map.ListPlayer[P].ListSquad[ActiveSquadIndex];
                    ActiveSquad.SetPosition(FinalPosition);
                    Owner.BattleMapGame.MovementAnimation.Add(ActiveSquad, StartPosition, FinalPosition);
                    return;
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            StartPosition = new Vector3(Sender.ReadInt32(), Sender.ReadInt32(), Sender.ReadInt32());
            FinalPosition = new Vector3(Sender.ReadInt32(), Sender.ReadInt32(), Sender.ReadInt32());
        }
    }
}
