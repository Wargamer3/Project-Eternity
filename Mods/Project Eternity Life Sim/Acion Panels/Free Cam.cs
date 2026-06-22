using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelFreeCam : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "FreeCam";

        private readonly List<MapInfo> ListVisisbleMap;
        private readonly Camera3D Camera;

        private PlayerCharacter HoverCharacter;

        public ActionPanelFreeCam(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, false)
        {
            Camera = new LifeSimFreeCamera(GameScreen.GraphicsDevice, Owner);
            ListVisisbleMap = new List<MapInfo>();
        }

        public override void OnSelect()
        {
            ListVisisbleMap.Add(MapManager.RootMapContainer);
            Owner.Camera = Camera;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            PlayerCharacter FoundCharacter = Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.MoveCursorAndGetCharacterUnderMouse();

            if (FoundCharacter != null)
            {
                HoverCharacter = FoundCharacter;
            }

            if (HoverCharacter != null && ActiveInputManager.InputConfirmPressed())
            {
                HoverCharacter.SharedMapContex.User = Owner;

                RemoveAllActionPanels();
                AddToPanelListAndSelect(new ActionPanelControlCharacter(Owner, HoverCharacter, MapManager, ListActionMenuChoice));
            }
            else if (HoverCharacter != null && ActiveInputManager.InputCancelPressed())
            {
                AddToPanelListAndSelect(new ActionPanelViewCharacterLogs(Owner, HoverCharacter, MapManager, ListActionMenuChoice));
                HoverCharacter = null;
            }
            else if (ActiveInputManager.InputLButtonPressed())//switch to next character
            {
                PlayerCharacter CharacterToSelect = Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.ListCharacter[0];
                Owner.ActiveControlledCharacter.RemoveControl();
                CharacterToSelect.TakeControl(Owner);
            }
            else if (ActiveInputManager.InputConfirmPressed())
            {
                AddToPanelListAndSelect(new ActionPanelAdminMenu(Owner, MapManager, ListActionMenuChoice));
            }

            Camera.Update(gameTime);
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
            return new ActionPanelFreeCam(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            foreach (MapInfo ActiveMap in ListVisisbleMap)
            {
                ActiveMap.MapMapContainer.ActiveMap.CursorPosition = new Vector3(Owner.InvisibleCharacterAsCursor.X, Owner.InvisibleCharacterAsCursor.Z, Owner.InvisibleCharacterAsCursor.Y);
                ActiveMap.MapMapContainer.ActiveMap.LayerManager.LayerHolderDrawable.SetCamera(Owner.Camera);
                ActiveMap.MapMapContainer.ActiveMap.Draw(g);
            }
        }
    }
}
