using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Item
{
    public class Scrollbar : IUIElement
    {
        public delegate void OnScrollbarChangeDelegate(float ScrollbarValue);

        private Texture2D sprScrollbar;

        private OnScrollbarChangeDelegate OnScrollbarChange;

        private Vector2 Position;
        private int Height;
        private int Width;
        private int SectionSize;
        private int ScrollbarBarHeight;
        private int ScrollbarBarUsableHeight;

        private float ScrollbarBarPositionY;
        private float ScrollbarValueMax;
        private float ScrollbarIncrementValue;
        private int MovingScrollbarSelectionPoint;

        public Scrollbar(Texture2D sprScrollbar, Vector2 Position, int Height, float ScrollbarValueMax, OnScrollbarChangeDelegate OnScrollbarChange)
        {
            this.sprScrollbar = sprScrollbar;
            this.Position = Position;
            this.Height = Height;
            this.ScrollbarValueMax = Math.Max(0, ScrollbarValueMax);
            this.OnScrollbarChange = OnScrollbarChange;

            Width = sprScrollbar.Width / 4;
            SectionSize = sprScrollbar.Height / 4;
            ScrollbarBarHeight = SectionSize;
            ScrollbarBarPositionY = Position.Y + SectionSize;

            ScrollbarBarUsableHeight = Height - ScrollbarBarHeight - SectionSize - SectionSize;
        }

        public void Update(GameTime gameTime)
        {
            if (MovingScrollbarSelectionPoint >= 0)
            {
                if (MouseHelper.MouseMoved() && MouseHelper.InputLeftButtonHold())
                {
                    float NewScrollbarValue = GetScrollbarValue(MouseHelper.MouseStateCurrent.Y - MovingScrollbarSelectionPoint);
                    if (NewScrollbarValue >= 0)
                    {
                        OnScrollbarChange(NewScrollbarValue);
                        ScrollbarBarPositionY = Math.Min(Position.Y + SectionSize + ScrollbarBarUsableHeight, MouseHelper.MouseStateCurrent.Y - MovingScrollbarSelectionPoint);
                    }
                }
            }
            else if (MouseHelper.MouseStateCurrent.X >= Position.X && MouseHelper.MouseStateCurrent.Y >= Position.Y
                && MouseHelper.MouseStateCurrent.X <= Position.X + Width && MouseHelper.MouseStateCurrent.Y <= Position.Y + Height)
            {
                {
                    if (MouseHelper.InputLeftButtonPressed() && IsInsideScrollbarBar(new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y)))
                    {
                        MovingScrollbarSelectionPoint = (MouseHelper.MouseStateCurrent.Y - (int)Position.Y - SectionSize) / ScrollbarBarUsableHeight;
                    }
                }
            }
            else if (MouseHelper.InputLeftButtonPressed())
            {
                MovingScrollbarSelectionPoint = -1;
            }
        }

        public void ChangeMaxValue(float NewMaxValue)
        {
            ScrollbarValueMax = NewMaxValue;
        }

        private bool IsInsideTopArrow(Vector2 PositionToCheck)
        {
            return PositionToCheck.X >= Position.X && PositionToCheck.Y >= Position.Y
                && PositionToCheck.X <= Position.X + Width && PositionToCheck.Y <= Position.Y + SectionSize;
        }

        private bool IsInsideBottomArrow(Vector2 PositionToCheck)
        {
            return PositionToCheck.X >= Position.X && PositionToCheck.Y >= Position.Y + Height - SectionSize
                && PositionToCheck.X <= Position.X + Width && PositionToCheck.Y <= Position.Y + Height;
        }

        private bool IsInsideScrollbarBar(Vector2 PositionToCheck)
        {
            if (PositionToCheck.X >= Position.X && PositionToCheck.Y >= Position.Y + SectionSize
                && PositionToCheck.X <= Position.X + Width && PositionToCheck.Y <= Height - SectionSize)
            {
                if (PositionToCheck.Y > ScrollbarBarPositionY && PositionToCheck.Y <= ScrollbarBarPositionY + ScrollbarBarHeight)
                {
                    return true;
                }
            }

            return false;
        }

        private float GetScrollbarValue(float PositionToCheck)
        {
            if (PositionToCheck >= Position.Y && PositionToCheck <= Position.Y + Height)
            {
                float PositionValue = (PositionToCheck - Position.Y - SectionSize) / ScrollbarBarUsableHeight;
                return Math.Min(PositionValue * ScrollbarValueMax, ScrollbarValueMax);
            }

            return -1;
        }

        public void Select()
        {
        }

        public void Unselect()
        {
        }

        public void Enable()
        {
        }

        public void Disable()
        {
        }

        public void Draw(CustomSpriteBatch g)
        {
            DrawTopArrow(g);
            DrawBottomArrow(g);
            DrawScrollbarBackground(g);
            DrawScrollbarBar(g);
        }

        private void DrawTopArrow(CustomSpriteBatch g)
        {
            g.Draw(sprScrollbar, new Rectangle((int)Position.X, (int)Position.Y, Width, SectionSize), new Rectangle(0, 0, Width, SectionSize), Color.White);
        }

        private void DrawBottomArrow(CustomSpriteBatch g)
        {
            g.Draw(sprScrollbar, new Rectangle((int)Position.X, (int)Position.Y + Height - SectionSize, Width, SectionSize), new Rectangle(0, sprScrollbar.Height - SectionSize, Width, SectionSize), Color.White);
        }

        private void DrawScrollbarBar(CustomSpriteBatch g)
        {
            g.Draw(sprScrollbar, new Rectangle((int)Position.X, (int)ScrollbarBarPositionY, Width, SectionSize), new Rectangle(0, SectionSize * 2, Width, ScrollbarBarHeight), Color.White);
        }

        private void DrawScrollbarBackground(CustomSpriteBatch g)
        {
            g.Draw(sprScrollbar, new Rectangle((int)Position.X, (int)Position.Y + SectionSize, Width, Height - SectionSize - SectionSize), new Rectangle(0, SectionSize, Width, SectionSize), Color.White);
        }
    }
}
