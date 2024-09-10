Shader "Custom/Window-Lights-XYRandomControl"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}  // Base texture
        _EmissionColor("Emission Color", Color) = (0, 1, 0, 1)  // Bright green for testing
        _Opacity("Opacity", Range(0,1)) = 1.0  // Opacity control
        _RandomnessX("Randomness X", Range(0,1)) = 0.5  // Controls the randomness on the X axis
        _RandomnessY("Randomness Y", Range(0,1)) = 0.5  // Controls the randomness on the Y axis
        _OverallRandomness("Overall Randomness", Range(0,1)) = 0.5  // Master randomness control
        _UV2Offset("UV2 Offset", Vector) = (0.5, 0.5, 0, 0)  // Offset for UV2
        _UV2Scale("UV2 Scale", Vector) = (0.5, 0.5, 0, 0)  // Scaling factors for UV2 on X and Y
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows

            sampler2D _MainTex;
            fixed4 _EmissionColor;
            half _Opacity;
            half _RandomnessX;
            half _RandomnessY;
            half _OverallRandomness;
            float4 _UV2Offset;
            float4 _UV2Scale;

            struct Input
            {
                float2 uv2_MainTex;
                float3 worldPos;
            };

            // Function to generate a pseudo-random value based on a seed
            float rand(float seed)
            {
                return frac(sin(seed) * 43758.5453);
            }

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Apply UV2 transformations
                float2 transformedUV2 = (IN.uv2_MainTex * _UV2Scale.xy) + _UV2Offset.xy;

                // Generate independent random values for X and Y axes
                float xRandom = rand(floor(transformedUV2.x * 10.0) + 100.0) * _RandomnessX;
                float yRandom = rand(floor(transformedUV2.y * 10.0) + 200.0) * _RandomnessY;

                // Combine the X and Y randomness, and apply the overall randomness control
                float combinedRandom = (xRandom + yRandom) * _OverallRandomness;

                // Sample base texture
                fixed4 c = tex2D(_MainTex, transformedUV2);

                // Apply emission based on the combined random value
                if (combinedRandom > 0.5)  // You can adjust this threshold as needed
                {
                    o.Emission = _EmissionColor.rgb;  // Full emission
                }
                else
                {
                    o.Emission = 0;  // No emission
                }

                // Set Albedo
                o.Albedo = c.rgb;
                o.Alpha = _Opacity;
            }

            ENDCG
        }
            FallBack "Diffuse"
}
