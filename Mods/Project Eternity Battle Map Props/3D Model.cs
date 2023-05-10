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

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Model3DSpawner : InteractiveProp
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

        private readonly BattleMap Map;

        Model ModelToDraw;

        private string _ModelPath;

        public Model3DSpawner(BattleMap Map)
            : base("3D model", PropCategories.Visual, new bool[,] { { true } }, false)
        {
            this.Map = Map;

            _ModelPath = string.Empty;
        }

        public override void Load(ContentManager Content)
        {
        }

        public override void DoLoad(BinaryReader BR)
        {
            _ModelPath = BR.ReadString();

            if (!string.IsNullOrEmpty(_ModelPath))
            {
                ModelToDraw = Map.Content.Load<Model>(_ModelPath);
            }
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(_ModelPath);
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
        }

        public override void OnUnitStop(Squad StoppedUnit)
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
            float PosX = (Position.X - Map.CameraPosition.X) * Map.TileSize.X;
            float PosY = (Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y;
            g.Draw(GameScreen.sprPixel, new Rectangle((int)PosX, (int)PosY, 32, 32), Color.Red);
        }

        public override void Draw3D(GraphicsDevice GraphicsDevice, Matrix View, CustomSpriteBatch g)
        {
            if (ModelToDraw != null)
            {
                DrawModel(ModelToDraw, View, Projection, Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateScale(32) * Matrix.CreateTranslation((Position.X + 0.5f) * Map.TileSize.X, Position.Z * 32, (Position.Y + 0.5f) * Map.TileSize.Y));
            }
        }

        private void DrawModel(Model model, Matrix view, Matrix projection, Matrix world)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

        protected override InteractiveProp Copy()
        {
            Model3DSpawner NewProp = new Model3DSpawner(Map);

            return NewProp;
        }

        [Editor(typeof(ModelSelector), typeof(UITypeEditor)),
        CategoryAttribute("Model"),
        DescriptionAttribute("The model path."),
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
                ModelToDraw = Map.Content.Load<Model>("Maps/Models/" + _ModelPath);
            }
        }
    }
}
