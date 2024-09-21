Shader "Custom/SimplifiedLava"
{
    Properties
    {
        _MainTex("Base Lava Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _DetailTex("Detail Noise Texture", 2D) = "white" {}
        _LavaColor("Lava Color", Color) = (1, 0.3, 0, 1)
        _DistortionColor("Distortion Color", Color) = (1, 0.1, 0.1, 1)
        _EmissionColor("Emission Color", Color) = (1, 0.3, 0, 1)
        _Speed("Speed", Range(0.01, 1.0)) = 0.15
        _Distortion("Distortion Amount", Range(0.1, 1.0)) = 0.3
        _DetailAmount("Detail Intensity", Range(0.1, 2.0)) = 1.0
        _Tiling("Texture Tiling", Vector) = (1, 1, 1, 1)
        _Displacement("Vertex Displacement", Range(0, 0.2)) = 0.1
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            sampler2D _DetailTex;
            float4 _LavaColor;
            float4 _DistortionColor;
            float4 _EmissionColor;
            float _Speed;
            float _Distortion;
            float _DetailAmount;
            float4 _Tiling;
            float _Displacement;

            struct Input
            {
                float2 uv_MainTex;
                float3 worldPos;
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Base texture coordinates with tiling
                float2 uv = IN.uv_MainTex * _Tiling.xy;

                // Apply animated noise for distortion
                float2 noiseUV = uv + _Time.y * _Speed;
                float2 noise = tex2D(_NoiseTex, noiseUV).rg;
                float2 distortedUV = uv + (noise - 0.5) * _Distortion;

                // Sample the main lava texture
                fixed4 baseColor = tex2D(_MainTex, distortedUV) * _LavaColor;

                // Tint the distortion separately
                fixed4 distortionColor = tex2D(_NoiseTex, noiseUV) * _DistortionColor;

                // Combine the lava color with the distortion color
                fixed4 finalColor = lerp(baseColor, distortionColor, 0.5);

                // Add noise detail for more realism
                fixed4 detailColor = tex2D(_DetailTex, distortedUV * _DetailAmount);
                finalColor.rgb = finalColor.rgb * detailColor.rgb;

                // Combine final color with emission
                o.Emission = finalColor.rgb * _EmissionColor.rgb;

                // Set the final color
                o.Albedo = finalColor.rgb;
                o.Metallic = 0.0;
                o.Smoothness = 0.9;

                // Optional: Apply vertex displacement for surface movement
                float displacement = (tex2D(_NoiseTex, distortedUV).r - 0.5) * _Displacement;
                o.Normal = normalize(o.Normal + displacement);
            }
            ENDCG
        }
            FallBack "Diffuse"
}
