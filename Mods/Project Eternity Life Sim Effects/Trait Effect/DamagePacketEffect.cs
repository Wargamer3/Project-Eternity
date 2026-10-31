using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class DamagePacketEffect : TraitEffect
    {
        private int _Count = 1;
        private int _Faces = 4;
        private int _Flat;
        private int _PerRank;
        private int _EveryRanks = 1;
        private int _BaseRank = 1;
        private string _Type = "Fire";
        private string _When = "Hit";
        private bool _Basic;
        private bool _Nonlethal;

        public DamagePacketEffect()
            : base("Damage packet")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Trigger == "List" || Roll.Trigger == "Targeted" || Roll.Trigger == "Ranged" || Roll.Trigger == "AllyMoved")
            {
                return;
            }

            int Extra = 0;
            int Step = EveryRanks < 1 ? 1 : EveryRanks;
            if (PerRank > 0 && Roll.SpellRank > BaseRank)
            {
                Extra = PerRank * ((Roll.SpellRank - BaseRank) / Step);
            }

            DamagePacket Packet = new DamagePacket();
            Packet.Count = Count + Extra;
            Packet.Faces = Faces;
            Packet.Flat = Flat;
            Packet.Type = Type;
            Packet.When = When;
            Packet.Basic = Basic;
            Packet.Nonlethal = Nonlethal;
            Roll.Packets.Add(Packet);
            if (Basic)
            {
                Roll.Basic = true;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Count);
            BW.Write(Faces);
            BW.Write(Flat);
            BW.Write(PerRank);
            BW.Write(EveryRanks);
            BW.Write(BaseRank);
            BW.Write(Type);
            BW.Write(When);
            BW.Write(Basic);
            BW.Write(Nonlethal);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            DamagePacketEffect Copy = new DamagePacketEffect();
            Copy.Count = BR.ReadInt32();
            Copy.Faces = BR.ReadInt32();
            Copy.Flat = BR.ReadInt32();
            Copy.PerRank = BR.ReadInt32();
            Copy.EveryRanks = BR.ReadInt32();
            Copy.BaseRank = BR.ReadInt32();
            Copy.Type = BR.ReadString();
            Copy.When = BR.ReadString();
            Copy.Basic = BR.ReadBoolean();
            Copy.Nonlethal = BR.ReadBoolean();
            return Copy;
        }

        [CategoryAttribute("Damage"), DescriptionAttribute("Dice added before the heightened rank."), DefaultValueAttribute(1)]
        public int Count { get { return _Count; } set { _Count = value; } }

        [CategoryAttribute("Damage"), DescriptionAttribute("Faces on each die."), DefaultValueAttribute(4)]
        public int Faces { get { return _Faces; } set { _Faces = value; } }

        [CategoryAttribute("Damage"), DescriptionAttribute("Flat damage added to the dice."), DefaultValueAttribute(0)]
        public int Flat { get { return _Flat; } set { _Flat = value; } }

        [CategoryAttribute("Damage"), DescriptionAttribute("Extra dice for each heightened step."), DefaultValueAttribute(0)]
        public int PerRank { get { return _PerRank; } set { _PerRank = value; } }

        [CategoryAttribute("Damage"), DescriptionAttribute("Ranks between heightened steps. 1 means every rank."), DefaultValueAttribute(1)]
        public int EveryRanks { get { return _EveryRanks; } set { _EveryRanks = value; } }

        [CategoryAttribute("Damage"), DescriptionAttribute("Rank where the base dice apply."), DefaultValueAttribute(1)]
        public int BaseRank { get { return _BaseRank; } set { _BaseRank = value; } }

        [CategoryAttribute("Damage"), DescriptionAttribute("Damage type for this packet."), DefaultValueAttribute("Fire")]
        public string Type { get { return _Type; } set { _Type = value; } }

        [CategoryAttribute("Damage"), DescriptionAttribute("Hit, Failure, Critical, Persistent, Break, or Sustain."), DefaultValueAttribute("Hit")]
        public string When { get { return _When; } set { _When = value; } }

        [CategoryAttribute("Damage"), DescriptionAttribute("Success on a basic save deals half."), DefaultValueAttribute(false)]
        public bool Basic { get { return _Basic; } set { _Basic = value; } }

        [CategoryAttribute("Damage"), DescriptionAttribute("The packet is nonlethal."), DefaultValueAttribute(false)]
        public bool Nonlethal { get { return _Nonlethal; } set { _Nonlethal = value; } }
    }
}
