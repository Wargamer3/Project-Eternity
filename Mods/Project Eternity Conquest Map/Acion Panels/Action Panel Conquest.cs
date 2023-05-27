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
            : base(Name, Map.ListActionMenuChoice, new KeyboardInput(), CanCancel)
        {
            this.Map = Map;

            BaseMenuX = (int)(Map.CursorPosition.X + 1 - Map.Camera2DPosition.X) * Map.TileSize.X;
            BaseMenuY = (int)(Map.CursorPosition.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y;

            if (BaseMenuX + MinActionMenuWidth >= Constants.Width)
                BaseMenuX = Constants.Width - MinActionMenuWidth;

            int MenuHeight = ListNextChoice.Count * PannelHeight + 6 * 2;
            if (BaseMenuY + MenuHeight >= Constants.Height)
                BaseMenuY = Constants.Height - MenuHeight;
        }

        protected override void OnCancelPanel()
        {
            Map.sndCancel.Play();
        }
    }
}
