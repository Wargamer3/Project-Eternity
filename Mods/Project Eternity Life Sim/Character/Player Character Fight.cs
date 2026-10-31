using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public partial class PlayerCharacter
    {
        public SkirmishSide Side;
        public bool PrintedArmor = true;
        public int SpeedFeet = 25;
        public int Perception;
        public int Fortitude;
        public int Reflex;
        public int Will;
        public int Athletics;
        public int Initiative;
        public int ActionsLeft = 3;
        public int ReactionLeft = 1;
        public int ExtraReactionLeft;
        public string ExtraReaction = "";
        public bool FlourishUsed;
        public int StrikesThisTurn;
        public bool Attacked;
        public int CircumstanceArmor;
        public int ItemArmor;
        public int DexCap = -1;
        public int ArmorProficiency;
        public bool OffGuard;
        public PlayerCharacter OffGuardFrom;
        public bool OffGuardMelee;
        public PlayerCharacter OffGuardMeleeFrom;
        public bool Prone;
        public bool Grabbed;
        public bool RaisedShield;
        public int ShieldBonus = 2;
        public int ShieldHardness;
        public int BlockHardness;
        public bool ResistanceSet;
        public int ResistanceFire;
        public int Images;
        public int TempHP;
        public int Frightened;
        public int FrightenedFloor;
        public int Enfeebled;
        public bool Muzzled;
        public bool Charmed;
        public string Commanded = "";
        public bool SeesConcealed;
        public bool Concealed;
        public bool HealHalved;
        public bool Loaded = true;
        public int ReachBonusFeet;
        public bool Swapped;
        public Weapon Weapon;
        public Weapon OtherWeapon;
        public bool HasShield;
        public int CastingAbility;
        public int SpellProficiency;
        public int[] Slots = new int[11];
        public PlayerCharacter Prey;
        public int NextDamage;
        public string NextDamageGroup = "";
        public List<Trait> Feats = new List<Trait>();
        public List<DamagePacket> Persistent = new List<DamagePacket>();
        public List<DamagePacket> BreakPackets = new List<DamagePacket>();
        public PlayerCharacter Companion;
        public PlayerCharacter Owner;
        public int CompanionRank;
        public bool SkipTurn;
        public string CreatureTrait = "";
        public FormCard Form;
        public int HandsFree = 2;
        public bool FarShot;
        public int SpeedPenaltyFeet;
        public int LeapFeet;
        public bool Dying;

        public Weapon CurrentWeapon()
        {
            if (Form != null && Form.Strike != null)
            {
                return Form.Strike;
            }

            return Weapon;
        }

        public PlayerCharacter PrepareForFight()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Name = "Creature";
            }

            Side = SkirmishSide.Monster;
            PrintedArmor = false;
            TakeProficiencies(this, Ancestry == null ? null : Ancestry.DicProficiencyLevelByName);
            TakeProficiencies(this, Background == null ? null : Background.DicProficiencyLevelByName);
            TakeProficiencies(this, Class == null ? null : Class.DicProficiencyLevelByName);
            TakeProficiencies(this, Heritage == null ? null : Heritage.DicProficiencyLevelByName);
            int AncestryHP = Ancestry == null ? 0 : Ancestry.BaseHP;
            int ClassHP = Class == null ? 0 : Class.BaseHP;
            MaxHP = PracticeMath.LevelHitPoints(AncestryHP, ClassHP, CON, Level);
            CurrentHP = MaxHP;
            SpeedFeet = Ancestry == null || Ancestry.Speed == 0 ? 25 : Ancestry.Speed;
            if (WornArmor != null)
            {
                ItemArmor = WornArmor.ItemBonus;
                DexCap = WornArmor.DexCap;
                ArmorProficiency = RankOf(this, WornArmor.Category);
            }
            else
            {
                ArmorProficiency = RankOf(this, "Unarmored");
            }

            AC = PracticeMath.GearArmorClass(DEX, DexCap, ItemArmor, ArmorProficiency, 0);
            Perception = Score(this, "Perception", WIS);
            Fortitude = Score(this, "Fortitude", CON);
            Reflex = Score(this, "Reflex", DEX);
            Will = Score(this, "Will", WIS);
            Athletics = Score(this, "Athletics", STR);
            TakeTraits(this, Ancestry == null ? null : Ancestry.ListTraits);
            TakeTraits(this, Heritage == null ? null : Heritage.ListTraits);
            TakeTraits(this, Background == null ? null : Background.ListTraits);
            TakeTraits(this, Class == null ? null : Class.ListTraits);
            if (ArrayCharacterAction != null)
            {
                for (int i = 0; i < ArrayCharacterAction.Length; i++)
                {
                    CharacterAction Action = ArrayCharacterAction[i];
                    if (Action != null)
                    {
                        TakeTraits(this, Action.ListTraits);
                    }
                }
            }

            if (ListWeapon != null)
            {
                for (int i = 0; i < ListWeapon.Count; i++)
                {
                    Weapon Held = ListWeapon[i];
                    if (Held == null)
                    {
                        continue;
                    }

                    if (Weapon == null)
                    {
                        Weapon = Held;
                    }
                    else if (OtherWeapon == null)
                    {
                        OtherWeapon = Held;
                    }
                }
            }

            return this;
        }

        static void TakeProficiencies(PlayerCharacter Hero, Dictionary<string, ProficiencyLink> Source)
        {
            if (Hero == null || Source == null)
            {
                return;
            }

            if (Hero.DicProficiencyLevelByName == null)
            {
                Hero.DicProficiencyLevelByName = new Dictionary<string, ProficiencyLink>();
            }

            foreach (KeyValuePair<string, ProficiencyLink> Pair in Source)
            {
                if (!Hero.DicProficiencyLevelByName.ContainsKey(Pair.Key))
                {
                    Hero.DicProficiencyLevelByName.Add(Pair.Key, Pair.Value);
                }
            }
        }


        static void TakeTraits(PlayerCharacter Body, List<Trait> Traits)
        {
            if (Body == null || Traits == null)
            {
                return;
            }

            for (int i = 0; i < Traits.Count; i++)
            {
                Trait Trait = Traits[i];
                if (Trait == null)
                {
                    continue;
                }

                bool Empty = Trait.ListEffect == null || Trait.ListEffect.Count == 0;
                if (Empty && string.IsNullOrEmpty(Body.CreatureTrait))
                {
                    Body.CreatureTrait = Trait.Name;
                    continue;
                }

                if (!Empty)
                {
                    Body.Feats.Add(Trait);
                }
            }
        }


        static int RankOf(PlayerCharacter Hero, string Name)
        {
            if (Hero == null || Hero.DicProficiencyLevelByName == null || string.IsNullOrEmpty(Name))
            {
                return 0;
            }

            ProficiencyLink Link;
            if (!Hero.DicProficiencyLevelByName.TryGetValue(Name, out Link) || Link == null)
            {
                return 0;
            }

            return PracticeMath.RankBonus(Link.ProficiencyRank);
        }


        static int Score(PlayerCharacter Hero, string Name, int Ability)
        {
            if (Hero == null || Hero.DicProficiencyLevelByName == null)
            {
                return Ability;
            }

            ProficiencyLink Link;
            if (!Hero.DicProficiencyLevelByName.TryGetValue(Name, out Link) || Link == null)
            {
                return Ability;
            }

            if (Link.ProficiencyRank == Proficiency.ProficiencyRanks.None)
            {
                return Ability;
            }

            return Ability + Hero.Level + PracticeMath.RankBonus(Link.ProficiencyRank);
        }

        public PlayerCharacter Copy()
        {
            PlayerCharacter Body = new PlayerCharacter(null, null, Vector3.Zero);
            Body.Name = Name;
            Body.Side = Side;
            Body.PrintedArmor = PrintedArmor;
            Body.AC = AC;
            Body.MaxHP = MaxHP;
            Body.CurrentHP = MaxHP;
            Body.SpeedFeet = SpeedFeet;
            Body.Perception = Perception;
            Body.Fortitude = Fortitude;
            Body.Reflex = Reflex;
            Body.Will = Will;
            Body.Athletics = Athletics;
            Body.CreatureTrait = CreatureTrait;
            Body.Level = Level;
            Body.STR = STR;
            Body.DEX = DEX;
            Body.CON = CON;
            Body.INT = INT;
            Body.WIS = WIS;
            Body.CHA = CHA;
            Body.DexCap = DexCap;
            Body.ItemArmor = ItemArmor;
            Body.ArmorProficiency = ArmorProficiency;
            Body.DicProficiencyLevelByName = DicProficiencyLevelByName;
            Body.Ancestry = Ancestry;
            Body.Class = Class;
            Body.Weapon = FightRoll.CopyWeapon(Weapon);
            Body.OtherWeapon = FightRoll.CopyWeapon(OtherWeapon);
            Body.Feats = Feats;
            return Body;
        }

        public void SetCompanionRank(int Rank)
        {
            CompanionRank = Rank;
            if (Rank >= 1)
            {
                ItemArmor += 2;
                MaxHP += 16;
                CurrentHP += 16;
                if (PrintedArmor)
                {
                    AC += 2;
                }

                STR += 5;
            }

            if (Rank >= 2)
            {
                ItemArmor += 3;
                MaxHP += 24;
                CurrentHP += 24;
                if (PrintedArmor)
                {
                    AC += 3;
                }

                STR += 3;

                if (Weapon != null)
                {
                    Weapon.Damage = "2d8";
                }
            }

            if (!PrintedArmor)
            {
                AC = PracticeMath.GearArmorClass(DEX, DexCap, ItemArmor, ArmorProficiency, 0);
            }
        }
    }
}
