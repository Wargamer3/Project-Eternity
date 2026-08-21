using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelSelectCharacterToEdit : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "SelectCharacterToEdit";

        public ActionPanelSelectCharacterToEdit(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            foreach (PlayerCharacter ActiveCharacter in Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.ListCharacter)
            {
                AddToPanelListAndSelect(new ActionPanelEditCharacter(Owner, ActiveCharacter, MapManager, ListActionMenuChoice));
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
            return new ActionPanelSelectCharacterToEdit(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
