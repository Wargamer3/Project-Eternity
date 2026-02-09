using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAttackMAPSpread : ActionPanelConquest
    {
        private const string PanelName = "AttackMAPSpread";

        private UnitConquest ActiveUnit;
        private int ActivePlayerIndex;
        private int ActiveUnitIndex;
        private List<Vector3> ListMVHoverPoints;
        private Attack CurrentAttack;
        public List<Vector3> ListAttackChoice;
        public List<MovementAlgorithmTile> ListAttackTerrain;

        public ActionPanelAttackMAPSpread(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAPSpread(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
            CurrentAttack = ActiveUnit.CurrentAttack;
        }

        public override void OnSelect()
        {
            ListAttackChoice = new List<Vector3>();
            ListAttackTerrain = new List<MovementAlgorithmTile>();
            for (int X = 0; X < CurrentAttack.MAPAttributes.ListChoice.Count; X++)
            {
                for (int Y = 0; Y < CurrentAttack.MAPAttributes.ListChoice[X].Count; Y++)
                {
                    if (CurrentAttack.MAPAttributes.ListChoice[X][Y])
                    {
                        Vector3 NewPosition = new Vector3(Map.CursorPosition.X +X - CurrentAttack.MAPAttributes.Width,
                                               Map.CursorPosition.Y + Y - CurrentAttack.MAPAttributes.Height, Map.CursorPosition.Z);
                        ListAttackChoice.Add(NewPosition);
                        ListAttackTerrain.Add(Map.GetTerrain(NewPosition));
                    }
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveInputManager.InputConfirmPressed())
            {
                if (ListAttackChoice.Contains(Map.CursorPosition))
                {
                    ActionPanelUseMAPAttack.SelectMAPEnemies(Map, ActivePlayerIndex, ActiveUnitIndex, ListMVHoverPoints, ListAttackTerrain);
                    Map.sndConfirm.Play();
                }
                else
                {
                    Map.sndDeny.Play();
                }
            }
            else
            {
                Map.CursorControl(gameTime, ActiveInputManager);//Move the cursor
            }

            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
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
            ListAttackChoice = new List<Vector3>(AttackChoiceCount);
            ListAttackTerrain = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                Vector3 NewTerrain = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadInt32());
                ListAttackChoice.Add(NewTerrain);
                ListAttackTerrain.Add(Map.GetTerrain(NewTerrain));
            }

            CurrentAttack = ActiveUnit.CurrentAttack;
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveUnitIndex);
            BW.AppendString(ActiveUnit.ItemName);
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
            return new ActionPanelAttackMAPSpread(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
