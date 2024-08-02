Shader "Custom/RealisticDoubleTextureMovingRainShader"
{
    Properties
    {
        _MainTex1("Rain Texture 1", 2D) = "white" {}
        _MainTex2("Rain Texture 2", 2D) = "white" {}
        _RainColor("Rain Color", Color) = (1,1,1,1)
        _RainSpeed1("Rain Speed 1", Range(0, 10)) = 1.0
        _RainSpeed2("Rain Speed 2", Range(0, 10)) = 1.5
        _Visibility("Visibility", Range(0, 1)) = 0.5
        _RotationAngle1("Rotation Angle 1", Range(0, 360)) = 0
        _RotationAngle2("Rotation Angle 2", Range(0, 360)) = 45
        _Tiling("Tiling", Vector) = (1, 1, 0, 0)
        _TopFadeStart("Top Fade Start", Range(0, 1)) = 0.8
        _TopFadeEnd("Top Fade End", Range(0, 1)) = 1.0
        _BottomFadeStart("Bottom Fade Start", Range(0, 1)) = 0.0
        _BottomFadeEnd("Bottom Fade End", Range(0, 1)) = 0.2
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 200
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0

                #include "UnityCG.cginc"

                sampler2D _MainTex1;
                sampler2D _MainTex2;
                float4 _RainColor;
                float _RainSpeed1;
                float _RainSpeed2;
                float _Visibility;
                float _RotationAngle1;
                float _RotationAngle2;
                float4 _Tiling;
                float _TopFadeStart;
                float _TopFadeEnd;
                float _BottomFadeStart;
                float _BottomFadeEnd;

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float fadeFactor : TEXCOORD1;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv * _Tiling.xy + _Tiling.zw;

                    // Calculate fade factor based on UV y-coordinate for top and bottom fades
                    float topFade = smoothstep(_TopFadeStart, _TopFadeEnd, o.uv.y);
                    float bottomFade = smoothstep(_BottomFadeEnd, _BottomFadeStart, o.uv.y);
                    o.fadeFactor = topFade * bottomFade;

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 center = float2(0.5, 0.5);

                    // Rotate and animate the UV coordinates for the first texture
                    float2 uv1 = i.uv - center;
                    float angle1 = radians(_RotationAngle1);
                    float cosAngle1 = cos(angle1);
                    float sinAngle1 = sin(angle1);
                    float2 rotatedUV1 = float2(
                        uv1.x * cosAngle1 - uv1.y * sinAngle1,
                        uv1.x * sinAngle1 + uv1.y * cosAngle1
                    ) + center;
                    rotatedUV1.y += _Time.y * _RainSpeed1;

                    // Rotate and animate the UV coordinates for the second texture
                    float2 uv2 = i.uv - center;
                    float angle2 = radians(_RotationAngle2);
                    float cosAngle2 = cos(angle2);
                    float sinAngle2 = sin(angle2);
                    float2 rotatedUV2 = float2(
                        uv2.x * cosAngle2 - uv2.y * sinAngle2,
                        uv2.x * sinAngle2 + uv2.y * cosAngle2
                    ) + center;
                    rotatedUV2.y += _Time.y * _RainSpeed2;

                    // Sample the rain textures
                    fixed4 rain1 = tex2D(_MainTex1, rotatedUV1);
                    fixed4 rain2 = tex2D(_MainTex2, rotatedUV2);

                    // Blend the rain textures
                    fixed4 blendedRain = lerp(rain1, rain2, 0.5);

                    // Apply rain color
                    blendedRain.rgb *= _RainColor.rgb;

                    // Apply visibility and fade factor
                    blendedRain.a *= _Visibility * i.fadeFactor;

                    return blendedRain;
                }
                ENDCG
            }
        }
            FallBack "Transparent"
}
