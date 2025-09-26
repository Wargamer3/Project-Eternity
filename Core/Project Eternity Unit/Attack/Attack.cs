using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core.Attacks
{
    public enum WeaponStyle { M, R, Special };

    /* The Primary property of an attack. PLA means it is a platoon attack, which can be used to support other units. ALL means that it is an All attack, which hits multiple targets instead of one.
     * PER is for Persistent which doesn't need to target an enemy
     */
    public enum WeaponPrimaryProperty : byte { None = 0, PLA = 1, ALL = 2, Break = 3, Multi = 4, Dash = 5, MAP = 6, PER = 7 };

    /* The secondary property of an attack. Examples of Secondary Properties are:
    * P: Post Movement. Means that the attack can be used after moving.
    * B: Beam Attack. Enemies that are weak to Beams will take more damage, but certain barriers will reduce their damage to 0.
    * S: Special Attack. Can reduce an enemy's stats for three turns. Which stat is reduced is listed under Special:, on the bottom right corner of the menu.*/
    [Flags]
    public enum WeaponSecondaryProperty : byte { None = 0, SwordCut = 0x1, ShootDown = 0x2, Partial = 0x4 };

    public enum WeaponMAPProperties : byte {  Spread = 0, Direction = 1, Targeted = 2 };

    public class Attack : ShopItem
    {
        public enum MinorAttributes { PostMovement, Beam, Assault, Status, IgnoreSizePenalty, CombinationAttack };

        public WeaponStyle Style;// - Shows either Melee, Ranged or Special; determines which Pilot Stat to call on.
        public string PowerFormula;
        public string MinDamageFormula;
        public byte RangeMinimum;
        public byte RangeMaximum;// - How close (or far) an enemy must be for this weapon to be used on them. Normally expressed in Min-Max (e.g. 2-4 means the weapon works on enemies between two and four cells away).
        public WeaponPrimaryProperty Pri;
        public WeaponSecondaryProperty Sec;
        public byte PostMovementLevel;
        public byte ReMoveLevel;
        public byte PostMovementAccuracyMalus;
        public byte PostMovementEvasionBonus;
        public BaseAutomaticSkill[] ArrayAttackAttributes;
        public sbyte Accuracy;// A flat bonus used in the accuracy formula. Hit can be improved via Pilot Abilities, Unit Abilities, and Parts.
        public sbyte Critical;// A flat bonus used in the critical hit rate formula. Crit can be improved via Pilot Abilities, Unit Abilities, and Parts.

        public byte _Ammo;// Certain attacks use ammo. If they hit 0, they will be unable to use that gun until resupplied. Ammo can can be improved via Pilot Abilities, Unit Abilities, and Parts.
        public byte Ammo { get { return Parent != null ? Parent.Ammo : _Ammo; } }
        public byte AmmoConsumption;//Number of ammo to use per attack. Used mostly for secondary and charged Attacks.
        private byte _MaxAmmo;
        public byte MaxAmmo { get { return Parent != null ? Parent.MaxAmmo : _MaxAmmo; } }
        public byte ENCost;// Certain attacks use EN. If EN is reduced to 0, the unit will be unable to use that attack. EN costs can be can be lowered via Pilot Abilities, Unit Abilities, and Parts.
        public byte MoraleRequirement;// Most attacks require morale to use. Morale is gained by defeating the enemy, but is not lowered by using an attack.

        public string AttackType;/* - Shows which category along the "Ways to knock down your foes" spectrum the weapon lies.
                                * Solid Shot - Machine guns, bazookas and solid-shell cannons.
                                * Energy Shot - Beam rifles, beam cannons and variations on such.
                                * Solid Blade - Metal weapons, both sharp and blunt, used at close range.
                                * Energy Blade - Beam swords and variants thereof.
                                * Melee - Punches, kicks and all the myriad ways.
                                * Remote - Attack drones hitting from all angles. Also comes in Energy and Solid.
                                * Special - Multiple types, or something more.*/
        public string SpecialEffects;// - Any additional information about an attack not shown above. Status attacks use this area to show

        public bool CanAttack { get { return _CanAttack; } }
        private bool _CanAttack;//Tell if an ennemy is in range.

        public List<string> ListQuoteSet;
        public Dictionary<byte, byte> DicRankByMovement;

        public MAPAttackAttributes MAPAttributes;
        public PERAttackAttributes PERAttributes;
        public ExplosionOptions ExplosionOption;
        public KnockbackAttackAttributes KnockbackAttributes;
        public RotationAttackAttributes RotationAttributes;
        public DestructibleTilesAttackAttributes DestructibleTilesAttributes;
        public byte ALLLevel;//Number of Units an ALL Attack can hit
        public byte DashMaxReach;
        public Attack Parent;
        public bool IsChargeable;
        public byte ChargedAttackCancelLevel;
        public List<Attack> ListChargedAttack;
        public List<Attack> ListSecondaryAttack;
        public readonly List<AttackContext> Animations;
        public readonly bool IsExternal;

        public Attack(string RelativeName)
            : base(RelativeName)
        {
            Animations = new List<AttackContext>();
            Animations.Add(new AttackContext());
            DicRankByMovement = new Dictionary<byte, byte>();
            DicRankByMovement.Add(UnitStats.TerrainLandIndex, 1);
            DicRankByMovement.Add(UnitStats.TerrainAirIndex, 1);
            DicRankByMovement.Add(UnitStats.TerrainSeaIndex, 1);
            DicRankByMovement.Add(UnitStats.TerrainSpaceIndex, 1);

            ListChargedAttack = new List<Attack>();
            ListSecondaryAttack = new List<Attack>();
        }

        public Attack(string Name, string Description, int Price, string PowerFormula, byte RangeMin, byte RangeMax, WeaponPrimaryProperty Pri, WeaponSecondaryProperty Sec,
            sbyte Hit, sbyte Crit, byte AmmoMax, byte ENCost, byte MoraleRequirement, string Type, Dictionary<byte, byte> DicTerrainAttribute)
            : base(Name, Description, Price)
        {
            ArrayAttackAttributes = new BaseAutomaticSkill[0];
            this.PowerFormula = PowerFormula;
            this.RangeMinimum = RangeMin;
            this.RangeMaximum = RangeMax;
            this.Pri = Pri;
            this.Sec = Sec;
            this.Accuracy = Hit;
            this.Critical = Crit;
            this._MaxAmmo = AmmoMax;
            this.ENCost = ENCost;
            this.MoraleRequirement = MoraleRequirement;
            this.AttackType = Type;
            this.DicRankByMovement = DicTerrainAttribute;

            ListChargedAttack = new List<Attack>();
            ListSecondaryAttack = new List<Attack>();
        }

        public Attack(Attack Weapon)
            : this(Weapon.RelativePath, Weapon.Description, Weapon.Price, Weapon.PowerFormula, Weapon.RangeMinimum, Weapon.RangeMaximum, Weapon.Pri, Weapon.Sec, Weapon.Accuracy, Weapon.Critical, Weapon.MaxAmmo, Weapon.ENCost, Weapon.MoraleRequirement, Weapon.AttackType, Weapon.DicRankByMovement)
        {
        }

        public Attack(string AttackPath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(AttackPath)
        {
            Animations = new List<AttackContext>();
            IsExternal = true;

            FileStream FS = new FileStream("Content/Attacks/" + AttackPath + ".pew", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            Init(BR, Content, AttackPath, DicRequirement, DicEffect, DicAutomaticSkillTarget);

            FS.Close();
            BR.Close();
        }

        public Attack(BinaryReader BR, ContentManager Content, string AttackPath, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(AttackPath)
        {
            Animations = new List<AttackContext>();
            IsExternal = false;

            Init(BR, Content, AttackPath, DicRequirement, DicEffect, DicAutomaticSkillTarget);
        }

        public void Init(BinaryReader BR, ContentManager Content, string AttackName, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            //Create the Part file.
            this.ItemName = Path.GetFileNameWithoutExtension("Content/Attacks/" + AttackName + ".pew");
            this.Description = BR.ReadString();

            this.PowerFormula = BR.ReadString();
            this.MinDamageFormula = BR.ReadString();
            this.ENCost = BR.ReadByte();
            this._MaxAmmo = BR.ReadByte();
            this.AmmoConsumption = BR.ReadByte();
            this.MoraleRequirement = BR.ReadByte();
            this.RangeMinimum = BR.ReadByte();
            this.RangeMaximum = BR.ReadByte();
            this.Accuracy = BR.ReadSByte();
            this.Critical = BR.ReadSByte();

            this.Pri = (WeaponPrimaryProperty)BR.ReadByte();
            if (this.Pri == WeaponPrimaryProperty.ALL)
            {
                ALLLevel = BR.ReadByte();
            }
            if (this.Pri == WeaponPrimaryProperty.Dash)
            {
                DashMaxReach = BR.ReadByte();
                KnockbackAttributes = new KnockbackAttackAttributes(BR);
            }
            else if (this.Pri == WeaponPrimaryProperty.MAP)
            {
                MAPAttributes = new MAPAttackAttributes(BR);
                KnockbackAttributes = new KnockbackAttackAttributes();
            }
            else if (this.Pri == WeaponPrimaryProperty.PER)
            {
                PERAttributes = new PERAttackAttributes(BR, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                KnockbackAttributes = new KnockbackAttackAttributes();
            }
            else
            {
                KnockbackAttributes = new KnockbackAttackAttributes(BR);
            }

            this.Sec = (WeaponSecondaryProperty)BR.ReadByte();
            ReMoveLevel = BR.ReadByte();
            PostMovementLevel = BR.ReadByte();
            PostMovementAccuracyMalus = BR.ReadByte();
            PostMovementEvasionBonus = BR.ReadByte();
            
            bool UseRotation = BR.ReadBoolean();

            if (UseRotation)
            {
                RotationAttributes = new RotationAttackAttributes();
            }

            DestructibleTilesAttributes = new DestructibleTilesAttackAttributes(BR);

            byte AttackType = BR.ReadByte();

            if (AttackType == 0)
            {
                this.AttackType = "Blank";
                Style = WeaponStyle.M;
            }
            else if (AttackType == 1)
            {
                this.AttackType = "Melee";
                Style = WeaponStyle.M;
            }
            else if (AttackType == 2)
            {
                this.AttackType = "Solid blade";
                Style = WeaponStyle.M;
            }
            else if (AttackType == 3)
            {
                this.AttackType = "Energy blade";
                Style = WeaponStyle.M;
            }
            else if (AttackType == 4)
            {
                this.AttackType = "Solid shot";
                Style = WeaponStyle.R;
            }
            else if (AttackType == 5)
            {
                this.AttackType = "Energy shot";
                Style = WeaponStyle.R;
            }
            else if (AttackType == 6)
            {
                this.AttackType = "Remote";
                Style = WeaponStyle.R;
            }
            else if (AttackType == 7)
            {
                this.AttackType = "Special";
                Style = WeaponStyle.R;
            }

            int TerrainGradeCount = BR.ReadInt32();
            DicRankByMovement = new Dictionary<byte, byte>(TerrainGradeCount);
            for (int G = 0; G < TerrainGradeCount; ++G)
            {
                DicRankByMovement.Add(BR.ReadByte(), BR.ReadByte());
            }

            int ListSecondaryAttackCount = BR.ReadInt32();
            ListSecondaryAttack = new List<Attack>(ListSecondaryAttackCount);
            for (int S = 0; S < ListSecondaryAttackCount; ++S)
            {
                Attack LoadedSecondaryAttack = new Attack(BR.ReadString(), Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                LoadedSecondaryAttack.Parent = this;
                ListSecondaryAttack.Add(LoadedSecondaryAttack);
            }

            int ListChargedAttackCount = BR.ReadInt32();
            ListChargedAttack = new List<Attack>(ListChargedAttackCount);
            for (int S = 0; S < ListChargedAttackCount; ++S)
            {
                Attack LoadedChargedAttack = new Attack(BR.ReadString(), Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                LoadedChargedAttack.Parent = this;
                ListChargedAttack.Add(LoadedChargedAttack);
            }
            if (ListChargedAttackCount > 0)
            {
                ChargedAttackCancelLevel = BR.ReadByte();
            }

            ExplosionOption = new ExplosionOptions(BR);

            Int32 AttackAttributesCount = BR.ReadInt32();
            this.ArrayAttackAttributes = new BaseAutomaticSkill[AttackAttributesCount];

            for (int S = 0; S < AttackAttributesCount; ++S)
            {
                string RelativePath = BR.ReadString();
                ArrayAttackAttributes[S] = new BaseAutomaticSkill("Content/Attacks/Attributes/" + RelativePath + ".peaa", RelativePath, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }

            ListQuoteSet = new List<string>();
            int ListQuoteSetCount = BR.ReadInt32();
            for (int R = 0; R < ListQuoteSetCount; R++)
                ListQuoteSet.Add(BR.ReadString());
        }

        public void UpdateAttack(Unit CurrentUnit, Vector3 StartPosition, int UnitTeam, Vector3 TargetPosition, int TargetTeam, bool[,] ArrayTargetMapSize, Point TerrainSize, byte TargetMovementType, bool CanMove)
        {
            if (CanAttackTarget(CurrentUnit, StartPosition, UnitTeam, TargetPosition, TargetTeam, ArrayTargetMapSize, TerrainSize, TargetMovementType, CanMove))
            {
                _CanAttack = true;
            }
            else
            {
                _CanAttack = false;
            }
        }

        public void DisableAttack()
        {
            _CanAttack = false;
        }

        private bool CanAttackTarget(Unit CurrentUnit, Vector3 StartPosition, int UnitTeam, Vector3 TargetPosition, int TargetTeam, bool[,] ArrayTargetMapSize, Point TerrainSize, byte TargetMovementType, bool CanMove)
        {
            //If the Mech have enough EN to use the weapon and the weapon have enough ammo to fire.
            if (CurrentUnit.EN < ENCost || (Ammo <= 0 && MaxAmmo > 0)
                || MoraleRequirement > CurrentUnit.PilotMorale)
                return false;

            if (!DicRankByMovement.ContainsKey(TargetMovementType) ||  DicRankByMovement[TargetMovementType] == '-')
                return false;

            //Define the minimum and maximum value of the attack range.
            int MinDistance = RangeMinimum;
            int MaxDistance = RangeMaximum;
            if (MaxDistance > 1)
                MaxDistance += CurrentUnit.Boosts.RangeModifier;

            //If it's a post-movement weapon or the mech can still move.
            if (CanMove || (!CanMove && IsPostMovement(CurrentUnit)))
            {
                if (IsChargeable)
                {
                    return true;
                }
                if (Pri == WeaponPrimaryProperty.PER)
                {
                    return true;
                }
                else if (Pri == WeaponPrimaryProperty.MAP)
                {
                    if (MAPAttributes.Delay > 0)
                    {
                        return true;
                    }
                    for (int X = 0; X < ArrayTargetMapSize.GetLength(0); X++)
                    {
                        for (int Y = 0; Y < ArrayTargetMapSize.GetLength(1); Y++)
                        {
                            if (ArrayTargetMapSize[X, Y])
                            {
                                if (MAPAttributes.CanAttackTarget(StartPosition, new Vector3(TargetPosition.X + X * TerrainSize.X, TargetPosition.Y + Y * TerrainSize.Y, TargetPosition.Z), (int)Math.Max(0, MinDistance + ExplosionOption.ExplosionRadius), MaxDistance + (int)ExplosionOption.ExplosionRadius))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (TargetTeam == UnitTeam)
                        return false;

                    for (int X = 0; X < ArrayTargetMapSize.GetLength(0); X++)
                    {
                        for (int Y = 0; Y < ArrayTargetMapSize.GetLength(1); Y++)
                        {
                            if (ArrayTargetMapSize[X, Y])
                            {
                                float Distance = Math.Abs(StartPosition.X - (TargetPosition.X + X * TerrainSize.X)) / TerrainSize.X + Math.Abs(StartPosition.Y - (TargetPosition.Y + Y * TerrainSize.Y)) / TerrainSize.Y;
                                //If a Unit is in range.
                                if (Distance >= MinDistance && Distance <= MaxDistance)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool IsPostMovement(Unit CurrentUnit)
        {
            return PostMovementLevel > 0 && (CurrentUnit.GetPostMovementLevel() >= PostMovementLevel || CurrentUnit.Boosts.PostMovementModifier.Attack);
        }

        public int GetPower(Unit Owner, FormulaParser ActiveParser)
        {
            if (Owner != null)
            {
                return (int)(Convert.ToInt32(ActiveParser.Evaluate(PowerFormula)) * (1 + Owner.UnitStat.AttackUpgrades.Value * 0.05));
            }
            else
            {
                return Convert.ToInt32(ActiveParser.Evaluate(PowerFormula));
            }
        }

        public void SetMaxAmmo(byte MaxAmmoToSet)
        {
            if (Parent != null)
            {
                Parent.SetMaxAmmo(MaxAmmoToSet);
            }
            else
            {
                _MaxAmmo = MaxAmmoToSet;
            }
        }

        public void RefillAmmo()
        {
            if (Parent != null)
            {
                Parent.RefillAmmo();
            }
            else
            {
                _Ammo = MaxAmmo;
            }
        }

        public void ConsumeAmmo()
        {
            if (Parent != null)
            {
                Parent.ConsumeAmmo();
            }
            else
            {
                _Ammo -= AmmoConsumption;
            }
        }

        public void IncreaseAmmo(byte AmmoToAdd)
        {
            if (Parent != null)
            {
                Parent.IncreaseAmmo(AmmoToAdd);
            }
            else
            {
                _Ammo = (byte)Math.Min(MaxAmmo, _Ammo + AmmoToAdd);
            }
        }

        public void EmptyAmmo()
        {
            if (Parent != null)
            {
                Parent.EmptyAmmo();
            }
            else
            {
                _Ammo = 0;
            }
        }

        public AttackAnimations GetAttackAnimations(FormulaParser ActiveParser)
        {
            return Animations[0].Animations;
        }

        public override string ToString()
        {
            return ItemName;
        }

        public int TerrainAttribute(byte TerrainIndex)
        {
            switch (Unit.Grades[DicRankByMovement[TerrainIndex]])
            {
                case 'S':
                    return 20;

                case 'A':
                    return 0;

                case 'B':
                    return -20;

                case 'C':
                    return -40;

                case 'D':
                    return -60;
            }
            return 0;
        }
    }
}
