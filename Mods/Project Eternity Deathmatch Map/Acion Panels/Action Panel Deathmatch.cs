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
            : base(Name, Map.ListActionMenuChoice, CanCancel)
        {
            this.Map = Map;

            MenuX = (int)((Map.CursorPosition.X - Map.CameraPosition.X + 1) * Map.TileSize.X);
            MenuY = (int)((Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y);

            if (MenuX + MinActionMenuWidth >= Constants.Width)
                MenuX = Constants.Width - MinActionMenuWidth;

            int MenuHeight = ListNextChoice.Count * PannelHeight + 6 * 2;
            if (MenuY + MenuHeight >= Constants.Height)
                MenuY = Constants.Height - MenuHeight;
        }

        protected override void OnCancelPanel()
        {
            Map.sndCancel.Play();
        }
    }
}
