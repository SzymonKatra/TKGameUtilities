using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace TKGameUtilities.Graphics
{
    public abstract class RenderTarget
    {
        #region Constructors
        public RenderTarget(Point2 size)
        {
            m_defaultCamera = new Camera2D();
            m_defaultCamera.Position = Vector2.Zero;
            m_defaultCamera.Rotation = 0;
            m_defaultCamera.Origin = Vector2.Zero;
            m_defaultCamera.Scale = Vector2.One;
            m_defaultCamera.Viewport = new Rectangle(Vector2.Zero, (Vector2)size);
            m_currentCamera = m_defaultCamera;

            m_defaultOrtho = Matrix4.CreateOrthographicOffCenter(0, size.X, size.Y, 0, 1.0f, -1.0f);
            m_projectionMatrix = m_defaultOrtho;
        }
        #endregion

        #region Properties
        private Camera2D m_defaultCamera;
        public Camera2D DefaultCamera
        {
            get { return m_defaultCamera; }
        }
        private Camera2D m_currentCamera;
        public Camera2D CurrentCamera
        {
            get { return m_currentCamera; }
            set
            {
                m_currentCamera = value;
                m_cameraNeedUpdate = true;
            }
        }
        private bool m_cameraNeedUpdate = true;

        private Matrix4 m_defaultOrtho;
        private Matrix4 m_projectionMatrix;
        #endregion

        #region Methods
        public void Clear()
        {
            Clear(Color.Black);
        }
        public void Clear(Color color)
        {
            Activate();

            GL.ClearColor(color.R / 1f, color.G / 1f, color.B / 1f, color.A / 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void PreDrawSetup(DisplayShader shader, BlendOptions blendOptions, Matrix4 transform, Texture texture, int textureMatrixLocation)
        {
            Activate();

            GL.BlendEquation(blendOptions.BlendEquation);
            GL.BlendFunc(blendOptions.BlendSource, blendOptions.BlendDestination);

            shader.Bind();

            GL.UniformMatrix4(shader.UniformProjectionMatrixLocation, false, ref m_projectionMatrix);
            if (textureMatrixLocation >= 0)
            {
                Matrix4 texMatrix = texture.TextureMatrix;
                GL.UniformMatrix4(textureMatrixLocation, false, ref texMatrix);
            }

            if (m_cameraNeedUpdate)
            {
                ApplyCameraGL(shader.UniformProjectionMatrixLocation);
                m_cameraNeedUpdate = false;
            }

            GL.UniformMatrix4(shader.UniformModelViewMatrixLocation, false, ref transform);

            if (textureMatrixLocation >= 0)
            {
                texture.Bind();

                Matrix4 texMatrix = texture.TextureMatrix;
                GL.UniformMatrix4(textureMatrixLocation, false, ref texMatrix);
            }
        }

        public void ResetCache()
        {
            m_cameraNeedUpdate = true;
        }

        public void ApplyCameraGL(DisplayShader shader)
        {
            GL.Viewport((int)m_currentCamera.Viewport.Position.X, (int)m_currentCamera.Viewport.Position.Y, (int)m_currentCamera.Viewport.Size.X, (int)m_currentCamera.Viewport.Size.Y);
            m_projectionMatrix = m_currentCamera.GetTransform() * m_defaultOrtho;
            GL.UniformMatrix4(shader.UniformProjectionMatrixLocation, false, ref m_projectionMatrix);
        }
        public void ApplyCameraGL(int uniformProjectionMatrixLocation)
        {
            GL.Viewport((int)m_currentCamera.Viewport.Position.X, (int)m_currentCamera.Viewport.Position.Y, (int)m_currentCamera.Viewport.Size.X, (int)m_currentCamera.Viewport.Size.Y);
            m_projectionMatrix = m_currentCamera.GetTransform() * m_defaultOrtho;
            GL.UniformMatrix4(uniformProjectionMatrixLocation, false, ref m_projectionMatrix);
        }

        public abstract void Display();
        public abstract void Activate();
        #endregion
    }
}
