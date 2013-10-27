Shader "Level_DiffuseBump"
{
	Properties 
	{
_Level_Diffuse("_Level_Diffuse", 2D) = "black" {}
_Level_Normal("_Level_Normal", 2D) = "black" {}
_Lightcookie("_Lightcookie", 2D) = "black" {}
_cookiespeed("_cookiespeed", Float) = 0
_Specular("Specular", 2D) = "black" {}
_Shininess("Shininess", Range(0,1) ) = 0.5

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 2.0


sampler2D _Level_Diffuse;
sampler2D _Level_Normal;
sampler2D _Lightcookie;
float _cookiespeed;
sampler2D _Specular;
float _Shininess;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}

			inline half4 LightingBlinnPhongEditor_DirLightmap (EditorSurfaceOutput s, fixed4 color, fixed4 scale, half3 viewDir, bool surfFuncWritesNormal, out half3 specColor)
			{
				UNITY_DIRBASIS
				half3 scalePerBasisVector;
				
				half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector);
				
				half3 lightDir = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
				half3 h = normalize (lightDir + viewDir);
			
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular * 128.0);
				
				// specColor used outside in the forward path, compiled out in prepass
				specColor = lm * _SpecColor.rgb * s.Gloss * spec;
				
				// spec from the alpha component is used to calculate specular
				// in the Lighting*_Prepass function, it's not used in forward
				return half4(lm, spec);
			}
			
			struct Input {
				float2 uv_Level_Diffuse;
float2 uv_Lightcookie;
float2 uv_Level_Normal;
float2 uv_Specular;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Tex2D0=tex2D(_Level_Diffuse,(IN.uv_Level_Diffuse.xyxy).xy);
float4 Multiply0=_Time * _cookiespeed.xxxx;
float4 UV_Pan0=float4((IN.uv_Lightcookie.xyxy).x + Multiply0.x,(IN.uv_Lightcookie.xyxy).y + Multiply0.x,(IN.uv_Lightcookie.xyxy).z,(IN.uv_Lightcookie.xyxy).w);
float4 Tex2D1=tex2D(_Lightcookie,UV_Pan0.xy);
float4 Multiply1=Tex2D0 * Tex2D1;
float4 Tex2DNormal0=float4(UnpackNormal( tex2D(_Level_Normal,(IN.uv_Level_Normal.xyxy).xy)).xyz, 1.0 );
float4 Tex2D2=tex2D(_Specular,(IN.uv_Specular.xyxy).xy);
float4 Multiply2=Tex2D2 * _Shininess.xxxx;
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Multiply1;
o.Normal = Tex2DNormal0;
o.Specular = Multiply2;
o.Gloss = Multiply2;
o.Alpha = Tex2D1;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}