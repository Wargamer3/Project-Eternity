using System;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchMutator : Mutator
    {
        protected readonly DeathmatchParams Params;

        protected DeathmatchMutator(DeathmatchParams Params)
        {
            this.Params = Params;
        }

        protected DeathmatchMutator(string Name, string Description, DeathmatchParams Params)
            : base(Name, Description)
        {
            this.Params = Params;
        }

        public virtual void OnSquadSelected(ActionPanel PanelOwner, int ActivePlayerIndex, int ActiveSquadIndex)
        {
        }
    }
}
