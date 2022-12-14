using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.LightningSystem
{
    public class LightningBolt
    {
        public RenderTarget2D LightningTexture;

        private LightningVertex[] ArrayVertex;
        private int[] ArrayIndex;
        private DynamicVertexBuffer VertexBuffer;
        private IndexBuffer IndexBuffer;

        private BlendState LightningBlendState = new BlendState()
        {
            AlphaBlendFunction = BlendFunction.Max,
            AlphaSourceBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,

            ColorBlendFunction = BlendFunction.Max,
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,

            BlendFactor = Color.White,
            ColorWriteChannels = ColorWriteChannels.All
        };

        public Vector3 StartPosition;
        public Vector3 Destination;

        int MaxParticles = 4000;
        int UsableParticles = 0;

        private Effect lightningDrawEffect;
        private PostProcessGlow postProcessGlow;

        private Random rand = new Random();
        public LightningDescriptor Descriptor;
        public List<LightingSection> ListLightningSection;
        double millisecondsSinceLastAnimation=0;

        public float ForkArmLength
        {
            get { return Descriptor.ForkLengthPercentage * Vector3.Distance(StartPosition, Destination); }
        }

        public LightningBolt()
        {
        }
        public LightningBolt(Vector3 StartPosition, Vector3 Destination)
        {
            this.StartPosition = StartPosition;
            this.Destination = Destination;
        }

        public void Init(ContentManager Content, GraphicsDevice GraphicsDevice, LightningDescriptor Descriptor)
        {
            this.Descriptor = Descriptor;
            this.ListLightningSection = this.Descriptor.ListTopology;

            ArrayVertex = new LightningVertex[MaxParticles * 4];
            for (int i = 0; i < MaxParticles; i++)
            {
                ArrayVertex[i * 4] = new LightningVertex(new Vector2(0, 0));
                ArrayVertex[i * 4 + 1] = new LightningVertex(new Vector2(1, 0));
                ArrayVertex[i * 4 + 2] = new LightningVertex(new Vector2(1, 1));
                ArrayVertex[i * 4 + 3] = new LightningVertex(new Vector2(0, 1));
            }

            VertexBuffer = new DynamicVertexBuffer(GraphicsDevice, LightningVertex.VertexDeclaration,
                                                   MaxParticles * 4, BufferUsage.WriteOnly);

            ArrayIndex = new int[MaxParticles * 6];

            for (int i = 0; i < MaxParticles; i++)
            {
                ArrayIndex[i * 6 + 0] = (ushort)(i * 4 + 0);
                ArrayIndex[i * 6 + 1] = (ushort)(i * 4 + 1);
                ArrayIndex[i * 6 + 2] = (ushort)(i * 4 + 2);

                ArrayIndex[i * 6 + 3] = (ushort)(i * 4 + 0);
                ArrayIndex[i * 6 + 4] = (ushort)(i * 4 + 2);
                ArrayIndex[i * 6 + 5] = (ushort)(i * 4 + 3);
            }

            IndexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), ArrayIndex.Length, BufferUsage.WriteOnly);

            IndexBuffer.SetData(ArrayIndex);

            lightningDrawEffect = Content.Load<Effect>("Shaders/LightningDraw").Clone();
            lightningDrawEffect.Parameters["StartColor"].SetValue(this.Descriptor.InteriorColor.ToVector3());
            lightningDrawEffect.Parameters["EndColor"].SetValue(this.Descriptor.ExteriorColor.ToVector3());


            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            LightningTexture = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, SurfaceFormat.Color, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            postProcessGlow = new PostProcessGlow(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, Content);
        }

        public void CreateNewLightningBolt()
        {
            UsableParticles = 0;
            ListLightningSection[0].CreateSection(StartPosition, Destination, 1);
        }

        public void CreateNewLightningBolt(Vector3 StartPosition, Vector3 Destination)
        {
            this.StartPosition = StartPosition;
            this.Destination = Destination;
            UsableParticles = 0;
            SetPointPosition(StartPosition, 1);
            SetPointPosition(Destination, 1);
            ListLightningSection[0].CreateSection(StartPosition, Destination, 1);
        }

        private float ComputeWidth(int WidthLevel)
        {
            if (Descriptor.IsWidthDecreasing)
                return Descriptor.BaseWidth / WidthLevel;
            else
                return Descriptor.BaseWidth;
        }

        public void SetLinePosition(Vector3 StartPosition, Vector3 Destination, Vector3 LeftVector, int WidthLevel)
        {
            float Width = ComputeWidth(WidthLevel);

            ArrayVertex[UsableParticles * 4].Position = StartPosition + LeftVector * Width;
            ArrayVertex[UsableParticles * 4 + 1].Position = Destination + LeftVector * Width;
            ArrayVertex[UsableParticles * 4 + 2].Position = Destination - LeftVector * Width;
            ArrayVertex[UsableParticles * 4 + 3].Position = StartPosition - LeftVector * Width;

            ArrayVertex[UsableParticles * 4].ColorGradient = new Vector2(1, 0);
            ArrayVertex[UsableParticles * 4 + 1].ColorGradient = new Vector2(1, 0);
            ArrayVertex[UsableParticles * 4 + 2].ColorGradient = new Vector2(-1, 0);
            ArrayVertex[UsableParticles * 4 + 3].ColorGradient = new Vector2(-1, 0);

            UsableParticles++;
        }

        public void SetPointPosition(Vector3 position, int WidthLevel)
        {
            float Width = ComputeWidth(WidthLevel);

            ArrayVertex[UsableParticles * 4].Position = position + Width * new Vector3(-1, -1, 0);
            ArrayVertex[UsableParticles * 4 + 1].Position = position + Width * new Vector3(1, -1, 0);
            ArrayVertex[UsableParticles * 4 + 2].Position = position + Width * new Vector3(1, 1, 0);
            ArrayVertex[UsableParticles * 4 + 3].Position = position + Width * new Vector3(-1, 1, 0);

            ArrayVertex[UsableParticles * 4].ColorGradient = new Vector2(-1, 1);
            ArrayVertex[UsableParticles * 4 + 1].ColorGradient = new Vector2(1, 1);
            ArrayVertex[UsableParticles * 4 + 2].ColorGradient = new Vector2(1, -1);
            ArrayVertex[UsableParticles * 4 + 3].ColorGradient = new Vector2(-1, -1);

            UsableParticles++;
        }

        public void SetWorldViewProjectionMatrix(Matrix World, Matrix View, Matrix Projection)
        {
            lightningDrawEffect.Parameters["World"].SetValue(World);
            lightningDrawEffect.Parameters["View"].SetValue(View);
            lightningDrawEffect.Parameters["Projection"].SetValue(Projection);
        }

        public void Update(GameTime gameTime)
        {
            if (Descriptor.AnimationFramerate >= 0f)
            {
                millisecondsSinceLastAnimation += gameTime.ElapsedGameTime.TotalMilliseconds;
                float frameLength = 1000.0f / Descriptor.AnimationFramerate;
                if (millisecondsSinceLastAnimation > frameLength)
                {
                    millisecondsSinceLastAnimation -= frameLength;
                    UsableParticles = 0;
                    ListLightningSection[0].CreateSection(StartPosition, Destination, 1);
                }
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            GraphicsDevice device = g.GraphicsDevice;

            device.SetRenderTarget(LightningTexture);
            device.Clear(ClearOptions.Target, Color.Transparent, 1.0f, 0);

            device.BlendState = LightningBlendState;
            device.RasterizerState = RasterizerState.CullNone;

            if (VertexBuffer.IsContentLost)
            {
                VertexBuffer.SetData(ArrayVertex);
            }
            VertexBuffer.SetData(ArrayVertex);
            device.SetVertexBuffer(VertexBuffer);
            device.Indices = IndexBuffer;

            lightningDrawEffect.CurrentTechnique.Passes[0].Apply();
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                0, UsableParticles * 4,
                0, UsableParticles * 2);

            device.SetRenderTarget(null);

            if (Descriptor.IsGlowEnabled)
            {
                postProcessGlow.ApplyEffect(g, LightningTexture, LightningTexture, Descriptor.GlowIntensity);
            }
        }

        public void Draw(CustomSpriteBatch g)
        {
            g.Draw(LightningTexture, Vector2.Zero, Color.White);
        }

        public void Draw(CustomSpriteBatch g, int Alpha)
        {
            g.Draw(LightningTexture, Vector2.Zero, Color.FromNonPremultiplied(255, 255, 255, Alpha));
        }

        public void EndDraw(SpriteBatch g)
        {
        }
    }
}
