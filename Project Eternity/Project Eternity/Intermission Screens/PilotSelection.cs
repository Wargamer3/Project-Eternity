using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens;

namespace Project_Eternity
{
    public sealed class PilotSelection : GameScreen
    {
        private class CharacterInfo
        {
            public string UnitName;
            public int UnitIndex;
            public int CharacterUnitIndex;

            public CharacterInfo()
            {
                UnitName = "";
                UnitIndex = -1;
                CharacterUnitIndex = -1;
            }
            public CharacterInfo(string UnitName, int UnitIndex, int CharacterUnitIndex)
            {
                this.UnitName = UnitName;
                this.UnitIndex = UnitIndex;
                this.CharacterUnitIndex = CharacterUnitIndex;
            }
        }

        int CursorIndexSubMenu;
        int CursorIndexUnitSelection;
        int CursorIndexPilotPosition;

        int PageCurrentPilotPosition;
        int PageCurrentUnitSelection;
        const int PageMax = 12;
        const int ItemPerPage = 8;
        const int PilotPerPage = 8;

        Unit SelectedUnit;

        Texture2D sprRectangle;
        Texture2D sprBackground;
        Texture2D sprBackgroundUnitSelection;
        SpriteFont fntArial8;
        SpriteFont fntArial12;
        List<CharacterInfo> ListCharacterInfo;

        int Stage;

        public PilotSelection()
            : base()
        {
            CursorIndexSubMenu = 0;
            CursorIndexUnitSelection = 0;
            PageCurrentPilotPosition = 1;
            PageCurrentUnitSelection = 1;
            Stage = -1;
            ListCharacterInfo = new List<CharacterInfo>();
        }

        public override void Load()
        {
            sprRectangle = Content.Load<Texture2D>("Rectangle");
            sprBackground = Content.Load<Texture2D>("Intermission Screens/Pilot Selection");
            sprBackgroundUnitSelection = Content.Load<Texture2D>("Intermission Screens/Unit Selection");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            PageCurrentUnitSelection = 1;
            PageCurrentPilotPosition = 1;

            ListCharacterInfo.Add(new CharacterInfo("------", -1, -1));
            for (int P = 0; P < Game.ListCharacter.Count; P++)
                if (Game.ListCharacter[P].CanPilot)
                    ListCharacterInfo.Add(new CharacterInfo());
            //Link characters to their Unit
            for (int U = 0; U < Game.ListUnit.Count; U++)
                for (int P = 0; P < Game.ListUnit[U].MaxCharacter; P++)
                    for (int i = 1; i < ListCharacterInfo.Count; i++)
                        if (Game.ListUnit[U].ArrayCharacterActive[P] != null && Game.ListCharacter[i - 1] == Game.ListUnit[U].ArrayCharacterActive[P])
                        {
                            ListCharacterInfo[i].UnitName = Game.ListUnit[U].Name;
                            ListCharacterInfo[i].UnitIndex = U;
                            ListCharacterInfo[i].CharacterUnitIndex = P;
                            break;
                        }
        }
        public override void Update(GameTime gameTime)
        {
            if (KeyboardHelper.InputUpPressed())
            {
                if (Stage == -1)
                {
                    if (CursorIndexUnitSelection > 0)
                        CursorIndexUnitSelection--;
                }
                else if (Stage == 0)
                {
                    if (CursorIndexSubMenu > 0)
                        CursorIndexSubMenu--;
                }
                else if (Stage == 1)
                {
                    if (CursorIndexPilotPosition > 0)
                        CursorIndexPilotPosition--;
                }
            }
            else if (KeyboardHelper.InputDownPressed())
            {
                if (Stage == -1)
                {
                    if (CursorIndexUnitSelection < ItemPerPage - 1 && CursorIndexUnitSelection + 1 + (PageCurrentUnitSelection - 1) * ItemPerPage < Game.ListUnit.Count)
                        CursorIndexUnitSelection++;
                }
                else if (Stage == 0)
                {
                    if (CursorIndexSubMenu < SelectedUnit.MaxCharacter)
                        CursorIndexSubMenu++;
                }
                else if (Stage == 1)
                {
                    if (CursorIndexPilotPosition < SelectedUnit.MaxCharacter)
                        CursorIndexPilotPosition++;
                }
            }
            else if (KeyboardHelper.InputLeftPressed())
            {
                if (Stage == -1)
                {
                    if (PageCurrentUnitSelection > 1)
                        PageCurrentUnitSelection--;
                }
                else if (Stage == 1)
                {
                    if (PageCurrentPilotPosition > 1)
                        PageCurrentPilotPosition--;
                }
            }
            else if (KeyboardHelper.InputRightPressed())
            {
                if (Stage == -1)
                {
                    if (PageCurrentUnitSelection * ItemPerPage < Game.ListUnit.Count)
                        PageCurrentUnitSelection++;
                }
                
                else if (Stage == 1)
                {
                    if ((PageCurrentPilotPosition + 1) * ItemPerPage < ListCharacterInfo.Count)
                        PageCurrentPilotPosition++;
                }
            }
            else if (KeyboardHelper.InputConfirmPressed())
            {
                if (Stage == -1)
                {
                    SelectedUnit = Game.ListUnit[CursorIndexUnitSelection + (PageCurrentUnitSelection - 1) * ItemPerPage];
                    CursorIndexSubMenu = 0;
                    Stage++;
                }
                else if (Stage == 0)
                {
                    CursorIndexPilotPosition = 0;
                    PageCurrentPilotPosition = 1;
                    Stage++;
                }
                else if (Stage == 1)
                {
                    int CharacterIndex = CursorIndexPilotPosition + (PageCurrentPilotPosition - 1) * PilotPerPage;
                    if (ListCharacterInfo[CharacterIndex].UnitIndex != -1)
                    {
                        Game.ListUnit[ListCharacterInfo[CharacterIndex].UnitIndex].ArrayCharacterActive[ListCharacterInfo[CharacterIndex].CharacterUnitIndex] = null;
                    }
                    //If the selection is not empty, add the pilot.
                    if (CharacterIndex != 0)
                    {
                        SelectedUnit.ArrayCharacterActive[CursorIndexSubMenu] = Game.ListCharacter[CharacterIndex - 1];
                        ListCharacterInfo[CharacterIndex].UnitName = SelectedUnit.Name;
                        ListCharacterInfo[CharacterIndex].UnitIndex = CursorIndexUnitSelection + (PageCurrentUnitSelection - 1) * ItemPerPage;
                        ListCharacterInfo[CharacterIndex].CharacterUnitIndex = CursorIndexSubMenu;
                    }
                    else
                    {
                        SelectedUnit.ArrayCharacterActive[CursorIndexSubMenu] = null;
                        ListCharacterInfo[CharacterIndex].UnitName = "------";
                        ListCharacterInfo[CharacterIndex].UnitIndex = -1;
                        ListCharacterInfo[CharacterIndex].CharacterUnitIndex = -1;
                    }
                    Stage = -1;
                }
            }
            else if (KeyboardHelper.InputCancelPressed())
            {
                if (Stage > -1)
                    Stage--;
                else
                    GameScreen.RemoveScreen(this);
            }
        }

        public override void Draw(SpriteBatch g)
        {
            switch (Stage)
            {
                case -1:
                    g.Draw(sprBackgroundUnitSelection, new Vector2(0, 0), Color.White);
                    g.DrawString(fntArial12, PageCurrentUnitSelection.ToString(), new Vector2(604, 380), Color.White);
                    g.DrawString(fntArial12, PageCurrentUnitSelection.ToString(), new Vector2(624, 380), Color.White);
                    //Unit drawing.
                    for (int U = (PageCurrentUnitSelection - 1) * 8, Pos = 0; U < Game.ListUnit.Count && U < PageCurrentUnitSelection * ItemPerPage; U++, Pos++)
                    {
                        g.DrawString(fntArial12, U.ToString(), new Vector2(14, 64 + Pos * 38), Color.White);
                        g.DrawString(fntArial12, Game.ListUnit[U].Name, new Vector2(50, 63 + Pos * 38), Color.White);
                    }
                    //Draw cursor.
                    g.Draw(sprRectangle, new Rectangle(47, 62 + CursorIndexUnitSelection * 38, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
                    g.Draw(sprRectangle, new Rectangle(47, 84 + CursorIndexUnitSelection * 38, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
                    break;

                case 0:
                    g.Draw(sprBackgroundUnitSelection, new Vector2(0, 0), Color.White);
                    g.DrawString(fntArial12, PageCurrentUnitSelection.ToString(), new Vector2(604, 380), Color.White);
                    g.DrawString(fntArial12, PageCurrentUnitSelection.ToString(), new Vector2(624, 380), Color.White);
                    //Unit drawing.
                    for (int U = (PageCurrentUnitSelection - 1) * 8, Pos = 0; U < Game.ListUnit.Count && U < PageCurrentUnitSelection * ItemPerPage; U++, Pos++)
                    {
                        g.DrawString(fntArial12, U.ToString(), new Vector2(14, 64 + Pos * 38), Color.White);
                        g.DrawString(fntArial12, Game.ListUnit[U].Name, new Vector2(50, 63 + Pos * 38), Color.White);
                    }
                    //Draw cursor.
                    g.Draw(sprRectangle, new Rectangle(47, 62 + CursorIndexUnitSelection * 38, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
                    g.Draw(sprRectangle, new Rectangle(47, 84 + CursorIndexUnitSelection * 38, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));

                    //Draw sub menu.
                    g.Draw(sprRectangle, new Rectangle(47, 83 + CursorIndexUnitSelection * 38, 150, SelectedUnit.MaxCharacter * fntArial12.LineSpacing + 5), Color.DarkBlue);
                    //Draw cursor.
                    g.Draw(sprRectangle, new Rectangle(52, 86 + CursorIndexUnitSelection * 38 + CursorIndexSubMenu * fntArial12.LineSpacing, 136, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
                    g.Draw(sprRectangle, new Rectangle(52, 103 + CursorIndexUnitSelection * 38 + CursorIndexSubMenu * fntArial12.LineSpacing, 136, 1), Color.FromNonPremultiplied(127, 107, 0, 255));

                    for (int C = 0; C < SelectedUnit.MaxCharacter; C++)
                    {
                        if (SelectedUnit.ArrayCharacterActive[C] == null)
                            g.DrawString(fntArial12, "------", new Vector2(50, 83 + CursorIndexUnitSelection * 38 + C * fntArial12.LineSpacing), Color.White);
                        else
                            g.DrawString(fntArial12, SelectedUnit.ArrayCharacterActive[C].Name, new Vector2(50, 83 + CursorIndexUnitSelection * 38 + C * fntArial12.LineSpacing), Color.White);
                    }
                    
                    break;

                case 1:
                    g.Draw(sprBackground, new Vector2(0, 0), Color.White);
                    g.DrawString(fntArial12, PageCurrentPilotPosition.ToString(), new Vector2(604, 380), Color.White);
                    g.DrawString(fntArial12, PageCurrentPilotPosition.ToString(), new Vector2(624, 380), Color.White);
                    //Pilot drawing.
                    for (int C = (PageCurrentPilotPosition - 1) * PilotPerPage, Pos = 0; C < ListCharacterInfo.Count && C < PageCurrentPilotPosition * PilotPerPage; C++, Pos++)
                    {
                        g.DrawString(fntArial12, C.ToString(), new Vector2(14, 64 + Pos * 40), Color.White);
                        if (C == 0)
                        {
                            g.DrawString(fntArial12, "------", new Vector2(50, 63 + Pos * 40), Color.White);
                        }
                        else
                        {
                            g.DrawString(fntArial12, Game.ListCharacter[C - 1].Name, new Vector2(50, 63 + Pos * 40), Color.White);
                            g.DrawString(fntArial12, ListCharacterInfo[C].UnitName, new Vector2(250, 63 + Pos * 40), Color.White);
                        }
                    }
                    //Draw cursor.
                    g.Draw(sprRectangle, new Rectangle(47, 62 + CursorIndexPilotPosition * 40, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
                    g.Draw(sprRectangle, new Rectangle(47, 84 + CursorIndexPilotPosition * 40, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
                    break;
            }
        }
    }
}