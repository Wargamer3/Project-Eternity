using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ImagesEffect : TraitEffect
    {
        private int _Count = 3;

        public ImagesEffect()
            : base("Images")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger != "List")
            {
                Roll.GrantImages = Count;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Count);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            ImagesEffect Copy = new ImagesEffect();
            Copy.Count = BR.ReadInt32();
            return Copy;
        }

        [CategoryAttribute("Defense"), DefaultValueAttribute(3)]
        public int Count { get { return _Count; } set { _Count = value; } }
    }
}
