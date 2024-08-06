Shader "Custom/FireFlicker"
{
    Properties
    {
        _StartColor("Start Emission Color", Color) = (1, 0.5, 0, 1) // Starting color for fire
        _EndColor("End Emission Color", Color) = (1, 0.25, 0, 1) // Ending color for fire
        _EmissionIntensity("Emission Intensity", Range(0, 1)) = 1.0
        _FlickerFrequency("Flicker Frequency", float) = 10.0
        _FlickerIntensity("Flicker Intensity", float) = 0.5
        _ColorChangeFrequency("Color Change Frequency", float) = 0.5
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows addshadow
        #pragma target 3.0

        float4 _StartColor;
        float4 _EndColor;
        float _EmissionIntensity;
        float _FlickerFrequency;
        float _FlickerIntensity;
        float _ColorChangeFrequency;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Flicker effect
            float flicker = 1.0 + sin(_Time.y * _FlickerFrequency) * _FlickerIntensity;

            // Color variation to simulate fire (interpolating between start and end colors)
            float3 fireColor = lerp(_StartColor.rgb, _EndColor.rgb, 0.5 + 0.5 * sin(_Time.y * _ColorChangeFrequency));

            // Calculate final emission color
            float3 finalEmission = fireColor * _EmissionIntensity * flicker;

            // Apply emission
            o.Emission = finalEmission;
        }
        ENDCG
    }
        FallBack "Diffuse"
}
