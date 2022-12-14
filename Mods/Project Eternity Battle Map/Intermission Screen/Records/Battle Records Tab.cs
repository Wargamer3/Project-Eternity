using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleRecordsTab : GameScreen
    {
        private readonly BattleMapPlayer ActivePlayer;
        private readonly int StartY;

        public BattleRecordsTab(BattleMapPlayer ActivePlayer, int StartY)
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

            TextHelper.DrawText(g, "Number Of Games Played", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.PlayerBattleRecords.NumberOfGamesPlayed.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "Number Of Games Won", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.PlayerBattleRecords.NumberOfGamesWon.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "Number Of Games Lost", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.PlayerBattleRecords.NumberOfGamesLost.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "Number Of Kills", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.PlayerBattleRecords.NumberOfKills.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "Number Of Units Lost", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.PlayerBattleRecords.NumberOfUnitsLost.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "Total Damage Given", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.PlayerBattleRecords.TotalDamageGiven.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "Total Damage Received", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.PlayerBattleRecords.TotalDamageReceived.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);

            LineY += Interline;
            TextHelper.DrawText(g, "Total Damage Recovered", new Vector2(10, LineY), Color.White);
            TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.PlayerBattleRecords.TotalDamageRecovered.ToString(), new Vector2(Constants.Width - 10, LineY), Color.White);
            g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);
        }
    }
}
