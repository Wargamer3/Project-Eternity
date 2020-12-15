using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackMAPDirection : ActionPanelDeathmatch
    {
        private readonly Squad ActiveSquad;
        private int ActivePlayerIndex;
        private readonly Attack CurrentAttack;
        public List<Vector3> AttackChoice;

        public ActionPanelAttackMAPDirection(DeathmatchMap Map, Squad ActiveSquad, int ActivePlayerIndex)
            : base("Attack MAP Direction", Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.ActivePlayerIndex = ActivePlayerIndex;
            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
            AttackChoice = new List<Vector3>();
        }

        public override void OnSelect()
        {
            Map.BattleMenuOffenseFormationChoice = FormationChoices.ALL;
            AimDown();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            #region Directions

            if (InputHelper.InputUpPressed())
            {
                AimUp();
            }
            else if (InputHelper.InputDownPressed())
            {
                AimDown();
            }
            else if (InputHelper.InputLeftPressed())
            {
                AimLeft();
            }
            else if (InputHelper.InputRightPressed())
            {
                AimRight();
            }

            #endregion

            else if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                Map.SelectMAPEnemies(ActiveSquad, ActivePlayerIndex, AttackChoice);
                Map.sndConfirm.Play();

            }

            Map.ListLayer[Map.ActiveLayerIndex].LayerGrid.AddDrawablePoints(AttackChoice, Color.FromNonPremultiplied(255, 0, 0, 190));
        }

        private void AimUp()
        {
            Map.CursorPosition.X = ActiveSquad.X;
            Map.CursorPosition.Y = ActiveSquad.Y - 1;

            AttackChoice.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalUp(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                    AttackChoice.Add(TargetPosition);
            }
        }

        private void AimDown()
        {
            Map.CursorPosition.X = ActiveSquad.X;
            Map.CursorPosition.Y = ActiveSquad.Y + 1;

            AttackChoice.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalDown(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                    AttackChoice.Add(TargetPosition);
            }
        }

        private void AimLeft()
        {
            Map.CursorPosition.X = ActiveSquad.X - 1;
            Map.CursorPosition.Y = ActiveSquad.Y;

            AttackChoice.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalLeft(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                    AttackChoice.Add(TargetPosition);
            }
        }

        private void AimRight()
        {
            Map.CursorPosition.X = ActiveSquad.X + 1;
            Map.CursorPosition.Y = ActiveSquad.Y;

            AttackChoice.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalRight(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                    AttackChoice.Add(TargetPosition);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
