using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.Core.Units.Conquest
{
    public class ConquestMapComponent : UnitMapComponent
    {
        public override int Width { get { return Unit.SpriteMap.Width; } }
        public override int Height { get { return Unit.SpriteMap.Height; } }
        public override bool[,] ArrayMapSize { get { return Unit.UnitStat.ArrayMapSize; } }
        public override bool IsActive { get { return true; } }

        public UnitConquest Unit;

        private static SpriteFont fntHPNumber;
        private static Texture2D[] sprHPNumber;

        public ConquestMapComponent(UnitConquest Unit, ContentManager Content)
        {
            this.Unit = Unit;
            if (Content != null && fntHPNumber == null)
            {
                fntHPNumber = Content.Load<SpriteFont>("Fonts/Accuracy Small");
                sprHPNumber = new Texture2D[11];
                sprHPNumber[0] = Content.Load<Texture2D>("Conquest/Ressources/Sprites/Digit/16px/0");
                sprHPNumber[1] = Content.Load<Texture2D>("Conquest/Ressources/Sprites/Digit/16px/1");
                sprHPNumber[2] = Content.Load<Texture2D>("Conquest/Ressources/Sprites/Digit/16px/2");
                sprHPNumber[3] = Content.Load<Texture2D>("Conquest/Ressources/Sprites/Digit/16px/3");
                sprHPNumber[4] = Content.Load<Texture2D>("Conquest/Ressources/Sprites/Digit/16px/4");
                sprHPNumber[5] = Content.Load<Texture2D>("Conquest/Ressources/Sprites/Digit/16px/5");
                sprHPNumber[6] = Content.Load<Texture2D>("Conquest/Ressources/Sprites/Digit/16px/6");
                sprHPNumber[7] = Content.Load<Texture2D>("Conquest/Ressources/Sprites/Digit/16px/7");
                sprHPNumber[8] = Content.Load<Texture2D>("Conquest/Ressources/Sprites/Digit/16px/8");
                sprHPNumber[9] = Content.Load<Texture2D>("Conquest/Ressources/Sprites/Digit/16px/9");
                sprHPNumber[10] = Content.Load<Texture2D>("Conquest/Ressources/Sprites/Digit/16px/10");
            }
        }

        public override void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor)
        {
            g.Draw(Unit.SpriteMap, new Rectangle((int)Position.X, (int)Position.Y, SizeX, SizeY), null, UnitColor, 0f, new Vector2(Unit.SpriteMap.Width / 2, Unit.SpriteMap.Height / 2), SpriteEffects.None, 1 / Position.Y);
        }

        public override void Draw3DOnMap(GraphicsDevice GraphicsDevice, Matrix View, Matrix Projection)
        {
            throw new NotImplementedException();
        }

        public override void DrawExtraOnMap(CustomSpriteBatch g, Vector3 Position, Color UnitColor)
        {
            if (Unit.HP < 10)
            {
                int VisibleHP = Unit.HP;
                int Digit = VisibleHP % 10;
                g.Draw(sprHPNumber[Digit], new Vector2(Position.X + 16, Position.Y + 16), null, Color.White, 0f, new Vector2(sprHPNumber[Digit].Width, sprHPNumber[Digit].Height), 1f, SpriteEffects.None, 0f);
            }
        }

        public override void DrawOverlayOnMap(CustomSpriteBatch g, Vector3 Position)
        {
        }

        public override void DrawTimeOfDayOverlayOnMap(CustomSpriteBatch g, Vector3 Position, int TimeOfDay)
        {
            g.Draw(GameScreens.GameScreen.sprPixel, new Rectangle((int)(Position.X - 1) * Width, (int)(Position.Y - 1) * Height, Width * 3, Height * 3), Color.FromNonPremultiplied(255, 255, 255, 120));
        }
    }

    public class UnitConquest : Unit
    {
        public enum ArmourTypes { Infantry, Vehicle, Air, Heli, Ship, Sub };

        public override string UnitTypeName => "Conquest";

        public int Ammo;
        public int MaxAmmo;
        public int Gaz;
        public int MaxGaz;
        public byte MovementTypeIndex => _UnitStat.UnitTypeIndex;
        public string ArmourType;
        public int GazCostPerTurn;
        public int VisionRange;

        public string Weapon1Name;
        public bool Weapon1PostMovement;
        public byte Weapon1MinimumRange;
        public byte Weapon1MaximumRange;

        public string Weapon2Name;
        public bool Weapon2PostMovement;
        public byte Weapon2MinimumRange;
        public byte Weapon2MaximumRange;

        public Dictionary<byte, int> DicUniqueVisionRange;

        private UnitMapComponent MapComponents;
        public AIContainer SquadAI;

        public Vector3 Position { get { return MapComponents.Position; } }

        public uint SpawnID { get { return MapComponents.ID; } set { MapComponents.ID = value; } }

        public bool CanMove { get { return MapComponents.CanMove; } }

        public byte CurrentMovement { get { return MapComponents.CurrentTerrainIndex; } set { MapComponents.CurrentTerrainIndex = value; } }

        public bool IsPlayerControlled { get; set; }
        public bool IsEventSquad { get; set; }

        public UnitConquest()
            : base()
        { }

        public UnitConquest(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
            : base(Name)
        {
            _UnitStat = new UnitStats(new bool[1, 1] { { true } });
            _UnitStat.ArrayUnitAbility = new BaseAutomaticSkill[0];
            MapComponents = new ConquestMapComponent(this, Content);
            ArrayCharacterActive = new Character[0];
            string[] ArrayFileParts = Name.Split('/', '\\');
            ArmourType = ItemName = ArrayFileParts[ArrayFileParts.Length - 1];

            FileStream FS = new FileStream("Content/Conquest/Units/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            MaxHP = BR.ReadInt32();
            MaxMovement = BR.ReadInt32();
            MaxAmmo = BR.ReadInt32();
            MaxGaz = BR.ReadInt32();
            Price = BR.ReadInt32();
            _UnitStat.UnitTypeIndex = BR.ReadByte();
            GazCostPerTurn = BR.ReadInt32();
            VisionRange = BR.ReadInt32();

            Weapon1Name = BR.ReadString();
            Weapon1PostMovement = BR.ReadBoolean();
            Weapon1MinimumRange = BR.ReadByte();
            Weapon1MaximumRange = BR.ReadByte();

            Weapon2Name = BR.ReadString();
            Weapon2PostMovement = BR.ReadBoolean();
            Weapon2MinimumRange = BR.ReadByte();
            Weapon2MaximumRange = BR.ReadByte();

            byte TransportCount = BR.ReadByte();
            for (int i = 0; i < TransportCount; ++i)
            {
            }

            byte UniqueVisionRangeCount = BR.ReadByte();
            DicUniqueVisionRange = new Dictionary<byte, int>(UniqueVisionRangeCount);
            for (int i = 0; i < UniqueVisionRangeCount; ++i)
            {
                byte TerrainIndex = BR.ReadByte();
                if (!DicUniqueVisionRange.ContainsKey(TerrainIndex))
                {
                    DicUniqueVisionRange.Add(TerrainIndex, BR.ReadInt32());
                }
            }

            if (Content != null)
            {
                if (File.Exists("Content/Conquest/Units/Map Sprite/" + Name + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>("Conquest/Units/Map Sprite/" + Name);
                else
                    SpriteMap = Content.Load<Texture2D>("Conquest/Units/Default");

                if (File.Exists("Content/Conquest/Units/Unit Sprite/" + Name + ".xnb"))
                    SpriteUnit = Content.Load<Texture2D>("Conquest/Units/Unit Sprite/" + Name);
                else
                    SpriteUnit = Content.Load<Texture2D>("Conquest/Units/Default");
            }

            FS.Close();
            BR.Close();
        }

        public override Unit FromFile(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            return new UnitConquest(Name, Content, DicRequirement, DicEffect);
        }

        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void DoQuickLoad(BinaryReader BR, ContentManager Content)
        {
        }

        public void InitStat()
        {
            _HP = MaxHP;
            Ammo = MaxAmmo;
            Gaz = MaxGaz;

            _UnitStat.PLAAttack = null;

            Attack Weapon1 = new Attack(Weapon1Name);
            Weapon1.RangeMinimum = Weapon1MinimumRange;
            Weapon1.RangeMaximum = Weapon1MaximumRange;
            if (Weapon1PostMovement)
                Weapon1.PostMovementLevel = 1;
            else
                Weapon1.Sec = WeaponSecondaryProperty.None;

            _UnitStat.ListAttack.Add(Weapon1);

            Attack Weapon2 = new Attack(Weapon2Name);
            Weapon2.RangeMinimum = Weapon2MinimumRange;
            Weapon2.RangeMaximum = Weapon2MaximumRange;
            if (Weapon2PostMovement)
                Weapon2.PostMovementLevel = 1;
            else
                Weapon2.Sec = WeaponSecondaryProperty.None;

            _UnitStat.ListAttack.Add(Weapon2);

            _UnitStat.Init();
        }

        public override void DoInit()
        {
        }

        public void StartTurn()
        {
            MapComponents.StartTurn();
        }

        public void EndTurn()
        {
            MapComponents.EndTurn();
        }

        public override byte GetPostMovementLevel()
        {
            return (byte)_UnitStat.PostMVLevel;
        }

        public bool IsUnitAtPosition(Vector3 PositionToCheck, Point TerrainSize)
        {
            return UnitStat.IsUnitAtPosition(Position, PositionToCheck, TerrainSize);
        }

        public void SetPosition(Vector3 Position)
        {
            MapComponents.SetPosition(Position);
        }

        public float X
        {
            get { return MapComponents.X; }
        }

        public float Y
        {
            get { return MapComponents.Y; }
        }

        public float Z
        {
            get { return MapComponents.Z; }
        }

        public UnitMapComponent Components
        {
            get { return MapComponents; }
        }

        public override GameScreens.GameScreen GetCustomizeScreen(List<Unit> ListPresentUnit, int SelectedUnitIndex, FormulaParser ActiveParser)
        {
            throw new NotImplementedException();
        }
    }
}
