using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetInventoryScreen : GameScreen
    {
        #region Ressources

        private SpriteFont fntOxanimumBoldTitle;
        private SpriteFont fntMenuText;
        private SpriteFont fntOxanimumRegular;

        public static CubeBackgroundSmall CubeBackground;

        private CardSymbols Symbols;
        private IconHolder Icons;

        private Texture2D sprExtraFrame;
        private Texture2D sprFrameTop;

        #endregion

        private readonly Player ActivePlayer;
        private int CursorIndex;

        public SorcererStreetInventoryScreen(Player ActivePlayer)
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            this.ActivePlayer = ActivePlayer;

            CubeBackground = new CubeBackgroundSmall();
        }

        public override void Load()
        {
            Symbols = CardSymbols.Symbols;
            Icons = IconHolder.Icons;

            CubeBackground.Load(Content);

            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumRegular = Content.Load<SpriteFont>("Fonts/Oxanium Regular");
            fntOxanimumBoldTitle = GameScreen.ContentFallback.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            sprExtraFrame = Content.Load<Texture2D>("Menus/Lobby/Extra Frame 2");
            sprFrameTop = Content.Load<Texture2D>("Menus/Lobby/Room/Frame Top");
        }

        public override void Update(GameTime gameTime)
        {
            MenuHelper.UpdateAnimationTimer(gameTime);

            float Ratio = Constants.Height / 2160f;
            int EntryHeight = (int)(108 * Ratio);
            int DrawX = (int)(250 * Ratio);
            int DrawY = (int)(470 * Ratio);

            if (MouseHelper.MouseMoved() && MouseHelper.MouseStateCurrent.X >= DrawX && MouseHelper.MouseStateCurrent.X - DrawX < (int)(sprFrameTop.Width * Ratio))
            {
                int MouseIndex = (int)Math.Floor((MouseHelper.MouseStateCurrent.Y - (float)DrawY) / EntryHeight);

                if (MouseIndex >= 0 && MouseIndex <= 6)
                {
                    CursorIndex = MouseIndex;
                }
            }

            if (InputHelper.InputConfirmPressed())
            {
                switch (CursorIndex)
                {
                    case 0:
                        PushScreen(new ChooseBookScreen(Symbols, Icons, ActivePlayer));
                        break;
                    case 1:
                        break;
                    case 2:
                        PushScreen(new CharacterSelectionScreen(Symbols, ActivePlayer));
                        break;
                    case 3:
                        break;
                    case 4:
                        PushScreen(new BattleTesterScreen(Symbols, ActivePlayer));
                        break;
                    case 5:
                        ActivePlayer.SaveLocally();
                        break;
                    case 6:
                        RemoveScreen(this);
                        break;
                }
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
            else if (InputHelper.InputUpPressed())
            {
                if (--CursorIndex < 0)
                {
                    CursorIndex = 6;
                }
            }
            else if(InputHelper.InputDownPressed())
            {
                if (++CursorIndex > 6)
                {
                    CursorIndex = 0;
                }
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            CubeBackground.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color ColorBox = Color.FromNonPremultiplied(204, 204, 204, 255);
            Color ColorText = Color.FromNonPremultiplied(65, 70, 65, 255);

            CubeBackground.Draw(g, true);

            g.DrawString(fntOxanimumBoldTitle, "INVENTORY", new Vector2((int)(210 * Ratio), (int)(58 * Ratio)), ColorText);

            float DrawX = (int)(150 * Ratio);
            float DrawY = (int)(400 * Ratio);

            int EntryHeight = (int)(108 * Ratio);
            int BoxHeight = (int)(994 * Ratio);

            g.Draw(sprFrameTop, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.Draw(sprPixel, new Rectangle((int)(DrawX), (int)(DrawY + sprFrameTop.Height * Ratio), (int)(sprFrameTop.Width * Ratio), BoxHeight), ColorBox);
            g.Draw(sprFrameTop, new Vector2(DrawX, DrawY + sprFrameTop.Height * Ratio + BoxHeight), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.FlipVertically, 0.9f);

            DrawX =  (int)(250 * Ratio);
            DrawY = (int)(470 * Ratio) + EntryHeight / 2 - fntOxanimumRegular.LineSpacing / 2;

            g.DrawString(fntOxanimumRegular, "Book Edit", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Change Book", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Character Selection", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Maintenance", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Battle Tester", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Save", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Return", new Vector2(DrawX, DrawY), ColorText);

            DrawX = (int)(40 * Ratio);
            DrawY = (int)(470 * Ratio);
            MenuHelper.DrawFingerIcon(g, new Vector2(DrawX, DrawY + CursorIndex * EntryHeight));

            DrawBookInformation(g, sprExtraFrame, fntMenuText, "Player Information", Symbols, Icons, ActivePlayer.Inventory.GlobalBook);

            DrawX = (int)(212 * Ratio);
            DrawY = (int)(2008 * Ratio);
            g.DrawString(fntOxanimumRegular, "Open the maintenance menu", new Vector2(DrawX, DrawY), ColorText);
        }


        public static void DrawBookIsReady(CustomSpriteBatch g, SpriteFont ActiveFont, string PlayerName, CardBook ActiveBook)
        {
            Color ColorBox = Color.FromNonPremultiplied(204, 204, 204, 255);
            Color ColorText = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f;

            int DrawX = Constants.Width - Constants.Width / 8;
            int DrawY = (int)(80 * Ratio);
            g.DrawStringMiddleAligned(ActiveFont, PlayerName + "/" + ActiveBook.BookName, new Vector2(Constants.Width / 2, DrawY), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCards + " card(s)", new Vector2(DrawX, DrawY), ColorText);
            if (ActiveBook.TotalCards < 50)
            {
                g.DrawString(ActiveFont, "Too low", new Vector2(DrawX + 20, DrawY), ColorText);
            }
            else if (ActiveBook.TotalCards > 50)
            {
                g.DrawString(ActiveFont, "Too high", new Vector2(DrawX + 20, DrawY), ColorText);
            }
            else
            {
                g.DrawString(ActiveFont, "OK", new Vector2(DrawX + 20, DrawY), ColorText);
            }
        }

        public static void DrawBookInformation(CustomSpriteBatch g, Texture2D sprExtraFrame, SpriteFont ActiveFont, string Title, CardSymbols Symbols, IconHolder Icons, CardBook ActiveBook)
        {
            Color ColorBox = Color.FromNonPremultiplied(204, 204, 204, 255);
            Color ColorText = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f;

            int OffsetX = (int)(2750 * Ratio);
            int OffsetY = (int)(400 * Ratio);

            int IconWidth = (int)(32 * 1.0f);
            int IconHeight = (int)(32 * 1.0f);

            int LineSize = (int)(60 * Ratio);

            int DrawX = OffsetX;
            int DrawY = OffsetY;
            g.Draw(sprExtraFrame, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);

            DrawX = OffsetX;
            DrawY = OffsetY + (int)(50 * Ratio);

            int BookInformationWidth = (int)(792 * Ratio);
            g.DrawString(ActiveFont, Title, new Vector2(DrawX + 10, DrawY - 20), ColorText);
            DrawY += (int)(20 * Ratio);
            g.DrawString(ActiveFont, "Updated", new Vector2(DrawX + 15, DrawY), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.DrawStringRightAligned(ActiveFont, $"{ActiveBook.LastModification:dd/MM/yyyy HH:mm}", new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(82 * Ratio);
            DrawY += (int)(40 * Ratio);
            g.DrawString(ActiveFont, "Win / Matches", new Vector2(DrawX + 15, DrawY), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.DrawStringRightAligned(ActiveFont, "0/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, "0", new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(82 * Ratio);
            DrawY += (int)(40 * Ratio);
            g.DrawLine(sprPixel, new Vector2(DrawX + 10, DrawY), new Vector2(DrawX + BookInformationWidth - 20, DrawY), ColorText);
            DrawY += (int)(10 * Ratio);
            g.DrawString(ActiveFont, "Type / Card Count", new Vector2(DrawX + 25, DrawY), ColorText);
            DrawY += (int)(40 * Ratio);

            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprElementNeutral, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesNeutral + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesNeutral.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprElementFire, new Vector2(DrawX, DrawY), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesFire + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesFire.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprElementWater, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesWater + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesWater.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprElementEarth, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesEarth + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesEarth.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprElementAir, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesAir + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesAir.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprElementMulti, new Vector2(DrawX, DrawY), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesMulti + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesMulti.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprItemsWeapon, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsWeapon + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsWeapon.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprItemsArmor, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsArmor + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsArmor.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprItemsTool, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsTool + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsTool.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprItemsScroll, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsScroll + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsScroll.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprSpellsSingle, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueSpellsSingle + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalSpellsSingle.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprSpellsMultiple, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueSpellsMultiple + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalSpellsMultiple.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprEnchantSingle, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueEnchantSingle + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalEnchantSingle.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprEnchantMultiple, new Vector2(DrawX, DrawY), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueEnchantMultiple + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalEnchantMultiple.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(402 * Ratio);
            g.DrawStringRightAligned(ActiveFont, "Total", new Vector2(DrawX + 20, DrawY), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.ListCard.Count + "/", new Vector2(DrawX + 60, DrawY), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCards.ToString(), new Vector2(DrawX + 110, DrawY), ColorText);
            DrawY += (int)(50 * Ratio);
            DrawX = OffsetX + (int)(82 * Ratio);
            g.DrawLine(sprPixel, new Vector2(DrawX + 10, DrawY), new Vector2(DrawX + BookInformationWidth - 20, DrawY), ColorText);
            DrawX = OffsetX + (int)(102 * Ratio);
            DrawY += (int)(10 * Ratio);
            g.Draw(Symbols.sprSpellsSingle, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(302 * Ratio);
            g.Draw(Symbols.sprSpellsSingle, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(502 * Ratio);
            g.Draw(Symbols.sprSpellsSingle, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
        }

        public static void DrawBookInformationSmall(CustomSpriteBatch g, Texture2D sprExtraFrame, SpriteFont ActiveFont, string Title, CardSymbols Symbols, IconHolder Icons, CardBook ActiveBook)
        {
            Color ColorBox = Color.FromNonPremultiplied(204, 204, 204, 255);
            Color ColorText = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f;

            int OffsetX = (int)(2750 * Ratio);
            int OffsetY = (int)(400 * Ratio);

            int IconWidth = (int)(32 * 1.0f);
            int IconHeight = (int)(32 * 1.0f);

            int LineSize = (int)(60 * Ratio);

            int DrawX = OffsetX;
            int DrawY = OffsetY;
            g.Draw(sprExtraFrame, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);

            DrawX = OffsetX;
            DrawY = OffsetY + (int)(50 * Ratio);
            g.DrawString(ActiveFont, Title, new Vector2(DrawX + 10, DrawY - 20), ColorText);
            DrawY += (int)(20 * Ratio);

            int BookInformationWidth = (int)(792 * Ratio);

            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprElementNeutral, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesNeutral + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesNeutral.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprElementFire, new Vector2(DrawX, DrawY), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesFire + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesFire.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprElementWater, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesWater + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesWater.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprElementEarth, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesEarth + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesEarth.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprElementAir, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesAir + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesAir.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprElementMulti, new Vector2(DrawX, DrawY), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesMulti + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesMulti.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprItemsWeapon, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsWeapon + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsWeapon.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprItemsArmor, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsArmor + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsArmor.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprItemsTool, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsTool + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsTool.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprItemsScroll, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsScroll + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsScroll.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprSpellsSingle, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueSpellsSingle + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalSpellsSingle.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprSpellsMultiple, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueSpellsMultiple + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalSpellsMultiple.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(102 * Ratio);
            g.Draw(Symbols.sprEnchantSingle, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueEnchantSingle + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalEnchantSingle.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(402 * Ratio);
            g.Draw(Symbols.sprEnchantMultiple, new Vector2(DrawX, DrawY), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueEnchantMultiple + "/", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalEnchantMultiple.ToString(), new Vector2(DrawX + 110, DrawY + 10 * Ratio), ColorText);
            DrawY += LineSize;
            DrawX = OffsetX + (int)(402 * Ratio);
            g.DrawStringRightAligned(ActiveFont, "Total", new Vector2(DrawX + 20, DrawY), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.ListCard.Count + "/", new Vector2(DrawX + 60, DrawY), ColorText);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCards.ToString(), new Vector2(DrawX + 110, DrawY), ColorText);
            DrawY += (int)(50 * Ratio);
            DrawX = OffsetX + (int)(82 * Ratio);
            g.DrawLine(sprPixel, new Vector2(DrawX + 10, DrawY), new Vector2(DrawX + BookInformationWidth - 20, DrawY), ColorText);
            DrawX = OffsetX + (int)(102 * Ratio);
            DrawY += (int)(10 * Ratio);
            g.Draw(Symbols.sprSpellsSingle, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(302 * Ratio);
            g.Draw(Symbols.sprSpellsSingle, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);
            DrawX = OffsetX + (int)(502 * Ratio);
            g.Draw(Symbols.sprSpellsSingle, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(DrawX + 60, DrawY + 10 * Ratio), ColorText);

            DrawX = OffsetX + (int)(102 * Ratio);
            DrawY = (int)(1100 * Ratio);
            g.Draw(Icons.sprOption, new Rectangle(DrawX, DrawY, IconWidth, IconHeight), Color.White);
            g.DrawString(ActiveFont, "Information Display On/Off", new Vector2(DrawX + 50, DrawY + 10 * Ratio), ColorText);
        }
    }
}
