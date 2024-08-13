using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.UI;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnitRecordsTabWhite : GameScreen
    {
        private Dictionary<string, uint> DicCharacterByKill;
        private Dictionary<string, uint> DicUnitByKill;

        private BoxScrollbar Scrollbar;

        private const int Interline = 27;
        float ScrollbarValue;

        private readonly BattleMapPlayer ActivePlayer;
        private readonly Roster PlayerRoster;
        private readonly int StartY;

        public UnitRecordsTabWhite(BattleMapPlayer ActivePlayer, Roster PlayerRoster, int StartY)
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


            if (PlayerRoster != null)
            {
                List<Character> ListCharacterByKill = PlayerRoster.TeamCharacters.ListAll.OrderByDescending(C => ActivePlayer.Records.PlayerUnitRecords.DicCharacterIDByNumberOfKills[C.ID]).ToList();
                List<Unit> ListUnitByKill = PlayerRoster.TeamUnits.ListAll.OrderByDescending(U => ActivePlayer.Records.PlayerUnitRecords.DicUnitIDByNumberOfKills[U.ID]).ToList();
            }
            else
            {
                DicCharacterByKill = ActivePlayer.Inventory.DicOwnedCharacter
                                        .GroupBy(C => C.Value.Pilot.ID, C =>
                                        {
                                            uint KillCount = 0; ActivePlayer.Records.PlayerUnitRecords.DicCharacterIDByNumberOfKills.TryGetValue(C.Value.Pilot.ID, out KillCount);
                                            return KillCount;
                                        })
                                        .OrderByDescending(U => U.First())
                                        .ToDictionary(C => C.Key, C => C.First());

                DicUnitByKill = ActivePlayer.Inventory.DicOwnedUnit
                                        .GroupBy(U => U.Value.Leader.ItemName, U =>
                                        {
                                            uint KillCount = 0; ActivePlayer.Records.PlayerUnitRecords.DicUnitIDByNumberOfKills.TryGetValue(U.Value.Leader.ID, out KillCount);
                                            return KillCount;
                                        })
                                        .OrderByDescending(U => U.First())
                                        .ToDictionary(U => U.Key, U => U.First());
            }

            TotalHeight += DicCharacterByKill.Count * Interline;
            TotalHeight += DicUnitByKill.Count * Interline;

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

            int RemainingCharacter = 5;
            foreach (KeyValuePair<string, uint> ActiveCharacterKill in DicCharacterByKill)
            {
                TextHelper.DrawText(g, ActiveCharacterKill.Key, new Vector2(10, LineY), Color.White);
                TextHelper.DrawTextRightAligned(g, ActiveCharacterKill.Value.ToString(), new Vector2(LineEndX + 5, LineY), Color.White);
                g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);
                LineY += Interline;
                if (--RemainingCharacter <= 0)
                {
                    break;
                }
            }

            LineY += 8;
            TextHelper.DrawText(g, "Unit kills", new Vector2(10, LineY), Color.Yellow);
            LineY += Interline + 8;

            int RemainingUnit = 5;
            foreach (KeyValuePair<string, uint> ActiveUnitKill in DicUnitByKill)
            {
                TextHelper.DrawText(g, ActiveUnitKill.Key, new Vector2(10, LineY), Color.White);
                TextHelper.DrawTextRightAligned(g, ActiveUnitKill.Value.ToString(), new Vector2(LineEndX + 5, LineY), Color.White);
                g.DrawLine(sprPixel, new Vector2(LineStartX, LineY + Interline - 3), new Vector2(LineEndX, LineY + Interline - 3), Color.YellowGreen);
                LineY += Interline;
                if (--RemainingUnit <= 0)
                {
                    break;
                }
            }
        }
    }
}
