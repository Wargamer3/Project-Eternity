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

        private Model ModelToDraw;

        private string _ModelPath;
        private Vector3 _Offset;
        private Vector3 _Scale;
        private Vector3 _Rotation;

        public Model3DSpawner(BattleMap Map)
            : base("3D model", PropCategories.Visual, new bool[,] { { true } }, false)
        {
            this.Map = Map;

            _ModelPath = string.Empty;

            Scale = Vector3.One;
        }

        public override void Load(ContentManager Content)
        {
        }

        public override void DoLoad(BinaryReader BR)
        {
            _ModelPath = BR.ReadString();

            _Offset = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            _Scale = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            _Rotation = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());

            if (!string.IsNullOrEmpty(_ModelPath))
            {
                ModelToDraw = Map.Content.Load<Model>("Maps/Models/" + _ModelPath);

                foreach (ModelMesh mesh in ModelToDraw.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        Effect NewEffect = Map.Content.Load<Effect>("Shaders/Default Shader 3D").Clone();
                        NewEffect.Parameters["ModelTexture"].SetValue(((BasicEffect)part.Effect).Texture);
                        part.Effect = NewEffect;
                    }
                }
            }
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(_ModelPath);

            BW.Write(_Offset.X);
            BW.Write(_Offset.Y);
            BW.Write(_Offset.Z);

            BW.Write(_Scale.X);
            BW.Write(_Scale.Y);
            BW.Write(_Scale.Z);

            BW.Write(_Rotation.X);
            BW.Write(_Rotation.Y);
            BW.Write(_Rotation.Z);
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
            float aspectRatio = GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);

            if (ModelToDraw != null && Map.Show3DObjects)
            {
                foreach (ModelMesh mesh in ModelToDraw.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect.Parameters["ShowAlpha"].SetValue(0f);
                    }

                }
                
                DrawModel2(ModelToDraw, View, Projection,
                    Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.Y))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.Z))
                    * Matrix.CreateScale(Scale.X, Scale.Y, Scale.Z)
                    * Matrix.CreateTranslation((Position.X + Offset.X) * Map.TileSize.X, (Position.Z + Offset.Z) * Map.LayerHeight, (Position.Y + Offset.Y) * Map.TileSize.Y));
                
                foreach (ModelMesh mesh in ModelToDraw.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect.Parameters["ShowAlpha"].SetValue(1f);
                    }
                }

                GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                DrawModel2(ModelToDraw, View, Projection,
                    Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.Y))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.Z))
                    * Matrix.CreateScale(Scale.X, Scale.Y, Scale.Z)
                    * Matrix.CreateTranslation((Position.X + Offset.X) * Map.TileSize.X, (Position.Z + Offset.Z) * Map.LayerHeight, (Position.Y + Offset.Y) * Map.TileSize.Y));

                GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                GraphicsDevice.BlendState = BlendState.AlphaBlend;
            }
        }

        private void DrawModel(Model model, Matrix view, Matrix projection, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            
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

        private void DrawModel2(Model model, Matrix view, Matrix projection, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(world));
                    part.Effect.Parameters["WorldInverseTransposeMatrix"].SetValue(worldInverseTransposeMatrix);
                    part.Effect.Parameters["WorldMatrix"].SetValue(world);
                    part.Effect.Parameters["ViewMatrix"].SetValue(view);
                    part.Effect.Parameters["ProjectionMatrix"].SetValue(projection);
                    part.Effect.Parameters["AmbienceColor"].SetValue(new Vector4(0.0f, 0.0f, 0.0f, 1));
                    part.Effect.Parameters["DiffuseColor"].SetValue(new Vector4(4f, 4f, 4f, 1));
                    part.Effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(0.3f, 0.9f, 0.9f));
                }

                mesh.Draw();
            }
        }

        protected override InteractiveProp Copy()
        {
            Model3DSpawner NewProp = new Model3DSpawner(Map);

            return NewProp;
        }

        #region Properties

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

        [CategoryAttribute("Model"),
        DescriptionAttribute("The offset."),
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
            }
        }

        [CategoryAttribute("Model"),
        DescriptionAttribute("The scale."),
        DefaultValueAttribute(0)]
        public Vector3 Scale
        {
            get
            {
                return _Scale;
            }
            set
            {
                _Scale = value;
            }
        }

        [CategoryAttribute("Model"),
        DescriptionAttribute("The rotation."),
        DefaultValueAttribute(0)]
        public Vector3 Rotation
        {
            get
            {
                return _Rotation;
            }
            set
            {
                _Rotation = value;
            }
        }

        #endregion
    }
}
