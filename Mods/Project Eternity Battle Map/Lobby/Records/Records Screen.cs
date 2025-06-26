using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class RecordsScreenWhite : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private CubeBackgroundSmall CubeBackground;

        private SpriteFont fntArial12;
        private SpriteFont fntOxanimumBoldTitle;

        private Texture2D sprBarLeft;
        private Texture2D sprBarMiddle;

        private IUIElement[] ArrayUIElement;

        public static Color BackgroundColor = Color.FromNonPremultiplied(65, 70, 65, 255);

        public readonly int TopSectionHeight;
        private readonly BattleMapPlayer ActivePlayer;
        private readonly Roster PlayerRoster;
        private GameScreen ActiveTab;
        private GameScreen[] ArrayRecordTabs;

        public RecordsScreenWhite(BattleMapPlayer ActivePlayer, Roster PlayerRoster)
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
            fntOxanimumBoldTitle = Content.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            sprBarLeft = Content.Load<Texture2D>("Menus/Lobby/Shop/Bar Left");
            sprBarMiddle = Content.Load<Texture2D>("Menus/Lobby/Shop/Bar Middle");

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

            GameScreen PlayerTab = new PlayerRecordsTabWhite(ActivePlayer, TopSectionHeight + 55);
            GameScreen UnitTab = new UnitRecordsTabWhite(ActivePlayer, PlayerRoster, TopSectionHeight + 55);
            GameScreen BattleTab = new BattleRecordsTabWhite(ActivePlayer, TopSectionHeight + 55);
            GameScreen BonusTab = new BonusRecordsTabWhite(ActivePlayer, TopSectionHeight + 55);
            GameScreen ProgressionTab = new ProgressionRecordsTabWhite(ActivePlayer, TopSectionHeight + 55);
            GameScreen MultiplayerTab = new MultiplayerRecordsTabWhite(ActivePlayer, TopSectionHeight + 55);

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

        public override void BeginDraw(CustomSpriteBatch g)
        {
            CubeBackground.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(BackgroundColor);

            DrawBackground(g);

            foreach (var a in ArrayUIElement)
            {
                a.Draw(g);
            }

            ActiveTab.Draw(g);
        }

        private void DrawBackground(CustomSpriteBatch g)
        {
            CubeBackground.Draw(g, true);
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f;

            int LineX = 110;
            int LineY = 1907;
            g.Draw(sprBarLeft, new Rectangle((int)(LineX * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(sprBarLeft.Width * Ratio), (int)Math.Ceiling(sprBarLeft.Height * Ratio)), Color.White);
            g.Draw(sprBarMiddle, new Rectangle((int)(LineX * Ratio + sprBarLeft.Width * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(3592 * Ratio), (int)Math.Ceiling(sprBarMiddle.Height * Ratio)), Color.White);

            g.DrawString(fntOxanimumBoldTitle, "RECORDS", new Vector2((int)(210 * Ratio), (int)(58 * Ratio)), TextColor);
        }
    }
}
