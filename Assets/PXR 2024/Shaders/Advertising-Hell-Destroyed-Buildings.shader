Shader "Custom/Advertising-Hell-Destroyed-Buildings"
{
    Properties
    {
        // **Albedo Properties**
        [Space(10)]
        [Header(Albedo Settings)]
        [Space(10)]
        [Toggle] _UseAlbedoTexture("Use Albedo Texture", Float) = 1.0
        _AlbedoMap("Albedo Map", 2D) = "white" {}
        _AlbedoColor("Albedo Color", Color) = (1,1,1,1)

            // **Normal Properties**
            [Space(20)]
            [Header(Normal Settings)]
            [Space(10)]
            _NormalMap("Normal Map", 2D) = "bump" {}

            // **Ambient Occlusion Properties**
            [Space(20)]
            [Header(Ambient Occlusion Settings)]
            [Space(10)]
            _AOMap("Ambient Occlusion Map", 2D) = "white" {}
            _AOIntensity("AO Intensity", Range(0, 1)) = 1.0

                // **Emission Properties**
                [Space(20)]
                [Header(Emission Settings)]
                [Space(10)]
                [Toggle] _UseEmission("Use Emission", Float) = 0.0
                _EmissionMap("Emission Map", 2D) = "black" {}
                _EmissionColor("Emission Color", Color) = (1,1,1,1)
                _EmissionIntensity("Emission Intensity", Range(0, 10)) = 1.0

                    // **Global UV Transformations**
                    [Space(20)]
                    [Header(Global UV Transformations)]
                    [Space(10)]
                    _Tiling("Tiling", Vector) = (1,1,0,0)
                    _Rotation("Rotation", Range(0, 360)) = 0
    }

        SubShader
                    {
                        Tags { "RenderType" = "Opaque" }
                        LOD 100

                        // Disable backface culling for double-sided rendering
                        Cull Off

                        CGPROGRAM
                        #pragma surface surf Lambert

                        sampler2D _AlbedoMap;
                        float _UseAlbedoTexture;
                        float4 _AlbedoColor;
                        sampler2D _NormalMap;
                        sampler2D _AOMap;
                        float _AOIntensity;
                        sampler2D _EmissionMap;
                        float4 _EmissionColor;
                        float _UseEmission;
                        float _EmissionIntensity;
                        float4 _Tiling;
                        float _Rotation;

                        struct Input
                        {
                            float2 uv_AlbedoMap;
                            float2 uv_NormalMap;
                            float2 uv_AOMap;
                        };

                        void surf(Input IN, inout SurfaceOutput o)
                        {
                            // Apply global tiling to UV coordinates
                            float2 uv = IN.uv_AlbedoMap;
                            uv = (uv - 0.5) * _Tiling.xy + 0.5 + _Tiling.zw;

                            // Apply rotation
                            float rotation = radians(_Rotation);
                            float2x2 rotMatrix = float2x2(cos(rotation), -sin(rotation), sin(rotation), cos(rotation));
                            uv = mul(rotMatrix, uv - 0.5) + 0.5;

                            // Albedo
                            fixed4 albedoTex = tex2D(_AlbedoMap, uv);
                            o.Albedo = albedoTex.rgb * _UseAlbedoTexture + _AlbedoColor.rgb * (1 - _UseAlbedoTexture);

                            // Normal map
                            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));

                            // Ambient Occlusion
                            float ao = tex2D(_AOMap, IN.uv_AOMap).r * _AOIntensity;

                            // Emission
                            if (_UseEmission)
                            {
                                fixed4 emissionColor = tex2D(_EmissionMap, uv) * _EmissionColor;
                                o.Emission = emissionColor.rgb * _EmissionIntensity * ao; // Apply ambient occlusion to emission
                            }

                            // Apply ambient occlusion to albedo
                            o.Albedo *= ao;
                        }
                        ENDCG
                    }
                        FallBack "Diffuse"
}
