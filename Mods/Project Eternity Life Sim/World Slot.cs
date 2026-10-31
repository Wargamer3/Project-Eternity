using System;
using System.Collections.Generic;
using System.IO;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum GroundKind
    {
        None = 0,
        Grid = 1,
        Heightmap = 2,
        Mesh = 3,
        Cubes = 4,
    }

    public class WorldSlot
    {
        public const float DefaultChunkSize = 256f;
        public const float DefaultSpacing = 16f;
        public const float DefaultGroundRadius = 800f;
        public const float DefaultLandmarkReach = 2400f;
        public const float TileMeters = 8f;

        public int Index;
        public string Folder;
        public float ChunkSize = DefaultChunkSize;
        public float Spacing = DefaultSpacing;
        public float GroundRadius = DefaultGroundRadius;
        public float LandmarkReach = DefaultLandmarkReach;
        public float TimeStart = 12f;
        public float HoursInDay = 24f;
        public float TimeMultiplier = 1f;
        public EnvironmentManager.TimeLoopTypes TimeLoop = EnvironmentManager.TimeLoopTypes.FirstDay;
        public EnvironmentManager.TimePeriods TimePeriod = EnvironmentManager.TimePeriods.RealTime;
        public int Seed;

        public string ChunkFolder
        {
            get { return Path.Combine(Folder, "Chunks"); }
        }

        public string MinimapPath
        {
            get { return Path.Combine(Folder, "minimap.png"); }
        }

        public string CitiesPath
        {
            get { return Path.Combine(Folder, "cities.bin"); }
        }

        public string ChunkPath(int X, int Z)
        {
            return Path.Combine(ChunkFolder, X.ToString() + "_" + Z.ToString() + ".bin");
        }

        public static string WorldsRoot()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Life Sim", "Worlds");
        }

        public static string ModelsRoot()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Life Sim", "Assets", "Models");
        }

        public static WorldSlot OpenOrCreate()
        {
            int Latest = LatestIndex();
            if (Latest > 0)
            {
                WorldSlot Existing = Read(Latest);
                if (Existing != null)
                {
                    return Existing;
                }
            }

            WorldSlot Created = new WorldSlot();
            Created.Index = 1;
            Created.Folder = Path.Combine(WorldsRoot(), "Save 1", "Overworld");
            Directory.CreateDirectory(Created.ChunkFolder);
            Created.Write();
            ChunkFile.WriteFlat(Created, 0, 0, 8f);
            return Created;
        }

        public static int LatestIndex()
        {
            string Root = WorldsRoot();
            if (!Directory.Exists(Root))
            {
                return 0;
            }

            int Latest = 0;
            string[] Folders = Directory.GetDirectories(Root);
            for (int i = 0; i < Folders.Length; i++)
            {
                string Name = Path.GetFileName(Folders[i]);
                if (Name == null || !Name.StartsWith("Save "))
                {
                    continue;
                }

                int Index;
                if (!int.TryParse(Name.Substring(5), out Index))
                {
                    continue;
                }

                string World = Path.Combine(Folders[i], "Overworld", "world.bin");
                if (Index > Latest && File.Exists(World))
                {
                    Latest = Index;
                }
            }

            return Latest;
        }

        public static int NextIndex()
        {
            return Math.Max(1, LatestIndex() + 1);
        }

        public static WorldSlot CreateEmpty(int Index)
        {
            WorldSlot Slot = new WorldSlot();
            Slot.Index = Index;
            Slot.Folder = Path.Combine(WorldsRoot(), "Save " + Index.ToString(), "Overworld");
            Directory.CreateDirectory(Slot.ChunkFolder);
            return Slot;
        }

        public static WorldSlot Read(int Index)
        {
            WorldSlot Slot = new WorldSlot();
            Slot.Index = Index;
            Slot.Folder = Path.Combine(WorldsRoot(), "Save " + Index.ToString(), "Overworld");
            string PathName = Path.Combine(Slot.Folder, "world.bin");
            if (!File.Exists(PathName))
            {
                return null;
            }

            using (FileStream Stream = new FileStream(PathName, FileMode.Open, FileAccess.Read))
            using (BinaryReader Reader = new BinaryReader(Stream))
            {
                byte[] Magic = Reader.ReadBytes(4);
                if (Magic.Length < 4 || Magic[0] != (byte)'L' || Magic[1] != (byte)'S' || Magic[2] != (byte)'W' || Magic[3] != (byte)'1')
                {
                    return null;
                }

                int Version = Reader.ReadInt32();
                if (Version != 2)
                {
                    return null;
                }

                Slot.ChunkSize = Reader.ReadSingle();
                Slot.Spacing = Reader.ReadSingle();
                Slot.GroundRadius = Reader.ReadSingle();
                Slot.LandmarkReach = Reader.ReadSingle();
                Slot.TimeStart = Reader.ReadSingle();
                Slot.HoursInDay = Reader.ReadSingle();
                Slot.TimeMultiplier = Reader.ReadSingle();
                Slot.TimeLoop = (EnvironmentManager.TimeLoopTypes)Reader.ReadByte();
                Slot.TimePeriod = (EnvironmentManager.TimePeriods)Reader.ReadByte();
                Slot.Seed = Reader.ReadInt32();
            }

            if (Slot.ChunkSize < 1f)
            {
                Slot.ChunkSize = DefaultChunkSize;
            }

            if (Slot.Spacing < 1f)
            {
                Slot.Spacing = DefaultSpacing;
            }

            return Slot;
        }

        public void Write()
        {
            Directory.CreateDirectory(Folder);
            string PathName = Path.Combine(Folder, "world.bin");
            using (FileStream Stream = new FileStream(PathName, FileMode.Create, FileAccess.Write))
            using (BinaryWriter Writer = new BinaryWriter(Stream))
            {
                Writer.Write((byte)'L');
                Writer.Write((byte)'S');
                Writer.Write((byte)'W');
                Writer.Write((byte)'1');
                Writer.Write(2);
                Writer.Write(ChunkSize);
                Writer.Write(Spacing);
                Writer.Write(GroundRadius);
                Writer.Write(LandmarkReach);
                Writer.Write(TimeStart);
                Writer.Write(HoursInDay);
                Writer.Write(TimeMultiplier);
                Writer.Write((byte)TimeLoop);
                Writer.Write((byte)TimePeriod);
                Writer.Write(Seed);
            }
        }

        public void ApplyClock(LifeSimMap Map)
        {
            if (Map == null || Map.MapEnvironment == null)
            {
                return;
            }

            Map.MapEnvironment.TimeStart = TimeStart;
            Map.MapEnvironment.HoursInDay = HoursInDay;
            Map.MapEnvironment.TimeMultiplier = TimeMultiplier;
            Map.MapEnvironment.TimeLoopType = TimeLoop;
            Map.MapEnvironment.TimePeriodType = TimePeriod;
        }
    }

    public class ForestRecipe
    {
        public string SetName = string.Empty;
        public int Seed;
        public float Density;
        public float MaxSlope = 0.7f;
        public float MinHeight = -1000f;
        public float MaxHeight = 1000f;

        public bool HasTrees
        {
            get { return !string.IsNullOrEmpty(SetName) && Density > 0f; }
        }
    }

    public class MeshBand
    {
        public float MinY = float.MinValue;
        public float MaxY = float.MaxValue;
        public string Model = string.Empty;
        public float Scale = 16f;
    }

    public class RoomHeader
    {
        public Microsoft.Xna.Framework.Vector3 Min;
        public Microsoft.Xna.Framework.Vector3 Max;
        public long PropOffset;
        public int PropLength;
        public bool PropsOpen;
        public List<FurnitureItem> Props = new List<FurnitureItem>();
    }

    public class LandmarkHeader
    {
        public string Name = string.Empty;
        public string Mesh = string.Empty;
        public Microsoft.Xna.Framework.Vector3 Min;
        public Microsoft.Xna.Framework.Vector3 Max;
        public List<RoomHeader> Rooms = new List<RoomHeader>();
        public bool MeshOpen;
    }

    public struct ScatterSpot
    {
        public Microsoft.Xna.Framework.Vector3 Position;
        public string Model;
        public float Meters;
        public float Height;
    }
}
