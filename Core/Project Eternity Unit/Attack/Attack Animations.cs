using ProjectEternity.Core.Units;
using System.IO;

namespace ProjectEternity.Core.Attacks
{
    public class AttackAnimations
    {
        public AnimationInfo[] ArrayAnimationPath;

        public AnimationInfo Start { get { return ArrayAnimationPath[0]; } set { ArrayAnimationPath[0] = value; } }

        public AnimationInfo EndHit { get { return ArrayAnimationPath[1]; } set { ArrayAnimationPath[1] = value; } }

        public AnimationInfo EndMiss { get { return ArrayAnimationPath[2]; } set { ArrayAnimationPath[2] = value; } }

        public AnimationInfo EndDestroyed { get { return ArrayAnimationPath[3]; } set { ArrayAnimationPath[3] = value; } }

        public AnimationInfo EndBlocked { get { return ArrayAnimationPath[4]; } set { ArrayAnimationPath[4] = value; } }

        public AnimationInfo EndParried { get { return ArrayAnimationPath[5]; } set { ArrayAnimationPath[5] = value; } }

        public AnimationInfo EndShootDown { get { return ArrayAnimationPath[6]; } set { ArrayAnimationPath[6] = value; } }

        public AnimationInfo EndNegated { get { return ArrayAnimationPath[7]; } set { ArrayAnimationPath[7] = value; } }

        public AttackAnimations()
        {
            ArrayAnimationPath = new AnimationInfo[8];
        }

        public AnimationInfo this[int Index]
        {
            get
            {
                return ArrayAnimationPath[Index];
            }
            set
            {
                ArrayAnimationPath[Index] = value;
            }
        }

        public int Count { get { return ArrayAnimationPath.Length; } }

        public void Load(BinaryReader BR)
        {
            int AttackAnimationCount = BR.ReadInt32();
            for (int A = 0; A < AttackAnimationCount; ++A)
            {
                ArrayAnimationPath[A] = new AnimationInfo(BR.ReadString());
            }
        }
    }
}
