using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    /// <summary>
    /// Used to make menus.
    /// </summary>
    public abstract class ActionPanelDeathmatch : BattleMapActionPanel
    {
        protected DeathmatchMap Map;

        public ActionPanelDeathmatch(string Name, DeathmatchMap Map, bool CanCancel = true)
            : this(Name, Map, Map.ListPlayer[Map.ActivePlayerIndex].InputManager, CanCancel)
        {
        }

        public ActionPanelDeathmatch(string Name, DeathmatchMap Map, PlayerInput ActiveInputManager, bool CanCancel = true)
            : base(Name, Map.ListActionMenuChoice, ActiveInputManager, CanCancel)
        {
            this.Map = Map;

            Point MenuPosition = Map.LayerManager.LayerHolderDrawable.GetMenuPosition();

            BaseMenuX = MenuPosition.X;
            BaseMenuY = MenuPosition.Y;

            UpdateFinalMenuPosition();
        }

        protected override void OnCancelPanel()
        {
            Map.sndCancel.Play();
        }
    }
}
