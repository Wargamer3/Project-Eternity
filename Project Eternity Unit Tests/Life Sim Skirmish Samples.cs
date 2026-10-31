using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class SkirmishSamples
    {
        public static PlayerCharacter Goblin()
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.Name = "Goblin Warrior";
            Who.Level = -1;
            Who.DEX = 4;
            Who.WIS = 2;
            CharacterAncestry Ancestry = new CharacterAncestry();
            Ancestry.Name = "Goblin";
            Ancestry.BaseHP = 6;
            Ancestry.Speed = 25;
            Ancestry.Size = CharacterAncestry.CharacterSizes.Small;
            Ancestry.ListTraits.Add(Feat("Humanoid"));
            Language Tongue = new Language();
            Tongue.Name = "Goblin";
            Ancestry.ListLanguage.Add(Tongue);
            Who.Ancestry = Ancestry;
            Who.ListLanguage = new List<Language>();
            Who.ListLanguage.Add(Tongue);
            CharacterClass Class = new CharacterClass();
            Class.Name = "Goblin Warrior";
            Class.DicProficiencyLevelByName.Add("Dagger", Link("Dagger", Proficiency.ProficiencyRanks.Expert));
            Class.DicProficiencyLevelByName.Add("Bow", Link("Bow", Proficiency.ProficiencyRanks.Expert));
            Class.DicProficiencyLevelByName.Add("Light Armor", Link("Light Armor", Proficiency.ProficiencyRanks.Trained));
            Class.DicProficiencyLevelByName.Add("Fortitude", Link("Fortitude", Proficiency.ProficiencyRanks.Master));
            Class.DicProficiencyLevelByName.Add("Reflex", Link("Reflex", Proficiency.ProficiencyRanks.Expert));
            Class.DicProficiencyLevelByName.Add("Will", Link("Will", Proficiency.ProficiencyRanks.Trained));
            Class.DicProficiencyLevelByName.Add("Athletics", Link("Athletics", Proficiency.ProficiencyRanks.Trained));
            Who.Class = Class;
            Armor Leather = new Armor();
            Leather.Name = "Leather";
            Leather.ItemBonus = 1;
            Leather.DexCap = 3;
            Leather.Category = "Light Armor";
            Who.WornArmor = Leather;
            Weapon Dagger = new Weapon();
            Dagger.Name = "Dagger";
            Dagger.Damage = "1d6";
            Dagger.WeaponType = Weapon.WeaponTypes.Melee;
            Dagger.HandType = Weapon.HandTypes.OneHand;
            Dagger.GroupName = "Dagger";
            Dagger.FarRange = 5f * PracticeMath.FeetToMeters;
            Dagger.ListTraits = new List<Trait>();
            Dagger.ListTraits.Add(Feat("Agile", new PenaltyOverrideEffect()));
            Dagger.ListTraits.Add(Feat("Finesse", new FinesseEffect()));
            Weapon Bow = new Weapon();
            Bow.Name = "Shortbow";
            Bow.Damage = "1d6";
            Bow.DamageType = "Piercing";
            Bow.WeaponType = Weapon.WeaponTypes.Range;
            Bow.GroupName = "Bow";
            Bow.Reload = 0;
            Bow.FarRange = 60f * PracticeMath.FeetToMeters;
            DeadlyEffect Deadly = new DeadlyEffect();
            Deadly.Faces = 10;
            Bow.ListTraits = new List<Trait>();
            Bow.ListTraits.Add(Feat("Deadly", Deadly));
            Who.Equip(Dagger);
            Who.Hold(Bow);
            CharacterAction Scuttle = new CharacterAction();
            Scuttle.Name = "Goblin Scuttle";
            Scuttle.ActionCost = 0;
            Scuttle.ListTraits.Add(Feat("Goblin Scuttle", new StepEffect()));
            Who.ArrayCharacterAction = new CharacterAction[] { Scuttle };
            return Who.PrepareForFight();
        }

        public static PlayerCharacter Raptor()
        {
            return Creature("Raptor", "Animal", 1, 3, 4, 2, 0, 12, 6, 40, "Beak", "1d8", "Beak");
        }

        public static PlayerCharacter Animal()
        {
            return Creature("Animal", "Animal", 1, 1, 3, 2, 0, 6, 2, 40, "Beak", "1d6", "Beak");
        }

        public static PlayerCharacter Undead()
        {
            return Creature("Undead", "Undead", 1, 1, 3, 2, 0, 6, 2, 40, "Beak", "1d6", "Beak");
        }

        public static FormCard WolfForm()
        {
            PlayerCharacter Wolf = Creature("Wolf", "Animal", 1, 7, 6, 2, 0, 8, 6, 35, "Jaws", "2d8", "Jaws");
            FormCard Form = new FormCard();
            Form.Name = Wolf.Name;
            Form.ArmorClass = Wolf.AC;
            Form.SpeedFeet = Wolf.SpeedFeet;
            Form.Strike = Wolf.Weapon;
            Form.Who = Wolf;
            return Form;
        }

        static PlayerCharacter Creature(string Name, string TraitName, int Level, int Strength, int Dexterity, int Constitution, int Wisdom, int AncestryHP, int ClassHP, int Speed, string AttackName, string Damage, string Group)
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.Name = Name;
            Who.Level = Level;
            Who.STR = Strength;
            Who.DEX = Dexterity;
            Who.CON = Constitution;
            Who.WIS = Wisdom;
            CharacterAncestry Ancestry = new CharacterAncestry();
            Ancestry.Name = Name;
            Ancestry.BaseHP = (byte)AncestryHP;
            Ancestry.Speed = (byte)Speed;
            Ancestry.Size = CharacterAncestry.CharacterSizes.Small;
            Ancestry.ListTraits.Add(Feat(TraitName));
            Who.Ancestry = Ancestry;
            CharacterClass Class = new CharacterClass();
            Class.Name = Name;
            Class.BaseHP = (byte)ClassHP;
            Class.DicProficiencyLevelByName.Add(Group, Link(Group, Proficiency.ProficiencyRanks.Expert));
            Class.DicProficiencyLevelByName.Add("Unarmored", Link("Unarmored", Proficiency.ProficiencyRanks.Trained));
            Class.DicProficiencyLevelByName.Add("Perception", Link("Perception", Proficiency.ProficiencyRanks.Expert));
            Class.DicProficiencyLevelByName.Add("Fortitude", Link("Fortitude", Proficiency.ProficiencyRanks.Trained));
            Class.DicProficiencyLevelByName.Add("Reflex", Link("Reflex", Proficiency.ProficiencyRanks.Expert));
            Class.DicProficiencyLevelByName.Add("Will", Link("Will", Proficiency.ProficiencyRanks.Trained));
            Who.Class = Class;
            Weapon Strike = new Weapon();
            Strike.Name = AttackName;
            Strike.Damage = Damage;
            Strike.WeaponType = Weapon.WeaponTypes.Melee;
            Strike.GroupName = Group;
            Strike.DamageType = "Piercing";
            Strike.FarRange = 5f * PracticeMath.FeetToMeters;
            Strike.ListTraits = new List<Trait>();
            Who.Equip(Strike);
            return Who.PrepareForFight();
        }

        public static List<SpellCard> WitchSpells()
        {
            List<SpellCard> List = new List<SpellCard>();
            List.Add(Card("Eat Fire", 1, 1, true, new ResistanceEffect()));
            CircumstanceArmorEffect Shield = new CircumstanceArmorEffect();
            Shield.Amount = 1;
            Shield.Trigger = "Cast";
            HardnessBlockEffect ShieldBlock = new HardnessBlockEffect();
            ShieldBlock.Hardness = 5;
            ShieldBlock.Step = 2;
            ShieldBlock.StepBonus = 5;
            List.Add(Card("Shield", 1, 1, true, Shield, ShieldBlock));
            CircumstanceArmorEffect Glass = new CircumstanceArmorEffect();
            Glass.Amount = 1;
            Glass.Trigger = "Cast";
            HardnessBlockEffect GlassHard = new HardnessBlockEffect();
            GlassHard.Hardness = 2;
            GlassHard.AtRank3 = 4;
            GlassHard.AtRank5 = 7;
            DamagePacketEffect Shards = Packet(1, 4, 0, "Piercing");
            Shards.When = "Break";
            Shards.Basic = true;
            Shards.Count = 4;
            List.Add(Card("Glass Shield", 1, 1, true, Glass, GlassHard, Shards));
            List.Add(Attack("Needle Darts", 2, 1, true, 3, 4, 1, 1, "Piercing", 60f));
            ConditionEffect Slow = Condition("Slowed", 10, "Always");
            List.Add(Card("Manifest Will", 2, 1, true, new ZoneEffect(), Slow));
            ConditionEffect Glee = Condition("Frightened", 1, "Failure");
            Glee.Floor = 1;
            BasicSaveEffect Will = Save("Will", false);
            List.Add(Card("Scrounger's Glee", 1, 1, true, Will, Glee));
            ConditionEffect Night = Condition("Concealed", 1, "Failure");
            Night.Scope = "Sight";
            List.Add(Card("Shroud of Night", 1, 1, true, Save("Will", false), Night));
            DamagePacketEffect Tele = Packet(1, 6, 1, 1, "Bludgeoning");
            Tele.Flat = 4;
            List.Add(Card("Telekinetic Projectile", 2, 1, true, new SpellAttackEffect(), Tele));
            SpellAttackEffect Phase = new SpellAttackEffect();
            Phase.RangeFeet = 60f;
            List.Add(Card("Phase Bolt", 2, 1, true, Phase, Packet(3, 4, 1, 1, "Piercing"), new IgnoreCircumstanceArmorEffect()));
            List.Add(Card("Void Warp", 2, 1, true, Save("Fortitude", true), Packet(2, 4, 1, 1, "Void"), Condition("Enfeebled", 1, "Failure")));
            List.Add(Card("Agitate", 2, 1, false, Save("Will", false), Condition("Off-guard", 1, "Failure")));
            List.Add(Card("Command", 1, 1, false, Save("Will", false), Condition("Commanded", 1, "Failure")));
            List.Add(Card("Fear", 2, 1, false, Save("Will", false), Condition("Frightened", 1, "Failure"), Condition("Frightened", 2, "CriticalFailure")));
            List.Add(Card("Gravitational Pull", 2, 1, false, Save("Reflex", false), Condition("Slowed", 10, "Failure")));
            DamagePacketEffect Tendril = Packet(2, 4, 1, 1, "Slashing");
            Tendril.Basic = true;
            DamagePacketEffect Bleed = Packet(1, 4, 0, "Bleed");
            Bleed.When = "Persistent";
            List.Add(Card("Grim Tendrils", 2, 1, false, Save("Reflex", true), Tendril, Bleed));
            List.Add(Card("Mystic Armor", 2, 1, false, new ItemArmorEffect()));
            DamagePacketEffect Scorch = Packet(2, 6, 1, 1, "Fire");
            Scorch.Basic = true;
            List.Add(Card("Scorching Blast", 2, 1, false, Save("Reflex", true), Scorch));
            SummonEffect Undead = new SummonEffect();
            Undead.Entry = "Undead";
            List.Add(Card("Summon Undead", 3, 1, false, Undead));
            DamagePacketEffect Assault = Packet(2, 10, 2, 2, "Bludgeoning");
            Assault.Basic = true;
            DamagePacketEffect AssaultSustain = Packet(1, 10, 1, 2, "Bludgeoning");
            AssaultSustain.Basic = true;
            AssaultSustain.When = "Sustain";
            ZoneEffect AssaultZone = new ZoneEffect();
            AssaultZone.Feet = 10f;
            List.Add(Card("Animated Assault", 2, 2, false, AssaultZone, Save("Reflex", true), Assault, AssaultSustain));
            DamagePacketEffect Boom = Packet(2, 6, 1, 1, "Fire");
            Boom.Basic = true;
            List.Add(Card("Final Sacrifice", 2, 2, false, new SacrificeEffect(), Boom));
            List.Add(Card("Blur", 2, 2, false, Condition("Concealed", 1, "Always")));
            ZoneEffect Mist = new ZoneEffect();
            Mist.Feet = 20f;
            List.Add(Card("Mist", 2, 2, false, Mist, Condition("Concealed", 1, "Always")));
            HealEffect Lantern = new HealEffect();
            Lantern.Halve = true;
            Lantern.Amount = 0;
            ZoneEffect Light = new ZoneEffect();
            Light.Feet = 20f;
            List.Add(Card("Reaper's Lantern", 2, 2, false, Light, Lantern));
            return List;
        }

        public static List<SpellCard> SorcererSpells()
        {
            List<SpellCard> List = new List<SpellCard>();
            SpellAttackEffect Touch = new SpellAttackEffect();
            Touch.RangeFeet = 30f;
            List.Add(Card("Ignition", 2, 1, true, Touch, Packet(2, 6, 1, 1, "Fire")));
            DamagePacketEffect Slash = Packet(1, 4, 1, 2, "Slashing");
            DamagePacketEffect Shock = Packet(1, 4, 1, 2, "Electricity");
            DamagePacketEffect ShockFail = Packet(1, 4, 1, 2, "Electricity");
            ShockFail.When = "Failure";
            DamagePacketEffect Persist = Packet(1, 4, 1, 2, "Electricity");
            Persist.When = "Persistent";
            SpellAttackEffect Wire = new SpellAttackEffect();
            Wire.RangeFeet = 30f;
            List.Add(Card("Live Wire", 2, 1, true, Wire, Slash, Shock, ShockFail, Persist));
            HealEffect Splash = new HealEffect();
            Splash.Temporary = true;
            Splash.Amount = 4;
            Splash.Recover = "Fire,Acid";
            List.Add(Card("Rousing Splash", 2, 1, true, Splash));
            SpellAttackEffect Gust = new SpellAttackEffect();
            Gust.RangeFeet = 60f;
            DamagePacketEffect GustBleed = Packet(1, 4, 1, 1, "Bleed");
            GustBleed.When = "Persistent";
            List.Add(Card("Slashing Gust", 2, 1, true, Gust, new SamePenaltyEffect(), Packet(2, 4, 1, 1, "Slashing"), GustBleed));
            List.Add(Card("Stabilize", 2, 1, true, new ClearDyingEffect()));
            SpellAttackEffect Ray = new SpellAttackEffect();
            Ray.RangeFeet = 60f;
            DamagePacketEffect Admonish = Packet(2, 6, 2, 1, "Bludgeoning");
            Admonish.Nonlethal = true;
            List.Add(Card("Admonishing Ray", 2, 1, false, Ray, Admonish));
            DamagePacketEffect Breath = Packet(2, 6, 1, 1, "Fire");
            Breath.Basic = true;
            List.Add(Card("Breathe Fire", 2, 1, false, Save("Reflex", true), Breath));
            List.Add(Card("Charm", 2, 1, false, Save("Will", false), Condition("Charmed", 1, "Failure")));
            List.Add(Card("Dehydrate", 2, 1, false, Save("Fortitude", true), Packet(2, 8, 1, 1, "Fire"), Condition("Enfeebled", 1, "Failure")));
            List.Add(Card("Enfeeble", 2, 1, false, Save("Fortitude", false), Condition("Enfeebled", 1, "Failure"), Condition("Enfeebled", 2, "CriticalFailure")));
            DamagePacketEffect Dart = Packet(1, 4, 0, "Force");
            Dart.Flat = 1;
            List.Add(Card("Force Barrage", 3, 1, false, new AutomaticDamageEffect(), Dart));
            List.Add(Card("Jump", 1, 1, false, new LeapEffect()));
            ZoneEffect Vapor = new ZoneEffect();
            Vapor.Feet = 10f;
            List.Add(Card("Noxious Vapors", 2, 1, false, Vapor, Save("Fortitude", false), Condition("Enfeebled", 1, "Failure")));
            HealEffect Soothe = new HealEffect();
            Soothe.Amount = 9;
            List.Add(Card("Soothe", 2, 1, false, Soothe));
            SummonEffect Animal = new SummonEffect();
            Animal.Entry = "Animal";
            List.Add(Card("Summon Animal", 3, 1, false, Animal));
            SpellAttackEffect Thunder = new SpellAttackEffect();
            Thunder.RangeFeet = 120f;
            DamagePacketEffect Sonic = Packet(1, 4, 0, "Sonic");
            Sonic.Basic = true;
            List.Add(Card("Thunderstrike", 2, 1, false, Thunder, Packet(2, 12, 1, 1, "Electricity"), Save("Reflex", true), Sonic));
            BattleFormEffect Form = new BattleFormEffect();
            Form.Form = "Wolf";
            List.Add(Card("Animal Form", 2, 2, false, Form));
            ImagesEffect Images = new ImagesEffect();
            List.Add(Card("Mirror Image", 2, 2, false, Images));
            DamagePacketEffect Shatter = Packet(2, 10, 1, 1, "Sonic");
            Shatter.Basic = true;
            List.Add(Card("Shatter", 2, 2, false, Save("Fortitude", true), Shatter));
            List.Add(Card("Speak with Animals", 2, 2, false));
            SpellAttackEffect Sticky = new SpellAttackEffect();
            Sticky.RangeFeet = 30f;
            DamagePacketEffect StickyBurn = Packet(1, 6, 0, "Fire");
            StickyBurn.When = "Persistent";
            List.Add(Card("Sticky Fire", 2, 2, false, Sticky, Packet(2, 6, 1, 1, "Fire"), StickyBurn));
            List.Add(Card("Binding Muzzle", 2, 2, false, Save("Fortitude", false), Condition("Muzzled", 1, "Failure")));
            return List;
        }

        public static List<PlayerCharacter> Party(Skirmish Fight)
        {
            List<PlayerCharacter> Party = new List<PlayerCharacter>();
            PlayerCharacter Rogue = Hero("Wisp Fetchling Rogue", 5, 4, 1, 4, 8, 8, 2);
            Rogue.Feats.Add(Feat("Nimble Dodge", ArmorTrigger("Targeted", 2, false)));
            StepEffect Roll = new StepEffect();
            Roll.Trigger = "Targeted";
            Rogue.Feats.Add(Feat("Nimble Roll", Roll));
            Rogue.Feats.Add(Feat("Nimble Strike", new FreeStrikeEffect()));
            PenaltyOverrideEffect Grace = new PenaltyOverrideEffect();
            Grace.Second = 3;
            Grace.Later = 6;
            Rogue.Feats.Add(Feat("Agile Grace", Grace));
            Rogue.Feats.Add(Feat("Cut from the Air", ArmorTrigger("Ranged", 4, false)));
            Rogue.Feats.Add(Feat("Flinging Charge", Act("Flinging Charge")));
            Rogue.Feats.Add(Feat("Skirmish Strike", Act("Skirmish Strike")));
            PrecisionEffect Sneak = new PrecisionEffect();
            Sneak.Count = 3;
            Rogue.Feats.Add(Feat("Sneak Attack", Sneak));
            Weapon Dagger = new Weapon();
            Dagger.Name = "Dagger";
            Dagger.Damage = "1d6";
            Dagger.GroupName = "Dagger";
            Dagger.ListTraits = new List<Trait>();
            Dagger.ListTraits.Add(Feat("Agile", new PenaltyOverrideEffect()));
            Dagger.ListTraits.Add(Feat("Finesse", new FinesseEffect()));
            Rogue.Weapon = Dagger;
            Rogue.STR = 2;
            Party.Add(Rogue);

            PlayerCharacter Bunny = Caster("Awakened Animal Sorcerer", 4, 4);
            Party.Add(Bunny);

            PlayerCharacter Witch = Caster("Tiefling Witch", 4, 4);
            Party.Add(Witch);

            PlayerCharacter Android = Hero("Android Fighter", 3, 2, 3, 4, 10, 10, 3);
            Android.Feats.Add(Feat("Sudden Charge", Act("Sudden Charge")));
            Android.Feats.Add(Feat("Snagging Strike", Act("Snagging Strike")));
            Android.Feats.Add(Feat("Vicious Swing", Act("Vicious Swing"), new ExtraDiceEffect()));
            Android.Feats.Add(Feat("Combat Grab", Act("Combat Grab")));
            Android.Feats.Add(Feat("Lunge", Act("Lunge")));
            Android.Feats.Add(Feat("Quick Reversal", Act("Quick Reversal"), new StrikeBackEffect()));
            Android.Feats.Add(Feat("Tactical Reflexes", Act("Reactive Strike")));
            Android.ExtraReaction = "Reactive Strike";
            Weapon Greatsword = new Weapon();
            Greatsword.Name = "Greatsword";
            Greatsword.Damage = "1d12";
            Greatsword.GroupName = "Sword";
            Greatsword.HandType = Weapon.HandTypes.TwoHands;
            Android.Weapon = Greatsword;
            Android.STR = 4;
            Party.Add(Android);

            PlayerCharacter Champion = Hero("Elf Champion", 2, 1, 2, 4, 10, 10, 3);
            Champion.HasShield = true;
            Champion.ShieldBonus = 2;
            Champion.ShieldHardness = 5;
            Champion.BlockHardness = 5;
            Champion.Feats.Add(Feat("Shield Block", Act("Shield Block")));
            CircumstanceArmorEffect Reactive = ArmorTrigger("Targeted", 2, true);
            Champion.Feats.Add(Feat("Reactive Shield", Reactive));
            Champion.Feats.Add(Feat("Reflexive Shield", Act("Reflexive Shield")));
            Champion.Feats.Add(Feat("Shield Warden", Act("Shield Warden")));
            Champion.Feats.Add(Feat("Quick Shield Block", Act("Shield Block")));
            Champion.ExtraReaction = "Shield Block";
            Weapon Spear = new Weapon();
            Spear.Name = "Spear";
            Spear.Damage = "1d6";
            Spear.GroupName = "Spear";
            Champion.Weapon = Spear;
            Champion.STR = 4;
            Party.Add(Champion);

            PlayerCharacter Yaoguai = Hero("Yaoguai Fighter", 1, 0, 6, 6, 10, 10, 3);
            Yaoguai.HasShield = true;
            Yaoguai.ShieldHardness = 5;
            Yaoguai.Feats.Add(Feat("Lightning Swap", Act("Lightning Swap")));
            Yaoguai.Feats.Add(Feat("Slam Down", Act("Slam Down")));
            Yaoguai.Feats.Add(Feat("Swipe", Act("Swipe")));
            Yaoguai.Feats.Add(Feat("Spear Dancer", Act("Spear Dancer")));
            Yaoguai.Feats.Add(Feat("Crashing Slam", new ReplaceTripEffect()));
            Weapon Longsword = new Weapon();
            Longsword.Name = "Longsword";
            Longsword.Damage = "1d8";
            Longsword.GroupName = "Sword";
            Weapon Lance = new Weapon();
            Lance.Name = "War Lance";
            Lance.Damage = "1d8";
            Lance.GroupName = "Spear";
            Lance.HandType = Weapon.HandTypes.TwoHands;
            Yaoguai.Weapon = Longsword;
            Yaoguai.OtherWeapon = Lance;
            Yaoguai.STR = 4;
            Party.Add(Yaoguai);

            PlayerCharacter Ranger = Hero("Ranger", 3, 2, 3, 4, 8, 10, 2);
            Ranger.Feats.Add(Feat("Animal Companion", Act("Command")));
            NextDamageEffect Ace = new NextDamageEffect();
            Ranger.Feats.Add(Feat("Crossbow Ace", Ace));
            AimEffect Aim = new AimEffect();
            Aim.Activity = "Hunter's Aim";
            Aim.Circumstance = 2;
            Ranger.Feats.Add(Feat("Hunter's Aim", Act("Hunter's Aim"), Aim));
            Ranger.Feats.Add(Feat("Quick Draw", Act("Quick Draw")));
            Ranger.Feats.Add(Feat("Far Shot", Act("Far Shot")));
            Ranger.Feats.Add(Feat("Running Reload", Act("Running Reload")));
            Ranger.Feats.Add(Feat("Defy Fey", new TraitBonusEffect()));
            Ranger.Feats.Add(Feat("Mature Animal Companion", Act("Mature")));
            AimEffect Deadly = new AimEffect();
            Deadly.Activity = "Deadly Aim";
            Deadly.Penalty = 2;
            Deadly.Bonus = 4;
            Ranger.Feats.Add(Feat("Deadly Aim", Act("Deadly Aim"), Deadly));
            Ranger.Feats.Add(Feat("Camouflage", Act("Camouflage")));
            Ranger.Feats.Add(Feat("Incredible Animal Companion", Act("Incredible")));
            Ranger.Feats.Add(Feat("Hunt Prey", Act("Hunt Prey")));
            Weapon Crossbow = new Weapon();
            Crossbow.Name = "Crossbow";
            Crossbow.Damage = "1d8";
            Crossbow.WeaponType = Weapon.WeaponTypes.Range;
            Crossbow.GroupName = "Crossbow";
            Crossbow.Reload = 1;
            Crossbow.FarRange = 120f * PracticeMath.FeetToMeters;
            Ranger.Weapon = Crossbow;
            Ranger.Loaded = true;
            Ranger.DEX = 4;
            PlayerCharacter Pet = Raptor();
            Pet.Owner = Ranger;
            Pet.SkipTurn = true;
            Ranger.Companion = Pet;
            if (Fight != null)
            {
                Pet.SetCompanionRank(2);
                Fight.Forms.Add(WolfForm());
                Fight.Summoned.Add(Undead());
                Fight.Summoned.Add(Animal());
            }

            Party.Add(Ranger);
            Party.Add(Pet);
            return Party;
        }

        public static PlayerCharacter Hero(string Name, int Dexterity, int DexCap, int Item, int Proficiency, int Ancestry, int ClassHitPoints, int Constitution)
        {
            PlayerCharacter Body = new PlayerCharacter(null, null, Vector3.Zero);
            Body.Name = Name;
            Body.Side = SkirmishSide.Hero;
            Body.Level = 10;
            Body.PrintedArmor = false;
            Body.DEX = Dexterity;
            Body.DexCap = DexCap;
            Body.ItemArmor = Item;
            Body.ArmorProficiency = Proficiency;
            Body.MaxHP = PracticeMath.LevelHitPoints(Ancestry, ClassHitPoints, Constitution, 10);
            Body.CurrentHP = Body.MaxHP;
            Body.Perception = 10;
            Body.SpeedFeet = 25;
            return Body;
        }

        static PlayerCharacter Caster(string Name, int Ability, int Proficiency)
        {
            PlayerCharacter Body = Hero(Name, Ability, -1, 0, 2, 8, 6, 2);
            Body.CastingAbility = Ability;
            Body.SpellProficiency = Proficiency;
            for (int Rank = 1; Rank <= 5; Rank++)
            {
                Body.Slots[Rank] = 3;
            }

            return Body;
        }

        static ProficiencyLink Link(string Name, Proficiency.ProficiencyRanks Rank)
        {
            Proficiency Body = new Proficiency();
            Body.Name = Name;
            ProficiencyLink Link = new ProficiencyLink(Name);
            Link.LinkedProficiency = Body;
            Link.ProficiencyRank = Rank;
            return Link;
        }

        static Trait Feat(string Name, params TraitEffect[] Effects)
        {
            Trait Trait = new Trait();
            Trait.Name = Name;
            for (int i = 0; i < Effects.Length; i++)
            {
                if (Effects[i] != null)
                {
                    Trait.ListEffect.Add(Effects[i]);
                }
            }

            return Trait;
        }

        static ActivityEffect Act(string Name)
        {
            ActivityEffect Effect = new ActivityEffect();
            Effect.Activity = Name;
            return Effect;
        }

        static CircumstanceArmorEffect ArmorTrigger(string Trigger, int Amount, bool Raise)
        {
            CircumstanceArmorEffect Effect = new CircumstanceArmorEffect();
            Effect.Trigger = Trigger;
            Effect.Amount = Amount;
            Effect.RaiseShield = Raise;
            return Effect;
        }

        static SpellCard Card(string Name, int Actions, int Rank, bool Cantrip, params TraitEffect[] Effects)
        {
            SpellCard Card = new SpellCard();
            Card.Name = Name;
            Card.Actions = Actions;
            Card.BaseRank = Rank;
            Card.Cantrip = Cantrip;
            Card.Spell = Feat(Name, Effects);
            return Card;
        }

        static SpellCard Attack(string Name, int Actions, int Rank, bool Cantrip, int Count, int Faces, int PerRank, int Every, string Type, float Range)
        {
            SpellAttackEffect Attack = new SpellAttackEffect();
            Attack.RangeFeet = Range;
            return Card(Name, Actions, Rank, Cantrip, Attack, Packet(Count, Faces, PerRank, Every, Type));
        }

        static DamagePacketEffect Packet(int Count, int Faces, int PerRank, int Every, string Type)
        {
            DamagePacketEffect Effect = new DamagePacketEffect();
            Effect.Count = Count;
            Effect.Faces = Faces;
            Effect.PerRank = PerRank;
            Effect.EveryRanks = Every;
            Effect.Type = Type;
            return Effect;
        }

        static DamagePacketEffect Packet(int Count, int Faces, int PerRank, string Type)
        {
            return Packet(Count, Faces, PerRank, 1, Type);
        }

        static BasicSaveEffect Save(string Defense, bool Basic)
        {
            BasicSaveEffect Effect = new BasicSaveEffect();
            Effect.Save = Defense;
            Effect.Basic = Basic;
            return Effect;
        }

        static ConditionEffect Condition(string Name, int Value, string When)
        {
            ConditionEffect Effect = new ConditionEffect();
            Effect.Name = Name;
            Effect.Value = Value;
            Effect.When = When;
            return Effect;
        }
    }
}
