using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Parts;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public class ActionPanelConsumableParts : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;
        private readonly ActionPanel Owner;
        private bool IsInit;

        public ActionPanelConsumableParts(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad)
            : base("Parts", Map)
        {
            this.Owner = Owner;
            this.ActiveSquad = ActiveSquad;

            IsInit = false;
        }

        public override void OnSelect()
        {
            for (int P = 0; P < ActiveSquad.CurrentLeader.ArrayParts.Length; ++P)
            {
                if (ActiveSquad.CurrentLeader.ArrayParts[P] != null && ActiveSquad.CurrentLeader.ArrayParts[P].PartType == PartTypes.Consumable)
                {
                    Owner.AddChoiceToCurrentPanel(this);
                    return;
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!IsInit)
            {
                IsInit = true;
                Map.PushScreen(new ConsumablePartMenu(Map, ActiveSquad));
            }

            //Player has exited the menu.
            if (!Map.SpiritMenu.Alive)
            {
                Map.CursorPosition = ActiveSquad.Position;
                CancelPanel();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
