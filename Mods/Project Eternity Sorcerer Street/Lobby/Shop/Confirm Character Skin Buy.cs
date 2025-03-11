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
    public class ShopConfirmBuyCharacterSkinScreen : GameScreen
    {
        private EmptyBoxButton BuyButton;
        private EmptyBoxButton CancelButton;

        private IUIElement[] ArrayUIElement;

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntFinlanderFont;

        private Texture2D sprLand;
        private Texture2D sprSea;
        private Texture2D sprSky;
        private Texture2D sprSpace;

        private SorcererStreetInventory Inventory;
        private string OwnerUnitTypeAndRelativePath;
        private UnlockableCharacterSkin CharacterSkinToBuy;

        private double AnimationProgression;

        int MenuSizeX;
        int MenuSizeY;
        int DrawX;
        int DrawY;

        public ShopConfirmBuyCharacterSkinScreen(SorcererStreetInventory Inventory, string OwnerUnitTypeAndRelativePath, UnlockableCharacterSkin CharacterSkinToBuy)
        {
            this.Inventory = Inventory;
            this.OwnerUnitTypeAndRelativePath = OwnerUnitTypeAndRelativePath;
            this.CharacterSkinToBuy = CharacterSkinToBuy;

            MenuSizeX = Constants.Width / 3;
            MenuSizeY = Constants.Height / 2;
            DrawX = Constants.Width / 2 - MenuSizeX / 2;
            DrawY = MenuSizeY / 2;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

            sprLand = Content.Load<Texture2D>("Menus/Status Screen/Ground");
            sprSea = Content.Load<Texture2D>("Menus/Status Screen/Sea");
            sprSky = Content.Load<Texture2D>("Menus/Status Screen/Sky");
            sprSpace = Content.Load<Texture2D>("Menus/Status Screen/Space");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            BuyButton = new EmptyBoxButton(new Rectangle(DrawX + 50, DrawY + MenuSizeY - 100, 100, 50), fntArial12, "BUY", OnButtonOver, BuySkin);
            CancelButton = new EmptyBoxButton(new Rectangle(DrawX + MenuSizeX - 150, DrawY + MenuSizeY - 100, 100, 50), fntArial12, "CANCEL", OnButtonOver, Cancel);

            ArrayUIElement = new IUIElement[]
            {
                BuyButton, CancelButton,
            };
        }

        public override void Update(GameTime gameTime)
        {
            AnimationProgression += gameTime.ElapsedGameTime.TotalSeconds * 2d;
            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Update(gameTime);
            }
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void BuySkin()
        {
            sndButtonClick.Play();
            if (Inventory.DicOwnedCharacter.ContainsKey(OwnerUnitTypeAndRelativePath))
            {
                Inventory.DicOwnedCharacter[OwnerUnitTypeAndRelativePath].ListOwnedUnitSkin.Add(new PlayerCharacterSkin(OwnerUnitTypeAndRelativePath, CharacterSkinToBuy.SkinTypeAndPath, CharacterSkinToBuy.CharacterSkinToBuy));
            }
            else
            {
                Inventory.DicOwnedCharacterSkin.Add(OwnerUnitTypeAndRelativePath, new PlayerCharacterSkin(OwnerUnitTypeAndRelativePath, CharacterSkinToBuy.SkinTypeAndPath, CharacterSkinToBuy.CharacterSkinToBuy));
            }

            RemoveScreen(this);
        }


        private void Cancel()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }


        public override void Draw(CustomSpriteBatch g)
        {
            int MenuSizeX = 760;
            int MenuSizeY = Constants.Height / 2;
            int DrawX = Constants.Width / 2 - MenuSizeX / 2;
            int DrawY = MenuSizeY / 2;

            g.Draw(sprPixel, new Rectangle(DrawX, DrawY, MenuSizeX, MenuSizeY),
                Color.FromNonPremultiplied(
                    (int)(ShopScreen.BackgroundColor.R * 0.9),
                    (int)(ShopScreen.BackgroundColor.G * 0.9),
                    (int)(ShopScreen.BackgroundColor.B * 0.9), 240));

            DrawEmptyBox(g, new Vector2(DrawX, DrawY), MenuSizeX, MenuSizeY, 7, MenuSizeX / 21, AnimationProgression);

            g.DrawStringCentered(fntArial12, "BUY A UNIT SKIN", new Vector2(DrawX + MenuSizeX / 2, DrawY + 25), Color.White);

            g.Draw(sprPixel, new Rectangle((int)(DrawX + MenuSizeX * 0.05f), DrawY + 50, (int)(MenuSizeX * 0.9f), 1), Color.White);

            g.Draw(CharacterSkinToBuy.CharacterSkinToBuy.SpriteMap, new Vector2(DrawX + MenuSizeX / 2 - CharacterSkinToBuy.CharacterSkinToBuy.SpriteMap.Width / 2, DrawY + 70), Color.White);

            g.DrawStringCentered(fntArial12, CharacterSkinToBuy.CharacterSkinToBuy.Name.ToUpper(), new Vector2(DrawX + MenuSizeX / 2, DrawY + 135), Color.White);

            g.DrawStringCentered(fntArial12, CharacterSkinToBuy.CharacterSkinToBuy.Price + " cr", new Vector2(DrawX + MenuSizeX / 2, DrawY + 175), Color.White);

            DrawSelectedUnitStats(g, DrawX + 20, DrawY + 220, CharacterSkinToBuy.CharacterSkinToBuy);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }
        }

        private void DrawSelectedUnitStats(CustomSpriteBatch g, int DrawX, int DrawY, PlayerCharacter ActiveUnit)
        {
            int BottomHeight = 182;
            DrawEmptyBox(g, new Vector2(DrawX, DrawY - 5), 240, BottomHeight);

            int DistanceBetweenText = 16;
            int MenuOffset = (int)(DistanceBetweenText * 0.5);

            DrawX += 200;
            int CurrentY = DrawY + 52;

            DrawEmptyBox(g, new Vector2(DrawX, DrawY), 130, BottomHeight);

            DrawX += 116;

            DrawEmptyBox(g, new Vector2(DrawX, DrawY), 130, BottomHeight);
        }
    }
}
