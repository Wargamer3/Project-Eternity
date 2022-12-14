using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Editors.AnimationBackgroundEditor
{
    class AnimationBackgroundViewerControl : GraphicsDeviceControl
    {
        public ContentManager content;
        public CustomSpriteBatch g;
        public AnimationBackground3D ActiveAnimationBackground;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;

            content = new ContentManager(Services, "Content");
            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
        }

        public void Preload()
        {
            OnCreateControl();
        }

        public AnimationBackground3DBase AddBackgroundSystem(string Path)
        {
            string BackgroundType = Path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[0];
            if (BackgroundType == "Background Sprites")
            {
                Path = Path.Substring(BackgroundType.Length + 1);
                BillboardSystem NewParticleSystem = new BillboardSystem("Animations/Background Sprites/" + Path, 20000, 1, BlendState.NonPremultiplied, true, content, GraphicsDevice);
                return new AnimationBackground3DBillboard(NewParticleSystem);
            }
            else if (BackgroundType == "Models")
            {
                Path = Path.Substring(BackgroundType.Length + 1);
                AnimatedModel NewModel = new AnimatedModel("Animations/Models/" + Path);
                NewModel.LoadContent(content);
                return new AnimationBackground3DModel(NewModel);
            }
            return null;
        }

        public void RemoveBackgroundSystem(int Index)
        {
            ActiveAnimationBackground.ListBackground.RemoveAt(Index);
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            ActiveAnimationBackground.Draw(g, Width, Height);
        }
    }
}