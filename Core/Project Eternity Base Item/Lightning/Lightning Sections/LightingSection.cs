using System;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.LightningSystem
{
    public abstract class LightingSection
    {
        public LightingSection NextSection;
        public int Level;
        protected static Random rand = new Random();
        protected LightningDescriptor Descriptor => Owner.Descriptor;
        protected LightningBolt Owner;

        public LightingSection(LightningBolt Owner, int Level)
        {
            this.Owner = Owner;
            this.Level = Level;

            NextSection = null;
        }

        protected float Decay(float Amount, float DecayRate)
        {
            return Amount * (float)Math.Pow(DecayRate, Level);
        }

        protected float Random(Range range)
        {
            float nr = (float)rand.NextDouble();
            return MathHelper.Lerp(range.Min, range.Max, nr);
        }

        protected Vector3 GetJittered(Vector3 StartPosition, Vector3 Destination, Vector3 ForwardVector, Vector3 LeftVector)
        {
            Vector2 delta = Decay(Descriptor.JitterDeviationRadius, Descriptor.JitterDecayRate) *
                                new Vector2(Random(Descriptor.JitterForwardDeviation), Random(Descriptor.JitterLeftDeviation));

            return Vector3.Lerp(StartPosition, Destination, Random(Descriptor.SubdivisionFraction)) + delta.X * ForwardVector + delta.Y * LeftVector;
        }

        public abstract void CreateSection(Vector3 StartPosition, Vector3 Destination, int WidthLevel);
    }
}
