using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Magic;
using ProjectEternity.Core.Online;
using static ProjectEternity.Core.ProjectileParams;

namespace ProjectEternity.Units.Magic
{
    public class ActionPanelSpellEditor : ActionPanel
    {
        private MagicSpell ActiveSpell;
        private MagicEditor ActiveMagicEditor;
        private ProjectileContext GlobalProjectileContext;
        private SharedProjectileParams SharedParams;

        public ActionPanelSpellEditor()
            : base("Dummy", null, true)
        {
        }

        public ActionPanelSpellEditor(ActionPanelHolder ListActionMenuChoice, MagicSpell ActiveSpell, ProjectileContext GlobalProjectileContext, SharedProjectileParams SharedParams)
            : base(ActiveSpell.Name, ListActionMenuChoice, true)
        {
            this.ActiveSpell = ActiveSpell;
            this.GlobalProjectileContext = GlobalProjectileContext;
            this.SharedParams = SharedParams;
        }

        public override void OnSelect()
        {
            ActiveMagicEditor = new MagicEditor(ActiveSpell, GlobalProjectileContext, SharedParams);
            ActiveMagicEditor.Load();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            ActiveMagicEditor.Update(gameTime);
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelSpellEditor();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActiveMagicEditor.Draw(g);
        }

        protected override void OnCancelPanel()
        {
        }
    }
}
