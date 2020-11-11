Shader "SHOL/Spawn Effect"
{
    Properties
    {
        _MainTex ("Main texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _EffectColor ("Effect color", Color) = (1,1,1,1)
        _EffectTex ("Effect texture", 2D) = "white" {}
        _Blend ("Blend", Range(0,1)) = 0
        _Thickness ("Thickness", float) = 0.05
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent+1" "LightMode"="ForwardBase"
        }

        PASS
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float4 _EffectColor;
            sampler2D _EffectTex;
            float _Blend;
            float _Thickness;

            struct v2f
            {
                float4 position : SV_POSITION;
                fixed4 diff : COLOR0;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_base _in)
            {
                v2f o;
                o.position = UnityObjectToClipPos(_in.vertex);
                float3 worldNormal = UnityObjectToWorldDir(_in.normal);
                o.diff = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz)) * _LightColor0;
                o.diff.rgb += ShadeSH9(float4(worldNormal,1));
                o.uv = _in.texcoord;
                return o;
            }

            float4 frag(v2f _in) : COLOR
            {
                float4 texColor = tex2D(_EffectTex, _in.uv);
                texColor.rgb += _Blend;
                //texColor.rgb *= 0.5;
                if (texColor.r + texColor.g + texColor.b <= 3) clip(-1);
                float4 color = _EffectColor;
                if (texColor.r + texColor.g + texColor.b > 3 + _Thickness * 3 * (1 - _Blend))
                {
                    color = tex2D(_MainTex, _in.uv) * _Color;
                    color.rgb *= _in.diff;
                    //color.a = _Color.a;
                }
                return color;
            }
            ENDCG
        }
    }
}