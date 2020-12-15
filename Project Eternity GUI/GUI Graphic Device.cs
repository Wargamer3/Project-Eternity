using System;
using ProjectEternity.Core.Editor;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GUI
{
    class GUIGraphicDevice : GraphicsDeviceControl
    {
        public ContentManager Content;

        public GUIGraphicDevice()
        {
            Content = new ContentManager(Services, "Content");
            OnCreateControl();
        }

        protected override void Draw()
        {
            throw new NotImplementedException();
        }

        protected override void Initialize()
        {
        }
    }
}
