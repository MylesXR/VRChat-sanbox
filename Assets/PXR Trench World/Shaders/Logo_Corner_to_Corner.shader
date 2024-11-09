// Custom Shader: Logo Corner to Corner
// Use for PXR 2023 & Single Thread Theatre Company Only.
// Author: Cole Paskuski
//socials and portfolios https://www.artstation.com/colepaskuski & https://github.com/ColePaskuski & https://www.linkedin.com/in/colepaskuski/

Shader "Custom/Logo Corner to Corner"
{
    Properties
    {
        _Logo("Logo Texture", 2D) = "white" {}
        _LogoColor("Logo Colour", Color) = (1,1,1,1)
        _BackgroundColor("Background Colour", Color) = (0,0,0,1)
        _LogoSize("Logo Size", float) = 0.2
        _Speed("Logo Speed", float) = 1.0
        _AlphaCutoff("Logo Edge Alpha Cutoff", float) = 0.5
        [Toggle(_ENABLE_RAINBOW)] _EnableRainbow("Enable Rainbow", Float) = 0
        _RainbowSpeed("Rainbow Colour Speed", float) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
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
                };

                sampler2D _Logo;
                float4 _LogoColor;
                float4 _BackgroundColor;
                float _LogoSize;
                float _Speed;
                float _EnableRainbow;
                float _AlphaCutoff;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 RainbowColor(float time)
                {
                    float3 color = float3(sin(time) * 0.5 + 0.5, sin(time + 2.0) * 0.5 + 0.5, sin(time + 4.0) * 0.5 + 0.5);
                    return fixed4(color, 1);
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float time = _Time.y * _Speed;
                    float2 phase = frac(float2(time, time) / float2(23.0, 31.0));
                    phase = abs(phase * 2 - 1);
                    float2 offset = phase * (1 - _LogoSize);
                    float2 logoUV = (i.uv - offset) / _LogoSize;

                    fixed4 bgColor = _BackgroundColor;

                    if (logoUV.x < 0 || logoUV.y < 0 || logoUV.x > 1 || logoUV.y > 1)
                    {
                        return bgColor;
                    }

                    fixed4 logoTexel = tex2D(_Logo, logoUV);
                    logoTexel.a = step(_AlphaCutoff, logoTexel.a); // Applying step function for sharper alpha cutoff

                    if (logoTexel.a < 0.01) // Near-zero value
                    {
                        return bgColor;
                    }

                    fixed4 finalColor = _LogoColor;

                    if (_EnableRainbow > 0.5)
                    {
                        finalColor = RainbowColor(time);
                    }

                    fixed4 mixedColor = lerp(bgColor, finalColor, logoTexel.a);
                    mixedColor.a = 1.0;
                    return mixedColor;
                }
                ENDCG
            }
        }
}