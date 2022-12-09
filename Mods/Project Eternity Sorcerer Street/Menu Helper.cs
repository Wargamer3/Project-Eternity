using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public static class MenuHelper
    {
        public static Texture2D sprTopLeft;
        public static Texture2D sprTopMiddle;
        public static Texture2D sprTopRight;
        public static Texture2D sprMiddleLeft;
        public static Texture2D sprMiddleMiddle;
        public static Texture2D sprMiddleRight;
        public static Texture2D sprBottomLeft;
        public static Texture2D sprBottomMiddle;
        public static Texture2D sprBottomRight;

        public static Texture2D sprCepterInformation;
        public static Texture2D sprCreatureCard;
        public static Texture2D sprMapInformation;
        public static Texture2D sprMenu;

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

            sprCepterInformation = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Cepter Information");
            sprCreatureCard = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Creature Card");
            sprMapInformation = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Map Information");
            sprMenu = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Menu");
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
                case "Creature":
                    g.Draw(sprCreatureCard, new Rectangle((int)Position.X + 1, (int)Position.Y - sprCreatureCard.Height, sprCreatureCard.Width, sprCreatureCard.Height), Color.White);
                    TopLeftWidth = sprCreatureCard.Width;
                    break;

                case "Menu":
                    g.Draw(sprMenu, new Rectangle((int)Position.X + 1, (int)Position.Y - sprMenu.Height, sprMenu.Width, sprMenu.Height), Color.White);
                    TopLeftWidth = sprMenu.Width;
                    break;

                case "Information":
                    g.Draw(sprCepterInformation, new Rectangle((int)Position.X + 1, (int)Position.Y - sprCepterInformation.Height, sprCepterInformation.Width, sprCepterInformation.Height), Color.White);
                    TopLeftWidth = sprCepterInformation.Width;
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
    }
}
