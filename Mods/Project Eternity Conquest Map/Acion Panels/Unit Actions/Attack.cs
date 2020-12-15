using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAttack : ActionPanelConquest
    {
        private UnitConquest ActiveUnit;
        private List<Tuple<int, int>> ListSquadFound;
        private int AttackIndex;

        public ActionPanelAttack(ConquestMap Map, UnitConquest ActiveUnit, List<Tuple<int, int>> ListSquadFound, int AttackIndex)
            : base("Attack", Map)
        {
            this.ActiveUnit = ActiveUnit;
            this.ListSquadFound = ListSquadFound;
            this.AttackIndex = AttackIndex;
        }

        public override void OnSelect()
        {
            UnitConquest EnemyUnit = Map.ListPlayer[ListSquadFound[0].Item1].ListUnit[ListSquadFound[0].Item2];
            int TotalDamage = 0;
            int TotalEnemyDamage = 0;

            if (AttackIndex == 1)
            {
                TotalDamage = Map.GetAttackDamageWithWeapon1(ActiveUnit, EnemyUnit, ActiveUnit.HP);
            }
            else if (AttackIndex == 2)
            {
                TotalDamage = Map.GetAttackDamageWithWeapon2(ActiveUnit, EnemyUnit, ActiveUnit.HP);
            }

            List<Tuple<int, int>> ListSquadFoundEnemy = Map.CanSquadAttackWeapon1((int)EnemyUnit.X, (int)EnemyUnit.Y, ListSquadFound[0].Item1, EnemyUnit.ArmourType, EnemyUnit.ListAttack[0]);
            if (ListSquadFoundEnemy != null && ListSquadFoundEnemy.Count > 0 && ListSquadFoundEnemy.Contains(new Tuple<int, int>(Map.ActivePlayerIndex, Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.IndexOf(ActiveUnit))))
            {
                TotalEnemyDamage = Map.GetAttackDamageWithWeapon1(EnemyUnit, ActiveUnit, EnemyUnit.HP - TotalDamage);
            }
            else if (ListSquadFoundEnemy == null || ListSquadFoundEnemy.Count == 0)
            {
                ListSquadFoundEnemy = Map.CanSquadAttackWeapon2((int)EnemyUnit.X, (int)EnemyUnit.Y, ListSquadFound[0].Item1, EnemyUnit.ArmourType, EnemyUnit.ListAttack[1]);
                if (ListSquadFoundEnemy != null && ListSquadFoundEnemy.Count > 0 && ListSquadFoundEnemy.Contains(new Tuple<int, int>(Map.ActivePlayerIndex, Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.IndexOf(ActiveUnit))))
                {
                    TotalEnemyDamage = Map.GetAttackDamageWithWeapon2(EnemyUnit, ActiveUnit, EnemyUnit.HP - TotalDamage);
                }
            }

            Map.PushScreen(new BattleAnimationScreen(Map, ActiveUnit, EnemyUnit, TotalDamage, TotalEnemyDamage, "", true));

            EnemyUnit.DamageUnit(TotalDamage);
            ActiveUnit.DamageUnit(TotalEnemyDamage);

            Map.FinalizeMovement(ActiveUnit);
            ActiveUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);

            RemoveAllSubActionPanels();
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
