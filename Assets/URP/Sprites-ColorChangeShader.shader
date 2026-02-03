Shader "Custom/Tree-Individual-Swap-Unity6-Lit"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _TargetColor ("Color to Replace", Color) = (0.1, 0.5, 0.1, 1)
        _NewColor ("New Color", Color) = (0.1, 0.1, 0.8, 1)        
        _Tolerance ("Tolerance", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Cull Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "SpriteLit"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 screenPos : TEXCOORD4;
            };

            sampler2D _MainTex;
            sampler2D _ShapeLightTexture0;

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _TargetColor;
                float4 _NewColor;
                float _Tolerance;
            CBUFFER_END

            Varyings vert (Attributes input) {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = input.uv;
                output.color = input.color * _Color;
                output.screenPos = ComputeScreenPos(output.positionCS);
                return output;
            }

            half4 frag (Varyings input) : SV_Target {
                half4 col = tex2D(_MainTex, input.uv);
                
                // 1. COLOR SWAP
                float d = distance(col.rgb, _TargetColor.rgb);

                float mask = step(d, _Tolerance);

                float brightness = dot(col.rgb, float3(0.333, 0.333, 0.333));
                half3 replacement = _NewColor.rgb * (brightness * 1.5);
                col.rgb = lerp(col.rgb, replacement, mask);
                col.a = lerp(col.a, col.a * _NewColor.a, mask);

                col *= input.color;

                // 2. LIGHTING (UNITY 6 2D)
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                half4 lightColor = tex2D(_ShapeLightTexture0, screenUV);
                col.rgb *= lightColor.rgb;

                return col;
            }
            ENDHLSL
        }
    }
}