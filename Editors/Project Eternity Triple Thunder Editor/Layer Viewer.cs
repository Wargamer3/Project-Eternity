using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.Editors.AnimationEditor
{
    internal class LayerViewerControl : GraphicsDeviceControl
    {
        private Texture2D sprPixel;
        private Texture2D sprRedTexture;

        public ContentManager content;
        private CustomSpriteBatch g;
        public FightingZone ActiveFightingZone;

        private BasicEffect PolygonEffect;

        public PolygonTriangle SelectedPolygonTriangle;
        public SimpleAnimation SelectedAnimation;
        public Prop SelectedProp;
        public SpawnPoint SelectedSpawn;
        public Layer SelectedLayer;
        public bool DisplayOtherLayers;
        public bool ShowScripts;
        private Vector3 CameraOldPosition = Vector3.Zero;
        public ContextMenuStrip cmsScriptMenu;
        private ToolStripMenuItem tsmDeleteScript;

        public MapScriptGUIHelper Helper;
        public Rectangle Camera;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;

            Camera = new Rectangle(0, 0, Width, Height);
            content = new ContentManager(Services, "Content");
            sprPixel = content.Load<Texture2D>("Pixel");
            SelectedPolygonTriangle = new PolygonTriangle();
            DisplayOtherLayers = true;

            sprRedTexture = content.Load<Texture2D>("Pixel");
            sprRedTexture.SetData<Color>(new Color[] { Color.FromNonPremultiplied(255, 0, 0, 120) });
            if (PolygonEffect == null)
            {
                PolygonEffect = new BasicEffect(GraphicsDevice);

                PolygonEffect.VertexColorEnabled = true;
                PolygonEffect.TextureEnabled = true;

                PolygonEffect.World = Matrix.Identity;
                PolygonEffect.View = Matrix.Identity;
                PolygonEffect.Texture = sprRedTexture;
            }

            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));

            Helper = new MapScriptGUIHelper();
            Helper.Load(content, g, GraphicsDevice);

            this.cmsScriptMenu = new ContextMenuStrip();
            this.tsmDeleteScript = new ToolStripMenuItem();
            // 
            // cmsScriptMenu
            // 
            this.cmsScriptMenu.Items.AddRange(new ToolStripItem[] {
            this.tsmDeleteScript});
            this.cmsScriptMenu.Name = "cmsScriptMenu";
            this.cmsScriptMenu.Size = new System.Drawing.Size(141, 26);

            // 
            // tsmDeleteScript
            //
            this.tsmDeleteScript.Name = "tsmDeleteScript";
            this.tsmDeleteScript.Size = new System.Drawing.Size(140, 22);
            this.tsmDeleteScript.Text = "Delete Script";
            this.tsmDeleteScript.Click += tsmDeleteScript_Click;
        }

        public void Preload()
        {
            OnCreateControl();
        }

        public void SetListMapScript(List<MapScript> ListMapScript)
        {
            Helper.SetListMapScript(ListMapScript);
        }

        public void CreateScript(MapScript OriginalToCopy)
        {
            Helper.CreateScript(OriginalToCopy);
        }

        private void tsmDeleteScript_Click(object sender, EventArgs e)
        {
            Helper.DeleteSelectedScript();
        }

        private void Update(GameTime gameTime)
        {
            Matrix Projection = Matrix.CreateOrthographicOffCenter(
                Camera.X,
                Camera.X + ClientSize.Width,
                Camera.Y + ClientSize.Height,
                Camera.Y,
                0,
                1);

            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            PolygonEffect.Projection = HalfPixelOffset * Projection;

            if (ActiveFightingZone.Background != null)
            {
                ActiveFightingZone.Background.MoveSpeed = new Vector3(Camera.X, Camera.Y, 0f) - CameraOldPosition;
                ActiveFightingZone.Background.Update(gameTime);
            }

            /*foreach (Layer ActiveLayer in ActiveFightingZone.ListLayer)
            {
                ActiveLayer.Update(gameTime);
            }*/

            CameraOldPosition = new Vector3(Camera.X, Camera.Y, 0f);
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            if (ShowScripts)
            {
                Helper.DrawScripts();
            }
            else
            {
                Update(new GameTime());

                Matrix TransformationMatrix = Matrix.CreateTranslation(-Camera.X, -Camera.Y, 0);

                g.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, TransformationMatrix);

                foreach (Layer ActiveLayer in ActiveFightingZone.ListLayer)
                {
                    foreach (Prop ActiveProp in ActiveLayer.ListProp)
                        ActiveProp.BeginDraw(g);
                }

                g.End();
                GraphicsDevice.SetRenderTarget(null);

                if (ActiveFightingZone.Background != null)
                {
                    ActiveFightingZone.Background.Draw(g, Width, Height);
                }

                g.Begin(SpriteSortMode.Deferred, null, null, null, null, null, TransformationMatrix);

                foreach (Layer ActiveLayer in ActiveFightingZone.ListLayer)
                {
                    if (!DisplayOtherLayers && SelectedLayer != ActiveLayer)
                    {
                        continue;
                    }

                    ActiveLayer.Draw(g);

                    foreach (Prop ActiveProp in ActiveLayer.ListProp)
                    {
                        g.Draw(sprPixel, new Rectangle((int)ActiveProp._Position.X + 13, (int)ActiveProp._Position.Y, 6, 20), Color.Black);
                        g.Draw(sprPixel, new Rectangle((int)ActiveProp._Position.X + 13, (int)ActiveProp._Position.Y + 24, 6, 6), Color.Black);
                    }

                    foreach (SpawnPoint ActiveSpawn in ActiveLayer.ListSpawnPointTeam)
                    {
                        g.Draw(sprPixel, new Rectangle((int)ActiveSpawn.SpawnLocation.X + 13, (int)ActiveSpawn.SpawnLocation.Y, 6, 20), Color.Black);
                        g.Draw(sprPixel, new Rectangle((int)ActiveSpawn.SpawnLocation.X + 13, (int)ActiveSpawn.SpawnLocation.Y + 24, 6, 6), Color.Black);
                    }

                    foreach (Polygon ActivePolygon in ActiveLayer.ListWorldCollisionPolygon)
                    {
                        PolygonTriangle.Draw(g, GraphicsDevice, sprPixel, new PolygonTriangle(PolygonTriangle.SelectionTypes.None, ActivePolygon, 0, 0));
                    }

                    if (SelectedAnimation != null)
                    {
                        Vector2 Position1 = SelectedAnimation.Position - SelectedAnimation.Origin;
                        Vector2 Position2 = Position1 + new Vector2(SelectedAnimation.PositionRectangle.Width, 0);
                        Vector2 Position3 = Position1 + new Vector2(0, SelectedAnimation.PositionRectangle.Height);
                        Vector2 Position4 = Position1 + new Vector2(SelectedAnimation.PositionRectangle.Width, SelectedAnimation.PositionRectangle.Height);

                        g.DrawLine(sprPixel, Position1, Position2, Color.Red);
                        g.DrawLine(sprPixel, Position1, Position3, Color.Red);
                        g.DrawLine(sprPixel, Position2, Position4, Color.Red);
                        g.DrawLine(sprPixel, Position3, Position4, Color.Red);
                    }

                    if (SelectedProp != null)
                    {
                        Vector2 Position1 = SelectedProp._Position;
                        Vector2 Position2 = Position1 + new Vector2(32, 0);
                        Vector2 Position3 = Position1 + new Vector2(0, 32);
                        Vector2 Position4 = Position1 + new Vector2(32, 32);

                        g.DrawLine(sprPixel, Position1, Position2, Color.Red);
                        g.DrawLine(sprPixel, Position1, Position3, Color.Red);
                        g.DrawLine(sprPixel, Position2, Position4, Color.Red);
                        g.DrawLine(sprPixel, Position3, Position4, Color.Red);
                    }

                    if (SelectedSpawn != null)
                    {
                        Vector2 Position1 = SelectedSpawn.SpawnLocation;
                        Vector2 Position2 = Position1 + new Vector2(32, 0);
                        Vector2 Position3 = Position1 + new Vector2(0, 32);
                        Vector2 Position4 = Position1 + new Vector2(32, 32);

                        g.DrawLine(sprPixel, Position1, Position2, Color.Red);
                        g.DrawLine(sprPixel, Position1, Position3, Color.Red);
                        g.DrawLine(sprPixel, Position2, Position4, Color.Red);
                        g.DrawLine(sprPixel, Position3, Position4, Color.Red);
                    }

                    //Draw selected polygons.
                    PolygonEffect.Texture = sprRedTexture;
                    PolygonEffect.CurrentTechnique.Passes[0].Apply();
                    GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                    Polygon ActiveGroundPolygon;

                    for (int V = 0; V < ActiveLayer.GroundLevelCollision.ArrayVertex.Length - 1; V++)
                    {
                        ActiveGroundPolygon = ActiveLayer.GroundLevelCollision;

                        g.Draw(sprPixel, new Rectangle((int)ActiveGroundPolygon.ArrayVertex[V].X - 2, (int)ActiveGroundPolygon.ArrayVertex[V].Y - 2, 5, 5), Color.Red);
                        g.DrawLine(sprPixel, ActiveGroundPolygon.ArrayVertex[V], ActiveGroundPolygon.ArrayVertex[V + 1], Color.Red);
                    }

                    ActiveGroundPolygon = ActiveLayer.GroundLevelCollision;
                    g.Draw(sprPixel, new Rectangle((int)ActiveGroundPolygon.ArrayVertex[ActiveGroundPolygon.ArrayVertex.Length - 1].X - 2,
                                                    (int)ActiveGroundPolygon.ArrayVertex[ActiveGroundPolygon.ArrayVertex.Length - 1].Y - 2, 5, 5), Color.Red);
                }

                foreach (Layer ActiveLayer in ActiveFightingZone.ListLayer)
                {
                    foreach (Prop ActiveProp in ActiveLayer.ListProp)
                        ActiveProp.EndDraw(g);
                }

                if (SelectedPolygonTriangle != null)
                {
                    g.End();
                    g.Begin(SpriteSortMode.Deferred, null, null, null, null, null, TransformationMatrix);

                    PolygonEffect.Texture = sprRedTexture;
                    PolygonEffect.CurrentTechnique.Passes[0].Apply();
                    GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                    SelectedPolygonTriangle.Draw(g, GraphicsDevice, sprPixel);
                    if (SelectedPolygonTriangle.ActivePolygon != null)
                    {
                        SelectedPolygonTriangle.ActivePolygon.Draw(GraphicsDevice);
                    }
                }

                g.DrawLine(sprPixel, new Vector2(ActiveFightingZone.CameraBounds.X, ActiveFightingZone.CameraBounds.Y), new Vector2(ActiveFightingZone.CameraBounds.Right, ActiveFightingZone.CameraBounds.Y), Color.Red);
                g.DrawLine(sprPixel, new Vector2(ActiveFightingZone.CameraBounds.Right, ActiveFightingZone.CameraBounds.Y), new Vector2(ActiveFightingZone.CameraBounds.Right, ActiveFightingZone.CameraBounds.Bottom), Color.Red);
                g.DrawLine(sprPixel, new Vector2(ActiveFightingZone.CameraBounds.X, ActiveFightingZone.CameraBounds.Y), new Vector2(ActiveFightingZone.CameraBounds.X, ActiveFightingZone.CameraBounds.Bottom), Color.Red);
                g.DrawLine(sprPixel, new Vector2(ActiveFightingZone.CameraBounds.X, ActiveFightingZone.CameraBounds.Bottom), new Vector2(ActiveFightingZone.CameraBounds.Right, ActiveFightingZone.CameraBounds.Bottom), Color.Red);

                Thread.Sleep(15);

                g.End();
            }
        }
    }
}
