using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelAttackMAPDirection : ActionPanelSorcererStreet
    {
        private const string PanelName = "AttackMAPDirection";

        private TerrainSorcererStreet ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        public MAPAttackAttributes MAPAttributes;
        private List<Vector3> ListMVHoverPoints;
        public List<MovementAlgorithmTile> ListAttackTerrain;

        public ActionPanelAttackMAPDirection(SorcererStreetMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAPDirection(SorcererStreetMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
            ListAttackTerrain = new List<MovementAlgorithmTile>();

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSummonedCreature[ActiveSquadIndex];
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
                Map.SelectMAPEnemies(ActivePlayerIndex, ActiveSquadIndex, MAPAttributes, ListMVHoverPoints, ListAttackTerrain);
                Map.sndConfirm.Play();

            }

            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
        }

        private void AimUp()
        {
            Map.CursorPosition.X = ActiveSquad.WorldPosition.X;
            Map.CursorPosition.Y = ActiveSquad.WorldPosition.Y - 1;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in MAPAttributes.GetTargetsDirectionalUp(ActiveSquad.WorldPosition))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition));
                }
            }
        }

        private void AimDown()
        {
            Map.CursorPosition.X = ActiveSquad.WorldPosition.X;
            Map.CursorPosition.Y = ActiveSquad.WorldPosition.Y + 1;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in MAPAttributes.GetTargetsDirectionalDown(ActiveSquad.WorldPosition))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition));
                }
            }
        }

        private void AimLeft()
        {
            Map.CursorPosition.X = ActiveSquad.WorldPosition.X - 1;
            Map.CursorPosition.Y = ActiveSquad.WorldPosition.Y;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in MAPAttributes.GetTargetsDirectionalLeft(ActiveSquad.WorldPosition))
            {
                if (TargetPosition.X >= 0f && TargetPosition.X < Map.MapSize.X && TargetPosition.Y >= 0f && TargetPosition.Y < Map.MapSize.Y)
                {
                    ListAttackTerrain.Add(Map.GetTerrain(TargetPosition));
                }
            }
        }

        private void AimRight()
        {
            Map.CursorPosition.X = ActiveSquad.WorldPosition.X + 1;
            Map.CursorPosition.Y = ActiveSquad.WorldPosition.Y;

            ListAttackTerrain.Clear();
            foreach (Vector3 TargetPosition in MAPAttributes.GetTargetsDirectionalRight(ActiveSquad.WorldPosition))
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
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSummonedCreature[ActiveSquadIndex];

            int AttackChoiceCount = BR.ReadInt32();
            ListAttackTerrain = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                ListAttackTerrain.Add(Map.GetTerrain(new Vector3(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32())));
            }
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
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
