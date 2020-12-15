using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelDeployAtPosition : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;

        public ActionPanelDeployAtPosition(DeathmatchMap Map, Squad ActiveSquad)
            : base("Deploy 2", Map, false)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Remove(ActiveSquad);
            RemoveAllSubActionPanels();
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
