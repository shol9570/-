Shader "SHOL/Fracture Decal"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main texture", 2D) = "white" {}
        _DepthMap ("Depth map", 2D) = "" {}
        _DepthStrength("Depth strength", Range(0, 1)) = 0.5
        _Fade("Main texture strength", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags{ "Queue" = "Transparent+1" "RenderType" = "Transparent" }
        Pass{
            
            Blend SrcAlpha OneMinusSrcAlpha
            //Blend DstColor DstColor

            Stencil{
                Ref 5
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata{
                float4 position : POSITION;
            };

            struct v2f{
                float4 position : SV_POSITION;
            };

            v2f vert(appdata _in)
            {
                v2f o;
                o.position = UnityObjectToClipPos(_in.position);
                return o;
            }

            half4 frag(v2f _in) : COLOR {
                half4 c = half4(1,1,1,0);
                return c;
            }
            
            ENDCG
        }

        Pass
        {
            Cull Back
            ZTest Always
            //Blend DstColor OneMinusDstColor
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil{
                Ref 5
                Comp Equal
                Pass Zero
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "SHOLFractureDecal.cginc"
            
            ENDCG
        }
    }
}
