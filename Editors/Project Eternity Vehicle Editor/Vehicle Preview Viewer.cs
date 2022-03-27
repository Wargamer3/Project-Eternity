using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Vehicle;

namespace ProjectEternity.Editors.VehicleEditor
{
    internal class VehiclePreviewViewerControl : GraphicsDeviceControl
    {
        public ContentManager content;
        private CustomSpriteBatch g;
        private Texture2D sprPixel;
        private AnimatedSprite Sprite;
        public List<VehicleSeat> ListSeat;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;

            ListSeat = new List<VehicleSeat>();

            content = new ContentManager(Services, "Content");

            sprPixel = content.Load<Texture2D>("Pixel");

            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
        }

        public void Preload()
        {
            OnCreateControl();
        }

        public void SetSprite(string AnimationPath)
        {
            Sprite = new AnimatedSprite(content, "Vehicles/Sprites/" + AnimationPath, Vector2.Zero);
            Size = new System.Drawing.Size(Sprite.SpriteWidth * 2, Sprite.SpriteHeight * 2);
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            if (Sprite != null)
            {
                g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                if (Sprite.AnimationEnded)
                    Sprite.RestartAnimation();

                Sprite.Draw(g, new Vector2(Sprite.SpriteWidth, Sprite.SpriteHeight), Color.White);

                foreach (VehicleSeat ActiveSeat in ListSeat)
                {
                    Vector2 SeatPosition = new Vector2(ActiveSeat.SeatOffset.X + Sprite.SpriteWidth / 2, ActiveSeat.SeatOffset.Y + Sprite.SpriteHeight / 2);
                    g.Draw(sprPixel, new Rectangle((int)SeatPosition.X - 2,
                        (int)SeatPosition.Y - 2, 5, 5), Color.Black);
                    g.Draw(sprPixel, new Rectangle((int)SeatPosition .X - 1,
                        (int)SeatPosition .Y- 1, 3, 3), Color.Red);

                    float WeaponAngleMin = MathHelper.ToRadians(ActiveSeat.Weapon.MinAngleLateral);
                    float WeaponAngleMax = MathHelper.ToRadians(ActiveSeat.Weapon.MaxAngleLateral);

                    GameScreen.DrawLine(g, SeatPosition, SeatPosition + new Vector2((float)Math.Cos(WeaponAngleMin) * 20, (float)Math.Sin(WeaponAngleMin) * 20), Color.Green, 2);
                    GameScreen.DrawLine(g, SeatPosition, SeatPosition + new Vector2((float)Math.Cos(WeaponAngleMax) * 20, (float)Math.Sin(WeaponAngleMax) * 20), Color.Green, 2);
                }

                g.End();
            }
        }
    }
}
