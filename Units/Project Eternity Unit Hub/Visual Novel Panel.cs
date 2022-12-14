using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.Core.Units.Hub
{
    public class ActionPanelVisualNovel : ActionPanel
    {
        private VisualNovel ActiveVisualNovel;

        public ActionPanelVisualNovel(ActionPanelHolder ListActionMenuChoice, VisualNovel ActiveVisualNovel)
            : base("Visual Novel", ListActionMenuChoice, false)
        {
            this.ActiveVisualNovel = ActiveVisualNovel;
            ActiveVisualNovel.OnVisualNovelEnded += OnVisualNovelEnded;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            ActiveVisualNovel.Update(gameTime);
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelVisualNovel(null, null);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActiveVisualNovel.Draw(g);
        }

        public override void OnSelect()
        {
        }

        protected override void OnCancelPanel()
        {
        }

        private void OnVisualNovelEnded()
        {
            RemoveFromPanelList(this);
        }
    }
}
