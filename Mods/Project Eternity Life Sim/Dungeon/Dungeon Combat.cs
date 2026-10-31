using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class DungeonCombat
    {
        public static bool BeginStrike(PlayerCharacter Actor, PlayerCharacter Target, double Seconds)
        {
            if (Actor == null || Target == null || Actor.SwingRemaining > 0 || Actor.CurrentHP <= 0 || Target.CurrentHP <= 0)
            {
                return false;
            }

            Weapon Attack = Actor.EquippedWeapon();
            if (Attack == null)
            {
                return false;
            }

            if (Attack.ListTraits == null)
            {
                Attack.ListTraits = new List<Trait>();
            }

            Actor.DayAttackBonus = Actor.Life.AttackNow(Actor.SimHour);
            Skirmish Field = new Skirmish();
            int Saved = Actor.ActionsLeft;
            Field.Strikes.Make(Actor, Target, Attack, 18, 4, false, "", false);
            Actor.ActionsLeft = Saved;
            if (Field.LastResult == "Out of reach." || Field.LastResult == "No target." || Field.LastResult == "Not loaded." || Field.LastResult == "Muzzled.")
            {
                return false;
            }

            Actor.SwingRemaining = Seconds < 0.2 ? 0.2 : Seconds;
            return true;
        }

        public static bool BeginSpell(PlayerCharacter Caster, PlayerCharacter Target, SpellCard Card)
        {
            if (Caster == null || Card == null || Card.Spell == null || Caster.SwingRemaining > 0 || Caster.CurrentHP <= 0)
            {
                return false;
            }

            Skirmish Field = new Skirmish();
            int Saved = Caster.ActionsLeft;
            if (Caster.ActionsLeft < Card.Actions)
            {
                Caster.ActionsLeft = Card.Actions;
            }

            Field.Spells.Cast(Caster, Card, Target, 16, Card.BaseRank, 4);
            Caster.ActionsLeft = Saved;
            if (Field.LastResult == "No slot." || Field.LastResult == "No actions left.")
            {
                return false;
            }

            double Seconds = 0.35 + Card.Actions * 0.25;
            Caster.SwingRemaining = Seconds;
            return true;
        }

        public static void Tick(PlayerCharacter Who, double Seconds)
        {
            if (Who == null || Seconds <= 0)
            {
                return;
            }

            Who.SwingRemaining -= Seconds;
            if (Who.SwingRemaining < 0)
            {
                Who.SwingRemaining = 0;
            }
        }
    }

    public static class Social
    {
        public const float FriendlyAt = 40f;
        public const float BestFriendAt = 70f;

        public static void Talk(CharacterRelationship Bond, float Amount)
        {
            if (Bond == null)
            {
                return;
            }

            Bond.AddProgress(Amount);
            Promote(Bond);
        }

        public static void Promote(CharacterRelationship Bond)
        {
            if (Bond == null || Bond.Status == RelationshipStatus.Lover || Bond.Status == RelationshipStatus.Married)
            {
                return;
            }

            if (Bond.Progress >= BestFriendAt)
            {
                Bond.Status = RelationshipStatus.BestFriend;
            }
            else if (Bond.Progress >= FriendlyAt)
            {
                Bond.Status = RelationshipStatus.Friendly;
            }
        }

        public static bool CanVisit(RelationshipStatus Status)
        {
            return Status == RelationshipStatus.Friendly || Status == RelationshipStatus.BestFriend || Status == RelationshipStatus.Lover || Status == RelationshipStatus.Married;
        }

        public static bool CanInvite(RelationshipStatus Status, int Beds, int Residents)
        {
            if (Beds <= Residents)
            {
                return false;
            }

            return Status == RelationshipStatus.BestFriend || Status == RelationshipStatus.Lover || Status == RelationshipStatus.Married;
        }
    }

    public static class Rescue
    {
        public static bool Free(DungeonParty Party, DungeonSpawn Captive, PlayerCharacter Saver, out bool SentHome)
        {
            SentHome = false;
            if (Party == null || Captive == null || Captive.Kind != DungeonPersonKind.Captive || Captive.Freed)
            {
                return false;
            }

            Captive.Freed = true;
            if (Saver != null && Captive.Body != null)
            {
                Social.Talk(Saver.RelationshipWith(Captive.Body), 45f);
            }

            bool Joined = Party.Recruit(Captive.Name, Captive.Name, Captive.Trade, out SentHome);
            PartyMember Member = Party.Find(Captive.Name);
            if (Member != null)
            {
                Member.Body = Captive.Body;
                Trades.Give(Captive.Body, Captive.Trade);
                if (Captive.Body != null)
                {
                    Member.Tool = Captive.Body.Life.Tool;
                }
            }

            return Joined;
        }
    }
}
