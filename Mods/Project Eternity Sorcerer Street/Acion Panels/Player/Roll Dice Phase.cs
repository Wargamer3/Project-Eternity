using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelRollDicePhase : ActionPanelSorcererStreet
    {
        private readonly Player ActivePlayer;

        private readonly Random Random;
        private readonly Vector2 DicePosition;
        private int VisibleDiceValue;

        public ActionPanelRollDicePhase(SorcererStreetMap Map, Player ActivePlayer)
            : base("Roll Dice", Map, false)
        {
            this.ActivePlayer = ActivePlayer;

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
                AddToPanelListAndSelect(new ActionPanelSpellCardSelectionPhase(Map, ActivePlayer));
            }
        }

        public void RollDice()
        {
            VisibleDiceValue = Random.Next(0, 6);

            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayer, 3));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            GameScreen.DrawBox(g, new Vector2(DicePosition.X, DicePosition.Y), 40, 50, Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, VisibleDiceValue.ToString(), new Vector2(DicePosition.X + 20, DicePosition.Y + 15), Color.White);
            g.Draw(Map.sprArrowUp, new Vector2(Constants.Width / 2, Constants.Height - 20), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
        }
    }
}
