Shader "Custom/AnimatedFogShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _FogColor("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogDensity("Fog Density", Range(0, 1)) = 0.1
        _Speed("Fog Speed", Range(0.0, 10.0)) = 1.0
        _Alpha("Alpha", Range(0.0, 1.0)) = 1.0
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 100

            Blend SrcAlpha OneMinusSrcAlpha
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 worldPos : TEXCOORD1;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _FogColor;
                float _FogDensity;
                float _Speed;
                float _Alpha;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Animate the fog using time
                    float time = _Time.y * _Speed;
                    float3 animatedWorldPos = i.worldPos + float3(sin(time), 0, cos(time));

                    // Sample the texture
                    fixed4 texColor = tex2D(_MainTex, i.uv);

                    // Calculate fog factor based on animated distance
                    float distance = length(animatedWorldPos - _WorldSpaceCameraPos);
                    float fogFactor = exp(-_FogDensity * distance);
                    fogFactor = clamp(fogFactor, 0.0, 1.0);

                    // Blend the texture color with the fog color
                    fixed4 fogColor = _FogColor;
                    fixed4 finalColor = lerp(fogColor, texColor, fogFactor);

                    // Apply alpha value
                    finalColor.a = _Alpha;

                    return finalColor;
                }
                ENDCG
            }
        }
            FallBack "Transparent/Diffuse"
}
