Shader "Custom/Sway"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _WindSpeed("Wind Speed", Float) = 2.0
        _WindStrength("Wind Strength", Float) = 0.05
        _AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.5
        _PlayerPos("Player Position", Vector) = (0,0,0,0)
        _Radius("Displacement Radius", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                float _WindSpeed;
                float _WindStrength;
                float _AlphaCutoff;
                float4 _PlayerPos;
                float _Radius;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float mask = IN.uv.y; 
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                float3 vPos = IN.positionOS.xyz;

                float dist = distance(worldPos.xy, _PlayerPos.xy);
                if (dist < _Radius)
                {
                    float falloff = 1.0 - (dist / _Radius);
                    float2 dir = normalize(worldPos.xy - _PlayerPos.xy);
                    vPos.xy += dir * falloff * 0.15 * mask;
                }

                float sway = (sin(_Time.y * _WindSpeed) + sin(_Time.y * _WindSpeed * 0.6)) * 0.5;
                vPos.x += sway * _WindStrength * mask;

                OUT.positionCS = TransformObjectToHClip(vPos);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                clip(color.a - _AlphaCutoff);
                return color;
            }
            ENDHLSL
        }
    }
}