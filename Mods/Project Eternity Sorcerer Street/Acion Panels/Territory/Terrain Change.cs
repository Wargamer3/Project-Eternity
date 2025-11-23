using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelTerrainChange : ActionPanelSorcererStreet
    {
        private const string PanelName = "Terrain Change";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private TerrainSorcererStreet ActiveTerrain;

        private string ChangeCost;

        public ActionPanelTerrainChange(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelTerrainChange(SorcererStreetMap Map, int ActivePlayerIndex, TerrainSorcererStreet ActiveTerrain)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            this.ActiveTerrain = ActiveTerrain;
        }

        public override void OnSelect()
        {
            ChangeCost = GetChangeCost().ToString();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                ActiveTerrain.UpdateValue(Map.DicTeam[ActivePlayer.TeamIndex].DicCreatureCountByElementType[ActiveTerrain.TerrainTypeIndex], ActiveTerrain.DefendingCreature);
                Map.EndPlayerPhase();
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

        private int GetChangeCost()
        {
            switch (Map.TerrainHolder.ListTerrainType[ActiveTerrain.TerrainTypeIndex])
            {
                case TerrainSorcererStreet.MultiElement:
                case TerrainSorcererStreet.NeutralElement:
                    return ActiveTerrain.LandLevel * 100;

                case TerrainSorcererStreet.FireElement:
                case TerrainSorcererStreet.WaterElement:
                case TerrainSorcererStreet.EarthElement:
                case TerrainSorcererStreet.AirElement:
                    return ActiveTerrain.LandLevel * 100 + 200;
            }

            return 0;
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelTerrainChange(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActionPanelPlayerDefault.DrawLandInformationTop(g, Map, ActiveTerrain);

            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = 137;
            float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth;
            float InfoBoxY = Constants.Height / 10;

            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY - 20), BoxWidth, 20, Color.White);
            g.DrawString(Map.fntMenuText, "Menu", new Vector2(InfoBoxX + 10, InfoBoxY - 20), SorcererStreetMap.TextColor);
            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight, Color.White);

            float CurrentX = InfoBoxX + 10;
            float CurrentY = InfoBoxY - 10;

            CurrentY += 20;
            g.DrawString(Map.fntMenuText, "Make it what terrain?", new Vector2(CurrentX, CurrentY), SorcererStreetMap.TextColor);

            CurrentY += 20;
            g.Draw(Map.Symbols.sprElementFire, new Vector2((int)InfoBoxX + 20, (int)CurrentY), Color.White);
            g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)CurrentX + 130, (int)CurrentY, 18, 18), Color.White);
            g.DrawStringRightAligned(Map.fntMenuText, ChangeCost, new Vector2(CurrentX + 190, CurrentY), Color.White);

            CurrentY += 20;
            g.Draw(Map.Symbols.sprElementWater, new Vector2((int)InfoBoxX + 20, (int)CurrentY), Color.White);
            g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)CurrentX + 130, (int)CurrentY, 18, 18), Color.White);
            g.DrawStringRightAligned(Map.fntMenuText, ChangeCost, new Vector2(CurrentX + 190, CurrentY), Color.White);

            CurrentY += 20;
            g.Draw(Map.Symbols.sprElementEarth, new Vector2((int)InfoBoxX + 20, (int)CurrentY), Color.White);
            g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)CurrentX + 130, (int)CurrentY, 18, 18), Color.White);
            g.DrawStringRightAligned(Map.fntMenuText, ChangeCost, new Vector2(CurrentX + 190, CurrentY), Color.White);

            CurrentY += 20;
            g.Draw(Map.Symbols.sprElementAir, new Vector2((int)InfoBoxX + 20, (int)CurrentY), Color.White);
            g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)CurrentX + 130, (int)CurrentY, 18, 18), Color.White);
            g.DrawStringRightAligned(Map.fntMenuText, ChangeCost, new Vector2(CurrentX + 190, CurrentY), Color.White);

            CurrentY += 20;
            g.DrawString(Map.fntMenuText, "Return", new Vector2(CurrentX + 10, CurrentY), SorcererStreetMap.TextColor);
        }
    }
}
