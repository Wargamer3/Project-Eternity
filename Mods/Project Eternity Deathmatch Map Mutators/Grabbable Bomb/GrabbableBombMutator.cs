using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class GrabbableBombMutator : DeathmatchMutator
    {
        public GrabbableBombMutator(DeathmatchParams Params)
            : base("Grabbable Bomb", "Add throw", Params)
        {
        }

        public override void OnSquadSelected(ActionPanel PanelOwner, int ActivePlayerIndex, int ActiveSquadIndex)
        {
            Vector3 ActiveSquadPosition = Params.Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex].Position + new Vector3(0.5f, 0.5f, 0f);

            foreach (PERAttack ActiveAttack in Params.Map.ListPERAttack)
            {
                if (ActiveAttack.IsOnGround && ActiveSquadPosition.X == ActiveAttack.Position.X
                    && ActiveSquadPosition.Y == ActiveAttack.Position.Y && ActiveSquadPosition.Z == ActiveAttack.Position.Z)
                {
                    PanelOwner.AddChoiceToCurrentPanel(new ActionPanelThrowBomb(Params.Map, ActivePlayerIndex, ActiveSquadIndex, ActiveAttack));
                    return;
                }
            }
        }
    }
}
