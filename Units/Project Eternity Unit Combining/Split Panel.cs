using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Units.Combining
{
    public class ActionPanelSplit : ActionPanelDeathmatch
    {
        private const string PanelName = "Split";

        private UnitCombining ActiveUnit;

        public ActionPanelSplit(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelSplit(DeathmatchMap Map, UnitCombining ActiveUnit)
            : base(PanelName, Map)
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

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelSplit(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
