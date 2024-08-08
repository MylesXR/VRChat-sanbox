Shader "Custom/SpottyEmissionMoss"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _Glossiness("Glossiness", Range(0.0, 1.0)) = 0.5
        _BumpMap("Normal Map", 2D) = "bump" {}
        _OcclusionMap("Occlusion (R)", 2D) = "white" {}
        _EmissionMap("Emission (RGB)", 2D) = "black" {}
        _GlowColor("Glow Color", Color) = (0, 0.5, 1, 1)
        _GlowIntensity("Glow Intensity", Range(0, 1)) = 0.5
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard alpha:fade

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_BumpMap;
                float2 uv_OcclusionMap;
                float2 uv_EmissionMap;
            };

            sampler2D _MainTex;
            sampler2D _BumpMap;
            sampler2D _OcclusionMap;
            sampler2D _EmissionMap;
            float4 _GlowColor;
            float _Glossiness;
            float _Metallic;
            float _GlowIntensity;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Albedo
                fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = albedo.rgb;

                // Normal
                o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

                // Occlusion
                fixed occlusion = tex2D(_OcclusionMap, IN.uv_OcclusionMap).r;
                o.Occlusion = occlusion;

                // Metallic and Glossiness
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;

                // Emission (glow effect)
                fixed4 emission = tex2D(_EmissionMap, IN.uv_EmissionMap);
                float4 glow = _GlowColor * emission.a * _GlowIntensity;

                // Combine emission with transparency
                o.Emission = glow.rgb;
                o.Alpha = albedo.a * emission.a * _GlowIntensity;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
