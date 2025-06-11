using System;
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
        private int MinimumDiceValue;
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
            MinimumDiceValue = Map.LowestDieRoll;
            MaximumDiceValue = Map.HighestDieRoll;
            int EnchantMin = ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).DiceValueMin;
            int EnchantMax = ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).DiceValueMax;

            if (EnchantMin >= 0)
            {
                MinimumDiceValue = EnchantMin;
            }

            if (EnchantMax >= 0)
            {
                MaximumDiceValue = EnchantMax;
            }

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
                VisibleDiceValue = Random.Next(MinimumDiceValue, MaximumDiceValue);
            }


            if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.D1) || KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.NumPad1))
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, 1));
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.D2) || KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.NumPad2))
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, 2));
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.D3) || KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.NumPad3))
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, 3));
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.D4) || KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.NumPad4))
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, 4));
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.D5) || KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.NumPad5))
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, 5));
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.D6) || KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.NumPad6))
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, 6));
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.D7) || KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.NumPad7))
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, 7));
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.D8) || KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.NumPad8))
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, 8));
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.D9) || KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.NumPad9))
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, 9));
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.D0) || KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.NumPad0))
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, 0));
            }
            else if (InputHelper.InputConfirmPressed())
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
            if (ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).Backward)
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, -VisibleDiceValue));
            }
            else
            {
                AddToPanelListAndSelect(new ActionPanelMovementPhase(Map, ActivePlayerIndex, VisibleDiceValue));
            }
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
