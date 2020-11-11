Shader "SHOL/Hologram"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Intensity ("Intensity", float) = 1.5
        _Glossy ("Glossy", float) = 2
        _ScanlineSpeed ("Scanline speed", float) = 1
        _ScanlineThickness ("Scanline thickness", float) = 0.01
        _ScanlineCount ("Scanline count", int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        PASS
        {
            Blend SrcAlpha OneMinusSrcAlpha
            //Cull off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            float4 _Color;
            float _Intensity;
            float _Glossy;
            float _ScanlineSpeed;
            float _ScanlineThickness;
            int _ScanlineCount;

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 normal : NORMAL;
                float4 objectPos : TEXCOORD0;
            };

            v2f vert(appdata_base _in)
            {
                v2f o;
                o.position = UnityObjectToClipPos(_in.vertex);
                o.normal = UnityObjectToWorldNormal(_in.normal);
                o.objectPos = mul(unity_ObjectToWorld, _in.vertex);
                return o;
            }

            float4 frag(v2f _in) : COLOR
            {
                float offset = frac(_Time * _ScanlineSpeed);
                float interval = 1.0 / _ScanlineCount;
                //if(fmod(_in.objectPos.y + offset, interval) > _ScanlineThickness) clip(-1);
                if(fmod(_in.objectPos.y + offset, interval) > _ScanlineThickness) clip(-1);
                //if(_in.objectPos.y > +_ScanlineThickness) clip(-1);
                float4 color = _Color;
                color.rgb *= _Intensity;
                return color;
            }
            ENDCG
        }
    }
}
