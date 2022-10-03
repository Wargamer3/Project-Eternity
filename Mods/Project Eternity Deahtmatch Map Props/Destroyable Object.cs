using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ModelSelector : UITypeEditor
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
                List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathMapModels);
                if (Items != null)
                {
                    value = Items[0].Substring(0, Items[0].Length - 4).Substring(20);
                }
            }
            return value;
        }
    }

    public class PropSelector : UITypeEditor
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
                DestroyableObject Owner = (DestroyableObject)context.Instance;
                PropPicker NewPropPicker = new PropPicker(Owner.Map, Owner, (InteractiveProp)value);
                NewPropPicker.ShowDialog();
                value = NewPropPicker.pgPropProperties.SelectedObject;
            }
            return value;
        }
    }

    public class TerrainSelector : UITypeEditor
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
                DestroyableObject Owner = (DestroyableObject)context.Instance;
                TileAttributes TileAttributesForm = new TileAttributes();
                TileAttributesForm.Init((Terrain)value, Owner.Map);
                TileAttributesForm.cboTerrainType.Items.Insert(0, "None");
                TileAttributesForm.ShowDialog();

                value = TileAttributesForm.ActiveTerrain;
            }
            return value;
        }
    }

    public class DestroyableObject : InteractiveProp
    {
        private Texture2D sprFlag;

        public readonly DeathmatchMap Map;

        private string _SpritePath;
        private string _ModelPath;

        private string _DestroyedSpritePath;
        private string _DestroyedModelPath;

        private InteractiveProp _DroppedProp;

        private Terrain _ReplacementTerrain;

        public DestroyableObject(DeathmatchMap Map)
            : base("Destroyable Object", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;
            _SpritePath = string.Empty;
            _ModelPath = string.Empty;

            _DestroyedSpritePath = string.Empty;
            _DestroyedModelPath = string.Empty;

            _ReplacementTerrain = new Terrain(0, 0, 0, 0);
        }

        public override void Load(ContentManager Content)
        {
        }

        public override void DoLoad(BinaryReader BR)
        {
            _SpritePath = BR.ReadString();
            _ModelPath = BR.ReadString();

            _DestroyedSpritePath = BR.ReadString();
            _DestroyedModelPath = BR.ReadString();

            string DroppedPropPath = BR.ReadString();

            if (!string.IsNullOrEmpty(_SpritePath))
            {
                sprFlag = Map.Content.Load<Texture2D>("Animations/Sprites/" + _SpritePath);
                CreateUnit3D();
            }

            if (!string.IsNullOrEmpty(_ModelPath))
            {
                Unit3DModel = new AnimatedModel("Maps/Models/" + _ModelPath);
                Unit3DModel.LoadContent(Map.Content);
                Unit3DModel.AddAnimation("Maps/Models/" + _ModelPath, "Idle", Map.Content);
                Unit3DModel.PlayAnimation("Idle");
            }
        }

        private void CreateUnit3D()
        {
            Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), sprFlag, 1);

            float TerrainZ = Map.LayerManager.ListLayer[(int)Position.Z].ArrayTerrain[(int)Position.X, (int)Position.Y].WorldPosition.Z;

            Unit3D.SetPosition(
                (Position.X + 0.5f) * Map.TileSize.X,
                TerrainZ * Map3DDrawable.LayerHeight,
                (Position.Y + 0.5f) * Map.TileSize.Y);
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(_SpritePath);
            BW.Write(_ModelPath);

            BW.Write(_DestroyedSpritePath);
            BW.Write(_DestroyedModelPath);

            if (_DroppedProp != null)
            {
                BW.Write(_DroppedProp.PropName);
                _DroppedProp.DoSave(BW);
            }
            else
            {
                BW.Write(string.Empty);
            }
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
            }
        }

        public override void OnUnitStop(Squad StoppedUnit)
        {
            if (StoppedUnit.X == Position.X && StoppedUnit.Y == Position.Y)
            {
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
            float PosX = (Position.X - Map.CameraPosition.X) * Map.TileSize.X;
            float PosY = (Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

            if (sprFlag == null)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle((int)PosX, (int)PosY, 32, 32), Color.Red);
            }
            else
            {
                g.Draw(sprFlag, new Vector2(PosX, PosY), Color.White);
            }
        }

        public override void Draw3D(GraphicsDevice GraphicsDevice, Matrix View, CustomSpriteBatch g)
        {
            if (Unit3D != null)
            {
                Unit3D.SetViewMatrix(View);
                Unit3D.Draw(GraphicsDevice);
            }
            else if (Unit3DModel != null)
            {
                Unit3DModel.Draw(View, Projection, Matrix.CreateTranslation((Position.X + 0.5f) * Map.TileSize.X, Position.Z * Map3DDrawable.LayerHeight, (Position.Y + 0.5f) * Map.TileSize.Y));
            }
        }

        protected override InteractiveProp Copy()
        {
            DestroyableObject NewProp = new DestroyableObject(Map);

            NewProp._SpritePath = _SpritePath;
            NewProp._ModelPath = _ModelPath;

            NewProp._DestroyedSpritePath = _DestroyedSpritePath;
            NewProp._DestroyedModelPath = _DestroyedModelPath;

            if (sprFlag != null)
            {
                NewProp.sprFlag = sprFlag;
                NewProp.CreateUnit3D();
            }

            if (Unit3DModel != null)
            {
                NewProp.Unit3DModel = new AnimatedModel("Maps/Models/" + _ModelPath);
                NewProp.Unit3DModel.LoadContent(Map.Content);
            }

            return NewProp;
        }

        #region Properties

        [Editor(typeof(AnimationSpritesSelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner"),
        DescriptionAttribute("The sprite path."),
        DefaultValueAttribute(0)]
        public string SpritePath
        {
            get
            {
                return _SpritePath;
            }
            set
            {
                _SpritePath = value;
                sprFlag = GameScreen.ContentFallback.Load<Texture2D>("Animations/Sprites/" + _SpritePath);
                CreateUnit3D();
            }
        }

        [Editor(typeof(ModelSelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner"),
        DescriptionAttribute("The sprite path."),
        DefaultValueAttribute(0)]
        public string ModelPath
        {
            get
            {
                return _ModelPath;
            }
            set
            {
                _ModelPath = value;
                Unit3DModel = new AnimatedModel("Maps/Models/" + _ModelPath);
                Unit3DModel.LoadContent(Map.Content);
            }
        }

        [Editor(typeof(AnimationSpritesSelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner"),
        DescriptionAttribute("The sprite path."),
        DefaultValueAttribute(0)]
        public string DestroyedSpritePath
        {
            get
            {
                return _SpritePath;
            }
            set
            {
                _DestroyedSpritePath = value;
            }
        }

        [Editor(typeof(ModelSelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner"),
        DescriptionAttribute("The sprite path."),
        DefaultValueAttribute(0)]
        public string DestroyedModelPath
        {
            get
            {
                return _DestroyedModelPath;
            }
            set
            {
                _DestroyedModelPath = value;
            }
        }


        [Editor(typeof(PropSelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner"),
        DescriptionAttribute("The sprite path."),
        DefaultValueAttribute(0)]
        public InteractiveProp DroppedProp
        {
            get
            {
                return _DroppedProp;
            }
            set
            {
                _DroppedProp = value;
            }
        }


        [Editor(typeof(TerrainSelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner"),
        DescriptionAttribute("The sprite path."),
        DefaultValueAttribute(0)]
        public Terrain ReplacementTerrain
        {
            get
            {
                return _ReplacementTerrain;
            }
            set
            {
                _ReplacementTerrain = value;
            }
        }

        #endregion
    }
}
