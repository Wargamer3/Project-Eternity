using System;
using System.IO;

namespace ProjectEternity.Core.Attacks
{
    public class AttackContext
    {
        public enum ContextTypes { Any, Air, Land, Sea, Space, Custom };

        public string ContextName;

        public ContextTypes AttackOriginType;
        public string AttackOriginCustomType;
        public ContextTypes AttackTargetType;
        public string AttackTargetCustomType;

        public readonly AttackAnimations Animations;

        public AttackContext()
        {
            ContextName = "Any";

            AttackOriginType = ContextTypes.Any;
            AttackOriginCustomType = string.Empty;
            AttackTargetType = ContextTypes.Any;
            AttackTargetCustomType = string.Empty;
            Animations = new AttackAnimations();
        }

        public AttackContext(BinaryReader BR)
        {
            ContextName = "Any";

            AttackOriginType = ContextTypes.Any;
            AttackOriginCustomType = string.Empty;
            AttackTargetType = ContextTypes.Any;
            AttackTargetCustomType = string.Empty;
            Animations = new AttackAnimations();
            Animations.Load(BR);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Animations.Count);
            for (int A = 0; A < Animations.Count; ++A)
            {
                Animations[A].Save(BW);
            }
        }
    }
}
