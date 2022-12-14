using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class TileViewer3DControl : GraphicsDeviceControl
    {
        public ContentManager content;
        private SpriteBatch g;
        public OrbitingCamera Camera;
        protected BasicEffect PolygonEffect;
        public List<Texture2D> ListTileSet;
        public List<Tile3D> ListTile3D;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;

            content = new ContentManager(Services, "Content");

            g = new SpriteBatch(GraphicsDevice);
            Camera = new OrbitingCamera(GraphicsDevice);

            PolygonEffect = new BasicEffect(GraphicsDevice);

            PolygonEffect.TextureEnabled = true;
            PolygonEffect.EnableDefaultLighting();

            float aspectRatio = GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);
            PolygonEffect.Projection = Projection;

            PolygonEffect.World = Matrix.Identity;
            PolygonEffect.View = Matrix.Identity;
        }

        public void Preload()
        {
            OnCreateControl();
        }

        protected override void Draw()
        {
            Camera.Update(null);

            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            PolygonEffect.View = Camera.View;
            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            foreach (Tile3D ActiveTile in ListTile3D)
            {
                PolygonEffect.Texture = ListTileSet[ActiveTile.TilesetIndex];
                PolygonEffect.CurrentTechnique.Passes[0].Apply();

                ActiveTile.Draw(g.GraphicsDevice);
            }

            g.End();
        }
    }
}