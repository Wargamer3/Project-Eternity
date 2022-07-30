using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnitRecordsTab : GameScreen
    {
        private List<Character> ListCharacterByKill;
        private List<Unit> ListUnitByKill;

        private BoxScrollbar Scrollbar;

        private const int Interline = 27;
        float ScrollbarValue;

        private readonly BattleMapPlayer ActivePlayer;
        private readonly Roster PlayerRoster;
        private readonly int StartY;

        public UnitRecordsTab(BattleMapPlayer ActivePlayer, Roster PlayerRoster, int StartY)
            : base()
        {
            this.ActivePlayer = ActivePlayer;
            this.PlayerRoster = PlayerRoster;
            this.StartY = StartY;

            this.RequireDrawFocus = true;
        }

        public override void Load()
        {
            int TotalHeight = 0;

            ListCharacterByKill = PlayerRoster.TeamCharacters.ListAll.OrderByDescending(C => ActivePlayer.Records.PlayerUnitRecords.DicCharacterIDByNumberOfKills[C.ID]).ToList();
            ListUnitByKill = PlayerRoster.TeamUnits.ListAll.OrderByDescending(U => ActivePlayer.Records.PlayerUnitRecords.DicUnitIDByNumberOfKills[U.ID]).ToList();

            TotalHeight += ListCharacterByKill.Count * Interline;
            TotalHeight += ListUnitByKill.Count * Interline;

            Scrollbar = new BoxScrollbar(new Vector2(Constants.Width - 20, StartY), Constants.Height - StartY, TotalHeight, OnScrollbarChange);
        }

        public override void Update(GameTime gameTime)
        {
            Scrollbar.Update(gameTime);
        }

        private void OnScrollbarChange(float ScrollbarValue)
        {
            this.ScrollbarValue = ScrollbarValue;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            Scrollbar.Draw(g);
            int LineStartX = 90;
            int LineEndX = Constants.Width - 30;
            int Interline = 27;
            float LineY = StartY;

            TextHelper.DrawText(g, "Character kills", new Vector2(10, LineY), Color.Yellow);
            LineY += Interline + 8;

            for (int C = 0; C < 5 && C < ListCharacterByKill.Count; ++C)
            {
                TextHelper.DrawText(g, ListCharacterByKill[C].Name, new Vector2(10, LineY), Color.White);
                TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.PlayerUnitRecords.DicCharacterIDByNumberOfKills[ListCharacterByKill[C].ID].ToString(), new Vector2(LineEndX + 5, LineY), Color.White);
                g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);
                LineY += Interline;
            }

            LineY += 8;
            TextHelper.DrawText(g, "Unit kills", new Vector2(10, LineY), Color.Yellow);
            LineY += Interline + 8;

            for (int U = 0; U < 5 && U < ListUnitByKill.Count; ++U)
            {
                TextHelper.DrawText(g, ListUnitByKill[U].ItemName, new Vector2(10, LineY), Color.White);
                TextHelper.DrawTextRightAligned(g, ActivePlayer.Records.PlayerUnitRecords.DicUnitIDByNumberOfKills[ListUnitByKill[U].ID].ToString(), new Vector2(LineEndX + 5, LineY), Color.White);
                g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);
                LineY += Interline;
            }
        }
    }
}
