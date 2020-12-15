using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using System.IO;
using System.Text;

namespace ProjectEternity.GameScreens.RacingScreen
{
    public class RacingMap : GameScreen
    {
        public static Vehicule ActiveVehicule;
        public static Vector3 Gravity = new Vector3(0f, 300f, 0f);
        private List<Vehicule> ListVehicule;
        private List<AITunnel> ListAITunnel;
        private List<PolygonMesh> ListObstacle;
        private List<Object3D> ListCollisionBox;
        public Camera3D Camera;
        private string MapPath;
        private SpriteFont fntFinlanderFont;

        public RacingMap()
            : this("New Item")
        {
        }

        public RacingMap(string MapPath)
        {
            this.MapPath = MapPath;

            ListVehicule = new List<Vehicule>();
            ListAITunnel = new List<AITunnel>();
            ListObstacle = new List<PolygonMesh>();
            ListCollisionBox = new List<Object3D>();
        }

        public override void Load()
        {
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            ListVehicule.Add(new Vehicule2D(ListAITunnel, ListCollisionBox, GraphicsDevice, new Vector3(5, 0, 0)));
            ListVehicule[0].ListActionMenuChoice.Add(new ActionPanelAIFirstAction(ListVehicule[0]));
            //ListVehicule[0].ListActionMenuChoice.Add(new ActionPanelPlayerAction(ListVehicule[0]));
            ListVehicule[0].Load(Content, GraphicsDevice);

            ListVehicule.Add(new Vehicule2D(ListAITunnel, ListCollisionBox, GraphicsDevice, new Vector3(0, 0, 0)));
            ListVehicule.Add(new Vehicule2D(ListAITunnel, ListCollisionBox, GraphicsDevice, new Vector3(-5, 0, 5)));
            ListVehicule.Add(new Vehicule2D(ListAITunnel, ListCollisionBox, GraphicsDevice, new Vector3(10, 0, 10)));
            ListVehicule.Add(new Vehicule2D(ListAITunnel, ListCollisionBox, GraphicsDevice, new Vector3(-10, 0, 16)));
            ListVehicule.Add(new Vehicule2D(ListAITunnel, ListCollisionBox, GraphicsDevice, new Vector3(0, 0, 20)));
            ListVehicule.Add(new Vehicule2D(ListAITunnel, ListCollisionBox, GraphicsDevice, new Vector3(-5, 0, 35)));
            ListVehicule.Add(new Vehicule2D(ListAITunnel, ListCollisionBox, GraphicsDevice, new Vector3(10, 0, 30)));
            ListVehicule.Add(new Vehicule2D(ListAITunnel, ListCollisionBox, GraphicsDevice, new Vector3(-10, 0, 46)));
            for (int V = 1; V < ListVehicule.Count; ++V)
            {
                ListVehicule[V].ListActionMenuChoice.Add(new ActionPanelAIFirstAction(ListVehicule[V]));
                ListVehicule[V].Load(Content, GraphicsDevice);
            }

            ActiveVehicule = ListVehicule[5];
            Camera = new ChaseCamera(GraphicsDevice, ActiveVehicule);

            if (!string.IsNullOrEmpty(MapPath))
            {
                FileStream FS = new FileStream("Content/Maps/Racing/" + MapPath + ".pem", FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
                BR.BaseStream.Seek(0, SeekOrigin.Begin);

                int ListAITunnelCount = BR.ReadInt32();
                for (int T = 0; T < ListAITunnelCount; ++T)
                {
                    ListAITunnel.Add(AITunnel.Load(BR, GraphicsDevice, Camera.Projection));
                }

                int ListCollisionBoxCount = BR.ReadInt32();
                for (int T = 0; T < ListCollisionBoxCount; ++T)
                {
                    ListCollisionBox.Add(CollisionBox.Load(BR, GraphicsDevice, Camera.Projection));
                }

                BR.Close();
                FS.Close();

                LinkAITunnelTogether();
            }
        }

        public void Save()
        {
            FileStream FS = new FileStream("Content/Maps/Racing/" + MapPath + ".pem", FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(ListAITunnel.Count);
            for (int T = 0; T < ListAITunnel.Count; ++T)
            {
                ListAITunnel[T].Save(BW);
            }

            BW.Write(ListCollisionBox.Count);
            for (int C = 0; C < ListCollisionBox.Count; ++C)
            {
                ListCollisionBox[C].Save(BW);
            }

            BW.Flush();
            BW.Close();
            FS.Close();
        }
        
        private void LinkAITunnelTogether()
        {
            foreach (AITunnel ActiveTunnel in ListAITunnel)
            {
                for (int T = ListAITunnel.Count - 1; T >= 0; --T)
                {
                    if (ListAITunnel[T] == ActiveTunnel)
                        continue;

                    float CurrentPosition = Vector3.Dot(ListAITunnel[T].Forward, ActiveTunnel.Position);
                    float OtherPosition = Vector3.Dot(ListAITunnel[T].Forward, ListAITunnel[T].Position);

                    if (CurrentPosition < OtherPosition && PolygonMesh.PolygonCollisionSAT(ActiveTunnel.CollisionBox, ListAITunnel[T].CollisionBox, Vector3.Zero).Collided)
                    {
                        ActiveTunnel.ListNextAITunnel.Add(ListAITunnel[T]);
                    }
                }
            }
        }

        public static VertexPositionNormalTexture[] CreateCube()
        {
            Vector2 Texcoords = new Vector2(0f, 0f);
            Vector3[] face = new Vector3[4];

            //FrontTopLeft
            face[0] = new Vector3(1f, 1f, 0.0f);
            //FrontBottomLeft
            face[1] = new Vector3(-1f, -1f, 0.0f);
            //FrontTopRight
            face[2] = new Vector3(-1f, 1f, 0.0f);
            //FrontBottomRight
            face[3] = new Vector3(-1f, -1f, 0.0f);

            Matrix RotZ180 = Matrix.CreateRotationZ(
                               -(float)Math.PI);

            VertexPositionNormalTexture[] vertexes = new VertexPositionNormalTexture[36];
            
            //front face
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i] =
                  new VertexPositionNormalTexture(
                       face[i] + Vector3.UnitZ,
                             Vector3.UnitZ, Texcoords);

                vertexes[i + 3] =
                  new VertexPositionNormalTexture(
                       Vector3.Transform(face[i], RotZ180) + Vector3.UnitZ,
                             Vector3.UnitZ, Texcoords);
            }

            Matrix RotY180 = Matrix.CreateRotationY(
                               -(float)Math.PI);

            //Back face
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 6] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(vertexes[i].Position, RotY180),
                            -Vector3.UnitZ, Texcoords);

                vertexes[i + 6 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(vertexes[i + 3].Position, RotY180),
                            -Vector3.UnitZ, Texcoords);
            }
            //left face
            Matrix RotY90 = Matrix.CreateRotationY(
                               -(float)Math.PI / 2f);

            Matrix RotY270 = Matrix.CreateRotationY(
                               (float)Math.PI / 2f);

            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 12] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(vertexes[i].Position, RotY90),
                            -Vector3.UnitX, Texcoords);

                vertexes[i + 12 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(vertexes[i + 3].Position, RotY90),
                            -Vector3.UnitX, Texcoords);
            }

            //Right face
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 18] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(vertexes[i].Position, RotY270),
                            Vector3.UnitX, Texcoords);

                vertexes[i + 18 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(vertexes[i + 3].Position, RotY270),
                            Vector3.UnitX, Texcoords);
            }

            //Top face
            Matrix RotX90 = Matrix.CreateRotationX(
                                -(float)Math.PI / 2f);
            Matrix RotX270 = Matrix.CreateRotationX(
                                (float)Math.PI / 2f);

            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 24] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(vertexes[i].Position, RotX90),
                            -Vector3.UnitY, Texcoords);

                vertexes[i + 24 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(vertexes[i + 3].Position, RotX90),
                            -Vector3.UnitY, Texcoords);
            }

            //Bottom face
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 30] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(vertexes[i].Position, RotX270),
                            Vector3.UnitY, Texcoords);

                vertexes[i + 30 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(vertexes[i + 3].Position, RotX270),
                            Vector3.UnitY, Texcoords);
            }

            return vertexes;
        }

        public static VertexPositionNormalTexture[] CreateTunnel()
        {
            Vector2 Texcoords = new Vector2(0f, 0f);
            Vector3[] face = new Vector3[4];

            //FrontTopLeft
            face[0] = new Vector3(1f, 1f, 0.0f);
            //FrontBottomLeft
            face[1] = new Vector3(-1f, -1f, 0.0f);
            //FrontTopRight
            face[2] = new Vector3(-1f, 1f, 0.0f);
            //FrontBottomRight
            face[3] = new Vector3(-1f, -1f, 0.0f);

            Matrix RotZ180 = Matrix.CreateRotationZ(
                               -(float)Math.PI);

            VertexPositionNormalTexture[] vertexes = new VertexPositionNormalTexture[24];
            VertexPositionNormalTexture[] FrontFace = new VertexPositionNormalTexture[6];

            //front face
            for (int i = 0; i <= 2; i++)
            {
                FrontFace[i] =
                  new VertexPositionNormalTexture(
                       face[i] + Vector3.UnitZ,
                             Vector3.UnitZ, Texcoords);

                FrontFace[i + 3] =
                  new VertexPositionNormalTexture(
                       Vector3.Transform(face[i], RotZ180) + Vector3.UnitZ,
                             Vector3.UnitZ, Texcoords);
            }
            
            //left face
            Matrix RotY90 = Matrix.CreateRotationY(
                               -(float)Math.PI / 2f);

            Matrix RotY270 = Matrix.CreateRotationY(
                               (float)Math.PI / 2f);

            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 0] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i].Position, RotY90),
                            -Vector3.UnitX, Texcoords);

                vertexes[i + 0 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i + 3].Position, RotY90),
                            -Vector3.UnitX, Texcoords);
            }

            //Right face
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 6] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i].Position, RotY270),
                            Vector3.UnitX, Texcoords);

                vertexes[i + 6 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i + 3].Position, RotY270),
                            Vector3.UnitX, Texcoords);
            }

            //Top face
            Matrix RotX90 = Matrix.CreateRotationX(
                                -(float)Math.PI / 2f);
            Matrix RotX270 = Matrix.CreateRotationX(
                                (float)Math.PI / 2f);

            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 12] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i].Position, RotX90),
                            -Vector3.UnitY, Texcoords);

                vertexes[i + 12 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i + 3].Position, RotX90),
                            -Vector3.UnitY, Texcoords);
            }

            //Bottom face
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 18] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i].Position, RotX270),
                            Vector3.UnitY, Texcoords);

                vertexes[i + 18 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i + 3].Position, RotX270),
                            Vector3.UnitY, Texcoords);
            }

            return vertexes;
        }

        public AITunnel GetAITunnel(int Index)
        {
            return ListAITunnel[Index];
        }

        public AITunnel GetAITunnelUnderMouse(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection)
        {
            foreach (AITunnel ActiveTunnel in ListAITunnel)
            {
                bool ActiveTunnelSelected = ActiveTunnel.CollideWithMouse(MouseX, MouseY, ActiveViewport, View, Projection, Matrix.Identity);
                if (ActiveTunnelSelected)
                    return ActiveTunnel;
            }

            return null;
        }

        public int GetAITunnelCount()
        {
            return ListAITunnel.Count;
        }

        public Object3D GetCollisionBox(int Index)
        {
            return ListCollisionBox[Index];
        }

        public CollisionBox GetCollisionBoxUnderMouse(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection)
        {
            foreach (CollisionBox ActiveCollisionBox in ListCollisionBox)
            {
                bool ActiveCollisionBoxSelected = ActiveCollisionBox.CollideWithMouse(MouseX, MouseY, ActiveViewport, View, Projection, Matrix.Identity);
                if (ActiveCollisionBoxSelected)
                    return ActiveCollisionBox;
            }

            return null;
        }

        public int GetCollisionBoxCount()
        {
            return ListCollisionBox.Count;
        }

        public AITunnel CreateAITunnel()
        {
            AITunnel NewAITunnel = new AITunnel(GraphicsDevice, Camera.Projection);
            ListAITunnel.Add(NewAITunnel);

            return NewAITunnel;
        }

        public CollisionBox CreateCollisionBox()
        {
            CollisionBox NewCollisionBox = new CollisionBox(GraphicsDevice, Camera.Projection);
            ListCollisionBox.Add(NewCollisionBox);

            return NewCollisionBox;
        }

        public void RemoveAITunnel(int Index)
        {
            ListAITunnel.RemoveAt(Index);
        }

        public override void Update(GameTime gameTime)
        {
            Camera.Update(gameTime);

            for (int V = ListVehicule.Count - 1; V >= 0; --V)
            {
                ListVehicule[V].Update(gameTime);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawMap(g);

            for (int V = ListVehicule.Count - 1; V >= 0; --V)
            {
                ListVehicule[V].Draw(g, Camera.View);
            }

            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            g.Draw(sprPixel, new Rectangle(Constants.Width - 50, Constants.Height - 100, 20, 80), Color.Black);
            g.Draw(sprPixel, new Rectangle(Constants.Width - 50, Constants.Height - 100, 20, (int)(80d * ActiveVehicule.BoostPower / ActiveVehicule.BrakePower)), Color.White);

            g.Draw(sprPixel, new Rectangle(30, Constants.Height - 50, 50, 30), Color.Black);
            g.DrawString(fntFinlanderFont, ((int)ActiveVehicule.Speed.Length()).ToString(), new Vector2(32, Constants.Height - 50), Color.White);
        }

        public void DrawMap(CustomSpriteBatch g)
        {
            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            for (int T = ListAITunnel.Count - 1; T >= 0; --T)
            {
                ListAITunnel[T].Draw(g, Camera.View);
            }

            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            for (int C = ListCollisionBox.Count - 1; C >= 0; --C)
            {
                ListCollisionBox[C].Draw(g, Camera.View);
            }
        }
    }
}
