using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public static class MenuHelper
    {
        private static Texture2D sprTopLeft;
        private static Texture2D sprTopMiddle;
        private static Texture2D sprTopRight;
        private static Texture2D sprMiddleLeft;
        private static Texture2D sprMiddleMiddle;
        private static Texture2D sprMiddleRight;
        private static Texture2D sprBottomLeft;
        private static Texture2D sprBottomMiddle;
        private static Texture2D sprBottomRight;

        private static Texture2D sprPlayerInformation;
        private static Texture2D sprBookInformation;
        private static Texture2D sprCommand;
        private static Texture2D sprCreatureCard;
        private static Texture2D sprInformation;
        private static Texture2D sprItemCard;
        private static Texture2D sprLandInformation;
        private static Texture2D sprMapInformation;
        private static Texture2D sprMenu;
        private static Texture2D sprSpellCard;
        private static Texture2D sprSupport;

        private static Texture2D sprConfirm1;
        private static Texture2D sprConfirm2;
        private static Texture2D sprConfirm3;

        private static Texture2D sprFinger1;
        private static Texture2D sprFinger2;
        private static Texture2D sprFinger3;

        private static Texture2D sprDiceHolder;
        private static Texture2D sprDiceHolderEffect;
        private static Texture2D sprDice0;
        private static Texture2D sprDice1;
        private static Texture2D sprDice2;
        private static Texture2D sprDice3;
        private static Texture2D sprDice4;
        private static Texture2D sprDice5;
        private static Texture2D sprDice6;
        private static Texture2D sprDice7;
        private static Texture2D sprDice8;
        private static Texture2D sprDice9;

        public static Texture2D sprArrowUp;

        public static Texture2D sprCardBack;

        private static float AnimationTimer;

        public static void Init(ContentManager Content)
        {
            sprTopLeft = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Top Left");
            sprTopMiddle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Top Middle");
            sprTopRight = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Top Right");
            sprMiddleLeft = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Middle Left");
            sprMiddleMiddle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Middle Middle");
            sprMiddleRight = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Middle Right");
            sprBottomLeft = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Bottom Left");
            sprBottomMiddle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Bottom Middle");
            sprBottomRight = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Bottom Right");

            sprPlayerInformation = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Player Information");
            sprBookInformation = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Book Information");
            sprCommand = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Command");
            sprCreatureCard = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Creature Card");
            sprInformation = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Information");
            sprItemCard = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Item Card");
            sprLandInformation = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Land Information");
            sprMapInformation = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Map Information");
            sprMenu = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Menu");
            sprSpellCard = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Spell Card");
            sprSupport = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Support");

            sprConfirm1 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Hand down 1");
            sprConfirm2 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Hand down 2");
            sprConfirm3 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Hand down 3");

            sprFinger1 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Finger 1");
            sprFinger2 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Finger 2");
            sprFinger3 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Finger 3");

            sprDiceHolder = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice Holder");
            sprDiceHolderEffect = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice Holder Effect");
            sprDice0 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice 0");
            sprDice1 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice 1");
            sprDice2 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice 2");
            sprDice3 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice 3");
            sprDice4 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice 4");
            sprDice5 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice 5");
            sprDice6 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice 6");
            sprDice7 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice 7");
            sprDice8 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice 8");
            sprDice9 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Dice/Dice 9");

            sprArrowUp = Content.Load<Texture2D>("Sorcerer Street/Ressources/Arrow Up");

            sprCardBack = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Back");
        }

        public static void UpdateAnimationTimer(GameTime gameTime)
        {
            AnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public static void DrawBox(CustomSpriteBatch g, Vector2 Position, int Width, int Height)
        {
            g.Draw(sprTopLeft, new Rectangle((int)Position.X + 1, (int)Position.Y - sprTopLeft.Height, sprTopLeft.Width, sprTopLeft.Height), Color.White);
            g.Draw(sprTopMiddle, new Rectangle((int)Position.X + sprTopLeft.Width, (int)Position.Y - sprTopMiddle.Height, Width - sprTopLeft.Width - sprMiddleRight.Width, sprTopMiddle.Height), Color.White);
            g.Draw(sprTopRight, new Rectangle((int)Position.X + Width - sprTopRight.Width - 1, (int)Position.Y - sprTopRight.Height, sprTopRight.Width, sprTopRight.Height), Color.White);

            g.Draw(sprMiddleLeft, new Rectangle((int)Position.X + 1, (int)Position.Y, sprMiddleLeft.Width, Height), Color.White);
            g.Draw(sprMiddleMiddle, new Rectangle((int)Position.X + sprMiddleLeft.Width, (int)Position.Y, Width - sprMiddleLeft.Width - sprMiddleRight.Width, Height), Color.White);
            g.Draw(sprMiddleRight, new Rectangle((int)Position.X + Width - sprMiddleRight.Width - 1, (int)Position.Y, sprMiddleRight.Width, Height), Color.White);

            g.Draw(sprBottomLeft, new Rectangle((int)Position.X + 1, (int)Position.Y + Height, sprBottomLeft.Width, sprBottomLeft.Height), Color.White);
            g.Draw(sprBottomMiddle, new Rectangle((int)Position.X + sprBottomLeft.Width, (int)Position.Y + Height, Width - sprBottomLeft.Width - sprBottomRight.Width, sprBottomMiddle.Height), Color.White);
            g.Draw(sprBottomRight, new Rectangle((int)Position.X + Width - sprBottomRight.Width - 1, (int)Position.Y + Height, sprBottomRight.Width, sprBottomRight.Height), Color.White);
        }

        public static void DrawBorderlessBox(CustomSpriteBatch g, Vector2 Position, int Width, int Height)
        {
            g.Draw(sprMiddleLeft, new Rectangle((int)Position.X + 1, (int)Position.Y, sprMiddleLeft.Width, Height), Color.White);
            g.Draw(sprMiddleMiddle, new Rectangle((int)Position.X + sprMiddleLeft.Width, (int)Position.Y, Width - sprMiddleLeft.Width - sprMiddleRight.Width, Height), Color.White);
            g.Draw(sprMiddleRight, new Rectangle((int)Position.X + Width - sprMiddleRight.Width - 1, (int)Position.Y, sprMiddleRight.Width, Height), Color.White);
        }

        public static void DrawNamedBox(CustomSpriteBatch g, string Name, Vector2 Position, int Width, int Height)
        {
            int TopLeftWidth;

            switch (Name)
            {
                case "Player Information":
                    g.Draw(sprPlayerInformation, new Rectangle((int)Position.X + 1, (int)Position.Y - sprPlayerInformation.Height, sprPlayerInformation.Width, sprPlayerInformation.Height), Color.White);
                    TopLeftWidth = sprPlayerInformation.Width;
                    break;

                case "Book Information":
                    g.Draw(sprBookInformation, new Rectangle((int)Position.X + 1, (int)Position.Y - sprCommand.Height, sprCommand.Width, sprCommand.Height), Color.White);
                    TopLeftWidth = sprCommand.Width;
                    break;

                case "Command":
                    g.Draw(sprCommand, new Rectangle((int)Position.X + 1, (int)Position.Y - sprCommand.Height, sprCommand.Width, sprCommand.Height), Color.White);
                    TopLeftWidth = sprCommand.Width;
                    break;

                case "Creature":
                    g.Draw(sprCreatureCard, new Rectangle((int)Position.X + 1, (int)Position.Y - sprCreatureCard.Height, sprCreatureCard.Width, sprCreatureCard.Height), Color.White);
                    TopLeftWidth = sprCreatureCard.Width;
                    break;

                case "Information":
                    g.Draw(sprInformation, new Rectangle((int)Position.X + 1, (int)Position.Y - sprInformation.Height, sprInformation.Width, sprInformation.Height), Color.White);
                    TopLeftWidth = sprInformation.Width;
                    break;

                case "Item Card":
                    g.Draw(sprItemCard, new Rectangle((int)Position.X + 1, (int)Position.Y - sprItemCard.Height, sprItemCard.Width, sprItemCard.Height), Color.White);
                    TopLeftWidth = sprItemCard.Width;
                    break;

                case "Land Information":
                    g.Draw(sprLandInformation, new Rectangle((int)Position.X + 1, (int)Position.Y - sprLandInformation.Height, sprLandInformation.Width, sprLandInformation.Height), Color.White);
                    TopLeftWidth = sprLandInformation.Width;
                    break;

                case "Map Information":
                    g.Draw(sprMapInformation, new Rectangle((int)Position.X + 1, (int)Position.Y - sprMapInformation.Height, sprMapInformation.Width, sprMapInformation.Height), Color.White);
                    TopLeftWidth = sprMapInformation.Width;
                    break;

                case "Menu":
                    g.Draw(sprMenu, new Rectangle((int)Position.X + 1, (int)Position.Y - sprMenu.Height, sprMenu.Width, sprMenu.Height), Color.White);
                    TopLeftWidth = sprMenu.Width;
                    break;

                case "Spell Card":
                    g.Draw(sprSpellCard, new Rectangle((int)Position.X + 1, (int)Position.Y - sprSpellCard.Height, sprSpellCard.Width, sprSpellCard.Height), Color.White);
                    TopLeftWidth = sprSpellCard.Width;
                    break;

                case "Support":
                    g.Draw(sprSupport, new Rectangle((int)Position.X + 1, (int)Position.Y - sprSpellCard.Height, sprSpellCard.Width, sprSpellCard.Height), Color.White);
                    TopLeftWidth = sprSpellCard.Width;
                    break;

                default:
                    g.Draw(sprTopLeft, new Rectangle((int)Position.X + 1, (int)Position.Y - sprTopLeft.Height, sprTopLeft.Width, sprTopLeft.Height), Color.White);
                    TopLeftWidth = sprTopLeft.Width;
                    break;
            }

            g.Draw(sprTopMiddle, new Rectangle((int)Position.X + TopLeftWidth, (int)Position.Y - sprTopMiddle.Height, Width - TopLeftWidth - sprMiddleRight.Width, sprTopMiddle.Height), Color.White);
            g.Draw(sprTopRight, new Rectangle((int)Position.X + Width - sprTopRight.Width - 1, (int)Position.Y - sprTopRight.Height, sprTopRight.Width, sprTopRight.Height), Color.White);

            g.Draw(sprMiddleLeft, new Rectangle((int)Position.X + 1, (int)Position.Y, sprMiddleLeft.Width, Height), Color.White);
            g.Draw(sprMiddleMiddle, new Rectangle((int)Position.X + sprMiddleLeft.Width, (int)Position.Y, Width - sprMiddleLeft.Width - sprMiddleRight.Width, Height), Color.White);
            g.Draw(sprMiddleRight, new Rectangle((int)Position.X + Width - sprMiddleRight.Width - 1, (int)Position.Y, sprMiddleRight.Width, Height), Color.White);

            g.Draw(sprBottomLeft, new Rectangle((int)Position.X + 1, (int)Position.Y + Height, sprBottomLeft.Width, sprBottomLeft.Height), Color.White);
            g.Draw(sprBottomMiddle, new Rectangle((int)Position.X + sprBottomLeft.Width, (int)Position.Y + Height, Width - sprBottomLeft.Width - sprBottomRight.Width, sprBottomMiddle.Height), Color.White);
            g.Draw(sprBottomRight, new Rectangle((int)Position.X + Width - sprBottomRight.Width - 1, (int)Position.Y + Height, sprBottomRight.Width, sprBottomRight.Height), Color.White);
        }

        public static void DrawUpArrow(CustomSpriteBatch g)
        {
            float Y = Constants.Height - Constants.Height / 6;
            float Scale = Constants.Width / 3764.70581f;

            g.Draw(sprArrowUp, new Vector2(Constants.Width / 2, Y - 400 * Scale + (float)Math.Sin(AnimationTimer * 10) * 3f), Color.White);
        }

        public static void DrawDownArrow(CustomSpriteBatch g)
        {
            g.Draw(sprArrowUp, new Vector2(Constants.Width / 2, Constants.Height - 66 - (float)Math.Sin(AnimationTimer * 10) * 3f), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
        }

        public static void DrawRightArrow(CustomSpriteBatch g)
        {
            g.Draw(sprArrowUp, new Vector2(Constants.Width / 2, Constants.Height - Constants.Height / 6f), null, Color.White, MathHelper.ToRadians(90), Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public static void DrawDiceHolder(CustomSpriteBatch g, Vector2 DicePosition, int VisibleDiceValue)
        {
            g.Draw(sprDiceHolder, DicePosition, null, Color.White, 0f, new Vector2(sprDiceHolder.Width / 2, sprDiceHolder.Height / 2), 1f, SpriteEffects.None, 0f);
            g.Draw(sprDiceHolderEffect, DicePosition, null, Color.FromNonPremultiplied(255, 255, 255, 127), AnimationTimer, new Vector2(sprDiceHolderEffect.Width / 2, sprDiceHolderEffect.Height / 2), 1f, SpriteEffects.None, 0.1f);
            g.Draw(sprDiceHolderEffect, DicePosition, null, Color.FromNonPremultiplied(255, 255, 255, 127), -AnimationTimer, new Vector2(sprDiceHolderEffect.Width / 2, sprDiceHolderEffect.Height / 2), 1f, SpriteEffects.None, 0.15f);

            float Scale = 0.6f;
            List<int> ListDigit = new List<int>();
            float NumberWidth = 0;
            if (VisibleDiceValue == 0)
            {
                ListDigit.Add(VisibleDiceValue);
                NumberWidth += DrawDiceNumber(VisibleDiceValue).Width * Scale;
            }
            else
            {
                while (VisibleDiceValue > 0)
                {
                    int ActiveDigit = VisibleDiceValue % 10;
                    ListDigit.Add(ActiveDigit);
                    VisibleDiceValue -= ActiveDigit;
                    VisibleDiceValue = (VisibleDiceValue / 10);
                    NumberWidth += DrawDiceNumber(ActiveDigit).Width * Scale;
                }
            }

            float StartX = DicePosition.X;
            StartX -= NumberWidth / 2;
            for (int D = ListDigit.Count - 1; D >= 0; D--)
            {
                //StartX = DicePosition.X - NumberWidth / 2 + (ListDigit.Count - D - 1) * (NumberWidth / ListDigit.Count);
                int ActiveDigit = ListDigit[D];
                Texture2D sprActiveDigit = DrawDiceNumber(ActiveDigit);
                g.Draw(sprActiveDigit, new Vector2(StartX - sprActiveDigit.Width * (1 - Scale) / 2, DicePosition.Y), null, Color.FromNonPremultiplied(255, 255, 255, 127), 0,
                    new Vector2(0, sprActiveDigit.Height / 2), 1f, SpriteEffects.None, 0.2f);

                StartX += sprActiveDigit.Width * Scale;
            }
        }

        private static Texture2D DrawDiceNumber(int VisibleDiceValue)
        {
            switch (VisibleDiceValue)
            {
                case 0:
                    return sprDice0;

                case 1:
                    return sprDice1;

                case 2:
                    return sprDice2;

                case 3:
                    return sprDice3;

                case 4:
                    return sprDice4;

                case 5:
                    return sprDice5;

                case 6:
                    return sprDice6;

                case 7:
                    return sprDice7;

                case 8:
                    return sprDice8;

                case 9:
                    return sprDice9;
            }

            return sprDice0;
        }

        public static void DrawConfirmIcon(CustomSpriteBatch g, Vector2 Position)
        {
            g.Draw(sprConfirm1, Position, null, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
        }

        public static void DrawFingerIcon(CustomSpriteBatch g, Vector2 Position)
        {
            g.Draw(sprFinger1, Position, Color.White);
        }
    }
}
