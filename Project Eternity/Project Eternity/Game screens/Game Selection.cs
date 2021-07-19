﻿using System.IO;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.RacingScreen;
using ProjectEternity.GameScreens.WorldMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using System.Collections.Generic;

namespace ProjectEternity
{
    public sealed class GameSelection : GameScreen
    {
        private enum MenuChoices { Normal, SuperTreeWar, Intermission, Multiplayer, WorldMap, Conquest, SorcererStreet, Racing, SuperTank, TripleThunder };

        private int SelectedChoice = 0;

        private FMODSound sndConfirm;
        private FMODSound sndSelection;
        private FMODSound sndDeny;

        public GameSelection()
            : base()
        {
            RequireDrawFocus = true;
            RequireFocus = true;
        }

        public override void Load()
        {
            sndConfirm = new FMODSound(FMODSystem, "Content/SFX/Confirm.mp3");
            sndDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");
            sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                SelectedChoice--;
                sndSelection.Play();

                if (SelectedChoice == -1)
                    SelectedChoice = 9;
            }
            else if (InputHelper.InputDownPressed())
            {
                SelectedChoice++;
                sndSelection.Play();

                if (SelectedChoice == 10)
                    SelectedChoice = 0;
            }
            else if (InputHelper.InputConfirmPressed())
            {
                switch ((MenuChoices)SelectedChoice)
                {
                    case MenuChoices.Normal:
                        int OldNumberOfGameScreen = ListGameScreen.Count;
                        StreamReader BR = new StreamReader("Content/Map path.ini");
                        DeathmatchMap NewMap = new DeathmatchMap(BR.ReadLine(), 0, new List<Core.Units.Squad>());
                        BR.Close();
                        NewMap.ListGameScreen = ListGameScreen;
                        NewMap.PlayerRoster = new Roster();
                        NewMap.Load();

                        //Remove any GameScreen created by the map so they don't show up immediately.
                        List<GameScreen>  ListGameScreenCreatedByMap = new List<GameScreen>(ListGameScreen.Count - OldNumberOfGameScreen);
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

                    case MenuChoices.SuperTreeWar:
                        PushScreen(new DeathmatchMap("Super Tree Wars/Holy Temple", 0, new List<Core.Units.Squad>()));
                        break;

                    case MenuChoices.Intermission:
                        BattleMap.NextMapType = "Deathmatch";
                        BattleMap.NextMapPath = "New Item";

                        PushScreen(new IntermissionScreen());
                        break;

                    case MenuChoices.Multiplayer:
                        PushScreen(new MultiplayerScreen());
                        break;

                    case MenuChoices.WorldMap:
                        PushScreen(new WorldMap("Test Map", 0, new List<Core.Units.Squad>()));
                        break;

                    case MenuChoices.Conquest:
                        PushScreen(new GameScreens.ConquestMapScreen.ConquestMap("Conquest Test", 0, null));
                        break;

                    case MenuChoices.SorcererStreet:
                        PushScreen(new GameScreens.SorcererStreetScreen.SorcererStreetMap("New Item", 0));
                        break;

                    case MenuChoices.Racing:
                        PushScreen(new RacingMap());
                        break;

                    case MenuChoices.SuperTank:
                        Constants.Width = 1024;
                        Constants.Height = 768;
                        Constants.ScreenSize = 0;
                        Constants.graphics.PreferredBackBufferWidth = Constants.Width;
                        Constants.graphics.PreferredBackBufferHeight = Constants.Height;
                        Constants.graphics.ApplyChanges();

                        PushScreen(new GameScreens.SuperTankScreen.SuperTank2());
                        break;

                    case MenuChoices.TripleThunder:
                        Constants.Width = 800;
                        Constants.Height = 600;
                        Constants.ScreenSize = 0;
                        Constants.graphics.PreferredBackBufferWidth = Constants.Width;
                        Constants.graphics.PreferredBackBufferHeight = Constants.Height;
                        Constants.graphics.ApplyChanges();
                        PushScreen(new GameScreens.TripleThunderScreen.Loby());
                        break;
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Deferred, null);

            int LineHeight = 20;
            DrawBox(g, new Vector2(40, 40), Constants.Width - 80, Constants.Height - 80, Color.White);
            TextHelper.DrawText(g, "Normal", new Vector2(50, 50), Color.White);
            TextHelper.DrawText(g, "Super Tree Wars", new Vector2(50, 50 + LineHeight * 1), Color.White);
            TextHelper.DrawText(g, "Intermission", new Vector2(50, 50 + LineHeight * 2), Color.White);
            TextHelper.DrawText(g, "Multiplayer", new Vector2(50, 50 + LineHeight * 3), Color.White);
            TextHelper.DrawText(g, "World Map", new Vector2(50, 50 + LineHeight * 4), Color.White);
            TextHelper.DrawText(g, "Conquest", new Vector2(50, 50 + LineHeight * 5), Color.White);
            TextHelper.DrawText(g, "Sorcerer Street", new Vector2(50, 50 + LineHeight * 6), Color.White);
            TextHelper.DrawText(g, "Racing", new Vector2(50, 50 + LineHeight * 7), Color.White);
            TextHelper.DrawText(g, "Super Tank", new Vector2(50, 50 + LineHeight * 8), Color.White);
            TextHelper.DrawText(g, "Triple Thunder", new Vector2(50, 50 + LineHeight * 9), Color.White);

            g.Draw(sprPixel, new Rectangle(50, 50 + SelectedChoice * LineHeight, Constants.Width - 100, LineHeight), Color.FromNonPremultiplied(255, 255, 255, 127));
        }
    }
}
