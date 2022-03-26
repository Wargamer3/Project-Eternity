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
        private List<Vector3> ListMVHoverPoints;
        private Attack CurrentAttack;
        public List<MovementAlgorithmTile> ListAttackTerrain;

        public ActionPanelAttackMAPDirection(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAPDirection(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
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
                Map.SelectMAPEnemies(ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints, ListAttackTerrain);
                Map.sndConfirm.Play();

            }

            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
        }

        private void AimUp()
        {
            Map.CursorPosition.X = ActiveSquad.X;
            Map.CursorPosition.Y = ActiveSquad.Y - 1;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalUp(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition.X, TargetPosition.Y, (int)TargetPosition.Z));
                }
            }
        }

        private void AimDown()
        {
            Map.CursorPosition.X = ActiveSquad.X;
            Map.CursorPosition.Y = ActiveSquad.Y + 1;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalDown(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition.X, TargetPosition.Y, (int)TargetPosition.Z));
                }
            }
        }

        private void AimLeft()
        {
            Map.CursorPosition.X = ActiveSquad.X - 1;
            Map.CursorPosition.Y = ActiveSquad.Y;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalLeft(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition.X, TargetPosition.Y, (int)TargetPosition.Z));
                }
            }
        }

        private void AimRight()
        {
            Map.CursorPosition.X = ActiveSquad.X + 1;
            Map.CursorPosition.Y = ActiveSquad.Y;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in CurrentAttack.MAPAttributes.GetTargetsDirectionalRight(ActiveSquad.Position))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
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
            ListAttackTerrain = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                ListAttackTerrain.Add(Map.GetTerrain(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32()));
            }

            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
            Map.BattleMenuOffenseFormationChoice = FormationChoices.ALL;
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendString(ActiveSquad.CurrentLeader.ItemName);
            BW.AppendInt32(ListAttackTerrain.Count);

            for (int A = 0; A < ListAttackTerrain.Count; ++A)
            {
                BW.AppendInt32((int)ListAttackTerrain[A].InternalPosition.X);
                BW.AppendInt32((int)ListAttackTerrain[A].InternalPosition.Y);
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
