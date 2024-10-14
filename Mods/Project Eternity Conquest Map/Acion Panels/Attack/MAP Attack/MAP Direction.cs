using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAttackMAPDirection : ActionPanelConquest
    {
        private const string PanelName = "AttackMAPDirection";

        private UnitConquest ActiveUnit;
        private int ActivePlayerIndex;
        private int ActiveUnitIndex;
        private List<Vector3> ListMVHoverPoints;
        private Attack CurrentAttack;
        public List<MovementAlgorithmTile> ListAttackTerrain;

        public ActionPanelAttackMAPDirection(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAPDirection(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
            ListAttackTerrain = new List<MovementAlgorithmTile>();

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
            CurrentAttack = ActiveUnit.CurrentAttack;
        }

        public override void OnSelect()
        {
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
                ActionPanelUseMAPAttack.SelectMAPEnemies(Map, ActivePlayerIndex, ActiveUnitIndex, ListMVHoverPoints, ListAttackTerrain);
                Map.sndConfirm.Play();

            }

            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
        }

        private void AimUp()
        {
            Map.CursorPosition.X = ActiveUnit.X;
            Map.CursorPosition.Y = ActiveUnit.Y - 1;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalUp(ActiveUnit.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition));
                }
            }
        }

        private void AimDown()
        {
            Map.CursorPosition.X = ActiveUnit.X;
            Map.CursorPosition.Y = ActiveUnit.Y + 1;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalDown(ActiveUnit.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition));
                }
            }
        }

        private void AimLeft()
        {
            Map.CursorPosition.X = ActiveUnit.X - 1;
            Map.CursorPosition.Y = ActiveUnit.Y;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalLeft(ActiveUnit.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition));
                }
            }
        }

        private void AimRight()
        {
            Map.CursorPosition.X = ActiveUnit.X + 1;
            Map.CursorPosition.Y = ActiveUnit.Y;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalRight(ActiveUnit.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition));
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveUnitIndex = BR.ReadInt32();
            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
            string ActiveUnitAttackName = BR.ReadString();
            if (!string.IsNullOrEmpty(ActiveUnitAttackName))
            {
                foreach (Attack ActiveAttack in ActiveUnit.ListAttack)
                {
                    if (ActiveAttack.ItemName == ActiveUnitAttackName)
                    {
                        ActiveUnit.CurrentAttack = ActiveAttack;
                        break;
                    }
                }
            }
            int AttackChoiceCount = BR.ReadInt32();
            ListAttackTerrain = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                ListAttackTerrain.Add(Map.GetTerrain(new Vector3(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32())));
            }

            CurrentAttack = ActiveUnit.CurrentAttack;
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveUnitIndex);
            BW.AppendString(ActiveUnit.ItemName);
            BW.AppendInt32(ListAttackTerrain.Count);

            for (int A = 0; A < ListAttackTerrain.Count; ++A)
            {
                BW.AppendInt32((int)ListAttackTerrain[A].GridPosition.X);
                BW.AppendInt32((int)ListAttackTerrain[A].GridPosition.Y);
                BW.AppendInt32(ListAttackTerrain[A].LayerIndex);
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAttackMAPDirection(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
