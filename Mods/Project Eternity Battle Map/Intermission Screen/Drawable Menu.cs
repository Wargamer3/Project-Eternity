using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class DrawableMenu
    {
        public delegate void DrawMenuItemDelegate(CustomSpriteBatch g, int ItemIndex, int X, int Y);

        public DrawMenuItemDelegate DrawMenuItem;
        private int NumberOfItems;
        public int SelectedItemIndex;
        public int CurrentPage;
        public readonly int PageCount;
        private int MaxPerPage;
        private FMODSound sndSelection;

        public int SelectedIndex { get { return SelectedItemIndex + (CurrentPage - 1) * MaxPerPage; } }

        public DrawableMenu(DrawMenuItemDelegate DrawMenuItem, int NumberOfItems, int MaxPerPage)
        {
            CurrentPage = 1;
            SelectedItemIndex = 0;
            sndSelection = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Selection.mp3");

            this.DrawMenuItem = DrawMenuItem;
            this.NumberOfItems = NumberOfItems;
            this.MaxPerPage = MaxPerPage;
            PageCount = (int)Math.Ceiling(NumberOfItems / (double)MaxPerPage);
        }

        public void Update(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                SelectedItemIndex -= (SelectedItemIndex > 0) ? 1 : 0;
                sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed())
            {
                ++SelectedItemIndex;

                if (SelectedItemIndex >= MaxPerPage)
                {
                    SelectedItemIndex = MaxPerPage - 1;
                }
                else if ((CurrentPage - 1) * MaxPerPage + SelectedItemIndex >= NumberOfItems)
                {
                    SelectedItemIndex = Math.Max(0, (NumberOfItems - 1) % MaxPerPage);
                }

                sndSelection.Play();
            }
            else if (InputHelper.InputLeftPressed())
            {
                CurrentPage -= (CurrentPage > 1) ? 1 : 0;
                sndSelection.Play();
            }
            else if (InputHelper.InputRightPressed())
            {
                CurrentPage += (CurrentPage < PageCount) ? 1 : 0;
                sndSelection.Play();
            }
        }

        public int GetItemPage(int Index)
        {
            return 1 + Index / MaxPerPage;
        }

        public void DrawMenu(CustomSpriteBatch g, int X, int Y, int LineSpacing)
        {
            int UnitIndex = (CurrentPage - 1) * MaxPerPage;

            for (int U = 0; U + UnitIndex < NumberOfItems && U < MaxPerPage; U++)
            {
                DrawMenuItem(g, U, X, Y);
                Y += LineSpacing;
            }
        }
    }
}
