Shader "Custom/VFX-Lava-Bubbles"
{
    Properties
    {
        _AlbedoColor("Albedo Color", Color) = (1,1,1,0)  // Set default alpha to 0 for transparency
        _Noise_Perlin("Noise Texture", 2D) = "white" {}
        _AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.5
        _ErosionSpeed("Erosion Speed", Range(0, 5)) = 1.0  // Controls how fast the alpha erosion happens
        _Transparency("Transparency", Range(0, 1)) = 0.5  // Adjusts overall transparency
    }

        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            LOD 100

            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows alpha:fade
            #pragma target 3.0

            sampler2D _Noise_Perlin;
            float4 _AlbedoColor;
            float _AlphaCutoff;
            float _ErosionSpeed;
            float _Transparency;

            struct Input
            {
                float2 uv_Noise_Perlin;
                float3 worldPos;
                float _Time;  // Built-in time variable in Unity
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Create a time-based panning effect to animate the noise texture
                float timeFactor = IN._Time * _ErosionSpeed;

                // Sample the noise texture with dynamic UVs
                float2 uvPanned = IN.uv_Noise_Perlin + timeFactor;
                float noiseValue = tex2D(_Noise_Perlin, uvPanned).r;

                // Calculate alpha based on noise value
                float alphaValue = noiseValue * _Transparency;

                // Apply the alpha cutoff for the breakaway effect
                clip(alphaValue - _AlphaCutoff);

                // Set the final color and alpha with transparency
                o.Albedo = _AlbedoColor.rgb;
                o.Alpha = alphaValue;  // Use the calculated alpha for transparency
            }
            ENDCG
        }

            FallBack "Diffuse"
}
