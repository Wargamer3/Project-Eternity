using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Units.Magic
{
    public class ActionPanelChannelExternalMana : ActionPanel
    {
        private UnitMagic ActiveUnit;

        public ActionPanelChannelExternalMana(ActionPanelHolder ListActionMenuChoice, UnitMagic ActiveUnit)
            : base("Channel External Mana", ListActionMenuChoice, true)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        protected override void OnCancelPanel()
        {
        }
    }
}
