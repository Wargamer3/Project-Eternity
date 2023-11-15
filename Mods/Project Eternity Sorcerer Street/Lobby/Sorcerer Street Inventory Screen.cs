﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetInventoryScreen : GameScreen
    {
        #region Ressources

        private CardSymbols Symbols;

        private SpriteFont fntArial12;

        #endregion

        private readonly Player ActivePlayer;
        private int CursorIndex;

        public SorcererStreetInventoryScreen(Player ActivePlayer)
        {
            this.ActivePlayer = ActivePlayer;
        }

        public override void Load()
        {
            Symbols = CardSymbols.Load(Content);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
        }

        public override void Update(GameTime gameTime)
        {
            MenuHelper.UpdateAnimationTimer(gameTime);

            if (InputHelper.InputConfirmPressed())
            {
                switch (CursorIndex)
                {
                    case 0:
                        PushScreen(new ChooseBookScreen(Symbols, ActivePlayer));
                        break;
                    case 1:
                        PushScreen(new BattleTesterScreen(Symbols, ActivePlayer));
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
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
                    CursorIndex = 4;
                }
            }
            else if(InputHelper.InputDownPressed())
            {
                if (++CursorIndex > 4)
                {
                    CursorIndex = 0;
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(-5, -5), Constants.Width + 10, Constants.Height + 10, Color.White);

            float X = -10;
            float Y = Constants.Height / 20;
            int HeaderHeight = Constants.Height / 16;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);

            X = Constants.Width / 20;
            Y += HeaderHeight / 2 - fntArial12.LineSpacing / 2;
            g.DrawString(fntArial12, "System", new Vector2(X, Y), Color.White);

            X = -10;
            Y = Constants.Height / 7;
            int EntryHeight = Constants.Height / 20;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Book Edit", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
            Y += EntryHeight + 10;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Change Book", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
            Y += EntryHeight + 10;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Change Parts", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
            Y += EntryHeight + 10;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Maintenance", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
            Y += EntryHeight + 10;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Return", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);

            MenuHelper.DrawFingerIcon(g, new Vector2(95, Constants.Height / 7 + EntryHeight / 3 + CursorIndex * (EntryHeight + 10)));

            DrawBookInformation(g, fntArial12, "Player Information", Symbols,
                ActivePlayer.Inventory.GlobalBook);

            X = -10;
            Y = Constants.Height - 100;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);
            X = Constants.Width / 18;
            Y += HeaderHeight / 2 - fntArial12.LineSpacing / 2;
            g.DrawString(fntArial12, "Open the maintenance menu", new Vector2(X, Y), Color.White);
        }

        public static void DrawBookInformation(CustomSpriteBatch g, SpriteFont ActiveFont, string Title, CardSymbols Symbols, CardBook ActiveBook)
        {
            float X = Constants.Width / 1.8f;
            float Y = Constants.Height / 6;
            int BookInformationWidth = (int)(Constants.Width / 2.7f);
            DrawBox(g, new Vector2(X, Y - 20), BookInformationWidth, 20, Color.White);
            g.DrawString(ActiveFont, Title, new Vector2(X + 10, Y - 20), Color.White);
            DrawBox(g, new Vector2(X, Y), BookInformationWidth, 280, Color.White);
            Y += 10;
            g.DrawString(ActiveFont, "Updated", new Vector2(X + 15, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.DrawStringRightAligned(ActiveFont, "12/30/2022 09:32", new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f;
            Y += 20;
            g.DrawString(ActiveFont, "Win / Matches", new Vector2(X + 15, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.DrawStringRightAligned(ActiveFont, "0/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0", new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f;
            Y += 20;
            g.DrawLine(sprPixel, new Vector2(X + 10, Y), new Vector2(X + BookInformationWidth - 20, Y), Color.White);
            Y += 5;
            g.DrawString(ActiveFont, "Type / Card Count", new Vector2(X + 25, Y), Color.White);
            Y += 20;
            int LineSize = 22;
            X = Constants.Width / 1.8f + 15;
            g.Draw(Symbols.sprElementNeutral, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesNeutral + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesNeutral.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(Symbols.sprElementFire, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesFire + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesFire.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(Symbols.sprElementWater, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesWater + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesWater.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(Symbols.sprElementEarth, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesEarth + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesEarth.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(Symbols.sprElementAir, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesAir + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesAir.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(Symbols.sprElementMulti, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesMulti + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesMulti.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(Symbols.sprItemsWeapon, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsWeapon + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsWeapon.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(Symbols.sprItemsArmor, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsArmor + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsArmor.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(Symbols.sprItemsTool, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsTool + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsTool.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(Symbols.sprItemsScroll, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsScroll + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsScroll.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(Symbols.sprSpellsSingle, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueSpellsSingle + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalSpellsSingle.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(Symbols.sprSpellsMultiple, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueSpellsMultiple + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalSpellsMultiple.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(Symbols.sprEnchantSingle, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueEnchantSingle + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalEnchantSingle.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(Symbols.sprEnchantMultiple, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueEnchantMultiple + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalEnchantMultiple.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 160;
            g.DrawStringRightAligned(ActiveFont, "Total", new Vector2(X + 20, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.ListCard.Count + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCards.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += 20;
            X = Constants.Width / 1.8f;
            g.DrawLine(sprPixel, new Vector2(X + 10, Y), new Vector2(X + BookInformationWidth - 20, Y), Color.White);
            X = Constants.Width / 1.8f + 15;
            Y += 5;
            g.Draw(Symbols.sprSpellsSingle, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(X + 60, Y), Color.White);
            X = Constants.Width / 1.8f + 120;
            g.Draw(Symbols.sprSpellsSingle, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(X + 60, Y), Color.White);
            X = Constants.Width / 1.8f + 210;
            g.Draw(Symbols.sprSpellsSingle, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(X + 60, Y), Color.White);

        }
    }
}
