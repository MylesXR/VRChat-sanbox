Shader "Custom/WaterPushEffect"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _NormalMap("Normal", 2D) = "bump" {}
        [Space(25)]

        [Toggle(_ENABLEEMISSION)] _EnableEmission("Enable Emission", int) = 0
        _EmissionMap("Emission", 2D) = "black" {}
        _EmissionColor("Emission Color", Color) = (1,1,1,1)
        _EmissionIntensity("Emission Intensity", Range(0, 1)) = 0
        [Space(10)]

        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        [Space(10)]

        _PushSpeed("Push Speed", Range(0, 1.5)) = 0.05
        _PushAmount("Push Amount", Range(0, 1.5)) = 0.01
        [Space(10)]

        _CircularSpeed("Circular Speed", Range(0, 1.5)) = 0.2
        _CircularAmount("Circular Amount", Range(0, 1.5)) = 0.1
        [Space(10)]

        _VerticalSpeed("Vertical Speed", Range(0, 1.5)) = 0.2
        _VerticalAmount("Vertical Amount", Range(0, 1.5)) = 0.1
        [Space(10)]

        _EyeHeight("Eye Height", Range(0, 2)) = 1.0
        _EyeRotationSpeed("Eye Rotation Speed", Range(0, 1)) = 0.5
        _EyeRotationLeft("Eye Rotation Left Degrees", Range(0, 360)) = 20.0
        _EyeRotationRight("Eye Rotation Right Degrees", Range(0, 360)) = 20.0
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            CGPROGRAM
            #pragma surface surf Standard vertex:vert
            #pragma multi_compile _ _ENABLEEMISSION

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_NormalMap;
                float2 uv_EmissionMap;
            };

            sampler2D _MainTex;
            sampler2D _NormalMap;
            sampler2D _EmissionMap;
            float4 _EmissionColor;
            float _EmissionIntensity;
            float _Smoothness;
            float _PushSpeed;
            float _PushAmount;
            float _CircularSpeed;
            float _CircularAmount;
            float _VerticalSpeed;
            float _VerticalAmount;
            float _EyeHeight;
            float _EyeRotationSpeed;
            float _EyeRotationLeft;
            float _EyeRotationRight;

            void vert(inout appdata_full v)
            {
                float3 newPosition = v.vertex.xyz;
                float3 worldPosition = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
                float time = _Time.y;

                float normalizedHeight = saturate((worldPosition.y - _EyeHeight) / (_EyeHeight * 2.0));

                float circularX = _CircularAmount * sin(time * _CircularSpeed + worldPosition.x);
                float circularZ = _CircularAmount * cos(time * _CircularSpeed + worldPosition.z);
                float push = _PushAmount * sin(time * _PushSpeed + worldPosition.x);

                newPosition.x += circularX + push;
                newPosition.z += circularZ;

                float vertical = _VerticalAmount * sin(time * _VerticalSpeed) * normalizedHeight;
                newPosition.y += vertical;

                if (worldPosition.y >= _EyeHeight)
                {
                    float oscillation = sin(_Time.y * _EyeRotationSpeed);
                    float rotationAmountDegrees = lerp(-_EyeRotationLeft, _EyeRotationRight, oscillation * 0.5 + 0.5);
                    float rotationAmountRadians = rotationAmountDegrees * (1 - normalizedHeight) * 3.14159 / 180.0;

                    float cosTheta = cos(rotationAmountRadians);
                    float sinTheta = sin(rotationAmountRadians);

                    float originalX = newPosition.x;
                    float originalZ = newPosition.z;

                    newPosition.x = cosTheta * originalX - sinTheta * originalZ;
                    newPosition.z = sinTheta * originalX + cosTheta * originalZ;
                }

                v.vertex.xyz = newPosition;
            }

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                half4 albedo = tex2D(_MainTex, IN.uv_MainTex);
                half3 normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
                half4 emission = tex2D(_EmissionMap, IN.uv_EmissionMap) * _EmissionColor * _EmissionIntensity;

                o.Albedo = albedo.rgb;
                o.Normal = normal;
                #if defined(_ENABLEEMISSION)
                o.Emission = emission.rgb;
                #endif
                o.Smoothness = _Smoothness;
                o.Alpha = albedo.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
