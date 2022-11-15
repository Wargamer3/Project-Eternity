using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelConfirmCreatureSummonBattle : ActionPanelSorcererStreet
    {
        private const string PanelName = "ConfirmCreatureSummonBattle";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private readonly CreatureCard ActiveCard;

        private int CursorIndex;

        public ActionPanelConfirmCreatureSummonBattle(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelConfirmCreatureSummonBattle(SorcererStreetMap Map, int ActivePlayerIndex, CreatureCard ActiveCard)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveCard = ActiveCard;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            CursorIndex = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                if (CursorIndex == 0)
                {
                    FinishPhase();
                }
                else if (CursorIndex == 1)
                {
                    RemoveFromPanelList(this);
                }
            }
            else if (InputHelper.InputUpPressed())
            {
                ++CursorIndex;
                if (CursorIndex > 1)
                    CursorIndex = 0;
            }
            else if (InputHelper.InputDownPressed())
            {
                --CursorIndex;
                if (CursorIndex < 0)
                    CursorIndex = 1;
            }
        }

        public void FinishPhase()
        {
            ActivePlayer.ListCardInHand.Remove(ActiveCard);
            ActivePlayer.Magic -= ActiveCard.MagicCost;

            RemoveAllActionPanels();
            Map.PushScreen(new ActionPanelBattleStartPhase(Map, ActivePlayerIndex, ActiveCard));
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
            return new ActionPanelConfirmCreatureSummonBattle(Map);
        }

        private void DrawIntro(CustomSpriteBatch g)
        {//Spin card from the left
        }

        private void DrawOutro(CustomSpriteBatch g)
        {//Spin card to its place in the hand
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActiveCard.DrawCard(g);
            ActiveCard.DrawCardInfo(g, Map.Symbols, Map.fntArial12, 0, 0);

            GameScreen.DrawBox(g, new Vector2(Constants.Width / 2 - 100, Constants.Height - 120), 200, 90, Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "Summon this creature?", new Vector2(Constants.Width / 2, Constants.Height - 110), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "Yes", new Vector2(Constants.Width / 2, Constants.Height - 85), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "No", new Vector2(Constants.Width / 2, Constants.Height - 60), Color.White);
            g.Draw(Map.sprMenuCursor, new Rectangle(Constants.Width / 2 - 60, Constants.Height - 85 + CursorIndex * 25, 40, 40), Color.White);
        }
    }
}
