Shader "SHOL/S_Xray"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Center("X-ray center", vector) = (0,0,0)
		_Raydir("X-ray direction", vector) = (0,0,0)
		_Radius("X-ray radius", Range(0, 1)) = 0
	}

		SubShader
		{
			Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" "LightMode"="ForwardBase" }
			Cull Off
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha
			Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#pragma target 3.0
				#include "UnityCG.cginc"
				#include "UnityLightingCommon.cginc"
				
				half4 _Color;
				sampler2D _MainTex;
				float3 _Center;
				float3 _Raydir;
				float _Radius;

				struct Input {
					float4 position : POSITION;
					float4 texCoord : TEXCOORD0;
					float3 normal : NORMAL0;
				};

				struct Output {
					float4 position : SV_POSITION;
					float4 texCoord : TEXCOORD0;
					float4 worldpos : TEXCOORD1;
					float3 normal : NORMAL0;
					fixed4 lightColor : COLOR0;
				};

				Output vert(Input _i) {
					Output o;
					o.position = UnityObjectToClipPos(_i.position);
					o.texCoord = _i.texCoord;
					o.worldpos = mul(unity_ObjectToWorld, _i.position);
					o.normal = UnityObjectToWorldNormal(_i.normal.xyz);

					//light
					half3 worldNormal = UnityObjectToWorldNormal(_i.normal);
					fixed NdotL = dot(worldNormal, _WorldSpaceLightPos0);
					NdotL = max(0, NdotL) * 0.5;
					o.lightColor = (0.5 + NdotL) * _LightColor0;
					
					return o;
				}

				half4 frag(Output _o) : COLOR
				{
				    fixed facing = dot(_Raydir, _o.normal);
				    float3 pixPos = _o.worldpos;
					
					//clipping pixel when in radius range
				    if(distance(pixPos, _Center.xyz) <= _Radius && facing < 0) clip(-1);
					
					half4 mainColor = tex2Dlod(_MainTex, _o.texCoord);
					half4 result;
					result.rgba = (mainColor * _Color);
					result.rgb = result.rgb * _o.lightColor;
					return result;
				}

				ENDCG
			}
		}
			FallBack "Diffuse"
}
