﻿using System;
using System.IO;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
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

                        int OldNumberOfGameScreen = ListGameScreen.Count;
                        StreamReader BR = new StreamReader("Content/Map path.ini");
                        BattleMap NewMap = BattleMap.DicBattmeMapType[DeathmatchMap.MapType].GetNewMap(null, string.Empty);
                        NewMap.BattleMapPath = BR.ReadLine();
                        BR.Close();
                        NewMap.ListGameScreen = ListGameScreen;
                        NewMap.PlayerRoster = new Roster();
                        NewMap.PlayerRoster.LoadRoster();
                        NewMap.Load();
                        NewMap.Init();
                        NewMap.TogglePreview(true);

                        //Remove any GameScreen created by the map so they don't show up immediately.
                        List<GameScreen> ListGameScreenCreatedByMap = new List<GameScreen>(ListGameScreen.Count - OldNumberOfGameScreen);
                        for (int S = ListGameScreen.Count - 1 - OldNumberOfGameScreen; S >= 0; --S)
                        {
                            ListGameScreenCreatedByMap.Add(ListGameScreen[S]);
                            ListGameScreen.RemoveAt(S);
                        }

                        RemoveAllScreens();
                        ListGameScreen.Insert(0, NewMap);
                        NewMap.Update(gameTime);

                        for (int S = 0; S < ListGameScreenCreatedByMap.Count; ++S)
                        {
                            ListGameScreen.Insert(0, ListGameScreenCreatedByMap[S]);
                            ListGameScreenCreatedByMap[S].Update(gameTime);
                        }

                        ListGameScreenCreatedByMap.Clear();
                        break;

                    case MenuChoices.QuickLoad:
                        if (File.Exists("User Data/Saves/TempSave.sav"))
                        {
                            sndIntroSong.Stop();
                            sndConfirm.Play();
                            BattleMap QuickLoadMap = BattleMap.LoadTemporaryMap(ListGameScreen);
                            QuickLoadMap.TogglePreview(true);
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
                        if (File.Exists("User Data/Saves/SRWE Save.bin"))
                        {
                            sndIntroSong.Stop();
                            sndConfirm.Play();
                            BattleContext.LoadDefaultValues();

                            BattleMapPlayer Player = new BattleMapPlayer();
                            Roster PlayerRoster = new Roster();
                            PlayerRoster.LoadRoster();
                            DataScreen.LoadProgression(Player, PlayerRoster, Unit.DicDefaultUnitType, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);
                            PushScreen(new NewIntermissionScreen(Player, PlayerRoster));
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
