using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    /// <summary>
    /// Only used by the player, will never update the input manager once it's set
    /// </summary>
    public abstract class ActionPanelLifeSimPlayer : BattleMapActionPanel
    {
        protected override PlayerInput ActiveInputManager => Owner.ActiveInputManager;

        protected readonly NavMapGameManager MapManager;

        protected readonly PlayerOverseer Owner;

        public ActionPanelLifeSimPlayer(string Name, PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, bool CanCancel = true)
            : base(Name, ListActionMenuChoice, CanCancel)
        {
            this.MapManager = MapManager;
            this.Owner = Owner;

            Point MenuPosition = Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.LayerManager.LayerHolderDrawable.GetVisiblePosition(Owner.InvisibleCharacterAsCursor.Position);

            BaseMenuX = MenuPosition.X;
            BaseMenuY = MenuPosition.Y;

            UpdateFinalMenuPosition();
        }

        protected override void OnCancelPanel()
        {
            MapManager.sndCancel.Play();
        }
    }
    public abstract class ActionPanelLifeSimCharacter : BattleMapActionPanel
    {
        protected override PlayerInput ActiveInputManager => Owner.SharedMapContex.User.ActiveInputManager;

        protected readonly NavMapGameManager MapManager;

        protected readonly PlayerCharacter Owner;

        public ActionPanelLifeSimCharacter(string Name, PlayerCharacter Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, bool CanCancel = true)
            : base(Name, ListActionMenuChoice, CanCancel)
        {
            this.Owner = Owner;
            this.MapManager = MapManager;
        }

        protected override void OnCancelPanel()
        {
            MapManager.sndCancel.Play();
        }
    }
}
