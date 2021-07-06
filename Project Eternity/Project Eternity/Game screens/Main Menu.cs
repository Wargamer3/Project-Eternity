using System.Collections.Generic;
using System.IO;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity
{
    /// <summary>
    /// This is the first menu to appear, where you you can choose to start a new game,
    /// load a game or continue the last one.
    /// </summary>
    public sealed class MainMenu : GameScreen
    {
        private enum MenuChoices { NewGame = 0, LoadGame, QuickLoad, Option, Encyclopedia };

        private int SelectedChoice = 0;
        private double EllapsedTime;

        private Texture2D sprNewGame;
        private Texture2D sprQuickLoad;
        private Texture2D sprEncyclopedia;
        private Texture2D sprOption;
        private Texture2D sprLoadGame;

        private FMODSound sndIntroSong;
        private FMODSound sndConfirm;
        private FMODSound sndSelection;
        private FMODSound sndDeny;

        /// <summary>
        /// Create a new Main Menu screen
        /// </summary>
        /// <param name="Game">Requires the main form of the project to have access to its members.</param>
        public MainMenu()
            : base()
        {
            RequireDrawFocus = true;
            RequireFocus = true;
            EllapsedTime = 0;
        }

        public override void Load()
        {
            sndIntroSong = new FMODSound(FMODSystem, "Content/[Title] The Eternal History.mp3");
            sndIntroSong.SetLoop(true);
            sndIntroSong.PlayAsBGM();
            sndConfirm = new FMODSound(FMODSystem, "Content/SFX/Confirm.mp3");
            sndDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");
            sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
            sprNewGame = Content.Load<Texture2D>("Main Menu New/Title screenSTART");
            sprQuickLoad = Content.Load<Texture2D>("Main Menu New/Title screenCONTINUE");
            sprEncyclopedia = Content.Load<Texture2D>("Main Menu New/Title screenLIBRARY");
            sprOption = Content.Load<Texture2D>("Main Menu New/Title screenOPTIONS");
            sprLoadGame = Content.Load<Texture2D>("Main Menu New/Title screenLOAD");
        }

        public override void Update(GameTime gameTime)
        {
            EllapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (EllapsedTime >= 10)
            {
                RemoveScreen(this);
                MultiplayerScreen Autoplay = new MultiplayerScreen();
                Autoplay.Load();
                Constants.ShowAnimation = false;
                PushScreen(Autoplay.LoadAutoplay());
            }

            if (InputHelper.InputUpPressed())
            {
                EllapsedTime = 0;

                SelectedChoice--;
                sndSelection.Play();

                if (SelectedChoice == -1)
                    SelectedChoice = 4;
            }
            else if (InputHelper.InputDownPressed())
            {
                EllapsedTime = 0;

                SelectedChoice++;
                sndSelection.Play();

                if (SelectedChoice == 5)
                    SelectedChoice = 0;
            }
            else if (InputHelper.InputConfirmPressed())
            {
                switch ((MenuChoices)SelectedChoice)
                {
                    case MenuChoices.NewGame:
                        sndIntroSong.Stop();
                        sndConfirm.Play();

                        /*StreamReader BR = new StreamReader("Content/Map path.ini");
                        PushScreen(new DeathmatchMap(BR.ReadLine(), 0, new System.Collections.Generic.List<Core.Units.Squad>()));
                        BR.Close();*/
                        PushScreen(new GameSelection());
                        break;

                    case MenuChoices.QuickLoad:
                        if (File.Exists("TempSave.sav"))
                        {
                            sndIntroSong.Stop();
                            sndConfirm.Play();
                            BattleMap QuickLoadMap = BattleMap.LoadTemporaryMap(ListGameScreen);
                            ListGameScreen.Insert(0, QuickLoadMap);
                        }
                        else
                        {
                            sndDeny.Play();
                        }
                        break;

                    case MenuChoices.Encyclopedia:
                        sndDeny.Play();
                        break;

                    case MenuChoices.Option:
                        PushScreen(new OptionMenu());
                        sndConfirm.Play();
                        break;

                    case MenuChoices.LoadGame:
                        if (File.Exists("SRWE Save.bin"))
                        {
                            sndIntroSong.Stop();
                            sndConfirm.Play();

                            Roster PlayerRoster = new Roster();
                            PlayerRoster.LoadRoster();
                            Dictionary<string, Unit> DicUnitType = Unit.LoadAllUnits();
                            Dictionary<string, BaseSkillRequirement> DicRequirement = BaseSkillRequirement.LoadAllRequirements();
                            Dictionary<string, BaseEffect> DicEffect = BaseEffect.LoadAllEffects();
                            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();
                            Dictionary<string, ManualSkillTarget> DicManualSkillTarget = ManualSkillTarget.LoadAllTargetTypes();
                            DataScreen.LoadProgression(PlayerRoster, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                            PushScreen(new NewIntermissionScreen(PlayerRoster));
                        }
                        else
                        {
                            sndDeny.Play();
                        }
                        break;
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            switch ((MenuChoices)SelectedChoice)
            {
                case MenuChoices.NewGame:
                    g.Draw(sprNewGame, Vector2.Zero, Color.White);
                    break;

                case MenuChoices.QuickLoad:
                    g.Draw(sprQuickLoad, Vector2.Zero, Color.White);
                    break;

                case MenuChoices.Encyclopedia:
                    g.Draw(sprEncyclopedia, Vector2.Zero, Color.White);
                    break;

                case MenuChoices.Option:
                    g.Draw(sprOption, Vector2.Zero, Color.White);
                    break;

                case MenuChoices.LoadGame:
                    g.Draw(sprLoadGame, Vector2.Zero, Color.White);
                    break;
            }
        }
    }
}
