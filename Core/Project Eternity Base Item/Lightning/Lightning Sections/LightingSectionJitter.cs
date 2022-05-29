using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.LightningSystem
{
    public class LightingSectionJitter : LightingSection
    {
        public LightingSectionJitter(LightningBolt Owner, int Level)
            : base(Owner, Level)
        {
        }

        public override void CreateSection(Vector3 StartPosition, Vector3 Destination, int WidthLevel)
        {
            Vector3 ForwardVector = Vector3.Normalize(Destination - StartPosition);
            Vector3 LeftVector = Vector3.Normalize(Vector3.Transform(ForwardVector, Matrix.CreateRotationZ(MathHelper.PiOver2)));

            Vector3 Jittered = GetJittered(StartPosition, Destination, ForwardVector, LeftVector);

            Owner.SetPointPosition(Jittered, WidthLevel);

            if (NextSection == null)
            {
                Owner.SetLinePosition(StartPosition, Jittered, LeftVector, WidthLevel);
                Owner.SetLinePosition(Jittered, Destination, LeftVector, WidthLevel);
            }
            else
            {
                NextSection.CreateSection(StartPosition, Jittered, WidthLevel);
                NextSection.CreateSection(Jittered, Destination, WidthLevel);
            }
        }
    }
}
