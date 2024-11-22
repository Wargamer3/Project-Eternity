using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMapLight;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class LightSource : InteractiveProp
    {
        private Texture2D sprFlag;
        private readonly BattleMap Map;

        private Vector3 _Offset;

        private BattleMapLight Light;

        private bool IsUsed;

        public LightSource(BattleMap Map)
            : base("Light", PropCategories.Visual, new bool[,] { { true } }, false)
        {
            this.Map = Map;
            IsUsed = false;

            Light = new BattleMapLight(GameScreen.GraphicsDevice, ShadowmapSizes.Size256, new Vector3(200, 200, 0), Color.Red);
        }

        public LightSource(BattleMap Map, LightSource Clone)
            : base("Light", PropCategories.Visual, new bool[,] { { true } }, false)
        {
            this.Map = Map;
            IsUsed = false;

            sprFlag = Clone.sprFlag;
            Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Clone.Unit3D.UnitEffect3D, Clone.sprFlag, 1);

            Light = new BattleMapLight(GameScreen.GraphicsDevice, ShadowmapSizes.Size256, new Vector3(200, 200, 0), Color.Red);
            Map.ListLight.Add(Light);
        }

        public override void LoadPreset(ContentManager Content)
        {
            if (!string.IsNullOrEmpty(Light.AIPath))
            {
                Light.AI = new BattleLightScripAIContainer(new BattleLightAIInfo(Map, Light));
                Light.AI.Load(Light.AIPath);
            }

            if (Content != null)
            {
                sprFlag = Content.Load<Texture2D>("Animations/Sprites/Spawn Weapons/Flag Red");
                Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Billboard 3D"), sprFlag, 1);
            }
        }

        public override void DoLoad(BinaryReader BR)
        {
            LightType = (LightTypes)BR.ReadByte();
            ShadowmapSize = (ShadowmapSizes)BR.ReadByte();

            Offset = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());

            PrimaryColor = Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), BR.ReadByte());
            SecondaryColor = Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), BR.ReadByte());

            TransitionsSpeed = BR.ReadSingle();
            TransitionsPhase = BR.ReadSingle();

            Size = BR.ReadSingle();
            CastShadows = BR.ReadBoolean();

            AIPath = BR.ReadString();
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)LightType);
            BW.Write((byte)ShadowmapSize);
            BW.Write(_Offset.X);
            BW.Write(_Offset.Y);
            BW.Write(_Offset.Z);

            BW.Write(PrimaryColor.R);
            BW.Write(PrimaryColor.G);
            BW.Write(PrimaryColor.B);
            BW.Write(PrimaryColor.A);
            BW.Write(SecondaryColor.R);
            BW.Write(SecondaryColor.G);
            BW.Write(SecondaryColor.B);
            BW.Write(SecondaryColor.A);

            BW.Write(TransitionsSpeed);
            BW.Write(TransitionsPhase);

            BW.Write(Size);
            BW.Write(CastShadows);

            BW.Write(AIPath);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnUnitSelected(ActionPanel PanelOwner, Squad SelectedUnit)
        {
        }

        public override void OnUnitBeforeStop(ActionPanel PanelOwner, Squad StoppedUnit, Vector3 PositionToStopOn)
        {
        }

        public override void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn)
        {
            if (PositionMovedOn.X == Position.X && PositionMovedOn.Y == Position.Y)
            {
                IsUsed = true;
            }
        }

        public override void OnMovedOverBeforeStop(Unit SelectedUnit, Vector3 PositionMovedOn, UnitMapComponent PositionStoppedOn)
        {
            if (PositionMovedOn.X == Position.X && PositionMovedOn.Y == Position.Y)
            {
                IsUsed = true;
            }
        }

        public override void OnUnitStop(Squad StoppedUnit)
        {
            if (StoppedUnit.X == Position.X && StoppedUnit.Y == Position.Y)
            {
                IsUsed = true;
            }
        }

        public override void OnUnitStop(Unit StoppedUnit, UnitMapComponent UnitPosition)
        {
            if (UnitPosition.X == Position.X && UnitPosition.Y == Position.Y)
            {
                IsUsed = true;
            }
        }

        public override void OnBattleEnd(Squad Attacker, Squad Defender)
        {
        }

        public override void OnTurnEnd(int PlayerIndex)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float PosX = Position.X - Map.Camera2DPosition.X;
            float PosY = Position.Y - Map.Camera2DPosition.Y;

            g.Draw(sprFlag, new Vector2(PosX, PosY), null, Color.White, 0f, new Vector2(sprFlag.Width / 2, sprFlag.Height / 2), 1f, SpriteEffects.None, 0f);
        }

        public override void Draw3D(GraphicsDevice GraphicsDevice, Matrix View, CustomSpriteBatch g)
        {
            if (!IsUsed)
            {
                Unit3D.SetViewMatrix(View);
                Unit3D.Draw(GraphicsDevice);
            }
        }

        protected override InteractiveProp Copy()
        {
            LightSource NewProp = new LightSource(Map, this);

            return NewProp;
        }

        #region Properties

        [CategoryAttribute("Light"),
        DescriptionAttribute(@"Select whether this light will stay on indefinitely(Fixed), flicker
 wildly between two colors(Flicker), Switch from one color to another for a few
 seconds(Switch) or transition smoothly from one color to the next(Pulse)"),
        DefaultValueAttribute(LightTypes.Fixed)]
        public LightTypes LightType
        {
            get
            {
                return Light.LightType;
            }
            set
            {
                Light.LightType = value;
            }
        }

        [CategoryAttribute("Light"),
        DescriptionAttribute("Quality of the shadows."),
        DefaultValueAttribute(ShadowmapSizes.Size256)]
        public ShadowmapSizes ShadowmapSize
        {
            get
            {
                return (ShadowmapSizes)(((int)Light.Size) >> 2);
            }
            set
            {
                int baseSize = 2 << (int)value;
                Light.Size = baseSize;
            }
        }

        [CategoryAttribute("Light"),
        DescriptionAttribute("Size of the light."),
        DefaultValueAttribute(200f)]
        public float Size
        {
            get
            {
                return Light.Size;
            }
            set
            {
                Light.Size = value;
            }
        }

        [CategoryAttribute("Light"),
        DescriptionAttribute("Compute shadows, not needed if not interacting with anything."),
        DefaultValueAttribute(true)]
        public bool CastShadows
        {
            get
            {
                return Light.CastShadows;
            }
            set
            {
                Light.CastShadows = value;
            }
        }

        [CategoryAttribute("Light"),
        DescriptionAttribute("Position offset."),
        DefaultValueAttribute(0)]
        public Vector3 Offset
        {
            get
            {
                return _Offset;
            }
            set
            {
                _Offset = value;
                Light.Position = Position + Offset;
            }
        }

        [CategoryAttribute("Light"),
        DescriptionAttribute("Primary color."),
        DefaultValueAttribute(0)]
        public Color PrimaryColor
        {
            get
            {
                return Light.PrimaryColor;
            }
            set
            {
                Light.PrimaryColor = value;
            }
        }

        [CategoryAttribute("Light"),
        DescriptionAttribute("Color to use if switching between colors."),
        DefaultValueAttribute(0)]
        public Color SecondaryColor
        {
            get
            {
                return Light.SecondaryColor;
            }
            set
            {
                Light.SecondaryColor = value;
            }
        }

        [CategoryAttribute("Light"),
        DescriptionAttribute("The speed to complete a transitions."),
        DefaultValueAttribute(0)]
        public float TransitionsSpeed
        {
            get
            {
                return Light.TransitionsSpeed;
            }
            set
            {
                Light.TransitionsSpeed = value;
            }
        }

        [CategoryAttribute("Light"),
        DescriptionAttribute("Time to wait between each cycle."),
        DefaultValueAttribute(0)]
        public float TransitionsPhase
        {
            get
            {
                return Light.TransitionsPhase;
            }
            set
            {
                Light.TransitionsPhase = value;
            }
        }

        [Editor(typeof(Selectors.AISelector), typeof(UITypeEditor)), 
        CategoryAttribute("Light"),
        DescriptionAttribute("Time to wait between each cycle."),
        DefaultValueAttribute(0)]
        public string AIPath
        {
            get
            {
                return Light.AIPath;
            }
            set
            {
                Light.AIPath = value;
            }
        }

        #endregion
    }
}
