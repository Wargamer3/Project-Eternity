using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.ParticleSystem
{
    public class ParticleSystem2DNoTexture
    {
        #region Fields

        public ParticleSettingsNoTexture settings;
        private Effect particleEffect;
        private EffectParameter effectViewProjectionParameter;
        private EffectParameter effectTimeParameter;
        private ParticleVertex[] particles;
        private DynamicVertexBuffer vertexBuffer;
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

        public int firstActiveParticle;
        public int firstNewParticle;
        public int firstFreeParticle;
        public int firstRetiredParticle;

        float currentTime;


        // Count how many times Draw has been called. This is used to know
        // when it is safe to retire old particles back into the free list.
        int drawCounter;

        public EffectParameterCollection parameters;

        private const int VertexPerModel = 3;
        private const int PrimitivesPerModel = 1;
        private const int IndexPerModel = VertexPerModel * PrimitivesPerModel;

        #endregion

        public ParticleSystem2DNoTexture(ParticleSettingsNoTexture settings)
        {
            this.settings = settings;
        }

        public void LoadContent(ContentManager Content, GraphicsDevice GraphicsDevice, Matrix Projection)
        {
            LoadContent(Content, GraphicsDevice, Projection, "Shaders/Particle shader");
        }

        public void LoadContent(ContentManager Content, GraphicsDevice GraphicsDevice, Matrix Projection, string ShaderPath)
        {
            // Allocate the particle array, and fill in the corner fields (which never change).
            particles = new ParticleVertex[settings.MaxParticles * VertexPerModel];

            for (int i = 0; i < settings.MaxParticles; i++)
            {
                particles[i * VertexPerModel + 0].UV = new Vector2(0, 0);
                particles[i * VertexPerModel + 1].UV = new Vector2(1, 0);
                particles[i * VertexPerModel + 2].UV = new Vector2(1, 1);
            }

            Effect effect = Content.Load<Effect>(ShaderPath);

            particleEffect = effect.Clone();

            parameters = particleEffect.Parameters;

            // Look up shortcuts for parameters that change every frame.
            effectViewProjectionParameter = parameters["ViewProjection"];
            effectTimeParameter = parameters["CurrentTime"];
            effectViewProjectionParameter.SetValue(Projection);

            // Set the values of parameters that do not change.
            parameters["Gravity"].SetValue(settings.Gravity);
            parameters["Duration"].SetValue((float)settings.DurationInSeconds);
            parameters["SpeedMultiplier"].SetValue(60f);
            parameters["StartingAlpha"].SetValue(settings.StartingAlpha);
            parameters["EndAlpha"].SetValue(settings.EndAlpha);

            parameters["Size"].SetValue(settings.Size);

            vertexBuffer = new DynamicVertexBuffer(GraphicsDevice, ParticleVertex.VertexDeclaration,
                                                   settings.MaxParticles * VertexPerModel, BufferUsage.WriteOnly);

            ushort[] indices = new ushort[settings.MaxParticles * IndexPerModel];

            for (int i = 0; i < settings.MaxParticles; i++)
            {
                indices[i * IndexPerModel + 0] = (ushort)(i * VertexPerModel + 0);
                indices[i * IndexPerModel + 1] = (ushort)(i * VertexPerModel + 1);
                indices[i * IndexPerModel + 2] = (ushort)(i * VertexPerModel + 2);
            }

            indexBuffer = new IndexBuffer(GraphicsDevice, typeof(ushort), indices.Length, BufferUsage.WriteOnly);

            indexBuffer.SetData(indices);
        }

        public void Update(double ElapsedTotalSeconds)
        {
            currentTime += (float)ElapsedTotalSeconds;

            RetireActiveParticles();
            FreeRetiredParticles();

            // If we let our timer go on increasing for ever, it would eventually
            // run out of floating point precision, at which point the particles
            // would render incorrectly. An easy way to prevent this is to notice
            // that the time value doesn't matter when no particles are being drawn,
            // so we can reset it back to zero any time the active queue is empty.

            if (firstActiveParticle == firstFreeParticle)
                currentTime = 0;

            if (firstRetiredParticle == firstActiveParticle)
                drawCounter = 0;
        }

        /// <summary>
        /// Helper for checking when active particles have reached the end of
        /// their life. It moves old particles from the active area of the queue
        /// to the retired section.
        /// </summary>
        void RetireActiveParticles()
        {
            float particleDuration = (float)settings.DurationInSeconds;

            while (firstActiveParticle != firstNewParticle)
            {
                // Is this particle old enough to retire?
                // We multiply the active particle index by four, because each
                // particle consists of a quad that is made up of four vertices.
                float particleAge = currentTime - particles[firstActiveParticle * VertexPerModel].Time;

                if (particleAge < particleDuration)
                    break;

                // Remember the time at which we retired this particle.
                particles[firstActiveParticle * VertexPerModel].Time = drawCounter;

                // Move the particle from the active to the retired queue.
                firstActiveParticle++;

                if (firstActiveParticle >= settings.MaxParticles)
                    firstActiveParticle = 0;
            }
        }

        /// <summary>
        /// Helper for checking when retired particles have been kept around long
        /// enough that we can be sure the GPU is no longer using them. It moves
        /// old particles from the retired area of the queue to the free section.
        /// </summary>
        void FreeRetiredParticles()
        {
            while (firstRetiredParticle != firstActiveParticle)
            {
                // Has this particle been unused long enough that
                // the GPU is sure to be finished with it?
                // We multiply the retired particle index by four, because each
                // particle consists of a quad that is made up of four vertices.
                int age = drawCounter - (int)particles[firstRetiredParticle * VertexPerModel].Time;

                // The GPU is never supposed to get more than 2 frames behind the CPU.
                // We add 1 to that, just to be safe in case of buggy drivers that
                // might bend the rules and let the GPU get further behind.
                if (age < 3)
                    break;

                // Move the particle from the retired to the free queue.
                firstRetiredParticle++;

                if (firstRetiredParticle >= settings.MaxParticles)
                    firstRetiredParticle = 0;
            }
        }

        public void Draw(GraphicsDevice GraphicsDevice, Vector2 Camera)
        {
            parameters["Camera"].SetValue(Camera);

            GraphicsDevice device = GraphicsDevice;

            // Restore the vertex buffer contents if the graphics device was lost.
            if (vertexBuffer.IsContentLost)
            {
                vertexBuffer.SetData(particles);
            }

            // If there are any particles waiting in the newly added queue,
            // we'd better upload them to the GPU ready for drawing.
            if (firstNewParticle != firstFreeParticle)
            {
                AddNewParticlesToVertexBuffer();
            }

            // If there are any active particles, draw them now!
            if (firstActiveParticle != firstFreeParticle)
            {
                device.BlendState = settings.BlendState;
                device.DepthStencilState = DepthStencilState.DepthRead;
                
                // Set an effect parameter describing the current time. All the vertex
                // shader particle animation is keyed off this value.
                effectTimeParameter.SetValue(currentTime);

                // Set the particle vertex and index buffer.
                device.SetVertexBuffer(vertexBuffer);
                device.Indices = indexBuffer;

                // Activate the particle effect.
                foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    if (firstActiveParticle < firstFreeParticle)
                    {
                        // If the active particles are all in one consecutive range,
                        // we can draw them all in a single call.
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     firstActiveParticle * VertexPerModel, (firstFreeParticle - firstActiveParticle) * VertexPerModel,
                                                     firstActiveParticle * IndexPerModel, (firstFreeParticle - firstActiveParticle) * PrimitivesPerModel);
                    }
                    else
                    {
                        // If the active particle range wraps past the end of the queue
                        // back to the start, we must split them over two draw calls.
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     firstActiveParticle * VertexPerModel, (settings.MaxParticles - firstActiveParticle) * VertexPerModel,
                                                     firstActiveParticle * IndexPerModel, (settings.MaxParticles - firstActiveParticle) * PrimitivesPerModel);

                        if (firstFreeParticle > 0)
                        {
                            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                         0, firstFreeParticle * VertexPerModel,
                                                         0, firstFreeParticle * PrimitivesPerModel);
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
        void AddNewParticlesToVertexBuffer()
        {
            int stride = ParticleVertex.SizeInBytes;

            if (firstNewParticle < firstFreeParticle)
            {
                // If the new particles are all in one consecutive range,
                // we can upload them all in a single call.
                vertexBuffer.SetData(firstNewParticle * stride * VertexPerModel, particles,
                                     firstNewParticle * VertexPerModel,
                                     (firstFreeParticle - firstNewParticle) * VertexPerModel,
                                     stride, SetDataOptions.NoOverwrite);
            }
            else
            {
                // If the new particle range wraps past the end of the queue
                // back to the start, we must split them over two upload calls.
                vertexBuffer.SetData(firstNewParticle * stride * VertexPerModel, particles,
                                     firstNewParticle * VertexPerModel,
                                     (settings.MaxParticles - firstNewParticle) * VertexPerModel,
                                     stride, SetDataOptions.NoOverwrite);

                if (firstFreeParticle > 0)
                {
                    vertexBuffer.SetData(0, particles,
                                         0, firstFreeParticle * VertexPerModel,
                                         stride, SetDataOptions.NoOverwrite);
                }
            }

            // Move the particles we just uploaded from the new to the active queue.
            firstNewParticle = firstFreeParticle;
        }

        public void AddParticle(Vector2 position, Vector2 velocity)
        {
            // Figure out where in the circular queue to allocate the new particle.
            int nextFreeParticle = firstFreeParticle + 1;

            if (nextFreeParticle >= settings.MaxParticles)
                nextFreeParticle = 0;

            // If there are no free particles, we just have to give up.
            if (nextFreeParticle == firstRetiredParticle)
                return;

            // Fill in the particle vertex structure.
            for (int i = 0; i < VertexPerModel; i++)
            {
                particles[firstFreeParticle * VertexPerModel + i].Time = currentTime;
				particles[firstFreeParticle * VertexPerModel + i].Speed = velocity;
				particles[firstFreeParticle * VertexPerModel + i].MinScale = settings.MinScale;
                particles[firstFreeParticle * VertexPerModel + i].Position = position;
            }

            firstFreeParticle = nextFreeParticle;
        }

        public void ClearParticles()
        {
            firstActiveParticle = 0;
            firstNewParticle = 0;
            firstFreeParticle = 0;
            firstRetiredParticle = 0;
            currentTime = 0;
            drawCounter = 0;
        }
    }
}
