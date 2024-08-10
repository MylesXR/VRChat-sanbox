Shader "Custom/DoubleSidedStandardShaderWithAlphaAndEmission"
{
    Properties
    {
        _AlbedoMap("Albedo", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _EmissionMap("Emission Map", 2D) = "black" {}
        _AlphaMap("Alpha Map", 2D) = "white" {}
        _EmissionIntensity("Emission Intensity", float) = 1.0
        [Toggle] _UseTransparency("Use Transparency", Int) = 0
        _AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.5
        _AlphaIntensity("Alpha Intensity", Range(0, 10)) = 1.0
        _TransparencyLevel("Transparency Level", Range(0, 1)) = 0.0
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            LOD 100

            // Disable backface culling
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma surface surf Lambert

            // Textures and variables
            sampler2D _AlbedoMap;
            sampler2D _NormalMap;
            sampler2D _EmissionMap;
            sampler2D _AlphaMap;
            float _EmissionIntensity;
            int _UseTransparency;
            float _AlphaCutoff;
            float _AlphaIntensity;
            float _TransparencyLevel;

            struct Input
            {
                float2 uv_AlbedoMap;
                float2 uv_NormalMap;
                float2 uv_EmissionMap;
                float2 uv_AlphaMap;
            };

            // Surface shader
            void surf(Input IN, inout SurfaceOutput o)
            {
                // Albedo
                fixed4 c = tex2D(_AlbedoMap, IN.uv_AlbedoMap);
                o.Albedo = c.rgb;

                // Normal map
                o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));

                // Emission
                fixed4 emissionColor = tex2D(_EmissionMap, IN.uv_EmissionMap);
                o.Emission = emissionColor.rgb * _EmissionIntensity;

                // Alpha and Transparency
                fixed alphaValue = 1.0; // Default to opaque

                if (_UseTransparency == 1)
                {
                    alphaValue = tex2D(_AlphaMap, IN.uv_AlphaMap).r * _AlphaIntensity;
                    clip(alphaValue - _AlphaCutoff);
                }

                // Apply the transparency level
                alphaValue = lerp(alphaValue, _TransparencyLevel, _TransparencyLevel);

                o.Alpha = alphaValue;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
