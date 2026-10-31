using System;
using System.Collections.Generic;
using System.Numerics;
using SharpGLTF.Runtime;
using Xna = Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public readonly struct GroundContact
    {
        public readonly float Height;
        public readonly float SlopeX;
        public readonly float SlopeY;

        public GroundContact(float Height, float SlopeX, float SlopeY)
        {
            this.Height = Height;
            this.SlopeX = SlopeX;
            this.SlopeY = SlopeY;
        }
    }

    public delegate GroundContact GroundSample(float MapX, float MapY);

    // After the clip, each Mixamo foot is moved onto the ground under it.
    // A walking step that has lifted stays lifted. An idle keeps both feet down.
    public static class FootPlant
    {
        const float PlantFraction = 0.22f;

        // Local axis of a Mixamo foot bone. In the bind pose it points down through the sole, and that sole lies on the ground.
        static readonly Vector3 SoleDown = new Vector3(-0.44495382f, 0.5214529f, -0.7280817f);

        // Distance from the foot bone to the sole mesh along SoleDown, in the armature's space.
        const float SoleClearance = 2.2818f;

        public static void Apply(PlayerCharacter Who, Xna.Vector3 ChunkShift)
        {
            Apply(Who, ChunkShift, null);
        }

        public static void Apply(PlayerCharacter Who, Xna.Vector3 ChunkShift, GroundSample Sample)
        {
            if (Who == null || Who.BodyModel == null || Who.CurrentPose == Pose.MidAir)
            {
                return;
            }

            ArmatureInstance Armature = Who.BodyModel.Armature;
            if (Armature == null)
            {
                return;
            }

            IReadOnlyList<NodeInstance> Nodes = Armature.LogicalNodes;
            Leg Left;
            Leg Right;
            if (!FindLeg(Nodes, "Left", out Left) || !FindLeg(Nodes, "Right", out Right))
            {
                return;
            }

            Xna.Matrix Drawn = Who.BodyDrawWorld(ChunkShift);
            Matrix4x4 World = ToNumerics(Drawn);
            if (!Matrix4x4.Invert(World, out Matrix4x4 Inverse))
            {
                return;
            }

            GroundSample Ground = Sample ?? Who.TownGround;
            bool Stay = Standing(Who);
            bool[] Posed = new bool[Nodes.Count];
            Place(Armature, Nodes, Left, World, Inverse, ChunkShift, Ground, Stay, Posed);
            Place(Armature, Nodes, Right, World, Inverse, ChunkShift, Ground, Stay, Posed);
            SettleChildren(Armature, Nodes, Posed);
        }

        // Animated and Ground are the sole and the floor in the same space. Origin is the floor the clip was built on.
        // Reach is how high a step lifts the sole. Weight fades from a planted foot to a foot in the air.
        public static float FootHeight(float Animated, float Ground, float Origin, float Reach)
        {
            float Lift = Animated - Origin;
            float Weight = 1f;
            if (Reach > 0.0001f)
            {
                float Along = Lift / Reach;
                if (Along < 0f)
                {
                    Along = 0f;
                }
                else if (Along > 1f)
                {
                    Along = 1f;
                }

                Weight = 1f - Along;
            }

            float Raised = Animated < Ground ? Ground : Animated;
            return Raised + (Ground - Raised) * Weight;
        }

        public static float PlantWeight(float Animated, float Origin, float Reach)
        {
            float Lift = Animated - Origin;
            if (Reach <= 0.0001f)
            {
                return 1f;
            }

            float Along = Lift / Reach;
            if (Along < 0f)
            {
                Along = 0f;
            }
            else if (Along > 1f)
            {
                Along = 1f;
            }

            return 1f - Along;
        }

        public static Vector3 SlopeNormal(float SlopeX, float SlopeY)
        {
            Vector3 Normal = new Vector3(-SlopeX, 1f, -SlopeY);
            if (Normal.LengthSquared() < 0.0000001f)
            {
                return Vector3.UnitY;
            }

            return Vector3.Normalize(Normal);
        }

        public static bool Solve(Vector3 Hip, Vector3 Knee, Vector3 Ankle, Vector3 Target, out Vector3 NewKnee, out Vector3 NewAnkle)
        {
            NewKnee = Knee;
            NewAnkle = Ankle;
            float Upper = Vector3.Distance(Hip, Knee);
            float Lower = Vector3.Distance(Knee, Ankle);
            if (Upper < 0.0001f || Lower < 0.0001f)
            {
                return false;
            }

            Vector3 To = Target - Hip;
            float Dist = To.Length();
            if (Dist < 0.0001f)
            {
                To = Knee - Hip;
                Dist = To.Length();
                if (Dist < 0.0001f)
                {
                    return false;
                }
            }

            float Max = Upper + Lower - 0.0001f;
            float Min = MathF.Abs(Upper - Lower) + 0.0001f;
            if (Dist > Max)
            {
                Dist = Max;
            }
            else if (Dist < Min)
            {
                Dist = Min;
            }

            Vector3 Direction = Vector3.Normalize(To);
            NewAnkle = Hip + Direction * Dist;

            Vector3 Pole = Knee - Hip;
            Vector3 Side = Vector3.Cross(Direction, Pole);
            if (Side.LengthSquared() < 0.0000001f)
            {
                Side = Vector3.Cross(Direction, Vector3.UnitX);
                if (Side.LengthSquared() < 0.0000001f)
                {
                    Side = Vector3.Cross(Direction, Vector3.UnitZ);
                }
            }

            Vector3 Bend = Vector3.Normalize(Vector3.Cross(Side, Direction));
            if (Vector3.Dot(Bend, Pole) < 0f)
            {
                Bend = -Bend;
            }

            float Cos = (Upper * Upper + Dist * Dist - Lower * Lower) / (2f * Upper * Dist);
            if (Cos < -1f)
            {
                Cos = -1f;
            }
            else if (Cos > 1f)
            {
                Cos = 1f;
            }

            float Sin = MathF.Sqrt(MathF.Max(0f, 1f - Cos * Cos));
            NewKnee = Hip + Direction * (Upper * Cos) + Bend * (Upper * Sin);
            return true;
        }

        static bool Standing(PlayerCharacter Who)
        {
            string Name = Who.BodyModel.ActiveAnimationName;
            if (string.IsNullOrEmpty(Name))
            {
                return false;
            }

            return Name.IndexOf("Idle", StringComparison.OrdinalIgnoreCase) >= 0
                && Name.IndexOf("Walk", StringComparison.OrdinalIgnoreCase) < 0;
        }

        static void Place(ArmatureInstance Armature, IReadOnlyList<NodeInstance> Nodes, Leg Chain, Matrix4x4 World, Matrix4x4 Inverse, Xna.Vector3 ChunkShift, GroundSample Ground, bool Stay, bool[] Posed)
        {
            Matrix4x4 Thigh = Nodes[Chain.Thigh].ModelMatrix;
            Matrix4x4 Shin = Nodes[Chain.Knee].ModelMatrix;
            Matrix4x4 Foot = Nodes[Chain.Foot].ModelMatrix;
            Vector3 Hip = Thigh.Translation;
            Vector3 Knee = Shin.Translation;
            Vector3 Ankle = Foot.Translation;
            float Upper = Vector3.Distance(Hip, Knee);
            float Lower = Vector3.Distance(Knee, Ankle);
            if (Upper < 0.0001f || Lower < 0.0001f)
            {
                return;
            }

            Vector3 AnkleRender = Vector3.Transform(Ankle, World);
            float MapX = AnkleRender.X - ChunkShift.X;
            float MapY = AnkleRender.Z - ChunkShift.Z;
            GroundContact Contact = Ground(MapX, MapY);
            float GroundRender = Contact.Height + ChunkShift.Y;
            float Scale = ScaleOf(World);
            float Reach = PlantFraction * (Upper + Lower) * Scale;
            if (!Matrix4x4.Decompose(Thigh, out Vector3 ThighScale, out Quaternion ThighRotation, out _)
                || !Matrix4x4.Decompose(Shin, out Vector3 ShinScale, out Quaternion ShinRotation, out _)
                || !Matrix4x4.Decompose(Foot, out Vector3 FootScale, out Quaternion FootRotation, out _))
            {
                return;
            }

            Vector3 ModelNormal = Vector3.TransformNormal(SlopeNormal(Contact.SlopeX, Contact.SlopeY), Inverse);
            if (ModelNormal.LengthSquared() > 0.0000001f)
            {
                ModelNormal = Vector3.Normalize(ModelNormal);
            }
            else
            {
                ModelNormal = Vector3.UnitY;
            }

            Vector3 AnimatedOffset = Vector3.TransformNormal(Vector3.Transform(SoleDown * SoleClearance, FootRotation), World);
            float AnimatedSole = AnkleRender.Y + AnimatedOffset.Y;
            float Weight = Stay ? 1f : PlantWeight(AnimatedSole, World.Translation.Y, Reach);
            float TargetSole = Stay ? GroundRender : FootHeight(AnimatedSole, GroundRender, World.Translation.Y, Reach);
            Vector3 PlantedOffset = Vector3.TransformNormal(-ModelNormal * SoleClearance, World);
            Vector3 UsedOffset = Vector3.Lerp(AnimatedOffset, PlantedOffset, Weight);
            float DropBy = TargetSole - (AnkleRender.Y + UsedOffset.Y);
            bool Drop = MathF.Abs(DropBy) >= 0.002f * Scale;
            if (!Drop && Weight <= 0.001f)
            {
                return;
            }

            Vector3 NewKnee = Knee;
            Vector3 NewAnkle = Ankle;
            Quaternion ThighSwing = Quaternion.Identity;
            Quaternion ShinSwing = Quaternion.Identity;
            if (Drop)
            {
                Vector3 TargetRender = new Vector3(AnkleRender.X, AnkleRender.Y + DropBy, AnkleRender.Z);
                Vector3 Target = Vector3.Transform(TargetRender, Inverse);
                if (!Solve(Hip, Knee, Ankle, Target, out NewKnee, out NewAnkle))
                {
                    return;
                }

                ThighSwing = FromTo(Knee - Hip, NewKnee - Hip);
                ShinSwing = FromTo(Ankle - Knee, NewAnkle - NewKnee);
            }

            Quaternion PosedFoot = ShinSwing * FootRotation;
            Vector3 Down = Vector3.Transform(SoleDown, PosedFoot);
            Quaternion Align = FromTo(Down, -ModelNormal);
            if (Weight < 0.999f)
            {
                Align = Quaternion.Slerp(Quaternion.Identity, Align, Weight);
            }

            if (Drop)
            {
                Write(Armature, Nodes[Chain.Thigh].Name, ThighScale, Quaternion.Normalize(ThighSwing * ThighRotation), Hip);
                Write(Armature, Nodes[Chain.Knee].Name, ShinScale, Quaternion.Normalize(ShinSwing * ShinRotation), NewKnee);
                Posed[Chain.Thigh] = true;
                Posed[Chain.Knee] = true;
            }

            Write(Armature, Nodes[Chain.Foot].Name, FootScale, Quaternion.Normalize(Align * PosedFoot), NewAnkle);
            Posed[Chain.Foot] = true;
        }

        static void Write(ArmatureInstance Armature, string Name, Vector3 Scale, Quaternion Rotation, Vector3 Position)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return;
            }

            Matrix4x4 Posed = Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position);
            Armature.SetModelMatrix(Name, Posed);
        }

        static void SettleChildren(ArmatureInstance Armature, IReadOnlyList<NodeInstance> Nodes, bool[] Posed)
        {
            int[] ParentOf = new int[Nodes.Count];
            var IndexOf = new Dictionary<NodeInstance, int>(Nodes.Count);
            for (int Index = 0; Index < Nodes.Count; Index++)
            {
                IndexOf[Nodes[Index]] = Index;
                ParentOf[Index] = -1;
            }

            for (int Index = 0; Index < Nodes.Count; Index++)
            {
                NodeInstance Parent = Nodes[Index].VisualParent;
                int ParentIndex;
                if (Parent != null && IndexOf.TryGetValue(Parent, out ParentIndex))
                {
                    ParentOf[Index] = ParentIndex;
                }
            }

            for (int Index = 0; Index < Nodes.Count; Index++)
            {
                int Parent = ParentOf[Index];
                if (Parent < 0 || !Posed[Parent] || Posed[Index])
                {
                    continue;
                }

                string Name = Nodes[Index].Name;
                if (!string.IsNullOrEmpty(Name))
                {
                    Armature.SetLocalMatrix(Name, Nodes[Index].LocalMatrix);
                }

                Posed[Index] = true;
            }
        }

        // World-space sole disks for the terrain contact. A foot that has lifted is left out.
        // Returns the number of disks written, starting at Count.
        public static int CollectSoles(PlayerCharacter Who, Xna.Vector3 ChunkShift, Xna.Vector4[] Dest, int Count, int Capacity)
        {
            if (Who == null || Dest == null || Count >= Capacity || Who.CurrentPose == Pose.MidAir || Who.DungeonPose)
            {
                return Count;
            }

            Xna.Matrix Drawn = Who.BodyDrawWorld(ChunkShift);
            Matrix4x4 World = ToNumerics(Drawn);
            ArmatureInstance Armature = Who.BodyModel == null ? null : Who.BodyModel.Armature;
            IReadOnlyList<NodeInstance> Nodes = Armature == null ? null : Armature.LogicalNodes;
            Leg Left = default;
            Leg Right = default;
            if (Nodes == null || !FindLeg(Nodes, "Left", out Left) || !FindLeg(Nodes, "Right", out Right))
            {
                return Count;
            }

            float Radius = 8f * ScaleOf(World);
            Xna.Vector3 Sole;
            if (Count < Capacity && TrySole(Who, Nodes, Left, World, ChunkShift, out Sole))
            {
                Dest[Count] = new Xna.Vector4(Sole.X, Sole.Y, Sole.Z, Radius);
                Count++;
            }

            if (Count < Capacity && TrySole(Who, Nodes, Right, World, ChunkShift, out Sole))
            {
                Dest[Count] = new Xna.Vector4(Sole.X, Sole.Y, Sole.Z, Radius);
                Count++;
            }

            return Count;
        }

        static bool TrySole(PlayerCharacter Who, IReadOnlyList<NodeInstance> Nodes, Leg Chain, Matrix4x4 World, Xna.Vector3 ChunkShift, out Xna.Vector3 Sole)
        {
            Sole = Xna.Vector3.Zero;
            Matrix4x4 Foot = Nodes[Chain.Foot].ModelMatrix;
            Quaternion Rotation;
            Vector3 Ankle;
            if (!Matrix4x4.Decompose(Foot, out _, out Rotation, out Ankle))
            {
                return false;
            }

            Vector3 AnkleRender = Vector3.Transform(Ankle, World);
            Vector3 Offset = Vector3.TransformNormal(Vector3.Transform(SoleDown * SoleClearance, Rotation), World);
            Vector3 At = AnkleRender + Offset;
            float MapX = At.X - ChunkShift.X;
            float MapY = At.Z - ChunkShift.Z;
            float GroundY = Who.TownGround(MapX, MapY).Height + ChunkShift.Y;
            if (At.Y - GroundY > 3.5f)
            {
                return false;
            }

            Sole = new Xna.Vector3(At.X, GroundY, At.Z);
            return true;
        }

        static bool FindLeg(IReadOnlyList<NodeInstance> Nodes, string Side, out Leg Chain)
        {
            Chain = new Leg();
            Chain.Thigh = Find(Nodes, Side + "UpLeg");
            Chain.Knee = Find(Nodes, Side + "Leg");
            Chain.Foot = Find(Nodes, Side + "Foot");
            Chain.Toe = Find(Nodes, Side + "ToeBase");
            return Chain.Thigh >= 0 && Chain.Knee >= 0 && Chain.Foot >= 0;
        }

        static int Find(IReadOnlyList<NodeInstance> Nodes, string Bone)
        {
            for (int Index = 0; Index < Nodes.Count; Index++)
            {
                string Name = Nodes[Index].Name;
                if (Name == Bone || (Name != null && Name.EndsWith(":" + Bone, StringComparison.Ordinal)))
                {
                    return Index;
                }
            }

            return -1;
        }

        static float ScaleOf(Matrix4x4 Matrix)
        {
            float Length = new Vector3(Matrix.M11, Matrix.M12, Matrix.M13).Length();
            return Length > 0.0001f ? Length : 1f;
        }

        static Matrix4x4 ToNumerics(Xna.Matrix World)
        {
            return new Matrix4x4(
                World.M11, World.M12, World.M13, World.M14,
                World.M21, World.M22, World.M23, World.M24,
                World.M31, World.M32, World.M33, World.M34,
                World.M41, World.M42, World.M43, World.M44);
        }

        static Quaternion FromTo(Vector3 From, Vector3 To)
        {
            if (From.LengthSquared() <= 0.0000001f || To.LengthSquared() <= 0.0000001f)
            {
                return Quaternion.Identity;
            }

            Vector3 A = Vector3.Normalize(From);
            Vector3 B = Vector3.Normalize(To);
            float Dot = Vector3.Dot(A, B);
            if (Dot > 0.9999f)
            {
                return Quaternion.Identity;
            }

            if (Dot < -0.9999f)
            {
                Vector3 Axis = Vector3.Cross(Vector3.UnitX, A);
                if (Axis.LengthSquared() < 0.000001f)
                {
                    Axis = Vector3.Cross(Vector3.UnitY, A);
                }

                return Quaternion.CreateFromAxisAngle(Vector3.Normalize(Axis), MathF.PI);
            }

            Vector3 Cross = Vector3.Cross(A, B);
            return Quaternion.Normalize(new Quaternion(Cross, 1f + Dot));
        }

        struct Leg
        {
            public int Thigh;
            public int Knee;
            public int Foot;
            public int Toe;
        }
    }
}
