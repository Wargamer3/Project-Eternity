using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum SkirmishSide
    {
        Hero,
        Monster,
    }

    public class SpellCard
    {
        public string Name = "";
        public Trait Spell;
        public int Actions = 2;
        public int BaseRank = 1;
        public bool Cantrip;
    }

    public class FormCard
    {
        public string Name = "";
        public int ArmorClass = 10;
        public int SpeedFeet = 25;
        public Weapon Strike;
        public PlayerCharacter Who;
    }

    public class SkirmishZone
    {
        public PlayerCharacter Owner;
        public Vector3 Position;
        public float Feet;
        public Trait Effect;
        public int Rank = 1;
        public bool HalveHeal;
        public bool Renewed;
    }

    public class Skirmish
    {
        public readonly List<PlayerCharacter> Bodies = new List<PlayerCharacter>();
        public readonly List<PlayerCharacter> Order = new List<PlayerCharacter>();
        public readonly List<SkirmishZone> Zones = new List<SkirmishZone>();
        public readonly List<FormCard> Forms = new List<FormCard>();
        public readonly List<PlayerCharacter> Summoned = new List<PlayerCharacter>();
        public int Turn;
        public string LastResult = "";
        public int LastDamage;
        public int LastPenalty;
        public bool InReaction;

        public readonly Strike Strikes;
        public readonly Activities Actions;
        public readonly Spellcast Spells;

        public Skirmish()
        {
            Strikes = new Strike(this);
            Actions = new Activities(this);
            Spells = new Spellcast(this);
        }

        public PlayerCharacter Active
        {
            get
            {
                if (Turn < 0 || Turn >= Order.Count)
                {
                    return null;
                }

                return Order[Turn];
            }
        }

        public void Add(PlayerCharacter Body)
        {
            if (Body != null)
            {
                Bodies.Add(Body);
            }
        }









        public void Begin(int[] Rolls)
        {
            Order.Clear();
            for (int i = 0; i < Bodies.Count; i++)
            {
                PlayerCharacter Body = Bodies[i];
                if (Body.SkipTurn || Body.Owner != null)
                {
                    continue;
                }

                int Roll = 0;
                if (Rolls != null && i < Rolls.Length)
                {
                    Roll = Rolls[i];
                }

                Body.Initiative = Roll + Body.Perception;
                Order.Add(Body);
            }

            Order.Sort(CompareInitiative);
            Turn = 0;
            if (Active != null)
            {
                StartTurn(Active);
            }
        }


        internal static int CompareInitiative(PlayerCharacter A, PlayerCharacter B)
        {
            return B.Initiative.CompareTo(A.Initiative);
        }


        public void EndTurn()
        {
            PlayerCharacter Who = Active;
            if (Who != null)
            {
                TickPersistent(Who, 0, 0);
                Who.OffGuardMelee = false;
                Who.OffGuardMeleeFrom = null;
            }

            for (int i = 0; i < Zones.Count; i++)
            {
                if (Zones[i].Owner != Who)
                {
                    continue;
                }

                if (!Zones[i].Renewed)
                {
                    Zones.RemoveAt(i);
                    i--;
                    continue;
                }

                Zones[i].Renewed = false;
            }

            Turn++;
            if (Turn >= Order.Count)
            {
                Turn = 0;
            }

            int Guard = 0;
            while (Active != null && Active.CurrentHP <= 0 && Guard < Order.Count)
            {
                Turn++;
                if (Turn >= Order.Count)
                {
                    Turn = 0;
                }

                Guard++;
            }

            if (Active != null)
            {
                StartTurn(Active);
                LastResult = Active.Name + " starts. " + Active.ActionsLeft + " actions.";
            }
        }


        public void StartTurn(PlayerCharacter Who)
        {
            if (Who == null)
            {
                return;
            }

            Who.ActionsLeft = 3;
            Who.StrikesThisTurn = 0;
            Who.Attacked = false;
            Who.FlourishUsed = false;
            Who.Swapped = false;
            Who.CircumstanceArmor = 0;
            Who.RaisedShield = false;
            Who.ReachBonusFeet = 0;
            Who.ReactionLeft = 1;
            Who.ExtraReactionLeft = string.IsNullOrEmpty(Who.ExtraReaction) ? 0 : 1;
            for (int i = 0; i < Bodies.Count; i++)
            {
                if (Bodies[i].OffGuardFrom == Who)
                {
                    Bodies[i].OffGuard = false;
                    Bodies[i].OffGuardFrom = null;
                }
            }

            if (Who.Companion != null)
            {
                Who.Companion.ActionsLeft = Who.Companion.CompanionRank >= 1 ? 1 : 0;
            }

            Spells.TickZones(Who);
        }


        public void Stride(PlayerCharacter Who, Vector3 Dest)
        {
            if (Who == null || Who.ActionsLeft < 1 || Who.CurrentHP <= 0)
            {
                LastResult = "No actions left.";
                return;
            }

            Move(Who, Dest, Who.SpeedFeet - Who.SpeedPenaltyFeet);
            Who.ActionsLeft--;
            Scuttle(Who);
            LastResult = Who.Name + " strides.";
        }


        public void Move(PlayerCharacter Who, Vector3 Dest, float Feet)
        {
            if (Who == null)
            {
                return;
            }

            float Budget = PracticeMath.ToUnits(Feet);
            if (Budget < 0f)
            {
                Budget = 0f;
            }

            Vector3 Delta = Dest - Who.Position;
            Delta.Z = 0f;
            float Length = Delta.Length();
            if (Length > Budget && Length > 0.001f)
            {
                Dest = Who.Position + Delta * (Budget / Length);
            }

            Dest.Z = Who.Position.Z;
            Who.Position = Dest;
        }


        public void Step(PlayerCharacter Who, Vector3 Dest)
        {
            Move(Who, Dest, 5f);
        }




        public void MonsterAct(PlayerCharacter Monster, int Natural, int DieFace)
        {
            if (Monster != null && Monster.Charmed)
            {
                LastResult = "Charmed.";
                return;
            }

            PlayerCharacter Foe = Nearest(Monster, SkirmishSide.Hero);
            if (Monster == null || Foe == null)
            {
                LastResult = "No target.";
                return;
            }

            float Distance = Vector3.Distance(Monster.Position, Foe.Position);
            Weapon Melee = null;
            Weapon Bow = null;
            List<Weapon> Hands = FightRoll.WeaponsOf(Monster);
            for (int i = 0; i < Hands.Count; i++)
            {
                Weapon Attack = Hands[i];
                if (Attack == null)
                {
                    continue;
                }

                if (Attack.WeaponType == Weapon.WeaponTypes.Range && Bow == null)
                {
                    Bow = Attack;
                }

                if (Attack.WeaponType != Weapon.WeaponTypes.Range && Melee == null)
                {
                    Melee = Attack;
                }
            }

            if (Melee != null && Distance <= PracticeMath.ToUnits(Melee.FarRange / PracticeMath.FeetToMeters))
            {
                Strikes.Make(Monster, Foe, Melee, Natural, DieFace);
                return;
            }

            if (Bow != null && Distance <= PracticeMath.ToUnits(Bow.FarRange / PracticeMath.FeetToMeters))
            {
                Strikes.Make(Monster, Foe, Bow, Natural, DieFace);
                return;
            }

            Stride(Monster, Foe.Position);
        }



















        public void RaiseShield(PlayerCharacter Who)
        {
            if (Who == null || Who.ActionsLeft < 1)
            {
                return;
            }

            Who.ActionsLeft--;
            Who.RaisedShield = true;
            Who.BlockHardness = Who.ShieldHardness;
            LastResult = "Shield raised.";
        }


        public int ShieldBlock(PlayerCharacter Who, PlayerCharacter Attacker, int Damage)
        {
            if (Who == null || Who.BlockHardness <= 0 || !SpendReaction(Who, "Shield Block"))
            {
                return Damage;
            }

            int Left = Damage - Who.BlockHardness;
            if (Left < 0)
            {
                Left = 0;
            }

            Who.RaisedShield = false;
            Who.CircumstanceArmor = 0;
            if (Who.BreakPackets.Count > 0 && Attacker != null && Vector3.Distance(Who.Position, Attacker.Position) <= PracticeMath.ToUnits(5f))
            {
                int Shard = FightRoll.PacketAmount(Who.BreakPackets[0], 4, false);
                Shard = FightRoll.BasicAmount(Shard, PracticeMath.Degree(10, Attacker.Reflex, 0, 10 + Spells.SpellBonus(Who)));
                FightRoll.ApplyHurt(Attacker, Shard);
            }

            Who.BreakPackets.Clear();
            LastResult = "Shield block. " + Left + " gets through.";
            return Left;
        }















        internal int OfferDefense(PlayerCharacter Target, string Trigger)
        {
            if (Target == null)
            {
                return 0;
            }

            StrikeRoll Roll = new StrikeRoll();
            Roll.Trigger = Trigger;
            Roll.ReactionAvailable = Target.ReactionLeft > 0 || Target.ExtraReactionLeft > 0;
            FightRoll.RunTraits(Target.Feats, Roll);
            if (Trigger == "Ranged")
            {
                Roll.Trigger = "Targeted";
                FightRoll.RunTraits(Target.Feats, Roll);
            }

            if (Roll.SpendReaction && (Roll.GrantArmor > 0 || Roll.StepFeet > 0 || Roll.RaiseShield))
            {
                SpendReaction(Target, "Defense");
            }

            if (Roll.RaiseShield)
            {
                Target.RaisedShield = true;
            }

            if (Roll.StepFeet > 0)
            {
                Vector3 Stepped = Target.Position;
                Stepped.X += PracticeMath.ToUnits(Roll.StepFeet);
                Target.Position = Stepped;
            }

            if (Roll.FreeStrike && !InReaction)
            {
                InReaction = true;
                int Made = Target.StrikesThisTurn;
                Target.StrikesThisTurn = 0;
                PlayerCharacter Foe = Nearest(Target, FightRoll.Opposite(Target.Side));
                if (Foe != null)
                {
                    Strikes.Make(Target, Foe, Target.CurrentWeapon(), 10, 4, false, "Nimble Strike", true);
                }

                Target.StrikesThisTurn = Made;
                InReaction = false;
            }

            int Bonus = Target.RaisedShield ? Target.ShieldBonus : 0;
            if (Target.CircumstanceArmor > Bonus)
            {
                Bonus = Target.CircumstanceArmor;
            }

            if (Roll.GrantArmor > Bonus)
            {
                Bonus = Roll.GrantArmor;
            }

            return Bonus;
        }


        internal int OfferBlock(PlayerCharacter Target, int Damage, PlayerCharacter Attacker)
        {
            if (Target == null || Damage <= 0)
            {
                return Damage;
            }

            if (Target.BlockHardness > 0 && (Target.RaisedShield || Target.CircumstanceArmor > 0))
            {
                return ShieldBlock(Target, Attacker, Damage);
            }

            for (int i = 0; i < Bodies.Count; i++)
            {
                PlayerCharacter Ally = Bodies[i];
                if (Ally.Side != Target.Side || Ally == Target || Ally.CurrentHP <= 0)
                {
                    continue;
                }

                if (Vector3.Distance(Ally.Position, Target.Position) > PracticeMath.ToUnits(5f))
                {
                    continue;
                }

                if (!Actions.HasActivity(Ally, "Shield Warden") || Ally.BlockHardness <= 0)
                {
                    continue;
                }

                return ShieldBlock(Ally, Attacker, Damage);
            }

            return Damage;
        }


        internal void OfferStrikeBack(PlayerCharacter Defender, PlayerCharacter Attacker, int DieFace)
        {
            if (InReaction || Defender == null)
            {
                return;
            }

            StrikeRoll Roll = new StrikeRoll();
            Roll.Trigger = "CriticalFailure";
            FightRoll.RunTraits(Defender.Feats, Roll);
            if (!Roll.StrikeBack || !SpendReaction(Defender, "Strike Back"))
            {
                return;
            }

            InReaction = true;
            Strikes.Make(Defender, Attacker, Defender.CurrentWeapon(), 12, DieFace, false, "Quick Reversal", false);
            InReaction = false;
        }


        internal void Scuttle(PlayerCharacter Mover)
        {
            if (Mover == null)
            {
                return;
            }

            for (int i = 0; i < Bodies.Count; i++)
            {
                PlayerCharacter Ally = Bodies[i];
                if (Ally == Mover || Ally.Side != Mover.Side || Ally.CurrentHP <= 0 || Ally.ReactionLeft <= 0)
                {
                    continue;
                }

                if (Vector3.Distance(Ally.Position, Mover.Position) > PracticeMath.ToUnits(5f))
                {
                    continue;
                }

                StrikeRoll Roll = new StrikeRoll();
                Roll.Trigger = "AllyMoved";
                Roll.ReactionAvailable = true;
                FightRoll.RunTraits(Ally.Feats, Roll);
                float Feet = Roll.StepFeet;
                if (Feet <= 0f && FightRoll.Named(Ally, "Goblin Scuttle"))
                {
                    Feet = 5f;
                }

                if (Feet > 0f && SpendReaction(Ally, "Step"))
                {
                    Vector3 Stepped = Ally.Position;
                    Stepped.X += PracticeMath.ToUnits(Feet);
                    Ally.Position = Stepped;
                }
            }
        }


        internal void Note(PlayerCharacter Who, string Trigger)
        {
            StrikeRoll Roll = new StrikeRoll();
            Roll.Trigger = Trigger;
            FightRoll.RunTraits(Who.Feats, Roll);
            if (Roll.NextDamage > 0)
            {
                Who.NextDamage = Roll.NextDamage;
                Who.NextDamageGroup = Roll.NextDamageGroup;
            }
        }



        internal bool SpendReaction(PlayerCharacter Who, string Use)
        {
            if (Who == null)
            {
                return false;
            }

            if (Who.ExtraReactionLeft > 0 && (Who.ExtraReaction == Use || Who.ExtraReaction == "Shield Block" && Use == "Shield Block"))
            {
                Who.ExtraReactionLeft--;
                return true;
            }

            if (Who.ReactionLeft > 0)
            {
                Who.ReactionLeft--;
                return true;
            }

            return false;
        }


        internal int ArmorOf(PlayerCharacter Target, int Ignore, int DefenseBonus)
        {
            int Circumstance = DefenseBonus;
            if (Ignore > 0)
            {
                Circumstance -= Ignore;
                if (Circumstance < 0)
                {
                    Circumstance = 0;
                }
            }

            int Base = Target.PrintedArmor ? Target.AC : PracticeMath.GearArmorClass(Target.DEX, Target.DexCap, Target.ItemArmor, Target.ArmorProficiency, 0);
            if (Target.Form != null)
            {
                Base = Target.Form.ArmorClass;
            }

            int Penalty = 0;
            if (Target.OffGuard || Target.OffGuardMelee || Target.Prone || Target.Grabbed)
            {
                Penalty = 2;
            }

            return Base + Circumstance - Penalty - Target.Frightened;
        }


        internal bool TargetOffGuard(PlayerCharacter Actor, PlayerCharacter Target, bool Ranged)
        {
            if (Target.OffGuard || Target.Prone || Target.Grabbed)
            {
                return true;
            }

            if (Target.OffGuardMelee && !Ranged && Target.OffGuardMeleeFrom == Actor)
            {
                return true;
            }

            return Flanked(Target, Actor);
        }


        internal bool Flanked(PlayerCharacter Target, PlayerCharacter Actor)
        {
            if (Target == null || Actor == null)
            {
                return false;
            }

            Vector3 ToActor = Actor.Position - Target.Position;
            ToActor.Z = 0f;
            if (ToActor.LengthSquared() < 0.01f || Vector3.Distance(Actor.Position, Target.Position) > PracticeMath.ToUnits(5f))
            {
                return false;
            }

            ToActor.Normalize();
            for (int i = 0; i < Bodies.Count; i++)
            {
                PlayerCharacter Other = Bodies[i];
                if (Other == Actor || Other.Side != Actor.Side || Other.CurrentHP <= 0)
                {
                    continue;
                }

                if (Vector3.Distance(Other.Position, Target.Position) > PracticeMath.ToUnits(5f))
                {
                    continue;
                }

                Vector3 ToOther = Other.Position - Target.Position;
                ToOther.Z = 0f;
                if (ToOther.LengthSquared() < 0.01f)
                {
                    continue;
                }

                ToOther.Normalize();
                if (Vector3.Dot(ToActor, ToOther) < -0.5f)
                {
                    return true;
                }
            }

            return false;
        }













        public int TickPersistent(PlayerCharacter Who, int Flat, int DieFace)
        {
            if (Who == null || Who.Persistent.Count == 0)
            {
                return 0;
            }

            int Check = Flat;
            if (Check < 1 || Check > 20)
            {
                Check = RandomHelper.Next(20) + 1;
            }

            int Damage = 0;
            for (int i = Who.Persistent.Count - 1; i >= 0; i--)
            {
                DamagePacket Packet = Who.Persistent[i];
                int Amount = FightRoll.ApplyResistance(Who, Packet.Type, FightRoll.PacketAmount(Packet, DieFace, false));
                FightRoll.ApplyHurt(Who, Amount);
                Damage += Amount;
                if (Check >= 15)
                {
                    Who.Persistent.RemoveAt(i);
                }
            }

            if (Damage > 0)
            {
                LastDamage = Damage;
            }

            return Damage;
        }








        internal PlayerCharacter Nearest(PlayerCharacter From, SkirmishSide Side)
        {
            PlayerCharacter Best = null;
            float Score = float.MaxValue;
            for (int i = 0; i < Bodies.Count; i++)
            {
                PlayerCharacter Body = Bodies[i];
                if (Body == From || Body.Side != Side || Body.CurrentHP <= 0)
                {
                    continue;
                }

                float Distance = Vector3.Distance(From.Position, Body.Position);
                if (Distance < Score)
                {
                    Score = Distance;
                    Best = Body;
                }
            }

            return Best;
        }






    }
}
