using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    /// <summary>
    /// Used to make menus.
    /// </summary>
    public abstract class ActionPanelConquest : BattleMapActionPanel
    {
        protected ConquestMap Map;

        public ActionPanelConquest(string Name, ConquestMap Map, bool CanCancel = true)
            : base(Name, Map.ListActionMenuChoice, CanCancel)
        {
            this.Map = Map;

            MenuX = (int)(Map.CursorPosition.X + 1 - Map.CameraPosition.X) * Map.TileSize.X;
            MenuY = (int)(Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

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
