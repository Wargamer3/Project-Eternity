using System;
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

            BaseMenuX = (int)((Map.CursorPosition.X - Map.CameraPosition.X + 1) * Map.TileSize.X);
            BaseMenuY = (int)((Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y);

            if (BaseMenuX + MinActionMenuWidth >= Constants.Width)
                BaseMenuX = Constants.Width - MinActionMenuWidth;

            int MenuHeight = ListNextChoice.Count * PannelHeight + 6 * 2;
            if (BaseMenuY + MenuHeight >= Constants.Height)
                BaseMenuY = Constants.Height - MenuHeight;

            UpdateFinalMenuPosition();
        }

        protected override void OnCancelPanel()
        {
            Map.sndCancel.Play();
        }
    }
}
