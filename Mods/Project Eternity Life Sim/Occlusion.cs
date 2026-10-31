using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    // Foot and trunk disks for the terrain shader. The screen depth pass is gone.
    public static class Occlusion
    {
        public const int Capacity = 24;

        public static readonly Vector4[] Contacts = new Vector4[Capacity];
        public static int Count;

        const int FootSlots = 8;
        const int TrunkSlots = 16;

        static readonly Vector4[] FootHold = new Vector4[32];
        static readonly float[] FootRank = new float[32];
        static readonly bool[] FootUsed = new bool[32];
        static int FootHoldCount;

        static readonly Vector4[] TrunkHold = new Vector4[TrunkSlots];
        static readonly float[] TrunkRank = new float[TrunkSlots];
        static int TrunkHoldCount;

        static readonly Vector4[] Pair = new Vector4[2];

        public static void Gather(NavMapGameManager World, Vector3 Eye)
        {
            SceneOcclusion.On = false;
            SceneOcclusion.Map = null;
            Count = 0;
            FootHoldCount = 0;
            TrunkHoldCount = 0;
            for (int i = 0; i < Contacts.Length; i++)
            {
                Contacts[i] = Vector4.Zero;
            }

            if (World == null || World.UpdatedMaps == null)
            {
                return;
            }

            IList<MapInfo> Maps = World.UpdatedMaps;
            for (int m = 0; m < Maps.Count; m++)
            {
                MapInfo Info = Maps[m];
                if (Info == null || Info.MapMapContainer == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Info);
                if (Info.People != null)
                {
                    for (int i = 0; i < Info.People.Count; i++)
                    {
                        ConsiderPerson(Info.People[i], Info, Shift, Eye);
                    }
                }

                if (Info.Scatter == null)
                {
                    continue;
                }

                for (int i = 0; i < Info.Scatter.Count; i++)
                {
                    ScatterSpot Spot = Info.Scatter[i];
                    float Reach = ForestScatter.TrunkReach(Spot);
                    if (Reach <= 0.01f)
                    {
                        continue;
                    }

                    Vector3 At = new Vector3(Spot.Position.X + Shift.X, Spot.Position.Z + Shift.Y, Spot.Position.Y + Shift.Z);
                    Vector3 Flat = At - Eye;
                    Flat.Y = 0f;
                    KeepTrunk(new Vector4(At, Reach), Flat.LengthSquared());
                }
            }

            int Feet = FootHoldCount < FootSlots ? FootHoldCount : FootSlots;
            for (int n = 0; n < Feet; n++)
            {
                int Best = -1;
                for (int i = 0; i < FootHoldCount; i++)
                {
                    if (FootUsed[i])
                    {
                        continue;
                    }

                    if (Best < 0 || FootRank[i] < FootRank[Best])
                    {
                        Best = i;
                    }
                }

                if (Best < 0 || Count >= Capacity)
                {
                    break;
                }

                FootUsed[Best] = true;
                Contacts[Count] = FootHold[Best];
                Count++;
            }

            int Trunks = TrunkHoldCount;
            if (Count + Trunks > Capacity)
            {
                Trunks = Capacity - Count;
            }

            for (int i = 0; i < Trunks; i++)
            {
                Contacts[Count] = TrunkHold[i];
                Count++;
            }
        }

        static void ConsiderPerson(PlayerCharacter Who, MapInfo Info, Vector3 Shift, Vector3 Eye)
        {
            if (Who == null || Who.DungeonPose || Who.BodyModel == null)
            {
                return;
            }

            if (Who.SharedMapContex == null || Who.SharedMapContex.CurrentMapInfo != Info)
            {
                return;
            }

            if (Who.Life != null && Who.Life.Away != null && Who.Life.Away.Busy)
            {
                return;
            }

            int Written = FootPlant.CollectSoles(Who, Shift, Pair, 0, Pair.Length);
            for (int i = 0; i < Written; i++)
            {
                Vector3 Flat = new Vector3(Pair[i].X, 0f, Pair[i].Z) - new Vector3(Eye.X, 0f, Eye.Z);
                KeepFoot(Pair[i], Flat.LengthSquared());
            }
        }

        static void KeepFoot(Vector4 At, float Rank)
        {
            if (FootHoldCount < FootHold.Length)
            {
                FootUsed[FootHoldCount] = false;
                FootHold[FootHoldCount] = At;
                FootRank[FootHoldCount] = Rank;
                FootHoldCount++;
                return;
            }

            int Worst = 0;
            for (int i = 1; i < FootHold.Length; i++)
            {
                if (FootRank[i] > FootRank[Worst])
                {
                    Worst = i;
                }
            }

            if (Rank < FootRank[Worst])
            {
                FootUsed[Worst] = false;
                FootHold[Worst] = At;
                FootRank[Worst] = Rank;
            }
        }

        static void KeepTrunk(Vector4 At, float Rank)
        {
            if (TrunkHoldCount < TrunkHold.Length)
            {
                TrunkHold[TrunkHoldCount] = At;
                TrunkRank[TrunkHoldCount] = Rank;
                TrunkHoldCount++;
                return;
            }

            int Worst = 0;
            for (int i = 1; i < TrunkHold.Length; i++)
            {
                if (TrunkRank[i] > TrunkRank[Worst])
                {
                    Worst = i;
                }
            }

            if (Rank < TrunkRank[Worst])
            {
                TrunkHold[Worst] = At;
                TrunkRank[Worst] = Rank;
            }
        }
    }
}
