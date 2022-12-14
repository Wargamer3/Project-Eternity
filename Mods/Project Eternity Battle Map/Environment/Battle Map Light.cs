using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapLight
    {
        /*Type:Select whether this light will stay on indefinitely(Fixed), flicker 
wildly between two colors(Flicker), Switch from one color to another for a few 
seconds(Switch) or transition smoothly from one color to the next(Pulse).*/
        public enum LightTypes { Fixed, Flicker, Switch, Pulse }

        public LightTypes LightType;

        public Vector3 Position;
        public Color PrimaryColor;
        public Color SecondaryColor;
        public float Speed;//Select the speed of the light transitions.
        public float Phase;//Select the cycle speed of the transitions.

        public float Angle;
        public float Size;
        public AIContainer AI;

        public bool CastShadows;
        public RenderTarget2D RenderTarget;

        public BattleMapLight(GraphicsDevice graphicsDevice, ShadowmapSize size, Vector3 Position, Color PrimaryColor)
        {
            this.Position = Position;
            this.PrimaryColor = PrimaryColor;

            int baseSize = 2 << (int)size;
            Size = baseSize;
            RenderTarget = new RenderTarget2D(graphicsDevice, baseSize, baseSize);
        }

        public Vector2 ToRelativePosition(Vector2 worldPosition)
        {
            return worldPosition - new Vector2(Position.X - Size, Position.Y - Size) * 0.5f;
        }

        public void Update(GameTime gameTime)
        {
            if (AI != null)
            {
                AI.UpdateStep(gameTime);
            }
        }

        public void BeginDrawingShadowCasters(CustomSpriteBatch g)
        {
            g.GraphicsDevice.SetRenderTarget(RenderTarget);
            g.GraphicsDevice.Clear(Color.Transparent);
        }

        public void EndDrawingShadowCasters(CustomSpriteBatch g)
        {
            g.GraphicsDevice.SetRenderTarget(null);
        }

        public void Draw(CustomSpriteBatch g)
        {

        }
    }
    public enum ShadowmapSize
    {
        Size128 = 6,
        Size256 = 7,
        Size512 = 8,
        Size1024 = 9,
    }

    public class ShadowmapResolver
    {
        private GraphicsDevice graphicsDevice;

        private int reductionChainCount;
        private int baseSize;
        private int depthBufferSize;

        Effect resolveShadowsEffect;
        Effect reductionEffect;

        public RenderTarget2D distortRT;
        public RenderTarget2D shadowMap;
        public RenderTarget2D shadowsRT;
        public RenderTarget2D processedShadowsRT;

        public QuadRenderComponent quadRender;
        public RenderTarget2D distancesRT;
        public RenderTarget2D[] reductionRT;


        /// <summary>
        /// Creates a new shadowmap resolver
        /// </summary>
        /// <param name="graphicsDevice">The Graphics Device used by the XNA game</param>
        /// <param name="quadRender"></param>
        /// <param name="baseSize">The size of the light regions </param>
        public ShadowmapResolver(GraphicsDevice graphicsDevice, QuadRenderComponent quadRender, ShadowmapSize maxShadowmapSize, ShadowmapSize maxDepthBufferSize)
        {
            this.graphicsDevice = graphicsDevice;
            this.quadRender = quadRender;

            reductionChainCount = (int)maxShadowmapSize;
            baseSize = 2 << reductionChainCount;
            depthBufferSize = 2 << (int)maxDepthBufferSize;
        }

        public void LoadContent(ContentManager content)
        {
            reductionEffect = content.Load<Effect>("Shaders/reductionEffect");
            resolveShadowsEffect = content.Load<Effect>("Shaders/resolveShadowsEffect");

            distortRT = new RenderTarget2D(graphicsDevice, baseSize, baseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            distancesRT = new RenderTarget2D(graphicsDevice, baseSize, baseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            shadowMap = new RenderTarget2D(graphicsDevice, 2, baseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            reductionRT = new RenderTarget2D[reductionChainCount];
            for (int i = 0; i < reductionChainCount; i++)
            {
                reductionRT[i] = new RenderTarget2D(graphicsDevice, 2 << i, baseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            }


            shadowsRT = new RenderTarget2D(graphicsDevice, baseSize, baseSize);
            processedShadowsRT = new RenderTarget2D(graphicsDevice, baseSize, baseSize);
        }

        public void ResolveShadows(Texture2D shadowCastersTexture, RenderTarget2D result)
        {
            graphicsDevice.BlendState = BlendState.Opaque;

            ExecuteTechnique(shadowCastersTexture, distancesRT, "ComputeDistances");
            ExecuteTechnique(distancesRT, distortRT, "Distort");
            ApplyHorizontalReduction(distortRT, shadowMap);
            ExecuteTechnique(null, shadowsRT, "DrawShadows", shadowMap);
            ExecuteTechnique(shadowsRT, processedShadowsRT, "BlurHorizontally");
            ExecuteTechnique(processedShadowsRT, result, "BlurVerticallyAndAttenuate");
        }

        private void ExecuteTechnique(Texture2D source, RenderTarget2D destination, string techniqueName)
        {
            ExecuteTechnique(source, destination, techniqueName, null);
        }

        private void ExecuteTechnique(Texture2D source, RenderTarget2D destination, string techniqueName, Texture2D shadowMap)
        {
            Vector2 renderTargetSize;
            renderTargetSize = new Vector2((float)baseSize, (float)baseSize);
            graphicsDevice.SetRenderTarget(destination);
            graphicsDevice.Clear(Color.White);
            resolveShadowsEffect.Parameters["renderTargetSize"].SetValue(renderTargetSize);

            if (source != null)
                resolveShadowsEffect.Parameters["InputTexture"].SetValue(source);
            if (shadowMap != null)
                resolveShadowsEffect.Parameters["ShadowMapTexture"].SetValue(shadowMap);

            resolveShadowsEffect.CurrentTechnique = resolveShadowsEffect.Techniques[techniqueName];

            foreach (EffectPass pass in resolveShadowsEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                quadRender.Render(Vector2.One * -1, Vector2.One);
            }
            graphicsDevice.SetRenderTarget(null);
        }


        private void ApplyHorizontalReduction(RenderTarget2D source, RenderTarget2D destination)
        {
            int step = reductionChainCount - 1;
            RenderTarget2D s = source;
            RenderTarget2D d = reductionRT[step];
            reductionEffect.CurrentTechnique = reductionEffect.Techniques["HorizontalReduction"];

            while (step >= 0)
            {
                d = reductionRT[step];

                graphicsDevice.SetRenderTarget(d);
                graphicsDevice.Clear(Color.White);

                reductionEffect.Parameters["SourceTexture"].SetValue(s);
                Vector2 textureDim = new Vector2(1.0f / (float)s.Width, 1.0f / (float)s.Height);
                reductionEffect.Parameters["TextureDimensions"].SetValue(textureDim);

                foreach (EffectPass pass in reductionEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    quadRender.Render(Vector2.One * -1, new Vector2(1, 1));
                }

                graphicsDevice.SetRenderTarget(null);

                s = d;
                step--;
            }

            //copy to destination
            graphicsDevice.SetRenderTarget(destination);
            reductionEffect.CurrentTechnique = reductionEffect.Techniques["Copy"];
            reductionEffect.Parameters["SourceTexture"].SetValue(d);

            foreach (EffectPass pass in reductionEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                quadRender.Render(Vector2.One * -1, new Vector2(1, 1));
            }

            reductionEffect.Parameters["SourceTexture"].SetValue(reductionRT[reductionChainCount - 1]);
        }


    }
    public partial class QuadRenderComponent
    {
        VertexPositionTexture[] verts = null;
        short[] ib = null;
        GraphicsDevice GraphicsDevice;
        // Constructor
        public QuadRenderComponent(GraphicsDevice GraphicsDevice)
        {
            this.GraphicsDevice = GraphicsDevice;
        }

        public void LoadContent()
        {
            verts = new VertexPositionTexture[]
            {
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,0)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,0))
            };

            ib = new short[] { 0, 1, 2, 2, 3, 0 };
        }

        public void Render(Vector2 v1, Vector2 v2)
        {
            verts[0].Position.X = v2.X;
            verts[0].Position.Y = v1.Y;

            verts[1].Position.X = v1.X;
            verts[1].Position.Y = v1.Y;

            verts[2].Position.X = v1.X;
            verts[2].Position.Y = v2.Y;

            verts[3].Position.X = v2.X;
            verts[3].Position.Y = v2.Y;
            GameScreen.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GameScreen.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GameScreen.GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Immediate;
            GameScreen.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, verts, 0, 4, ib, 0, 2);
        }
    }
}
