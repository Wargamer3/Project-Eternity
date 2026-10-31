using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class FightStand
    {
        public static void WeaponBand(PlayerCharacter Character, out float Near, out float Far)
        {
            Near = 2f;
            Far = 2f;
            Weapon Weapon = Character == null ? null : Character.EquippedWeapon();
            if (Weapon == null)
            {
                return;
            }

            if (Weapon.NearRange > 0f)
            {
                Near = Weapon.NearRange;
            }

            if (Weapon.FarRange > 0f)
            {
                Far = Weapon.FarRange;
            }

            if (Far < Near)
            {
                Far = Near;
            }
        }

        public static CombatTaste TasteOf(PlayerCharacter Character)
        {
            if (Character == null)
            {
                return CombatTaste.Close;
            }

            foreach (Personality ActivePersonality in Character.ListPersonality)
            {
                if (ActivePersonality.HasCombatTaste)
                {
                    return ActivePersonality.Taste;
                }
            }

            return CombatTaste.Close;
        }

        public static Vector3 Point(PlayerCharacter Self, Vector3 Target, float Distance)
        {
            Vector3 Offset = Self.Position - Target;
            Offset.Z = 0f;
            if (Offset.LengthSquared() < 0.001f)
            {
                Offset = Vector3.UnitY;
            }

            Offset.Normalize();
            return Target + Offset * Distance;
        }

        public static Vector3 Choose(PlayerCharacter Self, Vector3 TargetPosition)
        {
            float Near;
            float Far;
            WeaponBand(Self, out Near, out Far);
            CombatTaste Taste = TasteOf(Self);
            float Distance = Personality.FightDistance(Taste, Near, Far);
            Vector3 Ideal = Point(Self, TargetPosition, Distance);
            if (Taste == CombatTaste.Hide)
            {
                Vector3 Cover;
                if (TryTag(Self, "Cover", TargetPosition, Far, out Cover))
                {
                    return Cover;
                }
            }

            if (Taste == CombatTaste.BehindTheirLines)
            {
                Vector3 Flank;
                if (TryTag(Self, "Flank", TargetPosition, float.MaxValue, out Flank))
                {
                    return Flank;
                }
            }

            return Ideal;
        }

        static bool TryTag(PlayerCharacter Self, string Tag, Vector3 Target, float Limit, out Vector3 Point)
        {
            Point = Vector3.Zero;
            if (Self == null || Self.Belief == null)
            {
                return false;
            }

            bool Found = false;
            float Best = float.MaxValue;
            foreach (BelievedObject Object in Self.Belief.ListObject)
            {
                if (Object.Tag != Tag)
                {
                    continue;
                }

                if (Vector3.Distance(Object.LocalPosition, Target) > Limit)
                {
                    continue;
                }

                float Distance = Vector3.Distance(Object.LocalPosition, Self.Position);
                if (Distance < Best)
                {
                    Best = Distance;
                    Point = Object.LocalPosition;
                    Found = true;
                }
            }

            return Found;
        }
    }

    public class FightAction : LifeSimAIAction
    {
        public PlayerCharacter Target;
        public float ThreatRadius = 48f;

        public FightAction()
            : base("Fight", "Fight")
        {
        }

        public override AIAction Copy()
        {
            FightAction Clone = new FightAction();
            CopyTo(Clone);
            Clone.Target = Target;
            Clone.ThreatRadius = ThreatRadius;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            return true;
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (!Engaged())
            {
                return new List<AutomatedAction>();
            }

            Vector3 Stand = FightStand.Choose(Params.Owner, Target.Position);
            List<AutomatedAction> Steps = LifePlan.GoTo(Params.Owner, Params.CurrentMapInfo, Stand);
            Steps.Add(new StrikeStep(Params.Owner, Target));
            return StanceSequence.Splice(Params.Owner.CurrentPose, Steps);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            Precision = FightStand.TasteOf(Params == null ? null : Params.Owner) == CombatTaste.Snipe ? 1 : 0;
            AIWeight = Engaged() ? 1 : 0;
            Urgency = Engaged() ? PlayerCharacter.ShortTermUrgencyMargin : 0;
        }

        public override bool Tracks(out Vector3 Point)
        {
            Point = Vector3.Zero;
            if (!Engaged())
            {
                return false;
            }

            Point = FightStand.Choose(Params.Owner, Target.Position);
            return true;
        }

        bool Engaged()
        {
            if (Params == null || Params.Owner == null || Target == null)
            {
                return false;
            }

            return Vector3.Distance(Params.Owner.Position, Target.Position) <= ThreatRadius;
        }
    }

    public class StrikeStep : LifeSimStep
    {
        public Pose PoseAtHit;
        private readonly PlayerCharacter Target;
        private bool Struck;

        public StrikeStep(PlayerCharacter Owner, PlayerCharacter Target)
            : base("Strike", Owner, 0.3)
        {
            this.Target = Target;
        }

        public override void OnStarted()
        {
            PoseAtHit = Owner.CurrentPose;
            Struck = false;
        }

        public override bool Update(GameTime gameTime)
        {
            if (!Struck)
            {
                PoseAtHit = Owner.CurrentPose;
                Struck = true;
            }

            return base.Update(gameTime);
        }

        protected override void OnFinished()
        {
            if (Target == null || Owner == null)
            {
                return;
            }

            Weapon Weapon = Owner.EquippedWeapon();
            int Damage = Weapon == null ? 0 : Weapon.DamageAmount;
            if (Damage <= 0)
            {
                return;
            }

            Target.CurrentHP -= Damage;
            if (Target.CurrentHP > 0)
            {
                return;
            }

            Target.ApplyReaction(new IncapacitatedStep(Target));
        }

        protected override AutomatedAction Copy()
        {
            return new StrikeStep(Owner, Target);
        }
    }

    public class IncapacitatedStep : LifeSimStep
    {
        public IncapacitatedStep(PlayerCharacter Owner)
            : base("Incapacitated", Owner, 1)
        {
        }

        public override Pose PoseNeeded => Pose.Incapacitated;

        public override Pose PoseLeft => Pose.Incapacitated;

        public override bool Update(GameTime gameTime)
        {
            return true;
        }

        protected override AutomatedAction Copy()
        {
            return new IncapacitatedStep(Owner);
        }
    }
}
