Shader "Custom/Double-Sided-Faces-Transparency"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
        _NormalMap("Normal Map", 2D) = "bump" {}
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        _MaxDistance("Max Distance for Cutoff", Range(0,100)) = 50
    }
        SubShader
        {
            Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
            LOD 200

            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On

            CGPROGRAM
            #pragma surface surf Standard alpha:clip

            sampler2D _MainTex;
            sampler2D _NormalMap;
            fixed4 _Color;
            half _Cutoff;
            half _Smoothness;
            half _MaxDistance;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_NormalMap;
                float3 worldPos;
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                float distance = length(UnityWorldSpaceViewDir(IN.worldPos));

                float distanceFactor = saturate(distance / _MaxDistance);
                float adjustedCutoff = _Cutoff * (1.0 - (distanceFactor * 0.5));

                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;

                o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));

                o.Smoothness = _Smoothness;

                clip(c.a - adjustedCutoff);
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}