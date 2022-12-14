#region File Description

//-----------------------------------------------------------------------------
// ParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class BackgroundSystem
    {
        #region Fields

        // Settings class controls the appearance and animation of this particle system.
        public string TextureName;

        public int MaxParticles;
        public int NumberOfImages;

        public BlendState BlendState;
        private bool _UseAlphaBlend;
        private bool _RepeatX;
        private bool _RepeatY;
        private bool _RepeatZ;
        private Vector3 _RepeatOffset;
        private Vector3 _Speed;

        // Custom effect for drawing particles. This computes the particle
        // animation entirely in the vertex shader: no per-particle CPU work required!
        private Effect particleEffect;

        // Shortcuts for accessing frequently changed effect parameters.
        private EffectParameter effectViewParameter;

        private EffectParameter effectProjectionParameter;
        public EffectParameter effectViewportScaleParameter;
        private EffectParameter effectTimeParameter;

        // An array of particles, treated as a circular queue.
        public ParticleVertex[] ArrayParticles;

        // A vertex buffer holding our particles. This contains the same data as
        // the particles array, but copied across to where the GPU can access it.
        private DynamicVertexBuffer vertexBuffer;

        // Index buffer turns sets of four vertices into particle quads (pairs of triangles).
        private IndexBuffer indexBuffer;

        // The particles array and vertex buffer are treated as a circular queue.
        // Initially, the entire contents of the array are free, because no particles
        // are in use. When a new particle is created, this is allocated from the
        // beginning of the array. If more than one particle is created, these will
        // always be stored in a consecutive block of array elements. Because all
        // particles last for the same amount of time, old particles will always be
        // removed in order from the start of this active particle region, so the
        // active and free regions will never be intermingled. Because the queue is
        // circular, there can be times when the active particle region wraps from the
        // end of the array back to the start. The queue uses modulo arithmetic to
        // handle these cases. For instance with a four entry queue we could have:
        //
        //      0
        //      1 - first active particle
        //      2
        //      3 - first free particle
        //
        // In this case, particles 1 and 2 are active, while 3 and 4 are free.
        // Using modulo arithmetic we could also have:
        //
        //      0
        //      1 - first free particle
        //      2
        //      3 - first active particle
        //
        // Here, 3 and 0 are active, while 1 and 2 are free.
        //
        // But wait! The full story is even more complex.
        //
        // When we create a new particle, we add them to our managed particles array.
        // We also need to copy this new data into the GPU vertex buffer, but we don't
        // want to do that straight away, because setting new data into a vertex buffer
        // can be an expensive operation. If we are going to be adding several particles
        // in a single frame, it is faster to initially just store them in our managed
        // array, and then later upload them all to the GPU in one single call. So our
        // queue also needs a region for storing new particles that have been added to
        // the managed array but not yet uploaded to the vertex buffer.
        //
        // Another issue occurs when old particles are retired. The CPU and GPU run
        // asynchronously, so the GPU will often still be busy drawing the previous
        // frame while the CPU is working on the next frame. This can cause a
        // synchronization problem if an old particle is retired, and then immediately
        // overwritten by a new one, because the CPU might try to change the contents
        // of the vertex buffer while the GPU is still busy drawing the old data from
        // it. Normally the graphics driver will take care of this by waiting until
        // the GPU has finished drawing inside the VertexBuffer.SetData call, but we
        // don't want to waste time waiting around every time we try to add a new
        // particle! To avoid this delay, we can specify the SetDataOptions.NoOverwrite
        // flag when we write to the vertex buffer. This basically means "I promise I
        // will never try to overwrite any data that the GPU might still be using, so
        // you can just go ahead and update the buffer straight away". To keep this
        // promise, we must avoid reusing vertices immediately after they are drawn.
        //
        // So in total, our queue contains four different regions:
        //
        // Vertices between firstActiveParticle and firstNewParticle are actively
        // being drawn, and exist in both the managed particles array and the GPU
        // vertex buffer.
        //
        // Vertices between firstNewParticle and firstFreeParticle are newly created,
        // and exist only in the managed particles array. These need to be uploaded
        // to the GPU at the start of the next draw call.
        //
        // Vertices between firstFreeParticle and firstRetiredParticle are free and
        // waiting to be allocated.
        //
        // Vertices between firstRetiredParticle and firstActiveParticle are no longer
        // being drawn, but were drawn recently enough that the GPU could still be
        // using them. These need to be kept around for a few more frames before they
        // can be reallocated.

        public int FirstActiveParticle;
        public int FirstNewParticle;
        public int FirstFreeParticle;
        public int FirstRetiredParticle;

        // Store the current time, in seconds.
        private float CurrentTime;

        // Count how many times Draw has been called. This is used to know
        // when it is safe to retire old particles back into the free list.
        private int drawCounter;

        public EffectParameterCollection Parameters;

        #endregion

        public BackgroundSystem(string TextureName, int MaxParticles, int NumberOfImages, BlendState BlendState, bool RotateTowardCamera, ContentManager Content, GraphicsDevice GraphicsDevice)
        {
            this.TextureName = TextureName;
            this.MaxParticles = MaxParticles;
            this.NumberOfImages = NumberOfImages;
            this.BlendState = BlendState;
            _UseAlphaBlend = false;

            // Allocate the particle array, and fill in the corner fields (which never change).
            ArrayParticles = new ParticleVertex[MaxParticles * 4];

            for (int i = 0; i < MaxParticles; i++)
            {
                ArrayParticles[i * 4 + 0].UV = new Vector2(0, 0);
                ArrayParticles[i * 4 + 1].UV = new Vector2(1, 0);
                ArrayParticles[i * 4 + 2].UV = new Vector2(1, 1);
                ArrayParticles[i * 4 + 3].UV = new Vector2(0, 1);
            }

            Effect effect = Content.Load<Effect>("Shaders/Particle shader 3D");

            // If we have several particle systems, the content manager will return
            // a single shared effect instance to them all. But we want to preconfigure
            // the effect with parameters that are specific to this particular
            // particle system. By cloning the effect, we prevent one particle system
            // from stomping over the parameter settings of another.

            particleEffect = effect.Clone();

            Parameters = particleEffect.Parameters;

            // Look up shortcuts for parameters that change every frame.
            effectViewParameter = Parameters["View"];
            effectProjectionParameter = Parameters["Projection"];
            effectViewportScaleParameter = Parameters["ViewportScale"];
            effectTimeParameter = Parameters["CurrentTime"];

            // Set the values of parameters that do not change.
            Parameters["NumberOfImages"].SetValue(NumberOfImages);
            Parameters["RotateTowardCamera"].SetValue(RotateTowardCamera ? 1f : 0);

            // Load the particle texture, and set it onto the effect.
            Texture2D sprBackground = Content.Load<Texture2D>(TextureName);

            Parameters["Size"].SetValue(new Vector2((sprBackground.Width / NumberOfImages) * 0.5f, sprBackground.Height * 0.5f));
            Parameters["t0"].SetValue(sprBackground);
            _RepeatOffset.X = sprBackground.Width;
            _RepeatOffset.Y = sprBackground.Height;

            // Create a dynamic vertex buffer.
            vertexBuffer = new DynamicVertexBuffer(GraphicsDevice, ParticleVertex.VertexDeclaration,
                                                   MaxParticles * 4, BufferUsage.WriteOnly);

            // Create and populate the index buffer.
            ushort[] ArrayIndex = new ushort[MaxParticles * 6];

            for (int i = 0; i < MaxParticles; i++)
            {
                ArrayIndex[i * 6 + 0] = (ushort)(i * 4 + 0);
                ArrayIndex[i * 6 + 1] = (ushort)(i * 4 + 1);
                ArrayIndex[i * 6 + 2] = (ushort)(i * 4 + 2);

                ArrayIndex[i * 6 + 3] = (ushort)(i * 4 + 0);
                ArrayIndex[i * 6 + 4] = (ushort)(i * 4 + 2);
                ArrayIndex[i * 6 + 5] = (ushort)(i * 4 + 3);
            }

            indexBuffer = new IndexBuffer(GraphicsDevice, typeof(ushort), ArrayIndex.Length, BufferUsage.WriteOnly);

            indexBuffer.SetData(ArrayIndex);
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            CurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // If we let our timer go on increasing for ever, it would eventually
            // run out of floating point precision, at which point the particles
            // would render incorrectly. An easy way to prevent this is to notice
            // that the time value doesn't matter when no particles are being drawn,
            // so we can reset it back to zero any time the active queue is empty.

            if (FirstActiveParticle == FirstFreeParticle)
                CurrentTime = 0;

            if (FirstRetiredParticle == FirstActiveParticle)
                drawCounter = 0;
        }

        public void SetViewProjection(Matrix View, Matrix Projection)
        {
            effectViewParameter.SetValue(View);
            effectProjectionParameter.SetValue(Projection);
        }

        public void Draw(GraphicsDevice GraphicsDevice)
        {
            GraphicsDevice device = GraphicsDevice;

            // Restore the vertex buffer contents if the graphics device was lost.
            if (vertexBuffer.IsContentLost)
            {
                vertexBuffer.SetData(ArrayParticles);
            }

            // If there are any particles waiting in the newly added queue,
            // we'd better upload them to the GPU ready for drawing.
            if (FirstNewParticle != FirstFreeParticle)
            {
                AddNewParticlesToVertexBuffer();
            }

            // If there are any active particles, draw them now!
            if (FirstActiveParticle != FirstFreeParticle)
            {
                device.BlendState = BlendState;
                if (_UseAlphaBlend)
                    device.DepthStencilState = DepthStencilState.DepthRead;
                else
                    device.DepthStencilState = DepthStencilState.Default;

                // Set an effect parameter describing the viewport size. This is
                // needed to convert particle sizes into screen space point sizes.
                effectViewportScaleParameter.SetValue(new Vector2(0.5f / device.Viewport.AspectRatio, -0.5f));

                // Set an effect parameter describing the current time. All the vertex
                // shader particle animation is keyed off this value.
                effectTimeParameter.SetValue(CurrentTime);

                // Set the particle vertex and index buffer.
                device.SetVertexBuffer(vertexBuffer);
                device.Indices = indexBuffer;

                // Activate the particle effect.
                foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    if (FirstActiveParticle < FirstFreeParticle)
                    {
                        // If the active particles are all in one consecutive range,
                        // we can draw them all in a single call.
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     FirstActiveParticle * 4, (FirstFreeParticle - FirstActiveParticle) * 4,
                                                     FirstActiveParticle * 6, (FirstFreeParticle - FirstActiveParticle) * 2);
                    }
                    else
                    {
                        // If the active particle range wraps past the end of the queue
                        // back to the start, we must split them over two draw calls.
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     FirstActiveParticle * 4, (MaxParticles - FirstActiveParticle) * 4,
                                                     FirstActiveParticle * 6, (MaxParticles - FirstActiveParticle) * 2);

                        if (FirstFreeParticle > 0)
                        {
                            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                         0, FirstFreeParticle * 4,
                                                         0, FirstFreeParticle * 2);
                        }
                    }
                }

                // Reset some of the renderstates that we changed,
                // so as not to mess up any other subsequent drawing.
                device.DepthStencilState = DepthStencilState.Default;
            }

            drawCounter++;
        }

        /// <summary>
        /// Helper for uploading new particles from our managed
        /// array to the GPU vertex buffer.
        /// </summary>
        private void AddNewParticlesToVertexBuffer()
        {
            int stride = ParticleVertex.SizeInBytes;

            if (FirstNewParticle < FirstFreeParticle)
            {
                // If the new particles are all in one consecutive range,
                // we can upload them all in a single call.
                vertexBuffer.SetData(FirstNewParticle * stride * 4, ArrayParticles,
                                     FirstNewParticle * 4,
                                     (FirstFreeParticle - FirstNewParticle) * 4,
                                     stride, SetDataOptions.NoOverwrite);
            }
            else
            {
                // If the new particle range wraps past the end of the queue
                // back to the start, we must split them over two upload calls.
                vertexBuffer.SetData(FirstNewParticle * stride * 4, ArrayParticles,
                                     FirstNewParticle * 4,
                                     (MaxParticles - FirstNewParticle) * 4,
                                     stride, SetDataOptions.NoOverwrite);

                if (FirstFreeParticle > 0)
                {
                    vertexBuffer.SetData(0, ArrayParticles,
                                         0, FirstFreeParticle * 4,
                                         stride, SetDataOptions.NoOverwrite);
                }
            }

            // Move the particles we just uploaded from the new to the active queue.
            FirstNewParticle = FirstFreeParticle;
        }

        public void MoveParticle(int Index, Vector3 Movement)
        {
            int stride = ParticleVertex.SizeInBytes;

            for (int V = 3; V >= 0; --V)
            {
                ArrayParticles[Index + V].Position += Movement;
            }
            vertexBuffer.SetData(Index * stride * 4, ArrayParticles,
                                 Index * 4,
                                 4,
                                 stride, SetDataOptions.None);
        }

        public void AddParticle(Vector3[] ArrayPosition)
        {
            // Figure out where in the circular queue to allocate the new particle.
            int nextFreeParticle = FirstFreeParticle + 1;

            if (nextFreeParticle >= MaxParticles)
                nextFreeParticle = 0;

            // If there are no free particles, we just have to give up.
            if (nextFreeParticle == FirstRetiredParticle)
                return;

            // Fill in the particle vertex structure.
            for (int i = 0; i < 4; i++)
            {
                ArrayParticles[FirstFreeParticle * 4 + i].Time = CurrentTime;
                ArrayParticles[FirstFreeParticle * 4 + i].Position = ArrayPosition[i];
            }

            FirstFreeParticle = nextFreeParticle;
        }

        public void AddParticle(Vector3 Position)
        {
            // Figure out where in the circular queue to allocate the new particle.
            int nextFreeParticle = FirstFreeParticle + 1;

            if (nextFreeParticle >= MaxParticles)
                nextFreeParticle = 0;

            // If there are no free particles, we just have to give up.
            if (nextFreeParticle == FirstRetiredParticle)
                return;

            // Fill in the particle vertex structure.
            for (int i = 0; i < 4; i++)
            {
                ArrayParticles[FirstFreeParticle * 4 + i].Time = CurrentTime;
                if (RotateTowardCamera)
                {
                    ArrayParticles[FirstFreeParticle * 4 + i].Position = Position;
                }
            }

            FirstFreeParticle = nextFreeParticle;
        }

        public void RemoveParticle(int Index)
        {
            ParticleVertex[] NewArrayParticles = new ParticleVertex[ArrayParticles.Length - 4];
            Array.Copy(ArrayParticles, NewArrayParticles, Index * 4);
            Array.Copy(ArrayParticles, (Index + 1) * 4, NewArrayParticles, Index * 4, ArrayParticles.Length - (Index + 1) * 4);
            ArrayParticles = NewArrayParticles;

            --FirstFreeParticle;

            int stride = ParticleVertex.SizeInBytes;
            vertexBuffer.SetData(0, ArrayParticles,
                                 0, FirstFreeParticle * 4,
                                 stride, SetDataOptions.Discard);
        }

        public override string ToString()
        {
            return TextureName;
        }

        #region Properties

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        public bool UseAlphaBlend
        {
            get
            {
                return _UseAlphaBlend;
            }
            set
            {
                _UseAlphaBlend = value;
            }
        }

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        public bool RotateTowardCamera
        {
            get
            {
                return Parameters["RotateTowardCamera"].GetValueSingle() == 1 ? true : false;
            }
            set
            {
                Parameters["RotateTowardCamera"].SetValue(value ? 1f : 0f);
                Vector2 ParticleSize = Parameters["Size"].GetValueVector2();
                for (int P = 0; P < FirstFreeParticle; P++)
                {
                    if (!RotateTowardCamera)
                    {
                        Vector3 OriginalPosition = ArrayParticles[P * 4].Position;
                        ArrayParticles[P * 4].Position = new Vector3(OriginalPosition.X - ParticleSize.X, OriginalPosition.Y - ParticleSize.Y * 2, OriginalPosition.Z);
                        ArrayParticles[P * 4 + 1].Position = new Vector3(OriginalPosition.X + ParticleSize.X, OriginalPosition.Y - ParticleSize.Y * 2, OriginalPosition.Z);
                        ArrayParticles[P * 4 + 2].Position = new Vector3(OriginalPosition.X + ParticleSize.X, OriginalPosition.Y, OriginalPosition.Z);
                        ArrayParticles[P * 4 + 3].Position = new Vector3(OriginalPosition.X - ParticleSize.X, OriginalPosition.Y, OriginalPosition.Z);
                    }
                    else
                    {
                        Vector3 CenterPosition = new Vector3();

                        for (int i = 0; i < 4; i++)
                        {
                            CenterPosition += ArrayParticles[P * 4 + i].Position;
                        }
                        CenterPosition /= 4;
                        CenterPosition.Y += ParticleSize.Y;
                        for (int i = 0; i < 4; i++)
                        {
                            ArrayParticles[P * 4 + i].Position = CenterPosition;
                        }
                    }
                }

                int stride = ParticleVertex.SizeInBytes;
                vertexBuffer.SetData(0, ArrayParticles, 0,
                                     FirstFreeParticle * 4,
                                     stride, SetDataOptions.Discard);
            }
        }

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        public bool RepeatX
        {
            get
            {
                return _RepeatX;
            }
            set
            {
                _RepeatX = value;
            }
        }

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        public bool RepeatY
        {
            get
            {
                return _RepeatY;
            }
            set
            {
                _RepeatY = value;
            }
        }

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        public bool RepeatZ
        {
            get
            {
                return _RepeatZ;
            }
            set
            {
                _RepeatZ = value;
            }
        }

        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        public Vector3 RepeatOffset
        {
            get
            {
                return _RepeatOffset;
            }
            set
            {
                _RepeatOffset = value;
            }
        }
        
        [CategoryAttribute("Object Attributes"),
        DescriptionAttribute(".")]
        public Vector3 Speed
        {
            get
            {
                return _Speed;
            }
            set
            {
                _Speed = value;
            }
        }

        #endregion
    }
}
