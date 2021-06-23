using Microsoft.Xna.Framework;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using ProjectEternity.GameScreens;

namespace ProjectEternity.Core.Units.Transforming
{
    public class ActionPanelTransform : ActionPanelDeathmatch
    {
        private int ActionMenuSwitchSquadCursor;
        private Squad ActiveSquad;

        public ActionPanelTransform(DeathmatchMap Map, Squad ActiveSquad)
            : base("Transform", Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            int NumberOfTransformingUnitsInSquad = 0;

            for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                if (ActiveSquad[U] is UnitTransforming)
                {
                    ActionMenuSwitchSquadCursor = U;
                    UnitTransforming ActiveUnit = (UnitTransforming)ActiveSquad[U];
                    if (!ActiveUnit.PermanentTransformation)
                        ++NumberOfTransformingUnitsInSquad;
                }
            }

            if (NumberOfTransformingUnitsInSquad == 1)
                AddToPanelListAndSelect(new ActionPanelTransform2Wingman((UnitTransforming)ActiveSquad[ActionMenuSwitchSquadCursor], ActiveSquad, false, Map));
            else if (NumberOfTransformingUnitsInSquad >= 2)
                AddToPanelListAndSelect(new ActionPanelTransform2Wingman((UnitTransforming)ActiveSquad[ActionMenuSwitchSquadCursor], ActiveSquad, true, Map));

            RemoveFromPanelList(this);
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelTransformPickUnit : ActionPanelDeathmatch
    {
        private int SelectedUnitIndex;
        private Squad ActiveSquad;

        public ActionPanelTransformPickUnit(DeathmatchMap Map, Squad ActiveSquad)
            : base("FormationUnit", Map)
        {
            this.ActiveSquad = ActiveSquad;
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

                AddToPanelListAndSelect(new ActionPanelTransform2Wingman(((UnitTransforming)ActiveSquad[SelectedUnitIndex]), ActiveSquad, true, Map));
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
                    GameScreen.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
                else
                    GameScreen.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
            }
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth, (int)Y + SelectedUnitIndex * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));
        }
    }

    public class ActionPanelTransform2Wingman : ActionPanelDeathmatch
    {
        private int TransformationChoice;
        private readonly UnitTransforming TransformingUnit;
        private Squad ActiveSquad;
        private bool ShowSquadMembers;

        public ActionPanelTransform2Wingman(UnitTransforming TransformingUnit, Squad ActiveSquad, bool ShowSquadMembers, DeathmatchMap Map)
            : base("Formation2Wingman", Map)
        {
            this.TransformingUnit = TransformingUnit;
            this.ActiveSquad = ActiveSquad;
            this.ShowSquadMembers = ShowSquadMembers;
        }

        public override void OnSelect()
        {
            for (int i = 0; i < TransformingUnit.ArrayTransformingUnit.Length; ++i)
            {
                AddChoiceToCurrentPanel(new ActionPanelConfirmTransform(TransformingUnit, i, ActiveSquad, Map,
                    TransformingUnit.CanTransform(i, Map.ActiveSquad.CurrentWingmanA, Map.ActiveSquad.CurrentWingmanB)));
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                if (TransformingUnit.ArrayTransformingUnit[ActionMenuCursor].WillRequirement >= 0 && TransformingUnit.PilotMorale >= TransformingUnit.ArrayTransformingUnit[ActionMenuCursor].WillRequirement)
                {
                    AddToPanelListAndSelect(new ActionPanelTranform2WingmanWill(ActionMenuCursor, TransformingUnit, ActiveSquad, ShowSquadMembers, Map));
                }
                else if (TransformingUnit.ArrayTransformingUnit[ActionMenuCursor].TurnLimit >= 0)
                {
                    AddToPanelListAndSelect(new ActionPanelTranform2WingmanTurn(ActionMenuCursor, TransformingUnit, ActiveSquad, ShowSquadMembers, Map));
                }
                else
                {
                    TransformingUnit.ChangeUnit(ActionMenuCursor);
                    Map.UpdateSquadCurrentMovement(ActiveSquad);
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
                        GameScreen.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
                    else
                        GameScreen.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
                }
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth, (int)Y + ActiveSquad.IndexOf(TransformingUnit) * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));
            }

            /*
             * Might be needed if we use wingmans
             * for (int U = 0; U < TransformingUnit.ArrayTransformingUnit.Length; ++U)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth + MinActionMenuWidth, (int)Y + U * PannelHeight, 50, 20), Color.Navy);
                if (TransformingUnit.CanTransform(U, Map.ActiveSquad.CurrentWingmanA, Map.ActiveSquad.CurrentWingmanB))
                    GameScreen.DrawText(g, TransformingUnit.ArrayTransformingUnit[U].TransformingUnitName, new Vector2(X + MinActionMenuWidth + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
                else
                    GameScreen.DrawText(g, TransformingUnit.ArrayTransformingUnit[U].TransformingUnitName, new Vector2(X + MinActionMenuWidth + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
            }
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth + MinActionMenuWidth, (int)Y + TransformationChoice * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));*/
        }
    }

    public class ActionPanelTranform2WingmanWill : ActionPanelDeathmatch
    {
        private readonly int TransformationChoice;
        private readonly UnitTransforming TransformingUnit;
        private Squad ActiveSquad;
        private bool ShowSquadMembers;

        public ActionPanelTranform2WingmanWill(int TransformationChoice, UnitTransforming TransformingUnit, Squad ActiveSquad, bool ShowSquadMembers, DeathmatchMap Map)
            : base("Transform2Will", Map)
        {
            this.TransformingUnit = TransformingUnit;
            this.TransformationChoice = TransformationChoice;
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
                TransformingUnit.ChangeUnit(TransformationChoice);
                Map.UpdateSquadCurrentMovement(ActiveSquad);

                Map.ActiveSquadIndex = -1;
                Map.sndConfirm.Play();
            }
            else if (InputHelper.InputCancelPressed() || MouseHelper.InputRightButtonReleased())
            {
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X, Y;

            DrawNextChoice(g);

            X = (Map.CursorPosition.X + 1 - Map.CameraPosition.X) * Map.TileSize.X;
            Y = (Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

            if (X + MinActionMenuWidth + MinActionMenuWidth >= Constants.Width)
                X = Constants.Width - MinActionMenuWidth - MinActionMenuWidth;

            if (ShowSquadMembers)
            {
                for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; ++U)
                {
                    GameScreen.DrawBox(g, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), MinActionMenuWidth, PannelHeight, Color.White);
                    if (!(ActiveSquad[U] is UnitTransforming) ||
                        TransformingUnit.PermanentTransformation)
                        GameScreen.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
                    else
                        GameScreen.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
                }
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth, (int)Y + ActiveSquad.IndexOf(TransformingUnit) * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));
            }

            for (int U = 0; U < TransformingUnit.ArrayTransformingUnit.Length; ++U)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth + MinActionMenuWidth, (int)Y + U * PannelHeight, 50, 20), Color.Navy);
                if (TransformingUnit.CanTransform(U, ActiveSquad.CurrentWingmanA, ActiveSquad.CurrentWingmanB))
                    GameScreen.DrawText(g, TransformingUnit.ArrayTransformingUnit[U].TransformingUnitName, new Vector2(X + MinActionMenuWidth + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
                else
                    GameScreen.DrawText(g, TransformingUnit.ArrayTransformingUnit[U].TransformingUnitName, new Vector2(X + MinActionMenuWidth + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
            }

            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth, (int)Y + TransformationChoice * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));

            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width / 2 - 100, Constants.Height / 2 - 25, 200, 50), Color.Navy);
            GameScreen.DrawText(g, ActiveSquad.CurrentLeader.RelativePath + " will transform into " + TransformingUnit.ArrayTransformingUnit[TransformationChoice].TransformingUnitName + ".\n\rThis will be permanent. Continue?", new Vector2(Constants.Width / 2 - 100, Constants.Height / 2 - 25), Color.White);
        }
    }

    public class ActionPanelTranform2WingmanTurn : ActionPanelDeathmatch
    {
        private readonly int TransformationChoice;
        private readonly UnitTransforming TransformingUnit;
        private Squad ActiveSquad;
        private bool ShowSquadMembers;

        public ActionPanelTranform2WingmanTurn(int TransformationChoice, UnitTransforming TransformingUnit, Squad ActiveSquad, bool ShowSquadMembers, DeathmatchMap Map)
            : base("Transform2Turn", Map)
        {
            this.TransformingUnit = TransformingUnit;
            this.TransformationChoice = TransformationChoice;
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
                TransformingUnit.ChangeUnit(TransformationChoice);
                Map.UpdateSquadCurrentMovement(ActiveSquad);

                Map.ActiveSquadIndex = -1;
                Map.sndConfirm.Play();
            }
            else if (InputHelper.InputCancelPressed() || MouseHelper.InputRightButtonReleased())
            {
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X, Y;

            DrawNextChoice(g);

            X = (Map.CursorPosition.X + 1 - Map.CameraPosition.X) * Map.TileSize.X;
            Y = (Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

            if (X + MinActionMenuWidth + MinActionMenuWidth >= Constants.Width)
                X = Constants.Width - MinActionMenuWidth - MinActionMenuWidth;

            for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; ++U)
            {
                GameScreen.DrawBox(g, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), MinActionMenuWidth, PannelHeight, Color.White);
                if (!(ActiveSquad[U] is UnitTransforming) || TransformingUnit.PermanentTransformation)
                    GameScreen.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
                else
                    GameScreen.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
            }
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth, (int)Y + ActiveSquad.IndexOf(TransformingUnit) * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));

            for (int U = 0; U < TransformingUnit.ArrayTransformingUnit.Length; ++U)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth + MinActionMenuWidth, (int)Y + U * PannelHeight, 50, 20), Color.Navy);
                if (TransformingUnit.CanTransform(U, ActiveSquad.CurrentWingmanA, ActiveSquad.CurrentWingmanB))
                    GameScreen.DrawText(g, TransformingUnit.ArrayTransformingUnit[U].TransformingUnitName, new Vector2(X + MinActionMenuWidth + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
                else
                    GameScreen.DrawText(g, TransformingUnit.ArrayTransformingUnit[U].TransformingUnitName, new Vector2(X + MinActionMenuWidth + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
            }
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width / 2 - 100, Constants.Height / 2 - 25, 200, 50), Color.Navy);
            GameScreen.DrawText(g, ActiveSquad.CurrentLeader.RelativePath + " will transform into " + TransformingUnit.ArrayTransformingUnit[TransformationChoice].TransformingUnitName + " for\n\r" + TransformingUnit.ArrayTransformingUnit[TransformationChoice].TurnLimit + " turns. Continue?", new Vector2(Constants.Width / 2 - 100, Constants.Height / 2 - 25), Color.White);
        }
    }
}
