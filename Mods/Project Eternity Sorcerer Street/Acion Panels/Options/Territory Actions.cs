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
        private bool HasTerritoryAbility;
        private bool AllTerritory;

        public ActionPanelTerritoryActions(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelTerritoryActions(SorcererStreetMap Map, int ActivePlayerIndex, TerrainSorcererStreet ActiveTerrain, bool AllTerritory)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            this.ActiveTerrain = ActiveTerrain;
            this.AllTerritory = AllTerritory;
        }

        public override void OnSelect()
        {
            if (ActiveTerrain.DefendingCreature != null && ActiveTerrain.DefendingCreature.TerritoryAbility != null)
            {
                HasTerritoryAbility = true;
            }
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
                        if (ActiveTerrain.DefendingCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).LandLevelLock)
                        {
                        }
                        else
                        {
                            AddToPanelListAndSelect(new ActionPanelTerrainLevelUpCommands(Map, ActivePlayerIndex, ActiveTerrain));
                        }
                        break;
                    case 1:
                        if (ActiveTerrain.DefendingCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).IsDefensive)
                        {
                        }
                        else
                        {
                            AddToPanelListAndSelect(new ActionPanelTerrainChange(Map, ActivePlayerIndex, ActiveTerrain));
                        }
                        break;
                    case 2:
                        AddToPanelListAndSelect(new ActionPanelCreatureMovement(Map, ActivePlayerIndex, ActiveTerrain));
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
            float Ratio = Constants.Height / 720f;

            int BoxWidth = (int)(456 * Ratio);
            int BoxHeight = (int)(210 * Ratio);
            int InfoBoxX = 1075;
            int InfoBoxY = 108;

            int IconWidth = (int)(32 * Ratio);
            int IconHeight = (int)(32 * Ratio);
            int LineHeight = (int)(33 * Ratio);

            MenuHelper.DrawNamedBox(g, "Menu", new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight);

            int CurrentX = InfoBoxX + (int)(40 * Ratio);
            int CurrentY = InfoBoxY + (int)(10 * Ratio);

            g.DrawString(Map.fntMenuText, "What would you like to do?", new Vector2(CurrentX, CurrentY), Color.White);
            CurrentX += 50;
            CurrentY += LineHeight;
            if (ActiveTerrain.DefendingCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).LandLevelLock)
            {
                g.DrawString(Map.fntMenuText, "Land Level Up", new Vector2(CurrentX, CurrentY), Color.Gray);
                g.Draw(IconHolder.Icons.sprMenuLimitLand, new Rectangle(CurrentX + (int)(180 * Ratio), CurrentY, IconWidth, IconHeight), Color.White);
            }
            else
            {
                g.DrawString(Map.fntMenuText, "Land Level Up", new Vector2(CurrentX, CurrentY), Color.White);
            }
            CurrentY += LineHeight;
            g.DrawString(Map.fntMenuText, "Terrain Change", new Vector2(CurrentX, CurrentY), Color.White);
            CurrentY += LineHeight;
            g.DrawString(Map.fntMenuText, "Creature Movement", new Vector2(CurrentX, CurrentY), Color.White);
            if (ActiveTerrain.DefendingCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).IsDefensive)
            {
                g.DrawString(Map.fntMenuText, "X", new Vector2(CurrentX + 150, CurrentY), Color.White);
            }
            CurrentY += LineHeight;
            g.DrawString(Map.fntMenuText, "Creature Exchange", new Vector2(CurrentX + 10, CurrentY), Color.White);

            if (HasTerritoryAbility)
            {
                CurrentY += LineHeight;
                g.DrawString(Map.fntMenuText, "Creature Ability", new Vector2(CurrentX + 10, CurrentY), Color.White);
            }

            CurrentY += LineHeight;
            g.DrawString(Map.fntMenuText, "Return", new Vector2(CurrentX + 10, CurrentY), Color.White);

            MenuHelper.DrawFingerIcon(g, new Vector2(InfoBoxX, InfoBoxY + (int)(50 * Ratio) + LineHeight * ActionMenuCursor));

            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer);
            ActionPanelPlayerDefault.DrawLandInformationBottom(g, Map, ActiveTerrain);
        }
    }
}
