using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Vehicle
{
    public class Vehicle
    {
        public Texture2D sprVehicle;

        public short[] ArrayIndex;
        public VertexPositionNormalTexture[] ArrayVertex;

        public int TriangleCount;

        public string SpritePath;
        public string Name;
        public string VehiclePath;
        public float MaxMV;
        public float Acceleration;
        public float MaxSpeed;
        public float TurnSpeed;
        public float MaxClimbAngle;
        public string ControlType;
        public int MaxHP;

        public Vector3 Position;

        public List<VehicleSeat> ListSeat;

        public Vehicle()
        {
            Name = "New Seat";
            ListSeat = new List<VehicleSeat>();
        }

        public Vehicle(string VehiclePath, ContentManager Content)
        {
            this.VehiclePath = VehiclePath;
            this.Name = VehiclePath;

            FileStream FS = new FileStream("Content/Vehicles/" + VehiclePath + ".pev", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            SpritePath = BR.ReadString();
            sprVehicle = Content.Load<Texture2D>("Vehicles/Sprites/" + SpritePath);
            MaxMV = BR.ReadSingle();
            Acceleration = BR.ReadSingle();
            MaxSpeed = BR.ReadSingle();
            TurnSpeed = BR.ReadSingle();
            MaxClimbAngle = BR.ReadSingle();
            ControlType = BR.ReadString();
            MaxHP = BR.ReadInt32();

            int ListSeatCount = BR.ReadInt32();
            ListSeat = new List<VehicleSeat>(ListSeatCount);
            for (int S = 0; S < ListSeatCount; ++S)
            {
                ListSeat.Add(new VehicleSeat(BR));
            }

            FS.Close();
            BR.Close();
        }

        public void SetVertex(VertexPositionNormalTexture[] ArrayVertex, short[] ArrayIndex)
        {
            this.ArrayVertex = ArrayVertex;
            this.ArrayIndex = ArrayIndex;
            TriangleCount = ArrayIndex.Length / 3;
        }

        public Vehicle Copy()
        {
            Vehicle CopyVehicle = new Vehicle();
            CopyVehicle.sprVehicle = sprVehicle;
            CopyVehicle.SpritePath = SpritePath;
            CopyVehicle.Name = Name;
            CopyVehicle.VehiclePath = VehiclePath;
            CopyVehicle.MaxMV = MaxMV;
            CopyVehicle.Acceleration = Acceleration;
            CopyVehicle.MaxSpeed = MaxSpeed;
            CopyVehicle.TurnSpeed = TurnSpeed;
            CopyVehicle.MaxClimbAngle = MaxClimbAngle;
            CopyVehicle.ControlType = ControlType;
            CopyVehicle.MaxHP = MaxHP;

            CopyVehicle.ListSeat = new List<VehicleSeat>(ListSeat.Count);
            foreach (VehicleSeat ActiveSeat in ListSeat)
            {
                CopyVehicle.ListSeat.Add(ActiveSeat.Copy());
            }

            return CopyVehicle;
        }

        public void Draw3D(GraphicsDevice g)
        {
            g.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, ArrayVertex, 0, ArrayVertex.Length, ArrayIndex, 0, TriangleCount);
        }
    }
}
