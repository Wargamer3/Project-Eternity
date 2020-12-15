using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class PilotSwapScreen : GameScreen
    {
        private readonly Roster PlayerRoster;

        int CursorIndexSubMenu;

        UnitListScreen UnitList;
        PilotListScreen PilotList;

        Unit SelectedUnit;

        Texture2D sprBackground;
        Texture2D sprBackgroundUnitSelection;
        SpriteFont fntArial8;
        SpriteFont fntArial12;
        SpriteFont fntFinlanderFont;

        private List<Unit> ListPresentUnit;
        private List<Character> ListPresentCharacter;

        int Stage;

        public PilotSwapScreen(Roster PlayerRoster)
            : base()
        {
            this.PlayerRoster = PlayerRoster;

            CursorIndexSubMenu = 0;
            Stage = -1;
        }

        public override void Load()
        {
            sprBackground = Content.Load<Texture2D>("Intermission Screens/Pilot Selection");
            sprBackgroundUnitSelection = Content.Load<Texture2D>("Intermission Screens/Unit Selection");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

            UnitList = new UnitListScreen(PlayerRoster);
            UnitList.Load();
            PilotList = new PilotListScreen(PlayerRoster);
            PilotList.Load();
            PilotList.ListCharacterInfo.Insert(0, new PilotListScreen.CharacterInfo(-1));

            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();
            ListPresentCharacter = PlayerRoster.TeamCharacters.GetPresent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Stage == -1)
            {
                UnitList.UnitSelectionMenu.Update(gameTime);
                if (InputHelper.InputConfirmPressed() && ListPresentUnit.Count > 0)
                {
                    SelectedUnit = UnitList.SelectedUnit;
                    CursorIndexSubMenu = 0;
                    Stage++;
                }
            }
            else if (Stage == 0)
            {
                if (InputHelper.InputUpPressed())
                {
                    if (CursorIndexSubMenu > 0)
                        CursorIndexSubMenu--;
                }
                else if (InputHelper.InputDownPressed())
                {
                    if (CursorIndexSubMenu < SelectedUnit.MaxCharacter)
                        CursorIndexSubMenu++;
                }
                else if (InputHelper.InputConfirmPressed())
                {
                    Stage++;
                }
            }
            else if (Stage == 1)
            {
                PilotList.UpdateMenu(gameTime);
                if (InputHelper.InputConfirmPressed())
                {
                    int CharacterIndex = PilotList.PilotChoice + (PilotList.CurrentPage - 1) * PilotListScreen.MaxPerPage;
                    PilotListScreen.CharacterInfo SelectedCharacter = PilotList.SelectedCharacter;
                    if (SelectedCharacter.UnitIndex != -1)
                    {
                        Unit OldUnit = ListPresentUnit[SelectedCharacter.UnitIndex];
                        int OldCharacterIndex = Array.IndexOf(OldUnit.ArrayCharacterActive, ListPresentUnit[CharacterIndex - 1]);
                        OldUnit.ArrayCharacterActive[OldCharacterIndex] = null;
                    }
                    //If the selection is not empty, add the pilot.
                    if (CharacterIndex > 0)
                    {
                        SelectedUnit.ArrayCharacterActive[CursorIndexSubMenu] = ListPresentCharacter[CharacterIndex - 1];
                        SelectedCharacter.UnitIndex = UnitList.UnitSelectionMenu.SelectedIndex;
                    }
                    else
                    {
                        SelectedUnit.ArrayCharacterActive[CursorIndexSubMenu] = null;
                        SelectedCharacter.UnitIndex = -1;
                    }
                    Stage = -1;
                }
            }
            if (InputHelper.InputCancelPressed())
            {
                if (Stage > -1)
                    Stage--;
                else
                    RemoveScreen(this);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            g.DrawString(fntFinlanderFont, "Pilot Swap", new Vector2(120, 10), Color.White);

            switch (Stage)
            {
                case -1:
                    UnitList.DrawMenu(g);
                    break;

                case 0:
                    UnitList.DrawMenu(g);

                    //Draw sub menu.
                    g.Draw(sprPixel, new Rectangle(47, 83 + UnitList.UnitSelectionMenu.SelectedItemIndex * 38, 150, SelectedUnit.MaxCharacter * fntArial12.LineSpacing + 5), Color.DarkBlue);
                    //Draw cursor.
                    g.Draw(sprPixel, new Rectangle(52, 86 + UnitList.UnitSelectionMenu.SelectedItemIndex * 38 + CursorIndexSubMenu * fntArial12.LineSpacing, 136, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
                    g.Draw(sprPixel, new Rectangle(52, 103 + UnitList.UnitSelectionMenu.SelectedItemIndex * 38 + CursorIndexSubMenu * fntArial12.LineSpacing, 136, 1), Color.FromNonPremultiplied(127, 107, 0, 255));

                    for (int C = 0; C < SelectedUnit.MaxCharacter; C++)
                    {
                        if (SelectedUnit.ArrayCharacterActive.Length == 0 || SelectedUnit.ArrayCharacterActive[C] == null)
                            g.DrawString(fntArial12, "------", new Vector2(50, 83 + UnitList.UnitSelectionMenu.SelectedItemIndex * 38 + C * fntArial12.LineSpacing), Color.White);
                        else
                            g.DrawString(fntArial12, SelectedUnit.ArrayCharacterActive[C].Name, new Vector2(50, 83 + UnitList.UnitSelectionMenu.SelectedItemIndex * 38 + C * fntArial12.LineSpacing), Color.White);
                    }

                    break;

                case 1:
                    PilotList.Draw(g);
                    break;
            }
        }
    }
}