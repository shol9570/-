/*
 * Author : SHOL
 * Usage : Express fracture effect
 * Decription : Blend fracture textures color and edit height of vertices.
 *              Texture must be made in grayscale, and maximum texture count is 10 per each objects.
 * Notice : It is not using for decal. It just edit vertex height and make dark.
 *          Better with tessellation
 */

#ifndef SHOL_FRACTURE_INCLUDED
#define SHOL_FRACTURE_INCLUDED
#include <UnityInstancing.cginc>
//#include <Tessellation.cginc>

uniform sampler2D _FractureTex;
uniform float _FractureDepth = 0.0; //Fracture texture height depth strength

//Get fracture texture color with uv
inline float4 GetFractureTextureColor(float2 _uv, sampler2D _tex)
{
    float4 color = tex2Dlod(_tex, float4(_uv, 0, 0));
    return color;
}

//Convert color to grayscaled color
inline float4 GetGrayscaledColor(float4 _c)
{
    float average = (_c.r + _c.g + _c.b) / 3;
    return (average, average, average, _c.a);
}

//Get depth from color
inline float GetDepthFromColor(float4 _c)
{
    float4 grayscale = GetGrayscaledColor(_c);
    float depth = (1 - grayscale.r) * _c.a * _FractureDepth;
    return depth;
}

//Get fracture textures' blending result
inline float4 GetFractureColor(float2 _uv)
{
    if(_FractureDepth == 0) return (0,0,0,0);
    float4 fractureColor = GetFractureTextureColor(_uv, _FractureTex);
    return fractureColor;
}

//Get fracture vertex position with uv value
inline float3 GetVertexFracturePosition(float2 _uv, float3 _normal, float3 _pos)
{
    float4 color = GetFractureColor(_uv);
    float depth = GetDepthFromColor(color);
    float3 pos = _pos - _normal * depth;
    return pos;
}

//Get fracture vertex position with color value
// inline float3 GetVertexFracturePosition(float4 _color, float3 _normal, float3 _pos)
// {
//     float depth = GetDepthFromColor(_color);
//     float3 pos = _pos - _normal * depth;
//     return pos;
// }

// float4 tessDist(appdata_full v0, appdata_full v1, appdata_full v2)
// {
//     float minDist = 10.0;
//     float maxDist = 25.0;
//     return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, 4);
// }

#endif //SHOL_FRACTURE_INCLUDED