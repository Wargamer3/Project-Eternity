using System;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public abstract class ConquestMutator : Mutator
    {
        protected readonly ConquestParams Params;

        protected ConquestMutator(ConquestParams Params)
        {
            this.Params = Params;
        }

        protected ConquestMutator(string Name, string Description, ConquestParams Params)
            : base(Name, Description)
        {
            this.Params = Params;
        }

        public abstract void OnSquadSelected(ActionPanel PanelOwner, int ActivePlayerIndex, int ActiveUnitIndex);
    }
}
