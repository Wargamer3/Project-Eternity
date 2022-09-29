using System.IO;

namespace ProjectEternity.Core.Units
{
    public class UnitMovementType
    {
        public string Name;
        public string ActivationName;
        public string AnnulatioName;

        public UnitMovementType()
        {
            Name = "New Movement Type";
            ActivationName = string.Empty;
            AnnulatioName = string.Empty;
        }

        public UnitMovementType(BinaryReader BR)
        {
            Name = BR.ReadString();
            ActivationName = BR.ReadString();
            AnnulatioName = BR.ReadString();
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Name);
            BW.Write(ActivationName);
            BW.Write(AnnulatioName);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
