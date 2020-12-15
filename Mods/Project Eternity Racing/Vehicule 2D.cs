using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.RacingScreen
{
    public class Vehicule2D : Vehicule
    {
        Dictionary<int, Texture2D> DicVehiculeSpritePerAngle;
        BillboardSystem VehiculeDrawablePart;
        float LastAngle = 0;

        Matrix Projection;

        public Vehicule2D(List<AITunnel> ListAITunnel, List<Object3D> ListCollisionBox, GraphicsDevice g, Vector3 Position)
            : base(g, Matrix.Identity, ListAITunnel, ListCollisionBox)
        {
            DicVehiculeSpritePerAngle = new Dictionary<int, Texture2D>();

            float aspectRatio = g.Viewport.Width / (float)g.Viewport.Height;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                     aspectRatio,
                                                                     1, 10000);
            TeleportRelative(Position);
            ArrayAntiGravPropulsor = new AntiGravPropulsor[1];
            ArrayAntiGravPropulsor[0] = CreateAntiGravPropulsor(g, Projection, 1f, Position);
            ArrayAntiGravPropulsor[0].Position += new Vector3(0f, 2f, 0f);
        }

        public override void Load(ContentManager Content, GraphicsDevice g)
        {
            VehiculeDrawablePart = new BillboardSystem("Units/Racing/Sprites/Back", 2, 1, BlendState.NonPremultiplied, true, Content, g);
            VehiculeDrawablePart.Parameters["Size"].SetValue(new Vector2(188 * 0.1f, 112 * 0.1f));
            VehiculeDrawablePart.AddParticle(Vector3.Zero);

            DicVehiculeSpritePerAngle.Add(0, Content.Load<Texture2D>("Units/Racing/Sprites/Back"));
            DicVehiculeSpritePerAngle.Add(5, Content.Load<Texture2D>("Units/Racing/Sprites/Back5"));
            DicVehiculeSpritePerAngle.Add(10, Content.Load<Texture2D>("Units/Racing/Sprites/Back10"));
            DicVehiculeSpritePerAngle.Add(25, Content.Load<Texture2D>("Units/Racing/Sprites/Back25"));
            DicVehiculeSpritePerAngle.Add(35, Content.Load<Texture2D>("Units/Racing/Sprites/Back35"));
            DicVehiculeSpritePerAngle.Add(45, Content.Load<Texture2D>("Units/Racing/Sprites/Back45"));
            DicVehiculeSpritePerAngle.Add(65, Content.Load<Texture2D>("Units/Racing/Sprites/Back45"));
            DicVehiculeSpritePerAngle.Add(90, Content.Load<Texture2D>("Units/Racing/Sprites/Back45"));

            DicVehiculeSpritePerAngle.Add(-5, FlipTexture2D(DicVehiculeSpritePerAngle[5]));
            DicVehiculeSpritePerAngle.Add(-10, FlipTexture2D(DicVehiculeSpritePerAngle[10]));
            DicVehiculeSpritePerAngle.Add(-25, FlipTexture2D(DicVehiculeSpritePerAngle[25]));
            DicVehiculeSpritePerAngle.Add(-35, FlipTexture2D(DicVehiculeSpritePerAngle[35]));
            DicVehiculeSpritePerAngle.Add(-45, FlipTexture2D(DicVehiculeSpritePerAngle[45]));
            DicVehiculeSpritePerAngle.Add(-65, FlipTexture2D(DicVehiculeSpritePerAngle[65]));
            DicVehiculeSpritePerAngle.Add(-90, FlipTexture2D(DicVehiculeSpritePerAngle[90]));
        }

        private Texture2D FlipTexture2D(Texture2D ActiveTexture)
        {
            Texture2D OutputTexture = new Texture2D(ActiveTexture.GraphicsDevice, ActiveTexture.Width, ActiveTexture.Height);
            Color[] ActiveTextureData = new Color[ActiveTexture.Width * ActiveTexture.Height];
            Color[] OutputTextureData = new Color[ActiveTextureData.Length];

            ActiveTexture.GetData(ActiveTextureData);

            for (int X = ActiveTexture.Width - 1; X >= 0; --X)
            {
                for (int Y = ActiveTexture.Height - 1; Y >= 0; --Y)
                {
                    int index = ActiveTexture.Width - 1 - X + Y * ActiveTexture.Width;

                    OutputTextureData[X + Y * ActiveTexture.Width] = ActiveTextureData[index];
                }
            }

            OutputTexture.SetData(OutputTextureData);

            return OutputTexture;
        }

        protected override void DoUpdate(GameTime gameTime)
        {
            VehiculeDrawablePart.Update(gameTime);
            UpdateAngleFromPlayer(RacingMap.ActiveVehicule);
        }

        private void UpdateAngleFromPlayer(Vehicule PlayerVehicule)
        {
            float AngleBetweenPlayer = MathHelper.ToDegrees(PlayerVehicule.GetAngleBetweenTarget(this));
            int FinalAngle = 0;

            bool Mirror = AngleBetweenPlayer > 0;

            if (AngleBetweenPlayer <= 2f && AngleBetweenPlayer >= -2f)
            {
                FinalAngle = 0;
            }
            else if (AngleBetweenPlayer <= 7f && AngleBetweenPlayer >= -7f)
            {
                FinalAngle = 5;
            }
            else if (AngleBetweenPlayer <= 15f && AngleBetweenPlayer >= -15)
            {
                FinalAngle = 10;
            }
            else if (AngleBetweenPlayer <= 30f && AngleBetweenPlayer >= -30f)
            {
                FinalAngle = 25;
            }
            else if (AngleBetweenPlayer <= 40f && AngleBetweenPlayer >= -40f)
            {
                FinalAngle = 35;
            }
            else if (AngleBetweenPlayer <= 52f && AngleBetweenPlayer >= -52f)
            {
                FinalAngle = 45;
            }
            else if (AngleBetweenPlayer <= 75f && AngleBetweenPlayer >= -75f)
            {
                FinalAngle = 65;
            }
            else if (AngleBetweenPlayer <= 100f && AngleBetweenPlayer >= -100f)
            {
                FinalAngle = 90;
            }

            if (LastAngle != AngleBetweenPlayer)
            {
                if (Mirror)
                {
                    VehiculeDrawablePart.SetTexture(DicVehiculeSpritePerAngle[-FinalAngle]);
                }
                else
                {
                    VehiculeDrawablePart.SetTexture(DicVehiculeSpritePerAngle[FinalAngle]);
                }
                LastAngle = AngleBetweenPlayer;
            }
        }

        public override void Draw(CustomSpriteBatch g, Matrix View)
        {

            Matrix World = Matrix.CreateTranslation(0f, 0f, 0f);

            VehiculeDrawablePart.ArrayParticles[0].Position = Position;
            VehiculeDrawablePart.ArrayParticles[0 + 1].Position = Position;
            VehiculeDrawablePart.ArrayParticles[0 + 2].Position = Position;
            VehiculeDrawablePart.ArrayParticles[0 + 3].Position = Position;

            VehiculeDrawablePart.MoveParticle(0, Vector3.Zero);
            VehiculeDrawablePart.SetViewProjection(World * View, Projection);
            VehiculeDrawablePart.Draw(g.GraphicsDevice);

            foreach (Object3D AntiGravPropulsor in ArrayAntiGravPropulsor)
            {
                AntiGravPropulsor.Draw(g, View);
            }
        }
    }
}
