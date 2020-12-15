using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private enum InputStatus { FirstPress = 1, SecondPress = 70, ThirdPress = 120, FourthPress = 170, LastPress = 220 };

        private CustomSpriteBatch spriteBatch;
        private List<GameScreen> ListGameScreen;

        public Game()
        {
            Constants.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            ListGameScreen = new List<GameScreen>();

            Constants.graphics.PreferredBackBufferWidth = Constants.Width;
            Constants.graphics.PreferredBackBufferHeight = Constants.Height;
            Constants.graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            LoadGameContent();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
            GameScreen.GraphicsDevice = GraphicsDevice;
            GameScreen.serviceProvider = Services;
            GameScreen.FMODSystem = new SoundSystem();
            if (SoundSystem.AudioFound)
                SoundEffect.MasterVolume = 1;
            //Create a new Title screen.
            GameScreen FirstScreen = new FanprestoScreen();
            FirstScreen.Load();
            FirstScreen.ListGameScreen = ListGameScreen;
            ListGameScreen.Add(FirstScreen);

            IsMouseVisible = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            if (GameScreen.FMODSystem != null)
                GameScreen.FMODSystem.System.release();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Constants.TotalGameTime = gameTime.TotalGameTime.TotalSeconds;
            KeyboardHelper.UpdateKeyboardStatus();
            MouseHelper.MouseStateCurrent = Mouse.GetState();
            CheatCodeHelper.Update(gameTime);

            if (GameScreen.FMODSystem != null)
                GameScreen.FMODSystem.System.update();

            for (int S = ListGameScreen.Count - 1; S >= 0; --S)
            {
                if (!ListGameScreen[S].Alive)//Delete, then decrement i to go back at the last instance in the list.
                {
                    if (ListGameScreen[S].Content != Content)
                    {
                        ListGameScreen[S].Content.Unload();
                    }

                    ListGameScreen[S].Unload();
                    ListGameScreen.RemoveAt(S);
                }
            }

            for (int S = ListGameScreen.Count - 1; S >= 0; --S)
            {
                if (S == 0)
                    ListGameScreen[S].IsOnTop = true;
                else
                    ListGameScreen[S].IsOnTop = false;

                //If the GameScreen requires to be on top and is on top or doesn't requires focus to be updated.
                if ((ListGameScreen[S].RequireFocus && ListGameScreen[S].IsOnTop) || !ListGameScreen[S].RequireFocus)
                {
                    if (!ListGameScreen[S].Alive)//Delete, then decrement i to go back at the last instance in the list.
                    {
                    }
                    else
                    {
                        //Update everything in the GameScreen List and delete it if not Alive.
                        ListGameScreen[S].Update(gameTime);
                    }
                }
            }
            if (ListGameScreen.Count == 0)
                this.Exit();

            KeyboardHelper.PlayerStateLast = Keyboard.GetState();
            MouseHelper.MouseStateLast = MouseHelper.MouseStateCurrent;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            for (int S = ListGameScreen.Count - 1; S >= 0; --S)
                    ListGameScreen[S].BeginDraw(spriteBatch);

            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

            for (int S = ListGameScreen.Count - 1; S >= 0; --S)
            {
                if (S == 0)
                    ListGameScreen[S].IsOnTop = true;
                else
                    ListGameScreen[S].IsOnTop = false;

                if ((ListGameScreen[S].RequireDrawFocus && ListGameScreen[S].IsOnTop) || !ListGameScreen[S].RequireDrawFocus)
                        ListGameScreen[S].Draw(spriteBatch);
            }

            for (int S = ListGameScreen.Count - 1; S >= 0; --S)
                    ListGameScreen[S].EndDraw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void LoadGameContent()
        {
            SplashScreen.ShowSplashScreen();
            SplashScreen.SetStatus("Initializing Roslyn");
            SkillEffect.LoadAllEffects();
            SplashScreen.SetStatus("Initializing Graphics");
            Constants.Money = 100000;
            BattleMap.DicGlobalVariables = new Dictionary<string, string>();
            BattleMap.DicRouteChoices = new Dictionary<string, int>();
            GameScreen.LoadHelpers(Content);

            #region Key mapping

            FileStream FS = new FileStream("Keys.ini", FileMode.Open, FileAccess.Read);
            StreamReader SR = new StreamReader(FS);
            string StreamLine;
            string[] StreamData;
            string[] StreamKeys;
            List<Keys> ListNewKeys = new List<Keys>();
            Keys NewKey;
            Keys[] UsedKeys;

            while (!SR.EndOfStream)
            {
                StreamLine = SR.ReadLine();
                ListNewKeys.Clear();

                if (StreamLine.Contains('='))
                {
                    StreamData = StreamLine.Split('=');
                    StreamKeys = StreamData[1].Split(',');

                    //Read keys
                    for (int K = 0; K < StreamKeys.Length; K++)
                    {
                        NewKey = ConvertTextToKeys(StreamKeys[K].Trim());
                        if (!ListNewKeys.Contains(NewKey))
                            ListNewKeys.Add(NewKey);
                    }
                    UsedKeys = ListNewKeys.ToArray();

                    //Assign keys to the right command.
                    switch (StreamData[0].Trim())
                    {
                        case "Left":
                            KeyboardHelper.MoveLeft = UsedKeys;
                            break;

                        case "Right":
                            KeyboardHelper.MoveRight = UsedKeys;
                            break;

                        case "Up":
                            KeyboardHelper.MoveUp = UsedKeys;
                            break;

                        case "Down":
                            KeyboardHelper.MoveDown = UsedKeys;
                            break;

                        case "Confirm":
                            KeyboardHelper.ConfirmChoice = UsedKeys;
                            break;

                        case "Cancel":
                            KeyboardHelper.CancelChoice = UsedKeys;
                            break;

                        case "Command 1":
                            KeyboardHelper.Command1 = UsedKeys;
                            break;

                        case "Command 2":
                            KeyboardHelper.Command2 = UsedKeys;
                            break;

                        case "L Button":
                            KeyboardHelper.LButton = UsedKeys;
                            break;

                        case "R Button":
                            KeyboardHelper.RButton = UsedKeys;
                            break;

                        case "Skip":
                            KeyboardHelper.Skip = UsedKeys;
                            break;
                    }
                }
            }

            FS.Close();
            SR.Close();

            #endregion

            SplashScreen.SetStatus("Loading Ressources");

            Core.Skill.ManualSkillTarget.LoadAllSkillRequirement();
            Core.Item.AutomaticSkillTargetType.LoadAllTargetTypes();
            SystemList.LoadSystemLists();

            #region Ressources loading

            string[] Files;
            bool InstanceIsBaseObject;
            Type ObjectType;

            #region Battle Maps

            Files = Directory.GetFiles("Mods", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ass = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                Type[] types = null;
                //Get every classes in it.
                types = ass.GetTypes();
                for (int t = 0; t < types.Count(); t++)
                {
                    //Look if the class inherit from Unit somewhere.
                    ObjectType = types[t].BaseType;
                    InstanceIsBaseObject = ObjectType == typeof(BattleMap);
                    while (ObjectType != null && ObjectType != typeof(BattleMap))
                    {
                        ObjectType = ObjectType.BaseType;
                        if (ObjectType == null)
                            InstanceIsBaseObject = false;
                    }
                    //If this class is from BaseEditor, load it.
                    if (InstanceIsBaseObject)
                    {
                        BattleMap instance = Activator.CreateInstance(types[t]) as BattleMap;
                        BattleMap.DicBattmeMapType.Add(instance.GetMapType(), instance);
                    }
                }
            }

            #endregion

            #endregion

            SplashScreen.CloseForm();
        }

        public Keys ConvertTextToKeys(string Text)
        {
            switch (Text.ToUpper())
            {
                case "A":
                    return Keys.A;

                case "B":
                    return Keys.B;

                case "C":
                    return Keys.C;

                case "D":
                    return Keys.D;

                case "E":
                    return Keys.E;

                case "F":
                    return Keys.F;

                case "G":
                    return Keys.G;

                case "H":
                    return Keys.H;

                case "I":
                    return Keys.I;

                case "J":
                    return Keys.J;

                case "K":
                    return Keys.K;

                case "L":
                    return Keys.L;

                case "M":
                    return Keys.M;

                case "N":
                    return Keys.N;

                case "O":
                    return Keys.O;

                case "P":
                    return Keys.P;

                case "Q":
                    return Keys.Q;

                case "R":
                    return Keys.R;

                case "S":
                    return Keys.S;

                case "T":
                    return Keys.T;

                case "U":
                    return Keys.U;

                case "V":
                    return Keys.V;

                case "W":
                    return Keys.W;

                case "X":
                    return Keys.X;

                case "Y":
                    return Keys.Y;

                case "Z":
                    return Keys.Z;

                case "LEFT":
                    return Keys.Left;

                case "RIGHT":
                    return Keys.Right;

                case "UP":
                    return Keys.Up;

                case "DOWN":
                    return Keys.Down;

                case "ESCAPE":
                    return Keys.Escape;

                case "ENTER":
                    return Keys.Enter;
            }

            return Keys.None;
        }
    }
}
