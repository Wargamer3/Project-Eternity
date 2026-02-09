using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAttack : ActionPanelConquest
    {
        private const string PanelName = "Attack";

        private UnitConquest ActiveUnit;
        private List<Tuple<int, int>> ListSquadFound;
        private int AttackIndex;

        public ActionPanelAttack(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttack(ConquestMap Map, UnitConquest ActiveUnit, List<Tuple<int, int>> ListSquadFound, int AttackIndex)
            : base(PanelName, Map)
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

            TerrainConquest SquadTerrain = Map.GetTerrain(EnemyUnit.Components);

            if (AttackIndex == 1)
            {
                TotalDamage = Map.GetFinalDamage(ActiveUnit.HP, EnemyUnit.HP, Map.DicUnitDamageWeapon1[ActiveUnit.ArmourType][EnemyUnit.ArmourType], SquadTerrain);
                //TotalDamage = Map.GetAttackDamageWithWeapon1(ActiveUnit, EnemyUnit, ActiveUnit.HP);
            }
            else if (AttackIndex == 2)
            {
                TotalDamage = Map.GetFinalDamage(ActiveUnit.HP, EnemyUnit.HP, Map.DicUnitDamageWeapon2[ActiveUnit.ArmourType][EnemyUnit.ArmourType], SquadTerrain);
                //TotalDamage = Map.GetAttackDamageWithWeapon2(ActiveUnit, EnemyUnit, ActiveUnit.HP);
            }

            List<Tuple<int, int>> ListSquadFoundEnemy = Map.CanSquadAttackWeapon1((int)EnemyUnit.X, (int)EnemyUnit.Y, ListSquadFound[0].Item1, EnemyUnit.ArmourType, EnemyUnit.ListAttack[0]);
            if (ListSquadFoundEnemy != null && ListSquadFoundEnemy.Count > 0 && ListSquadFoundEnemy.Contains(new Tuple<int, int>(Map.ActivePlayerIndex, Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.IndexOf(ActiveUnit))))
            {
                TotalEnemyDamage = Map.GetFinalDamage(EnemyUnit.HP - TotalDamage, ActiveUnit.HP, Map.DicUnitDamageWeapon1[ActiveUnit.ArmourType][EnemyUnit.ArmourType], SquadTerrain);
                //TotalEnemyDamage = Map.GetAttackDamageWithWeapon1(EnemyUnit, ActiveUnit, EnemyUnit.HP - TotalDamage);
            }
            else if (ListSquadFoundEnemy == null || ListSquadFoundEnemy.Count == 0)
            {
                ListSquadFoundEnemy = Map.CanSquadAttackWeapon2((int)EnemyUnit.X, (int)EnemyUnit.Y, ListSquadFound[0].Item1, EnemyUnit.ArmourType, EnemyUnit.ListAttack[1]);
                if (ListSquadFoundEnemy != null && ListSquadFoundEnemy.Count > 0 && ListSquadFoundEnemy.Contains(new Tuple<int, int>(Map.ActivePlayerIndex, Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.IndexOf(ActiveUnit))))
                {
                    TotalEnemyDamage = Map.GetFinalDamage(EnemyUnit.HP - TotalDamage, ActiveUnit.HP, Map.DicUnitDamageWeapon2[ActiveUnit.ArmourType][EnemyUnit.ArmourType], SquadTerrain);
                    //TotalEnemyDamage = Map.GetAttackDamageWithWeapon2(EnemyUnit, ActiveUnit, EnemyUnit.HP - TotalDamage);
                }
            }

            Map.PushScreen(new BattleAnimationScreen(Map, ActiveUnit, EnemyUnit, TotalDamage, TotalEnemyDamage, "", true));

            EnemyUnit.DamageUnit(TotalDamage);
            ActiveUnit.DamageUnit(TotalEnemyDamage);

            Map.FinalizeMovement(ActiveUnit, 0, new List<Vector3>());
            ActiveUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);

            RemoveAllSubActionPanels();
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAttack(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
