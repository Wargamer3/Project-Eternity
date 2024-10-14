using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using System;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelUseMAPAttack : ActionPanelConquest
    {
        private const string PanelName = "UseMapAttack";

        private UnitConquest ActiveUnit;
        private int ActivePlayerIndex;
        private int ActiveUnitIndex;
        private List<Vector3> ListMVHoverPoints;
        private Attack CurrentAttack;
        public List<MovementAlgorithmTile> ListAttackChoice;

        public ActionPanelUseMAPAttack(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelUseMAPAttack(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex, List<Vector3> ListMVHoverPoints, List<MovementAlgorithmTile> AttackChoice)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
            this.ListAttackChoice = AttackChoice;

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
            CurrentAttack = ActiveUnit.CurrentAttack;
        }

        public override void OnSelect()
        {
        }

        public static void SelectMAPEnemies(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex, List<Vector3> ListMVHoverPoints, List<MovementAlgorithmTile> AttackChoice)
        {
            UnitConquest ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];

            if (ActiveUnit.CurrentAttack.MAPAttributes.Delay > 0)
            {
                Map.ListDelayedAttack.Add(new DelayedAttack(ActiveUnit.CurrentAttack, ActiveUnit, ActivePlayerIndex, AttackChoice));
                Map.ListActionMenuChoice.RemoveAllSubActionPanels();
                ActiveUnit.EndTurn();
            }
            else
            {
                Stack<Tuple<int, int>> ListMAPAttackTarget = Map.GetEnemies(ActiveUnit.CurrentAttack.MAPAttributes.FriendlyFire, AttackChoice);

                if (ListMAPAttackTarget.Count > 0)
                {
                    Map.GlobalBattleParams.GlobalContext.ArrayAttackPosition = AttackChoice.ToArray();

                    AttackWithMAPAttack(Map, ActivePlayerIndex, ActiveUnitIndex, ActiveUnit.CurrentAttack, ListMVHoverPoints, ListMAPAttackTarget);

                    //Remove Ammo if needed.
                    if (ActiveUnit.CurrentAttack.MaxAmmo > 0)
                        ActiveUnit.CurrentAttack.ConsumeAmmo();
                }
            }
        }

        public static void AttackWithMAPAttack(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex, Attack CurrentAttack, List<Vector3> ListMVHoverPoints, Stack<Tuple<int, int>> ListMAPAttackTarget)
        {
            if (ListMAPAttackTarget.Count > 0)
            {
                UnitConquest ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
                Map.ListMAPAttackTarget = ListMAPAttackTarget;
                Tuple<int, int> FirstEnemy = ListMAPAttackTarget.Pop();

                Map.CursorPosition = Map.ListPlayer[FirstEnemy.Item1].ListUnit[FirstEnemy.Item2].Position;
                Map.CursorPositionVisible = Map.ListPlayer[FirstEnemy.Item1].ListUnit[FirstEnemy.Item2].Position;

                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelStartBattle(Map, ActivePlayerIndex, ActiveUnitIndex, CurrentAttack, ListMVHoverPoints, FirstEnemy.Item1, FirstEnemy.Item2, false));

                ActiveUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            switch (ActiveUnit.CurrentAttack.MAPAttributes.Property)
            {
                case WeaponMAPProperties.Spread:
                    if (ActiveInputManager.InputConfirmPressed())
                    {
                    }
                    else
                    {
                        Map.CursorControl(ActiveInputManager);//Move the cursor
                    }
                    break;

                case WeaponMAPProperties.Direction:
                    SelectMAPEnemies(Map, ActivePlayerIndex, ActiveUnitIndex, ListMVHoverPoints, ListAttackChoice);
                    Map.sndConfirm.Play();
                    break;

                case WeaponMAPProperties.Targeted:
                    if (ActiveInputManager.InputConfirmPressed())
                    {
                        SelectMAPEnemies(Map, ActivePlayerIndex, ActiveUnitIndex, ListMVHoverPoints, ListAttackChoice);
                        Map.sndConfirm.Play();
                    }
                    else
                    {
                        Map.CursorControl(ActiveInputManager);//Move the cursor
                    }
                    break;
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
            ListAttackChoice = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                ListAttackChoice.Add(Map.GetTerrain(new Vector3(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32())));
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
                BW.AppendFloat(ListAttackChoice[A].GridPosition.X);
                BW.AppendFloat(ListAttackChoice[A].GridPosition.Y);
                BW.AppendInt32(ListAttackChoice[A].LayerIndex);
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelUseMAPAttack(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

}
