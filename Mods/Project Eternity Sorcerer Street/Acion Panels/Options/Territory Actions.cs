using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelTerritoryActions : ActionPanelSorcererStreet
    {
        private const string PanelName = "Territory Actions";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private TerrainSorcererStreet ActiveTerrain;

        public ActionPanelTerritoryActions(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelTerritoryActions(SorcererStreetMap Map, int ActivePlayerIndex, TerrainSorcererStreet ActiveTerrain)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            this.ActiveTerrain = ActiveTerrain;
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelChooseTerritory(Map, ActivePlayerIndex));
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                if (--ActionMenuCursor < 0)
                {
                    ActionMenuCursor = 4;
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (++ActionMenuCursor > 4)
                {
                    ActionMenuCursor = 0;
                }
            }
            else if (InputHelper.InputConfirmPressed())
            {
                switch (ActionMenuCursor)
                {
                    case 0:
                        AddToPanelListAndSelect(new ActionPanelTerrainLevelUpCommands(Map, ActivePlayerIndex, ActiveTerrain));
                        break;
                    case 1:
                        AddToPanelListAndSelect(new ActionPanelTerrainChange(Map, ActivePlayerIndex, ActiveTerrain));
                        break;
                    case 2:
                        AddToPanelListAndSelect(new ActionPanelCreatureMovement(Map));
                        break;
                    case 3:
                        AddToPanelListAndSelect(new ActionPanelCreatureExchange(Map));
                        break;
                    case 4:
                        RemoveFromPanelList(this);
                        break;
                }
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveFromPanelList(this);
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelTerritoryActions(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 2);
            float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth;
            float InfoBoxY = Constants.Height / 10;

            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY - 20), BoxWidth, 20, Color.White);
            g.DrawString(Map.fntMenuText, "Menu", new Vector2(InfoBoxX + 10, InfoBoxY - 20), Color.White);
            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight, Color.White);

            float CurrentX = InfoBoxX + 10;
            float CurrentY = InfoBoxY - 10;

            CurrentY += 20;
            g.DrawString(Map.fntMenuText, "What would you like to do?", new Vector2(CurrentX, CurrentY), Color.White);
            CurrentY += 20;
            g.DrawString(Map.fntMenuText, "Land Level Up", new Vector2(CurrentX + 10, CurrentY), Color.White);
            CurrentY += 20;
            g.DrawString(Map.fntMenuText, "Terrain Change", new Vector2(CurrentX + 10, CurrentY), Color.White);
            CurrentY += 20;
            g.DrawString(Map.fntMenuText, "Creature Movement", new Vector2(CurrentX + 10, CurrentY), Color.White);
            CurrentY += 20;
            g.DrawString(Map.fntMenuText, "Creature Exchange", new Vector2(CurrentX + 10, CurrentY), Color.White);
            CurrentY += 20;
            g.DrawString(Map.fntMenuText, "Return", new Vector2(CurrentX + 10, CurrentY), Color.White);

            MenuHelper.DrawFingerIcon(g, new Vector2(InfoBoxX - 20, InfoBoxY + 30 + 20 * ActionMenuCursor));

            ActionPanelPlayerDefault.DrawLandInformationTop(g, Map, ActiveTerrain);
        }
    }
}
