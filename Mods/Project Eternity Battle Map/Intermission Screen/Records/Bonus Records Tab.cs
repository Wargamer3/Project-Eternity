using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BonusRecordsTab : GameScreen
    {
        private readonly BattleMapPlayer ActivePlayer;
        private readonly int StartY;

        public BonusRecordsTab(BattleMapPlayer ActivePlayer, int StartY)
            : base()
        {
            this.ActivePlayer = ActivePlayer;
            this.StartY = StartY;

            this.RequireDrawFocus = true;
        }

        public override void Load()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int LineStartX = 90;
            int LineEndX = Constants.Width - 15;
            int Interline = 27;
            float LineY = StartY;

            foreach (KeyValuePair<string, uint> ActiveBonus in ActivePlayer.Records.PlayerBonusRecords.DicNumberOfBonusObtainedByName)
            {
                TextHelper.DrawText(g, ActiveBonus.Key, new Vector2(10, LineY), Color.White);
                TextHelper.DrawTextRightAligned(g, ActiveBonus.Value.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
                g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);
                LineY += Interline;
            }
        }
    }
}
