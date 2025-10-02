using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class LoadoutScreen : GameScreen
    {
        private enum Stages { SquadSelection, SquadInfo, Warning, Confirmation }

        Texture2D sprRectangle;
        Texture2D sprBackground;
        Texture2D sprCursor;
        Texture2D sprConfirmation;
        Texture2D sprWarning;
        SpriteFont fntArial8;
        SpriteFont fntArial12;
        SpriteFont fntArial14;

        private readonly BattleMapPlayer Player;
        private readonly Roster PlayerRoster;
        protected StatusMenuScreen StatusMenu;

        private Stages Stage;
        private int CursorIndex;
        private int PageCurrent;
        private int PageMax;
        private const int ItemPerPage = 8;
        private static BattleMap NewMap;
        private static List<Squad> ListSelectedSquad;
        private static List<GameScreen> ListGameScreenCreatedByMap;

        private List<Squad> ListPresentSquad;
        private List<Unit> ListPresentUnit;
        private List<Commander> ListSelectedCommander;

        public LoadoutScreen(BattleMapPlayer Player, Roster PlayerRoster, List<Commander> ListSelectedCommander)
            : base()
        {
            this.Player = Player;
            this.PlayerRoster = PlayerRoster;
            this.ListSelectedCommander = ListSelectedCommander;
            Stage = Stages.SquadSelection;
            CursorIndex = 0;
            PageCurrent = 1;
        }

        public override void Load()
        {
            sprRectangle = Content.Load<Texture2D>("Pixel");
            sprBackground = Content.Load<Texture2D>("Deathmatch/Intermission Screens/Unit Selection");
            sprCursor = Content.Load<Texture2D>("Deathmatch/Intermission Screens/Unit Selection Cursor");
            sprConfirmation = Content.Load<Texture2D>("Deathmatch/Intermission Screens/Unit Selection Confirmation");
            sprWarning = Content.Load<Texture2D>("Deathmatch/Intermission Screens/Unit Selection Warning");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntArial14 = Content.Load<SpriteFont>("Fonts/Arial");

            StatusMenu = new StatusMenuScreen(null);
            StatusMenu.Load();

            ListPresentSquad = PlayerRoster.TeamSquads.GetPresent();
            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();

            PageMax = (int)Math.Ceiling(ListPresentSquad.Count / (float)ItemPerPage);

            //Keep the map in cache in case the player goes back and forth between menus.
            if (NewMap == null)
            {
                LoadMap(ListGameScreen, Player, PlayerRoster);
            }
        }

        public static void LoadMap(List<GameScreen> ListGameScreen, BattleMapPlayer Player, Roster PlayerRoster)
        {
            int OldNumberOfGameScreen = ListGameScreen.Count;
            ListSelectedSquad = new List<Squad>();
            NewMap = BattleMap.DicBattmeMapType[BattleMap.NextMapType].GetNewMap(null, string.Empty);
            NewMap.BattleMapPath = BattleMap.NextMapPath;
            NewMap.ListGameScreen = ListGameScreen;
            NewMap.PlayerRoster = PlayerRoster;
            NewMap.AddLocalPlayer(Player);
            NewMap.Load();

            //Remove any GameScreen created by the map so they don't show up immediately.
            ListGameScreenCreatedByMap = new List<GameScreen>(ListGameScreen.Count - OldNumberOfGameScreen);
            for (int S = ListGameScreen.Count - 1 - OldNumberOfGameScreen; S >= 0; --S)
            {
                ListGameScreenCreatedByMap.Add(ListGameScreen[S]);
                ListGameScreen.RemoveAt(S);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Stage == Stages.SquadSelection)
            {
                if (InputHelper.InputUpPressed())
                {
                    if (CursorIndex > 0)
                        CursorIndex--;
                }
                else if (InputHelper.InputDownPressed())
                {
                    if (CursorIndex < 8 && CursorIndex + 1 + (PageCurrent - 1) * 8 < ListPresentSquad.Count)
                        CursorIndex++;
                }
                else if (InputHelper.InputLeftPressed())
                {
                    if (PageCurrent > 1)
                        PageCurrent--;
                }
                else if (InputHelper.InputRightPressed())
                {
                    if (PageCurrent * 8 < ListPresentSquad.Count)
                        PageCurrent++;
                }
                else if (InputHelper.InputConfirmPressed())
                {
                    if (ListSelectedSquad.Count == NewMap.PlayersMax || ListPresentSquad.Count == 0)
                    {
                        BattleMapPlayer Player1 = new BattleMapPlayer("", "Player 1", OnlinePlayerBase.PlayerTypes.Player, false, 0, true, Color.Blue);
                        Player1.Inventory.ActiveLoadout.ListSpawnSquad.AddRange(ListSelectedSquad);
                        Player1.Inventory.ActiveLoadout.ListSpawnCommander.AddRange(ListSelectedCommander);
                        StartMap(this, gameTime, Player1);
                    }
                    else
                    {
                        ListSelectedSquad.Add(ListPresentSquad[CursorIndex + (PageCurrent - 1) * 8]);
                    }
                }
                else if (InputHelper.InputCancelPressed())
                {
                    RemoveScreen(this);
                }
                else if (InputHelper.InputCommand1Pressed())
                {
                    Stage = Stages.SquadInfo;
                    StatusMenu.ActiveSquad = new Squad("", ListPresentUnit[0]);
                    StatusMenu.StatusPannel = StatusMenuScreen.StatusPannels.Unit;
                }
            }
            else
            {
                if (InputHelper.InputCancelPressed() || InputHelper.InputConfirmPressed())
                {
                    Stage = Stages.SquadSelection;
                }
            }
        }

        public static void StartMap(GameScreen Owner, GameTime gameTime, BattleMapPlayer Player1)
        {
            int OldNumberOfGameScreen = Owner.ListGameScreen.Count;
            NewMap.AddLocalPlayer(Player1);
            NewMap.Init();
            NewMap.TogglePreview(true);

            ListGameScreenCreatedByMap = new List<GameScreen>(Owner.ListGameScreen.Count - OldNumberOfGameScreen);
            for (int S = Owner.ListGameScreen.Count - 1 - OldNumberOfGameScreen; S >= 0; --S)
            {
                ListGameScreenCreatedByMap.Add(Owner.ListGameScreen[S]);
                Owner.ListGameScreen.RemoveAt(S);
            }

            Owner.RemoveAllScreens();

            Owner.ListGameScreen.Insert(0, NewMap);
            NewMap.Update(gameTime);

            for (int S = 0; S < ListGameScreenCreatedByMap.Count; ++S)
            {
                Owner.ListGameScreen.Insert(0, ListGameScreenCreatedByMap[S]);
                ListGameScreenCreatedByMap[S].Update(gameTime);
            }

            ListGameScreenCreatedByMap.Clear();
            NewMap = null;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprBackground, new Vector2(0, 0), Color.White);
            g.DrawString(fntArial14, ListSelectedSquad.Count.ToString(), new Vector2(511, 21), Color.Yellow);
            g.DrawString(fntArial14, NewMap.PlayersMax.ToString(), new Vector2(550, 21), Color.Yellow);
            g.DrawString(fntArial12, PageCurrent.ToString(), new Vector2(604, 380), Color.White);
            g.DrawString(fntArial12, PageMax.ToString(), new Vector2(624, 380), Color.White);
            //Unit drawing.
            for (int S = (PageCurrent - 1) * 8, Pos = 0; S < ListPresentSquad.Count && S < PageCurrent * 8; S++, Pos++)
			{
                g.DrawString(fntArial12, S.ToString(), new Vector2(14, 64 + Pos * 38), Color.White);
                g.DrawString(fntArial12, ListPresentSquad[S].SquadName, new Vector2(50, 63 + Pos * 38), Color.White);
				if (S == CursorIndex + (PageCurrent - 1) * 8)
				{
                    g.Draw(sprRectangle, new Rectangle(47, 62 + Pos * 38, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
                    g.Draw(sprRectangle, new Rectangle(47, 84 + Pos * 38, 316, 1), Color.FromNonPremultiplied(127, 107, 0, 255));
				}
                if (ListSelectedSquad.Contains(ListPresentSquad[S]))
                    g.Draw(sprCursor, new Vector2(40, 52 + Pos * 38), Color.White);
            }
            if (Stage == Stages.Warning)
            {
                g.Draw(sprWarning, new Vector2(100, 120), Color.White);
            }
            else if(Stage == Stages.Confirmation)
            {
                g.Draw(sprConfirmation, new Vector2(100, 120), Color.White);
            }
            else if (Stage == Stages.SquadInfo)
            {
                StatusMenu.Draw(g);
            }
        }
    }
}
