using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.ControlHelper
{
    public static class CheatCodeHelper
    {
        public static List<Keys> ListInput = new List<Keys>();
        public static List<Keys> KonamiCode = new List<Keys>() { Keys.Up, Keys.Up, Keys.Down, Keys.Down, Keys.Left, Keys.Right, Keys.Left, Keys.Right, Keys.B, Keys.A };
        private static long TimeSinceLastInput;

        public static void Update(GameTime gameTime)
        {
            TimeSinceLastInput += gameTime.ElapsedGameTime.Milliseconds;

            if (KeyboardHelper.PlayerState.GetPressedKeys().Length > 0)
            {
                ListInput.AddRange(KeyboardHelper.PlayerState.GetPressedKeys());
                TimeSinceLastInput = 0;
            }
            if (TimeSinceLastInput > 1000)
            {
                ListInput.Clear();
            }
        }
    }
}
