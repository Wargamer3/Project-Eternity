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
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackMAPDirection : ActionPanelDeathmatch
    {
        private const string PanelName = "AttackMAPDirection";

        private Squad ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Attack CurrentAttack;
        public List<Vector3> ListAttackChoice;
        public List<MovementAlgorithmTile> ListAttackTerrain;

        public ActionPanelAttackMAPDirection(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAPDirection(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            ListAttackChoice = new List<Vector3>();
            ListAttackTerrain = new List<MovementAlgorithmTile>();

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
                Map.SelectMAPEnemies(ActivePlayerIndex, ActiveSquadIndex, ListAttackChoice);
                Map.sndConfirm.Play();

            }

            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
        }

        private void AimUp()
        {
            Map.CursorPosition.X = ActiveSquad.X;
            Map.CursorPosition.Y = ActiveSquad.Y - 1;

            ListAttackChoice.Clear();
            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalUp(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackChoice.Add(TargetPosition);
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition.X, TargetPosition.Y, (int)TargetPosition.Z));
                }
            }
        }

        private void AimDown()
        {
            Map.CursorPosition.X = ActiveSquad.X;
            Map.CursorPosition.Y = ActiveSquad.Y + 1;

            ListAttackChoice.Clear();
            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalDown(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackChoice.Add(TargetPosition);
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition.X, TargetPosition.Y, (int)TargetPosition.Z));
                }
            }
        }

        private void AimLeft()
        {
            Map.CursorPosition.X = ActiveSquad.X - 1;
            Map.CursorPosition.Y = ActiveSquad.Y;

            ListAttackChoice.Clear();
            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalLeft(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackChoice.Add(TargetPosition);
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition.X, TargetPosition.Y, (int)TargetPosition.Z));
                }
            }
        }

        private void AimRight()
        {
            Map.CursorPosition.X = ActiveSquad.X + 1;
            Map.CursorPosition.Y = ActiveSquad.Y;

            ListAttackChoice.Clear();
            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalRight(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackChoice.Add(TargetPosition);
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition.X, TargetPosition.Y, (int)TargetPosition.Z));
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            string ActiveSquadAttackName = BR.ReadString();
            if (!string.IsNullOrEmpty(ActiveSquadAttackName))
            {
                foreach (Attack ActiveAttack in ActiveSquad.CurrentLeader.ListAttack)
                {
                    if (ActiveAttack.ItemName == ActiveSquadAttackName)
                    {
                        ActiveSquad.CurrentLeader.CurrentAttack = ActiveAttack;
                        break;
                    }
                }
            }
            int AttackChoiceCount = BR.ReadInt32();
            ListAttackChoice = new List<Vector3>(AttackChoiceCount);
            ListAttackTerrain = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                Vector3 NewTerrain = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadInt32());
                ListAttackChoice.Add(NewTerrain);
                ListAttackTerrain.Add(Map.GetTerrain(NewTerrain.X, NewTerrain.Y, (int)NewTerrain.Z));
            }

            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
            Map.BattleMenuOffenseFormationChoice = FormationChoices.ALL;
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendString(ActiveSquad.CurrentLeader.ItemName);
            BW.AppendInt32(ListAttackChoice.Count);

            for (int A = 0; A < ListAttackChoice.Count; ++A)
            {
                BW.AppendFloat(ListAttackChoice[A].X);
                BW.AppendFloat(ListAttackChoice[A].Y);
                BW.AppendInt32((int)ListAttackChoice[A].Z);
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
