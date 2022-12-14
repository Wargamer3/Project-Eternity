using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.LightningSystem
{
    class PostProcessGlow
    {

        GraphicsDevice graphicsDevice;
        int width;
        int height;
        Effect glowEffect;
        RenderTarget2D downsampleRT;
        RenderTarget2D temporaryRT;
        RenderTarget2D combineRT;

        public PostProcessGlow(GraphicsDevice graphicsDevice, int width, int height, ContentManager contentManager)
        {
            this.graphicsDevice = graphicsDevice;
            this.width = width;
            this.height = height;
            PresentationParameters pp = graphicsDevice.PresentationParameters;

            downsampleRT = new RenderTarget2D(graphicsDevice, width/4, height/4, false, SurfaceFormat.Color, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            temporaryRT = new RenderTarget2D(graphicsDevice, width / 4, height / 4, false, SurfaceFormat.Color, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            combineRT = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            glowEffect = contentManager.Load<Effect>("Shaders/LightningGlow");
        }

        private void DoTechnique(CustomSpriteBatch g, string technique, RenderTarget2D sourceRT, RenderTarget2D targetRT, int width, int height)
        {
            graphicsDevice.SetRenderTarget(targetRT);
            graphicsDevice.Clear(Color.Transparent);

            glowEffect.CurrentTechnique = glowEffect.Techniques[technique];
            glowEffect.Parameters["texelSize"].SetValue(new Vector2(1.0f / width, 1.0f / height));

            g.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            glowEffect.CurrentTechnique.Passes[0].Apply();

            g.Draw(sourceRT,
                                new Rectangle(0,0,width,height),
                                Color.White);
            g.End();
            //resolve
            graphicsDevice.SetRenderTarget(null);

        }
        public void ApplyEffect(CustomSpriteBatch g, RenderTarget2D sourceRT, RenderTarget2D targetRT, float glowStrength)
        {
            //first downsample
            //downsample by rendering to smaller RT
            DoTechnique(g, "Copy", sourceRT, downsampleRT, downsampleRT.Width, downsampleRT.Height);
            DoTechnique(g, "BlurHorizontal", downsampleRT, temporaryRT, downsampleRT.Width, downsampleRT.Height);
            DoTechnique(g, "BlurVertical", temporaryRT, downsampleRT, downsampleRT.Width, downsampleRT.Height);

            glowEffect.Parameters["glowMap"].SetValue(downsampleRT);
            glowEffect.Parameters["glowStrength"].SetValue(glowStrength);
            DoTechnique(g, "Combine", sourceRT, combineRT, width, height);
            DoTechnique(g, "Copy", combineRT, targetRT, width, height);
        }

        
    }
}
