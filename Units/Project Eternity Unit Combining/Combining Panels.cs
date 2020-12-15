using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.DeathmatchMap;

namespace ProjectEternity.Core.Units.Combining
{
    public class ActionPanelCombine : ActionPanelDeathmatch
    {
        private UnitCombining ActiveUnit;

        public ActionPanelCombine(DeathmatchMap Map, UnitCombining ActiveUnit)
            : base("Combine", Map)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
            Squad FoundCombiningUnit = GetFoundCombiningUnit();

            ActiveUnit.Combine(FoundCombiningUnit);

            RemoveAllSubActionPanels();
        }

        private Squad GetFoundCombiningUnit()
        {
            string LeaderName = ActiveUnit.ArrayCombiningUnitName[0];
            foreach (Squad ActiveSquad in ActiveUnit.ListFoundCombiningUnit)
            {
                if (LeaderName.Contains(ActiveSquad.CurrentLeader.FullName))
                {
                    return ActiveSquad;
                }
            }

            return null;
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelSplit : ActionPanelDeathmatch
    {
        private UnitCombining ActiveUnit;

        public ActionPanelSplit(DeathmatchMap Map, UnitCombining ActiveUnit)
            : base("Split", Map)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
            ActiveUnit.Uncombine();

            RemoveAllSubActionPanels();
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
