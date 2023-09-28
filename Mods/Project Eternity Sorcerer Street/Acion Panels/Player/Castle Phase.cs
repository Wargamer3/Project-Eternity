using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelCastlePhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "CastlePhase";

        private enum Commands { Territory, Map, Info, Options, Help, End }

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private int MovementRemaining;

        private double ItemAnimationTime;

        private int BasicBonus;
        private int TerritoryBonus;
        private int SymbolBonus;
        private int Fluctuation;
        private int Total;

        public ActionPanelCastlePhase(SorcererStreetMap Map)
                : base(PanelName, Map, false)
        {
        }

        public ActionPanelCastlePhase(SorcererStreetMap Map, int ActivePlayerIndex, int MovementRemaining)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.MovementRemaining = MovementRemaining;

            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            ++ActivePlayer.CompletedLaps;
            BasicBonus = Map.MagicGainPerLap + Map.MagicGainPerLap / 10 * ActivePlayer.CompletedLaps;
            int NumberOfLandPossessed = 0;
            foreach (byte NumberOfLand in ActivePlayer.DicCreatureCountByElementType.Values)
            {
                NumberOfLandPossessed += NumberOfLand;
            }
            TerritoryBonus = NumberOfLandPossessed * 20;
            SymbolBonus = 0;
            Fluctuation = 0;
            Total = BasicBonus + TerritoryBonus + SymbolBonus + Fluctuation;
            ActivePlayer.Gold += Total;
            ActivePlayer.TotalMagic += Total;
            Map.UpdatePlayersRank();
            /*Symbol Bonus = Value of symbols owned / 10G
            (One is only awarded the symbol bonus if they own the most symbols of a particular color in an area)*/
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ItemAnimationTime < 1)
            {
                ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (ItemAnimationTime < 5)
            {
                ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                RemoveFromPanelList(this);

                if (MovementRemaining == 0)
                {
                    AddToPanelListAndSelect(new ActionPanelTerritoryMenuPhase(Map, ActivePlayerIndex));
                }
            }

            if (InputHelper.InputConfirmPressed())
            {
                RemoveFromPanelList(this);

                if (MovementRemaining == 0)
                {
                    AddToPanelListAndSelect(new ActionPanelTerritoryMenuPhase(Map, ActivePlayerIndex));
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelCastlePhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            MenuHelper.DrawDiceHolder(g, new Vector2(Constants.Width / 8, Constants.Height / 4), MovementRemaining);

            if (ItemAnimationTime < 1)
            {
                int Size = Constants.Width / 4;
                int PosX = Constants.Width / 2 - Size / 2;
                int PosY = Constants.Height / 2 - Size / 2;
            }
            else
            {
                int BoxWidth = (int)(Constants.Width / 2);
                int BoxHeight = (int)(Constants.Height / 2);
                float InfoBoxX = Constants.Width / 2 - BoxWidth / 2;
                float InfoBoxY = Constants.Height / 4;

                GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY - 20), BoxWidth, 20, Color.White);
                g.DrawString(Map.fntArial12, "Information", new Vector2(InfoBoxX + 10, InfoBoxY - 20), Color.White);
                GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight, Color.White);

                float CurrentX = InfoBoxX + 10;
                float CurrentY = InfoBoxY - 10;

                CurrentY += 20;

                g.DrawStringMiddleAligned(Map.fntArial12, "Lap #" + ActivePlayer.CompletedLaps + " Bonus", new Vector2(Constants.Width / 2, CurrentY), Color.White);
                CurrentY += 20;

                g.DrawString(Map.fntArial12, "Basic Bonus", new Vector2(CurrentX, CurrentY), Color.White);
                g.DrawStringRightAligned(Map.fntArial12, BasicBonus + "G", new Vector2(CurrentX + BoxWidth - 20, CurrentY), Color.White);
                CurrentY += 20;

                g.DrawString(Map.fntArial12, "Territory Bonus", new Vector2(CurrentX, CurrentY), Color.White);
                g.DrawStringRightAligned(Map.fntArial12, TerritoryBonus + "G", new Vector2(CurrentX + BoxWidth - 20, CurrentY), Color.White);
                CurrentY += 20;

                g.DrawString(Map.fntArial12, "Symbol Bonus", new Vector2(CurrentX, CurrentY), Color.White);
                g.DrawStringRightAligned(Map.fntArial12, SymbolBonus + "G", new Vector2(CurrentX + BoxWidth - 20, CurrentY), Color.White);
                CurrentY += 20;

                g.DrawString(Map.fntArial12, "Fluctuation", new Vector2(CurrentX, CurrentY), Color.White);
                g.DrawStringRightAligned(Map.fntArial12, Fluctuation + "G", new Vector2(CurrentX + BoxWidth - 20, CurrentY), Color.White);
                CurrentY += 25;
                g.DrawLine(GameScreen.sprPixel, new Vector2(CurrentX, CurrentY - 3), new Vector2(CurrentX + BoxWidth - 20, CurrentY -3), Color.White);

                g.DrawString(Map.fntArial12, "Total", new Vector2(CurrentX, CurrentY), Color.White);
                g.DrawStringRightAligned(Map.fntArial12, Total + "G", new Vector2(CurrentX + BoxWidth - 20, CurrentY), Color.White);
                CurrentY += 40;

                g.DrawString(Map.fntArial12, ActivePlayer.Name + "'s creatures", new Vector2(CurrentX, CurrentY), Color.White);
                CurrentY += 20;
                g.DrawString(Map.fntArial12, "recovered 20% of MHP.", new Vector2(CurrentX, CurrentY), Color.White);

                MenuHelper.DrawConfirmIcon(g, new Vector2(InfoBoxX + BoxWidth - 20, InfoBoxY + BoxHeight - 35));
            }
        }
    }
}
