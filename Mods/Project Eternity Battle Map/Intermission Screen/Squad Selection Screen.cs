using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class SquadSelection : GameScreen
    {
        private enum MenuChoices { None, SquadSelected, LeaderStats, WingmanAStats, WingmanBStats, RenameSquad, SetLeader, SetWingmanA, SetWingmanB, DisbandSquad };

        private readonly Roster PlayerRoster;

        MenuChoices Stage;
        MenuChoices MenuChoice;
        int ActiveSquadIndex;
        int PageCurrent;
        int PageMax;
        int MaxItemToDraw;
        const int ItemPerPage = 8;

        int ActiveUnitMinIndex;
        int ActiveUnitIndex;

        Color CursorColor;
        
        Texture2D sprBackground;
        Texture2D sprCursor;
        SpriteFont fntArial8;
        SpriteFont fntArial12;
        SpriteFont fntArial14;

        Squad ActiveSquad;

        private List<Squad> ListPresentSquad;
        private List<Unit> ListPresentUnit;

        public SquadSelection(Roster PlayerRoster)
            : base()
        {
            this.PlayerRoster = PlayerRoster;

            RequireDrawFocus = true;
            Stage = MenuChoices.None;
            MenuChoice = MenuChoices.None;
            ActiveSquadIndex = 0;
            PageCurrent = 1;
            ActiveSquad = null;
        }

        public override void Load()
        {
            sprBackground = Content.Load<Texture2D>("Intermission Screens/Squad Selection");
            sprCursor = Content.Load<Texture2D>("Intermission Screens/Squad Selection Cursor");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntArial14 = Content.Load<SpriteFont>("Fonts/Arial");

            CursorColor = Color.FromNonPremultiplied(255, 255, 255, 128);

            ListPresentSquad = PlayerRoster.TeamSquads.GetPresent();
            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();

            PageMax = (int)Math.Ceiling((ListPresentSquad.Count + 1) / (float)ItemPerPage);
            MaxItemToDraw = (Constants.Height - 100) / fntArial12.LineSpacing - 1;
        }

        public override void Update(GameTime gameTime)
        {
            switch (Stage)
            {
                #region None

                case MenuChoices.None:
                    if (InputHelper.InputUpPressed())
                    {
                        if (ActiveSquadIndex > 0)
                            ActiveSquadIndex--;
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        if (ActiveSquadIndex < ItemPerPage && ActiveSquadIndex + 1 + (PageCurrent - 1) * ItemPerPage < ListPresentSquad.Count + 1)
                            ActiveSquadIndex++;
                    }
                    else if (InputHelper.InputLeftPressed())
                    {
                        if (PageCurrent > 1)
                            PageCurrent--;
                    }
                    else if (InputHelper.InputRightPressed())
                    {
                        if (PageCurrent * 8 < ListPresentSquad.Count)
                            PageCurrent++;
                    }
                    else if (InputHelper.InputConfirmPressed())
                    {
                        if (ActiveSquadIndex + (PageCurrent - 1) * ItemPerPage == ListPresentSquad.Count)
                        {
                            Squad NewSquad = new Squad("New Squad");
                            NewSquad.TeamTags.AddTag("Present");
                            NewSquad.IsPlayerControlled = true;
                            PlayerRoster.TeamSquads.Add(NewSquad);
                            ListPresentSquad = PlayerRoster.TeamSquads.GetPresent();
                            PageMax = (int)Math.Ceiling((ListPresentSquad.Count + 1) / (float)ItemPerPage);
                        }
                        else
                        {
                            ActiveSquad = ListPresentSquad[ActiveSquadIndex + (PageCurrent - 1) * ItemPerPage];
                            Stage = MenuChoices.SquadSelected;
                            MenuChoice = MenuChoices.LeaderStats;

                            ActiveUnitMinIndex = 0;
                            ActiveUnitIndex = 0;
                        }
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        RemoveScreen(this);
                    }
                    break;

                #endregion

                #region Squad Selected

                case MenuChoices.SquadSelected:

                    if (InputHelper.InputUpPressed())
                    {
                        if (MenuChoice > MenuChoices.LeaderStats)
                            MenuChoice--;
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        if (MenuChoice < MenuChoices.DisbandSquad)
                            MenuChoice++;
                    }
                    else if (InputHelper.InputConfirmPressed())
                    {
                        Stage = MenuChoice;
                        switch (MenuChoice)
                        {
                            case MenuChoices.LeaderStats:

                                break;

                            case MenuChoices.WingmanAStats:

                                break;

                            case MenuChoices.WingmanBStats:

                                break;

                            case MenuChoices.RenameSquad:

                                break;

                            case MenuChoices.SetLeader:

                                break;

                            case MenuChoices.SetWingmanA:

                                break;

                            case MenuChoices.SetWingmanB:

                                break;

                            case MenuChoices.DisbandSquad:

                                break;
                        }
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        Stage = MenuChoices.None;
                    }
                    break;

                #endregion

                #region Set Leader

                case MenuChoices.SetLeader:
                    if (InputHelper.InputConfirmPressed())
                    {
                        Stage = MenuChoices.SquadSelected;

                        Unit OldLeader = ActiveSquad.CurrentLeader;
                        if (OldLeader != null)
                        {
                            PlayerRoster.TeamUnits.Add(OldLeader);
                            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();
                        }

                        Unit NewLeader = ListPresentUnit[ActiveUnitIndex];
                        PlayerRoster.TeamUnits.Remove(NewLeader);
                        ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();

                        ActiveSquad.SetNewLeader(NewLeader);
                    }
                    else
                        UpdateUnitSelection();
                    break;

                case MenuChoices.SetWingmanA:
                    if (InputHelper.InputConfirmPressed())
                    {
                        Stage = MenuChoices.SquadSelected;

                        Unit OldWingmanA = ActiveSquad[1];
                        if (OldWingmanA != null)
                        {
                            PlayerRoster.TeamUnits.Add(OldWingmanA);
                            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();
                        }

                        Unit NewWingmanA = ListPresentUnit[ActiveUnitIndex];
                        PlayerRoster.TeamUnits.Remove(NewWingmanA);
                        ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();

                        ActiveSquad.SetNewWingmanA(NewWingmanA);
                    }
                    else
                        UpdateUnitSelection();
                    break;

                case MenuChoices.SetWingmanB:
                    if (InputHelper.InputConfirmPressed())
                    {
                        Stage = MenuChoices.SquadSelected;

                        Unit OldWingmanB = ActiveSquad[2];
                        if (OldWingmanB != null)
                        {
                            PlayerRoster.TeamUnits.Add(OldWingmanB);
                            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();
                        }

                        Unit NewWingmanB = ListPresentUnit[ActiveUnitIndex];
                        PlayerRoster.TeamUnits.Remove(NewWingmanB);
                        ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();

                        ActiveSquad.SetNewWingmanB(NewWingmanB);
                    }
                    else
                        UpdateUnitSelection();
                    break;

                #endregion

                default:
                    if (InputHelper.InputCancelPressed())
                    {
                        Stage = MenuChoices.None;
                    }
                    break;
            }
        }

        private void UpdateUnitSelection()
        {
            if (InputHelper.InputCancelPressed())
            {
                Stage = MenuChoices.SquadSelected;
            }
            else if (InputHelper.InputUpPressed())
            {
                ActiveUnitIndex--;
                if (ActiveUnitIndex < 0)
                {
                    ActiveUnitIndex = ListPresentUnit.Count - 1;
                    ActiveUnitMinIndex = Math.Max(0, ListPresentUnit.Count - MaxItemToDraw);
                }
                else if (ActiveUnitIndex < ActiveUnitMinIndex)
                    ActiveUnitMinIndex = ActiveUnitIndex;
            }
            else if (InputHelper.InputDownPressed())
            {
                ActiveUnitIndex++;
                if (ActiveUnitIndex >= ListPresentUnit.Count)
                {
                    ActiveUnitIndex = 0;
                    ActiveUnitMinIndex = 0;
                }
                else if (ActiveUnitIndex >= MaxItemToDraw + ActiveUnitMinIndex)
                    ActiveUnitMinIndex = ActiveUnitIndex - MaxItemToDraw + 1;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            g.Draw(sprBackground, new Vector2(0, 0), Color.White);
            g.DrawString(fntArial12, PageCurrent.ToString(), new Vector2(604, 0), Color.White);
            g.DrawString(fntArial12, PageMax.ToString(), new Vector2(624, 0), Color.White);
            //Unit drawing.
            for (int S = (PageCurrent - 1) * ItemPerPage, Pos = 0; S < ListPresentSquad.Count + 1 && S < PageCurrent * ItemPerPage; S++, Pos++)
			{
                if (S < ListPresentSquad.Count)
                {
                    Squad DrawnSquad = ListPresentSquad[S];

                    string OutputText = DrawnSquad.SquadName + "(";

                    if (DrawnSquad.CurrentLeader != null)
                        OutputText += DrawnSquad.CurrentLeader.RelativePath;

                    for (int U = 1; U < DrawnSquad.UnitsAliveInSquad; ++U)
                    {
                        OutputText += " / " + DrawnSquad[U].RelativePath;
                    }

                    OutputText += ") [";
                    if (DrawnSquad.IsNameLocked)
                        OutputText += "*";
                    else
                        OutputText += " ";
                    OutputText += ",";
                    if (DrawnSquad.IsLeaderLocked)
                        OutputText += "*";
                    else
                        OutputText += " ";
                    OutputText += ",";
                    if (DrawnSquad.IsWingmanALocked)
                        OutputText += "*";
                    else
                        OutputText += " ";
                    OutputText += ",";
                    if (DrawnSquad.IsWingmanBLocked)
                        OutputText += "*";
                    else
                        OutputText += " ";
                    OutputText += "]";
                    g.DrawString(fntArial12, OutputText, new Vector2(14, 46 + Pos * 46), Color.White);
                }
                else
                    g.DrawString(fntArial12, "Create New Squad", new Vector2(14, 46 + Pos * 46), Color.White);
			}
            g.Draw(sprCursor, new Vector2(2, 46 + ActiveSquadIndex * 46), Color.White);

            //Squad selection right menu.
            g.DrawString(fntArial12, "Leader Stats",    new Vector2(438, 334), Color.White);
            g.DrawString(fntArial12, "Wing 1 Stats",    new Vector2(438, 352), Color.White);
            g.DrawString(fntArial12, "Wing 2 Stats",    new Vector2(438, 370), Color.White);
            g.DrawString(fntArial12, "Rename Squad", new Vector2(438, 388), ActiveSquad != null && ActiveSquad.IsNameLocked ? Color.Gray : Color.White);
            g.DrawString(fntArial12, "Leader Set", new Vector2(438, 406), ActiveSquad != null && ActiveSquad.IsLeaderLocked ? Color.Gray : Color.White);
            g.DrawString(fntArial12, "Wing 1 Set", new Vector2(438, 424), ActiveSquad != null && ActiveSquad.IsWingmanALocked ? Color.Gray : Color.White);
            g.DrawString(fntArial12, "Wing 2 Set", new Vector2(438, 442), ActiveSquad != null && ActiveSquad.IsWingmanBLocked ? Color.Gray : Color.White);
            g.DrawString(fntArial12, "Disband Squad",   new Vector2(438, 460), Color.White);

            switch (Stage)
            {
                case MenuChoices.SquadSelected:
                    g.Draw(sprPixel, new Rectangle(436, 334 + (int)(MenuChoice - 2) * 18, 200, 18), Color.FromNonPremultiplied(255, 255, 255, 127));
                    break;

                case MenuChoices.SetLeader:
                case MenuChoices.SetWingmanA:
                case MenuChoices.SetWingmanB:
                    g.Draw(sprPixel, new Rectangle(50, 50, Constants.Width - 100, Constants.Height - 100), Color.Gray);
                    g.DrawString(fntArial12, "Unit Type", new Vector2(50, 50), Color.White);
                    g.DrawString(fntArial12, "Unit Name", new Vector2(160, 50), Color.White);
                    for (int U = MaxItemToDraw - 1; U >= 0; --U)
                    {
                        if (U + ActiveUnitMinIndex >= ListPresentUnit.Count)
                            continue;

                        g.DrawString(fntArial12, ListPresentUnit[U + ActiveUnitMinIndex].UnitTypeName, new Vector2(50, 70 + fntArial12.LineSpacing * U), Color.White);
                        g.DrawString(fntArial12, ListPresentUnit[U + ActiveUnitMinIndex].RelativePath, new Vector2(160, 70 + fntArial12.LineSpacing * U), Color.White);
                    }
                    g.Draw(sprPixel, new Rectangle(50, 50 + (ActiveUnitIndex + 1 - ActiveUnitMinIndex) * fntArial12.LineSpacing, Constants.Width - 100, fntArial12.LineSpacing), CursorColor);
                    break;
            }
        }
    }
}
