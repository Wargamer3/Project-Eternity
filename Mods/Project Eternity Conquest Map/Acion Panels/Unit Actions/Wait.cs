﻿using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelWait : ActionPanelConquest
    {
        private UnitConquest ActiveUnit;

        public ActionPanelWait(ConquestMap Map, UnitConquest ActiveUnit)
            : base("Wait", Map)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
            Map.FinalizeMovement(ActiveUnit, 0, new System.Collections.Generic.List<Vector3>());
            ActiveUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
            RemoveAllSubActionPanels();
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
            return new ActionPanelWait(Map, null);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
