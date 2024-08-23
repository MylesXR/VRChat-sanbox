Shader "Custom/ScrollingMaskedHexagonShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {} // The hexagonal pattern texture
        _MaskTex("Mask Texture", 2D) = "white" {} // The mask texture for scrolling effect
        _Color("Color", Color) = (1,1,1,1)
        _EmissionColor("Emission Color", Color) = (0.2, 0.6, 1, 1)
        _HexScrollSpeed("Hex Scroll Speed", Vector) = (0.1, 0.1, 0, 0) // Speed for hex pattern scrolling
        _MaskScrollSpeed("Mask Scroll Speed", Vector) = (0.2, 0.2, 0, 0) // Speed for mask scrolling
        _Transparency("Transparency", Range(0,1)) = 0.5
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows alpha:fade

            sampler2D _MainTex;
            sampler2D _MaskTex;
            fixed4 _Color;
            fixed4 _EmissionColor;
            float4 _HexScrollSpeed;
            float4 _MaskScrollSpeed;
            float _Transparency;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_MaskTex;
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Scroll the texture coordinates for the hex pattern
                float2 hexUV = IN.uv_MainTex + _HexScrollSpeed.xy * _Time.y;

                // Scroll the texture coordinates for the mask
                float2 maskUV = IN.uv_MaskTex + _MaskScrollSpeed.xy * _Time.y;

                // Sample the hexagonal pattern texture
                fixed4 hexTex = tex2D(_MainTex, hexUV);

                // Sample the mask texture
                fixed4 maskTex = tex2D(_MaskTex, maskUV);

                // Use the mask to modulate the visibility of the hexagonal pattern
                fixed4 maskedHex = hexTex * maskTex;

                // Apply transparency and color
                fixed4 c = _Color * maskedHex;
                c.a *= _Transparency * maskTex.a; // Use mask alpha to control transparency

                // Set the surface output
                o.Albedo = c.rgb;
                o.Emission = c.rgb * _EmissionColor.rgb * maskedHex.a;
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Transparent/Cutout/VertexLit"
}
