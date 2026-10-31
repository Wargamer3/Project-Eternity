using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelAdminMenu : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "AdminMenu";

        public ActionPanelAdminMenu(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            ListNextChoice.Clear();
            AddChoiceToCurrentPanel(new ActionPanelSelectCharacterToSpawn(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelSelectCharacterToEdit(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelSelectCharacterToControl(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelSpawnBlocks(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelWater(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelTerrain(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelPropBrush(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelGenerateMap(Owner, MapManager, ListActionMenuChoice, 0));
            if (WorldStream.Slot != null && WorldStream.Slot.Seed != 0)
            {
                AddChoiceToCurrentPanel(new ActionPanelGenerateMap(Owner, MapManager, ListActionMenuChoice, WorldStream.Slot.Seed));
            }
            AddChoiceToCurrentPanel(new ActionPanelPlaceDungeonGate(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelGlobalWeather(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelAssignCharacter(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelDeleteSelection(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelBuildingZone(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelDeleteBuildingZone(Owner, MapManager, ListActionMenuChoice));
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Owner.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                Owner.Session.CloseAdmin();
                RemoveFromPanelList(this);
                Owner.EndKeys();
                return;
            }

            if (NavigateThroughNextChoices(MapManager.sndSelection))
            {
            }
            else if (ConfirmNextChoices(MapManager.sndConfirm))
            {
            }

            Owner.EndKeys();
        }

        protected override void OnCancelPanel()
        {
            Owner.Session.CloseAdmin();
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAdminMenu(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            TextHelper.DrawText(g, "Admin Menu", new Vector2(40, 88), Color.Red);
            DrawNextChoice(g);
        }
    }
}
