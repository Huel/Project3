Shader "Team"
{
	Properties 
	{
_MainColor("_MainColor", Color) = (1,1,1,1)
_Shininess("_Shininess", Range(0,8) ) = 0.5
_Valve_Specular("_Valve_Specular", 2D) = "black" {}
_Valve_Diffuse("_Valve_Diffuse", 2D) = "black" {}
_Valve_Normal("_Valve_Normal", 2D) = "black" {}
_Lightcookie("_Lightcookie", 2D) = "black" {}
_cookiespeed("_cookiespeed", Float) = 0
_TeamMask("_TeamMask", 2D) = "black" {}
_Team("_Team", Color) = (1,1,1,1)

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


float4 _MainColor;
float _Shininess;
sampler2D _Valve_Specular;
sampler2D _Valve_Diffuse;
sampler2D _Valve_Normal;
sampler2D _Lightcookie;
float _cookiespeed;
sampler2D _TeamMask;
float4 _Team;

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
				float2 uv_Valve_Diffuse;
float2 uv_TeamMask;
float2 uv_Valve_Normal;
float2 uv_Valve_Specular;

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
				
float4 Tex2D0=tex2D(_Valve_Diffuse,(IN.uv_Valve_Diffuse.xyxy).xy);
float4 Multiply0=_MainColor * Tex2D0;
float4 Tex2D2=tex2D(_TeamMask,(IN.uv_TeamMask.xyxy).xy);
float4 Multiply2=_Team * Tex2D2;
float4 Add0=Multiply0 + Multiply2;
float4 Tex2DNormal0=float4(UnpackNormal( tex2D(_Valve_Normal,(IN.uv_Valve_Normal.xyxy).xy)).xyz, 1.0 );
float4 Tex2D1=tex2D(_Valve_Specular,(IN.uv_Valve_Specular.xyxy).xy);
float4 Multiply1=_Shininess.xxxx * Tex2D1;
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Add0;
o.Normal = Tex2DNormal0;
o.Specular = Multiply1;
o.Gloss = Multiply1;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}