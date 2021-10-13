using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelRollDicePhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "Roll Dice";

        private int ActivePlayerIndex;
        private Player ActivePlayer;

        private readonly Random Random;
        private readonly Vector2 DicePosition;
        private int VisibleDiceValue;

        public ActionPanelRollDicePhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
            Random = new Random();
            DicePosition = new Vector2(Constants.Width / 2 - 20, Constants.Height / 2 - 25);
        }

        public ActionPanelRollDicePhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];

            Random = new Random();
            DicePosition = new Vector2(Constants.Width / 2 - 20, Constants.Height / 2 - 25);
        }

        public override void OnSelect()
        {
            VisibleDiceValue = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            VisibleDiceValue = Random.Next(0, 6);

            if (InputHelper.InputConfirmPressed())
            {
                RollDice();
            }
            else if (InputHelper.InputDownPressed())
            {
                RemoveFromPanelList(this);
                AddToPanelListAndSelect(new ActionPanelSpellCardSelectionPhase(Map, ActivePlayerIndex));
            }
        }

        public void RollDice()
        {
            VisibleDiceValue = Random.Next(0, 6);

            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, 3));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            VisibleDiceValue = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];

        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(VisibleDiceValue);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelRollDicePhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            GameScreen.DrawBox(g, new Vector2(DicePosition.X, DicePosition.Y), 40, 50, Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, VisibleDiceValue.ToString(), new Vector2(DicePosition.X + 20, DicePosition.Y + 15), Color.White);
            g.Draw(Map.sprArrowUp, new Vector2(Constants.Width / 2, Constants.Height - 20), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
        }
    }
}
