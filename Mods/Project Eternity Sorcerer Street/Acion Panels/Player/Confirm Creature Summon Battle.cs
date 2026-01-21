using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelConfirmCreatureSummonBattle : ActionPanelSorcererStreet
    {
        private const string PanelName = "ConfirmCreatureSummonBattle";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private CreatureCard SelectedCard;
        private double AITimer;

        private int CursorIndex;

        public ActionPanelConfirmCreatureSummonBattle(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelConfirmCreatureSummonBattle(SorcererStreetMap Map, int ActivePlayerIndex, CreatureCard SelectedCard)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.SelectedCard = SelectedCard;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            CursorIndex = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!ActivePlayer.IsPlayerControlled)
            {
                AITimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (AITimer >= 1)
                {
                    FinishPhase();
                }
                return;
            }

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
            ActivePlayer.ListCardInHand.Remove(SelectedCard);
            ActivePlayer.Gold -= ActivePlayer.GetFinalCardCost(SelectedCard);

            RemoveAllActionPanels();
            AddToPanelListAndSelect(new ActionPanelBattleStartPhase(Map, ActivePlayerIndex, SelectedCard));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            string CardType = BR.ReadString();
            string CardPath = BR.ReadString();
            foreach (Card ActiveCard in ActivePlayer.ListCardInHand)
            {
                if (ActiveCard.CardType == CardType && ActiveCard.Path == CardPath)
                {
                    SelectedCard = (CreatureCard)ActiveCard;
                    break;
                }
            }

            Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature = SelectedCard;
            ActivePlayer.ListCardInHand.Remove(SelectedCard);
            ActivePlayer.Gold -= ActivePlayer.GetFinalCardCost(SelectedCard);
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendString(SelectedCard.CardType);
            BW.AppendString(SelectedCard.Path);
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
            SelectedCard.DrawCard(g);
            SelectedCard.DrawCardInfo(g, Map.Symbols, Map.fntMenuText, ActivePlayer, 0, 0);

            MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 2 - 150, Constants.Height - 120), 300, 90);
            g.DrawStringMiddleAligned(Map.fntMenuText, "Summon this creature?", new Vector2(Constants.Width / 2, Constants.Height - 110), Color.White);
            g.DrawStringMiddleAligned(Map.fntMenuText, "Yes", new Vector2(Constants.Width / 2, Constants.Height - 85), Color.White);
            g.DrawStringMiddleAligned(Map.fntMenuText, "No", new Vector2(Constants.Width / 2, Constants.Height - 60), Color.White);
            MenuHelper.DrawFingerIcon(g, new Vector2(Constants.Width / 2 - 100, Constants.Height - 95 + CursorIndex * 25));
        }
    }
}
