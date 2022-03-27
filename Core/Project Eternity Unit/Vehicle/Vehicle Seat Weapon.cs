using System;
using System.IO;

namespace ProjectEternity.Core.Vehicle
{
    public class VehicleWeapon
    {
        public string Name;
        public float MinAngleLateral;
        public float MaxAngleLateral;
        public float MinAngleUpward;
        public float MaxAngleUpward;

        public VehicleWeapon()
        {
            Name = "New Weapon";
            MinAngleLateral = 0;
            MaxAngleLateral = 0;
            MinAngleUpward = 0;
            MaxAngleUpward = 0;
        }

        public VehicleWeapon(BinaryReader BR)
        {
            Name = BR.ReadString();
            MinAngleLateral = BR.ReadSingle();
            MaxAngleLateral = BR.ReadSingle();
            MinAngleUpward = BR.ReadSingle();
            MaxAngleUpward = BR.ReadSingle();
        }

        internal void Save(BinaryWriter BW)
        {
            BW.Write(Name);
            BW.Write(MinAngleLateral);
            BW.Write(MaxAngleLateral);
            BW.Write(MinAngleUpward);
            BW.Write(MaxAngleUpward);
        }
    }
}
