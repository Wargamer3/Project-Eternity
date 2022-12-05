using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelConfirmCreatureSummon : ActionPanelSorcererStreet
    {
        private const string PanelName = "ConfirmCreatureSummon";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private CreatureCard SelectedCard;

        private int CursorIndex;

        public ActionPanelConfirmCreatureSummon(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelConfirmCreatureSummon(SorcererStreetMap Map, int ActivePlayerIndex, CreatureCard SelectedCard)
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
            if (InputHelper.InputConfirmPressed())
            {
                if (CursorIndex == 0)
                {
                    AddToPanelListAndSelect(new ActionPanelCreatureSummon(Map, ActivePlayerIndex, SelectedCard));
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

                Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
            }
            else if (InputHelper.InputDownPressed())
            {
                --CursorIndex;
                if (CursorIndex < 0)
                    CursorIndex = 1;

                Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
            }
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
        }

        public override void ExecuteUpdate(byte[] ArrayUpdateData)
        {
            CursorIndex = ArrayUpdateData[0];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendString(SelectedCard.CardType);
            BW.AppendString(SelectedCard.Path);
        }

        public override byte[] DoWriteUpdate()
        {
            return new byte[] { (byte)CursorIndex };
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelConfirmCreatureSummon(Map);
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
            SelectedCard.DrawCardInfo(g, Map.Symbols, Map.fntArial12, 0, 0);

            GameScreen.DrawBox(g, new Vector2(Constants.Width / 2 - 100, Constants.Height - 120), 200, 90, Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "Summon this creature?", new Vector2(Constants.Width / 2, Constants.Height - 110), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "Yes", new Vector2(Constants.Width / 2, Constants.Height - 85), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "No", new Vector2(Constants.Width / 2, Constants.Height - 60), Color.White);
            g.Draw(Map.sprMenuCursor, new Rectangle(Constants.Width / 2 - 60, Constants.Height - 85 + CursorIndex * 25, 40, 40), Color.White);
        }
    }
}
