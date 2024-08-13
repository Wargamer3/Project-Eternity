using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.LightningSystem;
using FMOD;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    class WeatherLightningManager
    {
        FMODSound Thunder1;
        FMODSound Thunder2;
        FMODSound Thunder3;
        LightningBolt Lightning;

        public float Intensity;
        public float AmbiantLightMultiplier;

        private double TimeRemainingBeforeLightningStrikeInSeconds;
        private const double LightningLengthInSeconds = 1.2f;
        private double TimeRemainingVisibleLightingInSeconds;

        public WeatherLightningManager(ContentManager Content, Matrix Projection)
        {
            AmbiantLightMultiplier = 1;

            Lightning = new LightningBolt();
            Lightning.Init(Content, GameScreen.GraphicsDevice, LightningDescriptor.LightningTree(Lightning));
            Lightning.SetWorldViewProjectionMatrix(Matrix.Identity, Matrix.Identity, Projection);

            Thunder1 = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Weather/Rain/Thunder 1.wav");
            Thunder2 = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Weather/Rain/Thunder 2.wav");
            Thunder3 = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Weather/Rain/Thunder 3.wav");
        }

        public void Update(GameTime gameTime)
        {
            double TimeElapsedInSeconds = gameTime.ElapsedGameTime.TotalSeconds;

            if (TimeRemainingVisibleLightingInSeconds > 0)
            {
                TimeRemainingVisibleLightingInSeconds -= TimeElapsedInSeconds;
                AmbiantLightMultiplier = (int)(Math.Sin((float)(TimeRemainingVisibleLightingInSeconds / LightningLengthInSeconds * Math.PI)) * 80);

                if (TimeRemainingVisibleLightingInSeconds <= 0)
                {
                    AmbiantLightMultiplier = 1;
                    if (RandomHelper.Next(5) == 0)
                    {
                        TimeRemainingBeforeLightningStrikeInSeconds = RandomHelper.NextDouble() * 5 + RandomHelper.NextDouble() * 2;
                    }
                    else
                    {
                        TimeRemainingBeforeLightningStrikeInSeconds = RandomHelper.NextDouble() * 30 + RandomHelper.NextDouble() * 10;
                    }
                }
            }
            else if (Intensity > 500)
            {
                TimeRemainingBeforeLightningStrikeInSeconds -= TimeElapsedInSeconds;

                if (TimeRemainingBeforeLightningStrikeInSeconds <= 0)
                {
                    Lightning.CreateNewLightningBolt(new Vector3(300, -20, 0), new Vector3(300, 600, 0));
                    TimeRemainingVisibleLightingInSeconds = LightningLengthInSeconds;

                    int RandomChoice = RandomHelper.Next(3);
                    if (RandomChoice == 0)
                    {
                        Thunder1.Play();
                    }
                    else if (RandomChoice == 1)
                    {
                        Thunder2.Play();
                    }
                    else
                    {
                        Thunder3.Play();
                    }
                }
            }
            else if (TimeRemainingBeforeLightningStrikeInSeconds <= 0)
            {
                TimeRemainingBeforeLightningStrikeInSeconds = RandomHelper.NextDouble() * 30 + RandomHelper.NextDouble() * 10;
            }
        }
        public void BeginDraw(CustomSpriteBatch g)
        {
            if (TimeRemainingVisibleLightingInSeconds > 0)
            {
                Lightning.BeginDraw(g);
            }
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (TimeRemainingVisibleLightingInSeconds > 0)
            {
                Lightning.Draw(g, (int)(Math.Sin((float)(TimeRemainingVisibleLightingInSeconds / LightningLengthInSeconds * Math.PI)) * 255));
            }
        }
    }
}
