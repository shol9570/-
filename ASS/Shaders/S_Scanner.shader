Shader "SHOL/Scanner"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Intencity ("Intencity", Range(0,5)) = 1.2
        _ScanSpeed ("ScanSpeed", float) = 5
        _Alpha ("Alpha", Range(0,1)) = 0.8
        _ScanlineThickness ("Scanline Thickness", float) = 0.1
        [MaterialToggle] _HeightScan("Enable height scanning", Range(0,1)) = 1
        _ScanlineTexture ("Scanline Texture", 2D) = "white" {}
        _ScanlineColor ("Scanline Color", Color) = (1,1,1,1)
        _ScanlineMask ("Scanline Mask", 2D) = "black" {}
        _ScanlineDistortionTexture ("Distortion Texture", 2D) = "black" {}
        [MaterialToggle] _ForwardDistortion("Enable forward distortion", Range(0,1)) = 1
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

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "SHOLScannerEffect.cginc"
            
            ENDCG
        }        
    }
}
