// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SHOL/Heat Effect"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Heat ("Heat", Range(0,1)) = 0
        _Beat ("Beat", Range(1,100)) = 20

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Overlay"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                half2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            float _Heat;
            float _Beat;
            float _UseUIAlphaClip;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                #ifdef UNITY_HALF_TEXEL_OFFSET
                 OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
                #endif
                OUT.color = IN.color * _Color;
                return OUT;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
                if (_UseUIAlphaClip) clip(color.a - 0.01);
                float orinOffset = 0;
                orinOffset = asin(color.r) / (UNITY_PI/2) + asin(color.b) / (UNITY_PI/2);
                orinOffset = orinOffset + _Heat * (0.95 + sin(radians(frac(_Time) * 180 * _Beat)) * 0.05);
                half3 heat = 0;
                if(orinOffset > 0.9)
                {
                    heat.r = 1;
                    heat.g = sin(radians((orinOffset - 0.9) / 0.1 * 90));
                    heat.b = sin(radians((orinOffset - 0.9) / 0.1 * 90));
                }
                else if(orinOffset > 0.7)
                {
                    heat.r = 1 - sin(radians((orinOffset - 0.7) / 0.2 * 90));
                }
                else if(orinOffset > 0.5)
                {
                    heat.r = sin(radians((orinOffset - 0.5) / 0.2 * 90));
                    heat.g = 1 - sin(radians((orinOffset - 0.5) / 0.2 * 90));
                }
                else if(orinOffset > 0.25)
                {
                    heat.g = sin(radians((orinOffset - 0.25) / 0.25 * 90));
                    heat.b = 1 - sin(radians((orinOffset - 0.25) / 0.25 * 90));
                }
                else
                {
                    heat.b = sin(radians(orinOffset / 0.25 * 90));
                }
                //heat.r = sin(radians(90 * max(0, orinOffset - 0.66) / 0.33));
                //heat.g = sin(radians(90 * max(0, orinOffset - 0.33) / 0.66));
                //heat.b = sin(radians(90 * orinOffset));

                color.rgb = heat.rgb;
                //color.rgb = IN.color.rgb;
                return color;//half4(orinOffset,orinOffset,orinOffset,1);
            }
            ENDCG
        }
    }
}