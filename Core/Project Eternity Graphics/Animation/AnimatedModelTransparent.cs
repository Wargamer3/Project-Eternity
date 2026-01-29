using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core.Graphics
{
    public class AnimatedModelTransparent
    {
        public AnimatedModel ModelToDraw;

        private AnimatedModelTransparent()
        {
        }

        public AnimatedModelTransparent(AnimatedModelTransparent Clone)
        {
            ModelToDraw = new AnimatedModel(Clone.ModelToDraw);
        }

        public static AnimatedModelTransparent Load(ContentManager Content, string FilePath)
        {
            return Load(Content, FilePath, new Vector4(0.0f, 0.0f, 0.0f, 1), new Vector4(40000f, 40000f, 40000f, 1), new Vector3(-0.1f, -0.1f, -0.9f));
        }

        public static AnimatedModelTransparent Load(ContentManager Content, string FilePath, Vector4 AmbienceColor, Vector4 DiffuseColor, Vector3 DiffuseLightDirection)
        {
            AnimatedModelTransparent NewModelTransparent = new AnimatedModelTransparent();

            AnimatedModel NewModel = new AnimatedModel(FilePath);

            NewModel.LoadContent(Content);

            foreach (ModelMesh mesh in NewModel.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    if (part.Effect is BasicEffect)
                    {
                        Effect NewEffect = Content.Load<Effect>("Shaders/Default Shader 3D").Clone();
                        NewEffect.Parameters["ModelTexture"].SetValue(((BasicEffect)part.Effect).Texture);
                        NewEffect.Parameters["AmbienceColor"].SetValue(AmbienceColor);
                        NewEffect.Parameters["DiffuseColor"].SetValue(DiffuseColor);
                        NewEffect.Parameters["DiffuseLightDirection"].SetValue(DiffuseLightDirection);
                        NewEffect.Parameters["UseLights"].SetValue(0f);
                        part.Effect = NewEffect;
                    }
                }
            }

            NewModelTransparent.ModelToDraw = NewModel;

            return NewModelTransparent;
        }

        public void SetLightDirection(Vector3 DiffuseLightDirection)
        {
            foreach (ModelMesh mesh in ModelToDraw.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect.Parameters["DiffuseLightDirection"].SetValue(DiffuseLightDirection);
                }
            }
        }

        public void DisableLights()
        {
            ModelToDraw.DisableLights();
        }

        public void Draw3D(GraphicsDevice GraphicsDevice, Matrix View, Matrix Projection, Matrix World)
        {
            foreach (ModelMesh mesh in ModelToDraw.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect.Parameters["ShowAlpha"].SetValue(0f);
                }

            }

            DrawModel(ModelToDraw, View, Projection, World);

            foreach (ModelMesh mesh in ModelToDraw.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect.Parameters["ShowAlpha"].SetValue(1f);
                }
            }

            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            DrawModel(ModelToDraw, View, Projection, World);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        private void DrawModel(AnimatedModel ModelToDraw, Matrix View, Matrix Projection, Matrix World)
        {
            var model = ModelToDraw.Model;
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh ActiveMesh in model.Meshes)
            {
                foreach (ModelMeshPart part in ActiveMesh.MeshParts)
                {
                    Matrix WorldFinal = ModelToDraw.Bones[ActiveMesh.ParentBone.Index].AbsoluteTransform * World;
                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(WorldFinal));
                    part.Effect.Parameters["WorldInverseTransposeMatrix"].SetValue(worldInverseTransposeMatrix);
                    part.Effect.Parameters["WorldMatrix"].SetValue(WorldFinal);
                    part.Effect.Parameters["ViewMatrix"].SetValue(View);
                    part.Effect.Parameters["ProjectionMatrix"].SetValue(Projection);
                }

                ActiveMesh.Draw();
            }
        }
    }
}
