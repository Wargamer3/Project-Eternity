using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Vehicle
{
    public class VehicleSeat
    {
        public string Name;
        public bool CanDrive;
        public bool IsVisible;
        public bool IsProtected;
        public Vector2 SeatOffset;
        public float Height;
        public VehicleWeapon Weapon;

        public VehicleSeat()
        {
            Name = "New Seat";
            Weapon = new VehicleWeapon();
        }

        public VehicleSeat(BinaryReader BR)
        {
            Name = BR.ReadString();
            CanDrive = BR.ReadBoolean();
            IsVisible = BR.ReadBoolean();
            IsProtected = BR.ReadBoolean();

            if (IsVisible)
            {
                SeatOffset = new Vector2(BR.ReadSingle(), BR.ReadSingle());
                Height = BR.ReadSingle();
            }

            Weapon = new VehicleWeapon(BR);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Name);
            BW.Write(CanDrive);
            BW.Write(IsVisible);
            BW.Write(IsProtected);

            if (IsVisible)
            {
                BW.Write(SeatOffset.X);
                BW.Write(SeatOffset.Y);
                BW.Write(Height);
            }

            Weapon.Save(BW);
        }
    }
}
