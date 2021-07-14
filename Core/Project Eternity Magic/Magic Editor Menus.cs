using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Magic
{
    class MagicElementSelectionPanel : ActionPanel
    {
        MagicEditor Editor;

        List<MagicElement> ListAllMagicElementChoice = new List<MagicElement>();
        private int MenuMaxVisibleHeight;
        private int MenuOffset;
        private int ElementSize;
        int MaxOffset;
        int MaxNumberVisibleElement;
        private int SpellBoxX = Constants.Width - 200;

        public MagicElementSelectionPanel(MagicEditor Editor)
            : base("Magic Elements", Editor.ListActionMenuChoice, true)
        {
            this.Editor = Editor;
            ListAllMagicElementChoice = new List<MagicElement>();
            ListAllMagicElementChoice.AddRange(MagicElement.LoadAllMagicElements().Values);
            foreach (MagicElement ActiveMagicElement in ListAllMagicElementChoice)
            {
                ActiveMagicElement.InitGraphics(Editor.Content);
            }

            MenuOffset = 0;
            ElementSize = 75;
            MenuMaxVisibleHeight = Constants.Height - 50;
            MaxNumberVisibleElement = ((MenuMaxVisibleHeight - 40) / ElementSize) * 2;
            MaxOffset = (int)Math.Ceiling((ListAllMagicElementChoice.Count - MaxNumberVisibleElement) / 2d);
            SpellBoxX = Constants.Width - 200;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Editor.CursorPosition.X >= SpellBoxX && Editor.CursorPosition.Y > 30)
            {
                if (MouseHelper.InputLeftButtonPressed())
                {
                    int X = (int)Math.Floor((Editor.CursorPosition.X - SpellBoxX) / ElementSize);
                    int Y = (int)Math.Floor((Editor.CursorPosition.Y - 30 + MenuOffset * 2) / ElementSize);
                    int SelectedIndex = X + (Y + MenuOffset) * 2;

                    if (SelectedIndex < ListAllMagicElementChoice.Count)
                    {
                        Editor.ListMagicElement.Add(ListAllMagicElementChoice[SelectedIndex]);
                        RemoveFromPanelList(this);
                    }
                }
                else if (MouseHelper.MouseStateCurrent.ScrollWheelValue != MouseHelper.MouseStateLast.ScrollWheelValue)
                {
                    MenuOffset += (MouseHelper.MouseStateLast.ScrollWheelValue - MouseHelper.MouseStateCurrent.ScrollWheelValue) / 120;
                    MenuOffset = Math.Max(0, Math.Min(MaxOffset, MenuOffset));
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            GameScreen.DrawBox(g, new Vector2(SpellBoxX, 30), 200, MenuMaxVisibleHeight, Color.White);

            for (int i = MenuOffset * 2, j = 0; i < ListAllMagicElementChoice.Count && j < MaxNumberVisibleElement; ++i, ++j)
            {
                int VisibleElementIndex = (i / 2) - MenuOffset;
                int X = SpellBoxX + 10 + (i % 2) * ElementSize;
                int Y = 40 + VisibleElementIndex * ElementSize;

                MagicElement ActiveElement = ListAllMagicElementChoice[i];
                g.Draw(Editor.sprMagicCircle, new Rectangle(X, Y, ElementSize, ElementSize), Color.White);
                TextHelper.DrawTextMiddleAligned(g, ActiveElement.ToString(), new Vector2(X + 38, Y + ElementSize), Color.White);
            }
        }

        public override void OnSelect()
        {
        }

        protected override void OnCancelPanel()
        {
        }
    }

    class MagicAttributesEditonPanel : ActionPanel
    {
        private readonly int SpellBoxX = Constants.Width - 200;
        private readonly int StartY = 30;
        private readonly int AttributeNameHeight = 10;

        private MagicElement ActiveMagicElement;

        public MagicAttributesEditonPanel(MagicEditor Editor, MagicElement ActiveMagicElement)
            : base("Magic Elements", Editor.ListActionMenuChoice, true)
        {
            this.ActiveMagicElement = ActiveMagicElement;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (MouseHelper.MouseStateCurrent.X >= SpellBoxX && MouseHelper.MouseStateCurrent.Y > StartY
                && (MouseHelper.MouseMoved() || MouseHelper.InputLeftButtonPressed() || MouseHelper.InputLeftButtonReleased()))
            {
                int Y = StartY;

                for (int A = 0; A < ActiveMagicElement.ArrayAttributes.Length; A++)
                {
                    int MouseX = MouseHelper.MouseStateCurrent.X - SpellBoxX;
                    int MouseY = MouseHelper.MouseStateCurrent.Y;

                    Y += AttributeNameHeight;

                    if (MouseY > Y && MouseY <= Y + ActiveMagicElement.ArrayAttributes[A].Height)
                    {
                        ActiveMagicElement.ArrayAttributes[A].Update(gameTime, MouseX, MouseY - StartY);
                    }

                    Y += ActiveMagicElement.ArrayAttributes[A].Height;
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int Y = StartY;

            GameScreen.DrawBox(g, new Vector2(SpellBoxX, Y), 200, Constants.Height, Color.White);

            for (int A = 0; A < ActiveMagicElement.ArrayAttributes.Length; A++)
            {
                TextHelper.DrawText(g, ActiveMagicElement.ArrayAttributes[A].Name, new Vector2(SpellBoxX + 18, Y), Color.White);

                Y += AttributeNameHeight;

                ActiveMagicElement.ArrayAttributes[A].Draw(g, new Vector2(SpellBoxX, Y));

                Y += ActiveMagicElement.ArrayAttributes[A].Height;
            }
        }

        protected override void OnCancelPanel()
        {
        }
    }
}
