using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.LightningSystem
{
    public class LightingSectionFork : LightingSection
    {
        public LightingSectionFork(LightningBolt Owner, int Level)
            : base(Owner, Level)
        {
        }

        private Vector3 GetForkDelta(Vector3 ForwardVector, Vector3 LeftVector)
        {
            Vector2 ForkDelta = Decay(Owner.ForkArmLength, Descriptor.ForkDecayRate) *
                                new Vector2(Random(Descriptor.ForkForwardDeviation), Random(Descriptor.ForkLeftDeviation));

            return ForkDelta.X * ForwardVector + ForkDelta.Y * LeftVector;
        }

        public override void CreateSection(Vector3 StartPosition, Vector3 Destination, int WidthLevel)
        {
            Vector3 ForwardVector = Vector3.Normalize(Destination - StartPosition);
            Vector3 LeftVector = Vector3.Normalize(Vector3.Transform(ForwardVector, Matrix.CreateRotationZ(MathHelper.PiOver2)));

            Vector3 Jittered = GetJittered(StartPosition, Destination, ForwardVector, LeftVector);
            Vector3 Forked = Jittered + GetForkDelta(ForwardVector, LeftVector);

            Owner.SetPointPosition(Jittered, WidthLevel);
            Owner.SetPointPosition(Forked, WidthLevel + 1);

            if (NextSection == null)
            {
                Owner.SetLinePosition(StartPosition, Jittered, LeftVector, WidthLevel);
                Owner.SetLinePosition(Jittered, Forked, LeftVector, WidthLevel + 1);
                Owner.SetLinePosition(Jittered, Destination, LeftVector, WidthLevel);
            }
            else
            {
                NextSection.CreateSection(StartPosition, Jittered, WidthLevel);
                NextSection.CreateSection(Jittered, Forked, WidthLevel + 1);
                NextSection.CreateSection(Jittered, Destination, WidthLevel);
            }
        }
    }
}
