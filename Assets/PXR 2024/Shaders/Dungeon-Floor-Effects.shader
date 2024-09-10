Shader "Custom/SporadicCockroachEffect"
{
    Properties
    {
        _BugTex("Bug Texture", 2D) = "white" {}
        _Speed("Speed", Range(0.1, 2.0)) = 0.5
        _Tiling("Tiling", Vector) = (1,1,0,0)
        _Transparency("Transparency", Range(0.0, 1.0)) = 0.7
        _AppearRate("Appearance Rate", Range(0.1, 5.0)) = 1.0
    }
        SubShader
        {
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 200

            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off
                Cull Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _BugTex;
                float4 _BugTex_ST;
                float _Speed;
                float4 _Tiling;
                float _Transparency;
                float _AppearRate;

                // Random function based on UVs and time for sporadic appearance
                float random(float2 uv)
                {
                    return frac(sin(dot(uv.xy + _Time.y * 0.1, float2(12.9898,78.233))) * 43758.5453);
                }

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _BugTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Sporadic appearance control
                    float appear = step(1.0 - _AppearRate, random(i.uv));

                // Apply random movement
                float2 animatedUV = i.uv * _Tiling.xy + float2(_Time.y * _Speed, _Time.y * _Speed);
                animatedUV += float2(random(i.uv) - 0.5, random(i.uv) - 0.5) * _Speed;

                // Fetch texture and apply transparency and appearance rate
                fixed4 col = tex2D(_BugTex, frac(animatedUV)) * appear;

                // Apply transparency
                col.a *= _Transparency;

                return col;
            }
            ENDCG
        }
        }
            FallBack "Transparent/Cutout/VertexLit"
}
