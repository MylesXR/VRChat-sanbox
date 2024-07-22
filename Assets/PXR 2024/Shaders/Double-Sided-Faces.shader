Shader "Custom/Double-Sided-Faces"
{
    Properties
    {
        _AlbedoMap("Albedo", 2D) = "white" {}
        [Toggle] _UseAlbedoTexture("Use Albedo Texture", Float) = 1.0
        _AlbedoColor("Albedo Color", Color) = (1,1,1,1)
        _TintColor("Tint Color", Color) = (0,1,0,1) // New property for tint color
        _TintIntensity("Tint Intensity", Range(0, 1)) = 0.5 // New property for tint intensity
        _NormalMap("Normal Map", 2D) = "bump" {}
        _EmissionMap("Emission Map", 2D) = "black" {}
        _AlphaMap("Alpha Map", 2D) = "white" {}
        _EmissionIntensity("Emission Intensity", float) = 1.0
        [Toggle] _UseTransparency("Use Transparency", Int) = 0
        _AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.5
        _AlphaIntensity("Alpha Intensity", Range(0, 10)) = 1.0
        _TransparencyLevel("Transparency Level", Range(0, 1)) = 0.0
        _Tiling("Tiling", Vector) = (1,1,0,0)
        _Rotation("Rotation", Range(0, 360)) = 0
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
            float _UseAlbedoTexture;
            float4 _AlbedoColor;
            float4 _TintColor; // Tint color variable
            float _TintIntensity; // Tint intensity variable
            sampler2D _NormalMap;
            sampler2D _EmissionMap;
            sampler2D _AlphaMap;
            float _EmissionIntensity;
            int _UseTransparency;
            float _AlphaCutoff;
            float _AlphaIntensity;
            float _TransparencyLevel;
            float4 _Tiling;
            float _Rotation;

            struct Input
            {
                float2 uv_AlbedoMap;
                float2 uv_NormalMap;
            };

            void surf(Input IN, inout SurfaceOutput o)
            {
                // Apply tiling to UV coordinates
                float2 uv = IN.uv_AlbedoMap;
                uv = (uv - 0.5) * _Tiling.xy + 0.5 + _Tiling.zw;

                // Apply rotation
                float rotation = radians(_Rotation);
                float2x2 rotMatrix = float2x2(cos(rotation), -sin(rotation), sin(rotation), cos(rotation));
                uv = mul(rotMatrix, uv - 0.5) + 0.5;

                // Albedo
                fixed4 albedoTex = tex2D(_AlbedoMap, uv);
                fixed4 finalAlbedo = lerp(albedoTex, albedoTex * _TintColor, _TintIntensity); // Blend albedo texture with tint color based on tint intensity
                o.Albedo = finalAlbedo.rgb * _UseAlbedoTexture + _AlbedoColor.rgb * (1 - _UseAlbedoTexture);

                // Normal map
                o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));

                // Emission
                fixed4 emissionColor = tex2D(_EmissionMap, uv);
                o.Emission = emissionColor.rgb * _EmissionIntensity;

                // Alpha and Transparency
                fixed alphaValue = 1.0; // Default to opaque

                if (_UseTransparency == 1)
                {
                    alphaValue = tex2D(_AlphaMap, uv).r * _AlphaIntensity;
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
