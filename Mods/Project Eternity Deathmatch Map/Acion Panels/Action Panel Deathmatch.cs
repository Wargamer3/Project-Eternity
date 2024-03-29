﻿using System;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class ActionPanelDeathmatch : BattleMapActionPanel
    {
        protected DeathmatchMap Map;

        public ActionPanelDeathmatch(string Name, DeathmatchMap Map, bool CanCancel = true)
            : base(Name, Map.ListActionMenuChoice, null, CanCancel)
        {
            this.Map = Map;

            if (Map.ListPlayer.Count > 0)
            {
                ActiveInputManager = Map.ListPlayer[Map.ActivePlayerIndex].InputManager;
            }

            if (!Map.IsServer && Map.IsInit)
            {
                Point MenuPosition = Map.LayerManager.LayerHolderDrawable.GetVisiblePosition(Map.CursorPosition);

                BaseMenuX = MenuPosition.X + Map.TileSize.X;
                BaseMenuY = MenuPosition.Y;

                UpdateFinalMenuPosition();
            }
        }

        public ActionPanelDeathmatch(string Name, DeathmatchMap Map, PlayerInput ActiveInputManager, bool CanCancel = true)
            : base(Name, Map.ListActionMenuChoice, ActiveInputManager, CanCancel)
        {
            this.Map = Map;

            if (!Map.IsServer && Map.IsInit)
            {
                Point MenuPosition = Map.LayerManager.LayerHolderDrawable.GetVisiblePosition(Map.CursorPosition);

                BaseMenuX = MenuPosition.X + Map.TileSize.X;
                BaseMenuY = MenuPosition.Y;

                UpdateFinalMenuPosition();
            }
        }

        protected override void OnCancelPanel()
        {
            Map.sndCancel.Play();
        }
    }
}
