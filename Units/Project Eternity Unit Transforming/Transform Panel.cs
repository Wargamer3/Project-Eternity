using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Units.Transforming
{
    public class ActionPanelTransform : ActionPanelDeathmatch
    {
        private const string PanelName = "Transform";

        private int ActivePlayerIndex;
        private int ActionMenuSwitchSquadCursor;
        private Squad ActiveSquad;

        public ActionPanelTransform(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelTransform(DeathmatchMap Map, int ActivePlayerIndex, Squad ActiveSquad)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            int NumberOfTransformingUnitsInSquad = 0;

            for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                if (ActiveSquad[U] is UnitTransforming)
                {
                    ActionMenuSwitchSquadCursor = U;
                    UnitTransforming ActiveUnit = (UnitTransforming)ActiveSquad[U];
                    if (!ActiveUnit.PermanentTransformation)
                        ++NumberOfTransformingUnitsInSquad;
                }
            }

            if (NumberOfTransformingUnitsInSquad == 1)
                AddToPanelListAndSelect(new ActionPanelTransform2Wingman(Map, ActivePlayerIndex, ActionMenuSwitchSquadCursor, ActiveSquad, false));
            else if (NumberOfTransformingUnitsInSquad >= 2)
                AddToPanelListAndSelect(new ActionPanelTransform2Wingman(Map, ActivePlayerIndex, ActionMenuSwitchSquadCursor, ActiveSquad, true));

            RemoveFromPanelList(this);
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelTransform(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
