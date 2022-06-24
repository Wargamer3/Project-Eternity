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

        private readonly SpriteFont fntHPNumber;

        public ConquestMapComponent(UnitConquest Unit, ContentManager Content)
        {
            this.Unit = Unit;
            if (Content != null)
            {
                fntHPNumber = Content.Load<SpriteFont>("Fonts/Accuracy Small");
            }
        }

        public override void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor)
        {
            g.Draw(Unit.SpriteMap, new Rectangle((int)Position.X, (int)Position.Y, SizeX, SizeY), UnitColor);
        }

        public override void DrawExtraOnMap(CustomSpriteBatch g, Vector3 Position, Color UnitColor)
        {
            if (Unit.HP < 91)
            {
                int VisibleHP = (int)Math.Ceiling(Unit.HP / 10d);
                g.Draw(GameScreens.GameScreen.sprPixel, new Rectangle((int)Position.X + Width - 15, (int)Position.Y + Height - 15, 15, 15), Color.Black);
                g.DrawStringMiddleAligned(fntHPNumber, VisibleHP.ToString(), new Vector2(Position.X + Width - 9, Position.Y + Height - 15), Color.White);
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
        public int Material;
        public int MaxMaterial;
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

        private UnitMapComponent MapComponents;
        public AIContainer SquadAI;

        public Vector3 Position { get { return MapComponents.Position; } }

        public uint ID { get { return MapComponents.ID; } set { MapComponents.ID = value; } }
        
        public bool CanMove { get { return MapComponents.CanMove; } }

        public string CurrentMovement { get { return MapComponents.CurrentMovement; } set { MapComponents.CurrentMovement = value; } }

        public bool IsFlying { get { return MapComponents.IsFlying; } set { MapComponents.IsFlying = value; } }

        public UnitConquest()
            : base()
        { }

        public UnitConquest(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
            : base(Name)
        {
            _UnitStat = new UnitStats(new bool[1, 1] { { true } });
            _UnitStat.ListTerrainChoices = new List<string>(1);
            _UnitStat.ArrayUnitAbility = new BaseAutomaticSkill[0];
            MapComponents = new ConquestMapComponent(this, Content);
            ArrayCharacterActive = new Character[0];
            
            FileStream FS = new FileStream("Content/Units/Conquest/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            MaxHP = BR.ReadInt32() * 10;
            MaxMovement = BR.ReadInt32();
            MaxAmmo = BR.ReadInt32();
            MaxGaz = BR.ReadInt32();
            MaxMaterial = BR.ReadInt32();
            ListTerrainChoices.Add(BR.ReadString());
            ArmourType = BR.ReadString();
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

            if (Content != null)
            {
                if (File.Exists("Content\\Units\\Conquest\\Map Sprite\\" + Name + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>("Units\\Conquest\\Map Sprite\\" + Name);
                else
                    SpriteMap = Content.Load<Texture2D>("Units/Default");

                SpriteUnit = Content.Load<Texture2D>("Units\\Conquest\\Unit Sprite\\" + Name);
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
            Material = MaxMaterial;

            _UnitStat.PLAAttack = null;

            Attack Weapon1 = new Attack(Weapon1Name);
            Weapon1.RangeMinimum = Weapon1MinimumRange;
            Weapon1.RangeMaximum = Weapon1MaximumRange;
            if (Weapon1PostMovement)
                Weapon1.PostMovementLevel = 1;
            else
                Weapon1.Sec = WeaponSecondaryProperty.None;

            ListAttack.Add(Weapon1);

            Attack Weapon2 = new Attack(Weapon2Name);
            Weapon2.RangeMinimum = Weapon2MinimumRange;
            Weapon2.RangeMaximum = Weapon2MaximumRange;
            if (Weapon2PostMovement)
                Weapon2.PostMovementLevel = 1;
            else
                Weapon2.Sec = WeaponSecondaryProperty.None;

            ListAttack.Add(Weapon2);

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

        public bool IsUnitAtPosition(Vector3 PositionToCheck)
        {
            return UnitStat.IsUnitAtPosition(Position, PositionToCheck);
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
