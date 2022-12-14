using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Magic
{
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

        public override void DoRead(ByteReader BR)
        {
            throw new System.NotImplementedException();
        }

        public override void DoWrite(ByteWriter BW)
        {
            throw new System.NotImplementedException();
        }

        protected override ActionPanel Copy()
        {
            throw new System.NotImplementedException();
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
