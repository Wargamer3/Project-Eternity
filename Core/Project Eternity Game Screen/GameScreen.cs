using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens
{
    /// <summary>
    /// Represent a basic class for every screen that will need the be drawn.
    /// </summary>
    public abstract partial class GameScreen
    {
        public ContentManager Content;
        public static IServiceProvider serviceProvider;
        public static ContentManager ContentFallback; //If there is no service provider because of running inside a GUI, use this content manager instead of creating a new one.
        public List<GameScreen> ListGameScreen;// The GameScreen list is shared through every GameScreen of the game but each GameScreen inside a GUI will have its separated list.
        public static SoundSystem FMODSystem;
        public static GraphicsDevice GraphicsDevice;
        public static string GameLanguage = "eng";
        public static bool UseDebugMode = true;

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
        /// Instanciate a new GameScreen object.
        /// </summary>
        /// <param name="serviceProvider">A IServiceProvider used to create a new ContentManager.</param>
        public GameScreen()
        {
            if (serviceProvider != null)
            {
                Content = new ContentManager(serviceProvider);
                Content.RootDirectory = "Content";
            }
            else
            {
                Content = ContentFallback;
            }
        }

        public void PushScreen(GameScreen Screen)
        {
            Screen.ListGameScreen = ListGameScreen;
            Screen.Load();
            ListGameScreen.Insert(0, Screen);
        }

        protected void RemoveWithoutUnloading(GameScreen Screen)
        {
            ListGameScreen.Remove(Screen);
        }

        public void RemoveScreen(int Pos)
        {
            ListGameScreen[Pos].Alive = false;
        }

        public void RemoveScreen(GameScreen Screen)
        {
            Screen.Alive = false;
        }

        public void RemoveAllScreens(GameScreen ExcludedScreen = null)
        {
            for (int S = 0; S < ListGameScreen.Count; S++)
                if (ListGameScreen[S] != ExcludedScreen)
                    ListGameScreen[S].Alive = false;
        }

        public abstract void Load();

        public virtual void Unload() { }

        /// <summary>
        /// Override the Update to make your own game logic for the screen.
        /// </summary>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Override the Draw to make your own drawing logic for the screen.
        /// </summary>
        public abstract void Draw(CustomSpriteBatch g);

        /// <summary>
        /// Called before the Draw method, used to avoid messing with the main RenderTarget. Will be called even if it doesn't have focus.
        /// </summary>
        public virtual void BeginDraw(CustomSpriteBatch g) { }

        /// <summary>
        /// Called after the Draw method, used to draw on top of everything. Will be called even if it doesn't have focus.
        /// </summary>
        public virtual void EndDraw(CustomSpriteBatch g) { }
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
            : base()
        {
            this.Screens = Screens;
            this.BackgroundBuffer = BackgroundBuffer;
            //Remove everything else.
            for (int i = 0; i < ListGameScreen.Count; i++)
                RemoveScreen(i);
        }

        public override void Load()
        { }

        public override void Update(GameTime gameTime)
        {//If all the GameScreen are unloaded and only this GameScreen is loaded.
            if (ListGameScreen.Count == 1)
            {//Ask the Game to remove this GameScreen the next time it update.
                RemoveScreen(this);
                //Create and load the pending GameScreen array.
                for (int i = 0; i < Screens.Length; i++)
                    PushScreen(Screens[i]);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (BackgroundBuffer != null)
                g.Draw(BackgroundBuffer, new Vector2(0, 0), Color.White);
            //g.DrawString(Game.fntArial, "Loading", new Vector2(Game.Width - 100, Game.Height - 80), Color.Black);
        }
    }
}
