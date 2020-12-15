using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    class PauseMenu : GameScreen
    {
        private Rectangle ResumeButtonLocation;
        private Rectangle ExitButtonLocation;

        public override void Load()
        {
            int X = Constants.Width / 2 - Constants.Width / 8;
            int Y = Constants.Height / 4;
            int Width = Constants.Width / 4;

            X += 5;
            Y += 5;
            ResumeButtonLocation = new Rectangle(X, Y, Width - 10, 40);
            ExitButtonLocation = new Rectangle(X, Y, Width - 10, 40);
        }

        public override void Update(GameTime gameTime)
        {
            if (MouseHelper.InputLeftButtonPressed())
            {
                if (ResumeButtonLocation.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    RemoveScreen(this);
                }
                else if (ExitButtonLocation.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    //RoomInformations NewRoom = new RoomInformations("No ID needed", "", "", "", false, 3, 1);

                    //PushScreen(new MissionSelect(null, NewRoom));
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X = Constants.Width / 2 - Constants.Width / 8;
            float Y = Constants.Height / 4;
            int Width = Constants.Width / 4;

            DrawBox(g, new Vector2(X, Y), Width, Constants.Height / 2, Color.White);

            Y += 5;
            DrawBox(g, new Vector2(X + 5, Y), Width - 10, 40, Color.White);
            DrawTextMiddleAligned(g, "Resume", new Vector2(Constants.Width / 2, Y + 10), Color.White);

            Y += 45;
            DrawBox(g, new Vector2(X + 5, Y), Width - 10, 40, Color.White);
            DrawTextMiddleAligned(g, "Exit", new Vector2(Constants.Width / 2, Y + 10), Color.White);
        }
    }
}
