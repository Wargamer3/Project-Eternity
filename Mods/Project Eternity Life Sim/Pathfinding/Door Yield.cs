using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class StepAsideAction : LifeSimStep
    {
        readonly Vector3 Spot;

        public StepAsideAction(PlayerCharacter Owner, Vector3 Spot)
            : base("Step Aside", Owner, 0.2)
        {
            this.Spot = Spot;
        }

        public override void OnStarted()
        {
            Owner.SetPosition(Spot);
        }

        protected override AutomatedAction Copy()
        {
            return new StepAsideAction(Owner, Spot);
        }
    }

    public static class DoorYield
    {
        public const float DoorRadius = 12f;

        public static void Settle(PlayerCharacter A, PlayerCharacter B, Vector3 Door, Vector3 Beside)
        {
            if (A == null || B == null)
            {
                return;
            }

            bool ACrossing = IsCrossing(A);
            bool BCrossing = IsCrossing(B);
            if (ACrossing && BCrossing)
            {
                PlayerCharacter Far = Vector3.Distance(A.Position, Door) > Vector3.Distance(B.Position, Door) ? A : B;
                InsertWait(Far);
                return;
            }

            if (!ACrossing && InDoor(A, Door))
            {
                Sidestep(A, Beside);
            }
            else if (!BCrossing && InDoor(B, Door))
            {
                Sidestep(B, Beside);
            }
        }

        public static void Consider(PlayerCharacter Self)
        {
            if (Self == null || Self.SharedMapContex == null || Self.SharedMapContex.CurrentMapInfo == null)
            {
                return;
            }

            MapInfo Here = Self.SharedMapContex.CurrentMapInfo;
            if (Here.People == null || Here.ListMarker == null)
            {
                return;
            }

            for (int m = 0; m < Here.ListMarker.Count; m++)
            {
                WorldMarker Marker = Here.ListMarker[m];
                if (Marker.Kind != WorldMarkerKind.Door)
                {
                    continue;
                }

                for (int c = 0; c < Here.People.Count; c++)
                {
                    PlayerCharacter Other = Here.People[c];
                    if (Other == null || Other == Self)
                    {
                        continue;
                    }

                    if (Vector3.Distance(Self.Position, Marker.LocalPosition) > DoorRadius && Vector3.Distance(Other.Position, Marker.LocalPosition) > DoorRadius)
                    {
                        continue;
                    }

                    Vector3 Side = Marker.LocalPosition + new Vector3(20f, 0f, 0f);
                    Settle(Self, Other, Marker.LocalPosition, Side);
                }
            }
        }

        public static bool IsCrossing(PlayerCharacter Who)
        {
            AIObjective Top = Who.TopObjective();
            if (Top == null || Top.ListRemainingStep.Count == 0)
            {
                return false;
            }

            AutomatedAction Step = Top.ListRemainingStep[0];
            return Step is FollowPath || Step is CrossPortalAction;
        }

        static bool InDoor(PlayerCharacter Who, Vector3 Door)
        {
            return Vector3.Distance(Who.Position, Door) <= DoorRadius;
        }

        static void Sidestep(PlayerCharacter Who, Vector3 Beside)
        {
            AIObjective Top = Who.TopObjective();
            if (Top == null)
            {
                return;
            }

            if (Top.ListRemainingStep.Count > 0 && Top.ListRemainingStep[0] is StepAsideAction)
            {
                return;
            }

            StepAsideAction Aside = new StepAsideAction(Who, Beside);
            Top.ListRemainingStep.Insert(0, Aside);
            Top.Yielding = true;
            Who.ListCurrentAIObjectiveStep = Top.ListRemainingStep;
            Aside.OnStarted();
        }

        static void InsertWait(PlayerCharacter Who)
        {
            AIObjective Top = Who.TopObjective();
            if (Top == null || Top.ListRemainingStep.Count == 0 || Top.ListRemainingStep[0] is WaitAction)
            {
                return;
            }

            WaitAction Pause = new WaitAction(Who, 1);
            Top.ListRemainingStep.Insert(0, Pause);
            Top.Yielding = true;
            Who.ListCurrentAIObjectiveStep = Top.ListRemainingStep;
            Pause.OnStarted();
        }
    }
}
