using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelMainMenuEnd : ActionPanelConquest
    {
        public ActionPanelMainMenuEnd(ConquestMap Map)
            : base("End", Map)
        {
        }

        public override void OnSelect()
        {
            RemoveAllActionPanels();
            ActionPanelPhaseChange EndPhase = new ActionPanelPhaseChange(Map);
            EndPhase.ActiveSelect = true;
            ListActionMenuChoice.AddToPanelListAndSelect(EndPhase);
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
            return new ActionPanelMainMenuEnd(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
