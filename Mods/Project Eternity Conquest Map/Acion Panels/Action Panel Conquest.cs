using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    /// <summary>
    /// Used to make menus.
    /// </summary>
    public abstract class ActionPanelConquest : BattleMapActionPanel
    {
        protected override PlayerInput ActiveInputManager => _ActiveInputManager;

        protected ConquestMap Map;
        private readonly PlayerInput _ActiveInputManager;

        public ActionPanelConquest(string Name, ConquestMap Map, bool CanCancel = true)
            : base(Name, Map.ListActionMenuChoice, CanCancel)
        {
            this.Map = Map;
            _ActiveInputManager = new KeyboardInput();

            Point MenuPosition = Map.LayerManager.LayerHolderDrawable.GetVisiblePosition(Map.CursorPosition);

            BaseMenuX = MenuPosition.X + Map.TileSize.X;
            BaseMenuY = MenuPosition.Y;

            UpdateFinalMenuPosition();
        }

        public ActionPanelConquest(string Name, ConquestMap Map, PlayerInput ActiveInputManager, bool CanCancel = true)
            : base(Name, Map.ListActionMenuChoice, CanCancel)
        {
            this.Map = Map;
            _ActiveInputManager = ActiveInputManager;

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
