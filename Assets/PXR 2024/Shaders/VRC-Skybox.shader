Shader "Custom/VRC-Skybox"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        [MaterialToggle] _StereoEnabled("Stereo Enabled", Float) = 0
        _Rotation("Rotation", Range(0, 360)) = 0
        _Tile("Tiling", Vector) = (1, 1, 0, 0)
        _Exposure("Exposure", Range(0, 10)) = 1
        _NeonIntensity("Neon Intensity", Range(0, 5)) = 1
        _Contrast("Contrast", Range(0, 2)) = 1
        _Saturation("Saturation", Range(0, 2)) = 1
    }
        SubShader
        {
            Tags
            {
                "Queue" = "Background"
                "RenderType" = "Opaque"
            }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                #pragma multi_compile_fwdbase_fullshadows
                #pragma target 3.0

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _StereoEnabled;
                float _Rotation;
                float4 _Tile;
                float _Exposure;
                float _NeonIntensity;
                float _Contrast;
                float _Saturation;

                float3 RotateVector(float3 v, float angle)
                {
                    float rad = radians(angle);
                    float cosA = cos(rad);
                    float sinA = sin(rad);
                    float3x3 rotationMatrix = float3x3(
                        cosA, 0, sinA,
                        0, 1, 0,
                        -sinA, 0, cosA
                    );
                    return mul(rotationMatrix, v);
                }

                float2 StereoPanoProjection(float3 coords)
                {
                    float3 normalizedCoords = normalize(coords);
                    float latitude = acos(normalizedCoords.y);
                    float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
                    float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
                    sphereCoords = float2(0.5, 1.0) - sphereCoords;
                    return (sphereCoords + float2(0, unity_StereoEyeIndex * 0.5)) * _Tile.xy + _Tile.zw;
                }

                float2 MonoPanoProjection(float3 coords)
                {
                    float3 normalizedCoords = normalize(coords);
                    float latitude = acos(normalizedCoords.y);
                    float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
                    float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
                    sphereCoords = float2(0.5, 1.0) - sphereCoords;
                    return sphereCoords * _Tile.xy + _Tile.zw;
                }

                struct appdata_t
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float3 worldPos : TEXCOORD0;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    return o;
                }

                float4 AdjustContrast(float4 color, float contrast)
                {
                    float3 midpoint = float3(0.5, 0.5, 0.5);
                    color.rgb = lerp(midpoint, color.rgb, contrast);
                    return color;
                }

                float4 AdjustSaturation(float4 color, float saturation)
                {
                    float luminance = dot(color.rgb, float3(0.299, 0.587, 0.114));
                    float3 grey = float3(luminance, luminance, luminance);
                    color.rgb = lerp(grey, color.rgb, saturation);
                    return color;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
                    viewDir = RotateVector(viewDir, _Rotation);

                    float2 uv = _StereoEnabled > 0.5 ? StereoPanoProjection(viewDir) : MonoPanoProjection(viewDir);

                    float4 texColor = tex2D(_MainTex, uv);
                    texColor.rgb *= _Exposure;
                    texColor = AdjustContrast(texColor, _Contrast);
                    texColor = AdjustSaturation(texColor, _Saturation);

                    // Boost the brightness and saturation of the neon colors
                    float3 neonFactor = pow(texColor.rgb, float3(_NeonIntensity, _NeonIntensity, _NeonIntensity));
                    texColor.rgb = lerp(texColor.rgb, neonFactor, texColor.rgb);

                    return texColor;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
