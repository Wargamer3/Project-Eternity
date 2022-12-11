using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelLandChainUpdate : ActionPanelSorcererStreet
    {
        private const string PanelName = "DefenderReplace";

        public ActionPanelLandChainUpdate(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            RemoveFromPanelList(this);
            Map.EndPlayerPhase();
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActionPanelBattle.ReadPlayerInfo(BR, Map);
            OnSelect();
        }

        public override void DoWrite(ByteWriter BW)
        {
            ActionPanelBattle.WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelLandChainUpdate(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw creature being replaced
        }
    }
}
