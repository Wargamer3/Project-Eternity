using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Units.Combining
{
    public class ActionPanelCombine : ActionPanelDeathmatch
    {
        private const string PanelName = "Combine";

        private UnitCombining ActiveUnit;

        public ActionPanelCombine(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelCombine(DeathmatchMap Map, UnitCombining ActiveUnit)
            : base(PanelName, Map)
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
                if (LeaderName.Contains(ActiveSquad.CurrentLeader.RelativePath))
                {
                    return ActiveSquad;
                }
            }

            return null;
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
            return new ActionPanelCombine(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
