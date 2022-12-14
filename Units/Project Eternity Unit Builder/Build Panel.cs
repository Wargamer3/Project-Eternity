using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Units.Builder
{
    public class ActionPanelBuild : ActionPanelDeathmatch
    {
        private const string PanelName = "Build";

        private int SelectedUnitIndex;
        UnitBuilder ActiveUnit;

        public ActionPanelBuild(DeathmatchMap Map)
                : base(PanelName, Map, true)
        {
        }

        public ActionPanelBuild(DeathmatchMap Map, UnitBuilder ActiveUnit)
                : base(PanelName, Map, true)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
            SelectedUnitIndex = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                AddToPanelListAndSelect(new ActionPanelBuildUnit(Map, ActiveUnit));
                Map.sndConfirm.Play();
            }
            else if (InputHelper.InputCancelPressed() || MouseHelper.InputRightButtonReleased())
            {
            }
            else if (InputHelper.InputUpPressed())
            {
                SelectedUnitIndex -= (SelectedUnitIndex > 0) ? 1 : 0;
                Map.sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed())
            {
                SelectedUnitIndex += (SelectedUnitIndex < 0) ? 1 : 0;
                Map.sndSelection.Play();
            }
        }

        public override void DoRead(ByteReader BR)
        {
            SelectedUnitIndex = BR.ReadInt32();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(SelectedUnitIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBuild(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int X = (int)(Map.CursorPosition.X + 1 - Map.CameraPosition.X) * Map.TileSize.X;
            int Y = (int)(Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

            if (X + MinActionMenuWidth + MinActionMenuWidth >= Constants.Width)
                X = Constants.Width - MinActionMenuWidth - MinActionMenuWidth;
            
            for (int U = 0; U < 1; ++U)
            {
                GameScreen.DrawBox(g, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), MinActionMenuWidth, PannelHeight, Color.White);
                TextHelper.DrawText(g, "Electric Wall", new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
            }
            g.Draw(GameScreen.sprPixel, new Rectangle(X + MinActionMenuWidth, Y + SelectedUnitIndex * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));
        }

        protected override void OnCancelPanel()
        {
        }
    }
}
