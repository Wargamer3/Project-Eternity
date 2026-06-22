using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelEditCharacter : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "EditCharacter";

        private readonly PlayerCharacter ControlledCharacter;

        public ActionPanelEditCharacter(PlayerOverseer Owner, PlayerCharacter ControlledCharacter, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, true)
        {
            this.ControlledCharacter = ControlledCharacter;
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelEditCharacterInventory(Owner, ControlledCharacter, MapManager, ListActionMenuChoice));
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
            return new ActionPanelEditCharacter(Owner, ControlledCharacter, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
