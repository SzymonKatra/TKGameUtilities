using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace TKGameUtilities.Graphics
{
    public static class ContextManager
    {
        static ContextManager()
        {
            GraphicsContext.ShareContexts = true;
        }

        [ThreadStatic]
        private static GraphicsContext s_defaultContext;
        [ThreadStatic]
        private static INativeWindow s_defaultDummyNativeWindow;
        [ThreadStatic]
        private static GraphicsContext s_currentContext; //we don't use GraphicsContext.CurrentContext because it's slower than this

        public static void ActivateDefault()
        {
            if (s_currentContext != s_defaultContext || s_currentContext == null)
            {
                CreateDefaultContext();
                s_defaultContext.MakeCurrent(s_defaultDummyNativeWindow.WindowInfo);
                s_currentContext = s_defaultContext;
            }
        }
        public static void ActivateDefaultIfNoCurrent()
        {
            if (s_currentContext == null)
            {
                CreateDefaultContext();
                s_defaultContext.MakeCurrent(s_defaultDummyNativeWindow.WindowInfo);
                s_currentContext = s_defaultContext;
            }
        }
        public static void Activate(GraphicsContext context, IWindowInfo info)
        {
            if(s_currentContext != context)
            {
                context.MakeCurrent(info);
                s_currentContext = context;
            }
        }
        public static bool CheckIsCurrent(GraphicsContext context)
        {
            return (context == s_currentContext);
        }

        private static void CreateDefaultContext()
        {
            if (s_defaultContext == null)
            {
                s_defaultDummyNativeWindow = new NativeWindow();
                s_defaultContext = new GraphicsContext(GraphicsMode.Default, s_defaultDummyNativeWindow.WindowInfo);
                s_defaultContext.LoadAll();
            }
        }
    }
}
