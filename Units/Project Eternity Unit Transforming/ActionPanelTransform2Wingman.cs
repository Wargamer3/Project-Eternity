using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Units.Transforming
{
    public class ActionPanelTransform2Wingman : ActionPanelDeathmatch
    {
        private const string PanelName = "Formation2Wingman";

        private int ActivePlayerIndex;
        private int TransformingUnitIndex;
        private Squad ActiveSquad;
        private bool ShowSquadMembers;
        private UnitTransforming TransformingUnit;
        private List<Unit> ListWingman;

        public ActionPanelTransform2Wingman(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelTransform2Wingman(DeathmatchMap Map, int ActivePlayerIndex, int TransformingUnitIndex, Squad ActiveSquad, bool ShowSquadMembers)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.TransformingUnitIndex = TransformingUnitIndex;
            this.ActiveSquad = ActiveSquad;
            this.ShowSquadMembers = ShowSquadMembers;
            TransformingUnit = (UnitTransforming)ActiveSquad[TransformingUnitIndex];
            ListWingman = new List<Unit>();
            for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 1; --U)
            {
                ListWingman.Add(ActiveSquad[U]);
            }
        }

        public override void OnSelect()
        {
            for (int i = 0; i < TransformingUnit.ArrayTransformingUnit.Length; ++i)
            {
                AddChoiceToCurrentPanel(new ActionPanelConfirmTransform(TransformingUnit, i, ActiveSquad, Map,
                    TransformingUnit.CanTransform(i, ListWingman)));
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                if (TransformingUnit.ArrayTransformingUnit[ActionMenuCursor].WillRequirement >= 0 && TransformingUnit.PilotMorale >= TransformingUnit.ArrayTransformingUnit[ActionMenuCursor].WillRequirement)
                {
                    AddToPanelListAndSelect(new ActionPanelTranform2WingmanWill(Map, ActivePlayerIndex, TransformingUnitIndex, ActionMenuCursor, ActiveSquad, ShowSquadMembers));
                }
                else if (TransformingUnit.ArrayTransformingUnit[ActionMenuCursor].TurnLimit >= 0)
                {
                    AddToPanelListAndSelect(new ActionPanelTranform2WingmanTurn(Map, ActivePlayerIndex, TransformingUnitIndex, ActionMenuCursor, ActiveSquad, ShowSquadMembers));
                }
                else
                {
                    TransformingUnit.ChangeUnit(ActionMenuCursor);
                }

                Map.sndConfirm.Play();
            }
            else if (InputHelper.InputCancelPressed() || MouseHelper.InputRightButtonReleased())
            {
                RemoveFromPanelList(this);
            }
            else if (InputHelper.InputUpPressed())
            {
                ActionMenuCursor -= (ActionMenuCursor > 0) ? 1 : 0;
                Map.sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed())
            {
                ActionMenuCursor += (ActionMenuCursor < TransformingUnit.ArrayTransformingUnit.Length - 1) ? 1 : 0;
                Map.sndSelection.Play();
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            TransformingUnitIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[BR.ReadInt32()];
            ShowSquadMembers = BR.ReadBoolean();
            TransformingUnit = (UnitTransforming)ActiveSquad[TransformingUnitIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(TransformingUnitIndex);
            BW.AppendInt32(Map.ListPlayer[ActivePlayerIndex].ListSquad.IndexOf(ActiveSquad));
            BW.AppendBoolean(ShowSquadMembers);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelTransform2Wingman(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X, Y;

            DrawNextChoice(g);

            X = (Map.CursorPosition.X + 1 - Map.CameraPosition.X) * Map.TileSize.X;
            Y = (Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

            if (X + MinActionMenuWidth + MinActionMenuWidth + MinActionMenuWidth >= Constants.Width)
                X = Constants.Width - MinActionMenuWidth - MinActionMenuWidth - MinActionMenuWidth;

            if (ShowSquadMembers)
            {
                for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; ++U)
                {
                    GameScreen.DrawBox(g, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), MinActionMenuWidth, PannelHeight, Color.White);
                    if (!(ActiveSquad[U] is UnitTransforming) || TransformingUnit.PermanentTransformation)
                        TextHelper.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
                    else
                        TextHelper.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
                }
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth, (int)Y + ActiveSquad.IndexOf(TransformingUnit) * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));
            }

            /*
             * Might be needed if we use wingmans
             * for (int U = 0; U < TransformingUnit.ArrayTransformingUnit.Length; ++U)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth + MinActionMenuWidth, (int)Y + U * PannelHeight, 50, 20), Color.Navy);
                if (TransformingUnit.CanTransform(U, Map.ActiveSquad.CurrentWingmanA, Map.ActiveSquad.CurrentWingmanB))
                    TextHelper.DrawText(g, TransformingUnit.ArrayTransformingUnit[U].TransformingUnitName, new Vector2(X + MinActionMenuWidth + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
                else
                    TextHelper.DrawText(g, TransformingUnit.ArrayTransformingUnit[U].TransformingUnitName, new Vector2(X + MinActionMenuWidth + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
            }
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth + MinActionMenuWidth, (int)Y + TransformationChoice * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));*/
        }
    }
}
