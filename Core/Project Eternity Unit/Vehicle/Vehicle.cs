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
        public float Yaw;
        public float Pitch;
        public float Roll;
        public Matrix World;

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

        public void DrawVehicle(GraphicsDevice g, Matrix View, Matrix Projection)
        {
            Draw3D(g);

            foreach (VehicleSeat ActiveSeat in ListSeat)
            {
                if (ActiveSeat.User != null)
                {
                    Vector3 UserPositon = new Vector3(Position.X - sprVehicle.Width / 2 + ActiveSeat.SeatOffset.X,
                        Position.Y, Position.Z - sprVehicle.Height / 2 + ActiveSeat.SeatOffset.Y);
                    var a = Matrix.CreateTranslation(
                        new Vector3(
                            -Position.X,
                            -Position.Y,
                            -Position.Z))
                        * Matrix.CreateRotationY(Yaw) * Matrix.CreateTranslation(Position);
                    Vector3 UserPos2 = Vector3.Transform(UserPositon, a);

                    ActiveSeat.User.SetPosition(new Vector3(UserPos2.X, UserPos2.Y + 8, UserPos2.Z));

                    ActiveSeat.User.Draw3DOnMap(g, View, Projection);
                }
            }
        }
    }
}
