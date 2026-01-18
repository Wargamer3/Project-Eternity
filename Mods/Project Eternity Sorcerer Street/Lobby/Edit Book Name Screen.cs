using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class EditBookNameScreen : GameScreen
    {
        #region Ressources

        protected FMODSound sndButtonOver;
        protected FMODSound sndButtonClick;

        private SpriteFont fntMenuText;

        protected BoxButton CancelButton;
        protected BoxButton OKButton;

        protected TextInput BookNameInput;

        private IUIElement[] ArrayMenuButton;

        #endregion

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;
        private readonly string OriginalBookName;

        private bool CreateNewBook;

        public EditBookNameScreen(Player ActivePlayer)
        {
            this.ActivePlayer = ActivePlayer;
            OriginalBookName = "New Book";
            CreateNewBook = true;
        }

        public EditBookNameScreen(Player ActivePlayer, CardBook ActiveBook)
        {
            this.ActivePlayer = ActivePlayer;
            this.ActiveBook = ActiveBook;
            OriginalBookName = ActiveBook.BookName;
            CreateNewBook = false;
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial12");

            int BoxWidth = (int)(Constants.Width * 0.55);
            int InnerBoxWidth = (int)(BoxWidth * 0.95);
            int BoxHeigth = (int)(Constants.Height * 0.2);
            int BoxX = Constants.Width / 2 - BoxWidth / 2;
            int InnerBoxX = BoxX + (int)(BoxWidth * 0.025);
            int BoxY = Constants.Height / 2 - BoxHeigth / 2;

            int RoomInfoX = InnerBoxX;
            int RoomInfoY = BoxY + (int)(BoxHeigth * 0.1);
            int RoomInfoHeight = (int)(BoxHeigth * 0.5);

            int ButtonsWidth = (int)(BoxWidth * 0.17);
            int ButtonsHeight = 29;
            int ButtonOKX = InnerBoxX + (int)(InnerBoxWidth * 0.8);
            int ButtonCancelX = ButtonOKX - ButtonsWidth - 5;
            int ButtonsY = BoxY + BoxHeigth - ButtonsHeight - 10;

            CancelButton = new BoxButton(new Rectangle(ButtonCancelX, ButtonsY, ButtonsWidth, ButtonsHeight), fntMenuText, "Cancel", OnButtonOver, Cancel);
            OKButton = new BoxButton(new Rectangle(ButtonOKX, ButtonsY, ButtonsWidth, ButtonsHeight), fntMenuText, "OK", OnButtonOver, ConfirmName);

            BookNameInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(RoomInfoX + 74, RoomInfoY + 20), new Vector2(314, 20));

            BookNameInput.SetText(OriginalBookName);

            ArrayMenuButton = new IUIElement[]
            {
                BookNameInput,
                CancelButton, OKButton,
            };
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Update(gameTime);
            }
        }

        #region Button methods

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        public void ConfirmName()
        {
            if (CreateNewBook)
            {
                CardBook NewBook = new CardBook(BookNameInput.Text);
                ActivePlayer.Inventory.RootBookContainer.AddBook(NewBook);
            }
            else
            {
                ActiveBook.BookName = BookNameInput.Text;
            }

            sndButtonClick.Play();
            OKButton.Disable();
            RemoveScreen(this);
        }

        public void Cancel()
        {
            if (!CreateNewBook)
            {
                ActiveBook.BookName = OriginalBookName;
            }

            sndButtonClick.Play();
            RemoveScreen(this);
        }

        #endregion

        public override void Draw(CustomSpriteBatch g)
        {
            int BoxWidth = (int)(Constants.Width * 0.55);
            int InnerBoxWidth = (int)(BoxWidth * 0.95);
            int BoxHeigth = (int)(Constants.Height * 0.2);
            int BoxX = Constants.Width / 2 - BoxWidth / 2;
            int InnerBoxX = BoxX + (int)(BoxWidth * 0.025);
            int BoxY = Constants.Height / 2 - BoxHeigth / 2;

            int RoomInfoX = InnerBoxX;
            int RoomInfoY = BoxY + (int)(BoxHeigth * 0.1);
            int RoomInfoHeight = (int)(BoxHeigth * 0.5);
            DrawBox(g, new Vector2(BoxX, BoxY), BoxWidth, BoxHeigth, Color.White);
            g.DrawString(fntMenuText, "Choose Book Name", new Vector2(BoxX + 20, BoxY + 15), Color.White);
            DrawBox(g, new Vector2(RoomInfoX, RoomInfoY), InnerBoxWidth, RoomInfoHeight, Color.White);
            g.DrawString(fntMenuText, "Name", new Vector2(BoxX + 25, RoomInfoY + 18), Color.White);
            DrawBox(g, new Vector2(RoomInfoX + 64, RoomInfoY + 15), (int)(BoxWidth * 0.75), 30, Color.White);

            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}
