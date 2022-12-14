using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.RacingScreen
{
    public class AITunnel : Object3D
    {
        public List<AITunnel> ListNextAITunnel;
        private VertexPositionNormalTexture[] ArrayVertexBound;
        private Lines3D VisibleDirection;

        public AITunnel()
        {
        }

        public AITunnel(GraphicsDevice g, Matrix Projection)
            : base(g, Projection)
        {
            Init(g, Projection);
        }

        public AITunnel(BinaryReader BR, GraphicsDevice g, Matrix Projection)
            : base(BR, g, Projection)
        {
            Init(g, Projection);
        }

        public void Select()
        {
            ObjectEffect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.1f, 0.8f);
        }

        private void Init(GraphicsDevice g, Matrix Projection)
        {
            ListNextAITunnel = new List<AITunnel>();
            
            ObjectEffect.AmbientLightColor = Vector3.Zero;
            ObjectEffect.DirectionalLight0.Enabled = true;
            ObjectEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            ObjectEffect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);
            ObjectEffect.LightingEnabled = true;

            ArrayVertexBound = RacingMap.CreateTunnel();
            CreateVisibleDirection(g, Projection);
        }

        private void CreateVisibleDirection(GraphicsDevice g, Matrix Projection)
        {
            VertexPositionColor[] ArrayVertex = new VertexPositionColor[14];
            // Initialize an array of indices of type short.
            short[] ArrayIndices = new short[(ArrayVertex.Length * 2) - 2];
            
            int Index = 0;
            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(0.5f, 0, 1), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(0.5f, 0, -0.5f), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(0.5f, 0, -0.5f), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(1f, 0, -0.5f), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            //Top of arrow
            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(1f, 0, -0.5f), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(0, 0, -1f), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(0, 0, -1f), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(-1f, 0, -0.5f), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(-1f, 0, -0.5f), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(-0.5f, 0, -0.5f), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(-0.5f, 0, -0.5f), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(-0.5f, 0, 1), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(-0.5f, 0, 1), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(0.5f, 0, 1), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            VisibleDirection = new Lines3D(g, Projection, ArrayVertex, ArrayIndices);
        }

        public static AITunnel Load(BinaryReader BR, GraphicsDevice g, Matrix Projection)
        {
            AITunnel NewAITunnel = new AITunnel(BR, g, Projection);

            return NewAITunnel;
        }
        
        public void GetEntryPoints(Vector3 Axis, out float Min, out float Max)
        {
            Min = Vector3.Dot(Axis, CollisionBox.ArrayVertex[1]);
            Max = Min;
            float DotProduct = Vector3.Dot(Axis, CollisionBox.ArrayVertex[3]);
            if (DotProduct < Min)
            {
                Min = DotProduct;
            }
            else if (DotProduct > Max)
            {
                Max = DotProduct;
            }
        }

        public void GetExitPoints(Vector3 Axis, out float Min, out float Max)
        {
            Min = Vector3.Dot(Axis, CollisionBox.ArrayVertex[0]);
            Max = Min;
            float DotProduct = Vector3.Dot(Axis, CollisionBox.ArrayVertex[2]);
            if (DotProduct < Min)
            {
                Min = DotProduct;
            }
            else if (DotProduct > Max)
            {
                Max = DotProduct;
            }
        }

        public float GetDistanceEntryPoint()
        {
            return Vector3.Dot(Forward, CollisionBox.ArrayVertex[1]);
        }

        public float GetDistanceExitPoint()
        {
            return Vector3.Dot(Forward, CollisionBox.ArrayVertex[2]);
        }

        public override void Draw(CustomSpriteBatch g, Matrix View)
        {
            ObjectEffect.View = View;
            
            VisibleDirection.LinesEffect.World = ObjectEffect.World;

            ObjectEffect.CurrentTechnique.Passes[0].Apply();
            g.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, ArrayVertexBound, 0, 8);

            VisibleDirection.Draw(g, View);
        }
    }
}
