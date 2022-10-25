using System;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class DeathmatchMap
    {
        /**
         * ATTACK = (((Attack Power * ((Pilot Will + Pilot Stat)/200) * Attack Side Terrain Performance) + Additive Base Damage Bonuses) * Base Damage Multiplier Bonuses
DEFENSE = ((Robot Armor Stat * ((Pilot Will + Pilot Def)/200) * Defense Side Terrain Performance) + Additive Base Defense Bonuses * Multiplying base defense bonuses) * Tile Bonus

FINAL DAMAGE = (((ATTACK - DEFENSE) * (ATTACKED AND DEFENDER SIZE COMPARISON)) + Additive Final Damage Bonuses) * Final Damage Multiplier Bonuses
    */
        public static int AttackFormula(Unit Attacker, Attack CurrentAttack, int WeaponTerrain, FormulaParser ActiveParser)
        {
            int PilotMorale = Attacker.PilotMorale;
            int PilotPower;

            if (CurrentAttack.Style == WeaponStyle.M)
                PilotPower = Attacker.PilotMEL;
            else
                PilotPower = Attacker.PilotRNG;

            int AttackFormula = (int)(CurrentAttack.GetPower(Attacker, ActiveParser) * (PilotMorale + PilotPower) / 200f * (1 + WeaponTerrain / 100f));
            return AttackFormula;
        }

        public int AttackFormula(Unit Attacker, Attack CurrentAttack, byte AttackerTerrainType)
        {
            int WeaponTerrain = CurrentAttack.TerrainAttribute(AttackerTerrainType);
            
            return AttackFormula(Attacker, CurrentAttack, WeaponTerrain, ActiveParser);
        }

        public static int DefenseFormula(int Armor, int PilotMorale, int DefenderPilotDEF, int DefenderTerrain)
        {
            int DefenseFormula = (int)(Armor * ((PilotMorale + DefenderPilotDEF) / 200f) * (1 + DefenderTerrain / 100f));
            return DefenseFormula;
        }

        public int DefenseFormula(Unit Defender, byte DefenderTerrainType, Terrain DefenderTerrain)
        {
            int Armor = Defender.Armor + GetTerrainBonus(DefenderTerrain, TerrainActivation.OnEveryTurns, TerrainBonus.Armor);
            int DefenderTerrainRate = Defender.TerrainArmorAttribute(DefenderTerrainType);
            int DefenderPilotDEF = Defender.PilotDEF;
            
            return DefenseFormula(Armor, Defender.PilotMorale, DefenderPilotDEF, DefenderTerrainRate);
        }

        public BattleResult DamageFormula(Unit Attacker, Attack CurrentAttack, Squad AttackerSquad, float DamageModifier,
            int TargetPlayerIndex, int TargetSquadIndex, int TargetUnitIndex, Unit.BattleDefenseChoices DefenseChoice, bool CalculateCritical)
        {
            Squad DefenderSquad = ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex];
            Unit Defender = DefenderSquad[TargetUnitIndex];

            byte AttackerTerrainType;
            byte DefenderTerrainType;
            Terrain DefenderTerrain;

            AttackerTerrainType = GetTerrain(AttackerSquad).TerrainTypeIndex;

            DefenderTerrainType = GetTerrain(DefenderSquad).TerrainTypeIndex;
            DefenderTerrain = GetTerrain(DefenderSquad);

            //Check if the Unit can counter the attack.
            bool NullifyAttack = CanNullifyAttack(CurrentAttack, AttackerTerrainType, DefenderSquad.CurrentTerrainIndex, DefenderSquad, Defender.Boosts);

            int Attack = AttackFormula(Attacker, CurrentAttack, AttackerTerrainType);
            int Defense = DefenseFormula(Defender, DefenderTerrainType, DefenderTerrain);

            BattleResult Result = DamageFormula(Attacker, CurrentAttack, DamageModifier, Attack, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Defender, DefenseChoice, NullifyAttack, Defense, CalculateCritical);

            return Result;
        }

        //(((Pilot Hit Stat/2 + 130) * Final Terrain Multiplier) + Weapon Hit Rate) + Base Hit Rate Effect
        public static int Accuracy(Unit Attacker, Attack CurrentAttack, int FinalAttackerTerrainMultiplier)
        {
            return (int)((((Attacker.PilotHIT / 2 + 130) * ((100 + FinalAttackerTerrainMultiplier) / 100.0)) + CurrentAttack.Accuracy + Attacker.Boosts.AccuracyModifier) * Attacker.Boosts.AccuracyMultiplier);
        }

        public int Accuracy(Unit Attacker, Attack CurrentAttack, byte AttackerTerrainType)
        {
            int AttackerTerrain = 0;
            int AttackerPilotTerrain = 0;
            char AttackerTerrainLetter = Attacker.TerrainLetterAttribute(AttackerTerrainType);
            char AttackerPilotTerrainLetter = Character.ListGrade[Attacker.Pilot.DicRankByMovement[AttackerTerrainType]];

            switch (AttackerTerrainLetter)
            {
                case 'S':
                    AttackerTerrain = 20;
                    break;

                case 'A':
                    AttackerTerrain = 10;
                    break;

                case 'B':
                    AttackerTerrain = 0;
                    break;

                case 'C':
                    AttackerTerrain = -10;
                    break;

                case 'D':
                    AttackerTerrain = -20;
                    break;
            }

            switch (AttackerPilotTerrainLetter)
            {
                case 'S':
                    AttackerPilotTerrain = 20;
                    break;

                case 'A':
                    AttackerPilotTerrain = 10;
                    break;

                case 'B':
                    AttackerPilotTerrain = 0;
                    break;

                case 'C':
                    AttackerPilotTerrain = -10;
                    break;

                case 'D':
                    AttackerPilotTerrain = -20;
                    break;
            }

            int FinalAttackerTerrainMultiplier = 0;

            switch (AttackerTerrain + AttackerPilotTerrain)
            {
                case -40:
                case -30:
                    FinalAttackerTerrainMultiplier = -60;
                    break;

                case -20:
                case -10:
                    FinalAttackerTerrainMultiplier = -40;
                    break;

                case 0:
                case 10:
                    FinalAttackerTerrainMultiplier = -20;
                    break;

                case 20:
                case 30:
                    FinalAttackerTerrainMultiplier = 0;
                    break;

                case 40:
                    FinalAttackerTerrainMultiplier = 20;
                    break;
            }

            return Accuracy(Attacker, CurrentAttack, FinalAttackerTerrainMultiplier);
        }

        //((Pilot Evasion/2)+Robot Mobility) * Final Terrain Multiplier) + Base Evasion Effect
        public static int Evasion(Unit Defender, int TerrainBonus, int FinalDefenderTerrainMultiplier)
        {
            int other = Defender.Boosts.EvasionModifier + TerrainBonus;

            return (int)((Defender.PilotEVA / 2 + Defender.Mobility) * ((100 + FinalDefenderTerrainMultiplier) / 100.0)) + other;
        }

        public int Evasion(Unit Defender, byte DefenderTerrainType, Terrain DefenderTerrain)
        {
            int DefenderTerrainRate = 0;
            int DefenderPilotTerrain = 0;
            char DefenderTerrainLetter = Defender.TerrainLetterAttribute(DefenderTerrainType);
            char DefenderPilotTerrainLetter = Character.ListGrade[Defender.Pilot.DicRankByMovement[DefenderTerrainType]];

            switch (DefenderTerrainLetter)
            {
                case 'S':
                    DefenderTerrainRate = 20;
                    break;

                case 'A':
                    DefenderTerrainRate = 10;
                    break;

                case 'B':
                    DefenderTerrainRate = 0;
                    break;

                case 'C':
                    DefenderTerrainRate = -10;
                    break;

                case 'D':
                    DefenderTerrainRate = -20;
                    break;
            }

            switch (DefenderPilotTerrainLetter)
            {
                case 'S':
                    DefenderPilotTerrain = 20;
                    break;

                case 'A':
                    DefenderPilotTerrain = 10;
                    break;

                case 'B':
                    DefenderPilotTerrain = 0;
                    break;

                case 'C':
                    DefenderPilotTerrain = -10;
                    break;

                case 'D':
                    DefenderPilotTerrain = -20;
                    break;
            }

            int FinalDefenderTerrainMultiplier = 0;

            switch (DefenderTerrainRate + DefenderPilotTerrain)
            {
                case -40:
                case -30:
                    FinalDefenderTerrainMultiplier = -60;
                    break;

                case -20:
                case -10:
                    FinalDefenderTerrainMultiplier = -40;
                    break;

                case 0:
                case 10:
                    FinalDefenderTerrainMultiplier = -20;
                    break;

                case 20:
                case 30:
                    FinalDefenderTerrainMultiplier = 0;
                    break;

                case 40:
                    FinalDefenderTerrainMultiplier = 20;
                    break;
            }

            return Evasion(Defender, GetTerrainBonus(DefenderTerrain, TerrainActivation.OnEveryTurns, TerrainBonus.Evasion), FinalDefenderTerrainMultiplier);
        }

        //(((Attacker Hit Rate + Defender Evasion) * Size Difference Multiplier) + Additive final hit rate effect) * Multiplying final hit rate effect
        public int CalculateHitRate(Unit Attacker, Attack CurrentAttack, byte AttackerTerrainType, Unit Defender, byte DefenderTerrainType, Terrain DefenderTerrain, Unit.BattleDefenseChoices DefenseChoice)
        {
            int SizeCompare = Attacker.SizeValue - Defender.SizeValue;

            float BaseHitRate;
            //If the Attacker have an accuracy modifier, use it.
            if (Attacker.Boosts.AccuracyFixedModifier > 0)
                BaseHitRate = Attacker.Boosts.AccuracyFixedModifier;
            //If the Defender have an accuracy modifier, use it.
            else if (Defender.Boosts.EvasionFixedModifier > 0)
                BaseHitRate = 100 - Defender.Boosts.EvasionFixedModifier;
            //Force the defender to dodge the attack.
            else if (Defender.Boosts.AutoDodgeModifier)
                BaseHitRate = 0;
            else//No particular modifier, use basic hit rate formula.
            {
                BaseHitRate = (Accuracy(Attacker, CurrentAttack, AttackerTerrainType) - Evasion(Defender, DefenderTerrainType, DefenderTerrain)) * (1 + -SizeCompare / 100f);
                if (DefenseChoice == Unit.BattleDefenseChoices.Evade)
                    BaseHitRate *= 0.5f;
            }
            return (int)Math.Max(0, Math.Min(100, BaseHitRate));
        }

        public int CalculateHitRate(Unit Attacker, Attack CurrentAttack, Squad AttackerSquad, Unit Defender, Squad DefenderSquad, Unit.BattleDefenseChoices DefenseChoice)
        {
            if (CurrentAttack.Pri == WeaponPrimaryProperty.PER)
            {
                return 100;
            }

            byte AttackerTerrainType;
            byte DefenderTerrainType;
            Terrain DefenderTerrain;

            AttackerTerrainType = GetTerrain(AttackerSquad).TerrainTypeIndex;

            DefenderTerrainType = GetTerrain(DefenderSquad).TerrainTypeIndex;
            DefenderTerrain = GetTerrain(DefenderSquad);

            return CalculateHitRate(Attacker, CurrentAttack, AttackerTerrainType, Defender, DefenderTerrainType, DefenderTerrain, DefenseChoice);
        }

        private static int GetTerrainBonus(Terrain ActiveTerrain, TerrainActivation TerrainActivationType, TerrainBonus TerrainBonusType)
        {
            if (ActiveTerrain == null)
            {
                return 0;
            }

            int Output = 0;

            for (int i = 0; i < ActiveTerrain.ListActivation.Length; i++)
            {
                if (ActiveTerrain.ListActivation[i] == TerrainActivationType && ActiveTerrain.ListBonus[i] == TerrainBonusType)
                    Output += ActiveTerrain.ListBonusValue[i];
            }

            return Output;
        }

        private BattleResult GetBattleResult(Unit Attacker, Attack CurrentAttack, Squad AttackerSquad, float DamageModifier,
            int TargetPlayerIndex, int TargetSquadIndex, int TargetUnitIndex, bool ActivateSkills, bool CalculateCritical)
        {
            Squad DefenderSquad = ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex];
            Unit Defender = DefenderSquad[TargetUnitIndex];

            ActivateAutomaticSkills(AttackerSquad, Attacker, DeathmatchSkillRequirement.BeforeAttackRequirementName, DefenderSquad, Defender);
            ActivateAutomaticSkills(DefenderSquad, Defender, DeathmatchSkillRequirement.BeforeGettingAttackedRequirementName, AttackerSquad, Attacker);
            
            BattleResult Result;

            int BaseHitRate;

            BaseHitRate = CalculateHitRate(Attacker, CurrentAttack, AttackerSquad, Defender, DefenderSquad, Defender.BattleDefenseChoice);

            bool AttackHit = RandomHelper.RandomActivationCheck(BaseHitRate);
            
            if (AttackHit)
            {
                if (ActivateSkills)
                {
                    ActivateAutomaticSkills(AttackerSquad, Attacker, DeathmatchSkillRequirement.BeforeHitRequirementName, DefenderSquad, Defender);
                    ActivateAutomaticSkills(DefenderSquad, Defender, DeathmatchSkillRequirement.BeforeGettingHitRequirementName, AttackerSquad, Attacker);
                }

                Result = DamageFormula(Attacker, CurrentAttack, AttackerSquad, DamageModifier, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Defender.BattleDefenseChoice, CalculateCritical);
            }
            else
            {
                if (ActivateSkills)
                {
                    ActivateAutomaticSkills(AttackerSquad, Attacker, DeathmatchSkillRequirement.BeforeMissRequirementName, DefenderSquad, Defender);
                    ActivateAutomaticSkills(DefenderSquad, Defender, DeathmatchSkillRequirement.BeforeGettingMissedRequirementName, AttackerSquad, Attacker);
                }

                Result = new BattleResult();
                Result.AttackDamage = 0;
                Result.AttackMissed = true;
                Result.SetTarget(TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Defender);
            }

            Result.Accuracy = BaseHitRate;

            //Remove EN from the weapon cost.
            if (CurrentAttack.ENCost > 0)
            {
                Result.AttackAttackerFinalEN = Math.Max(0, Attacker.EN - (CurrentAttack.ENCost + Attacker.Boosts.ENCostModifier));
            }
            else
            {
                Result.AttackAttackerFinalEN = Attacker.EN;
            }

            Params.GlobalContext.Result = Result;

            if (ActivateSkills)
            {
                Attacker.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAttack);
                Defender.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnEnemyAttack);

                if (AttackHit)
                {
                    Attacker.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnHit);
                    Defender.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnEnemyHit);

                    ActivateAutomaticSkills(AttackerSquad, Attacker, DeathmatchSkillRequirement.AfterHitRequirementName, DefenderSquad, Defender);
                    ActivateAutomaticSkills(DefenderSquad, Defender, DeathmatchSkillRequirement.AfterGettingHitRequirementName, AttackerSquad, Attacker);
                }
                else
                {
                    ActivateAutomaticSkills(AttackerSquad, Attacker, DeathmatchSkillRequirement.AfterMissRequirementName, DefenderSquad, Defender);
                    ActivateAutomaticSkills(DefenderSquad, Defender, DeathmatchSkillRequirement.AfterGettingMissedRequirementName, AttackerSquad, Attacker);
                }

                ActivateAutomaticSkills(AttackerSquad, Attacker, DeathmatchSkillRequirement.AfterAttackRequirementName, DefenderSquad, Defender);
                ActivateAutomaticSkills(DefenderSquad, Defender, DeathmatchSkillRequirement.AfterGettingAttackedRequirementName, AttackerSquad, Attacker);
            }

            return Result;
        }

        public void FinalizeBattle(Squad Attacker, Attack CurrentAttack, SupportSquadHolder ActiveSquadSupport, int AttackerPlayerIndex,
            Squad TargetSquad, SupportSquadHolder TargetSquadSupport, int DefenderPlayerIndex, SquadBattleResult ResultAttack, SquadBattleResult ResultDefend)
        {
            List<Unit> ListDeadDefender = new List<Unit>();
            List<LevelUpMenu> ListBattleRecap = new List<LevelUpMenu>();
            bool HasRecap = false;

            if (Attacker != null)
            {
                Squad Target = TargetSquad;

                if (TargetSquadSupport.ActiveSquadSupport != null)
                {
                    Target = TargetSquadSupport.ActiveSquadSupport;
                    //Remove 1 Support Defend.
                    --TargetSquadSupport.ActiveSquadSupport.CurrentLeader.Boosts.SupportDefendModifier;
                }

                ListBattleRecap.AddRange(FinalizeBattle(Attacker, CurrentAttack, AttackerPlayerIndex, Target, DefenderPlayerIndex, ResultAttack, ListDeadDefender));

                //Counter attack
                if (TargetSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack && TargetSquad.CurrentLeader.HP > 0)
                {
                    ListBattleRecap.AddRange(FinalizeBattle(TargetSquad, TargetSquad.CurrentLeader.CurrentAttack, DefenderPlayerIndex, Attacker, AttackerPlayerIndex, ResultDefend, new List<Unit>()));
                }

                //Support Attack
                if (ActiveSquadSupport.ActiveSquadSupport != null && Attacker.CurrentLeader.HP > 0 && TargetSquad.CurrentLeader.HP > 0)
                {
                    //Remove 1 Support Defend.
                    --ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.Boosts.SupportAttackModifier;

                    LevelUpMenu BattleRecap = FinalizeBattle(ActiveSquadSupport.ActiveSquadSupport.CurrentLeader, ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.CurrentAttack, ActiveSquadSupport.ActiveSquadSupport, AttackerPlayerIndex,
                        TargetSquad.CurrentLeader, TargetSquad, DefenderPlayerIndex, ResultAttack.ResultSupportAttack, ListDeadDefender);

                    if (BattleRecap != null)
                    {
                        ListBattleRecap.Add(BattleRecap);
                    }
                }
                //Explosion of death cutscene
                if (Attacker.CurrentLeader == null)
                    PushScreen(new ExplosionCutscene(CenterCamera, this, Attacker));

                for (int R = ListBattleRecap.Count - 1; R >= 0; --R)
                {
                    if (Constants.ShowBattleRecap && ListBattleRecap[R].IsHuman)
                    {
                        PushScreen(ListBattleRecap[R]);

                        if (!HasRecap)
                        {
                            ListBattleRecap[R].SetBattleContent(true, AttackerPlayerIndex, Attacker, ActiveSquadSupport, DefenderPlayerIndex, TargetSquad, TargetSquadSupport);
                        }
                        else
                        {
                            ListBattleRecap[R].SetBattleContent(false, AttackerPlayerIndex, Attacker, ActiveSquadSupport, DefenderPlayerIndex, TargetSquad, TargetSquadSupport);
                        }

                        HasRecap = true;
                    }
                    else
                    {
                        ListBattleRecap[R].LevelUp();
                    }
                }
            }
            else
            {
                LevelUpMenu BattleRecap = FinalizeBattle(null, null, null, AttackerPlayerIndex, ResultAttack.ArrayResult[0].Target, TargetSquad, DefenderPlayerIndex, ResultAttack.ArrayResult[0], ListDeadDefender);
                if (BattleRecap != null)
                {
                    ListBattleRecap.Add(BattleRecap);
                }
            }

            //Explosion of death cutscene
            if (TargetSquad.CurrentLeader == null)
                PushScreen(new ExplosionCutscene(CenterCamera, this, TargetSquad));

            if (!HasRecap)
            {
                if (Attacker != null)
                {
                    Params.GlobalContext.SetContext(Attacker, Attacker.CurrentLeader, Attacker.CurrentLeader.Pilot, TargetSquad, TargetSquad.CurrentLeader, TargetSquad.CurrentLeader.Pilot, ActiveParser);
                }

                UpdateMapEvent(EventTypeOnBattle, 1);

                Params.GlobalContext.SetContext(null, null, null, null, null, null, ActiveParser);

                //Don't update the leader until after the events are processed. (If a battle map event try to read the leader of a dead unit it will crash on a null pointer as dead units have no leader)

                if (Attacker != null)
                {
                    Attacker.UpdateSquad();
                    if (ActiveSquadSupport != null && ActiveSquadSupport.ActiveSquadSupport != null)
                        ActiveSquadSupport.ActiveSquadSupport.UpdateSquad();

                    if (Attacker.IsDead)
                    {
                        GameRule.OnSquadDefeated(DefenderPlayerIndex, TargetSquad, AttackerPlayerIndex, Attacker);
                    }
                }

                TargetSquad.UpdateSquad();
                if (TargetSquadSupport != null && TargetSquadSupport.ActiveSquadSupport != null)
                    TargetSquadSupport.ActiveSquadSupport.UpdateSquad();

                if (TargetSquad.IsDead)
                {
                    GameRule.OnSquadDefeated(AttackerPlayerIndex, Attacker, DefenderPlayerIndex, TargetSquad);
                }
            }
        }

        private List<LevelUpMenu> FinalizeBattle(Squad Attacker, Attack CurrentAttack, int AttackerPlayerIndex,
                                   Squad Defender, int DefenderPlayerIndex,
                                   SquadBattleResult Result, List<Unit> ListDeadDefender)
        {

            List<LevelUpMenu> ListBattleRecap = new List<LevelUpMenu>();
            for (int U = 0; U < Attacker.UnitsAliveInSquad; U++)
            {
                LevelUpMenu BattleRecap = FinalizeBattle(Attacker[U], CurrentAttack, Attacker, AttackerPlayerIndex, Result.ArrayResult[U].Target, Defender, DefenderPlayerIndex, Result.ArrayResult[U], ListDeadDefender);
                if (BattleRecap != null)
                {
                    ListBattleRecap.Add(BattleRecap);
                }
            }

            if (!Attacker.ListAttackedTeam.Contains(ListPlayer[DefenderPlayerIndex].Team))
                Attacker.ListAttackedTeam.Add(ListPlayer[DefenderPlayerIndex].Team);

            if (!Defender.ListAttackedTeam.Contains(ListPlayer[AttackerPlayerIndex].Team))
                Defender.ListAttackedTeam.Add(ListPlayer[AttackerPlayerIndex].Team);

            return ListBattleRecap;
        }

        private LevelUpMenu FinalizeBattle(Unit Attacker, Attack CurrentAttack, Squad AttackerSquad, int AttackerPlayerIndex,
                                   Unit Defender, Squad DefenderSquad, int DefenderPlayerIndex,
                                   BattleResult Result, List<Unit> ListDeadDefender)
        {
            LevelUpMenu BattleRecap = null;
            if (!ListDeadDefender.Contains(Result.Target))
            {
                Result.Target.DamageUnit(Result.AttackDamage);

                //Remove Leader Ammo if needed.
                if (CurrentAttack != null && CurrentAttack.MaxAmmo > 0)
                {
                    CurrentAttack.ConsumeAmmo();
                }

                if (AttackerSquad != null)//Can get hurt by the environment
                {
                    Attacker.ConsumeEN(Attacker.EN - Result.AttackAttackerFinalEN);

                    int PilotPoint = 0;
                    Attacker.PilotPilotPoints += (int)(PilotPoint * Attacker.Boosts.PPMultiplier);

                    ActivateAutomaticSkills(AttackerSquad, Attacker, string.Empty, AttackerSquad, Attacker);
                }

                ActivateAutomaticSkills(ListPlayer[Result.TargetPlayerIndex].ListSquad[Result.TargetSquadIndex], Result.Target, string.Empty, null, Result.Target);

                //Will Gains
                if (Result.Target.HP <= 0)
                {
                    ListDeadDefender.Add(Result.Target);

                    if (AttackerPlayerIndex == 0)
                    {
                        int Money = 500;
                        ListPlayer[AttackerPlayerIndex].Records.CurrentMoney += (uint)(Money * Attacker.Boosts.MoneyMultiplier);
                    }

                    if (AttackerSquad != null)//Can get hurt by the environment
                    {
                        BattleRecap = new LevelUpMenu(this, Attacker.Pilot, Attacker, AttackerSquad, ListPlayer[AttackerPlayerIndex].IsPlayerControlled);
                        BattleRecap.TotalExpGained += (int)((Result.Target.Pilot.EXPValue + Result.Target.UnitStat.EXPValue) * Attacker.Boosts.EXPMultiplier);

                        FinalizeDeath(Attacker, AttackerSquad, AttackerPlayerIndex, Result.Target, DefenderSquad, DefenderPlayerIndex);
                    }
                }
                else if (Result.AttackMissed)
                {
                    if (AttackerSquad != null)//Can get hurt by the environment
                    {
                        for (int C = 0; C < Attacker.ArrayCharacterActive.Length; C++)
                        {
                            Attacker.ArrayCharacterActive[C].Will += Attacker.ArrayCharacterActive[C].Personality.WillGainMissedEnemy;
                        }
                    }

                    for (int C = 0; C < Result.Target.ArrayCharacterActive.Length; C++)
                    {
                        Result.Target.ArrayCharacterActive[C].Will += Result.Target.ArrayCharacterActive[C].Personality.WillGainEvaded;
                    }
                }
                else if (!Result.AttackMissed)
                {
                    if (AttackerSquad != null)//Can get hurt by the environment
                    {
                        for (int C = 0; C < Attacker.ArrayCharacterActive.Length; C++)
                        {
                            Attacker.ArrayCharacterActive[C].Will += Attacker.ArrayCharacterActive[C].Personality.WillGainHitEnemy;
                        }
                    }

                    for (int C = 0; C < Result.Target.ArrayCharacterActive.Length; C++)
                    {
                        Result.Target.ArrayCharacterActive[C].Will += Result.Target.ArrayCharacterActive[C].Personality.WillGainGotHit;
                    }
                }
            }

            return BattleRecap;
        }

        private void FinalizeDeath(Unit Attacker, Squad AttackerSquad, int AttackerPlayerIndex,
                                   Unit DeadDefender, Squad DefenderSquad, int DefenderPlayerIndex)
        {
            //Unit killed.
            //Every allies gain morale.
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                if (ListPlayer[P].Team == ListPlayer[AttackerPlayerIndex].Team)
                {
                    for (int U = 0; U < ListPlayer[P].ListSquad.Count; U++)
                    {
                        if (ListPlayer[P].ListSquad[U].CurrentLeader == null || ListPlayer[P].ListSquad[U] == AttackerSquad)
                            continue;

                        for (int C = 0; C < ListPlayer[P].ListSquad[U].CurrentLeader.ArrayCharacterActive.Length; C++)
                        {
                            ListPlayer[P].ListSquad[U].CurrentLeader.ArrayCharacterActive[C].Will += 1;
                        }
                    }
                }
                else if (ListPlayer[P].Team == ListPlayer[DefenderPlayerIndex].Team)
                {
                    for (int U = 0; U < ListPlayer[P].ListSquad.Count; U++)
                    {
                        if (ListPlayer[P].ListSquad[U].CurrentLeader == null)
                            continue;

                        for (int C = 0; C < ListPlayer[P].ListSquad[U].CurrentLeader.ArrayCharacterActive.Length; C++)
                        {
                            ListPlayer[P].ListSquad[U].CurrentLeader.ArrayCharacterActive[C].Will += ListPlayer[P].ListSquad[U].CurrentLeader.ArrayCharacterActive[C].Personality.WillGainAlliedUnitDestroyed;
                        }
                    }
                }
            }

            for (int C = 0; C < Attacker.ArrayCharacterActive.Length; C++)
            {
                Attacker.ArrayCharacterActive[C].Will += Attacker.ArrayCharacterActive[C].Personality.WillGainDestroyedEnemy;
            }

            for (int U = 0; U < AttackerSquad.UnitsAliveInSquad; U++)
            {
                if (Attacker == AttackerSquad[U])
                    continue;

                for (int C = 1; C < AttackerSquad[U].ArrayCharacterActive.Length; C++)
                {
                    AttackerSquad[U].ArrayCharacterActive[C].Will += 2;
                }
            }

            ListPlayer[AttackerPlayerIndex].Records.PlayerUnitRecords.AddUnitKill(Attacker.ID);
            ListPlayer[AttackerPlayerIndex].Records.PlayerUnitRecords.AddCharacterKill(Attacker.Pilot.ID);
        }

        public SquadBattleResult CalculateFinalHP(Squad Attacker, Attack CurrentAttack, Squad SupportAttacker, int AttackerPlayerIndex, FormationChoices AttackerFormationChoice,
            Squad Defender, Squad SupportDefender, int DefenderPlayerIndex, int DefenderSquadIndex, bool ActivateSkills, bool CalculateCritical)
        {
            SquadBattleResult SquadResult = new SquadBattleResult(new BattleResult[Attacker.UnitsAliveInSquad]);

            Squad TargetSquad = Defender;
            if (SupportDefender != null)
            {
                TargetSquad = SupportDefender;
            }

            Params.GlobalContext.Result.SetTarget(-1, -1, -1, null);
            Params.GlobalContext.SupportAttack = null;
            Params.GlobalContext.SupportDefend = null;

            if (SupportAttacker != null)
            {
                Params.GlobalContext.SupportAttack = SupportAttacker.CurrentLeader;
            }
            if (SupportDefender != null)
            {
                Params.GlobalContext.SupportDefend = SupportDefender.CurrentLeader;
            }

            int TotalLeaderDamage = 0;

            if (ActivateSkills)
            {
                if (CurrentAttack != null)
                {
                    TotalLeaderDamage = GetBattleResult(Attacker.CurrentLeader, CurrentAttack, Attacker, 1, DefenderPlayerIndex, DefenderSquadIndex, 0, false, CalculateCritical).AttackDamage;
                }

                ActivateAutomaticSkills(Attacker, Attacker.CurrentLeader, DeathmatchSkillRequirement.BattleStartRequirementName, TargetSquad, TargetSquad.CurrentLeader);
                ActivateAutomaticSkills(TargetSquad, TargetSquad.CurrentLeader, DeathmatchSkillRequirement.BattleStartRequirementName, Attacker, Attacker.CurrentLeader);

                if (AttackerFormationChoice == FormationChoices.Spread)
                {
                    for (int i = 1; i < Attacker.UnitsAliveInSquad && i < TargetSquad.UnitsAliveInSquad; i++)
                    {
                        if (Attacker[i].CurrentAttack != null)
                        {
                            ActivateAutomaticSkills(Attacker, Attacker[i], DeathmatchSkillRequirement.BattleStartRequirementName, TargetSquad, TargetSquad[i]);
                            ActivateAutomaticSkills(TargetSquad, TargetSquad[i], DeathmatchSkillRequirement.BattleStartRequirementName, Attacker, Attacker[i]);
                        }
                    }
                }
                else if (AttackerFormationChoice == FormationChoices.Focused)
                {
                    int DefenderHP = TargetSquad.CurrentLeader.HP;

                    for (int i = 1; i < Attacker.UnitsAliveInSquad; i++)
                    {
                        if (Attacker[i].CurrentAttack != null && DefenderHP >= 0)
                        {
                            TotalLeaderDamage += GetBattleResult(Attacker[i], Attacker[i].CurrentAttack, Attacker, WingmanDamageModifier, DefenderPlayerIndex, DefenderSquadIndex, 0, false, CalculateCritical).AttackDamage;

                            DefenderHP = TargetSquad.CurrentLeader.ComputeRemainingHPAfterDamage(TotalLeaderDamage);

                            ActivateAutomaticSkills(Attacker, Attacker[i], DeathmatchSkillRequirement.BattleStartRequirementName, TargetSquad, TargetSquad.CurrentLeader);
                            ActivateAutomaticSkills(TargetSquad, TargetSquad.CurrentLeader, DeathmatchSkillRequirement.BattleStartRequirementName, Attacker, Attacker[i]);
                        }
                    }
                }
                else if (AttackerFormationChoice == FormationChoices.ALL)
                {
                    for (int i = 1; i < TargetSquad.UnitsAliveInSquad; i++)
                    {
                        ActivateAutomaticSkills(Attacker, Attacker.CurrentLeader, DeathmatchSkillRequirement.BattleStartRequirementName, TargetSquad, TargetSquad[i]);
                        ActivateAutomaticSkills(TargetSquad, TargetSquad[i], DeathmatchSkillRequirement.BattleStartRequirementName, Attacker, Attacker.CurrentLeader);
                    }
                }

                if (SupportAttacker != null && TargetSquad.CurrentLeader.ComputeRemainingHPAfterDamage(TotalLeaderDamage) > 0)
                {
                    ActivateAutomaticSkills(SupportAttacker, SupportAttacker.CurrentLeader, DeathmatchSkillRequirement.BattleStartRequirementName, TargetSquad, TargetSquad.CurrentLeader);
                    ActivateAutomaticSkills(TargetSquad, TargetSquad.CurrentLeader, DeathmatchSkillRequirement.BattleStartRequirementName, SupportAttacker, SupportAttacker.CurrentLeader);
                }

                if (SupportDefender != null)
                {
                    ActivateAutomaticSkills(SupportDefender, SupportDefender.CurrentLeader, DeathmatchSkillRequirement.BattleStartRequirementName, Attacker, null);
                    ActivateAutomaticSkills(SupportDefender, SupportDefender.CurrentLeader, DeathmatchSkillRequirement.SupportDefendRequirementName, Attacker, null);
                }
            }

            if (CurrentAttack != null)
            {
                SquadResult.ArrayResult[0] = GetBattleResult(Attacker.CurrentLeader, CurrentAttack, Attacker, 1, DefenderPlayerIndex, DefenderSquadIndex, 0, ActivateSkills, CalculateCritical);
            }

            TotalLeaderDamage = SquadResult.ArrayResult[0].AttackDamage;

            if (AttackerFormationChoice == FormationChoices.Spread)
            {
                for (int i = 1; i < Attacker.UnitsAliveInSquad && i < TargetSquad.UnitsAliveInSquad; i++)
                {
                    if (Attacker[i].CurrentAttack != null)
                    {
                        SquadResult.ArrayResult[i] = GetBattleResult(Attacker[i], Attacker[i].CurrentAttack, Attacker, WingmanDamageModifier, DefenderPlayerIndex, DefenderSquadIndex, i, ActivateSkills, CalculateCritical);
                    }
                }
            }
            else if (AttackerFormationChoice == FormationChoices.Focused)
            {
                int DefenderHP = TargetSquad.CurrentLeader.HP;

                for (int i = 1; i < Attacker.UnitsAliveInSquad; i++)
                {
                    if (Attacker[i].CurrentAttack != null && DefenderHP >= 0)
                    {
                        SquadResult.ArrayResult[i] = GetBattleResult(Attacker[i], Attacker[i].CurrentAttack, Attacker, WingmanDamageModifier, DefenderPlayerIndex, DefenderSquadIndex, 0, ActivateSkills, CalculateCritical);

                        TotalLeaderDamage += SquadResult.ArrayResult[i].AttackDamage;

                        DefenderHP = TargetSquad.CurrentLeader.ComputeRemainingHPAfterDamage(SquadResult.ArrayResult[i].AttackDamage);
                    }
                }
            }
            else if (AttackerFormationChoice == FormationChoices.ALL)
            {
                for (int i = 1; i < TargetSquad.UnitsAliveInSquad; i++)
                {
                    SquadResult.ArrayResult[i] = GetBattleResult(Attacker.CurrentLeader, CurrentAttack, Attacker, 1, DefenderPlayerIndex, DefenderSquadIndex, i, ActivateSkills, CalculateCritical);
                }
            }

            if (SupportAttacker != null && TargetSquad.CurrentLeader.ComputeRemainingHPAfterDamage(TotalLeaderDamage) > 0)
            {
                if (ActivateSkills)
                {
                    ActivateAutomaticSkills(SupportAttacker, SupportAttacker.CurrentLeader, DeathmatchSkillRequirement.BattleStartRequirementName, TargetSquad, null);
                    ActivateAutomaticSkills(SupportAttacker, SupportAttacker.CurrentLeader, DeathmatchSkillRequirement.SupportAttackRequirementName, TargetSquad, null);
                }
                
                SquadResult.ResultSupportAttack = GetBattleResult(SupportAttacker.CurrentLeader, SupportAttacker.CurrentLeader.CurrentAttack, SupportAttacker, 0.75f, DefenderPlayerIndex, DefenderSquadIndex, 0, ActivateSkills, CalculateCritical);
            }

            if (ActivateSkills)
            {
                UpdateMapEvent(EventTypeOnBattle, 0);
            }

            return SquadResult;
        }
    }
}
