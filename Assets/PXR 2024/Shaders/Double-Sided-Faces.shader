Shader "Custom/Double-Sided-Faces"
{
    Properties
    {
        _AlbedoMap("Albedo Texture", 2D) = "white" {}
        _AlbedoColor("Albedo Color", Color) = (1,1,1,1)
        _TintColor("Tint Color", Color) = (1,1,1,1)
        _TintIntensity("Tint Intensity", Range(0, 1)) = 0.5
        _NormalMap("Normal Map", 2D) = "bump" {}
        _MetallicSmoothnessMap("Metallic & Smoothness Texture", 2D) = "white" {}  // Combined map
        _Metallic("Metallic", Range(0, 1)) = 0.0
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _EmissionMap("Emission Texture", 2D) = "black" {}
        _EmissionIntensity("Emission Intensity", float) = 1.0
        _Tiling("Texture Tiling", Vector) = (1,1,0,0)
        _Rotation("Texture Rotation", Range(0, 360)) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Cull Off

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows
            #pragma target 3.0  // Allow more texture interpolators

            sampler2D _AlbedoMap;
            float4 _AlbedoColor;
            float4 _TintColor;
            float _TintIntensity;
            sampler2D _NormalMap;
            sampler2D _MetallicSmoothnessMap;  // Combined map
            float _Metallic;
            float _Smoothness;
            sampler2D _EmissionMap;
            float _EmissionIntensity;
            float4 _Tiling;
            float _Rotation;

            struct Input
            {
                float2 uv_AlbedoMap;
                float2 uv_NormalMap;
                float2 uv_MetallicSmoothnessMap;  // Combined map UV
                float2 uv_EmissionMap;
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Apply tiling and rotation to UV coordinates
                float2 uv = IN.uv_AlbedoMap;
                uv = (uv - 0.5) * _Tiling.xy + 0.5 + _Tiling.zw;
                float rotation = radians(_Rotation);
                float2x2 rotMatrix = float2x2(cos(rotation), -sin(rotation), sin(rotation), cos(rotation));
                uv = mul(rotMatrix, uv - 0.5) + 0.5;

                // Albedo
                fixed4 albedoTex = tex2D(_AlbedoMap, uv) * _AlbedoColor;
                fixed4 finalAlbedo = lerp(albedoTex, albedoTex * _TintColor, _TintIntensity);
                o.Albedo = finalAlbedo.rgb;

                // Normal map
                o.Normal = UnpackNormal(tex2D(_NormalMap, uv));

                // Metallic & Smoothness combined
                float4 metallicSmoothnessTex = tex2D(_MetallicSmoothnessMap, uv);
                o.Metallic = metallicSmoothnessTex.r * _Metallic;
                o.Smoothness = metallicSmoothnessTex.g * _Smoothness;

                // Emission
                fixed4 emissionColor = tex2D(_EmissionMap, uv) * _EmissionIntensity;
                o.Emission = emissionColor.rgb;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
