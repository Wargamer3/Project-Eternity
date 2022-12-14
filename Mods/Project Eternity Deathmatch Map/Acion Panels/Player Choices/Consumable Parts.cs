using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Parts;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public class ActionPanelConsumableParts : ActionPanelDeathmatch
    {
        private const string PanelName = "Parts";

        private Squad ActiveSquad;
        private readonly ActionPanel Owner;
        private bool IsInit;

        public ActionPanelConsumableParts(DeathmatchMap Map)
            : base(PanelName, Map)
        {
            IsInit = false;
        }

        public ActionPanelConsumableParts(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad)
            : base(PanelName, Map)
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

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelConsumableParts(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
