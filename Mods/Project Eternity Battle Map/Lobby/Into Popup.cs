using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class IntroPopup : GameScreen
    {
        public class LobbyVisualNovel : VisualNovelScreen.VisualNovel
        {
            public Texture2D sprFrameDescription;

            public LobbyVisualNovel(string VisualNovelPath)
                : base(VisualNovelPath)
            {
            }

            protected override void DrawTextBox(CustomSpriteBatch g, int X, int Y)
            {
                base.DrawTextBox(g, X, Y);
            }
        }

        private TextButton ConfirmButton;
        private TextButton CancelButton;

        private IUIElement[] ArrayUIElement;

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntOxanimumBoldBigger;
        private SpriteFont fntOxanimumLightBigger;

        private Texture2D sprPopup;
        private Texture2D sprFrameDescription;

        private double AnimationCounter;

        private LobbyWhite Owner;
        private LobbyVisualNovel Intro;
        private CreateRoomScreen IntroCreateRoomScreen;
        private GamePreparationScreen PreparationScreen;
        private GameOptionsScreen OptionScreen;
        private GameOptionsGametypeScreen GameModeSelectionScreen;
        private GameOptionsSelectMapScreen GameMapSelectionScreen;

        public IntroPopup(LobbyWhite Owner)
        {
            this.Owner = Owner;
            RequireFocus = false;
            RequireDrawFocus = true;
        }

        public override void Load()
        {
            fntOxanimumBoldBigger = Content.Load<SpriteFont>("Fonts/Oxanium Bold Bigger");
            fntOxanimumLightBigger = Content.Load<SpriteFont>("Fonts/Oxanium Light Bigger");

            sprPopup = Content.Load<Texture2D>("Menus/Lobby/Shop/Frame Confirm Buy");
            sprFrameDescription = Content.Load<Texture2D>("Menus/Lobby/Room/Frame Description");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            float Ratio = Constants.Height / 2160f;
            int DrawX = (int)(Constants.Width / 2 - 300 * Ratio);
            int DrawY = Constants.Height / 2 - (int)(sprPopup.Height * Ratio / 2) + (int)(750 * Ratio);
            ConfirmButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Bigger}{Centered}{Color:65,70,65,255}Read}}", "Menus/Lobby/Button Color", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, StartIntro);

            DrawX = (int)(Constants.Width / 2 + 300 * Ratio);
            CancelButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Bigger}{Centered}{Color:243, 243, 243, 255}Skip}}", "Menus/Lobby/Button Close", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, Cancel);

            ArrayUIElement = new IUIElement[]
            {
                ConfirmButton, CancelButton,
            };
        }

        public void OnIntroFrameChanged()
        {
            switch (Intro.TimelineIndex)
            {
                case 2:
                    IntroCreateRoomScreen = Owner.CreateARoom();
                    ListGameScreen.Remove(Intro);
                    ListGameScreen.Insert(0, Intro);
                    break;

                case 3:
                    PreparationScreen = IntroCreateRoomScreen.CreateRoom();
                    ListGameScreen.Remove(Intro);
                    ListGameScreen.Insert(0, Intro);
                    break;

                case 7:
                    OptionScreen = PreparationScreen.OpenRoomSettingsScreen();
                    GameModeSelectionScreen = (GameOptionsGametypeScreen)OptionScreen.ArrayOptionTab[0];
                    GameMapSelectionScreen = (GameOptionsSelectMapScreen)OptionScreen.ArrayOptionTab[1];
                    ListGameScreen.Remove(Intro);
                    ListGameScreen.Insert(0, Intro);
                    break;

                case 9://Select Campaign
                    GameModeSelectionScreen.SelectGametype(0, 0);
                    break;

                case 10:
                    OptionScreen.SelectMapButton.Select();
                    OptionScreen.OnSelectMapTabPressed();
                    break;

                case 11:
                    GameMapSelectionScreen.SelectMap("Multiplayer/Campaign/Tutorial");
                    break;

                case 12:
                    OptionScreen.OnClosePressed();
                    GameModeSelectionScreen = null;
                    GameMapSelectionScreen = null;
                    OptionScreen = null;
                    break;

                case 15:
                    PreparationScreen.ReturnToLobby();
                    break;

                case 16:
                    Owner.OpenShop();
                    ListGameScreen.Remove(Intro);
                    ListGameScreen.Insert(0, Intro);
                    break;

                case 21:
                    ListGameScreen.RemoveAt(1);
                    break;

                case 22:
                    Owner.OpenInventory();
                    ListGameScreen.Remove(Intro);
                    ListGameScreen.Insert(0, Intro);
                    break;

                case 25:
                    ListGameScreen.RemoveAt(1);
                    ListGameScreen.RemoveAt(0);
                    break;
            }
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void StartIntro()
        {
            ArrayUIElement = new IUIElement[0];
            sndButtonClick.Play();

            Intro = new LobbyVisualNovel("MP Intro");
            Intro.sprFrameDescription = sprFrameDescription;
            Intro.OnVisualNovelFrameChanged += OnIntroFrameChanged;
            PushScreen(Intro);

            //hide popup
            ListGameScreen.Remove(this);
            ListGameScreen.Add(this);
        }

        private void Cancel()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Update(gameTime);
            }

            if (Intro != null)
            {
                if (Owner.OnlineGameClient != null)
                {
                }
                else
                {
                    PendingUnlockScreen.CheckForUnlocks(this);
                }

                AnimationCounter += gameTime.ElapsedGameTime.TotalSeconds * 1.8;

                switch (Intro.TimelineIndex)
                {
                    case 4:
                        if (((int)AnimationCounter) % 2 == 0)
                        {
                            PreparationScreen.RoomSettingButton.Hover();
                        }
                        else
                        {
                            PreparationScreen.RoomSettingButton.Unselect();
                        }
                        break;

                    case 5:
                        PreparationScreen.RoomSettingButton.Unselect();
                        if (((int)AnimationCounter) % 2 == 0)
                        {
                            PreparationScreen.PlayerSettingsButton.Hover();
                        }
                        else
                        {
                            PreparationScreen.PlayerSettingsButton.Unselect();
                        }
                        break;

                    case 6:
                        PreparationScreen.RoomSettingButton.Hover();
                        PreparationScreen.PlayerSettingsButton.Unselect();
                        break;

                    case 15:
                        if (((int)AnimationCounter) % 2 == 0)
                        {
                            Owner.ShopButton.Hover();
                        }
                        else
                        {
                            Owner.ShopButton.Unselect();
                        }
                        break;

                    case 21:
                        if (((int)AnimationCounter) % 2 == 0)
                        {
                            Owner.InventoryButton.Hover();
                        }
                        else
                        {
                            Owner.InventoryButton.Unselect();
                        }
                        break;
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color TextColor = Color.FromNonPremultiplied(74, 79, 74, 255);

            int DrawX = Constants.Width / 2 - (int)(sprPopup.Width * Ratio / 2);
            int DrawY = Constants.Height / 2 - (int)(sprPopup.Height * Ratio / 2);

            g.Draw(sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.FromNonPremultiplied(255, 255, 255, 50));

            g.Draw(sprPopup, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);

            g.DrawStringCentered(fntOxanimumBoldBigger, "INTRODUCTION", new Vector2(Constants.Width / 2, DrawY + 120 * Ratio), Color.White);

            g.DrawStringCentered(fntOxanimumLightBigger, "It looks like this is the first time you are\n\rplaying.", new Vector2(Constants.Width / 2, DrawY + 300 * Ratio), TextColor);

            g.DrawStringCentered(fntOxanimumLightBigger, "Would you like to read the introduction?", new Vector2(Constants.Width / 2, DrawY + 520 * Ratio), TextColor);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}
