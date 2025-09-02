using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Units.Conquest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class BuildingConquest
    {
        public string SpriteUnitPath;
        public AnimatedSprite SpriteUnit;

        public string SpriteMapPath;
        public AnimatedSprite SpriteMap;
        public UnitMap3D Unit3DSprite;
        public string Model3DPath;
        public AnimatedModel Unit3DModel;

        public readonly string Name;
        public readonly string RelativePath;
        public readonly List<UnitConquest> ListUnitToSpawn;
        public byte TerrainType;
        public bool CanBeCaptured;
        public byte VisionRange;
        public int MaxHP;
        public int CurrentHP;
        public int CreditPerTurn;
        public bool Resupply;

        public Vector3 Position;
        public uint SpawnID;
        public int CapturedTeamIndex;

        public BuildingConquest()
        {
        }

        public BuildingConquest(string FilePath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            RelativePath = FilePath;
            string[] ArrayFileParts = FilePath.Split('/', '\\');
            Name = ArrayFileParts[ArrayFileParts.Length - 1];

            FileStream FS = new FileStream("Content/Buildings/Conquest/" + FilePath + ".peb", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            int ListUnitToSpawnCount = BR.ReadInt32();
            ListUnitToSpawn = new List<UnitConquest>(ListUnitToSpawnCount);
            for (int i = 0; i < ListUnitToSpawnCount; ++i)
            {
                UnitConquest LoadedUnit = new UnitConquest(BR.ReadString(), null, DicRequirement, DicEffect);
                ListUnitToSpawn.Add(LoadedUnit);
            }

            CapturedTeamIndex = -1;
            TerrainType = BR.ReadByte();
            CanBeCaptured = BR.ReadBoolean();
            VisionRange = BR.ReadByte();
            MaxHP = BR.ReadInt32();
            CreditPerTurn = BR.ReadInt32();
            Resupply = BR.ReadBoolean();

            FS.Close();
            BR.Close();

            if (Content != null)
            {
                if (File.Exists("Content/Buildings/Conquest/Map Sprites/" + RelativePath + ".xnb"))
                    SpriteMap = new AnimatedSprite(Content, "Buildings/Conquest/Map Sprites/" + RelativePath, Vector2.Zero, 4, 1, 1);
                else
                    SpriteMap = new AnimatedSprite(Content, "Units/Default", Vector2.Zero, 1);

                SpriteMap.Origin = new Vector2(16, 48);

                if (File.Exists("Content/Buildings/Conquest/Menu Sprites/" + RelativePath + ".xnb"))
                    SpriteUnit = new AnimatedSprite(Content, "Buildings/Conquest/Menu Sprites/" + RelativePath, Vector2.Zero, 4, 1, 1);
                else
                    SpriteUnit = new AnimatedSprite(Content, "Units/Default", Vector2.Zero, 1);
            }
        }

        public void InitStats()
        {
            CurrentHP = MaxHP;
        }

        public void SetPosition(Vector3 Position)
        {
            this.Position = Position;
        }
    }
}
