﻿using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

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
        private int MaximumDiceValue;
        private int ForcedDiceValue;

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
            DicePosition = new Vector2(Constants.Width / 2, Constants.Height / 2);
        }

        public override void OnSelect()
        {
            VisibleDiceValue = 0;
            MaximumDiceValue = Map.HighestDieRoll;

            EnchantHelper.ActivateOnPlayer(Map.GlobalSorcererStreetBattleContext, ActivePlayer, null, null);
            ForcedDiceValue = ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).DiceValue;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ForcedDiceValue >= 0)
            {
                VisibleDiceValue = ForcedDiceValue;
            }
            else
            {
                VisibleDiceValue = Random.Next(0, MaximumDiceValue) + 1;
            }

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

        public override void UpdatePassive(GameTime gameTime)
        {
            if (ForcedDiceValue >= 0)
            {
                VisibleDiceValue = ForcedDiceValue;
            }
            else
            {
                VisibleDiceValue = Random.Next(0, MaximumDiceValue) + 1;
            }
        }

        public void RollDice()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, VisibleDiceValue));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            VisibleDiceValue = BR.ReadInt32();
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
            MenuHelper.DrawDiceHolder(g, DicePosition, VisibleDiceValue);

            MenuHelper.DrawDownArrow(g);
        }
    }
}
