using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKGameUtilities.Graphics
{
    public class ColoredPrimitiveShader : DisplayShader
    {
        #region Constructors
        public ColoredPrimitiveShader()
            : this(s_vertexShaderSource, s_fragmentShaderSource)
        {
        }
        protected ColoredPrimitiveShader(string vertexShaderSource, string fragmentShaderSource)
            : base(vertexShaderSource, fragmentShaderSource)
        {
            PreCacheDefaultShaderVariables();
        }
        #endregion

        #region Properties
        public const string AttributeVertexPosition_DefaultShaderName = "VertexPosition";
        public const string AttributeVertexColor_DefaultShaderName = "VertexColor";
        public const string VaryingVertexColor_DefaultShaderName = "Color";

        #region VertexShaderSource
        //Vertex shader string format - names:
        // {0} - (mat4, uniform) projection matrix
        // {1} - (mat4, uniform) modelview matrix
        // {2} - (vec2, attribute) vertex position
        // {3} - (vec4, attribute) vertex color
        // {4} - (vec4, varying) color
        private static string s_vertexShaderSource = string.Format(@"
#version 120

uniform mat4 {0};
uniform mat4 {1};

attribute vec2 {2};
attribute vec4 {3};

varying vec4 {4};

void main()
{{
    {4} = {3};

    gl_Position = {0} * {1} * vec4({2}.x, {2}.y, 0.0, 1.0);
}}
",      UniformProjectionMatrix_DefaultShaderName,
        UniformModelViewMatrix_DefaultShaderName,
        AttributeVertexPosition_DefaultShaderName,
        AttributeVertexColor_DefaultShaderName,
        VaryingVertexColor_DefaultShaderName);
        #endregion
        #region FragmentShaderSource
        //Fragment shader string format - names:
        // {0} - (vec4, varying) color
        private static readonly string s_fragmentShaderSource = string.Format(@"
#version 120

varying vec4 {0};

void main()
{{
    gl_FragColor = {0};
}}
",      VaryingVertexColor_DefaultShaderName);
        #endregion

        private int m_attributeVertexPositionLocation;
        public int AttributeVertexPositionLocation
        {
            get { return m_attributeVertexPositionLocation; }
        }

        private int m_attributeVertexColorLocation;
        public int AttributeVertexColorLocation
        {
            get { return m_attributeVertexColorLocation; }
        }
        #endregion

        #region Methods
        private void PreCacheDefaultShaderVariables()
        {
            m_attributeVertexPositionLocation = GetAttributeLocation(AttributeVertexPosition_DefaultShaderName);
            m_attributeVertexColorLocation = GetAttributeLocation(AttributeVertexColor_DefaultShaderName);
        }
        #endregion
    }
}
