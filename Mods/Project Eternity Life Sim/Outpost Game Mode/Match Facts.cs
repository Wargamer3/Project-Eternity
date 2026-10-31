using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class MatchFacts
    {
        public Vector3 OurFlag;
        public Vector3 EnemyFlag;
        public Vector3 DroppedFlag;
        public bool FlagIsDropped;
        public PlayerCharacter Holder;
        public PlayerCharacter EnemyCarrier;
        public List<Outpost> Outposts = new List<Outpost>();
        public int Eggs;
        public int Points;
        public int TurretCost = 50;
        public int WallCost = 20;
        public int MineCost = 25;
        public int SpikeCost = 15;
        public int RadarCost = 30;
        public int DepotCost = 40;
        public int JumpPadCost = 20;
        public int SpeedStripCost = 15;
        public int FireWallCost = 25;
        public float SecondsPerEgg = 10f;
        public float PointRemainder;
        public float EggRemainder;

        public int CountHeldBy(OutpostSide Side)
        {
            int Count = 0;
            for (int i = 0; i < Outposts.Count; i++)
            {
                if (Outposts[i].Side == Side)
                {
                    Count++;
                }
            }

            return Count;
        }

        public void TickBase(float Seconds)
        {
            if (Seconds <= 0f)
            {
                return;
            }

            PointRemainder += Seconds * CountHeldBy(OutpostSide.Ours);
            int Earned = (int)PointRemainder;
            if (Earned > 0)
            {
                Points += Earned;
                PointRemainder -= Earned;
            }

            if (SecondsPerEgg <= 0f)
            {
                return;
            }

            EggRemainder += Seconds;
            while (EggRemainder >= SecondsPerEgg)
            {
                Eggs++;
                EggRemainder -= SecondsPerEgg;
            }
        }

        public Outpost Nearest(Vector3 From, OutpostSide Side, bool NeedsLivingCreature)
        {
            Outpost Best = null;
            float BestDistance = float.MaxValue;
            for (int i = 0; i < Outposts.Count; i++)
            {
                Outpost Candidate = Outposts[i];
                if (Candidate.Side != Side)
                {
                    continue;
                }

                if (NeedsLivingCreature && !Candidate.CreatureAlive)
                {
                    continue;
                }

                float Distance = Vector3.Distance(From, Candidate.Position);
                if (Distance < BestDistance)
                {
                    Best = Candidate;
                    BestDistance = Distance;
                }
            }

            return Best;
        }
    }

    public enum OutpostSide
    {
        Empty,
        Ours,
        Enemy,
    }

    public class Outpost
    {
        public string Name;
        public MapInfo Chunk;
        public Vector3 Position;
        public OutpostSide Side;
        public bool CreatureAlive;
    }

    public static class MatchRole
    {
        public static bool Accepts(PlayerCharacter Character, string RoleName)
        {
            return Character != null && !Character.Forbids(RoleName);
        }

        public static AIObjective Order(PlayerCharacter Owner, LifeSimAIAction Action, Vector3 Place, float Leash, bool Standing)
        {
            if (Owner == null || Action == null || !Accepts(Owner, Action.AIActionName))
            {
                return null;
            }

            Action.Init(Owner.SharedMapContex);
            AIObjective Bottom = new AIObjective();
            Bottom.GoalName = Action.AIGoal;
            Bottom.ChosenAction = Action;
            Bottom.IsStandingOrder = Standing;
            Bottom.HasPlace = true;
            Bottom.Place = Place;
            Bottom.LeashRadius = Leash;
            return Bottom;
        }

        public static void AssignFlagRoles(IList<PlayerCharacter> Team, MatchFacts Match)
        {
            if (Team == null || Match == null)
            {
                return;
            }

            bool[] Taken = new bool[4];
            string[] Names = { "Take the Flag", "Guard our Flag", "Disrupt", "Escort the Holder" };
            for (int i = 0; i < Team.Count; i++)
            {
                PlayerCharacter Character = Team[i];
                for (int Role = 0; Role < Names.Length; Role++)
                {
                    if (Taken[Role] || !Accepts(Character, Names[Role]))
                    {
                        continue;
                    }

                    LifeSimAIAction Action = FlagAction(Role, Match);
                    float Leash = Role == 0 ? 0f : 40f;
                    Vector3 Place = Role == 0 ? Match.EnemyFlag : Match.OurFlag;
                    AIObjective Bottom = Order(Character, Action, Place, Leash, true);
                    if (Bottom == null)
                    {
                        continue;
                    }

                    Character.CommitBottom(Bottom);
                    Taken[Role] = true;
                    break;
                }
            }
        }

        static LifeSimAIAction FlagAction(int Role, MatchFacts Match)
        {
            if (Role == 0)
            {
                TakeTheFlagAction Take = new TakeTheFlagAction();
                Take.Match = Match;
                return Take;
            }

            if (Role == 1)
            {
                GuardOurFlagAction Guard = new GuardOurFlagAction();
                Guard.Match = Match;
                return Guard;
            }

            if (Role == 2)
            {
                DisruptAction Disrupt = new DisruptAction();
                Disrupt.Match = Match;
                return Disrupt;
            }

            EscortHolderAction Escort = new EscortHolderAction();
            Escort.Match = Match;
            return Escort;
        }
    }

    public class TakeTheFlagAction : LifeSimAIAction
    {
        public MatchFacts Match;

        public TakeTheFlagAction()
            : base("Take the Flag", "Take the Flag")
        {
        }

        public override AIAction Copy()
        {
            TakeTheFlagAction Clone = new TakeTheFlagAction();
            CopyTo(Clone);
            Clone.Match = Match;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            return true;
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (Params == null || Params.Owner == null || Match == null)
            {
                return new List<AutomatedAction>();
            }

            Vector3 Goal = Match.EnemyFlag;
            if (Match.FlagIsDropped)
            {
                Goal = Match.DroppedFlag;
            }

            if (Match.Holder == Params.Owner)
            {
                Goal = Match.OurFlag;
            }

            return LifePlan.GoTo(Params.Owner, Params.CurrentMapInfo, Goal);
        }

        public override bool Tracks(out Vector3 Point)
        {
            Point = Vector3.Zero;
            if (Params == null || Params.Owner == null || Match == null)
            {
                return false;
            }

            Point = Match.EnemyFlag;
            if (Match.FlagIsDropped)
            {
                Point = Match.DroppedFlag;
            }

            if (Match.Holder == Params.Owner)
            {
                Point = Match.OurFlag;
            }

            return true;
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = Params == null || Params.Owner == null || Match == null || !MatchRole.Accepts(Params.Owner, AIActionName) ? 0 : 1;
        }
    }

    public class GuardOurFlagAction : LifeSimAIAction
    {
        public MatchFacts Match;

        public GuardOurFlagAction()
            : base("Guard our Flag", "Guard our Flag")
        {
        }

        public override AIAction Copy()
        {
            GuardOurFlagAction Clone = new GuardOurFlagAction();
            CopyTo(Clone);
            Clone.Match = Match;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            return true;
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (Params == null || Params.Owner == null || Match == null)
            {
                return new List<AutomatedAction>();
            }

            return LifePlan.GoTo(Params.Owner, Params.CurrentMapInfo, Match.OurFlag);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = Params == null || Params.Owner == null || Match == null || !MatchRole.Accepts(Params.Owner, AIActionName) ? 0 : 1;
        }
    }

    public class DisruptAction : LifeSimAIAction
    {
        public MatchFacts Match;

        public DisruptAction()
            : base("Disrupt", "Disrupt")
        {
        }

        public override AIAction Copy()
        {
            DisruptAction Clone = new DisruptAction();
            CopyTo(Clone);
            Clone.Match = Match;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            return true;
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (Params == null || Params.Owner == null || Match == null)
            {
                return new List<AutomatedAction>();
            }

            if (Match.EnemyCarrier != null)
            {
                MapInfo Chunk = Match.EnemyCarrier.SharedMapContex.CurrentMapInfo;
                return LifePlan.GoTo(Params.Owner, Chunk, Match.EnemyCarrier.Position);
            }

            return LifePlan.GoTo(Params.Owner, Params.CurrentMapInfo, Match.OurFlag);
        }

        public override bool Tracks(out Vector3 Point)
        {
            Point = Match == null ? Vector3.Zero : Match.OurFlag;
            if (Params == null || Params.Owner == null || Match == null)
            {
                return false;
            }

            if (Match.EnemyCarrier != null)
            {
                Point = Match.EnemyCarrier.Position;
            }

            return true;
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = Params == null || Params.Owner == null || Match == null || !MatchRole.Accepts(Params.Owner, AIActionName) ? 0 : 1;
        }
    }

    public class EscortHolderAction : LifeSimAIAction
    {
        public MatchFacts Match;
        public Vector3 Offset = new Vector3(0f, 16f, 0f);

        public EscortHolderAction()
            : base("Escort the Holder", "Escort the Holder")
        {
        }

        public override AIAction Copy()
        {
            EscortHolderAction Clone = new EscortHolderAction();
            CopyTo(Clone);
            Clone.Match = Match;
            Clone.Offset = Offset;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            return true;
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (Params == null || Params.Owner == null || Match == null)
            {
                return new List<AutomatedAction>();
            }

            if (Match.Holder != null && Match.Holder != Params.Owner)
            {
                MapInfo Chunk = Match.Holder.SharedMapContex.CurrentMapInfo;
                return LifePlan.GoTo(Params.Owner, Chunk, Match.Holder.Position + Offset);
            }

            if (Match.FlagIsDropped)
            {
                return LifePlan.GoTo(Params.Owner, Params.CurrentMapInfo, Match.DroppedFlag);
            }

            return new List<AutomatedAction>();
        }

        public override bool Tracks(out Vector3 Point)
        {
            Point = Vector3.Zero;
            if (Params == null || Params.Owner == null || Match == null)
            {
                return false;
            }

            if (Match.Holder != null && Match.Holder != Params.Owner)
            {
                Point = Match.Holder.Position + Offset;
                return true;
            }

            if (Match.FlagIsDropped)
            {
                Point = Match.DroppedFlag;
                return true;
            }

            return false;
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = Params == null || Params.Owner == null || Match == null || !MatchRole.Accepts(Params.Owner, AIActionName) ? 0 : 1;
        }
    }
}
