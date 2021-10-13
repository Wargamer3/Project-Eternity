using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelFly : ActionPanelDeathmatch
    {
        private const string PanelName = "Fly";

        private Squad ActiveSquad;

        public ActionPanelFly(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelFly(DeathmatchMap Map, Squad ActiveSquad)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            ActiveSquad.CurrentMovement = UnitStats.TerrainAir;
            ActiveSquad.IsFlying = true;
            RemoveFromPanelList(this);
            ReplaceChoiceInCurrentPanel(new ActionPanelLand(Map, ActiveSquad), typeof(ActionPanelFly));
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
            return new ActionPanelFly(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
