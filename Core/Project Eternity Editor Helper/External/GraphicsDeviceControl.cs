using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Editor
{
    using Color = System.Drawing.Color;

    /// <summary>
    /// WinForms control that draws with the shared MonoGame device.
    /// Derived classes override Initialize and Draw.
    /// </summary>
    public class GraphicsDeviceControl : Control
    {
        private GraphicsDeviceService graphicsDeviceService;
        private SwapChainRenderTarget swapChain;
        private bool IsInit = false;
        private ServiceContainer services = new ServiceContainer();

        private bool IsDesignTime => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDeviceService.GraphicsDevice; }
        }

        public ServiceContainer Services
        {
            get { return services; }
        }

        protected override void OnCreateControl()
        {
            if (!IsDesignTime && !IsInit)
            {
                IsInit = true;
                graphicsDeviceService = GraphicsDeviceService.AddRef(Handle, ClientSize.Width, ClientSize.Height);
                services.AddService<IGraphicsDeviceService>(graphicsDeviceService);
                Initialize();
            }

            base.OnCreateControl();
        }

        public void Reset()
        {
            EnsureSwapChain(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (swapChain != null)
            {
                swapChain.Dispose();
                swapChain = null;
            }

            if (graphicsDeviceService != null)
            {
                graphicsDeviceService.Release(disposing);
                graphicsDeviceService = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (graphicsDeviceService == null)
            {
                PaintUsingSystemDrawing(e.Graphics, Text + "\n\n" + GetType());
                return;
            }

            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
            {
                return;
            }

            try
            {
                EnsureSwapChain(false);
                GraphicsDevice.SetRenderTarget(swapChain);
                GraphicsDevice.Viewport = new Viewport(0, 0, ClientSize.Width, ClientSize.Height);
                Draw();
            }
            catch (Exception DrawError)
            {
                GraphicsDevice.SetRenderTarget(null);
                PaintUsingSystemDrawing(e.Graphics, "Draw failed\n\n" + DrawError.Message);
                return;
            }

            GraphicsDevice.SetRenderTarget(null);

            try
            {
                swapChain.Present();
            }
            catch
            {
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected virtual void PaintUsingSystemDrawing(System.Drawing.Graphics graphics, string text)
        {
            graphics.Clear(Color.CornflowerBlue);

            using (Brush brush = new SolidBrush(Color.Black))
            {
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    graphics.DrawString(text, Font, brush, ClientRectangle, format);
                }
            }
        }

        protected virtual void Initialize() { }

        protected virtual void Draw() { }

        // SetRenderTarget(null) leaves the shared device back buffer. Editor draws have to return to this control.
        protected void SetRenderTargetToControl()
        {
            if (swapChain == null)
            {
                return;
            }

            GraphicsDevice.SetRenderTarget(swapChain);
            GraphicsDevice.Viewport = new Viewport(0, 0, Math.Max(ClientSize.Width, 1), Math.Max(ClientSize.Height, 1));
        }

        private void EnsureSwapChain(bool Force)
        {
            int Width = Math.Max(ClientSize.Width, 1);
            int Height = Math.Max(ClientSize.Height, 1);
            if (!Force && swapChain != null && !swapChain.IsDisposed && swapChain.Width == Width && swapChain.Height == Height)
            {
                return;
            }

            if (swapChain != null)
            {
                swapChain.Dispose();
                swapChain = null;
            }

            swapChain = new SwapChainRenderTarget(GraphicsDevice, Handle, Width, Height, false,
                SurfaceFormat.Color, DepthFormat.Depth24, 0,
                RenderTargetUsage.DiscardContents, PresentInterval.Immediate);
        }
    }
}
