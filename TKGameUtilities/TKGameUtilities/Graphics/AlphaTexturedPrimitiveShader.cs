using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKGameUtilities.Graphics
{
    public class AlphaTexturedPrimitiveShader : TexturedPrimitiveShader
    {
        #region Constructors
        public AlphaTexturedPrimitiveShader()
            : this(s_vertexShaderSource, s_fragmentShaderSource)
        {
        }
        protected AlphaTexturedPrimitiveShader(string vertexShaderSource, string fragmentShaderSource)
            : base(vertexShaderSource, fragmentShaderSource)
        {
        }
        #endregion

        #region Properties
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
    gl_FragColor = vec4(1.0, 1.0, 1.0, texture2D({1}, ({0} * vec4({3}.x, {3}.y, 0.0, 0.0)).st).a) * {2};
}}
",      UniformTextureMatrix_DefaultShaderName,
        UniformTexture_DefaultShaderName,
        VaryingVertexColor_DefaultShaderName,
        VaryingTexCoords_DefaultShaderName);
        #endregion
        #endregion
    }
}
