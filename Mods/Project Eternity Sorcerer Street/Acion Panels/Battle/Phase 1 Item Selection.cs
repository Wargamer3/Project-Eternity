using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleItemSelectionPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "BattleItemSelection";

        public ActionPanelBattleItemSelectionPhase(SorcererStreetMap Map)
            : base(PanelName, Map)
        {
            DrawDrawInfo = false;
        }

        public ActionPanelBattleItemSelectionPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, ListActionMenuChoice, Map, ActivePlayerIndex, "Item", "End")
        {
            DrawDrawInfo = false;
        }

        public override void OnCardSelected(Card CardSelected)
        {
        }

        public override void OnEndCardSelected()
        {
            RemoveFromPanelList(this);

            if (ActivePlayerIndex == Map.ActivePlayerIndex)
            {
                AddToPanelListAndSelect(new ActionPanelBattleItemSelectionPhase(ListActionMenuChoice, Map, Map.ListPlayer.IndexOf(Map.GlobalSorcererStreetBattleContext.DefenderPlayer)));
            }
            else
            {
                AddToPanelListAndSelect(new ActionPanelBattleLandModifierPhase(ListActionMenuChoice, Map, ActivePlayer.GamePiece));
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleItemSelectionPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);

            GameScreen.DrawBox(g, new Vector2(Constants.Width / 6, Constants.Height / 12), Constants.Width - Constants.Width / 3, 30, Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.ListPlayer[ActivePlayerIndex].Name + "'s item selection", new Vector2(Constants.Width - Constants.Width / 2, Constants.Height / 12 + 5), Color.White);

            int Y = Constants.Height / 4 - 25;
            TextHelper.DrawTextMiddleAligned(g, "BATTLE", new Vector2(Constants.Width / 2, Y), Color.White);
            Y = Constants.Height / 4;
            GameScreen.DrawBox(g, new Vector2(Constants.Width / 16, Y), Constants.Width - Constants.Width / 8, Constants.Height / 3, Color.White);
            Y = Constants.Height / 4 + 10;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderPlayer.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderPlayer.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "Invasion / Defense", new Vector2(Constants.Width - Constants.Width / 2, Y), Color.White);

            //Invader
            Y = Constants.Height / 4 + 35;
            g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 7, Y), new Vector2(Constants.Width - Constants.Width / 7, Y), Color.White);
            Y = Constants.Height / 4 + 40;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderCard.Name, new Vector2(Constants.Width / 4, Y), Color.White);
            int X = Constants.Width / 4;
            Y += 30;
            g.Draw(Map.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.MaxST.ToString(), new Vector2(X - 20, Y), Color.White);
            g.Draw(Map.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.MaxHP.ToString(), new Vector2(X + 45, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "Ability Values", new Vector2(Constants.Width / 2, Y), Color.White);

            Y += 25;
            g.DrawString(Map.fntArial12, "+0", new Vector2(X - 20, Y), Color.White);
            g.DrawString(Map.fntArial12, "+0", new Vector2(X + 45, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "Support / Land", new Vector2(Constants.Width / 2, Y), Color.White);
            Y += 25;
            g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 7, Y), new Vector2(Constants.Width - Constants.Width / 7, Y), Color.White);
            Y += 10;
            g.Draw(Map.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalST.ToString(), new Vector2(X - 20, Y), Color.White);
            g.Draw(Map.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalHP.ToString(), new Vector2(X + 45, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "Total", new Vector2(Constants.Width / 2, Y), Color.White);

            //Defender
            X = Constants.Width - Constants.Width / 4;
            Y = Constants.Height / 4 + 40;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderCard.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
            Y += 30;
            g.Draw(Map.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.MaxST.ToString(), new Vector2(X - 20, Y), Color.White);
            g.Draw(Map.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.MaxHP.ToString(), new Vector2(X + 45, Y), Color.White);

            Y += 25;
            g.DrawString(Map.fntArial12, "+0", new Vector2(X - 20, Y), Color.White);
            g.DrawString(Map.fntArial12, "+0", new Vector2(X + 45, Y), Color.White);
            Y += 25;
            g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 7, Y), new Vector2(Constants.Width - Constants.Width / 7, Y), Color.White);
            Y += 10;
            g.Draw(Map.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalST.ToString(), new Vector2(X - 20, Y), Color.White);
            g.Draw(Map.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalHP.ToString(), new Vector2(X + 45, Y), Color.White);
        }
    }
}
