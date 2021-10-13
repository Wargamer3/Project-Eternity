using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.Units.Magic
{
    public class ActionPanelChannelExternalMana : ActionPanel
    {
        private UnitMagic ActiveUnit;

        public ActionPanelChannelExternalMana()
            : base("Channel External Mana", null, true)
        {
        }


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

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelChannelExternalMana();
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        protected override void OnCancelPanel()
        {
        }
    }
}
