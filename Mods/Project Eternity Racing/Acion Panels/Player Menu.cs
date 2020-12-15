﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.RacingScreen
{
    public class ActionPanelPlayerAction : ActionPanelRacing
    {
        public ActionPanelPlayerAction(Vehicule ActiveVehicule)
            : base("Player Action", ActiveVehicule, false)
        {
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (KeyboardHelper.KeyHold(Keys.Down) && KeyboardHelper.KeyHold(Keys.Right))
            {
                ActiveVehicule.PrepareBoost(gameTime);
            }
            else if (KeyboardHelper.KeyHold(Keys.Down))
            {
                ActiveVehicule.Accelerate(gameTime);
            }
            else if (KeyboardHelper.KeyHold(Keys.Right))
            {
                ActiveVehicule.Decelerate(gameTime);
                ActiveVehicule.BoostPower = 0;
            }
            if (KeyboardHelper.KeyHold(Keys.A))
            {
                ActiveVehicule.Turn(gameTime, - 1);
            }
            if (KeyboardHelper.KeyHold(Keys.D))
            {
                ActiveVehicule.Turn(gameTime, 1);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
