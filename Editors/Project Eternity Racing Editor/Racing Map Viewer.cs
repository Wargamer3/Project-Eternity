using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.RacingScreen;

namespace ProjectEternity.Editors.RacingMapEditor
{
    public class RacingMapViewerControl : Scene3DViewerControl
    {
        public RacingMap ActiveMap;

        public RacingMapViewerControl()
        {
            DoUpdate = Update;
            DoDraw = Draw;
        }

        protected void Update(GameTime gameTime)
        {
            KeyboardHelper.UpdateKeyboardStatus();
            ActiveMap.Update(gameTime);
            KeyboardHelper.PlayerStateLast = Keyboard.GetState();
        }

        protected void Draw(CustomSpriteBatch g)
        {
            ActiveMap.DrawMap(g);
        }
    }
}
