using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBankruptPhase : ActionPanelSorcererStreet
    {
        public const string CastleReached = "Castle Reached";
        private const string PanelName = "BankruptPhase";

        private int ActivePlayerIndex;
        private Player ActivePlayer;

        private double ItemAnimationTime;

        private int BasicBonus;
        private int TerritoryBonus;
        private int SymbolBonus;
        private int Fluctuation;
        private int Total;

        public ActionPanelBankruptPhase(SorcererStreetMap Map)
                : base(PanelName, Map, false)
        {
        }

        public ActionPanelBankruptPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;

            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            AddToPanelListAndSelect(new ActionPanelBankruptPopup(Map, ActivePlayerIndex));
            BasicBonus = Map.MagicGainPerLap + Map.MagicGainPerLap / 10 * ActivePlayer.CompletedLaps;

            int NumberOfLandPossessed = 0;
            TerritoryBonus = NumberOfLandPossessed * 20;
            SymbolBonus = 0;
            Fluctuation = 0;
            Total = BasicBonus + TerritoryBonus + SymbolBonus + Fluctuation;

            Total = (int)(Total * ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).CastleValueMultiplier);

            ActivePlayer.Gold = Total;
            ActivePlayer.ListPassedCheckpoint.Clear();
            Map.UpdateTotalMagic();
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
                Map.EndPlayerPhase();
            }

            if (InputHelper.InputConfirmPressed())
            {
                Map.EndPlayerPhase();
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
            return new ActionPanelBankruptPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
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

                float Ratio = Constants.Height / 720f;
                MenuHelper.DrawNamedBox(g, "Information", new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight);

                float CurrentX = (int)(InfoBoxX + 30 * Ratio);
                float CurrentY = InfoBoxY - 10;

                CurrentY += (int)(20 * Ratio);

                g.DrawStringMiddleAligned(Map.fntMenuText, "Lap #" + ActivePlayer.CompletedLaps + " Bonus", new Vector2(Constants.Width / 2, CurrentY), SorcererStreetMap.TextColor);
                CurrentY += (int)(20 * Ratio);

                g.DrawString(Map.fntMenuText, "Basic Bonus", new Vector2(CurrentX, CurrentY), SorcererStreetMap.TextColor);
                g.DrawStringRightAligned(Map.fntMenuText, BasicBonus + "G", new Vector2((int)(CurrentX + BoxWidth - 60 * Ratio), CurrentY), SorcererStreetMap.TextColor);
                CurrentY += (int)(30 * Ratio);

                g.DrawString(Map.fntMenuText, "Territory Bonus", new Vector2(CurrentX, CurrentY), SorcererStreetMap.TextColor);
                g.DrawStringRightAligned(Map.fntMenuText, TerritoryBonus + "G", new Vector2((int)(CurrentX + BoxWidth - 60 * Ratio), CurrentY), SorcererStreetMap.TextColor);
                CurrentY += (int)(30 * Ratio);

                g.DrawString(Map.fntMenuText, "Symbol Bonus", new Vector2(CurrentX, CurrentY), SorcererStreetMap.TextColor);
                g.DrawStringRightAligned(Map.fntMenuText, SymbolBonus + "G", new Vector2((int)(CurrentX + BoxWidth - 60 * Ratio), CurrentY), SorcererStreetMap.TextColor);
                CurrentY += (int)(30 * Ratio);

                g.DrawString(Map.fntMenuText, "Fluctuation", new Vector2(CurrentX, CurrentY), SorcererStreetMap.TextColor);
                g.DrawStringRightAligned(Map.fntMenuText, Fluctuation + "G", new Vector2((int)(CurrentX + BoxWidth - 60 * Ratio), CurrentY), SorcererStreetMap.TextColor);
                CurrentY += (int)(35 * Ratio);
                g.DrawLine(GameScreen.sprPixel, new Vector2(CurrentX, CurrentY), new Vector2((int)(CurrentX + BoxWidth - 60 * Ratio), CurrentY), SorcererStreetMap.TextColor);
                CurrentY += (int)(5 * Ratio);
                g.DrawString(Map.fntMenuText, "Total", new Vector2(CurrentX, CurrentY), SorcererStreetMap.TextColor);
                g.DrawStringRightAligned(Map.fntMenuText, Total + "G", new Vector2((int)(CurrentX + BoxWidth - 60 * Ratio), CurrentY), SorcererStreetMap.TextColor);
                CurrentY += (int)(60 * Ratio);

                g.DrawString(Map.fntMenuText, ActivePlayer.Name + "'s creatures", new Vector2(CurrentX, CurrentY), SorcererStreetMap.TextColor);
                CurrentY += (int)(30 * Ratio);
                g.DrawString(Map.fntMenuText, "recovered 20% of MHP.", new Vector2(CurrentX, CurrentY), SorcererStreetMap.TextColor);

                MenuHelper.DrawConfirmIcon(g, new Vector2((int)(InfoBoxX + BoxWidth - 40 * Ratio), (int)(InfoBoxY + BoxHeight - 50 * Ratio)));
            }
        }
    }
}
