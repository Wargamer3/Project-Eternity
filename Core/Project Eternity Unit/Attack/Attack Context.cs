using System;
using System.IO;

namespace ProjectEternity.Core.Attacks
{
    public class AttackContext
    {
        public enum ContextTypes { Any, Air, Land, Sea, Space, Custom };

        public string ContextName;
        public string ParserValue;
        public int Weight;

        public ContextTypes AttackOriginType;
        public string AttackOriginCustomType;
        public ContextTypes AttackTargetType;
        public string AttackTargetCustomType;

        public readonly AttackAnimations Animations;

        public AttackContext()
        {
            ContextName = "Any";
            ParserValue = string.Empty;
            Weight = 0;

            AttackOriginType = ContextTypes.Any;
            AttackOriginCustomType = string.Empty;
            AttackTargetType = ContextTypes.Any;
            AttackTargetCustomType = string.Empty;
            Animations = new AttackAnimations();
        }

        public AttackContext(BinaryReader BR)
        {
            ContextName = BR.ReadString();
            ParserValue = BR.ReadString();
            Weight = BR.ReadInt32();

            AttackOriginType = (ContextTypes)BR.ReadByte();
            AttackOriginCustomType = BR.ReadString();
            AttackTargetType = (ContextTypes)BR.ReadByte();
            AttackTargetCustomType = BR.ReadString();

            Animations = new AttackAnimations();
            Animations.Load(BR);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ContextName);
            BW.Write(ParserValue);
            BW.Write(Weight);
            BW.Write((byte)AttackOriginType);
            BW.Write(AttackOriginCustomType);
            BW.Write((byte)AttackTargetType);
            BW.Write(AttackTargetCustomType);

            BW.Write(Animations.Count);
            for (int A = 0; A < Animations.Count; ++A)
            {
                Animations[A].Save(BW);
            }
        }
    }
}
