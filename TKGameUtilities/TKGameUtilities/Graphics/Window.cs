using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Platform;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    public class Window : RenderTarget, IDisposable
    {
        #region Constructors
        public Window(Point2 size, string title, bool fullScreen, GraphicsMode graphicsMode)
            : base(size)
        {
            //GraphicsMode graphicsMode = new GraphicsMode(new ColorFormat(settings.BitsPerPixel), settings.DepthBufferBits,settings.StencilBufferBits, settings.Antialiasing);
            m_nativeWindow = new NativeWindow(size.X,
                                              size.Y,
                                              title,
                                              (fullScreen ? GameWindowFlags.Fullscreen : GameWindowFlags.Default),
                                              graphicsMode,
                                              DisplayDevice.Default);

            m_windowContext = new GraphicsContext(graphicsMode, m_nativeWindow.WindowInfo);
            m_windowContext.LoadAll();

            ContextManager.Activate(m_windowContext, m_nativeWindow.WindowInfo);

            m_exists = true;
            m_nativeWindow.Closed += (s, e) => { m_exists = false; };

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }
        #endregion

        #region Properties
        private bool m_disposed = false;
        private NativeWindow m_nativeWindow;
        private GraphicsContext m_windowContext;

        public bool m_exists = false;
        public bool Exists
        {
            get { return m_exists; }
        }

        public bool Visible
        {
            get { return m_nativeWindow.Visible; }
            set { m_nativeWindow.Visible = value; }
        }
        public string Title
        {
            get { return m_nativeWindow.Title; }
            set { m_nativeWindow.Title = value; }
        }
        public Point2 Size
        {
            get { return new Point2(m_nativeWindow.Width, m_nativeWindow.Height); }
            set
            {
                m_nativeWindow.Width = value.X;
                m_nativeWindow.Height = value.Y;
            }
        }
        #endregion

        #region Methods
        public void Show()
        {
            m_nativeWindow.Visible = true;
        }
        public void Hide()
        {
            m_nativeWindow.Visible = false;
        }
        public void Close()
        {
            m_nativeWindow.Close();
        }
        public void ProcessEvents()
        {
            m_nativeWindow.ProcessEvents();
        }

        public override void Display()
        {
            Activate();

            if (m_exists) m_windowContext.SwapBuffers();
        }
        public override void Activate()
        {
            ContextManager.Activate(m_windowContext, m_nativeWindow.WindowInfo);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    m_nativeWindow.Dispose();
                    m_windowContext.Dispose();
                }

                m_disposed = true;
            }
        }
        ~Window()
        {
            Dispose(false);
        }
        #endregion
    }
}
