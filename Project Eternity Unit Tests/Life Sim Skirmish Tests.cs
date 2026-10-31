using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimSkirmishTests
    {
        [TestMethod]
        public void LevelTenHitPointsAndArmorUseTheGearFormula()
        {
            Assert.AreEqual(108, PracticeMath.LevelHitPoints(8, 8, 2, 10));
            Assert.AreEqual(19, PracticeMath.GearArmorClass(5, 4, 1, 4, 0));
            Assert.AreEqual(5, PracticeMath.CantripRank(10));
            float Sixty = PracticeMath.ToUnits(60f);
            Assert.AreEqual(0, PracticeMath.RangeCircumstancePenalty(Sixty, 60f, false));
            Assert.AreEqual(2, PracticeMath.RangeCircumstancePenalty(Sixty + 1f, 60f, false));
            Assert.AreEqual(0, PracticeMath.RangeCircumstancePenalty(Sixty + 1f, 60f, true));
        }

        [TestMethod]
        public void AWeaponGroupNameMatchesProficiency()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.Level = 2;
            Who.DicProficiencyLevelByName.Add("Spear", Ranked("Spear", Proficiency.ProficiencyRanks.Expert));
            Weapon Spear = new Weapon();
            Spear.GroupName = "Spear";
            Spear.WeaponType = Weapon.WeaponTypes.Melee;
            Assert.AreEqual(6, PracticeMath.AttackBonus(Who, Spear, 0));
        }

        [TestMethod]
        public void ThePartyAndGoblinsShareOneTurnOrder()
        {
            Skirmish Fight = new Skirmish();
            List<PlayerCharacter> Party = SkirmishSamples.Party(Fight);
            int Heroes = 0;
            for (int i = 0; i < Party.Count; i++)
            {
                Fight.Add(Party[i]);
                if (Party[i].Side == SkirmishSide.Hero && Party[i].Owner == null)
                {
                    Heroes++;
                }
            }

            for (int i = 0; i < 21; i++)
            {
                Fight.Add(SkirmishSamples.Goblin());
            }

            Assert.AreEqual(7, Heroes);
            Assert.AreEqual(29, Fight.Bodies.Count);
            int[] Rolls = new int[Fight.Bodies.Count];
            for (int i = 0; i < Rolls.Length; i++)
            {
                Rolls[i] = 10;
            }

            Fight.Begin(Rolls);
            Assert.AreEqual(SkirmishSide.Hero, Fight.Active.Side);
            Assert.AreEqual(3, Fight.Active.ActionsLeft);
        }

        [TestMethod]
        public void AGoblinChoosesDaggerShortbowOrAStride()
        {
            Skirmish Fight = new Skirmish();
            PlayerCharacter Hero = new PlayerCharacter(null, null, Vector3.Zero);
            Hero.Name = "Hero";
            Hero.Side = SkirmishSide.Hero;
            Hero.CurrentHP = 40;
            Hero.MaxHP = 40;
            Hero.AC = 10;
            PlayerCharacter Goblin = SkirmishSamples.Goblin();
            Goblin.Position = Hero.Position;
            Fight.Add(Hero);
            Fight.Add(Goblin);
            Fight.MonsterAct(Goblin, 10, 6);
            Assert.IsTrue(Fight.LastResult.Contains("Dagger"));
            Assert.AreEqual(34, Hero.CurrentHP);

            Goblin.ActionsLeft = 3;
            Goblin.Position = new Vector3(PracticeMath.ToUnits(30f), 0f, 0f);
            Fight.MonsterAct(Goblin, 18, 6);
            Assert.IsTrue(Fight.LastResult.Contains("Shortbow"));

            Goblin.ActionsLeft = 3;
            Goblin.Position = new Vector3(PracticeMath.ToUnits(80f), 0f, 0f);
            Vector3 Before = Goblin.Position;
            Fight.MonsterAct(Goblin, 18, 1);
            Assert.IsTrue(Fight.LastResult.Contains("strides"));
            Assert.AreNotEqual(Before.X, Goblin.Position.X);
        }

        [TestMethod]
        public void GoblinScuttleStepsWhenAnAllyArrives()
        {
            Skirmish Fight = new Skirmish();
            PlayerCharacter Mover = SkirmishSamples.Goblin();
            PlayerCharacter Ally = SkirmishSamples.Goblin();
            Mover.Position = new Vector3(PracticeMath.ToUnits(25f), 0f, 0f);
            Fight.Add(Mover);
            Fight.Add(Ally);
            Fight.Stride(Mover, Ally.Position);
            Assert.AreEqual(0, Ally.ReactionLeft);
            Assert.IsTrue(Ally.Position.X > 0f);
        }

        [TestMethod]
        public void AgileGraceLowersTheDaggerPenalty()
        {
            Skirmish Fight = new Skirmish();
            List<PlayerCharacter> Party = SkirmishSamples.Party(Fight);
            PlayerCharacter Rogue = Party[0];
            PlayerCharacter Dummy = new PlayerCharacter(null, null, Vector3.Zero);
            Dummy.Name = "Dummy";
            Dummy.Side = SkirmishSide.Monster;
            Dummy.CurrentHP = 80;
            Dummy.MaxHP = 80;
            Dummy.AC = 10;
            Dummy.Position = Rogue.Position;
            Fight.Add(Rogue);
            Fight.Add(Dummy);
            Fight.Strikes.Make(Rogue, Dummy, Rogue.CurrentWeapon(), 15, 4);
            Assert.AreEqual(0, Fight.LastPenalty);
            Fight.Strikes.Make(Rogue, Dummy, Rogue.CurrentWeapon(), 15, 4);
            Assert.AreEqual(3, Fight.LastPenalty);
        }

        [TestMethod]
        public void LightningSwapChangesTheYaoguaiWeapon()
        {
            Skirmish Fight = new Skirmish();
            List<PlayerCharacter> Party = SkirmishSamples.Party(Fight);
            PlayerCharacter Yaoguai = null;
            for (int i = 0; i < Party.Count; i++)
            {
                if (Party[i].Name == "Yaoguai Fighter")
                {
                    Yaoguai = Party[i];
                }
            }

            Assert.AreEqual("Longsword", Yaoguai.Weapon.Name);
            Fight.Actions.LightningSwap(Yaoguai);
            Assert.AreEqual("War Lance", Yaoguai.Weapon.Name);
            Assert.AreEqual("Spear", Yaoguai.Weapon.GroupName);
        }

        [TestMethod]
        public void ARaisedShieldAndNimbleDodgeChangeTheIncomingStrike()
        {
            Skirmish Fight = new Skirmish();
            PlayerCharacter Goblin = SkirmishSamples.Goblin();
            PlayerCharacter Rogue = new PlayerCharacter(null, null, Vector3.Zero);
            Rogue.Name = "Rogue";
            Rogue.Side = SkirmishSide.Hero;
            Rogue.CurrentHP = 30;
            Rogue.MaxHP = 30;
            Rogue.AC = 16;
            Rogue.Feats.Add(SkirmishSamples.Party(Fight)[0].Feats[0]);
            Fight.Add(Goblin);
            Fight.Add(Rogue);
            Fight.Strikes.Make(Goblin, Rogue, Goblin.Weapon, 9, 3);
            Assert.AreEqual(30, Rogue.CurrentHP);
            Assert.AreEqual(0, Rogue.ReactionLeft);
        }

        [TestMethod]
        public void ShieldBlockSubtractsHardness()
        {
            Skirmish Fight = new Skirmish();
            PlayerCharacter Goblin = SkirmishSamples.Goblin();
            PlayerCharacter Champion = new PlayerCharacter(null, null, Vector3.Zero);
            Champion.Name = "Champion";
            Champion.Side = SkirmishSide.Hero;
            Champion.CurrentHP = 30;
            Champion.MaxHP = 30;
            Champion.AC = 10;
            Champion.RaisedShield = true;
            Champion.ShieldHardness = 5;
            Champion.BlockHardness = 5;
            Fight.Add(Goblin);
            Fight.Add(Champion);
            Fight.Strikes.Make(Goblin, Champion, Goblin.Weapon, 10, 6);
            Assert.AreEqual(29, Champion.CurrentHP);
            Assert.IsFalse(Champion.RaisedShield);
        }

        [TestMethod]
        public void CrashingSlamTripsWithoutAnAthleticsRoll()
        {
            Skirmish Fight = new Skirmish();
            List<PlayerCharacter> Party = SkirmishSamples.Party(Fight);
            PlayerCharacter Yaoguai = null;
            for (int i = 0; i < Party.Count; i++)
            {
                if (Party[i].Name == "Yaoguai Fighter")
                {
                    Yaoguai = Party[i];
                }
            }

            PlayerCharacter Goblin = SkirmishSamples.Goblin();
            Goblin.Position = Yaoguai.Position;
            Fight.Add(Yaoguai);
            Fight.Add(Goblin);
            Fight.Actions.SlamDown(Yaoguai, Goblin, 20, 1, 6);
            Assert.IsTrue(Goblin.Prone);
            Assert.IsTrue(Goblin.OffGuard);
        }

        [TestMethod]
        public void SwipeUsesOnePenaltyForBothTargets()
        {
            Skirmish Fight = new Skirmish();
            List<PlayerCharacter> Party = SkirmishSamples.Party(Fight);
            PlayerCharacter Yaoguai = null;
            for (int i = 0; i < Party.Count; i++)
            {
                if (Party[i].Name == "Yaoguai Fighter")
                {
                    Yaoguai = Party[i];
                }
            }

            PlayerCharacter First = SkirmishSamples.Goblin();
            PlayerCharacter Second = SkirmishSamples.Goblin();
            First.Position = Yaoguai.Position;
            Second.Position = Yaoguai.Position;
            Fight.Add(Yaoguai);
            Fight.Add(First);
            Fight.Add(Second);
            Fight.Actions.Swipe(Yaoguai, First, Second, 18, 18, 6);
            Assert.AreEqual(2, Yaoguai.StrikesThisTurn);
            Assert.IsTrue(First.CurrentHP < First.MaxHP);
            Assert.IsTrue(Second.CurrentHP < Second.MaxHP);
        }

        [TestMethod]
        public void ACrossbowReloadsAndFarShotIsAFeat()
        {
            Skirmish Fight = new Skirmish();
            List<PlayerCharacter> Party = SkirmishSamples.Party(Fight);
            PlayerCharacter Ranger = null;
            for (int i = 0; i < Party.Count; i++)
            {
                if (Party[i].Name == "Ranger")
                {
                    Ranger = Party[i];
                }
            }

            PlayerCharacter Goblin = SkirmishSamples.Goblin();
            Goblin.CurrentHP = 40;
            Goblin.MaxHP = 40;
            Goblin.Position = new Vector3(PracticeMath.ToUnits(30f), 0f, 0f);
            Fight.Add(Ranger);
            Fight.Add(Goblin);
            Fight.Actions.HuntPrey(Ranger, Goblin);
            Fight.Strikes.Make(Ranger, Goblin, Ranger.CurrentWeapon(), 18, 4);
            Assert.IsFalse(Ranger.Loaded);
            Fight.Strikes.Make(Ranger, Goblin, Ranger.CurrentWeapon(), 18, 4);
            Assert.AreEqual("Not loaded.", Fight.LastResult);
            Fight.Actions.RunningReload(Ranger, Ranger.Position);
            Assert.IsTrue(Ranger.Loaded);
            Assert.IsTrue(Fight.Actions.HasActivity(Ranger, "Far Shot"));
        }

        [TestMethod]
        public void TheRaptorActsOnTheRangersTurn()
        {
            Skirmish Fight = new Skirmish();
            PlayerCharacter Ranger = SkirmishSamples.Hero("Ranger", 3, 2, 3, 4, 8, 10, 2);
            PlayerCharacter Pet = SkirmishSamples.Raptor();
            Pet.Owner = Ranger;
            Ranger.Companion = Pet;
            Fight.Add(Ranger);
            Fight.Add(Pet);
            Fight.StartTurn(Ranger);
            Assert.AreEqual(0, Pet.ActionsLeft);
            Pet.SetCompanionRank(1);
            Fight.StartTurn(Ranger);
            Assert.AreEqual(1, Pet.ActionsLeft);
            Fight.Actions.Command(Ranger);
            Assert.AreEqual(2, Pet.ActionsLeft);
            Assert.IsTrue(Pet.AC > 16);
        }

        [TestMethod]
        public void PhaseBoltIgnoresShieldCircumstanceAndFearIsAWillSave()
        {
            Skirmish Fight = new Skirmish();
            PlayerCharacter Witch = SkirmishSamples.Hero("Witch", 4, -1, 0, 0, 8, 6, 0);
            Witch.Level = 1;
            Witch.CastingAbility = 6;
            Witch.SpellProficiency = 0;
            Witch.ActionsLeft = 6;
            Witch.Slots[1] = 2;
            PlayerCharacter Goblin = SkirmishSamples.Goblin();
            Goblin.CurrentHP = 40;
            Goblin.MaxHP = 40;
            Goblin.RaisedShield = true;
            Goblin.ShieldBonus = 2;
            Fight.Add(Witch);
            Fight.Add(Goblin);
            SpellCard Needles = Find(SkirmishSamples.WitchSpells(), "Needle Darts");
            SpellCard Phase = Find(SkirmishSamples.WitchSpells(), "Phase Bolt");
            Fight.Spells.Cast(Witch, Needles, Goblin, 10, 1, 4);
            Assert.AreEqual(0, Fight.LastDamage);
            Witch.StrikesThisTurn = 0;
            Fight.Spells.Cast(Witch, Phase, Goblin, 10, 1, 4);
            Assert.AreEqual(12, Fight.LastDamage);

            SpellCard Fear = Find(SkirmishSamples.WitchSpells(), "Fear");
            Goblin.Will = 3;
            Fight.Spells.Cast(Witch, Fear, Goblin, 8, 1, 4);
            Assert.AreEqual(1, Goblin.Frightened);
        }

        [TestMethod]
        public void ForceBarrageGlassShieldAndSummonUseTheSharedEffects()
        {
            Skirmish Fight = new Skirmish();
            Fight.Forms.Add(SkirmishSamples.WolfForm());
            Fight.Summoned.Add(SkirmishSamples.Animal());
            PlayerCharacter Bunny = SkirmishSamples.Hero("Bunny", 4, -1, 0, 0, 8, 6, 0);
            Bunny.Level = 1;
            Bunny.CastingAbility = 4;
            Bunny.SpellProficiency = 4;
            Bunny.ActionsLeft = 12;
            Bunny.Slots[1] = 4;
            Bunny.Slots[2] = 4;
            PlayerCharacter Goblin = SkirmishSamples.Goblin();
            Goblin.CurrentHP = 40;
            Goblin.MaxHP = 40;
            Fight.Add(Bunny);
            Fight.Add(Goblin);
            int Before = Fight.Bodies.Count;
            Fight.Spells.Cast(Bunny, Find(SkirmishSamples.SorcererSpells(), "Force Barrage"), Goblin, 10, 1, 4);
            Assert.AreEqual(15, Fight.LastDamage);
            Fight.Spells.Cast(Bunny, Find(SkirmishSamples.SorcererSpells(), "Summon Animal"), null, 10, 1, 4);
            Assert.AreEqual(Before + 1, Fight.Bodies.Count);
            Fight.Spells.Cast(Bunny, Find(SkirmishSamples.SorcererSpells(), "Animal Form"), Bunny, 10, 2, 4);
            Assert.AreEqual("Wolf", Bunny.Form.Name);
            Assert.AreEqual("Jaws", Bunny.CurrentWeapon().Name);

            PlayerCharacter Witch = SkirmishSamples.Hero("Witch", 4, -1, 0, 0, 8, 6, 0);
            Witch.Level = 10;
            Witch.ActionsLeft = 3;
            Fight.Spells.Cast(Witch, Find(SkirmishSamples.WitchSpells(), "Glass Shield"), Witch, 10, 5, 4);
            Assert.AreEqual(7, Witch.BlockHardness);
            Assert.AreEqual(23, SkirmishSamples.WitchSpells().Count);
            Assert.AreEqual(22, SkirmishSamples.SorcererSpells().Count);
        }

        [TestMethod]
        public void LiveWireDealsElectricityWhenTheAttackFails()
        {
            Skirmish Fight = new Skirmish();
            PlayerCharacter Bunny = SkirmishSamples.Hero("Bunny", 4, -1, 0, 0, 8, 6, 0);
            Bunny.Level = 1;
            Bunny.CastingAbility = 0;
            Bunny.ActionsLeft = 3;
            PlayerCharacter Goblin = SkirmishSamples.Goblin();
            Goblin.CurrentHP = 40;
            Goblin.MaxHP = 40;
            Fight.Add(Bunny);
            Fight.Add(Goblin);
            Fight.Spells.Cast(Bunny, Find(SkirmishSamples.SorcererSpells(), "Live Wire"), Goblin, 10, 1, 4);
            Assert.AreEqual(4, Fight.LastDamage);
            Assert.AreEqual(36, Goblin.CurrentHP);
            Assert.AreEqual(0, Goblin.Persistent.Count);
        }

        [TestMethod]
        public void PersistentDamageTicksAndRousingSplashEndsFire()
        {
            Skirmish Fight = new Skirmish();
            PlayerCharacter Bunny = SkirmishSamples.Hero("Bunny", 4, -1, 0, 0, 8, 6, 0);
            Bunny.Level = 1;
            Bunny.CastingAbility = 6;
            Bunny.SpellProficiency = 0;
            Bunny.ActionsLeft = 9;
            Bunny.Slots[2] = 1;
            PlayerCharacter Goblin = SkirmishSamples.Goblin();
            Goblin.CurrentHP = 40;
            Goblin.MaxHP = 40;
            Fight.Add(Bunny);
            Fight.Add(Goblin);
            Fight.Spells.Cast(Bunny, Find(SkirmishSamples.SorcererSpells(), "Live Wire"), Goblin, 20, 1, 4);
            Assert.AreEqual(16, Fight.LastDamage);
            Assert.AreEqual(1, Goblin.Persistent.Count);
            Assert.AreEqual("Electricity", Goblin.Persistent[0].Type);
            Fight.TickPersistent(Goblin, 10, 4);
            Assert.AreEqual(20, Goblin.CurrentHP);
            Assert.AreEqual(1, Goblin.Persistent.Count);
            Fight.TickPersistent(Goblin, 15, 4);
            Assert.AreEqual(16, Goblin.CurrentHP);
            Assert.AreEqual(0, Goblin.Persistent.Count);

            PlayerCharacter Second = SkirmishSamples.Goblin();
            Second.CurrentHP = 40;
            Second.MaxHP = 40;
            Fight.Add(Second);
            Fight.Spells.Cast(Bunny, Find(SkirmishSamples.SorcererSpells(), "Sticky Fire"), Second, 20, 2, 4);
            Assert.AreEqual(1, Second.Persistent.Count);
            Assert.AreEqual("Fire", Second.Persistent[0].Type);
            Fight.Spells.Cast(Bunny, Find(SkirmishSamples.SorcererSpells(), "Rousing Splash"), Second, 15, 1, 4);
            Assert.AreEqual(0, Second.Persistent.Count);
            Assert.AreEqual(4, Second.TempHP);

            PlayerCharacter Witch = SkirmishSamples.Hero("Witch", 4, -1, 0, 0, 8, 6, 0);
            Witch.Level = 1;
            Witch.CastingAbility = 0;
            Witch.ActionsLeft = 2;
            Witch.Slots[1] = 1;
            PlayerCharacter Third = SkirmishSamples.Goblin();
            Third.CurrentHP = 40;
            Third.MaxHP = 40;
            Fight.Add(Witch);
            Fight.Add(Third);
            Fight.Spells.Cast(Witch, Find(SkirmishSamples.WitchSpells(), "Grim Tendrils"), Third, 1, 1, 4);
            Assert.AreEqual(1, Third.Persistent.Count);
            Assert.AreEqual("Bleed", Third.Persistent[0].Type);
        }

        [TestMethod]
        public void AnimatedAssaultSustainUsesTheSmallerPacket()
        {
            Skirmish Fight = new Skirmish();
            PlayerCharacter Witch = SkirmishSamples.Hero("Witch", 4, -1, 0, 0, 8, 6, 0);
            Witch.Level = 1;
            Witch.CastingAbility = 0;
            Witch.Perception = 10;
            Witch.ActionsLeft = 6;
            Witch.Slots[2] = 1;
            PlayerCharacter Goblin = SkirmishSamples.Goblin();
            Goblin.CurrentHP = 40;
            Goblin.MaxHP = 40;
            Fight.Add(Witch);
            Fight.Add(Goblin);
            Fight.Begin(new int[] { 20, 1 });
            Fight.Spells.Cast(Witch, Find(SkirmishSamples.WitchSpells(), "Animated Assault"), Goblin, 2, 2, 4);
            Assert.AreEqual(32, Goblin.CurrentHP);
            Assert.AreEqual(1, Fight.Zones.Count);
            Fight.Spells.Sustain(Witch, 2, 4);
            Assert.AreEqual(28, Goblin.CurrentHP);
            Fight.EndTurn();
            Assert.AreEqual(1, Fight.Zones.Count);
            Fight.EndTurn();
            Fight.EndTurn();
            Assert.AreEqual(0, Fight.Zones.Count);
        }

        static SpellCard Find(List<SpellCard> Cards, string Name)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                if (Cards[i].Name == Name)
                {
                    return Cards[i];
                }
            }

            return null;
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
