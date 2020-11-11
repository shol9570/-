Shader "SHOL/Lantern Light"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Intencity ("Intencity", Range(0,5)) = 1.2
        _Alpha ("Alpha", Range(0,1)) = 0.8
        _LightDistortionStrength("Light Distortion Strength", Range(0,1)) = 0.5
        _LightDistortionSpeed("Light Distortion Speed", Range(0.1, 5)) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass{
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM

            #pragma vertex vertLanternLight
            #pragma fragment fragLanternLight
            #pragma target 3.0
            #include "SHOLScannerEffect.cginc"
            
            ENDCG
        }        
    }
}
