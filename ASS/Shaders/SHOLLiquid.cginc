#ifndef SHOL_LIQUID_INCLUDED
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members lightDir)
#pragma exclude_renderers d3d11
#define SHOL_LIQUID_INCLUDED
#include <Lighting.cginc>
#include <UnityCG.cginc>
#include <UnityInstancing.cginc>

float4 _Color;

float3 _PlanePosition;
float3 _PlaneNormal;

struct v2f
{
    float4 pos : SV_POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
    float3 localPos : TEXCOORD1;
    //float3 lightDir : TEXCOORD2;
};

bool IsAtFront(float3 _vertPos)
{
    float3 vert2PlaneDir = normalize(_vertPos - _PlanePosition);
    float dotRes = dot(_PlaneNormal, vert2PlaneDir);
    return dotRes > 0;
};

// inline half4 DefaultLight(half4 _c, v2f _in)
// {
//     half4 c = _c;
//     float lightDot = dot(_in.normal, _in.lightDir);
//     c.rgb *= (lightDot + 1) * 0.1 + 0.8;
//     return c;
// }

v2f normalVert(appdata_full _in)
{
    v2f o;
    o.pos = UnityObjectToClipPos(_in.vertex);
    o.localPos = _in.vertex;
    o.normal = UnityObjectToWorldNormal(_in.normal);
    o.uv = _in.texcoord;
    //o.lightDir = ObjSpaceLightDir(_in.vertex);
    return o;
}

half4 normalFrag(v2f _in) : COLOR
{
    if(IsAtFront(_in.localPos)) discard;
    half4 c;
    c.rgb = _Color.rgb;
    c.a = _Color.a;
    return c; //DefaultLight(c, _in);
}

v2f maskVert(appdata_full _in)
{
    v2f o;
    o.pos = UnityObjectToClipPos(_in.vertex);
    o.localPos = _in.vertex;
    o.normal = UnityObjectToWorldNormal(_in.normal);
    o.uv = _in.texcoord;
    return o;
}

half4 maskFrag(v2f _in) : COLOR
{
    if(IsAtFront(_in.localPos)) discard;
    return half4(0,0,0,0);
}

v2f normalTopSurfVert(appdata_full _in)
{
    v2f o;
    o.pos = UnityObjectToClipPos(_in.vertex);
    o.normal = UnityObjectToWorldNormal(_PlaneNormal);
    o.uv = _in.texcoord;
    //o.lightDir = ObjSpaceLightDir(_in.vertex);
    return o;
}

half4 normalTopSurfFrag(v2f _in) : COLOR
{
    half4 c;
    c.rgb = _Color.rgb;
    c.a = _Color.a;
    return c; //DefaultLight(c, _in);
}

#endif //SHOL_LIQUID_INCLUDED