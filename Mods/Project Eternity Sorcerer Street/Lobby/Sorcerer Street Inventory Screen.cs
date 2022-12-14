using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetInventoryScreen : GameScreen
    {
        #region Ressources

        private SpriteFont fntArial12;

        public Texture2D sprMenuCursor;

        public Texture2D sprElementAir;
        public Texture2D sprElementEarth;
        public Texture2D sprElementFire;
        public Texture2D sprElementWater;
        public Texture2D sprElementNeutral;
        public Texture2D sprElementMulti;

        public Texture2D sprItemsWeapon;
        public Texture2D sprItemsArmor;
        public Texture2D sprItemsTool;
        public Texture2D sprItemsScroll;

        public Texture2D sprSpellsSingle;
        public Texture2D sprSpellsMultiple;

        public Texture2D sprEnchantSingle;
        public Texture2D sprEnchantMultiple;

        #endregion

        private readonly Player ActivePlayer;
        private int CursorIndex;

        public SorcererStreetInventoryScreen(Player ActivePlayer)
        {
            this.ActivePlayer = ActivePlayer;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sprMenuCursor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cursor");

            sprElementAir = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Air");
            sprElementEarth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Earth");
            sprElementFire = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Fire");
            sprElementWater = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Water");
            sprElementMulti = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Multi");
            sprElementNeutral = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Neutral");

            sprItemsWeapon = Content.Load<Texture2D>("Sorcerer Street/Ressources/Weapon");
            sprItemsArmor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Armor");
            sprItemsTool = Content.Load<Texture2D>("Sorcerer Street/Ressources/Tool");
            sprItemsScroll = Content.Load<Texture2D>("Sorcerer Street/Ressources/Scroll");

            sprSpellsSingle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Single Instant");
            sprSpellsMultiple = Content.Load<Texture2D>("Sorcerer Street/Ressources/Multiple Instant");

            sprEnchantSingle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Single Enchant");
            sprEnchantMultiple = Content.Load<Texture2D>("Sorcerer Street/Ressources/Multiple Enchant");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                switch (CursorIndex)
                {
                    case 0:
                        PushScreen(new ChooseBookScreen(ActivePlayer));
                        break;
                    case 1:
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

            g.Draw(sprMenuCursor, new Rectangle(95, Constants.Height / 7 + EntryHeight / 3 + CursorIndex * (EntryHeight + 10), 40, 40), Color.White);

            DrawBookInformation(g, fntArial12, "Cepter Information", sprElementNeutral, sprElementFire, sprElementWater, sprElementEarth, sprElementAir, sprElementMulti,
                sprItemsWeapon, sprItemsArmor, sprItemsTool, sprItemsScroll, sprSpellsSingle, sprSpellsMultiple, sprEnchantSingle, sprEnchantMultiple,
                ActivePlayer.Inventory.GlobalBook);

            X = -10;
            Y = Constants.Height - 100;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);
            X = Constants.Width / 18;
            Y += HeaderHeight / 2 - fntArial12.LineSpacing / 2;
            g.DrawString(fntArial12, "Open the maintenance menu", new Vector2(X, Y), Color.White);
        }

        public static void DrawBookInformation(CustomSpriteBatch g, SpriteFont ActiveFont, string Title,
            Texture2D sprElementNeutral, Texture2D sprElementFire, Texture2D sprElementWater, Texture2D sprElementEarth, Texture2D sprElementAir, Texture2D sprElementMulti,
            Texture2D sprItemsWeapon, Texture2D sprItemsArmor, Texture2D sprItemsTool, Texture2D sprItemsScroll, Texture2D sprSpellsSingle, Texture2D sprSpellsMultiple,
            Texture2D sprEnchantSingle, Texture2D sprEnchantMultiple,
            CardBook ActiveBook)
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
            g.Draw(sprElementNeutral, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesNeutral + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesNeutral.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(sprElementFire, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesFire + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesFire.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(sprElementWater, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesWater + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesWater.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(sprElementEarth, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesEarth + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesEarth.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(sprElementAir, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesAir + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesAir.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(sprElementMulti, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueCreaturesMulti + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalCreaturesMulti.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(sprItemsWeapon, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsWeapon + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsWeapon.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(sprItemsArmor, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsArmor + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsArmor.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(sprItemsTool, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsTool + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsTool.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(sprItemsScroll, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueItemsScroll + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalItemsScroll.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(sprSpellsSingle, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueSpellsSingle + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalSpellsSingle.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(sprSpellsMultiple, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueSpellsMultiple + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalSpellsMultiple.ToString(), new Vector2(X + 110, Y), Color.White);
            Y += LineSize;
            X = Constants.Width / 1.8f + 15;
            g.Draw(sprEnchantSingle, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.UniqueEnchantSingle + "/", new Vector2(X + 60, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, ActiveBook.TotalEnchantSingle.ToString(), new Vector2(X + 110, Y), Color.White);
            X = Constants.Width / 1.8f + 160;
            g.Draw(sprEnchantMultiple, new Vector2(X, Y), Color.White);
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
            g.Draw(sprSpellsSingle, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(X + 60, Y), Color.White);
            X = Constants.Width / 1.8f + 120;
            g.Draw(sprSpellsSingle, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(X + 60, Y), Color.White);
            X = Constants.Width / 1.8f + 210;
            g.Draw(sprSpellsSingle, new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(ActiveFont, "0%", new Vector2(X + 60, Y), Color.White);

        }
    }
}
