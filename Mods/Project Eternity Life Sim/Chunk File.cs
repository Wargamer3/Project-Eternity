using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class ChunkFile
    {
        public static void WriteFlat(WorldSlot Slot, int X, int Z, float Height)
        {
            MapInfo Info = new MapInfo(new MapContainer(X + "_" + Z, null), null, null);
            Info.ChunkX = X;
            Info.ChunkZ = Z;
            Info.Kind = GroundKind.Heightmap;
            Info.Heights = HeightField.Create(Slot.ChunkSize, Slot.Spacing, Height);
            Info.Forest = new ForestRecipe();
            Place(Slot, Info);
            Write(Slot, Info, true);
        }

        public static void Place(WorldSlot Slot, MapInfo Info)
        {
            if (Info.MapMapContainer == null)
            {
                Info.MapMapContainer = new MapContainer(Info.ChunkX + "_" + Info.ChunkZ, null);
            }

            Info.MapMapContainer.OffsetX = (long)Math.Round(Info.ChunkX * Slot.ChunkSize);
            Info.MapMapContainer.OffsetY = (long)Math.Round(Info.ChunkZ * Slot.ChunkSize);
            Info.MapMapContainer.SpanX = Slot.ChunkSize;
            Info.MapMapContainer.SpanY = Slot.ChunkSize;
            Info.MapMapContainer.AreaName = Info.ChunkX + "_" + Info.ChunkZ;
        }

        public static bool Read(WorldSlot Slot, MapInfo Info, bool Body, bool Props)
        {
            string PathName = Slot.ChunkPath(Info.ChunkX, Info.ChunkZ);
            if (!File.Exists(PathName))
            {
                return false;
            }

            using (FileStream Stream = new FileStream(PathName, FileMode.Open, FileAccess.Read))
            using (BinaryReader Reader = new BinaryReader(Stream))
            {
                byte[] Magic = Reader.ReadBytes(4);
                if (Magic.Length < 4 || Magic[0] != (byte)'L' || Magic[1] != (byte)'S' || Magic[2] != (byte)'C' || Magic[3] != (byte)'1')
                {
                    return false;
                }

                int Version = Reader.ReadInt32();
                if (Version != 1)
                {
                    return false;
                }

                Info.ChunkX = Reader.ReadInt32();
                Info.ChunkZ = Reader.ReadInt32();
                float Size = Reader.ReadSingle();
                Info.Kind = (GroundKind)Reader.ReadInt32();
                int BodyLength = Reader.ReadInt32();
                Place(Slot, Info);
                Info.MapMapContainer.SpanX = Size > 1f ? Size : Slot.ChunkSize;
                Info.MapMapContainer.SpanY = Info.MapMapContainer.SpanX;
                ReadLandmarks(Reader, Info, Props);
                Info.HeaderReady = true;
                if (!Body)
                {
                    return true;
                }

                long BodyAt = Reader.BaseStream.Position;
                ReadBody(Reader, Info, Slot);
                if (Reader.BaseStream.Position < BodyAt + BodyLength)
                {
                    Reader.BaseStream.Position = BodyAt + BodyLength;
                }

                Info.BodyReady = true;
            }

            return true;
        }

        public static void Write(WorldSlot Slot, MapInfo Info, bool Props)
        {
            Directory.CreateDirectory(Slot.ChunkFolder);
            MemoryStream Body = new MemoryStream();
            using (BinaryWriter BodyWriter = new BinaryWriter(Body, System.Text.Encoding.UTF8, true))
            {
                WriteBody(BodyWriter, Info);
            }

            byte[] BodyBytes = Body.ToArray();
            using (FileStream Stream = new FileStream(Slot.ChunkPath(Info.ChunkX, Info.ChunkZ), FileMode.Create, FileAccess.Write))
            using (BinaryWriter Writer = new BinaryWriter(Stream))
            {
                Writer.Write((byte)'L');
                Writer.Write((byte)'S');
                Writer.Write((byte)'C');
                Writer.Write((byte)'1');
                Writer.Write(1);
                Writer.Write(Info.ChunkX);
                Writer.Write(Info.ChunkZ);
                Writer.Write(Slot.ChunkSize);
                Writer.Write((int)Info.Kind);
                Writer.Write(BodyBytes.Length);
                WriteLandmarks(Writer, Info, Props);
                Writer.Write(BodyBytes);
            }

            Info.HeaderReady = true;
            Info.BodyReady = true;
        }

        static void WriteLandmarks(BinaryWriter Writer, MapInfo Info, bool Props)
        {
            List<LandmarkHeader> Landmarks = Info.Landmarks ?? new List<LandmarkHeader>();
            Writer.Write(Landmarks.Count);
            for (int i = 0; i < Landmarks.Count; i++)
            {
                LandmarkHeader Landmark = Landmarks[i];
                Writer.Write(Landmark.Name ?? string.Empty);
                Writer.Write(Landmark.Mesh ?? string.Empty);
                WriteVector(Writer, Landmark.Min);
                WriteVector(Writer, Landmark.Max);
                int Rooms = Landmark.Rooms == null ? 0 : Landmark.Rooms.Count;
                Writer.Write(Rooms);
                for (int r = 0; r < Rooms; r++)
                {
                    RoomHeader Room = Landmark.Rooms[r];
                    WriteVector(Writer, Room.Min);
                    WriteVector(Writer, Room.Max);
                    if (!Props || Room.Props == null || Room.Props.Count == 0)
                    {
                        Writer.Write(0);
                        continue;
                    }

                    MemoryStream Blob = new MemoryStream();
                    using (BinaryWriter BlobWriter = new BinaryWriter(Blob, System.Text.Encoding.UTF8, true))
                    {
                        WriteFurniture(BlobWriter, Room.Props);
                    }

                    byte[] Bytes = Blob.ToArray();
                    Writer.Write(Bytes.Length);
                    Writer.Write(Bytes);
                }
            }
        }

        static void ReadLandmarks(BinaryReader Reader, MapInfo Info, bool Props)
        {
            int Count = Reader.ReadInt32();
            Info.Landmarks = new List<LandmarkHeader>(Count);
            for (int i = 0; i < Count; i++)
            {
                LandmarkHeader Landmark = new LandmarkHeader();
                Landmark.Name = Reader.ReadString();
                Landmark.Mesh = Reader.ReadString();
                Landmark.Min = ReadVector(Reader);
                Landmark.Max = ReadVector(Reader);
                int Rooms = Reader.ReadInt32();
                for (int r = 0; r < Rooms; r++)
                {
                    RoomHeader Room = new RoomHeader();
                    Room.Min = ReadVector(Reader);
                    Room.Max = ReadVector(Reader);
                    int Length = Reader.ReadInt32();
                    if (Length < 0)
                    {
                        Length = 0;
                    }

                    if (Props && Length > 0)
                    {
                        byte[] Bytes = Reader.ReadBytes(Length);
                        using (MemoryStream Blob = new MemoryStream(Bytes))
                        using (BinaryReader BlobReader = new BinaryReader(Blob))
                        {
                            Room.Props = ReadFurniture(BlobReader);
                        }

                        Room.PropsOpen = true;
                    }
                    else if (Length > 0)
                    {
                        Room.PropOffset = Reader.BaseStream.Position;
                        Room.PropLength = Length;
                        Reader.BaseStream.Position += Length;
                    }

                    Landmark.Rooms.Add(Room);
                }

                Info.Landmarks.Add(Landmark);
            }
        }

        public static void ReadRoomProps(WorldSlot Slot, MapInfo Info, RoomHeader Room)
        {
            if (Room == null || Room.PropsOpen || Room.PropLength <= 0)
            {
                Room.PropsOpen = true;
                return;
            }

            string PathName = Slot.ChunkPath(Info.ChunkX, Info.ChunkZ);
            if (!File.Exists(PathName))
            {
                Room.PropsOpen = true;
                return;
            }

            using (FileStream Stream = new FileStream(PathName, FileMode.Open, FileAccess.Read))
            using (BinaryReader Reader = new BinaryReader(Stream))
            {
                Reader.BaseStream.Position = Room.PropOffset;
                byte[] Bytes = Reader.ReadBytes(Room.PropLength);
                using (MemoryStream Blob = new MemoryStream(Bytes))
                using (BinaryReader BlobReader = new BinaryReader(Blob))
                {
                    Room.Props = ReadFurniture(BlobReader);
                }
            }

            Room.PropsOpen = true;
        }

        static void WriteBody(BinaryWriter Writer, MapInfo Info)
        {
            if (Info.Kind == GroundKind.Grid)
            {
                Writer.Write(Info.GridPath ?? string.Empty);
                return;
            }

            if (Info.Kind == GroundKind.Mesh)
            {
                WriteMesh(Writer, Info);
                WriteLists(Writer, Info);
                return;
            }

            HeightField Field = Info.Heights;
            if (Field == null)
            {
                Writer.Write(0);
                Writer.Write(0);
                Writer.Write(0f);
            }
            else
            {
                Writer.Write(Field.SamplesX);
                Writer.Write(Field.SamplesZ);
                Writer.Write(Field.Spacing);
                for (int i = 0; i < Field.Heights.Length; i++)
                {
                    Writer.Write(Field.Heights[i]);
                }

                for (int i = 0; i < Field.Blends.Length; i++)
                {
                    Writer.Write(Field.Blends[i].X);
                    Writer.Write(Field.Blends[i].Y);
                    Writer.Write(Field.Blends[i].Z);
                    Writer.Write(Field.Blends[i].W);
                }
            }

            WriteLists(Writer, Info);
        }

        static void ReadBody(BinaryReader Reader, MapInfo Info, WorldSlot Slot)
        {
            if (Info.Kind == GroundKind.Grid)
            {
                Info.GridPath = Reader.ReadString();
                return;
            }

            if (Info.Kind == GroundKind.Mesh)
            {
                ReadMesh(Reader, Info);
                ReadLists(Reader, Info);
                return;
            }

            if (Info.Kind != GroundKind.Heightmap && Info.Kind != GroundKind.None)
            {
                return;
            }

            int SamplesX = Reader.ReadInt32();
            int SamplesZ = Reader.ReadInt32();
            float Spacing = Reader.ReadSingle();
            if (SamplesX >= 2 && SamplesZ >= 2)
            {
                HeightField Field = new HeightField();
                Field.SamplesX = SamplesX;
                Field.SamplesZ = SamplesZ;
                Field.Spacing = Spacing > 0.01f ? Spacing : Slot.Spacing;
                int Count = SamplesX * SamplesZ;
                Field.Heights = new float[Count];
                Field.Blends = new Vector4[Count];
                for (int i = 0; i < Count; i++)
                {
                    Field.Heights[i] = Reader.ReadSingle();
                }

                for (int i = 0; i < Count; i++)
                {
                    Field.Blends[i] = new Vector4(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
                }

                Field.Rebuild();
                Info.Heights = Field;
                Info.Kind = GroundKind.Heightmap;
            }

            ReadLists(Reader, Info);
        }

        static void WriteMesh(BinaryWriter Writer, MapInfo Info)
        {
            Writer.Write(Info.MeshPath ?? string.Empty);
            int Bands = Info.Bands == null ? 0 : Info.Bands.Count;
            Writer.Write(Bands);
            for (int i = 0; i < Bands; i++)
            {
                MeshBand Band = Info.Bands[i];
                Writer.Write(Band.MinY);
                Writer.Write(Band.MaxY);
                Writer.Write(Band.Model ?? string.Empty);
                Writer.Write(Band.Scale);
            }
        }

        static void ReadMesh(BinaryReader Reader, MapInfo Info)
        {
            Info.MeshPath = Reader.ReadString();
            int Bands = Reader.ReadInt32();
            Info.Bands = new List<MeshBand>(Bands);
            for (int i = 0; i < Bands; i++)
            {
                MeshBand Band = new MeshBand();
                Band.MinY = Reader.ReadSingle();
                Band.MaxY = Reader.ReadSingle();
                Band.Model = Reader.ReadString();
                Band.Scale = Reader.ReadSingle();
                Info.Bands.Add(Band);
            }

            if (Info.Bands.Count == 0 && !string.IsNullOrEmpty(Info.MeshPath))
            {
                MeshBand Whole = new MeshBand();
                Whole.Model = Info.MeshPath;
                Info.Bands.Add(Whole);
            }
        }

        static void WriteLists(BinaryWriter Writer, MapInfo Info)
        {
            WriteWater(Writer, Info.Water);
            WriteBuildings(Writer, Info.Buildings);
            WriteFurniture(Writer, LiveFurniture(Info));
            WriteRecipe(Writer, Info.Forest);
        }

        static void ReadLists(BinaryReader Reader, MapInfo Info)
        {
            if (Reader.BaseStream.Position >= Reader.BaseStream.Length)
            {
                return;
            }

            Info.Water = ReadWater(Reader);
            Info.Buildings = ReadBuildings(Reader);
            Info.Furniture = ReadFurniture(Reader);
            Info.Forest = ReadRecipe(Reader);
            if (Info.MapMapContainer != null && Info.MapMapContainer.ActiveMap != null)
            {
                Info.MapMapContainer.ActiveMap.Water = Info.Water;
                Info.MapMapContainer.ActiveMap.Buildings = Info.Buildings;
                Info.MapMapContainer.ActiveMap.Furniture = Info.Furniture;
            }
        }

        static List<FurnitureItem> LiveFurniture(MapInfo Info)
        {
            List<FurnitureItem> Live = new List<FurnitureItem>();
            if (Info.Furniture == null)
            {
                return Live;
            }

            for (int i = 0; i < Info.Furniture.Count; i++)
            {
                FurnitureItem Item = Info.Furniture[i];
                if (Item != null && !Item.RoomProp)
                {
                    Live.Add(Item);
                }
            }

            return Live;
        }

        static void WriteWater(BinaryWriter Writer, List<WaterVolume> Volumes)
        {
            int Count = Volumes == null ? 0 : Volumes.Count;
            Writer.Write(Count);
            for (int i = 0; i < Count; i++)
            {
                WaterVolume Volume = Volumes[i];
                Writer.Write((int)Volume.Kind);
                Writer.Write((int)Volume.Look);
                WriteVector(Writer, Volume.Start);
                WriteVector(Writer, Volume.End);
                Writer.Write(Volume.Radius);
                Writer.Write(Volume.Width);
                Writer.Write(Volume.Depth);
                Writer.Write(Volume.Speed);
                Writer.Write(Volume.Cell);
                int Cells = Volume.LakeCells == null ? 0 : Volume.LakeCells.Length;
                Writer.Write(Cells);
                for (int c = 0; c < Cells; c++)
                {
                    Writer.Write(Volume.LakeCells[c]);
                }
            }
        }

        static List<WaterVolume> ReadWater(BinaryReader Reader)
        {
            int Count = Reader.ReadInt32();
            List<WaterVolume> Volumes = new List<WaterVolume>(Count);
            for (int i = 0; i < Count; i++)
            {
                WaterVolume Volume = new WaterVolume();
                Volume.Kind = (WaterKind)Reader.ReadInt32();
                Volume.Look = (WaterLook)Reader.ReadInt32();
                Volume.Start = ReadVector(Reader);
                Volume.End = ReadVector(Reader);
                Volume.Radius = Reader.ReadSingle();
                Volume.Width = Reader.ReadSingle();
                Volume.Depth = Reader.ReadSingle();
                Volume.Speed = Reader.ReadSingle();
                Volume.Cell = Reader.ReadSingle();
                int Cells = Reader.ReadInt32();
                if (Cells > 0)
                {
                    Volume.LakeCells = new int[Cells];
                    for (int c = 0; c < Cells; c++)
                    {
                        Volume.LakeCells[c] = Reader.ReadInt32();
                    }
                }

                Volumes.Add(Volume);
            }

            return Volumes;
        }

        static void WriteBuildings(BinaryWriter Writer, List<BuildingItem> Buildings)
        {
            int Count = Buildings == null ? 0 : Buildings.Count;
            Writer.Write(Count);
            for (int i = 0; i < Count; i++)
            {
                BuildingItem Item = Buildings[i];
                Writer.Write((int)Item.Kind);
                Writer.Write(Item.Interaction ?? string.Empty);
                Writer.Write(Item.Model ?? string.Empty);
                WriteVector(Writer, Item.Position);
                WriteVector(Writer, Item.Swatch);
                WriteVector(Writer, Item.ModelShift);
                Writer.Write(Item.Width);
                Writer.Write(Item.Height);
                Writer.Write(Item.Length);
                Writer.Write(Item.Angle);
                Writer.Write(Item.ModelMeters);
                Writer.Write(Item.Floor);
                Writer.Write(Item.Price);
            }
        }

        static List<BuildingItem> ReadBuildings(BinaryReader Reader)
        {
            int Count = Reader.ReadInt32();
            List<BuildingItem> Buildings = new List<BuildingItem>(Count);
            for (int i = 0; i < Count; i++)
            {
                BuildingItem Item = new BuildingItem();
                Item.Kind = (BuildingKind)Reader.ReadInt32();
                Item.Interaction = Reader.ReadString();
                Item.Model = Reader.ReadString();
                Item.Position = ReadVector(Reader);
                Item.Swatch = ReadVector(Reader);
                Item.ModelShift = ReadVector(Reader);
                Item.Width = Reader.ReadSingle();
                Item.Height = Reader.ReadSingle();
                Item.Length = Reader.ReadSingle();
                Item.Angle = Reader.ReadSingle();
                Item.ModelMeters = Reader.ReadSingle();
                Item.Floor = Reader.ReadInt32();
                Item.Price = Reader.ReadInt32();
                Buildings.Add(Item);
            }

            return Buildings;
        }

        static void WriteFurniture(BinaryWriter Writer, List<FurnitureItem> Furniture)
        {
            int Count = Furniture == null ? 0 : Furniture.Count;
            Writer.Write(Count);
            for (int i = 0; i < Count; i++)
            {
                FurnitureItem Item = Furniture[i];
                Writer.Write((int)Item.Kind);
                Writer.Write(Item.Upright);
                Writer.Write(Item.Open);
                Writer.Write(Item.Interaction ?? string.Empty);
                Writer.Write(Item.Model ?? string.Empty);
                WriteVector(Writer, Item.Position);
                WriteVector(Writer, Item.Swatch);
                WriteVector(Writer, Item.ModelShift);
                Writer.Write(Item.Width);
                Writer.Write(Item.Height);
                Writer.Write(Item.Length);
                Writer.Write(Item.Angle);
                Writer.Write(Item.ModelMeters);
                Writer.Write(Item.Floor);
                Writer.Write(Item.Price);
                int Brushes = Item.Brushes == null ? 0 : Item.Brushes.Count;
                Writer.Write(Brushes);
                for (int b = 0; b < Brushes; b++)
                {
                    ConvexBrush Brush = Item.Brushes[b];
                    int Planes = Brush.Planes == null ? 0 : Brush.Planes.Count;
                    Writer.Write(Planes);
                    for (int p = 0; p < Planes; p++)
                    {
                        Vector4 Plane = Brush.Planes[p];
                        Writer.Write(Plane.X);
                        Writer.Write(Plane.Y);
                        Writer.Write(Plane.Z);
                        Writer.Write(Plane.W);
                    }
                }
            }
        }

        static List<FurnitureItem> ReadFurniture(BinaryReader Reader)
        {
            int Count = Reader.ReadInt32();
            List<FurnitureItem> Furniture = new List<FurnitureItem>(Count);
            for (int i = 0; i < Count; i++)
            {
                FurnitureItem Item = new FurnitureItem();
                Item.Kind = (BoxKind)Reader.ReadInt32();
                Item.Upright = Reader.ReadBoolean();
                Item.Open = Reader.ReadBoolean();
                Item.Interaction = Reader.ReadString();
                Item.Model = Reader.ReadString();
                Item.Position = ReadVector(Reader);
                Item.Swatch = ReadVector(Reader);
                Item.ModelShift = ReadVector(Reader);
                Item.Width = Reader.ReadSingle();
                Item.Height = Reader.ReadSingle();
                Item.Length = Reader.ReadSingle();
                Item.Angle = Reader.ReadSingle();
                Item.ModelMeters = Reader.ReadSingle();
                Item.Floor = Reader.ReadInt32();
                Item.Price = Reader.ReadInt32();
                int Brushes = Reader.ReadInt32();
                if (Brushes > 0)
                {
                    Item.Brushes = new List<ConvexBrush>(Brushes);
                    for (int b = 0; b < Brushes; b++)
                    {
                        ConvexBrush Brush = new ConvexBrush();
                        int Planes = Reader.ReadInt32();
                        for (int p = 0; p < Planes; p++)
                        {
                            Brush.Planes.Add(new Vector4(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()));
                        }

                        Item.Brushes.Add(Brush);
                    }
                }

                Furniture.Add(Item);
            }

            return Furniture;
        }

        static void WriteRecipe(BinaryWriter Writer, ForestRecipe Recipe)
        {
            if (Recipe == null)
            {
                Writer.Write(string.Empty);
                Writer.Write(0);
                Writer.Write(0f);
                Writer.Write(0.7f);
                Writer.Write(-1000f);
                Writer.Write(1000f);
                return;
            }

            Writer.Write(Recipe.SetName ?? string.Empty);
            Writer.Write(Recipe.Seed);
            Writer.Write(Recipe.Density);
            Writer.Write(Recipe.MaxSlope);
            Writer.Write(Recipe.MinHeight);
            Writer.Write(Recipe.MaxHeight);
        }

        static ForestRecipe ReadRecipe(BinaryReader Reader)
        {
            if (Reader.BaseStream.Position >= Reader.BaseStream.Length)
            {
                return new ForestRecipe();
            }

            ForestRecipe Recipe = new ForestRecipe();
            Recipe.SetName = Reader.ReadString();
            Recipe.Seed = Reader.ReadInt32();
            Recipe.Density = Reader.ReadSingle();
            Recipe.MaxSlope = Reader.ReadSingle();
            Recipe.MinHeight = Reader.ReadSingle();
            Recipe.MaxHeight = Reader.ReadSingle();
            return Recipe;
        }

        static void WriteVector(BinaryWriter Writer, Vector3 Value)
        {
            Writer.Write(Value.X);
            Writer.Write(Value.Y);
            Writer.Write(Value.Z);
        }

        static Vector3 ReadVector(BinaryReader Reader)
        {
            return new Vector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
        }
    }
}
