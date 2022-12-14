using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class MultiplayerModeSelectionScreen : GameScreen
    {
        private SpriteFont fntArial12;

        private int CursorIndex;

        public MultiplayerModeSelectionScreen()
            : base()
        {
            this.RequireDrawFocus = true;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                --CursorIndex;
                if (CursorIndex < 0)
                    CursorIndex = BattleMap.DicBattmeMapType.Count - 1;
            }
            else if (InputHelper.InputDownPressed())
            {
                ++CursorIndex;
                if (CursorIndex >= BattleMap.DicBattmeMapType.Count)
                    CursorIndex = 0;
            }
            else if (InputHelper.InputConfirmPressed())
            {
                PushScreen(BattleMap.DicBattmeMapType.ElementAt(CursorIndex).Value.GetMultiplayerScreen());
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int i = 0;
            foreach(KeyValuePair<string, BattleMap> ActiveBattleMap in BattleMap.DicBattmeMapType)
            {
                g.DrawString(fntArial12, ActiveBattleMap.Key, new Vector2(0, i * 20), Color.White);
                ++i;
            }
            g.Draw(sprPixel, new Rectangle(0, CursorIndex * 20, 200, 20), Color.FromNonPremultiplied(255, 255, 255, 128));
        }
    }
}
