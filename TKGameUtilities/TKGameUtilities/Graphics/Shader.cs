using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    [Serializable]
    public class ShaderException : Exception
    {
        public ShaderException(string message)
            : base(message)
        {
        }
    }

    public class Shader : IDisposable
    {
        #region Constructors
        public Shader(string vertexShaderSource, string fragmentShaderSource)
        {
            ContextManager.ActivateDefaultIfNoCurrent();

            m_uniformCache = new Dictionary<string, int>();
            m_attributeCache = new Dictionary<string, int>();

            m_GLprogramID = GL.CreateProgram();

            if (!string.IsNullOrEmpty(vertexShaderSource))
            {
                CreateShader(ShaderType.VertexShader, vertexShaderSource, m_GLprogramID, out m_GLvertexShaderID);
            }
            if (!string.IsNullOrEmpty(fragmentShaderSource))
            {
                CreateShader(ShaderType.FragmentShader, fragmentShaderSource, m_GLprogramID, out m_GLfragmentShaderID);
            }

            GL.LinkProgram(m_GLprogramID);
            int success = 0;
            GL.GetProgram(m_GLprogramID, GetProgramParameterName.LinkStatus, out success);
            if(success<=0)
            {
                throw new ShaderException("Failed to LINK shader");
            }
        }
        #endregion

        #region Properties
        private bool m_disposed = false;

        private int m_GLprogramID;
        public int GLProgramID
        {
            get { return m_GLprogramID; }
        }

        private int m_GLvertexShaderID;
        public int GLVertexShaderID
        {
            get { return m_GLvertexShaderID; }
        }

        private int m_GLfragmentShaderID;
        public int GLFragmentShaderID
        {
            get { return m_GLfragmentShaderID; }
        }

        private Dictionary<string, int> m_uniformCache;
        private Dictionary<string, int> m_attributeCache;
        #endregion

        #region Methods
        public void Bind()
        {
            GL.UseProgram(m_GLprogramID);
        }
        public static void Unbind()
        {
            GL.UseProgram(0);
        }

        public int GetUniformLocation(string name)
        {
            if(!m_uniformCache.ContainsKey(name))
            {
                m_uniformCache.Add(name, GL.GetUniformLocation(m_GLprogramID, name));
            }

            return m_uniformCache[name];
        }
        public int GetAttributeLocation(string name)
        {
            if(!m_attributeCache.ContainsKey(name))
            {
                m_attributeCache.Add(name, GL.GetAttribLocation(m_GLprogramID, name));
            }

            return m_attributeCache[name];
        }

        public string GetProgramLog()
        {
            return GL.GetProgramInfoLog(m_GLprogramID);
        }
        public string GetVertexShaderLog()
        {
            return GetShaderLog(m_GLvertexShaderID);
        }
        public string GetFragmentShaderLog()
        {
            return GetShaderLog(m_GLfragmentShaderID);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if(!m_disposed)
            {
                if(disposing)
                {
                }

                GL.DeleteProgram(m_GLprogramID);

                m_disposed = true;
            }
        }
        ~Shader()
        {
            Dispose(false);
        }

        ////////////////
        protected static void CreateShader(ShaderType type, string source, int programID, out int shaderID)
        {
            shaderID = GL.CreateShader(type);

            GL.ShaderSource(shaderID, source);

            GL.CompileShader(shaderID);

            int success = 0;
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out success);
            if (success <= 0)
            {
                throw new ShaderException("Failed to COMPILE shader. Source:\n" + source);
            }

            GL.AttachShader(programID, shaderID);
        }
        protected static string GetShaderLog(int shaderID)
        {
            if(GL.IsShader(shaderID))
            {
                return GL.GetShaderInfoLog(shaderID);
            }

            return string.Empty;
        }
        #endregion
    }
}
