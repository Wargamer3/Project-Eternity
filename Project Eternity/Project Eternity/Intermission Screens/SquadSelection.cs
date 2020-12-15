using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens;

namespace Project_Eternity
{
    public sealed class SquadSelection : GameScreen
    {
        int Stage;
        int CursorIndex;
        int CursorIndexSub;
        int PageCurrent;
        int PageMax;
        const int ItemPerPage = 8;

        Texture2D sprRectangle;
        Texture2D sprBackground;
        Texture2D sprCursor;
        SpriteFont fntArial8;
        SpriteFont fntArial12;
        SpriteFont fntArial14;
		int UnitSelected;

        public SquadSelection()
            : base()
        {
            Stage = -1;
            CursorIndex = 0;
            CursorIndexSub = 0;
            PageCurrent = 1;
            UnitSelected = -1;
        }

        public override void Load()
        {
            sprRectangle = Content.Load<Texture2D>("Rectangle");
            sprBackground = Content.Load<Texture2D>("Intermission Screens/Squad Selection");
            sprCursor = Content.Load<Texture2D>("Intermission Screens/Squad Selection Cursor");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntArial14 = Content.Load<SpriteFont>("Fonts/Arial");

            PageMax = (int)Math.Ceiling(Game.ListUnit.Count / (float)ItemPerPage);
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
                    if (CursorIndex < ItemPerPage && CursorIndex + 1 + (PageCurrent - 1) * ItemPerPage < Game.ListUnit.Count)
                        CursorIndex++;
                }
                else if (KeyboardHelper.InputLeftPressed())
                {
                    if (PageCurrent > 1)
                        PageCurrent--;
                }
                else if (KeyboardHelper.InputRightPressed())
                {
                    if (PageCurrent * 8 < Game.ListUnit.Count)
                        PageCurrent++;
                }
                else if (KeyboardHelper.InputConfirmPressed())
                {
                    Stage++;
                }
            }
            else if (Stage == 0)
            {
                if (KeyboardHelper.InputUpPressed())
                {
                    if (CursorIndexSub > 0)
                        CursorIndexSub--;
                }
                else if (KeyboardHelper.InputDownPressed())
                {
                    if (CursorIndexSub < 6)
                        CursorIndexSub++;
                }
                else if (KeyboardHelper.InputConfirmPressed())
                {
                    switch (CursorIndexSub)
                    {
                        case 0://Leader stats.

                            break;

                        case 1://Wing 1 stats.

                            break;

                        case 2://Wing 2 stats.

                            break;

                        case 3://Rename squad.

                            break;

                        case 4://Leader stats.

                            break;

                        case 5://Wing 1 set.

                            break;

                        case 6://Wing B set.

                            break;

                        case 7://Disband squad.

                            break;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch g)
        {
            g.Draw(sprBackground, new Vector2(0, 0), Color.White);
            g.DrawString(fntArial12, PageCurrent.ToString(), new Vector2(604, 0), Color.White);
            g.DrawString(fntArial12, PageMax.ToString(), new Vector2(624, 0), Color.White);
            //Unit drawing.
            for (int U = (PageCurrent - 1) * ItemPerPage, Pos = 0; U < Game.ListUnit.Count && U < PageCurrent * ItemPerPage; U++, Pos++)
			{
                g.DrawString(fntArial12, "Hello"/*Game.ListUnit[U].SquadName*/, new Vector2(14, 46 + Pos * 46), Color.White);
                g.DrawString(fntArial12, Game.ListUnit[U].Name, new Vector2(100, 46 + Pos * 46), Color.White);
                g.DrawString(fntArial8, "HP " + Game.ListUnit[U].MaxHP, new Vector2(100, 63 + Pos * 46), Color.White);
                g.DrawString(fntArial8, "EN " + Game.ListUnit[U].MaxEN, new Vector2(100, 75 + Pos * 46), Color.White);
                if (Game.ListUnit[U].CurrentWingmanA != null)
                {
                    g.DrawString(fntArial12, Game.ListUnit[U].CurrentWingmanA.Name, new Vector2(150, 140 + Pos * 46), Color.White);
                }
                if (Game.ListUnit[U].CurrentWingmanB != null)
                {
                    g.DrawString(fntArial12, Game.ListUnit[U].CurrentWingmanB.Name, new Vector2(150, 160 + Pos * 46), Color.White);
                }
                if (Pos == CursorIndex)
                {
                    g.DrawString(fntArial12, Game.ListUnit[U].Name, new Vector2(440, 26), Color.White);
                    if (Game.ListUnit[U].ArrayCharacterActive[0] != null)
                        g.DrawString(fntArial12, Game.ListUnit[U].PilotName, new Vector2(440, 26), Color.White);
                    g.DrawString(fntArial8, "HP " + Game.ListUnit[U].MaxHP, new Vector2(440, 63), Color.White);
                    g.DrawString(fntArial8, "EN " + Game.ListUnit[U].MaxEN, new Vector2(500, 63), Color.White);
                    g.DrawString(fntArial8, "Armor " + Game.ListUnit[U].Armor, new Vector2(440, 75), Color.White);
                    g.DrawString(fntArial8, "Mobility " + Game.ListUnit[U].Mobility, new Vector2(440, 87), Color.White);
                    g.DrawString(fntArial8, "MV " + Game.ListUnit[U].Movement, new Vector2(440, 99), Color.White);
                }
			}
            g.Draw(sprCursor, new Vector2(2, 46 + CursorIndex * 46), Color.White);

            //Squad selection right menu.
            g.DrawString(fntArial12, "Leader Stats",    new Vector2(438, 352), Color.White);
            g.DrawString(fntArial12, "Wing 1 Stats",    new Vector2(438, 370), Color.White);
            g.DrawString(fntArial12, "Wing 2 Stats",    new Vector2(438, 388), Color.White);
            g.DrawString(fntArial12, "Rename Squad",    new Vector2(438, 406), Color.White);
            g.DrawString(fntArial12, "Wing 1 Set",      new Vector2(438, 424), Color.White);
            g.DrawString(fntArial12, "Wing 2 Set",      new Vector2(438, 442), Color.White);
            g.DrawString(fntArial12, "Disband Squad",   new Vector2(438, 460), Color.White);

            if (Stage == 0)//Right side menu
            {
                g.Draw(sprRectangle, new Rectangle(436, 352 + CursorIndexSub * 18, 200, 18), Color.FromNonPremultiplied(255, 255, 255, 127));
            }
        }
    }
}
