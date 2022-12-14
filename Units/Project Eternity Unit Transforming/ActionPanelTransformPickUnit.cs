using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Units.Transforming
{
    public class ActionPanelTransformPickUnit : ActionPanelDeathmatch
    {
        private const string PanelName = "FormationUnit";

        private int ActivePlayerIndex;
        private int SelectedUnitIndex;
        private Squad ActiveSquad;
        private bool ShowSquadMembers;

        public ActionPanelTransformPickUnit(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelTransformPickUnit(DeathmatchMap Map, int ActivePlayerIndex, Squad ActiveSquad, bool ShowSquadMembers)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquad = ActiveSquad;
            this.ShowSquadMembers = ShowSquadMembers;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                if (!(ActiveSquad[SelectedUnitIndex] is UnitTransforming) ||
                    ((UnitTransforming)ActiveSquad[SelectedUnitIndex]).PermanentTransformation)
                    return;

                AddToPanelListAndSelect(new ActionPanelTransform2Wingman(Map, ActivePlayerIndex, SelectedUnitIndex, ActiveSquad, ShowSquadMembers));
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
                SelectedUnitIndex += (SelectedUnitIndex < ActiveSquad.UnitsAliveInSquad - 1) ? 1 : 0;
                Map.sndSelection.Play();
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            SelectedUnitIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[BR.ReadInt32()];
            ShowSquadMembers = BR.ReadBoolean();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(SelectedUnitIndex);
            BW.AppendInt32(Map.ListPlayer[ActivePlayerIndex].ListSquad.IndexOf(ActiveSquad));
            BW.AppendBoolean(ShowSquadMembers);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelTransformPickUnit(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X, Y;

            DrawNextChoice(g);
            UnitTransforming TempUnit = (UnitTransforming)ActiveSquad[SelectedUnitIndex];

            X = (Map.CursorPosition.X + 1 - Map.CameraPosition.X) * Map.TileSize.X;
            Y = (Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

            if (X + MinActionMenuWidth + MinActionMenuWidth >= Constants.Width)
                X = Constants.Width - MinActionMenuWidth - MinActionMenuWidth;

            for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; ++U)
            {
                GameScreen.DrawBox(g, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), MinActionMenuWidth, PannelHeight, Color.White);
                if (!(ActiveSquad[U] is UnitTransforming) ||
                    ((UnitTransforming)ActiveSquad[SelectedUnitIndex]).PermanentTransformation)
                    TextHelper.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
                else
                    TextHelper.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
            }
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth, (int)Y + SelectedUnitIndex * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));
        }
    }
}
