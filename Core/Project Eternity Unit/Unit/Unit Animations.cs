using System.IO;

namespace ProjectEternity.Core.Units
{
    public class UnitAnimations
    {
        public AnimationInfo[] ArrayAnimationPath;

        public AnimationInfo Default { get { return ArrayAnimationPath[0]; } }

        public AnimationInfo Hit { get { return ArrayAnimationPath[1]; } }

        public AnimationInfo HitToDefault { get { return ArrayAnimationPath[2]; } }

        public AnimationInfo MoveFoward { get { return ArrayAnimationPath[3]; } }

        public AnimationInfo MoveFowardToDefault { get { return ArrayAnimationPath[4]; } }

        public AnimationInfo MoveBackward { get { return ArrayAnimationPath[5]; } }

        public AnimationInfo MoveBackwardToDefault { get { return ArrayAnimationPath[6]; } }

        public AnimationInfo Destroyed { get { return ArrayAnimationPath[7]; } }

        public AnimationInfo Shield { get { return ArrayAnimationPath[8]; } }

        public AnimationInfo ShieldToDefault { get { return ArrayAnimationPath[9]; } }

        public AnimationInfo ParryStart { get { return ArrayAnimationPath[10]; } }

        public AnimationInfo ParryEnd { get { return ArrayAnimationPath[11]; } }

        public AnimationInfo ShootDownFiring { get { return ArrayAnimationPath[12]; } }

        public AnimationInfo ShootDownShot { get { return ArrayAnimationPath[13]; } }

        public UnitAnimations()
        {
            ArrayAnimationPath = new AnimationInfo[14];
        }

        public UnitAnimations(BinaryReader BR)
        {
            int AnimtionCount = BR.ReadInt32();
            ArrayAnimationPath = new AnimationInfo[AnimtionCount];

            for (int A = 0; A < AnimtionCount; ++A)
                ArrayAnimationPath[A] = new AnimationInfo(BR.ReadString());
        }

        public string this[int i]
        {
            set
            {
                ArrayAnimationPath[i] = new AnimationInfo(value);
            }
        }
    }
}
