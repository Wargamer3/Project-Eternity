using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace Project_Eternity
{
    public sealed class UnitSelection : GameScreen
    {
        int NbUnitsToSelect;
        int Stage;
        int CursorIndex;
        int PageCurrent;
        int PageMax;

        Texture2D sprRectangle;
        Texture2D sprBackground;
        Texture2D sprCursor;
        Texture2D sprConfirmation;
        Texture2D sprWarning;
        SpriteFont fntArial8;
        SpriteFont fntArial12;
        SpriteFont fntArial14;
		List<int> ListUnitSelected;

		List<Unit> ListUnitSelection;

        public UnitSelection(int NbUnitsToSelect)
            : base()
        {
            this.NbUnitsToSelect = NbUnitsToSelect;
            Stage = -1;
            CursorIndex = 0;
            PageCurrent = 1;
            ListUnitSelected = new List<int>(NbUnitsToSelect);
            ListUnitSelection = new List<Unit>(Game.ListUnit.Count);
        }

        public override void Load()
        {
            sprRectangle = Content.Load<Texture2D>("Rectangle");
            sprBackground = Content.Load<Texture2D>("Intermission Screens/Unit Selection");
            sprCursor = Content.Load<Texture2D>("Intermission Screens/Unit Selection Cursor");
            sprConfirmation = Content.Load<Texture2D>("Intermission Screens/Unit Selection Confirmation");
            sprWarning = Content.Load<Texture2D>("Intermission Screens/Unit Selection Warning");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntArial14 = Content.Load<SpriteFont>("Fonts/Arial");
			
			for (int U = 0; U < Game.ListUnit.Count; U++)
				ListUnitSelection.Add(Game.ListUnit[U]);
            //Link Squad members to their Leader.
            for (int i = 0; i < ListUnitSelection.Count; i++)
                for (int U = 0; U < Game.ListUnit.Count; U++)
					for (int S = 0; S < Game.ListUnit[U].ListSquad.Count; S++)
						if (Game.ListUnit[U].ListSquad[S] == Game.ListUnit[i])
						{
                            ListUnitSelection.RemoveAt(i);
							break;
						}

            PageMax = (int)Math.Ceiling(ListUnitSelection.Count / 8.0);
        }
        public override void Update(GameTime gameTime)
        {
            if (Stage == -1)
            {
                if (KeyboardHelper.InputUpPressed())
                {
                    if (CursorIndex > 0)
                        CursorIndex--;
                }
                else if (KeyboardHelper.InputDownPressed())
                {
                    if (CursorIndex < 8 && CursorIndex + 1 + (PageCurrent - 1) * 8 < ListUnitSelection.Count)
                        CursorIndex++;
                }
                else if (KeyboardHelper.InputLeftPressed())
                {
                    if (PageCurrent > 1)
                        PageCurrent--;
                }
                else if (KeyboardHelper.InputRightPressed())
                {
                    if (PageCurrent * 8 < ListUnitSelection.Count)
                        PageCurrent++;
                }
                else if (KeyboardHelper.InputConfirmPressed())
                {
                    //If the Unit has a pilot.
                    if (ListUnitSelection[CursorIndex + (PageCurrent - 1) * 8].ArrayCharacterActive[0] != null)
                    {
                        if (ListUnitSelected.Contains(CursorIndex + (PageCurrent - 1) * 8))
                            ListUnitSelected.Remove(CursorIndex + (PageCurrent - 1) * 8);
                        else
                            ListUnitSelected.Add(CursorIndex + (PageCurrent - 1) * 8);
                        if (ListUnitSelected.Count >= NbUnitsToSelect)
                        {
                            List<Player> ListPlayers = new List<Player>();
                            List<Unit> ListPlayerUnit = new List<Unit>();
                            for (int U = 0; U < ListUnitSelected.Count; U++)
                            {
                                ListPlayerUnit.Add(ListUnitSelection[ListUnitSelected[U]]);
                            }
                            ListPlayers.Add(new Player("noname", PlayerTypes.Human, ListPlayerUnit, 0));

                            GameScreen.PushScreen(new DeathmatchMap(ListPlayers));
                            GameScreen.RemoveScreen(this);
                        }
                    }
                    else//Open the pop up menu.
                        Stage++;
                }
                else if (KeyboardHelper.InputCancelPressed())
                {
                    GameScreen.RemoveAllScreens();
                    GameScreen.PushScreen(new IntermissionScreen());
                }
            }
            else
            {
                if (KeyboardHelper.InputCancelPressed() || KeyboardHelper.InputConfirmPressed())
                    Stage--;
            }
        }

        public override void Draw(SpriteBatch g)
        {
            g.Draw(sprBackground, new Vector2(0, 0), Color.White);
            g.DrawString(fntArial14, ListUnitSelected.Count.ToString(), new Vector2(511, 21), Color.Yellow);
            g.DrawString(fntArial14, NbUnitsToSelect.ToString(), new Vector2(541, 21), Color.Yellow);
            g.DrawString(fntArial12, PageCurrent.ToString(), new Vector2(604, 380), Color.White);
            g.DrawString(fntArial12, PageMax.ToString(), new Vector2(624, 380), Color.White);
            //Unit drawing.
            for (int U = (PageCurrent - 1) * 8, Pos = 0; U < ListUnitSelection.Count && U < PageCurrent * 8; U++, Pos++)
			{
                g.DrawString(fntArial12, U.ToString(), new Vector2(14, 64 + Pos * 38), Color.White);
                g.DrawString(fntArial12, ListUnitSelection[U].Name, new Vector2(50, 63 + Pos * 38), Color.White);
                if (ListUnitSelection[U].CurrentWingmanA != null)
                    g.DrawString(fntArial8, ListUnitSelection[U].CurrentWingmanA.Name, new Vector2(150, 140 + Pos * 38), Color.White);
                if (ListUnitSelection[U].CurrentWingmanB != null)
                    g.DrawString(fntArial8, ListUnitSelection[U].CurrentWingmanB.Name, new Vector2(150, 160 + Pos * 38), Color.White);
				if (ListUnitSelected.Contains(U))
				{
                    g.Draw(sprCursor, new Vector2(34, 52 + Pos * 38), Color.White);
                    g.Draw(sprRectangle, new Rectangle(47, 62 + Pos * 38, 316, 1), Color.LawnGreen);
                    g.Draw(sprRectangle, new Rectangle(47, 84 + Pos * 38, 316, 1), Color.LawnGreen);
				}
				if (U == CursorIndex + (PageCurrent - 1) * 8)
				{
                    g.Draw(sprRectangle, new Rectangle(47, 62 + Pos * 38, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
                    g.Draw(sprRectangle, new Rectangle(47, 84 + Pos * 38, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
				}
			}
            if (Stage == 0)
            {
                g.Draw(sprWarning, new Vector2(100, 120), Color.White);
            }
            if (Stage == -2)
                g.Draw(sprConfirmation, new Vector2(100, 120), Color.White);
        }
    }
}
