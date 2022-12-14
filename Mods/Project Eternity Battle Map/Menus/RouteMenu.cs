using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class RouteMenu : GameScreen
    {
        public struct RouteParams
        {
            public string Title;
            public string Summary;
            public string Description;

            public RouteParams(string Title, string Summary, string Description)
            {
                this.Title = Title;
                this.Summary = Summary;
                this.Description = Description;
            }

            public override string ToString()
            {
                return Title;
            }
        }

        Texture2D Background;
        Texture2D TopBar;
        Texture2D Choice;
        Texture2D Description;
        Texture2D Highlight;
        SpriteFont fntTitle;

        public int CursorSelection;
        public string RouteName;
        List<RouteParams> ListChoice;
        public bool SelectionFinalized;

        public RouteMenu(string RouteName, List<RouteParams> ListChoice)
            : base()
        {
            this.RouteName = RouteName;
            this.ListChoice = ListChoice;
            SelectionFinalized = false;
            CursorSelection = 0;

            ListChoice = new List<RouteParams>();
            ListChoice.Add(new RouteParams("Go to the space Amazon planet", "Coop, Jamie and Kira will head for the space\n\rAmazon planet in search of something.\n\rDifficulty: Easy", ""));
            ListChoice.Add(new RouteParams("Stay to defend the base", "Master Asia asked Domon to stay with him to\n\rdefend the last bastion of humanity.\n\rDifficulty: Hard", ""));
        }

        public override void Load()
        {
            Background = Content.Load<Texture2D>("Intermission Screens/Route Menu/Background");
            TopBar = Content.Load<Texture2D>("Intermission Screens/Route Menu/TopBar");
            Choice = Content.Load<Texture2D>("Intermission Screens/Route Menu/Choice");
            Description = Content.Load<Texture2D>("Intermission Screens/Route Menu/Description");
            Highlight = Content.Load<Texture2D>("Intermission Screens/Route Menu/Highlight");

            fntTitle = Content.Load<SpriteFont>("Fonts/Arial11Bold");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                CursorSelection -= CursorSelection > 0 ? 1 : 0;
            }
            else if (InputHelper.InputDownPressed())
            {
                CursorSelection += CursorSelection < ListChoice.Count - 1 ? 1 : 0;
            }
            else if (InputHelper.InputConfirmPressed())
            {
                SelectionFinalized = true;
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
        }

        public override void Draw(CustomSpriteBatch g)
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
            {
                g.Draw(Choice, new Vector2(30, 62 + C * 99), Color.White);
                g.DrawString(fntTitle, ListChoice[C].Title, new Vector2(35, 65 + C * 99), Color.White);
                g.DrawString(fntTitle, ListChoice[C].Summary, new Vector2(35, 90 + C * 99), Color.White);
            }

            g.Draw(Highlight, new Vector2(30, 85 + CursorSelection * 99), Color.White);
        }
    }
}