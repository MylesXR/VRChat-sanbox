Shader "Custom/OverlayScanlinesWithDistortion"
{
    Properties
    {
        [Header(Main Texture Settings)]
        _Texture1("Main Texture", 2D) = "white" {}
        _BaseEmissionColor("Base Emission Color", Color) = (1, 1, 1, 1)

        [Space(10)]
        [Toggle] _EnableMultiTexture("Enable Multi-Texture", Float) = 0.0
        _Texture2("Texture 2", 2D) = "white" {}
        _Texture3("Texture 3", 2D) = "white" {}
        _Texture4("Texture 4", 2D) = "white" {}
        [Range(5, 30)] _TextureSwitchInterval("Texture Switch Interval (s)", Float) = 10.0
        [Range(0.1, 1.0)] _TextureFadeDuration("Texture Fade Duration (s)", Float) = 0.5
        [Toggle] _SyncWithTextureSettings("Sync with Texture Settings", Float) = 0.0
        [Toggle] _ReverseTextureOrder("Reverse Texture Order", Float) = 0.0  // New toggle for reversing texture order

        [Header(Scanline Settings)]
        [Space(10)]
        [Toggle] _EnableScanlines("Enable Scanlines", Float) = 1.0
        _ScanlineTex("Scanline Texture", 2D) = "white" {}
        [Range(0.001, 1)] _ScanlineOpacity("Scanline Opacity", Float) = 0.1
        [Range(0.001, 1)] _ScanlineSpeed("Scanline Speed", Float) = 0.1
        [Range(0.001, 10)] _ScanlineTileY("Scanline Tiling Y", Float) = 1.0
        _ScanlineColor("Scanline Color", Color) = (1, 1, 1, 1)
        _ScanlineEmissionColor("Scanline Emission Color", Color) = (1, 1, 1, 1)

        [Header(Distortion Settings)]
        [Space(10)]
        [Toggle] _EnableDistortion("Enable Distortion", Float) = 1.0
        _DistortionTex("Distortion Texture", 2D) = "white" {}
        [Range(5, 30)] _DistortionInterval("Distortion Interval (s)", Float) = 15.0
        [Range(0.1, 10)] _DistortionDuration("Distortion Duration (s)", Float) = 1.0
        [Range(0.001, 1)] _DistortionIntensity("Distortion Intensity", Float) = 0.1
        [Range(0.001, 1)] _DistortionSpeed("Distortion Speed", Float) = 0.5
        [Range(0.001, 2)] _DistortionScale("Distortion Scale", Float) = 1.0
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows

            sampler2D _Texture1;
            sampler2D _Texture2;
            sampler2D _Texture3;
            sampler2D _Texture4;
            sampler2D _ScanlineTex;
            sampler2D _DistortionTex;
            float _TextureSwitchInterval;
            float _TextureFadeDuration;
            float _ScanlineOpacity;
            float _ScanlineSpeed;
            float _ScanlineTileY;
            float _DistortionInterval;
            float _DistortionDuration;
            float _DistortionIntensity;
            float _DistortionSpeed;
            float _DistortionScale;
            fixed4 _ScanlineColor;
            fixed4 _ScanlineEmissionColor;
            fixed4 _BaseEmissionColor;
            float _EnableScanlines;
            float _EnableDistortion;
            float _EnableMultiTexture;
            float _SyncWithTextureSettings;
            float _ReverseTextureOrder;  // New variable for reversing texture order

            struct Input
            {
                float2 uv_Texture1;
                float3 worldPos;
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                float switchTime = fmod(_Time.y, _TextureSwitchInterval * 4.0);

                // Determine the order of texture switching
                bool reverse = _ReverseTextureOrder > 0.5;
                fixed4 currentTexColor;
                fixed4 nextTexColor;

                if (_EnableMultiTexture > 0.5)
                {
                    if (!reverse)
                    {
                        if (switchTime < _TextureSwitchInterval)
                        {
                            currentTexColor = tex2D(_Texture1, IN.uv_Texture1);
                            nextTexColor = tex2D(_Texture2, IN.uv_Texture1);
                        }
                        else if (switchTime < _TextureSwitchInterval * 2.0)
                        {
                            currentTexColor = tex2D(_Texture2, IN.uv_Texture1);
                            nextTexColor = tex2D(_Texture3, IN.uv_Texture1);
                        }
                        else if (switchTime < _TextureSwitchInterval * 3.0)
                        {
                            currentTexColor = tex2D(_Texture3, IN.uv_Texture1);
                            nextTexColor = tex2D(_Texture4, IN.uv_Texture1);
                        }
                        else
                        {
                            currentTexColor = tex2D(_Texture4, IN.uv_Texture1);
                            nextTexColor = tex2D(_Texture1, IN.uv_Texture1);
                        }
                    }
                    else
                    {
                        if (switchTime < _TextureSwitchInterval)
                        {
                            currentTexColor = tex2D(_Texture4, IN.uv_Texture1);
                            nextTexColor = tex2D(_Texture3, IN.uv_Texture1);
                        }
                        else if (switchTime < _TextureSwitchInterval * 2.0)
                        {
                            currentTexColor = tex2D(_Texture3, IN.uv_Texture1);
                            nextTexColor = tex2D(_Texture2, IN.uv_Texture1);
                        }
                        else if (switchTime < _TextureSwitchInterval * 3.0)
                        {
                            currentTexColor = tex2D(_Texture2, IN.uv_Texture1);
                            nextTexColor = tex2D(_Texture1, IN.uv_Texture1);
                        }
                        else
                        {
                            currentTexColor = tex2D(_Texture1, IN.uv_Texture1);
                            nextTexColor = tex2D(_Texture4, IN.uv_Texture1);
                        }
                    }

                    // Apply the texture switching effect
                    float fadeFactor = smoothstep(0.0, _TextureFadeDuration, switchTime - floor(switchTime / _TextureSwitchInterval) * _TextureSwitchInterval);
                    fixed4 baseColor = lerp(currentTexColor, nextTexColor, fadeFactor);

                    o.Albedo = baseColor.rgb;
                    o.Emission = baseColor.rgb * _BaseEmissionColor.rgb;
                }
                else
                {
                    currentTexColor = tex2D(_Texture1, IN.uv_Texture1);
                    o.Albedo = currentTexColor.rgb;
                    o.Emission = currentTexColor.rgb * _BaseEmissionColor.rgb;
                }

                // Ensure the scanlines are only applied when enabled
                if (_EnableScanlines > 0.5)
                {
                    // Calculate the UV coordinates for the scanline texture
                    float2 scanlineUV = IN.uv_Texture1;
                    scanlineUV.y *= _ScanlineTileY;
                    scanlineUV.y += _Time.y * _ScanlineSpeed;

                    // Distortion effect if enabled
                    if (_EnableDistortion > 0.5)
                    {
                        float duration = _SyncWithTextureSettings > 0.5 ? _TextureFadeDuration : _DistortionDuration;
                        float timeModulo = fmod(_Time.y, _SyncWithTextureSettings > 0.5 ? _TextureSwitchInterval : _DistortionInterval);
                        if (timeModulo < duration) // Distortion duration within the interval
                        {
                            float2 distortionUV = IN.uv_Texture1;
                            distortionUV *= _DistortionScale;
                            distortionUV += float2(_Time.y * _DistortionSpeed, _Time.y * _DistortionSpeed);

                            float2 distortion = tex2D(_DistortionTex, distortionUV).rg * _DistortionIntensity;
                            scanlineUV += (distortion - 0.5) * 2.0; // Center distortion
                        }
                    }

                    // Sample the scanline texture
                    float scanline = tex2D(_ScanlineTex, scanlineUV).r * _ScanlineOpacity;

                    // Overlay the scanline effect on the base color
                    fixed4 scanlineColor = lerp(fixed4(o.Albedo, 1.0), _ScanlineColor, scanline);
                    o.Albedo = scanlineColor.rgb;

                    // Apply the scanline effect to the emission separately
                    o.Emission += lerp(float3(0, 0, 0), _ScanlineEmissionColor.rgb, scanline) * _ScanlineOpacity;
                }

                o.Alpha = currentTexColor.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
