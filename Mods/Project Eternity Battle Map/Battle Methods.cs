using System;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMap
    {
        public struct BattleResult
        {
            public int AttackDamage;
            public int Accuracy;
            public bool AttackMissed;
            public bool AttackShootDown;
            public bool AttackSwordCut;
            public bool Shield;
            public string Barrier;
            public bool AttackWasCritical;
            public int AttackAttackerFinalEN;
            public int TargetPlayerIndex;
            public int TargetSquadIndex;
            public int TargetUnitIndex;
            private Unit _Target;
            public Unit Target { get { return _Target; } }
            public void SetTarget(int TargetPlayerIndex, int TargetSquadIndex, int TargetUnitIndex, Unit Target)
            {
                this.TargetPlayerIndex = TargetPlayerIndex;
                this.TargetSquadIndex = TargetSquadIndex;
                this.TargetUnitIndex = TargetUnitIndex;

                _Target = Target;
            }
        }

        public struct SquadBattleResult
        {
            public BattleResult[] ArrayResult;
            public BattleResult ResultSupportAttack;

            public SquadBattleResult(BattleResult[] ArrayResult)
            {
                this.ArrayResult = ArrayResult;
                ResultSupportAttack = new BattleResult();
            }
        }

        public bool CanNullifyAttack(Attack ActiveWeapon, byte AttackerMovementType, byte DefenderMovementType, Squad DefenderSquad, StatsBoosts DefenderBoosts)
        {
            //Check if the Unit can counter the attack.
            bool NullifyAttack = false;

            if (ActiveWeapon.DicRankByMovement[Core.Units.UnitStats.TerrainSeaIndex] == Array.IndexOf(Unit.Grades, '-') &&
                (DefenderMovementType == Core.Units.UnitStats.TerrainSeaIndex ||
                AttackerMovementType == Core.Units.UnitStats.TerrainSeaIndex))
            {
                NullifyAttack = true;
            }
            else if (DefenderBoosts.ParryModifier.Contains(ActiveWeapon.RelativePath) || DefenderBoosts.NullifyDamageModifier)
                NullifyAttack = true;
            else if (DefenderSquad != null)
            {
                for (int U = DefenderSquad.UnitsAliveInSquad - 1; U >= 0 && !NullifyAttack; --U)
                {
                    if (DefenderSquad[U].Boosts.NullifyAttackModifier.Contains(ActiveWeapon.RelativePath))
                        NullifyAttack = true;
                }
            }
            return NullifyAttack;
        }

        public BattleResult DamageFormula(Unit Attacker, Attack CurrentAttack, float DamageModifier, int Attack,
            int TargetPlayerIndex, int TargetSquadIndex, int TargetUnitIndex, Unit Defender, Unit.BattleDefenseChoices DefenseChoice,
            bool NullifyAttack, int Defense, bool CalculateCritical = false)
        {
            //FINAL DAMAGE = (((ATTACK - DEFENSE) * (ATTACKED AND DEFENDER SIZE COMPARISON)) + Additive Final Damage Bonuses) * Final Damage Multiplier Bonuses
            BattleResult Result = new BattleResult();
            Result.SetTarget(TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Defender);

            if (NullifyAttack)
            {
                Result.AttackDamage = 0;
            }
            else if (Defender.Boosts.FinalDamageTakenFixedModifier > 0)
            {
                Result.AttackDamage = Defender.Boosts.FinalDamageTakenFixedModifier;
            }
            else
            {
                Result.AttackWasCritical = false;
                int AttackerSize = Attacker.SizeValue;
                int DefenderSize = Defender.SizeValue;

                int AttackerSizeComparison = AttackerSize - DefenderSize;
                float Damage = Math.Max(0, (int)(((Attack - Defense) * (1 + AttackerSizeComparison / 100f) + Attacker.Boosts.BaseDamageModifier) * (Attacker.Boosts.BaseDamageMultiplier * Defender.Boosts.BaseDamageTakenReductionMultiplier)));

                if (CalculateCritical && !Attacker.Boosts.CriticalAlwaysFail)
                {
                    //(((Attacker Skill Stat - Defender Skill Stat) + Weapon Critical Hit Rate) + Additive effect) * Multiplying effec
                    int Critical = (Attacker.PilotSKL - Defender.PilotSKL) + CurrentAttack.Critical + Attacker.Boosts.CriticalHitRateModifier;
                    //Don't calculate critical if there is a damage multiplier.
                    if (Attacker.Boosts.BaseDamageMultiplier == 1)
                    {
                        if (Attacker.Boosts.CriticalAlwaysSucceed)
                            Critical = 100;
                    }
                    if (RandomHelper.Next(101) <= Critical)
                    {
                        float CriticalDamageMultiplier = 1.2f;
                        CriticalDamageMultiplier += Attacker.Boosts.CriticalBaseDamageMultiplier;
                        Damage += Attacker.Boosts.CriticalBaseDamageModifier;
                        Damage *= CriticalDamageMultiplier;
                        Damage *= Attacker.Boosts.CriticalFinalDamageMultiplier;
                        Damage += Attacker.Boosts.CriticalFinalDamageModifier;

                        Result.AttackWasCritical = true;
                    }
                }

                if (CurrentAttack.Pri != WeaponPrimaryProperty.MAP && CurrentAttack.Pri != WeaponPrimaryProperty.PER && CurrentAttack.ExplosionOption.ExplosionRadius == 0 && DefenseChoice == Unit.BattleDefenseChoices.Defend)
                {
                    Damage *= 0.5f;
                }

                Result.AttackDamage = (int)(Damage * DamageModifier) + Attacker.Boosts.FinalDamageModifier;

                Result.AttackDamage = (int)(Result.AttackDamage * Attacker.Boosts.FinalDamageMultiplier);
            }

            Result.AttackMissed = false;

            #region Sword Cut

            if (Result.AttackDamage > 0 && (CurrentAttack.Sec & WeaponSecondaryProperty.SwordCut) == WeaponSecondaryProperty.SwordCut)
            {
                bool PilotSwordCut = false;
                bool UnitSwordCut = Defender.Boosts.SwordCutModifier;

                if (UnitSwordCut)
                {
                    foreach (KeyValuePair<string, List<BaseEffect>> ActiveListEffect in Defender.Pilot.Effects.GetEffects())
                    {
                        if (Defender.ListIgnoreSkill.Contains(ActiveListEffect.Key))
                            continue;

                        for (int E = ActiveListEffect.Value.Count - 1; E >= 0 && !PilotSwordCut; --E)
                        {
                            if (ActiveListEffect.Value[E] is SwordCutEffect)
                            {
                                PilotSwordCut = true;
                            }
                        }
                    }
                    if (PilotSwordCut)
                    {
                        int SwordCutActivation = (Defender.PilotSKL - Attacker.PilotSKL) + 10;

                        bool SwordCutActivated = RandomHelper.RandomActivationCheck(SwordCutActivation);

                        if (SwordCutActivated)
                        {
                            Result.AttackMissed = true;
                            Result.AttackSwordCut = true;
                        }
                    }
                }
            }

            #endregion

            #region Shoot Down

            if (Result.AttackDamage > 0 && (CurrentAttack.Sec & WeaponSecondaryProperty.ShootDown) == WeaponSecondaryProperty.ShootDown)
            {
                bool PilotShootDown = false;
                bool UnitShootDown = Defender.Boosts.ShootDownModifier;

                if (UnitShootDown)
                {
                    foreach (KeyValuePair<string, List<BaseEffect>> ActiveListEffect in Defender.Pilot.Effects.GetEffects())
                    {
                        if (Defender.ListIgnoreSkill.Contains(ActiveListEffect.Key))
                            continue;

                        for (int E = ActiveListEffect.Value.Count - 1; E >= 0 && !PilotShootDown; --E)
                        {
                            if (ActiveListEffect.Value[E] is ShootDownEffect)
                            {
                                PilotShootDown = true;
                            }
                        }
                    }
                    if (PilotShootDown)
                    {
                        int ShootDownActivation = (Defender.PilotSKL - Attacker.PilotSKL) + 10;

                        bool ShootDownActivated = RandomHelper.RandomActivationCheck(ShootDownActivation);

                        if (ShootDownActivated)
                        {
                            Result.AttackMissed = true;
                            Result.AttackShootDown = true;
                        }
                    }
                }
            }

            #endregion

            #region Shield

            if (Result.AttackDamage > 0)
            {
                bool PilotShield = false;
                bool UnitShield = Defender.Boosts.ShieldModifier;

                if (UnitShield)
                {
                    foreach (KeyValuePair<string, List<BaseEffect>> ActiveListEffect in Defender.Pilot.Effects.GetEffects())
                    {
                        if (Defender.ListIgnoreSkill.Contains(ActiveListEffect.Key))
                            continue;

                        for (int E = ActiveListEffect.Value.Count - 1; E >= 0 && !PilotShield; --E)
                        {
                            if (ActiveListEffect.Value[E] is ShieldEffect)
                            {
                                PilotShield = true;
                            }
                        }
                    }
                    if (PilotShield)
                    {
                        int ShieldActivation = (Defender.PilotSKL - Attacker.PilotSKL) + 10;

                        bool ShieldActivated = RandomHelper.RandomActivationCheck(ShieldActivation);

                        if (ShieldActivated)
                        {
                            Result.Shield = true;
                            if (DefenseChoice == Unit.BattleDefenseChoices.Defend)
                            {
                                Result.AttackDamage = (int)(Result.AttackDamage * 0.25f);
                            }
                            else
                            {
                                Result.AttackDamage = (int)(Result.AttackDamage * 0.5f);
                            }
                        }
                    }
                }
            }

            #endregion

            #region Barrier

            if (Result.AttackDamage > 0)
            {
                bool IsBarrierBreak = false;
                bool IsBarrierActive = false;

                for (int C = Defender.ArrayCharacterActive.Length - 1; C >= 0 && !IsBarrierBreak && !IsBarrierActive; --C)
                {
                    foreach (KeyValuePair<string, List<BaseEffect>> ActiveListEffect in Defender.ArrayCharacterActive[C].Effects.GetEffects())
                    {
                        if (Defender.ListIgnoreSkill.Contains(ActiveListEffect.Key))
                            continue;

                        if (IsBarrierActive)
                            break;

                        for (int E = ActiveListEffect.Value.Count - 1; E >= 0 && !IsBarrierBreak && !IsBarrierActive; --E)
                        {
                            BarrierEffect ActiveBarrierEffect = ActiveListEffect.Value[E] as BarrierEffect;

                            if (ActiveBarrierEffect != null)
                            {
                                int ENCost = int.Parse(Params.ActiveParser.Evaluate(ActiveBarrierEffect.ENCost), CultureInfo.InvariantCulture);

                                if (Result.AttackAttackerFinalEN > ENCost)
                                {
                                    Result.AttackAttackerFinalEN -= ENCost;
                                    int BreakingDamage = int.Parse(Params.ActiveParser.Evaluate(ActiveBarrierEffect.BreakingDamage), CultureInfo.InvariantCulture);
                                    //Look for weapon breaker or damage breaker or if the Barrier can protect against that Attack.
                                    if ((ActiveBarrierEffect.EffectiveAttacks.Count > 0 && !ActiveBarrierEffect.EffectiveAttacks.Contains(CurrentAttack.RelativePath)) ||
                                        ActiveBarrierEffect.BreakingAttacks.Contains(CurrentAttack.RelativePath) ||
                                        Result.AttackDamage >= BreakingDamage)
                                    {
                                        IsBarrierBreak = true;
                                    }
                                    else
                                    {//Look for Skill breaker.
                                        for (int C2 = Attacker.ArrayCharacterActive.Length - 1; C2 >= 0 && !IsBarrierBreak; --C2)
                                        {
                                            foreach (KeyValuePair<string, List<BaseEffect>> AttackerListEffect in Attacker.ArrayCharacterActive[C2].Effects.GetEffects())
                                            {
                                                if (ActiveBarrierEffect.BreakingAttacks.Contains(AttackerListEffect.Key))
                                                {
                                                    IsBarrierBreak = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (!IsBarrierBreak)
                                    {
                                        IsBarrierActive = true;
                                        Result.Barrier = ActiveListEffect.Key;
                                        if (Result.AttackDamage <= 10)
                                        {
                                            if (ActiveBarrierEffect.BarrierType == BarrierEffect.BarrierTypes.Dodge)
                                                Result.AttackMissed = true;
                                            else if (ActiveBarrierEffect.BarrierType == BarrierEffect.BarrierTypes.Defend)
                                                Result.AttackDamage = 0;
                                        }
                                        else
                                        {
                                            if (ActiveBarrierEffect.BarrierType == BarrierEffect.BarrierTypes.Dodge)
                                            {
                                                Result.AttackMissed = true;
                                            }
                                            else if (ActiveBarrierEffect.BarrierType == BarrierEffect.BarrierTypes.Defend)
                                            {
                                                float DamageReduction = float.Parse(Params.ActiveParser.Evaluate(ActiveBarrierEffect.DamageReduction), CultureInfo.InvariantCulture);
                                                if (ActiveBarrierEffect.NumberType == Operators.NumberTypes.Absolute)
                                                    Result.AttackDamage = Math.Max(0, Result.AttackDamage - (int)DamageReduction);
                                                else if (ActiveBarrierEffect.NumberType == Operators.NumberTypes.Relative)
                                                    Result.AttackDamage = (int)(Result.AttackDamage * DamageReduction);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            return Result;
        }

        public virtual List<Vector3> GetAttackChoice(Unit CurrentUnit, Vector3 CurrentPosition)
        {
            //Define the minimum and maximum value of the attack range.
            int StartingMV = CurrentUnit.CurrentAttack.RangeMinimum;
            int FinishingMV = CurrentUnit.CurrentAttack.RangeMaximum;
            if (FinishingMV > 1)
                FinishingMV += CurrentUnit.Boosts.RangeModifier;

            return ComputeRange(CurrentPosition, StartingMV, FinishingMV);
        }

        public List<Vector3> ComputeRange(Vector3 CurrentPosition, int StartingMV, int FinishingMV)
        {
            //Start a the cursor position.
            float PosX = CurrentPosition.X;
            float PosY = CurrentPosition.Y;
            float PosZ = CurrentPosition.Z;
            int x = 0;
            int y;

            List<Vector3> AttackChoice = new List<Vector3>();
            //As long as not out of map or out of range.
            while (x <= FinishingMV)
            {
                y = 0;
                //As long as not out of map or out of range.
                while (PosY + y >= 0 && (x + y) <= FinishingMV)
                {//If at least at the minimum range.
                    if ((x + y) >= StartingMV)
                    {//Add point of the position, up, down, left, right.
                        Vector3 NewPoint = new Vector3(PosX - x, PosY - y, PosZ);
                        if (!AttackChoice.Contains(NewPoint) &&
                            PosX - x >= 0 && PosX - x < MapSize.X &&
                            PosY - y >= 0 && PosY - y < MapSize.Y)
                        {
                            AttackChoice.Add(NewPoint);
                        }

                        NewPoint = new Vector3(PosX - x, PosY + y, PosZ);
                        if (!AttackChoice.Contains(NewPoint) &&
                            PosX - x >= 0 && PosX - x < MapSize.X &&
                            PosY + y >= 0 && PosY + y < MapSize.Y)
                        {
                            AttackChoice.Add(NewPoint);
                        }

                        NewPoint = new Vector3(PosX + x, PosY - y, PosZ);
                        if (!AttackChoice.Contains(NewPoint) &&
                            PosX + x >= 0 && PosX + x < MapSize.X &&
                            PosY - y >= 0 && PosY - y < MapSize.Y)
                        {
                            AttackChoice.Add(NewPoint);
                        }

                        NewPoint = new Vector3(PosX + x, PosY + y, PosZ);
                        if (!AttackChoice.Contains(NewPoint) &&
                            PosX + x >= 0 && PosX + x < MapSize.X &&
                            PosY + y >= 0 && PosY + y < MapSize.Y)
                        {
                            AttackChoice.Add(NewPoint);
                        }
                    }
                    y++;//Proceed vertically.
                }
                x++;//Proceed horizontally.
            }

            return AttackChoice;
        }
        
        public void DrawAttackPanel(CustomSpriteBatch g, SpriteFont ActiveFont)
        {
            AttackPicker.DrawAttackPanel(g, ActiveFont);
        }
    }
}
