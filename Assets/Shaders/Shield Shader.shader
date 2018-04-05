Shader "Unlit/Shield Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainColor ("Color", Color) = (1,1,1,0.5)
		_Atten ("Light Attenuation", float) = 0.1
		_LightColor("Light Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }
		//Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
						
			#include "UnityCG.cginc"

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				
				float4 pos : SV_POSITION;
				float normal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 viewDir: TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainColor;
			float _Atten;
			float4 _LightColor;

			v2f vert (vertexInput v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float4 normal4 = float4(v.normal, 0.0);
				o.normal = normalize(mul(normal4, unity_WorldToObject).xyz);
				o.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float edgeFactor = abs(dot(i.viewDir, i.normal));
				float oneMinusEdge = 1.0 - edgeFactor;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				half diff = dot(i.normal, lightDir)*0.5+0.5;
				
				col.rgb = _MainColor.rgb * _LightColor.rgb*(diff * _Atten) ;
				col.a = 0.5*abs(sin(_Time.w + i.uv.y*3.1415*2));
				//col.a = 0.5; 
				return col;
			}
			ENDCG
		}
	}
}
