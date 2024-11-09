Shader "Custom/DoubleSidedFaces" {
    Properties{
        _MainTex("Main Texture", 2D) = "white" {}
        _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
        _Color("Tint Color", Color) = (1,1,1,1)
    }

        SubShader{
            Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }

            Cull Off // Render both sides

            CGPROGRAM
            #pragma surface surf Lambert alpha
            #pragma target 3.0
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Cutoff;
            float4 _Color;

            struct Input {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutput o) {
                half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                clip(c.a - _Cutoff);
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
        }

            Fallback "Diffuse"
}
