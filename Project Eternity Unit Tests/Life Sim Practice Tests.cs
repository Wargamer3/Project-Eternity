using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimPracticeTests
    {
        [TestMethod]
        public void DiceRollFromOneThroughTheNumberOfFaces()
        {
            DiceRoll One = new DiceRoll("1d6");
            for (int i = 0; i < 200; i++)
            {
                int Roll = One.Roll();
                Assert.IsTrue(Roll >= 1 && Roll <= 6);
            }

            DiceRoll Two = new DiceRoll("2d6");
            for (int i = 0; i < 200; i++)
            {
                int Roll = Two.Roll();
                Assert.IsTrue(Roll >= 2 && Roll <= 12);
            }
        }

        [TestMethod]
        public void DegreesOfSuccessFollowTheArmorClass()
        {
            Assert.AreEqual(StrikeDegree.Success, PracticeMath.Degree(10, 5, 0, 15));
            Assert.AreEqual(StrikeDegree.CriticalSuccess, PracticeMath.Degree(18, 7, 0, 15));
            Assert.AreEqual(StrikeDegree.Failure, PracticeMath.Degree(6, 0, 0, 15));
            Assert.AreEqual(StrikeDegree.CriticalFailure, PracticeMath.Degree(5, 0, 0, 15));
            Assert.AreEqual(StrikeDegree.CriticalSuccess, PracticeMath.Degree(20, 0, 0, 20));
            Assert.AreEqual(StrikeDegree.Failure, PracticeMath.Degree(20, 0, 0, 30));
            Assert.AreEqual(StrikeDegree.Failure, PracticeMath.Degree(1, 20, 0, 15));
            Assert.AreEqual(StrikeDegree.Success, PracticeMath.Degree(1, 30, 0, 15));
        }

        [TestMethod]
        public void MultipleAttackPenaltyGrowsThenStops()
        {
            Assert.AreEqual(0, PracticeMath.MapPenalty(0));
            Assert.AreEqual(5, PracticeMath.MapPenalty(1));
            Assert.AreEqual(10, PracticeMath.MapPenalty(2));
            Assert.AreEqual(10, PracticeMath.MapPenalty(4));
            Assert.AreEqual(StrikeDegree.Success, PracticeMath.Degree(15, 5, PracticeMath.MapPenalty(1), 15));
            Assert.AreEqual(StrikeDegree.Failure, PracticeMath.Degree(15, 5, PracticeMath.MapPenalty(2), 15));
        }

        [TestMethod]
        public void StrikeDamageUsesTheWeaponTextAndDoublesACritical()
        {
            Assert.AreEqual(13, PracticeMath.Damage("1d8+1", 4, true, false, new int[] { 8 }));
            Assert.AreEqual(26, PracticeMath.Damage("1d8+1", 4, true, true, new int[] { 8 }));
            Assert.AreEqual(4, PracticeMath.Damage("1d8", 4, false, false, new int[] { 4 }));
            Assert.AreEqual(6, PracticeMath.Damage(null, 2, true, false, new int[] { 4 }));
            Assert.AreEqual(0, PracticeMath.Damage("1d4-3", 1, true, false, new int[] { 1 }));
        }

        [TestMethod]
        public void AttackBonusUsesStrengthOrDexterityAndWeaponRank()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.STR = 3;
            Who.DEX = 2;
            Who.Level = 2;
            Weapon Sword = new Weapon();
            Sword.Name = "Longsword";
            Sword.WeaponType = Weapon.WeaponTypes.Melee;
            Sword.WeaponGroup = Weapon.WeaponGroups.Sword;
            Assert.AreEqual(3, PracticeMath.AttackBonus(Who, Sword));

            Who.DicProficiencyLevelByName.Add("Perception", Ranked("Perception", Proficiency.ProficiencyRanks.Legendary));
            Assert.AreEqual(3, PracticeMath.AttackBonus(Who, Sword));

            Who.DicProficiencyLevelByName.Add("Weapons/Sword", Ranked("Sword", Proficiency.ProficiencyRanks.Trained));
            Assert.AreEqual(7, PracticeMath.AttackBonus(Who, Sword));

            Weapon Bow = new Weapon();
            Bow.Name = "Bow";
            Bow.WeaponType = Weapon.WeaponTypes.Range;
            Bow.WeaponGroup = Weapon.WeaponGroups.Firearm;
            Who.Level = 1;
            Who.DicProficiencyLevelByName.Add("Firearm", Ranked("Firearm", Proficiency.ProficiencyRanks.Expert));
            Assert.AreEqual(7, PracticeMath.AttackBonus(Who, Bow));
        }

        [TestMethod]
        public void StrideBudgetUsesTheWalkedPath()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Assert.AreEqual(25, PracticeMath.LandSpeedFeet(Who));
            Who.Ancestry.Speed = 25;
            float Budget = PracticeMath.StrideUnits(Who);
            Assert.AreEqual(25f * PracticeMath.FeetToMeters * PracticeMath.UnitsPerMeter, Budget, 0.01f);
            PathSegment Segment = new PathSegment();
            Segment.Polyline.Add(Vector3.Zero);
            Segment.Polyline.Add(new Vector3(32f, 0f, 0f));
            FollowPath Path = new FollowPath(Who, new List<PathSegment> { Segment });
            List<AutomatedAction> Steps = new List<AutomatedAction>();
            Steps.Add(Path);
            Assert.IsTrue(PracticeMath.StrideFits(PracticeMath.PathLength(Steps), Budget));

            Segment.Polyline.Add(new Vector3(Budget + 32f, 0f, 0f));
            Assert.IsFalse(PracticeMath.StrideFits(PracticeMath.PathLength(Steps), Budget));
        }

        [TestMethod]
        public void UpdateStatsAppliesConstitutionAndDexterity()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.Ancestry.BaseHP = 8;
            Who.Class.BaseHP = 6;
            Who.Level = 4;
            StatsToAsign Boost = new StatsToAsign();
            Boost.ArrayStatToChoose = new PlayerCharacter.CharacterStats[]
            {
                PlayerCharacter.CharacterStats.STR,
                PlayerCharacter.CharacterStats.DEX,
                PlayerCharacter.CharacterStats.CON,
            };
            Who.Ancestry.ListStatBoosts.Add(Boost);
            ProficiencyLink Sword = Ranked("Sword", Proficiency.ProficiencyRanks.Trained);
            Sword.LinkedProficiency.ListStatModifier.Add(PlayerCharacter.CharacterStats.STR);
            Who.Class.DicProficiencyLevelByName.Add("Sword", Sword);
            Who.UpdateStats();

            Assert.AreEqual(1, Who.STR);
            Assert.AreEqual(1, Who.DEX);
            Assert.AreEqual(1, Who.CON);
            Assert.AreEqual(15, Who.MaxHP);
            Assert.AreEqual(11, Who.AC);
            Assert.AreEqual(7, Sword.GetValue());
        }

        [TestMethod]
        public void InitFillsCurrentHitPointsOnce()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.Ancestry.BaseHP = 5;
            Who.Class.BaseHP = 5;
            Who.Init();
            Assert.AreEqual(10, Who.MaxHP);
            Assert.AreEqual(10, Who.CurrentHP);

            Who.CurrentHP = 4;
            Who.Init();
            Assert.AreEqual(4, Who.CurrentHP);
        }

        [TestMethod]
        public void AStrikeRollsAgainstTheDummyAndEndsTheTurnCleanly()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.STR = 3;
            Weapon Sword = new Weapon();
            Sword.Damage = "1d1";
            Sword.WeaponType = Weapon.WeaponTypes.Melee;
            Sword.FarRange = 2f;
            Who.Equip(Sword);
            FurnitureItem Dummy = new FurnitureItem();
            Dummy.Position = Who.Position;
            Dummy.Width = 1f;
            Dummy.Length = 1f;
            PracticeEncounter Fight = new PracticeEncounter(Who, Dummy);

            Assert.IsFalse(Fight.Strike(20));
            Assert.AreEqual(1, Who.Logs.ListLog.Count);
            Assert.IsTrue(Who.Logs.ListLog[0].TextLogged.Contains("d20 20"));
            Assert.IsTrue(Who.Logs.ListLog[0].TextLogged.Contains("damage") || Who.Logs.ListLog[0].TextLogged.Contains("DiceResult"));
            Assert.AreEqual(2, Fight.ActionsLeft);
            Assert.AreEqual(1, Fight.StrikesThisTurn);
            Assert.AreEqual(22, Fight.HP);

            Assert.IsFalse(Fight.Strike(10));
            Assert.AreEqual(22, Fight.HP);
            Assert.AreEqual(1, Fight.ActionsLeft);

            Fight.EndTurn();
            Assert.AreEqual(3, Fight.ActionsLeft);
            Assert.AreEqual(0, Fight.StrikesThisTurn);

            Dummy.Position = new Vector3(PracticeMath.WeaponReach(Sword) + Dummy.Width, 0f, 0f);
            Assert.IsFalse(Fight.Strike(18));
            Assert.AreEqual(3, Fight.ActionsLeft);
            Assert.AreEqual("Out of reach.", Fight.LastResult);

            Who.GiveOrder("Wait", new List<AutomatedAction> { new SleepInBedAction(Who) });
            Assert.IsFalse(Fight.Strike(18));
            Assert.AreEqual("Still moving.", Fight.LastResult);

            Who.ListAIObjective.Clear();
            Sword.Damage = "40";
            Dummy.Position = Who.Position;
            Assert.IsTrue(Fight.Strike(20));
            Assert.AreEqual(0, Fight.HP);
        }

        [TestMethod]
        public void CombatDummyIsOnTheMiscellaneousPage()
        {
            CatalogItem Card = BuildCatalog.Find(PracticeEncounter.Interaction);
            Assert.IsNotNull(Card);
            Assert.AreEqual(CatalogPage.Miscellaneous, Card.Page);
        }

        [TestMethod]
        public void ALowerPenaltyOverrideBeatsTheDefaultAndAnotherTrait()
        {
            Weapon Sword = new Weapon();
            Sword.Damage = "1d8";
            Sword.WeaponType = Weapon.WeaponTypes.Melee;
            PenaltyOverrideEffect Agile = new PenaltyOverrideEffect();
            Give(Sword, Agile);

            StrikeRoll Second = PracticeMath.Prepare(null, Sword, 1, 1, false, 0f);
            PracticeMath.AlterAttack(Second);
            Assert.AreEqual(4, Second.UntypedPenalty);

            PenaltyOverrideEffect Better = new PenaltyOverrideEffect();
            Better.Second = 3;
            Better.Later = 6;
            Give(Sword, Agile, Better);
            Second = PracticeMath.Prepare(null, Sword, 1, 1, false, 0f);
            PracticeMath.AlterAttack(Second);
            Assert.AreEqual(3, Second.UntypedPenalty);

            StrikeRoll Later = PracticeMath.Prepare(null, Sword, 2, 2, false, 0f);
            PracticeMath.AlterAttack(Later);
            Assert.AreEqual(6, Later.UntypedPenalty);
            Assert.AreEqual(10, PracticeMath.MapPenalty(2));
        }

        [TestMethod]
        public void FinesseUsesDexterityForTheAttackAndStrengthForTheDamage()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.STR = 0;
            Who.DEX = 3;
            Weapon Sword = new Weapon();
            Sword.Damage = "1d6";
            Sword.WeaponType = Weapon.WeaponTypes.Melee;
            Give(Sword, new FinesseEffect());

            StrikeRoll Roll = PracticeMath.Prepare(Who, Sword, 0, 0, false, 0f);
            PracticeMath.AlterAttack(Roll);
            Assert.AreEqual(3, Roll.AttackAbility);
            Assert.AreEqual(0, Roll.DamageAbility);
            Assert.AreEqual(3, PracticeMath.AttackBonus(Who, Sword, Roll.AttackAbility));
            Assert.AreEqual(0, Who.STR);
            Assert.AreEqual(3, Who.DEX);
            Assert.AreEqual(4, PracticeMath.ResolveDamage(Roll, new int[] { 4 }, null));
        }

        [TestMethod]
        public void DeadlyAddsADieAfterTheCriticalDouble()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.STR = 4;
            Weapon Sword = new Weapon();
            Sword.Damage = "1d6";
            Sword.WeaponType = Weapon.WeaponTypes.Melee;
            Give(Sword, new DeadlyEffect());

            StrikeRoll Roll = PracticeMath.Prepare(Who, Sword, 0, 0, false, 0f);
            Roll.Critical = true;
            PracticeMath.AlterDamage(Roll);
            Assert.AreEqual(1, Roll.ExtraDiceCount);
            Assert.AreEqual(8, Roll.ExtraFaces);
            Assert.AreEqual(28, PracticeMath.ResolveDamage(Roll, new int[] { 6 }, new int[] { 8 }));
        }

        [TestMethod]
        public void FatalGrowsTheDieAndAddsOneDieBeforeTheDouble()
        {
            Weapon Sword = new Weapon();
            Sword.Damage = "1d6";
            Sword.WeaponType = Weapon.WeaponTypes.Melee;
            FatalEffect Larger = new FatalEffect();
            FatalEffect Smaller = new FatalEffect();
            Smaller.Faces = 10;
            Give(Sword, Larger, Smaller);

            StrikeRoll Roll = PracticeMath.Prepare(null, Sword, 0, 0, false, 0f);
            Roll.Critical = true;
            PracticeMath.AlterDamage(Roll);
            Assert.AreEqual(12, Roll.DiceFaces);
            Assert.AreEqual(2, Roll.DiceCount);
            Assert.AreEqual(0, Roll.ExtraDiceCount);
            Assert.AreEqual(48, PracticeMath.ResolveDamage(Roll, new int[] { 12, 12 }, null));
        }

        [TestMethod]
        public void PropulsiveAddsHalfStrengthAndAFullPenalty()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.STR = 3;
            Weapon Bow = new Weapon();
            Bow.Damage = "1d6";
            Bow.WeaponType = Weapon.WeaponTypes.Range;
            Give(Bow, new PropulsiveEffect());

            StrikeRoll Roll = PracticeMath.Prepare(Who, Bow, 0, 0, false, 0f);
            PracticeMath.AlterDamage(Roll);
            Assert.AreEqual(1, Roll.DamageAbility);

            Who.STR = -3;
            Roll = PracticeMath.Prepare(Who, Bow, 0, 0, false, 0f);
            PracticeMath.AlterDamage(Roll);
            Assert.AreEqual(-3, Roll.DamageAbility);
            Roll.Critical = true;
            Assert.AreEqual(2, PracticeMath.ResolveDamage(Roll, new int[] { 4 }, null));
        }

        [TestMethod]
        public void ForcefulAddsOneDamagePerDieOnTheSecondStrike()
        {
            Weapon Sword = new Weapon();
            Sword.Damage = "1d8";
            Sword.WeaponType = Weapon.WeaponTypes.Melee;
            Give(Sword, new ForcefulEffect());

            StrikeRoll Roll = PracticeMath.Prepare(null, Sword, 1, 1, false, 0f);
            PracticeMath.AlterDamage(Roll);
            Assert.AreEqual(1, Roll.ExtraDamage);
            Assert.AreEqual(6, PracticeMath.ResolveDamage(Roll, new int[] { 5 }, null));
        }

        [TestMethod]
        public void BackswingBonusesTheNextAttackAfterAMiss()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Weapon Sword = new Weapon();
            Sword.Name = "Sword";
            Sword.Damage = "1";
            Sword.WeaponType = Weapon.WeaponTypes.Melee;
            Sword.FarRange = 2f;
            Give(Sword, new BackswingEffect());
            Who.Equip(Sword);
            FurnitureItem Dummy = new FurnitureItem();
            Dummy.Position = Who.Position;
            Dummy.Width = 1f;
            Dummy.Length = 1f;
            PracticeEncounter Fight = new PracticeEncounter(Who, Dummy);

            Assert.IsFalse(Fight.Strike(1));
            Assert.IsFalse(Who.Logs.ListLog[0].TextLogged.Contains(" + 1 "));

            Assert.IsFalse(Fight.Strike(20));
            Assert.IsTrue(Who.Logs.ListLog[1].TextLogged.Contains(" + 1 "));

            Assert.IsFalse(Fight.Strike(20));
            Assert.IsFalse(Who.Logs.ListLog[2].TextLogged.Contains(" + 1 "));
        }

        [TestMethod]
        public void VolleyAndKickbackKeepTheWorseCircumstancePenalty()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Weapon Bow = new Weapon();
            Bow.WeaponType = Weapon.WeaponTypes.Range;
            VolleyEffect Volley = new VolleyEffect();
            KickbackEffect Kickback = new KickbackEffect();
            Kickback.Penalty = 5;
            Give(Bow, Volley);

            StrikeRoll Roll = PracticeMath.Prepare(Who, Bow, 0, 0, false, 0f);
            PracticeMath.AlterAttack(Roll);
            Assert.AreEqual(2, Roll.CircumstancePenalty);

            Give(Bow, Kickback);
            Roll = PracticeMath.Prepare(Who, Bow, 0, 0, false, 0f);
            PracticeMath.AlterAttack(Roll);
            Assert.AreEqual(5, Roll.CircumstancePenalty);

            Give(Bow, Volley, Kickback);
            Roll = PracticeMath.Prepare(Who, Bow, 0, 0, false, 0f);
            PracticeMath.AlterAttack(Roll);
            Assert.AreEqual(5, Roll.CircumstancePenalty);

            float Outside = 30f * PracticeMath.FeetToMeters * PracticeMath.UnitsPerMeter + 1f;
            Roll = PracticeMath.Prepare(Who, Bow, 0, 0, false, Outside);
            PracticeMath.AlterAttack(Roll);
            Assert.AreEqual(5, Roll.CircumstancePenalty);

            Who.STR = 1;
            Give(Bow, Kickback);
            Roll = PracticeMath.Prepare(Who, Bow, 0, 0, false, 0f);
            PracticeMath.AlterAttack(Roll);
            Assert.AreEqual(0, Roll.CircumstancePenalty);
        }

        [TestMethod]
        public void TwoHandGrowsTheDamageDie()
        {
            Weapon Sword = new Weapon();
            Sword.Damage = "1d8";
            Sword.HandType = Weapon.HandTypes.TwoHands;
            Sword.WeaponType = Weapon.WeaponTypes.Melee;
            Give(Sword, new TwoHandEffect());

            StrikeRoll Roll = PracticeMath.Prepare(null, Sword, 0, 0, false, 0f);
            PracticeMath.AlterDamage(Roll);
            Assert.AreEqual(12, Roll.DiceFaces);
            Assert.AreEqual(1, Roll.DiceCount);

            Sword.HandType = Weapon.HandTypes.OneHand;
            Roll = PracticeMath.Prepare(null, Sword, 0, 0, false, 0f);
            PracticeMath.AlterDamage(Roll);
            Assert.AreEqual(8, Roll.DiceFaces);
        }

        [TestMethod]
        public void BackstabberAddsPrecisionDamageOnlyWhenOffGuard()
        {
            Weapon Sword = new Weapon();
            Sword.Damage = "1d4";
            Sword.WeaponType = Weapon.WeaponTypes.Melee;
            Give(Sword, new BackstabberEffect());

            StrikeRoll Roll = PracticeMath.Prepare(null, Sword, 0, 0, false, 0f);
            PracticeMath.AlterDamage(Roll);
            Assert.AreEqual(0, Roll.ExtraDamage);
            Assert.AreEqual(2, PracticeMath.ResolveDamage(Roll, new int[] { 2 }, null));

            Roll.OffGuard = true;
            PracticeMath.AlterDamage(Roll);
            Assert.AreEqual(1, Roll.ExtraDamage);
            Assert.AreEqual(3, PracticeMath.ResolveDamage(Roll, new int[] { 2 }, null));

            Roll.ExtraDamage = 0;
            Roll.Potency = 3;
            PracticeMath.AlterDamage(Roll);
            Assert.AreEqual(2, Roll.ExtraDamage);
        }

        [TestMethod]
        public void VersatileSetsTheDamageTypeOnlyWhenChosen()
        {
            Weapon Sword = new Weapon();
            Sword.Damage = "1d6";
            Sword.WeaponType = Weapon.WeaponTypes.Melee;
            VersatileEffect Versatile = new VersatileEffect();
            Assert.AreEqual(VersatileEffect.DamageTypes.Slashing, Versatile.DamageType);
            Give(Sword, Versatile);

            StrikeRoll Offered = PracticeMath.Prepare(null, Sword, 0, 0, false, 0f);
            PracticeMath.AlterAttack(Offered);
            Assert.AreEqual("Slashing", Offered.VersatileType);
            Assert.IsTrue(string.IsNullOrEmpty(Offered.DamageType));

            StrikeRoll Chosen = PracticeMath.Prepare(null, Sword, 0, 0, false, 0f);
            Chosen.RequestedType = "Slashing";
            PracticeMath.AlterAttack(Chosen);
            Assert.AreEqual("Slashing", Chosen.DamageType);

            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.STR = 4;
            Sword.FarRange = 2f;
            Who.Equip(Sword);
            FurnitureItem Dummy = new FurnitureItem();
            Dummy.Position = Who.Position;
            Dummy.Width = 1f;
            Dummy.Length = 1f;
            PracticeEncounter Fight = new PracticeEncounter(Who, Dummy);
            Fight.Strike(15, "Slashing");
            Assert.IsTrue(Who.Logs.ListLog[0].TextLogged.Contains("Slashing"));
            Fight.Strike(15);
            Assert.IsFalse(Who.Logs.ListLog[1].TextLogged.Contains("Slashing"));
        }

        [TestMethod]
        public void ATraitRoundTripKeepsThePenaltyOverride()
        {
            Trait Source = new Trait();
            Source.Name = "Agile";
            Source.Description = "Light";
            PenaltyOverrideEffect Agile = new PenaltyOverrideEffect();
            Agile.Second = 3;
            Agile.Later = 6;
            Source.ListEffect.Add(Agile);
            TraitEffect.Register(new PenaltyOverrideEffect());

            MemoryStream Stream = new MemoryStream();
            BinaryWriter Writer = new BinaryWriter(Stream, Encoding.UTF8, true);
            Source.Write(Writer);
            Writer.Flush();
            Stream.Position = 0;
            Trait Loaded = new Trait();
            Loaded.Read(new BinaryReader(Stream, Encoding.UTF8, true));
            PenaltyOverrideEffect Read = (PenaltyOverrideEffect)Loaded.ListEffect[0];
            Assert.AreEqual(3, Read.Second);
            Assert.AreEqual(6, Read.Later);

            MemoryStream Old = new MemoryStream();
            BinaryWriter OldWriter = new BinaryWriter(Old, Encoding.UTF8, true);
            OldWriter.Write("Old");
            OldWriter.Write("Plain");
            OldWriter.Flush();
            Old.Position = 0;
            Trait Plain = new Trait();
            Plain.Read(new BinaryReader(Old, Encoding.UTF8, true));
            Assert.AreEqual("Old", Plain.Name);
            Assert.AreEqual(0, Plain.ListEffect.Count);
        }

        [TestMethod]
        public void TheDummySwingsWhenTheTurnEndsAndDealsNoHitPoints()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.DEX = 2;
            Who.CurrentHP = 20;
            FurnitureItem Dummy = new FurnitureItem();
            Dummy.Position = Who.Position;
            Dummy.Width = 1f;
            Dummy.Length = 1f;
            PracticeEncounter Fight = new PracticeEncounter(Who, Dummy);

            Fight.EndTurn(7);
            Assert.AreEqual(20, Who.CurrentHP);
            Assert.AreEqual(0, Fight.CircumstanceArmor);
            Assert.AreEqual(3, Fight.ActionsLeft);
            Assert.AreEqual(6, Fight.LastIncoming);
            Assert.IsTrue(Fight.LastResult.Contains("vs AC 12"));
            Assert.IsTrue(Fight.LastResult.Contains("success"));
            Assert.IsTrue(Fight.LastResult.Contains("6 Bludgeoning damage"));
            Assert.IsTrue(Fight.LastResult.Contains("You take 0."));
            Assert.IsTrue(Who.Logs.ListLog[0].TextLogged.Contains("{{DiceResult:6}}"));

            Fight.CircumstanceArmor = 1;
            Fight.EndTurn(7);
            Assert.AreEqual(20, Who.CurrentHP);
            Assert.AreEqual(0, Fight.LastIncoming);
            Assert.IsTrue(Fight.LastResult.Contains("vs AC 13"));
            Assert.IsTrue(Fight.LastResult.Contains("failure"));

            Dummy.Position = new Vector3(PracticeMath.WeaponReach(null) + Dummy.Width + 1f, 0f, 0f);
            Fight.EndTurn(15);
            Assert.AreEqual(20, Who.CurrentHP);
            Assert.AreEqual(0, Fight.LastIncoming);
            Assert.AreEqual("The dummy is out of reach. Turn ended. 3 actions.", Fight.LastResult);
        }

        static void Give(Weapon Weapon, params TraitEffect[] Effects)
        {
            Trait Trait = new Trait();
            Trait.Name = "Test";
            for (int i = 0; i < Effects.Length; i++)
            {
                Trait.ListEffect.Add(Effects[i]);
            }

            Weapon.ListTraits = new List<Trait>();
            Weapon.ListTraits.Add(Trait);
        }

        static ProficiencyLink Ranked(string Name, Proficiency.ProficiencyRanks Rank)
        {
            Proficiency Body = new Proficiency();
            Body.Name = Name;
            ProficiencyLink Link = new ProficiencyLink(Name);
            Link.LinkedProficiency = Body;
            Link.ProficiencyRank = Rank;
            return Link;
        }
    }
}
