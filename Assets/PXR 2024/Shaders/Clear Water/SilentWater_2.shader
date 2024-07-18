Shader "Silent/Clear Water 2 Normal" {
	Properties {
		_Tint ("Surface Colour", Color) = (1,1,1,1)
		_MainTex ("Surface Texture", 2D) = "white" {}

		_Glossiness ("Smoothness", Range(0,1)) = 1.0
		_Metallic ("Metallic", Range(0,1)) = 0.0

		[Toggle(_)]_IgnoreUVs("Ignore Mesh UVs", Float) = 1
		[Toggle(_)]_IgnoreVertexColour("Ignore Vertex Colour", Float) = 1

		[Header(Wave Settings)]
		[Normal]_Wave("Wave", 2D) = "bump" {}
		_WaveStrength("Wave Strength", Range( -1 , 1)) = 0.1
		_WaveScrollX("Wave Scroll X", Range( -1 , 1)) = 0
		_WaveScrollY("Wave Scroll Y", Range( -1 , 1)) = 0
		_WaveScrollSpeed("Wave Scroll Multiplier", Range( 0 , 4)) = 0
		[Space]
		[Normal]_Wave2("Wave 2", 2D) = "bump" {}
		_Wave2Strength("Wave 2 Strength", Range( -1 , 1)) = 0.1
		_Wave2ScrollX("Wave 2 Scroll X", Range( -1 , 1)) = 0
		_Wave2ScrollY("Wave 2 Scroll Y", Range( -1 , 1)) = 0
		_Wave2ScrollSpeed("Wave 2 Scroll Multiplier", Range( -0 , 4)) = 0

		[Header(Foam Settings)]
		_Foam("Foam Texture", 2D) = "white" {}
		[HDR]_FoamColour("Foam Colour", Color) = (1,1,1,1)
		_FoamDensity("Foam Density", Range(0, 1)) = 1
		[Gamma]_FoamMax("Foam Start", Range( 0 , 4)) = 0
		[Gamma]_FoamMin("Foam End", Range( 0 , 4)) = 1
		[Gamma]_FoamDistortion("Foam Distortion", Range( 0 , 1)) = 0.3
		[Gamma]_FoamScrollSpeed("Foam Scroll Multiplier", Range( 0 , 4)) = 1

		[Header(Water Fog Settings)]
		_DepthDensity("Water Fog Density", Range(0, 1)) = 0.15
		_DepthDensityVertical("Water Fog Vertical Density", Range(0, 1)) = 0.15
		_DepthColour("Water Fog Colour", Color) = (0,0,0,1)

		[Header(Forward Rendering Options)]
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Int) = 2
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent-10" "IgnoreProjector" = "True" "IsEmissive" = "true" }
		Cull [_CullMode]
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard alpha vertex:vert
		#pragma target 3.5

		#include "SilentWater.cginc"

		void surf (Input IN, inout SurfaceOutputStandard o) {
			IN.color = _IgnoreVertexColour? 1.0 : IN.color;
			fixed4 surface = tex2D (_MainTex, IN.foamUVs.zw) * _Tint * IN.color;
			fixed3 interior = 0;

			IN.worldNormal = WorldNormalVector( IN, float3( 0, 0, 1 ) );

			// == Ambient environment sample ==
		    float3 viewDir = UnityWorldSpaceViewDir(IN.worldPos);
		    float3 reflectionDir = -viewDir;
		    float4 envSample = UNITY_SAMPLE_TEXCUBE_LOD(
		            unity_SpecCube0, reflectionDir, UNITY_SPECCUBE_LOD_STEPS
		        );

			// == Wave normals ==
			float3 wave1 = UnpackScaleNormal(tex2D(_Wave, IN.waveUVs.xy), _WaveStrength);
			float3 wave2 = UnpackScaleNormal(tex2D(_Wave2, IN.waveUVs.zw), _Wave2Strength);

			o.Normal = BlendNormalsPD(wave1, wave2);

			// Apply specular AA filter to smooth shimmering.
			o.Smoothness = GetGeometricNormalVariance(_Glossiness, IN.worldNormal, 0.5, 0.5);

			// == Surface reduction ==
			float NdotV = max(0, dot(IN.VFace * IN.worldNormal, normalize(viewDir)));
			float surfaceReduction = (FresnelLerpFast(1-_Metallic, 1-_Glossiness, NdotV));

			// Get the base depth values.
			depthData d = getDepthValues(IN);
			float edgeFade = 1-saturate(1-(d.foamFade)*10);

			// == Underwater fog ==
			//float4 underFog = UnderwaterFog(d); 
			// For fog, 0 is full intensity; 1 is fully transparent

			// Fade the fog to black where it's more blended,
			// but also remove the surface opacity.
			//float fogReduce = saturate(underFog.w + surface.a);
			//underFog.rgb *= 1-fogReduce;
			// Fade the surface to black where it isn't fogged.
			//surface.rgb *= fogReduce;
			// Add the fog to the interior colour, so it doesn't get lit by add pass lights.
			//interior.rgb += underFog.rgb;
			// Replace the surface alpha with the fog alpha where fog is present.
			//surface.a = max(surface.a,(1-underFog.w));

			// Apply some lighting to the foam for dynamic lighting conditions.
			interior.rgb *= envSample;

			// == Surface foam ==
			float2 foamUVs = IN.foamUVs.xy + o.Normal * _FoamDistortion * 10;
			float4 surfaceFoam = tex2D(_Foam, foamUVs) * _FoamColour;

			float foamFade = d.foamFade; 

			// Remap the edge fade to match the foam range.
			_FoamMin += 0.001;
			foamFade = saturate((foamFade - _FoamMin) / (_FoamMax - _FoamMin));

			surfaceFoam = saturate(foamFade + foamFade - (1-surfaceFoam));
			surfaceFoam *= _FoamDensity;
			interior += surfaceFoam * envSample;

			surface *= edgeFade;
			interior *= edgeFade;

			// Fix later: instances where vertex colour alpha is 0, 
			// but output alpha is > 0 so the edge is dark. 
			o.Albedo = surface.rgb * surfaceReduction;
			o.Emission = interior.rgb * surfaceReduction * IN.color.a;
			o.Alpha = saturate(surface.a + 1-surfaceReduction) * IN.color.a;

			// On the edges of water, reflections should fade out. 
			// We also want to fade out specular intensity (metallic) to account 
			// for the premultiplied alpha calculation.
			// The occlusion property affects indirect lighting (i.e. reflections).
			float edgeFade2 = saturate(saturate(edgeFade*10)-surfaceFoam.a);
			o.Occlusion = edgeFade * IN.color.a;
			o.Metallic = _Metallic * edgeFade * IN.color.a;
			//debugThis(o, o.Normal.z);
		}
		ENDCG
	}
	Fallback "Standard"
}
