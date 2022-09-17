using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Magic;
using ProjectEternity.Core.ControlHelper;
using System.Collections.Generic;

namespace ProjectEternity.Editors.SpellEditor
{
    class SpellEditorViewerControl : GraphicsDeviceControl
    {
        public ContentManager content;
        public CustomSpriteBatch g;

        private MagicSpell ActiveSpell;
        private MagicEditor ActiveMagicEditor;

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

        public void Init(string Name)
        {
            Projectile2DContext GlobalProjectileContext = new Projectile2DContext();
            Projectile2DParams MagicProjectileParams = new Projectile2DParams(GlobalProjectileContext);

            MagicUserContext GlobalMagicContext = new MagicUserContext();
            MagicUserParams MagicParams = new MagicUserParams(GlobalMagicContext);

            Dictionary<string, MagicElement> DicMagicElement = MagicElement.LoadRegularCore(MagicParams);
            foreach (KeyValuePair<string, MagicElement> ActiveMagicElement in MagicElement.LoadProjectileCores(MagicParams, MagicProjectileParams))
            {
                DicMagicElement.Add(ActiveMagicElement.Key, ActiveMagicElement.Value);
            }
            foreach (KeyValuePair<string, MagicElement> ActiveMagicElement in MagicElement.LoadElements(MagicParams))
            {
                DicMagicElement.Add(ActiveMagicElement.Key, ActiveMagicElement.Value);
            }

            ActiveSpell = new MagicSpell(Name, null, GlobalMagicContext, DicMagicElement);
            ActiveMagicEditor = new MagicEditor(ActiveSpell, GlobalProjectileContext, MagicProjectileParams.SharedParams);
            ActiveMagicEditor.Content = new ContentManager(Services);
            ActiveMagicEditor.Content.RootDirectory = "Content";
            Services.AddService(GraphicsDevice);
            ActiveMagicEditor.Load();
        }

        public void Save(BinaryWriter BW)
        {
            ActiveMagicEditor.FinishEditing();
            ActiveSpell.Save(BW);
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            Thread.Sleep(33);

            MouseHelper.MouseStateCurrent = Mouse.GetState();
            KeyboardHelper.UpdateKeyboardStatus();

            ActiveMagicEditor.Update(new GameTime());

            KeyboardHelper.PlayerStateLast = Keyboard.GetState();
            MouseHelper.MouseStateLast = MouseHelper.MouseStateCurrent;

            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            g.Begin();

            ActiveMagicEditor.Draw(g);

            g.End();
        }
    }
}