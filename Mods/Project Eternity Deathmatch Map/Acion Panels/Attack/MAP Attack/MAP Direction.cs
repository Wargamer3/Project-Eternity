using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackMAPDirection : ActionPanelDeathmatch
    {
        private const string PanelName = "AttackMAPDirection";

        private Squad ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Attack CurrentAttack;
        public List<Vector3> AttackChoice;

        public ActionPanelAttackMAPDirection(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAPDirection(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            AttackChoice = new List<Vector3>();

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
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
                Map.SelectMAPEnemies(ActivePlayerIndex, ActiveSquadIndex, AttackChoice);
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

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            ActiveSquad.CurrentLeader.AttackIndex = BR.ReadInt32();
            int AttackChoiceCount = BR.ReadInt32();
            AttackChoice = new List<Vector3>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                AttackChoice.Add(new Vector3(BR.ReadFloat(), BR.ReadFloat(), 0f));
            }

            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
            Map.BattleMenuOffenseFormationChoice = FormationChoices.ALL;
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendInt32(ActiveSquad.CurrentLeader.AttackIndex);
            BW.AppendInt32(AttackChoice.Count);

            for (int A = 0; A < AttackChoice.Count; ++A)
            {
                BW.AppendFloat(AttackChoice[A].X);
                BW.AppendFloat(AttackChoice[A].Y);
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
