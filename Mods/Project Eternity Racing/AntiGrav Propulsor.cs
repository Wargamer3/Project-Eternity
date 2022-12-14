using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.RacingScreen
{
    public class AntiGravPropulsor : Object3D
    {
        private VertexPositionNormalTexture[] ArrayVertexBound;
        public float MinPushStrength;
        public float MaxPushStrength;
        public float AbsorbtionStrength;//Strength applied to stop fall
        public float RepulsiveStrength;//Strength applied to regain proper altitude
        public float Length;
        float PreferedAltitude = 0.2f;

        public AntiGravPropulsor(GraphicsDevice g, Matrix Projection, float Length)
            : base(g, Projection)
        {
            Init(g, Projection);
            MinPushStrength = 10f;
            MaxPushStrength = 600f;
            AbsorbtionStrength = 600f;
            RepulsiveStrength = 310f;
            this.Length = Length * 2;
            Scale(new Vector3(1f, Length, 1f));
        }

        private void Init(GraphicsDevice g, Matrix Projection)
        {
            ObjectEffect.AmbientLightColor = new Vector3(1, 0.5f, 0.5f);
            ObjectEffect.DirectionalLight0.Enabled = true;
            ObjectEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            ObjectEffect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);
            ObjectEffect.LightingEnabled = true;

            ArrayVertexBound = RacingMap.CreateCube();
        }

        public Vector3 UpdatePropulsorTrust(GameTime gameTime, Vector3 VehiculeSpeed, List<Object3D> ListCollisionBox)
        {
            Vector3 OutputSpeed = Vector3.Zero;
            float MinPush = MinPushStrength;
            float MaxPush = MaxPushStrength;

            PolygonMesh.PolygonMeshCollisionResult Result = GetClosestObject(ListCollisionBox, VehiculeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            if (Result.Collided)
            {
                float CurrentPush = Vector3.Dot(VehiculeSpeed, Up);
                float CurrentSpeed = Vector3.Dot(VehiculeSpeed, Result.Axis);
                float FinalRepulsiveStrength = RepulsiveStrength * (float)gameTime.ElapsedGameTime.TotalSeconds;
                float FinalAbsorbtionStrength = AbsorbtionStrength * (float)gameTime.ElapsedGameTime.TotalSeconds;

                float WillImpact = TimeBeforeImpact(VehiculeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, Result, Vector3.Zero);

                //Going downward and the repulsive strength isn't enough to stop the fall, use absorbtion strength
                if (WillImpact >= 0)
                {
                    OutputSpeed += Up * FinalAbsorbtionStrength;
                }
                else
                {//Going downward but can use repulsive strength to stabilize
                    if (CurrentPush < 0f)
                    {
                        if (CurrentPush + FinalRepulsiveStrength > 0)
                        {
                            OutputSpeed -= Up * CurrentPush;//Cancel forces to keep same altitude
                            OutputSpeed += StabilizeAltitude(gameTime, Result);
                        }
                        else
                        {
                            OutputSpeed -= Up * -FinalRepulsiveStrength;
                        }
                    }
                    //Going upward and can use repulsive strength to stabilize
                    else if (CurrentPush > 0f)
                    {
                        if (CurrentPush - FinalRepulsiveStrength < 0)
                        {
                            OutputSpeed -= Up * CurrentPush;//Cancel forces to keep same altitude
                            OutputSpeed += StabilizeAltitude(gameTime, Result);
                        }
                        else
                        {
                            OutputSpeed -= Up * FinalRepulsiveStrength;
                        }
                    }
                    else
                    {
                        OutputSpeed -= RacingMap.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        OutputSpeed += StabilizeAltitude(gameTime, Result);
                    }
                }
            }
            else
            {
                OutputSpeed += Up * MinPush * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            return OutputSpeed;
        }

        public Vector3 StabilizeAltitude(GameTime gameTime, PolygonMesh.PolygonMeshCollisionResult Result)
        {
            float CurrentAltitude = Length - Result.Distance;
            float FinalRepulsiveStrength = RepulsiveStrength * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float FinalAbsorbtionStrength = AbsorbtionStrength * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Higher than PreferedAltitude
            if (CurrentAltitude > PreferedAltitude)
            {
                float AltitudeDifference = CurrentAltitude - PreferedAltitude;
                if (AltitudeDifference > FinalRepulsiveStrength)
                {
                    return -Up * FinalAbsorbtionStrength;
                }
                else
                {
                    return Up * (Result.Distance - PreferedAltitude);
                }
            }
            //Lower than PreferedAltitude
            else if (CurrentAltitude < PreferedAltitude)
            {
                float AltitudeDifference = PreferedAltitude - CurrentAltitude;
                if (AltitudeDifference > FinalRepulsiveStrength)
                {
                    return Up * FinalAbsorbtionStrength;
                }
                else
                {
                    return Up * (Result.Distance - PreferedAltitude);
                }
            }

            return Vector3.Zero;
        }

        public float TimeBeforeImpact(Vector3 ActiveVehiculeSpeed, PolygonMesh.PolygonMeshCollisionResult Result, Vector3 ExtraSpeed)
        {
            float CurrentAltitude = Length - Result.Distance;
            if (CurrentAltitude < 0)
            {
                CurrentAltitude = 0;
            }

            float CurrentSpeed = Vector3.Dot(ActiveVehiculeSpeed + ExtraSpeed, Result.Axis);
            float FinalAbsorbtionStrength = Math.Abs(Vector3.Dot((AbsorbtionStrength - 300) * Up, Result.Axis));
            float FinalRepulsiveStrength = Math.Abs(Vector3.Dot((RepulsiveStrength - 300) * Up, Result.Axis));

            if (CurrentSpeed < 0)
            {
                float EstimatedTimeBeforeImapact = CurrentAltitude / -CurrentSpeed;
                if (FinalRepulsiveStrength * EstimatedTimeBeforeImapact >= -CurrentSpeed)
                {
                    //Impact can be avoided softly.
                    return -2;
                }
                else if (FinalAbsorbtionStrength * EstimatedTimeBeforeImapact >= -CurrentSpeed)
                {
                    //Impact can be avoided.
                    return EstimatedTimeBeforeImapact;
                }

                //Impact can't be avoided.
                return EstimatedTimeBeforeImapact;
            }

            //No impact possible.
            return -1;
        }

        public override void Draw(CustomSpriteBatch g, Matrix View)
        {
            ObjectEffect.View = View;
            
            ObjectEffect.CurrentTechnique.Passes[0].Apply();
            g.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, ArrayVertexBound, 0, 12);
        }
    }
}
