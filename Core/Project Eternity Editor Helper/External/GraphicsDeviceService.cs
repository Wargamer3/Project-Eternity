using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

#pragma warning disable 67

namespace ProjectEternity.Core.Editor
{
    /// <summary>
    /// One shared MonoGame device for every editor viewport. Each control presents
    /// through its own swap chain. MonoGame has no GraphicsDevice.Present overload
    /// that targets a WinForms handle.
    /// </summary>
    public class GraphicsDeviceService : IGraphicsDeviceService
    {
        static GraphicsDeviceService singletonInstance;
        static int referenceCount;

        Form DeviceWindow;
        GraphicsDevice graphicsDevice;
        PresentationParameters parameters;

        GraphicsDeviceService()
        {
            DeviceWindow = new Form
            {
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.None,
                ClientSize = new System.Drawing.Size(1, 1)
            };

            // The hidden window stays 1x1 and is never presented. Weather, lightning, and glow
            // size their render targets from this back buffer. Glow downsamples by 4, so a 1x1
            // buffer becomes a 0-wide texture and Stage 1 rain fails to load. 640x480 matches
            // Constants.Width/Height and BattleMap.MapRenderTarget.
            parameters = new PresentationParameters
            {
                DeviceWindowHandle = DeviceWindow.Handle,
                BackBufferWidth = 640,
                BackBufferHeight = 480,
                BackBufferFormat = SurfaceFormat.Color,
                DepthStencilFormat = DepthFormat.Depth24,
                PresentationInterval = PresentInterval.Immediate,
                IsFullScreen = false
            };

            graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, parameters);
        }

        public static GraphicsDeviceService AddRef(IntPtr windowHandle, int width, int height)
        {
            if (Interlocked.Increment(ref referenceCount) == 1)
            {
                singletonInstance = new GraphicsDeviceService();
            }

            return singletonInstance;
        }

        public void Release(bool disposing)
        {
            if (Interlocked.Decrement(ref referenceCount) == 0)
            {
                if (disposing)
                {
                    if (DeviceDisposing != null)
                    {
                        DeviceDisposing(this, EventArgs.Empty);
                    }

                    graphicsDevice.Dispose();
                    DeviceWindow.Dispose();
                }

                graphicsDevice = null;
                DeviceWindow = null;
                singletonInstance = null;
            }
        }

        /// <summary>
        /// Kept so existing callers still compile. Resizing a viewport recreates that
        /// control's swap chain and must not reset the shared device.
        /// </summary>
        public void ResetDevice(int width, int height)
        {
            if (parameters == null)
            {
                return;
            }

            parameters.BackBufferWidth = Math.Max(parameters.BackBufferWidth, width);
            parameters.BackBufferHeight = Math.Max(parameters.BackBufferHeight, height);
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
        }

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;
    }
}
