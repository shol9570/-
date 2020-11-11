Shader "SHOL/Liquid"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _PlanePosition ("Plane Position", Vector) = (0,0,0,1)
        _PlaneNormal ("Plane Normal", Vector) = (0,1,0,0)
    }
    SubShader
    {
        PASS
        {
            Tags { "RenderType"="Opaque" "Queue"="Geometry" }
            LOD 200

            Stencil{
                Ref 50
                CompBack Always
                PassBack Replace
            }

            Cull front

            CGPROGRAM
            
            #pragma vertex maskVert
            #pragma fragment maskFrag
            #pragma target 3.0
            #include "SHOLLiquid.cginc"
            
            ENDCG
        }

        PASS
        {
            Tags { "RenderType"="Opaque" "Queue"="Geometry + 1" }
            LOD 200

            Stencil{
                Ref 50
                Comp Always
                Pass Zero
            }

            Cull back

            CGPROGRAM
            
            #pragma vertex normalVert
            #pragma fragment normalFrag
            #pragma target 3.0
            #include "SHOLLiquid.cginc"
            
            ENDCG
        }

        PASS
        {
            Tags { "RenderType"="Opaque" "Queue"="Geometry + 2" }
            LOD 200

            //Blend SrcAlpha OneMinusSrcAlpha

            Stencil{
                Ref 50
                Comp Equal
            }

            CGPROGRAM
            
            #pragma vertex normalTopSurfVert
            #pragma fragment normalTopSurfFrag
            #pragma target 3.0
            #include "SHOLLiquid.cginc"
            
            ENDCG
        }
    }
}
