#ifndef SHOL_SCANNER_EFFECT
#define SHOL_SCANNER_EFFECT

#include "UnityCG.cginc"

//Common
float4 _Color;
float _Intencity;
float _Alpha;

//Scanner
float _ScanSpeed;
float _ScanlineThickness;
float _HeightScan;
sampler2D _ScanlineTexture;
float4 _ScanlineColor;
sampler2D _ScanlineMask;
sampler2D _ScanlineDistortionTexture;
float _ForwardDistortion;

//LanternLight
float _LightDistortionStrength;
float _LightDistortionSpeed;

uniform float4 _Center = float4(0,0,0,0);
uniform float4 _Extends = float4(0.5,0.5,0.5,0);

struct v2f
{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float far : TEXCOORD1;
    float height : TEXCOORD2;
};

float DepthCoefficient(float _z)
{
    float min = _Center.z - _Extends.z;
    float max = _Center.z + _Extends.z;
    float coefficient;
    if (max - min != 0) coefficient = (_z - min) / (max - min);
    else coefficient = 0;
    return clamp(coefficient,0,1);
};

float HeightCoefficient(float _y)
{
    float min = _Center.y - _Extends.y;
    float max = _Center.y + _Extends.y;
    float coefficient;
    if (max - min != 0) coefficient = (_y - min) / (max - min);
    else coefficient = 0;
    return clamp(coefficient,0,1);
};

float SideCoefficient(float _x)
{
    float min = _Center.x - _Extends.x;
    float max = _Center.x + _Extends.x;
    float coefficient;
    if (max - min != 0) coefficient = (_x - min) / (max - min);
    else coefficient = 0;
    return clamp(coefficient,0,1);
};

v2f vert(appdata_base _in)
{
    v2f o;
    o.pos = UnityObjectToClipPos(_in.vertex);
    o.uv = _in.texcoord;
    o.far = cos(radians(DepthCoefficient(_in.vertex.z) * 90));
    o.height = HeightCoefficient(_in.vertex.y);
    return o;
}

half4 frag(v2f _in) : COLOR
{
    half4 c;
    c.rgb = _Color.rgb * _Intencity;

    if (_HeightScan)
    {
        float scanlineTextureColor = tex2D(_ScanlineTexture, (0, _in.height + frac(_Time * _ScanSpeed)));
        if (scanlineTextureColor.r < 0.5) c.rgb = _ScanlineColor.rgb;
    }

    float2 maskUV = _in.uv + float2(sin(frac(_Time.x) * 0.1f), 0) + tex2D(_ScanlineDistortionTexture, _in.uv + _Time.x).r;
    float xOffset = 1 - tex2D(_ScanlineMask, maskUV).r;
    float far = _in.far;
    if(_ForwardDistortion) far *= frac(_Time.x) * 0.1;
    c.a = _Alpha * _in.far * xOffset;
    return c;
}

v2f vertLanternLight(appdata_base _in)
{
    v2f o;
    float xOffset = (sin(frac(_Time.x / _LightDistortionSpeed) * 360) + 1) * 0.5;
    o.pos = UnityObjectToClipPos(_in.vertex * (1 + xOffset * _LightDistortionStrength));
    o.uv = _in.texcoord;
    float alphaOffset = cos(radians(DepthCoefficient(_in.vertex.z) * 90));
    o.far = alphaOffset;
    return o;
}

half4 fragLanternLight(v2f _in) : COLOR
{
    half4 c;
    c.rgb = _Color.rgb * _Intencity;
    
    c.a = _Alpha * _in.far;
    return c;
}

#endif
