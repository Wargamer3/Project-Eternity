using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class TileInformationPopupManagerDeathmatch : TileInformationPopupManager
    {
        protected DeathmatchMap Map;

        public TileInformationPopupManagerDeathmatch(DeathmatchMap Map, LayerHolder LayerHolder)
            : base(Map, LayerHolder)
        {
            this.Map = Map;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (TileInformationPopup ActivePopup in ListTilePopup)
            {
                ActivePopup.ListVisibleExtraText.Clear();

                foreach (Player ActivePlayer in Map.ListPlayer)
                {
                    foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                    {
                        if (ActiveSquad.IsDead)
                        {
                            continue;
                        }

                        if (ActiveSquad.Position.X == ActivePopup.CurrentTile.InternalPosition.X
                            && ActiveSquad.Position.Y == ActivePopup.CurrentTile.InternalPosition.Y)
                        {
                            ActivePopup.ListVisibleExtraText.AddRange(TextHelper.FitToWidth(TextHelper.fntShadowFont, ActiveSquad.CurrentLeader.ToString(), 120));
                        }
                    }
                }

                MapLayer ActiveLayer = Map.LayerManager.ListLayer[ActivePopup.CurrentTile.LayerIndex];

                foreach (InteractiveProp ActiveProp in ActiveLayer.ListProp)
                {
                    if (ActiveProp.Position.X == ActivePopup.CurrentTile.InternalPosition.X 
                        && ActiveProp.Position.Y == ActivePopup.CurrentTile.InternalPosition.Y)
                    {
                        ActivePopup.ListVisibleExtraText.AddRange(TextHelper.FitToWidth(TextHelper.fntShadowFont, ActiveProp.ToString(), 120));
                    }
                }
            }
        }
    }
}
