// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Polyart/Dreamscape Waterfall"
{
	Properties
	{
		_LineScaleWidth("Line Scale Width", Float) = 0
		[Header(COLOR)]_ColorShallow("Color Shallow", Color) = (0.5990566,0.9091429,1,1)
		_ColorDeep("Color Deep", Color) = (0.1213065,0.347919,0.5471698,1)
		_Smoothness("Smoothness", Float) = 0
		_LineAcceleration("Line Acceleration", Float) = 1
		_NormalWave("Normal Wave", 2D) = "bump" {}
		_ColorDepthFade("Color Depth Fade", Float) = 0
		_Line01ScaleX("Line 01 Scale X", Float) = 0
		_Line02ScaleX("Line 02 Scale X", Float) = 0
		_Line01Speed("Line 01 Speed", Float) = 0
		_Line02Speed("Line 02 Speed", Float) = 0
		_Line01ScaleY("Line 01 Scale Y", Float) = 0
		_Line02ScaleY("Line 02 Scale Y", Float) = 0
		_LineUVFade("Line UV Fade", Float) = 0
		_LineTreshold("Line Treshold", Range(0 , 1)) = 0.1
		_Displace1("Displace 1", 2D) = "white" {}
		_WaveNormalIntensity("Wave Normal Intensity", Range(0 , 1)) = 1
		[IntRange]_WaveTiling01("Wave Tiling 01", Range(0 , 50)) = 1
		[IntRange]_WaveTiling02("Wave Tiling 02", Range(0 , 50)) = 1
		_DisplaceStrength("Displace Strength", Range(-1 , 1)) = 0
		_WaterLines("Water Lines", 2D) = "white" {}
		_LineColor("Line Color", Color) = (1,1,1,1)
		_DisplacementMovement("Displacement Movement", Vector) = (0,3,0,0)
		[HideInInspector] _texcoord("", 2D) = "white" {}
		[HideInInspector] __dirty("", Int) = 1
	}

		SubShader
		{
			Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
			Cull Back
			// Removed blending mode, as we are rendering opaque
			// Blend SrcAlpha OneMinusSrcAlpha 

			CGPROGRAM
			#include "UnityShaderVariables.cginc"
			#include "UnityStandardUtils.cginc"
			#include "UnityCG.cginc"
			#pragma target 3.0
			#pragma surface surf Standard keepalpha noshadow vertex:vertexDataFunc 
			struct Input
			{
				float2 uv_texcoord;
				float4 screenPos;
			};

			uniform sampler2D _Displace1;
			uniform float2 _DisplacementMovement;
			uniform float _WaveTiling02;
			uniform float _DisplaceStrength;
			uniform sampler2D _NormalWave;
			uniform float _WaveTiling01;
			uniform float _WaveNormalIntensity;
			uniform float4 _LineColor;
			uniform float4 _ColorShallow;
			uniform float4 _ColorDeep;
			UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _ColorDepthFade;
			uniform float _LineUVFade;
			uniform sampler2D _WaterLines;
			uniform float _Line01ScaleX;
			uniform float _LineScaleWidth;
			uniform float _LineAcceleration;
			uniform float _Line01Speed;
			uniform float _Line01ScaleY;
			uniform float _Line02ScaleX;
			uniform float _Line02Speed;
			uniform float _Line02ScaleY;
			uniform float _LineTreshold;
			uniform float _Smoothness;

			void vertexDataFunc(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				float2 temp_cast_0 = (_WaveTiling02).xx;
				float2 uv_TexCoord142 = v.texcoord.xy * temp_cast_0;
				float2 panner143 = (1.0 * _Time.y * _DisplacementMovement + uv_TexCoord142);
				float4 vDisplacement136 = (saturate(tex2Dlod(_Displace1, float4(panner143, 0, 0.0))) * _DisplaceStrength);
				v.vertex.xyz += vDisplacement136.rgb;
				v.vertex.w = 1;
			}

			void surf(Input i , inout SurfaceOutputStandard o)
			{
				float2 temp_cast_0 = (_WaveTiling01).xx;
				float2 uv_TexCoord72 = i.uv_texcoord * temp_cast_0;
				float2 panner76 = (1.0 * _Time.y * float2(0,0.75) + uv_TexCoord72);
				float3 vNormal91 = UnpackScaleNormal(tex2D(_NormalWave, panner76), _WaveNormalIntensity);
				o.Normal = vNormal91;
				float4 ase_screenPos = float4(i.screenPos.xyz , i.screenPos.w + 0.00000000001);
				float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
				ase_screenPosNorm.z = (UNITY_NEAR_CLIP_VALUE >= 0) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth105 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, ase_screenPosNorm.xy));
				float distanceDepth105 = abs((screenDepth105 - LinearEyeDepth(ase_screenPosNorm.z)) / (_ColorDepthFade));
				float4 lerpResult109 = lerp(_ColorShallow , _ColorDeep , saturate(distanceDepth105));
				float2 temp_cast_2 = (_WaveTiling02).xx;
				float2 uv_TexCoord142 = i.uv_texcoord * temp_cast_2;
				float2 panner143 = (1.0 * _Time.y * _DisplacementMovement + uv_TexCoord142);
				float4 vDisplacement136 = (saturate(tex2D(_Displace1, panner143)) * _DisplaceStrength);
				float4 lerpResult140 = lerp(lerpResult109 , (lerpResult109 + float4(0.4414383,0.5903998,0.6037736,1)) , vDisplacement136);
				float4 vDepthColor110 = lerpResult140;
				float2 appendResult26 = (float2((_LineScaleWidth * i.uv_texcoord.x) , pow(i.uv_texcoord.y , _LineAcceleration)));
				float2 vMovementLines112 = appendResult26;
				float2 break27 = vMovementLines112;
				float2 appendResult30 = (float2((_Line01ScaleX * break27.x) , ((break27.y - (_Time.y * _Line01Speed)) * _Line01ScaleY)));
				float4 vLine01113 = tex2D(_WaterLines, appendResult30);
				float2 break55 = vMovementLines112;
				float2 appendResult61 = (float2((_Line02ScaleX * break55.x) , ((break55.y - (_Time.y * _Line02Speed)) * _Line02ScaleY)));
				float4 vLine02120 = tex2D(_WaterLines, appendResult61);
				float4 vLinesCombined122 = floor(saturate((saturate(((i.uv_texcoord.y * _LineUVFade) + ((vLine01113 * vLine02120) * 2.0))) + (1.0 - _LineTreshold))));
				float4 lerpResult124 = lerp(_LineColor , vDepthColor110 , vLinesCombined122);
				o.Albedo = lerpResult124.rgb;
				o.Smoothness = _Smoothness;
				o.Alpha = 1; // Always fully opaque
			}

			ENDCG
		}
}
