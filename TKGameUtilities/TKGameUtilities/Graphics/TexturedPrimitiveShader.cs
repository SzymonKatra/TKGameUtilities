using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    public class TexturedPrimitiveShader : ColoredPrimitiveShader
    {
        #region Constructors
        public TexturedPrimitiveShader()
            : this(s_vertexShaderSource, s_fragmentShaderSource)
        {
        }
        protected TexturedPrimitiveShader(string vertexShaderSource, string fragmentShaderSource)
            : base(vertexShaderSource, fragmentShaderSource)
        {
            PreCacheDefaultShaderVariables();
        }
        #endregion

        #region Properties
        public const string UniformTextureMatrix_DefaultShaderName = "GU_TextureMatrix";
        public const string UniformTexture_DefaultShaderName = "GU_Texture";
        public const string AttributeVertexTexCoords_DefaultShaderName = "GU_VertexTexCoords";
        public const string VaryingTexCoords_DefaultShaderName = "GU_TexCoords";

        #region VertexShaderSource
        //Vertex shader string format - names:
        // {0} - (mat4, uniform) projection matrix
        // {1} - (mat4, uniform) modelview matrix
        // {2} - (vec2, attribute) vertex position
        // {3} - (vec4, attribute) vertex color
        // {4} - (vec2, attribute) vertex tex coords
        // {5} - (vec4, varying) color
        // {6} - (vec2, varying) tex coords
        private static string s_vertexShaderSource = string.Format(@"
#version 120

uniform mat4 {0};
uniform mat4 {1};

attribute vec2 {2};
attribute vec4 {3};
attribute vec2 {4};

varying vec4 {5};
varying vec2 {6};

void main()
{{
    {5} = {3};
    {6} = {4};

    gl_Position = {0} * {1} * vec4({2}.x, {2}.y, 0.0, 1.0);
}}
",      UniformProjectionMatrix_DefaultShaderName,
        UniformModelViewMatrix_DefaultShaderName,
        AttributeVertexPosition_DefaultShaderName,
        AttributeVertexColor_DefaultShaderName,
        AttributeVertexTexCoords_DefaultShaderName,
        VaryingVertexColor_DefaultShaderName,
        VaryingTexCoords_DefaultShaderName);
        #endregion
        #region FragmentShaderSource
        //Fragment shader string format - names:
        // {0} - (mat4, uniform) texture matrix
        // {1} - (sampler2D, uniform) texture
        // {2} - (vec4, varying) color
        // {3} - (vec2, varying) tex coords
        private static readonly string s_fragmentShaderSource = string.Format(@"
#version 120

uniform mat4 {0};
uniform sampler2D {1};

varying vec4 {2};
varying vec2 {3};

void main()
{{
    gl_FragColor = texture2D({1}, ({0} * vec4({3}.x, {3}.y, 0.0, 0.0)).st) * {2};
}}
",      UniformTextureMatrix_DefaultShaderName,
        UniformTexture_DefaultShaderName,
        VaryingVertexColor_DefaultShaderName,
        VaryingTexCoords_DefaultShaderName);
        #endregion

        private int m_uniformTextureMatrixLocation;
        public int UniformTextureMatrixLocation
        {
            get { return m_uniformTextureMatrixLocation; }
        }

        private int m_uniformTextureLocation;
        public int UniformTextureLocation
        {
            get { return m_uniformTextureLocation; }
        }

        private int m_attributeVertexTexCoordsLocation;
        public int AttributeVertexTexCoordsLocation
        {
            get { return m_attributeVertexTexCoordsLocation; }
        }
        #endregion

        #region Methods
        private void PreCacheDefaultShaderVariables()
        {
            m_uniformTextureMatrixLocation = GetUniformLocation(UniformTextureMatrix_DefaultShaderName);
            m_uniformTextureLocation = GetUniformLocation(UniformTexture_DefaultShaderName);
            m_attributeVertexTexCoordsLocation = GetAttributeLocation(AttributeVertexTexCoords_DefaultShaderName);
        }
        #endregion
    }
}
