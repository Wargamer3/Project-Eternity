using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public partial class PowerSupply
    {
        public class ActionPanelUpgradePowerSupply : ActionPanelWorldMap
        {
            PowerSupply ActivePowerSupply;

            public ActionPanelUpgradePowerSupply(WorldMap Map, PowerSupply ActivePowerSupply)
                : base("Upgrade Power Supply", Map, false)
            {
                this.ActivePowerSupply = ActivePowerSupply;
            }

            public override void OnSelect()
            {
                ++ActivePowerSupply.UpgadeLevel;
                RemoveFromPanelList(this);
            }

            public override void DoUpdate(GameTime gameTime)
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
                return new ActionPanelUpgradePowerSupply(Map, null);
            }


            public override void Draw(CustomSpriteBatch g)
            {
            }
        }
    }
}
