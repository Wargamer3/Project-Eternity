using System.IO;

namespace ProjectEternity.Core.Units
{
    public class UnitMovementType
    {
        public string Name;

        public UnitMovementType()
        {
            Name = "New Movement Type";
        }

        public UnitMovementType(BinaryReader BR)
        {
            Name = BR.ReadString();

        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
