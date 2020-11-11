Shader "SHOL/S_FurShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _FurLength ("Fur Length", Range(0.0002, 1)) = 0.25
        _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
        _CutoffEnd("Alpha Cutoff end", Range(0, 1)) = 0.4
        _EdgeFade("Edge Fade", Range(0,1)) = 0.4
        _Gravity("Gravity Direction", Vector) = (0,0,1,0)
        _GravityStrength("Gravity Strength", Range(0, 1)) = 0.25
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.05
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.1
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.15
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.2
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.25
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.3
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.35
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.4
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.45
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.5
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.55
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.6
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.65
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.7
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.75
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.8
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.85
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.9
        #include "CGINC_Fur.cginc"
        ENDCG
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
        #define FUR_MULTIPLIER 0.95
        #include "CGINC_Fur.cginc"
        ENDCG
    }
    FallBack "Diffuse"
}
