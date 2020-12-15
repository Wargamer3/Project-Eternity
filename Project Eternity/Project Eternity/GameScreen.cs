using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Eternity
{
    /// <summary>
    /// Represent a basic class for every screen that will need the be drawn.
    /// </summary>
    public abstract class GameScreen
    {
        /// <summary>
        /// Decide if the current GameScreen is active or need to be removed.
        /// </summary>
        public bool Alive = true;
        /// <summary>
        /// Tell if the current GameScreen is on the of the screen.
        /// </summary>
        public bool IsOnTop = true;
        /// <summary>
        /// Force the screen to idle if the current GameScreen IsOnTop is false.
        /// </summary>
        public bool RequireFocus = true;
        /// <summary>
        /// Force the screen to hide if the current GameScreen IsOnTop is false.
        /// </summary>
        public bool RequireDrawFocus = false;
        /// <summary>
        /// Decide at which transparancy the current GameScreen need to be drawn(such as popup screen).
        /// </summary>
        public int Transparancy = 255;//Work in progress
        public GameScreen()
        {
        }
        public abstract void Load(ContentManager Content);
        /// <summary>
        /// Override the Update to make your own game logic for the screen.
        /// </summary>
        public abstract void Update(GameTime gameTime);
        /// <summary>
        /// Override the Update to make your own drawing logic for the screen.
        /// </summary>
        public abstract void Draw(SpriteBatch g);
    }

    /// <summary>
    /// Represent a temporary GameScreen that will unload every other GameScreen then load a new one.
    /// </summary>
    public class LoadScreen : GameScreen
    {
        private GameScreen[] Screens;
        private Texture2D BackgroundBuffer;
        /// <summary>
        /// Initialize a new LoadScreen that will clear the other screens and load the new one automatically.
        /// </summary>
        /// <param name="Game">Requires the main form of the project to have access to its members.</param>
        /// <param name="Screens">An array of GameScreen to create while loading.</param>
        /// <param name="BackgroundBuffer">A background picture to use for the loading screen. Can be null.</param>
        public LoadScreen(GameScreen[] Screens, Texture2D BackgroundBuffer)
        {
            this.Screens = Screens;
            this.BackgroundBuffer = BackgroundBuffer;
            //Remove everything else.
            for (int i = 0; i < Game.listGameScreen.Count; i++)
                Game.RemoveScreen(i);
        }

        public override void Load(ContentManager Content)
        { }
        public override void Update(GameTime gameTime)
        {//If all the GameScreen are unloaded and only this GameScreen is loaded.
            if (Game.listGameScreen.Count == 1)
            {//Ask the Game to remove this GameScreen the next time it update.
                Game.RemoveScreen(this);
                //Create and load the pending GameScreen array.
                for (int i = 0; i < Screens.Length; i++)
                    Game.AddScreen(Screens[i]);
            }
        }
        public override void Draw(SpriteBatch g)
        {
            if (BackgroundBuffer!=null)
                g.Draw(BackgroundBuffer, new Vector2(0, 0), Color.White);
            g.DrawString(Game.fntArial, "Loading", new Vector2(Game.Width - 100, Game.Height - 80), Color.Black);
        }
    }
}
