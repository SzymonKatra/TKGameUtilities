using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKGameUtilities.Graphics
{
    public class DisplayShader : Shader
    {
        #region Constructors
        protected DisplayShader(string vertexShaderSource, string fragmentShaderSource)
            : base(vertexShaderSource, fragmentShaderSource)
        {
            PreCacheDefaultShaderVariables();
        }
        #endregion

        #region Properties
        public const string UniformProjectionMatrix_DefaultShaderName = "ProjectionMatrix";
        public const string UniformModelViewMatrix_DefaultShaderName = "ModelViewMatrix";

        private int m_uniformProjectionMatrixLocation;
        public int UniformProjectionMatrixLocation
        {
            get { return m_uniformProjectionMatrixLocation; }
        }

        private int m_uniformModelViewMatrixLocation;
        public int UniformModelViewMatrixLocation
        {
            get { return m_uniformModelViewMatrixLocation; }
        }
        #endregion

        #region Methods
        private void PreCacheDefaultShaderVariables()
        {
            m_uniformProjectionMatrixLocation = GetUniformLocation(UniformProjectionMatrix_DefaultShaderName);
            m_uniformModelViewMatrixLocation = GetUniformLocation(UniformModelViewMatrix_DefaultShaderName);
        }
        #endregion
    }
}
