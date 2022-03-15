﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class CommanderLoadoutScreen : GameScreen
    {
        private Texture2D sprRectangle;
        private Texture2D sprBackground;
        private Texture2D sprCursor;
        private SpriteFont fntArial8;
        private SpriteFont fntArial12;
        private SpriteFont fntArial14;

        private const int MaxCommander = 3;

        private readonly Roster PlayerRoster;

        private int CursorIndex;
        private static List<Commander> ListSelectedCommander;

        public CommanderLoadoutScreen(Roster PlayerRoster)
            : base()
        {
            this.PlayerRoster = PlayerRoster;

            ListSelectedCommander = new List<Commander>();

            CursorIndex = 0;
            PlayerRoster.ListCommander.Clear();
            PlayerRoster.ListCommander.Add(new Commander("Saotome Institute"));
        }

        public override void Load()
        {
            sprRectangle = Content.Load<Texture2D>("Pixel");
            sprBackground = Content.Load<Texture2D>("Menus/Intermission Screens/Unit Selection");
            sprCursor = Content.Load<Texture2D>("Menus/Intermission Screens/Unit Selection Cursor");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntArial14 = Content.Load<SpriteFont>("Fonts/Arial");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                if (CursorIndex > 0)
                    CursorIndex--;
            }
            else if (InputHelper.InputDownPressed())
            {
                if (CursorIndex < 8 && CursorIndex + 1 < PlayerRoster.ListCommander.Count)
                    CursorIndex++;
            }
            else if (InputHelper.InputConfirmPressed())
            {
                if (ListSelectedCommander.Count == MaxCommander || ListSelectedCommander.Count == PlayerRoster.ListCommander.Count || PlayerRoster.ListCommander.Count == 0)
                {
                    LoadoutScreen.LoadMap(ListGameScreen, PlayerRoster);
                    BattleMapPlayer Player1 = new BattleMapPlayer("", "Player 1", BattleMapPlayer.PlayerTypes.Host, false, 0, true, Color.Blue);
                    Player1.Inventory.ActiveLoadout.ListSpawnCommander.AddRange(ListSelectedCommander);
                    LoadoutScreen.StartMap(this, gameTime, Player1);
                }
                else
                {
                    ListSelectedCommander.Add(PlayerRoster.ListCommander[CursorIndex]);
                }
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            g.GraphicsDevice.Clear(Color.Black);
            DrawBox(g, new Vector2(5, 0), 450, 25, Color.Green);
            g.DrawStringMiddleAligned(fntArial12, "Choose the commanders you would like to deploy", new Vector2(230, 4), Color.White);

            DrawBox(g, new Vector2(Constants.Width - 170, 0), 170, 50, Color.Green);
            g.DrawString(fntArial8, "Select your commanders", new Vector2(Constants.Width - 160, 5), Color.White);
            g.DrawString(fntArial14, ListSelectedCommander.Count.ToString() + " / " + MaxCommander, new Vector2(511, 21), Color.Yellow);
            g.DrawStringMiddleAligned(fntArial12, "Left", new Vector2(570, 23), Color.White);

            for (int S = 0, Pos = 0; S < PlayerRoster.ListCommander.Count; S++, Pos++)
			{
                g.DrawString(fntArial12, PlayerRoster.ListCommander[S].Name, new Vector2(50, 63 + Pos * 38), Color.White);
				if (S == CursorIndex)
				{
                    g.Draw(sprRectangle, new Rectangle(47, 62 + Pos * 38, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
                    g.Draw(sprRectangle, new Rectangle(47, 84 + Pos * 38, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
				}

                if (ListSelectedCommander.Contains(PlayerRoster.ListCommander[S]))
                {
                    g.Draw(sprCursor, new Vector2(40, 52 + Pos * 38), Color.White);
                }
            }


            DrawBox(g, new Vector2(Constants.Width - 455, Constants.Height - 25), 450, 25, Color.Green);
            g.DrawString(fntArial8, "Arrow Keys: Select", new Vector2(Constants.Width - 445, Constants.Height - 20), Color.White);
            g.DrawString(fntArial8, "Enter: Confirm", new Vector2(Constants.Width - 255, Constants.Height - 20), Color.White);
            g.DrawString(fntArial8, "Escape: Cancel", new Vector2(Constants.Width - 95, Constants.Height - 20), Color.White);
        }
    }
}
