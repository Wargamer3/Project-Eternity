using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens;

namespace Project_Eternity
{
    class RouteMenu : GameScreen
    {
        int CursorSelection;

        Texture2D Background;
        Texture2D TopBar;
        Texture2D Choice;
        Texture2D Description;
        Texture2D Highlight;
        List<int> ListChoice;

        public RouteMenu()
            : base()
        {
            CursorSelection = 0;
            ListChoice = new List<int>();
            ListChoice.Add(0);
            ListChoice.Add(1);
        }

        public override void Load()
        {
            Background = Content.Load<Texture2D>("Intermission Screens/Route Menu/Background");
            TopBar = Content.Load<Texture2D>("Intermission Screens/Route Menu/TopBar");
            Choice = Content.Load<Texture2D>("Intermission Screens/Route Menu/Choice");
            Description = Content.Load<Texture2D>("Intermission Screens/Route Menu/Description");
            Highlight = Content.Load<Texture2D>("Intermission Screens/Route Menu/Highlight");
        }

        public override void Update(GameTime gameTime)
        {
            if (KeyboardHelper.InputUpPressed())
            {
                CursorSelection -= CursorSelection > 0 ? 1 : 0;
            }
            else if (KeyboardHelper.InputDownPressed())
            {
                CursorSelection += CursorSelection < ListChoice.Count - 1 ? 1 : 0;
            }
            else if (KeyboardHelper.InputConfirmPressed())
            {

            }
            else if (KeyboardHelper.InputCancelPressed())
            {
                GameScreen.RemoveScreen(this);
            }
        }

        public override void Draw(SpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap,
                    DepthStencilState.Default, RasterizerState.CullNone);

            g.Draw(Background, Vector2.Zero, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.White);
            g.End();
            g.Begin();
            g.Draw(TopBar, new Vector2(0, 25), Color.White);
            g.Draw(Description, new Vector2(410, 62), Color.White);

            for (int C = 0; C < ListChoice.Count; C++)
                g.Draw(Choice, new Vector2(30, 62 + C * 99), Color.White);

            g.Draw(Highlight, new Vector2(30, 62 + CursorSelection * 99), Color.White);
        }
    }
}