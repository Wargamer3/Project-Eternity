using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class PlayerRecordsTabWhite : GameScreen
    {
        private readonly BattleMapPlayer ActivePlayer;
        private readonly int StartY;

        public PlayerRecordsTabWhite(BattleMapPlayer ActivePlayer, int StartY)
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

            TextHelper.DrawText(g, "Player time", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.TotalSecondsPlayed.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "All time turn passed", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.TotalTurnPlayed.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "All time kills", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.TotalKills.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "All time tile traveled", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.TotalTilesTraveled.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "Current money", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.CurrentMoney.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "All time money", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.TotalMoney.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "Current coins", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.CurrentCoins.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "All time money", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.TotalMoney.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);
        }
    }
}
