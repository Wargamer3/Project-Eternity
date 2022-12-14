using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public partial class PowerSupply : Construction
    {
        public PowerSupply(Texture2D MenuIcon, AnimatedSprite Sprite, List<Texture2D> ListInteraction, int MaxHP)
            : base("Power supply", MenuIcon, Sprite, ListInteraction, MaxHP, Price: 100, BuildingTime: 1)
        {
        }

        public override Construction Copy()
        {
            return new PowerSupply(MenuIcon, SpriteMap, ListInteraction, MaxHP);
        }
        
        public override bool IsActionAvailable(Player Player, int InteractionIndex)
        {
            switch (InteractionIndex)
            {//Upgrade
                case 0:
                    if (Player.EnergyReserve >= UpgadeLevel * 200)
                        return true;
                    return false;
            }

            return false;
        }

        public override ActionPanelWorldMap GetSelectionPanel(WorldMap Map, int InteractionIndex)
        {
            switch (InteractionIndex)
            {
                case 0:
                    return new ActionPanelUpgradePowerSupply(Map, this);
            }

            return null;
        }

        public override void DrawExtraMenuInformation(CustomSpriteBatch g, WorldMap Map)
        {
        }
    }
}
