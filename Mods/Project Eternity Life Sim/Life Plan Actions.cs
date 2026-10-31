using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class LifeSimStep : AutomatedAction
    {
        private double Remaining;

        protected LifeSimStep(string Name, PlayerCharacter Owner, double Seconds)
            : base(Name, Owner)
        {
            Remaining = Seconds;
        }

        public override Pose PoseNeeded => Pose.Stand;

        public override Pose PoseLeft => Pose.Stand;

        public override void OnStarted()
        {
        }

        public override bool Update(GameTime gameTime)
        {
            Remaining -= gameTime.ElapsedGameTime.TotalSeconds;
            if (Remaining > 0)
            {
                return true;
            }

            OnFinished();
            return false;
        }

        protected virtual void OnFinished()
        {
        }

        public override void OnCancel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override void DrawIcon(CustomSpriteBatch g, Vector2 Position)
        {
        }
    }

    public class EatMealAction : LifeSimStep
    {
        private readonly bool ConsumesJerky;

        public EatMealAction(PlayerCharacter Owner, bool ConsumesJerky)
            : base("Eat", Owner, 0.4)
        {
            this.ConsumesJerky = ConsumesJerky;
        }

        protected override void OnFinished()
        {
            if (ConsumesJerky)
            {
                Owner.ConsumeCarried("Jerky");
            }

            Kitchen.Eat(Owner.Life, Kitchen.FromNothing(), Owner.SimHour, Owner);
        }

        protected override AutomatedAction Copy()
        {
            return new EatMealAction(Owner, ConsumesJerky);
        }
    }

    public class OpenContainerAction : LifeSimStep
    {
        public OpenContainerAction(PlayerCharacter Owner)
            : base("Open", Owner, 0.3)
        {
        }

        protected override AutomatedAction Copy()
        {
            return new OpenContainerAction(Owner);
        }
    }

    public class TakeItemAction : LifeSimStep
    {
        private readonly BelievedObject Source;
        private readonly string ItemName;

        public TakeItemAction(PlayerCharacter Owner, BelievedObject Source, string ItemName)
            : base("Take " + ItemName, Owner, 0.3)
        {
            this.Source = Source;
            this.ItemName = ItemName;
        }

        protected override void OnFinished()
        {
            Item Carried = new Item();
            Carried.Name = ItemName;
            Carried.ArrayTags = new string[] { ItemName };
            Owner.AddItem(new ItemInfo(Carried, 1));

            if (Source == null)
            {
                return;
            }

            Source.Tag = string.Empty;
            Owner.Belief.ListObject.Remove(Source);
            if (Source.Chunk == null)
            {
                return;
            }

            foreach (WorldMarker Marker in Source.Chunk.ListMarker)
            {
                if (Marker.Name == Source.Name)
                {
                    Marker.Tag = string.Empty;
                }
            }
        }

        protected override AutomatedAction Copy()
        {
            return new TakeItemAction(Owner, Source, ItemName);
        }
    }

    public class DoorInteractionAction : LifeSimStep
    {
        private readonly BelievedObject Door;
        private readonly PassageState Result;
        private readonly double Seconds;

        public DoorInteractionAction(PlayerCharacter Owner, BelievedObject Door, string Interaction, PassageState Result, double Seconds)
            : base(Interaction, Owner, Seconds)
        {
            this.Door = Door;
            this.Result = Result;
            this.Seconds = Seconds;
        }

        protected override void OnFinished()
        {
            if (Door == null)
            {
                return;
            }

            if (Result == PassageState.Open && Door.Chunk != null)
            {
                foreach (WorldMarker Marker in Door.Chunk.ListMarker)
                {
                    if (Marker.Name != Door.Name || Marker.State != PassageState.Locked)
                    {
                        continue;
                    }

                    Owner.Belief.LearnThatItIsLocked(Door, Marker.Link);
                    Owner.PassageAttemptFailed = true;
                    return;
                }
            }

            Door.State = Result;
            if (Door.Chunk == null)
            {
                return;
            }

            bool Through = Passage.LetsPeopleThrough(Result);
            foreach (WorldMarker Marker in Door.Chunk.ListMarker)
            {
                if (Marker.Name != Door.Name)
                {
                    continue;
                }

                Marker.State = Result;
                if (Marker.Link != null)
                {
                    Marker.Link.IsOpen = Through;
                    Owner.Belief.SetLinkOpen(Marker.Link.FromChunk, Marker.Link.ToChunk, Through);
                }
            }
        }

        protected override AutomatedAction Copy()
        {
            return new DoorInteractionAction(Owner, Door, ActionName, Result, Seconds);
        }
    }

    public class CrossPortalAction : LifeSimStep
    {
        private readonly ChunkHop Hop;
        private readonly double Seconds;

        public CrossPortalAction(PlayerCharacter Owner, ChunkHop Hop)
            : this(Owner, Hop, 0.1)
        {
        }

        public CrossPortalAction(PlayerCharacter Owner, ChunkHop Hop, double Seconds)
            : base("Cross Portal", Owner, Seconds)
        {
            this.Hop = Hop;
            this.Seconds = Seconds;
        }

        public static void Land(PlayerCharacter Owner, ChunkHop Hop)
        {
            if (Owner == null || Hop == null)
            {
                return;
            }

            WorldMarker Blocking = Passage.Find(Hop);
            if (Blocking != null && Blocking.State == PassageState.Locked)
            {
                BelievedObject Known = Owner.Belief.FindObject(Blocking.Name);
                if (Known == null)
                {
                    Known = new BelievedObject();
                    Known.Name = Blocking.Name;
                    Known.Kind = Blocking.Kind;
                    Known.Chunk = Hop.FromChunk;
                    Owner.Belief.ListObject.Add(Known);
                }

                Owner.Belief.LearnThatItIsLocked(Known, Hop);
                Owner.PassageAttemptFailed = true;
                return;
            }

            if (Blocking != null && Blocking.State == PassageState.Closed)
            {
                Blocking.State = PassageState.Open;
                if (Blocking.Link != null)
                {
                    Blocking.Link.IsOpen = true;
                }

                BelievedObject Known = Owner.Belief.FindObject(Blocking.Name);
                if (Known != null)
                {
                    Known.State = PassageState.Open;
                }

                Owner.Belief.SetLinkOpen(Hop.FromChunk, Hop.ToChunk, true);
            }

            Vector3 Landing = new Vector3(Hop.ToPosition.X, Hop.ToPosition.Y, Owner.Position.Z);
            if (Hop.ToChunk != null && Owner.SharedMapContex != null)
            {
                Vector3 FromShift = ChunkOrigin.Shift(Owner.SharedMapContex.CurrentMapInfo);
                float WorldX = Owner.Position.X + FromShift.X;
                float WorldY = Owner.Position.Y + FromShift.Z;
                if (ChunkCross.At(WorldX, WorldY) == Hop.ToChunk)
                {
                    Vector3 ToShift = ChunkOrigin.Shift(Hop.ToChunk);
                    Landing = new Vector3(WorldX - ToShift.X, WorldY - ToShift.Z, Owner.Position.Z);
                }

                if (Owner.SharedMapContex.CurrentMapInfo != null)
                {
                    Owner.SharedMapContex.CurrentMapInfo.Drop(Owner);
                }

                Hop.ToChunk.Adopt(Owner);
                Owner.SharedMapContex.CurrentMapInfo = Hop.ToChunk;
            }

            Owner.SetPosition(Landing);

            if (Owner.PlannedHopIndex < Owner.ListPlannedHop.Count)
            {
                Owner.PlannedHopIndex++;
            }
        }

        protected override void OnFinished()
        {
            Land(Owner, Hop);
        }

        protected override AutomatedAction Copy()
        {
            return new CrossPortalAction(Owner, Hop, Seconds);
        }
    }

    public class BorderCrossAction : LifeSimStep
    {
        private readonly ChunkHop Hop;

        public BorderCrossAction(PlayerCharacter Owner, ChunkHop Hop)
            : base("Cross Border", Owner, 0)
        {
            this.Hop = Hop;
        }

        protected override void OnFinished()
        {
            CrossPortalAction.Land(Owner, Hop);
        }

        protected override AutomatedAction Copy()
        {
            return new BorderCrossAction(Owner, Hop);
        }
    }

    public class DriveLegAction : LifeSimStep
    {
        private readonly Vector3 Goal;

        public DriveLegAction(PlayerCharacter Owner, Vector3 Goal)
            : base("Drive", Owner, 0.1)
        {
            this.Goal = Goal;
        }

        public override bool Update(GameTime gameTime)
        {
            Vector3 Offset = Goal - Owner.Position;
            Offset.Z = 0f;
            float Distance = Offset.Length();
            float Step = Owner.BaseMoveSpeed * 3f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Distance <= Step || Distance < NavMeshLimits.AgentRadius)
            {
                Owner.StepBody(new Vector3(Offset.X, Offset.Y, Goal.Z - Owner.Position.Z));
                OnFinished();
                return false;
            }

            Owner.StepBody(Offset / Distance * Step);
            Owner.PlayClip("Walking");
            return true;
        }

        protected override AutomatedAction Copy()
        {
            return new DriveLegAction(Owner, Goal);
        }
    }

    public class WaitAction : LifeSimStep
    {
        public WaitAction(PlayerCharacter Owner, double Seconds)
            : base("Wait", Owner, Seconds)
        {
        }

        public override Pose PoseNeeded => Owner.CurrentPose;

        public override Pose PoseLeft => Owner.CurrentPose;

        protected override AutomatedAction Copy()
        {
            return new WaitAction(Owner, 1);
        }
    }
}
