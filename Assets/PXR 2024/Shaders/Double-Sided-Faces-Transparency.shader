Shader "Custom/Double-Sided-Albedo-Cutout"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
    }
        SubShader
        {
            Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
            LOD 200

            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On

            CGPROGRAM
            #pragma surface surf Lambert alpha:clip

            sampler2D _MainTex;
            fixed4 _Color;
            half _Cutoff;

            struct Input
            {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutput o)
            {
                // Sample the main texture
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;

                // Apply alpha cutoff
                clip(c.a - _Cutoff);
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
