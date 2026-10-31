using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelSelectCharacterToControl : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "SelectCharacterToControl";

        public ActionPanelSelectCharacterToControl(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            MapInfo Here = Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
            if (Here == null || Here.People == null)
            {
                return;
            }

            foreach (PlayerCharacter ActiveCharacter in Here.People)
            {
                AddChoiceToCurrentPanel(new ActionPanelTakeCharacter(Owner, MapManager, ListActionMenuChoice, ActiveCharacter));
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (NavigateThroughNextChoices(MapManager.sndSelection))
            {
            }
            else if (ConfirmNextChoices(MapManager.sndConfirm))
            {
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelSelectCharacterToControl(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
