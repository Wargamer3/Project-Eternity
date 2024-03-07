using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelDialogPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "DialogPhase";

        private int ActivePlayerIndex;
        private Player ActivePlayer;

        private static DynamicText Text;

        public ActionPanelDialogPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelDialogPhase(SorcererStreetMap Map, int ActivePlayerIndex, Player ActivePlayer)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActivePlayer = ActivePlayer;

            if (Text == null)
            {
                Text = new DynamicText();
                Text.TextMaxWidthInPixel = Constants.Width;
                Text.LineHeight = 20;
                Text.ListProcessor.Add(new RegularTextProcessor(Text));
                Text.ListProcessor.Add(new IconProcessor(Text));
                Text.ListProcessor.Add(new DefaultTextProcessor(Text));
                Text.SetDefaultProcessor(new DefaultTextProcessor(Text));
                Text.Load(Map.Content);
            }

            //Text.ParseText(Message);
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Text.Update(gameTime);

        }

        protected override void OnCancelPanel()
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
            return new ActionPanelDialogPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            Text.Draw(g, new Vector2());
        }
    }
}
