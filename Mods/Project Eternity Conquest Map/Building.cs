using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Units.Conquest;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class BuildingConquest
    {
        public string SpriteUnitPath;
        public Texture2D SpriteUnit;

        public string SpriteMapPath;
        public Texture2D SpriteMap;
        public UnitMap3D Unit3DSprite;
        public string Model3DPath;
        public AnimatedModel Unit3DModel;

        public readonly string Name;
        public readonly string RelativePath;
        public readonly List<UnitConquest> ListUnitToSpawn;
        private UnitMapComponent MapComponents;
        public byte TerrainType;
        public bool CanBeCaptured;
        public byte VisionRange;
        public int HP;
        public bool Resupply;


        public BuildingConquest()
        {
        }

        public BuildingConquest(string FilePath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            RelativePath = FilePath;
            Name = FilePath;

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

            TerrainType = BR.ReadByte();
            CanBeCaptured = BR.ReadBoolean();
            VisionRange = BR.ReadByte();
            HP = BR.ReadInt32();
            Resupply = BR.ReadBoolean();

            FS.Close();
            BR.Close();

            if (Content != null)
            {
                if (File.Exists("Buildings\\Conquest\\Map Sprite\\" + Name + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>("Buildings\\Conquest\\Map Sprite\\" + Name);
                else
                    SpriteMap = Content.Load<Texture2D>("Units/Default");

                SpriteUnit = Content.Load<Texture2D>("Buildings\\Conquest\\Menu Sprite\\" + Name);
            }
        }
    }
}
