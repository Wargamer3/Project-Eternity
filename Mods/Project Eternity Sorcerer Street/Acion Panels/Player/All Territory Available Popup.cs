using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelAllTerritoryAvailalblePopup : ActionPanelSorcererStreet
    {
        private const string PanelName = "AllTerritoryAvailalblePopup";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private double AITimer;

        public ActionPanelAllTerritoryAvailalblePopup(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelAllTerritoryAvailalblePopup(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!ActivePlayer.IsPlayerControlled)
            {
                AITimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (AITimer >= 1)
                {
                    RemoveFromPanelList(this);
                }
                return;
            }

            if (ActiveInputManager.InputConfirmPressed())
            {
                RemoveFromPanelList(this);
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAllTerritoryAvailalblePopup(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 720f;
            int BoxWidth = (int)(720 * Ratio);
            int BoxHeight = (int)(67 * 1 * Ratio);
            int BoxX = (Constants.Width - BoxWidth) / 2;
            int BoxY = Constants.Height - BoxHeight - (int)(74 * Ratio);
            MenuHelper.DrawBorderlessBox(g, new Vector2(BoxX, BoxY), BoxWidth, BoxHeight);

            g.DrawStringMiddleAligned(Map.fntMenuText, "All lands may be selected for territory commands.", new Vector2(BoxX + BoxWidth / 2, (int)(BoxY + 16 * Ratio)), Color.White);
            MenuHelper.DrawConfirmIcon(g, new Vector2(BoxX + BoxWidth - 70, BoxY + BoxHeight - 80));
        }
    }
}
