using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class UnitSpawner : InteractiveProp
    {
        public class UnitSelector : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                    provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathUnits);
                    if (Items != null)
                    {
                        value = Items[0].Substring(0, Items[0].Length - 4).Substring(14);
                    }
                }
                return value;
            }
        }

        public class CharacterSelector : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                    provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathCharacters);

                    if (Items != null)
                    {
                        string[] ArraySelectedCharacter = new string[Items.Count];

                        for (int C = 0; C < Items.Count; C++)
                        {
                            ArraySelectedCharacter[C] = Items[C].Substring(0, Items[C].Length - 4).Substring(19);
                        }
                        value = ArraySelectedCharacter;
                    }
                }
                return value;
            }
        }

        private readonly DeathmatchMap Map;

        private BasicEffect PolygonEffect;

        private string _UnitPath;
        private Unit UnitToSpawn;
        private Unit LastUnitSpawned;
        private Tile3D Preview3D;

        private int _TurnsBeforeRespawn;
        private int _MaxNumberOfUnitsSpawned;

        private int _SpawnPlayer;
        private string _AIPath;
        private uint _LeaderUnitId;
        private string _DefenseBattleBehavior;
        private string[] _SpawnCharacter;
        private int _SpawnCharacterLevel;

        public UnitSpawner(DeathmatchMap Map)
            : base("Unit Spawner", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;

            _UnitPath = "";
            AIPath = string.Empty;
            DefenseBattleBehavior = string.Empty;
            _SpawnCharacter = new string[0];
        }

        public override void Load(ContentManager Content)
        {
            if (Content != null)
            {
                PolygonEffect = new BasicEffect(GameScreen.GraphicsDevice);

                PolygonEffect.TextureEnabled = true;
                PolygonEffect.EnableDefaultLighting();

                float aspectRatio = GameScreen.GraphicsDevice.Viewport.Width / (float)GameScreen.GraphicsDevice.Viewport.Height;

                Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                        aspectRatio,
                                                                        1, 10000);
                PolygonEffect.Projection = Projection;

                PolygonEffect.World = Matrix.Identity;
                PolygonEffect.View = Matrix.Identity;
            }
        }

        public override void DoLoad(BinaryReader BR)
        {
            CreateUnit();
        }

        private void CreateUnit()
        {
            if (!string.IsNullOrEmpty(_UnitPath))
            {
                UnitToSpawn = Unit.FromFullName(_UnitPath, Map.Content, Map.Params.DicUnitType, Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget);
                List<Character> ListUnitCharacter = new List<Character>();
                for (int C = 0; C < _SpawnCharacter.Length; C++)
                {
                    Character NewCharacter = new Character(_SpawnCharacter[C], Map.Content, Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget, Map.Params.DicManualSkillTarget);
                    NewCharacter.Level = _SpawnCharacterLevel;
                    ListUnitCharacter.Add(NewCharacter);
                    if (NewCharacter.Slave != null)
                    {
                        NewCharacter.Slave.Level = _SpawnCharacterLevel;
                        ListUnitCharacter.Add(NewCharacter.Slave);
                    }
                }

                UnitToSpawn.ArrayCharacterActive = ListUnitCharacter.ToArray();
                if (UnitToSpawn.Pilot != null)
                {
                    UnitToSpawn.Pilot.Level = _SpawnCharacterLevel;
                }
            }
        }

        private void CreatePreview()
        {
            Preview3D = Map.CreateTile3D(0, Position, Point.Zero, Map.TileSize, Map.TileSize, 0f);
        }

        private void SpawnVehicle()
        {
            MovementAlgorithmTile SpawnTerrain = Map.GetTerrain(Position);

            int X = SpawnTerrain.GridPosition.X;
            int Y = SpawnTerrain.GridPosition.Y;
            float Z = SpawnTerrain.WorldPosition.Z * Map.LayerHeight + 0.1f;

            Squad NewSquad = new Squad("", UnitToSpawn, null, null, null);
            NewSquad.IsEventSquad = false;
            NewSquad.IsPlayerControlled = true;
            NewSquad.SquadDefenseBattleBehavior = DefenseBattleBehavior;

            if (!string.IsNullOrEmpty(AIPath))
            {
                NewSquad.SquadAI = new DeathmatchScripAIContainer(new DeathmatchAIInfo(Map, NewSquad));
                NewSquad.SquadAI.Load(AIPath);
            }

            LastUnitSpawned = UnitToSpawn;

            Map.SpawnSquad(_SpawnPlayer, NewSquad, _LeaderUnitId, new Vector2(X, Y), (int)Z);
        }

        public override void DoSave(BinaryWriter BW)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (UnitToSpawn != null && LastUnitSpawned == null)
            {
                CreatePreview();
                SpawnVehicle();
            }
        }

        public override void OnUnitSelected(ActionPanel PanelOwner, Squad SelectedUnit)
        {
        }

        public override void OnUnitBeforeStop(ActionPanel PanelOwner, Squad StoppedUnit, Vector3 PositionToStopOn)
        {
        }

        public override void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn)
        {
        }

        public override void OnMovedOverBeforeStop(Unit SelectedUnit, Vector3 PositionMovedOn, UnitMapComponent PositionStoppedOn)
        {
        }

        public override void OnUnitStop(Squad StoppedUnit)
        {
        }

        public override void OnUnitStop(Unit StoppedUnit, UnitMapComponent UnitPosition)
        {
        }

        public override void OnBattleEnd(Squad Attacker, Squad Defender)
        {
        }

        public override void OnTurnEnd(int PlayerIndex)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float PosX = (Position.X - Map.Camera2DPosition.X) * Map.TileSize.X;
            float PosY = (Position.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y;
            g.Draw(GameScreen.sprPixel, new Rectangle((int)PosX, (int)PosY, 32, 32), Color.Red);
        }

        public override void Draw3D(GraphicsDevice GraphicsDevice, Matrix View, CustomSpriteBatch g)
        {
            if (Preview3D != null && Map.IsEditor)
            {
                PolygonEffect.View = Unit3D.UnitEffect3D.Parameters["View"].GetValueMatrix();
                PolygonEffect.Texture = UnitToSpawn.SpriteMap;
                PolygonEffect.CurrentTechnique.Passes[0].Apply();

                Preview3D.Draw(g.GraphicsDevice);
            }
        }

        protected override InteractiveProp Copy()
        {
            UnitSpawner NewProp = new UnitSpawner(Map);

            NewProp.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), GameScreen.sprPixel, 1);
            NewProp.PolygonEffect = PolygonEffect;

            return NewProp;
        }

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public int SpawnPlayer
        {
            get
            {
                return _SpawnPlayer;
            }
            set
            {
                _SpawnPlayer = value;
            }
        }

        [Editor(typeof(Core.AI.Selectors.AISelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute("The AI path"),
        DefaultValueAttribute(0)]
        public string AIPath
        {
            get
            {
                return _AIPath;
            }
            set
            {
                _AIPath = value;
            }
        }

        [TypeConverter(typeof(DefenseBattleBehaviorStringConverter)),
        CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute("Defense Battle Behavior."),
        DefaultValueAttribute("Smart Counterattack")]
        public string DefenseBattleBehavior
        {
            get
            {
                return _DefenseBattleBehavior;
            }
            set
            {
                _DefenseBattleBehavior = value;
            }
        }

        [CategoryAttribute("Unit Attributes"),
        DescriptionAttribute(".")]
        public uint LeaderUnitId
        {
            get
            {
                return _LeaderUnitId;
            }
            set
            {
                _LeaderUnitId = value;
            }
        }

        [Editor(typeof(UnitSelector), typeof(UITypeEditor)),
        CategoryAttribute("Unit Attributes"),
        DescriptionAttribute(".")]
        public string UnitPath
        {
            get
            {
                return _UnitPath;
            }
            set
            {
                _UnitPath = value;
                CreateUnit();
                CreatePreview();
            }
        }

        [Editor(typeof(CharacterSelector), typeof(UITypeEditor)),
        TypeConverter(typeof(CsvConverter)),
        CategoryAttribute("Unit Attributes"),
        DescriptionAttribute(".")]
        public string[] SpawnCharacter
        {
            get
            {
                return _SpawnCharacter;
            }
            set
            {
                _SpawnCharacter = value;
            }
        }

        [CategoryAttribute("Unit Attributes"),
        DescriptionAttribute(".")]
        public int SpawnCharacterLevel
        {
            get
            {
                return _SpawnCharacterLevel;
            }
            set
            {
                if (value < 1)
                    _SpawnCharacterLevel = 1;
                else
                    _SpawnCharacterLevel = value;
            }
        }
    }
}
