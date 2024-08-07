Shader "Custom/TilableScanlinesSeparateEmission"
{
    Properties
    {
        [Header(Main Texture Settings)]
        _MainTex("Main Texture", 2D) = "white" {}
        [Space(10)]
        _BaseEmissionColor("Base Emission Color", Color) = (1, 1, 1, 1)

        [Header(Scanline Settings)]
        [Space(10)]
        [Toggle] _EnableScanlines("Enable Scanlines", Float) = 1.0
        _ScanlineTex("Scanline Texture", 2D) = "white" {}
        [Range(0.001, 1)] _ScanlineOpacity("Scanline Opacity", Float) = 0.1
        [Range(0.001, 1)] _ScanlineSpeed("Scanline Speed", Float) = 0.1
        [Range(0.001, 10)] _ScanlineTileY("Scanline Tiling Y", Float) = 1.0
        _ScanlineColor("Scanline Color", Color) = (1, 1, 1, 1)
        _ScanlineEmissionColor("Scanline Emission Color", Color) = (1, 1, 1, 1)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows

            sampler2D _MainTex;
            sampler2D _ScanlineTex;
            float _ScanlineOpacity;
            float _ScanlineSpeed;
            float _ScanlineTileY;
            fixed4 _ScanlineColor;
            fixed4 _ScanlineEmissionColor;
            fixed4 _BaseEmissionColor;
            float _EnableScanlines;

            struct Input
            {
                float2 uv_MainTex;
                float3 worldPos;
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Base texture
                fixed4 baseColor = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = baseColor.rgb;
                o.Emission = baseColor.rgb * _BaseEmissionColor.rgb;

                // Ensure the scanlines are only applied when enabled
                if (_EnableScanlines > 0.5)
                {
                    // Calculate the UV coordinates for the scanline texture
                    float2 scanlineUV = IN.uv_MainTex;
                    scanlineUV.y *= _ScanlineTileY;
                    scanlineUV.y += _Time.y * _ScanlineSpeed;

                    // Sample the scanline texture
                    float scanline = tex2D(_ScanlineTex, scanlineUV).r * _ScanlineOpacity;

                    // Apply the scanline effect to the base color
                    baseColor.rgb = lerp(baseColor.rgb, _ScanlineColor.rgb, scanline);
                    o.Albedo = baseColor.rgb;

                    // Apply the scanline effect to the emission separately
                    o.Emission += lerp(float3(0, 0, 0), _ScanlineEmissionColor.rgb, scanline) * _ScanlineOpacity;
                }

                o.Alpha = baseColor.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
