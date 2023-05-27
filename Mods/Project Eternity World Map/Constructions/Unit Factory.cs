using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public partial class UnitFactory : Construction
    {
        public List<Unit> ListSpawnUnit;
        public Vector3 Waypoint;

        public UnitFactory(Texture2D MenuIcon, AnimatedSprite Sprite, List<Texture2D> ListInteraction, List<Unit> ListSpawnUnit, int MaxHP)
            : base("Unit factory", MenuIcon, Sprite, ListInteraction, MaxHP, Price: 100, BuildingTime: 1)
        {
            this.ListSpawnUnit = ListSpawnUnit;
            Waypoint = new Vector3(-1, -1, -1);
            MapSize = new Point(2, 2);
        }

        public override Construction Copy()
        {
            return new UnitFactory(MenuIcon, SpriteMap, ListInteraction, ListSpawnUnit, MaxHP);
        }

        public override bool IsActionAvailable(Player Player, int InteractionIndex)
        {
            switch (InteractionIndex)
            {//Upgrade
                case 0:
                    if (Player.EnergyReserve >= UpgadeLevel * 200)
                        return true;
                    return false;

                //Build Unit
                case 1:
                    if (Player.EnergyReserve >= UpgadeLevel * 200)
                        return true;
                    return false;

                //Set Waypoint
                case 2:
                    return true;
            }

            return false;
        }
        
        public override void DrawExtraMenuInformation(CustomSpriteBatch g, WorldMap Map)
        {
            if (Waypoint.X != -1)
                g.Draw(Map.sprWaypoint, new Vector2((Waypoint.X - Map.Camera2DPosition.X) * Map.TileSize.X, (Waypoint.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y), Color.White);
        }

        public override ActionPanelWorldMap GetSelectionPanel(WorldMap Map, int InteractionIndex)
        {
            switch (InteractionIndex)
            {
                //Upgrade.
                case 0:
                    return new ActionPanelUpgrade(Map, this);

                #region Spawn Ninja

                case 1:
                    return new ActionPanelSpawnNinja(Map, this);

                #endregion

                case 2:
                    return new ActionPanelSetWaypoint(Map, this);
            }

            return null;
        }
    }
}
