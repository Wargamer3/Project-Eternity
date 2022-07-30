using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using FMOD;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class RecordsScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private IUIElement[] ArrayUIElement;

        public string UnitHighestPlayTime;
        public string CharacterHighestPlayTime;

        public string UnitLeastPlayTimeUnit;
        public string CharacterLeastPlayTime;

        public string UnitMostGameWon;
        public string CharacterMostGameWon;

        public string UnitMostGameLost;
        public string CharacterMostGameLost;

        public string UnitMostKill;
        public string CharacterMostKill;

        public string UnitLeastKill;
        public string CharacterLeastKill;

        public string UnitMostDamage;
        public string CharacterMostDamage;

        public string UnitMostDamageTaken;
        public string CharacterMostDamageTaken;

        public readonly int TopSectionHeight;
        private readonly BattleMapPlayer ActivePlayer;
        private readonly Roster PlayerRoster;
        private GameScreen ActiveTab;
        private GameScreen[] ArrayRecordTabs;

        public RecordsScreen(BattleMapPlayer ActivePlayer, Roster PlayerRoster)
            : base()
        {
            this.ActivePlayer = ActivePlayer;
            this.PlayerRoster = PlayerRoster;

            TopSectionHeight = (int)(Constants.Height * 0.1);

            this.RequireDrawFocus = true;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            int LineY = TopSectionHeight + 5;
            int TabWidth = 100;
            float NumberOfTabs = 5;
            float Spacing = (Constants.Width - (NumberOfTabs * TabWidth)) / NumberOfTabs;
            float TabX = 5;

            BoxButton PlayerButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Player", OnButtonOver, SelectPlayerRecordsButton);
            PlayerButton.CanBeChecked = true;
            PlayerButton.Select();
            TabX += Spacing + TabWidth;
            BoxButton UnitButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Units", OnButtonOver, SelectUnitRecordsButton);
            UnitButton.CanBeChecked = true;
            TabX += Spacing + TabWidth;
            BoxButton BattleButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Battle", OnButtonOver, SelectBattleRecordsButton);
            BattleButton.CanBeChecked = true;
            TabX += Spacing + TabWidth;
            BoxButton BonusButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Bonuses", OnButtonOver, SelectBonusRecordsButton);
            BonusButton.CanBeChecked = true;
            TabX += Spacing + TabWidth;
            BoxButton ProgressionButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Progression", OnButtonOver, SelectProgressionRecordsButton);
            ProgressionButton.CanBeChecked = true;
            TabX += Spacing + TabWidth;
            BoxButton MultiplayerButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Progression", OnButtonOver, SelectMultiplayerRecordsButton);
            MultiplayerButton.CanBeChecked = true;
            TabX += Spacing + TabWidth;

            ArrayUIElement = new IUIElement[] { PlayerButton, UnitButton, BattleButton, BonusButton, ProgressionButton };

            GameScreen PlayerTab = new PlayerRecordsTab(ActivePlayer, TopSectionHeight + 55);
            GameScreen UnitTab = new UnitRecordsTab(ActivePlayer, PlayerRoster, TopSectionHeight + 55);
            GameScreen BattleTab = new BattleRecordsTab(ActivePlayer, TopSectionHeight + 55);
            GameScreen BonusTab = new BonusRecordsTab(ActivePlayer, TopSectionHeight + 55);
            GameScreen ProgressionTab = new ProgressionRecordsTab(ActivePlayer, TopSectionHeight + 55);
            GameScreen MultiplayerTab = new MultiplayerRecordsTab(ActivePlayer, TopSectionHeight + 55);

            ArrayRecordTabs = new GameScreen[] { PlayerTab, UnitTab, BattleTab, BonusTab, ProgressionTab };

            foreach (GameScreen ActiveScreen in ArrayRecordTabs)
            {
                ActiveScreen.Load();
            }

            ActiveTab = ArrayRecordTabs[0];
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var a in ArrayUIElement)
            {
                a.Update(gameTime);
            }

            ActiveTab.Update(gameTime);
        }
        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        #region Buttons

        private void SelectPlayerRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[0];
            ArrayUIElement[1].Unselect();
            ArrayUIElement[2].Unselect();
            ArrayUIElement[3].Unselect();
            ArrayUIElement[4].Unselect();
        }

        private void SelectUnitRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[1];
            ArrayUIElement[0].Unselect();
            ArrayUIElement[2].Unselect();
            ArrayUIElement[3].Unselect();
            ArrayUIElement[4].Unselect();
        }

        private void SelectBattleRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[2];
            ArrayUIElement[0].Unselect();
            ArrayUIElement[1].Unselect();
            ArrayUIElement[3].Unselect();
            ArrayUIElement[4].Unselect();
        }

        private void SelectBonusRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[3];
            ArrayUIElement[0].Unselect();
            ArrayUIElement[1].Unselect();
            ArrayUIElement[3].Unselect();
            ArrayUIElement[4].Unselect();
        }

        private void SelectProgressionRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[4];
            ArrayUIElement[0].Unselect();
            ArrayUIElement[1].Unselect();
            ArrayUIElement[2].Unselect();
            ArrayUIElement[3].Unselect();
        }

        private void SelectMultiplayerRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[4];
            ArrayUIElement[0].Unselect();
            ArrayUIElement[1].Unselect();
            ArrayUIElement[2].Unselect();
            ArrayUIElement[3].Unselect();
        }

        #endregion

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin();

            DrawBox(g, new Vector2(0, 0), Constants.Width, TopSectionHeight, Color.White);
            g.DrawString(fntArial12, "RECORDS", new Vector2(10, 15), Color.White);

            DrawBox(g, new Vector2(0, TopSectionHeight), Constants.Width, 50, Color.White);

            foreach (var a in ArrayUIElement)
            {
                a.Draw(g);
            }
            int LineY = TopSectionHeight + 5;
            LineY += 40;
            DrawBox(g, new Vector2(0, LineY), Constants.Width, Constants.Height - LineY, Color.Black);

            ActiveTab.Draw(g);
        }
    }
}
