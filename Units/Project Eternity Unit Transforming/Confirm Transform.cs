using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Units.Transforming
{
    public class ActionPanelConfirmTransform : ActionPanelDeathmatch
    {
        private readonly UnitTransforming TransformingUnit;
        private readonly int TransformationChoice;
        private readonly Squad ActiveSquad;

        public ActionPanelConfirmTransform(UnitTransforming TransformingUnit, int TransformationChoice, Squad ActiveSquad, DeathmatchMap Map, bool CanTransform)
            : base(TransformingUnit.ArrayTransformingUnit[TransformationChoice].UnitTransformed.ItemName, Map)
        {
            this.TransformingUnit = TransformingUnit;
            this.TransformationChoice = TransformationChoice;
            this.ActiveSquad = ActiveSquad;

            IsEnabled = CanTransform;
        }

        public override void OnSelect()
        {
            TransformingUnit.ChangeUnit(TransformationChoice);
            Map.UpdateSquadCurrentMovement(ActiveSquad);
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
