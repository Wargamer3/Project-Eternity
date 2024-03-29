﻿using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelEndPlayerPhase : ActionPanelSorcererStreet
    {
        public ActionPanelEndPlayerPhase(SorcererStreetMap Map)
            : base("End", Map, false)
        {
        }

        public override void OnSelect()
        {
            Map.EndPlayerPhase();
        }

        public override void DoUpdate(GameTime gameTime)
        {
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

        protected override ActionPanel Copy()
        {
            return new ActionPanelEndPlayerPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override string ToString()
        {
            return "Ends your turn.";
        }
    }
}
