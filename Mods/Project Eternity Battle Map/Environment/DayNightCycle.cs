using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class DayNightCycleColorOnly : BattleMapOverlay
    {
        private readonly BattleMap Map;
        private readonly MapZone Owner;

        public DayNightCycleColorOnly(BattleMap Map, MapZone Owner)
        {
            this.Map = Map;
            this.Owner = Owner;
        }

        public void Reset()
        {
        }

        public void Update(GameTime gameTime)
        {
            Color Color1;
            Color Color2;
            Color FinalColor = Color.White;
            double CurrentTime = Map.MapEnvironment.CurrentHour;

            if (CurrentTime >= 10 && CurrentTime < 18)
            {
                FinalColor = Color.FromNonPremultiplied(255, 255, 255, 0);
            }
            else if (CurrentTime >= 18 && CurrentTime < 20)
            {
                double Factor = (CurrentTime - 18) / 2;
                Color1 = Color.White;
                Color2 = Color.Orange;
                int newColorR = (int)(Color1.R * (1f - Factor) + Color2.R * Factor);
                int newColorG = (int)(Color1.G * (1f - Factor) + Color2.G * Factor);
                int newColorB = (int)(Color1.B * (1f - Factor) + Color2.B * Factor);
                FinalColor = Color.FromNonPremultiplied(newColorR, newColorG, newColorB, (int)(Factor * 100));
            }
            else if (CurrentTime >= 20 && CurrentTime < 22)
            {
                double Factor = (CurrentTime - 20) / 2;
                Color1 = Color.Orange;
                Color2 = Color.Navy;

                int newColorR = (int)(Color1.R * (1f - Factor) + Color2.R * Factor);
                int newColorG = (int)(Color1.G * (1f - Factor) + Color2.G * Factor);
                int newColorB = (int)(Color1.B * (1f - Factor) + Color2.B * Factor);
                FinalColor = Color.FromNonPremultiplied(newColorR, newColorG, newColorB, 100 + (int)(Factor * 27f));
            }
            else if (CurrentTime >= 22 || CurrentTime < 6)
            {
                FinalColor = Color.FromNonPremultiplied(Color.Navy.R, Color.Navy.G, Color.Navy.B, 127);
            }
            else if (CurrentTime >= 6 && CurrentTime < 8)
            {
                double Factor = (CurrentTime - 6) / 2;
                Color1 = Color.Navy;
                Color2 = Color.Orange;

                int newColorR = (int)(Color1.R * (1f - Factor) + Color2.R * Factor);
                int newColorG = (int)(Color1.G * (1f - Factor) + Color2.G * Factor);
                int newColorB = (int)(Color1.B * (1f - Factor) + Color2.B * Factor);
                FinalColor = Color.FromNonPremultiplied(newColorR, newColorG, newColorB, 127 - (int)(Factor * 27f));
            }
            else if (CurrentTime >= 8 && CurrentTime < 10)
            {
                double Factor = (CurrentTime - 8) / 2;
                Color1 = Color.Orange;
                Color2 = Color.White;

                int newColorR = (int)(Color1.R * (1f - Factor) + Color2.R * Factor);
                int newColorG = (int)(Color1.G * (1f - Factor) + Color2.G * Factor);
                int newColorB = (int)(Color1.B * (1f - Factor) + Color2.B * Factor);
                FinalColor = Color.FromNonPremultiplied(newColorR, newColorG, newColorB, (int)((1d - Factor) * 100d));
            }

            Owner.TimeOfDayColor = FinalColor;
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
        }

        public void Draw(CustomSpriteBatch g)
        {
            g.Draw(GameScreen.sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), Owner.TimeOfDayColor);
        }

        public void EndDraw(CustomSpriteBatch g)
        {
        }

        public void SetCrossfadeValue(double Value)
        {
            throw new NotImplementedException();
        }
    }

    public class DayNightCycleColorAndDark : BattleMapOverlay
    {
        private double CurrentHour;
        RenderTarget2D renderTarget;
        BasicEffect PolygonEffect;

        public DayNightCycleColorAndDark()
        {
            CurrentHour = 24;
            PolygonEffect = new BasicEffect(GameScreen.GraphicsDevice);

            PolygonEffect.VertexColorEnabled = true;
            PolygonEffect.TextureEnabled = true;
            PolygonEffect.View = Matrix.Identity;

            PolygonEffect.World = Matrix.Identity;
            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 0, 1);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            PolygonEffect.Projection = HalfPixelOffset * Projection;
            renderTarget = new RenderTarget2D(
                GameScreen.GraphicsDevice,
                GameScreen.GraphicsDevice.PresentationParameters.BackBufferWidth,
                GameScreen.GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.NormalizedByte4, DepthFormat.Depth24Stencil8);
        }

        public void Reset()
        {
        }

        public void Update(GameTime gameTime)
        {
            double EllapsedMinute = gameTime.ElapsedGameTime.TotalHours * 5d;
            CurrentHour += EllapsedMinute;
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            g.End();
            g.GraphicsDevice.SetRenderTarget(renderTarget);
            g.GraphicsDevice.Clear(Color.White);
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            DrawSurface(g);
        }

        private void DrawSurface(CustomSpriteBatch g)
        {
            Color Color1;
            Color Color2;
            Color FinalColor = Color.White;
            double CurrentTime = CurrentHour;

            if (CurrentHour >= 10 && CurrentHour < 18)
            {
            }
            // Sunset
            else if (CurrentHour >= 18 && CurrentHour < 20)
            {
                double Factor = (CurrentTime - 18) / 2;
                Color1 = Color.White;
                Color2 = Color.Orange;
                int newColorR = (int)(Color1.R * (1f - Factor) + Color2.R * Factor);
                int newColorG = (int)(Color1.G * (1f - Factor) + Color2.G * Factor);
                int newColorB = (int)(Color1.B * (1f - Factor) + Color2.B * Factor);
                FinalColor = Color.FromNonPremultiplied(newColorR, newColorG, newColorB, (int)(Factor * 100));
                g.Draw(GameScreen.sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), FinalColor);
            }
            //dusk
            else if (CurrentHour >= 20 && CurrentHour < 22)
            {
                double Factor = (CurrentTime - 20) / 2;
                Color1 = Color.Orange;
                Color2 = Color.Navy;

                int newColorR = (int)(Color1.R * (1f - Factor) + Color2.R * Factor);
                int newColorG = (int)(Color1.G * (1f - Factor) + Color2.G * Factor);
                int newColorB = (int)(Color1.B * (1f - Factor) + Color2.B * Factor);
                FinalColor = Color.FromNonPremultiplied(newColorR, newColorG, newColorB, 100 + (int)(Factor * 27f));
                g.Draw(GameScreen.sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), FinalColor);
            }
            //Night
            else if (CurrentHour >= 22 || CurrentHour < 6)
            {
                FinalColor = Color.FromNonPremultiplied(Color.Navy.R, Color.Navy.G, Color.Navy.B, 127);
                g.Draw(GameScreen.sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), FinalColor);
            }//Dawn
            else if (CurrentHour >= 6 && CurrentHour < 8)
            {
                double Factor = (CurrentTime - 6) / 2;
                Color1 = Color.Navy;
                Color2 = Color.Orange;

                int newColorR = (int)(Color1.R * (1f - Factor) + Color2.R * Factor);
                int newColorG = (int)(Color1.G * (1f - Factor) + Color2.G * Factor);
                int newColorB = (int)(Color1.B * (1f - Factor) + Color2.B * Factor);
                FinalColor = Color.FromNonPremultiplied(newColorR, newColorG, newColorB, 127 - (int)(Factor * 27f));
                g.Draw(GameScreen.sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), FinalColor);
            }//Sunrise
            else if (CurrentHour >= 8 && CurrentHour < 10)
            {
                double Factor = (CurrentTime - 8) / 2;
                Color1 = Color.Orange;
                Color2 = Color.White;

                int newColorR = (int)(Color1.R * (1f - Factor) + Color2.R * Factor);
                int newColorG = (int)(Color1.G * (1f - Factor) + Color2.G * Factor);
                int newColorB = (int)(Color1.B * (1f - Factor) + Color2.B * Factor);
                FinalColor = Color.FromNonPremultiplied(newColorR, newColorG, newColorB, (int)((1d - Factor) * 100d));
                g.Draw(GameScreen.sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), FinalColor);
            }
        }

        public void Draw(CustomSpriteBatch g)
        {
            BlendState blendState = new BlendState();
            blendState.AlphaDestinationBlend = Blend.SourceAlpha;
            blendState.ColorDestinationBlend = Blend.SourceColor;
            blendState.AlphaSourceBlend = Blend.Zero;
            blendState.ColorSourceBlend = Blend.Zero;

            g.End();
            g.Begin(SpriteSortMode.Deferred, blendState);
            g.Draw(renderTarget, Vector2.Zero, Color.White);
            g.End();
            g.Begin();
        }

        public void EndDraw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin();
        }

        public void SetCrossfadeValue(double Value)
        {
            throw new NotImplementedException();
        }
    }

    public class PerlinNoiseFogOverlay : BattleMapOverlay
    {
        Color CouleurCiel;
        Color CouleurTransparant;
        Texture2D texture;
        Color[] data;

        public PerlinNoiseFogOverlay()
        {
            CouleurCiel = Color.FromNonPremultiplied(80, 80, 80, 255);
            CouleurTransparant = Color.FromNonPremultiplied(0x30, 0x30, 0x30, 0x10);

            texture = new Texture2D(GameScreen.GraphicsDevice,
                GameScreen.GraphicsDevice.PresentationParameters.BackBufferWidth,
                GameScreen.GraphicsDevice.PresentationParameters.BackBufferHeight);

            data = new Color[texture.Width * texture.Height];
        }

        public void Reset()
        {
        }

        public void Update(GameTime gameTime)
        {
            float Temps = 0;
            AlgoDePerlin AlgoDePerlin = new AlgoDePerlin();
            AlgoDePerlin.Frequency = 0.02f;
            AlgoDePerlin.Amplitude = 1f;
            AlgoDePerlin.Octaves = 1;
            AlgoDePerlin.Persistence = 0f;
            //the array holds the color for each pixel in the texture
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                {
                    float a = AlgoDePerlin.Compute(x, y, Temps);
                    float ai = 1f - a;

                    //the function applies the color according to the specified pixel
                    data[x + y * texture.Width] = Color.FromNonPremultiplied(
                        (int)(a * CouleurCiel.R + ai * CouleurTransparant.R),
                        (int)(a * CouleurCiel.G + ai * CouleurTransparant.G),
                        (int)(a * CouleurCiel.B + ai * CouleurTransparant.B),
                        (int)(a * CouleurCiel.A + ai * CouleurTransparant.A));
                }
            //set the color
            texture.SetData(data);
            Temps += 0.2f;
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
        }

        public void Draw(CustomSpriteBatch g)
        {
            g.Draw(texture, Vector2.Zero, Color.White);
        }

        public void EndDraw(CustomSpriteBatch g)
        {
        }

        public void SetCrossfadeValue(double Value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation of 3D Perlin Noise after Ken Perlin's reference implementation.
        /// </summary>
        public class AlgoDePerlin
        {
            private int[] permutation;
            private int[] p;
            public float Frequency;
            public float Amplitude;
            public float Persistence;
            public int Octaves;
            public AlgoDePerlin()
            {
                permutation = new int[256];
                p = new int[permutation.Length * 2];
                Init();

                Frequency = 0.023f;
                Amplitude = 2.2f;
                Persistence = 0.9f;
                Octaves = 2;
            }

            private void Init()
            {
                Random rand = new Random();
                //On s'assure que rien soit a null pour pas avoir un beau null exception
                for (int i = 0; i < permutation.Length; i++)
                    permutation[i] = -1;
                //On remplit le tout de nombres entiers aléatoires
                for (int i = 0; i < permutation.Length; i++)
                    while (true)
                    {
                        int iP = rand.Next() % permutation.Length;
                        if (permutation[iP] == -1)
                        {
                            permutation[iP] = i;
                            break;
                        }
                    }
                //Double remplissage, gain de vitesse plus tard pour accéder aux random tout en gardant une cohérence
                for (int i = 0; i < permutation.Length; i++)
                    p[permutation.Length + i] = p[i] = permutation[i];
            }

            public float Compute(float x, float y, float z)
            {
                float noise = 0;
                float amp = this.Amplitude;
                float freq = this.Frequency;
                for (int i = 0; i < this.Octaves; i++)
                {
                    noise += Noise(x * freq, y * freq, z * freq) * amp;
                    freq *= 2;                                // octave is the double of the previous frequency
                    amp *= this.Persistence;
                }

                // Clamp and return the result
                if (noise < 0)
                {
                    return 0;
                }
                else if (noise > 1)
                {
                    return 1;
                }
                return noise;
            }

            private float Noise(float x, float y, float z)
            {
                //On prend le premier byte(valeur limité a 255)
                int iX = (int)Math.Floor(x) & 255;
                int iY = (int)Math.Floor(y) & 255;
                int iZ = (int)Math.Floor(z) & 255;

                // Find relative x, y, z of the point in the cube.
                x -= (float)Math.Floor(x);
                y -= (float)Math.Floor(y);
                z -= (float)Math.Floor(z);

                // Compute fade curves for each of x, y, z
                float u = Fade(x);
                float v = Fade(y);
                float w = Fade(z);

                // Hash coordinates of the 8 cube corners
                int A = p[iX] + iY;
                int AA = p[A] + iZ;
                int AB = p[A + 1] + iZ;
                int B = p[iX + 1] + iY;
                int BA = p[B] + iZ;
                int BB = p[B + 1] + iZ;

                // And add blended results from 8 corners of cube.
                return Lerp(w, Lerp(v, Lerp(u, Grad(p[AA], x, y, z),
                                   Grad(p[BA], x - 1, y, z)),
                           Lerp(u, Grad(p[AB], x, y - 1, z),
                                   Grad(p[BB], x - 1, y - 1, z))),
                   Lerp(v, Lerp(u, Grad(p[AA + 1], x, y, z - 1),
                                   Grad(p[BA + 1], x - 1, y, z - 1)),
                           Lerp(u, Grad(p[AB + 1], x, y - 1, z - 1),
                                   Grad(p[BB + 1], x - 1, y - 1, z - 1))));
            }

            private static float Fade(float t)
            {
                //Interpolation non linéaire
                return (t * t * t * (t * (t * 6 - 15) + 10));
            }

            private static float Lerp(float alpha, float a, float b)
            {
                //Interpolation linéaire
                return (a + alpha * (b - a));
            }

            private static float Grad(int hashCode, float x, float y, float z)
            {
                // Convert lower 4 bits of hash code into 12 gradient directions
                int h = hashCode & 15;
                float u = h < 8 ? x : y;
                float v = h < 4 ? y : h == 12 || h == 14 ? x : z;
                return (((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v));
            }
        }
    }
}
