using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.AnimationScreen;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelStartBattle : ActionPanelConquest
    {
        private const string PanelName = "StartBattle";

        private int ActivePlayerIndex;
        private int ActiveUnitIndex;
        private UnitConquest ActiveUnit;
        private Attack CurrentAttack;
        private List<Vector3> ListMVHoverPoint;

        private int TargetPlayerIndex;
        private int TargetSquadIndex;
        private UnitConquest TargetUnit;

        private bool IsDefending;
        private bool IsActiveUnitOnRight;

        private SquadBattleResult AttackingResult;
        private SquadBattleResult DefendingResult;

        private List<GameScreen> ListNextAnimationScreen;

        public ActionPanelStartBattle(ConquestMap Map)
            : base(PanelName, Map, false)
        {
            ListNextAnimationScreen = new List<GameScreen>();
            SendBackToSender = true;
        }

        public ActionPanelStartBattle(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex, Attack CurrentAttack, 
            List<Vector3> ListMVHoverPoints,
             int TargetPlayerIndex, int TargetSquadIndex,  bool IsDefending)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;
            this.CurrentAttack = CurrentAttack;
            this.ListMVHoverPoint = ListMVHoverPoints;

            this.TargetPlayerIndex = TargetPlayerIndex;
            this.TargetSquadIndex = TargetSquadIndex;

            this.IsDefending = IsDefending;

            SendBackToSender = true;

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
            TargetUnit = Map.ListPlayer[TargetPlayerIndex].ListUnit[TargetSquadIndex];

            ListNextAnimationScreen = new List<GameScreen>();
        }

        public override void OnSelect()
        {
            if (Map.IsOnlineClient)
            {
                return;
            }

            if (IsDefending || Map.ListPlayer[TargetPlayerIndex].IsPlayerControlled)
            {
               // InitPlayerDefence();
            }
            else
            {
               // InitPlayerAttack();
                ActiveUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ListNextAnimationScreen.Count > 0)
            {
                Map.PushScreen(ListNextAnimationScreen[0]);
                ListNextAnimationScreen.Remove(ListNextAnimationScreen[0]);
            }
            else if (CurrentAttack.Pri == WeaponPrimaryProperty.PER || CurrentAttack.Pri == WeaponPrimaryProperty.MAP)
            {
                ListActionMenuChoice.Remove(this);
            }
            else
            {
                ListActionMenuChoice.RemoveAllSubActionPanels();
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            if (ListNextAnimationScreen.Count > 0)
            {
                Map.PushScreen(ListNextAnimationScreen[0]);
                ListNextAnimationScreen.Remove(ListNextAnimationScreen[0]);
            }
            else if (CurrentAttack.Pri == WeaponPrimaryProperty.PER)
            {
                ListActionMenuChoice.Remove(this);
            }
            else
            {
                ListActionMenuChoice.RemoveAllSubActionPanels();
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
                        CurrentAttack = ActiveUnit.CurrentAttack = ActiveAttack;
                        break;
                    }
                }
            }

            int ListMVHoverPointCount = BR.ReadInt32();
            ListMVHoverPoint = new List<Vector3>(ListMVHoverPointCount);
            for (int M = 0; M < ListMVHoverPointCount; ++M)
            {
                ListMVHoverPoint.Add(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            }

            TargetPlayerIndex = BR.ReadInt32();
            TargetSquadIndex = BR.ReadInt32();
            TargetUnit = Map.ListPlayer[TargetPlayerIndex].ListUnit[TargetSquadIndex];
            string TargetSquadAttackName = BR.ReadString();
            if (!string.IsNullOrEmpty(TargetSquadAttackName))
            {
                foreach (Attack ActiveAttack in TargetUnit.ListAttack)
                {
                    if (ActiveAttack.ItemName == TargetSquadAttackName)
                    {
                        TargetUnit.CurrentAttack = ActiveAttack;
                        break;
                    }
                }
            }

            IsDefending = BR.ReadBoolean();
            IsActiveUnitOnRight = BR.ReadBoolean();

            if (Map.IsServer)
            {
                //InitPlayerBattle(IsActiveUnitOnRight);
            }
            else
            {
                int AttackingResultArrayResultLength = BR.ReadInt32();
                AttackingResult.ArrayResult = new BattleResult[AttackingResultArrayResultLength];

                for (int R = 0; R < AttackingResultArrayResultLength; ++R)
                {
                    AttackingResult.ArrayResult[R] = new BattleResult();
                    AttackingResult.ArrayResult[R].Accuracy = BR.ReadInt32();
                    AttackingResult.ArrayResult[R].AttackAttackerFinalEN = BR.ReadInt32();
                    AttackingResult.ArrayResult[R].AttackDamage = BR.ReadInt32();
                    AttackingResult.ArrayResult[R].AttackMissed = BR.ReadBoolean();
                    AttackingResult.ArrayResult[R].AttackShootDown = BR.ReadBoolean();
                    AttackingResult.ArrayResult[R].AttackSwordCut = BR.ReadBoolean();
                    AttackingResult.ArrayResult[R].AttackWasCritical = BR.ReadBoolean();
                    AttackingResult.ArrayResult[R].Barrier = BR.ReadString();
                    AttackingResult.ArrayResult[R].Shield = BR.ReadBoolean();

                    int TargetPlayerIndex = BR.ReadInt32();
                    int TargetSquadIndex = BR.ReadInt32();

                    AttackingResult.ArrayResult[R].TargetPlayerIndex = TargetPlayerIndex;
                    AttackingResult.ArrayResult[R].TargetSquadIndex = TargetSquadIndex;

                    AttackingResult.ArrayResult[R].SetTarget(TargetPlayerIndex, TargetSquadIndex, -1, Map.ListPlayer[TargetPlayerIndex].ListUnit[TargetSquadIndex]);
                }

                int DefendingResultArrayResultLength = BR.ReadInt32();
                DefendingResult.ArrayResult = new BattleResult[DefendingResultArrayResultLength];

                for (int R = 0; R < DefendingResultArrayResultLength; ++R)
                {
                    DefendingResult.ArrayResult[R] = new BattleResult();
                    DefendingResult.ArrayResult[R].Accuracy = BR.ReadInt32();
                    DefendingResult.ArrayResult[R].AttackAttackerFinalEN = BR.ReadInt32();
                    DefendingResult.ArrayResult[R].AttackDamage = BR.ReadInt32();
                    DefendingResult.ArrayResult[R].AttackMissed = BR.ReadBoolean();
                    DefendingResult.ArrayResult[R].AttackShootDown = BR.ReadBoolean();
                    DefendingResult.ArrayResult[R].AttackSwordCut = BR.ReadBoolean();
                    DefendingResult.ArrayResult[R].AttackWasCritical = BR.ReadBoolean();
                    DefendingResult.ArrayResult[R].Barrier = BR.ReadString();
                    DefendingResult.ArrayResult[R].Shield = BR.ReadBoolean();

                    int TargetPlayerIndex = BR.ReadInt32();
                    int TargetSquadIndex = BR.ReadInt32();

                    DefendingResult.ArrayResult[R].TargetPlayerIndex = TargetPlayerIndex;
                    DefendingResult.ArrayResult[R].TargetSquadIndex = TargetSquadIndex;

                    DefendingResult.ArrayResult[R].SetTarget(TargetPlayerIndex, TargetSquadIndex, -1, Map.ListPlayer[TargetPlayerIndex].ListUnit[TargetSquadIndex]);
                }

                if (IsDefending || Map.ListPlayer[TargetPlayerIndex].IsPlayerControlled)
                {
                    //InitPlayerDefence();
                }
                else
                {
                    //InitPlayerAttack();
                }
            }
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveUnitIndex);
            if (ActiveUnit.CurrentAttack != null)
            {
                BW.AppendString(ActiveUnit.CurrentAttack.ItemName);
            }
            else
            {
                BW.AppendString("");
            }

            BW.AppendInt32(ListMVHoverPoint.Count);
            for (int M = 0; M < ListMVHoverPoint.Count; ++M)
            {
                BW.AppendFloat(ListMVHoverPoint[M].X);
                BW.AppendFloat(ListMVHoverPoint[M].Y);
                BW.AppendFloat(ListMVHoverPoint[M].Z);
            }

            BW.AppendInt32(TargetPlayerIndex);
            BW.AppendInt32(TargetSquadIndex);

            if (TargetUnit.CurrentAttack != null)
            {
                BW.AppendString(TargetUnit.CurrentAttack.ItemName);
            }
            else
            {
                BW.AppendString("");
            }

            BW.AppendBoolean(IsDefending);
            BW.AppendBoolean(IsActiveUnitOnRight);

            if (Map.IsServer)
            {
                BW.AppendInt32(AttackingResult.ArrayResult.Length);
                for (int R = 0; R < AttackingResult.ArrayResult.Length; ++R)
                {
                    BW.AppendInt32(AttackingResult.ArrayResult[R].Accuracy);
                    BW.AppendInt32(AttackingResult.ArrayResult[R].AttackAttackerFinalEN);
                    BW.AppendInt32(AttackingResult.ArrayResult[R].AttackDamage);
                    BW.AppendBoolean(AttackingResult.ArrayResult[R].AttackMissed);
                    BW.AppendBoolean(AttackingResult.ArrayResult[R].AttackShootDown);
                    BW.AppendBoolean(AttackingResult.ArrayResult[R].AttackSwordCut);
                    BW.AppendBoolean(AttackingResult.ArrayResult[R].AttackWasCritical);
                    BW.AppendString(AttackingResult.ArrayResult[R].Barrier);
                    BW.AppendBoolean(AttackingResult.ArrayResult[R].Shield);
                    BW.AppendInt32(AttackingResult.ArrayResult[R].TargetPlayerIndex);
                    BW.AppendInt32(AttackingResult.ArrayResult[R].TargetSquadIndex);
                }

                BW.AppendInt32(DefendingResult.ArrayResult.Length);
                for (int R = 0; R < DefendingResult.ArrayResult.Length; ++R)
                {
                    BW.AppendInt32(DefendingResult.ArrayResult[R].Accuracy);
                    BW.AppendInt32(DefendingResult.ArrayResult[R].AttackAttackerFinalEN);
                    BW.AppendInt32(DefendingResult.ArrayResult[R].AttackDamage);
                    BW.AppendBoolean(DefendingResult.ArrayResult[R].AttackMissed);
                    BW.AppendBoolean(DefendingResult.ArrayResult[R].AttackShootDown);
                    BW.AppendBoolean(DefendingResult.ArrayResult[R].AttackSwordCut);
                    BW.AppendBoolean(DefendingResult.ArrayResult[R].AttackWasCritical);
                    BW.AppendString(DefendingResult.ArrayResult[R].Barrier);
                    BW.AppendBoolean(DefendingResult.ArrayResult[R].Shield);
                    BW.AppendInt32(DefendingResult.ArrayResult[R].TargetPlayerIndex);
                    BW.AppendInt32(DefendingResult.ArrayResult[R].TargetSquadIndex);
                }
            }
            else
            {

            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelStartBattle(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
