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

            sampler2D _ShapeLightTexture0;

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 positionWS : TEXCOORD1;
                float4 screenPos : TEXCOORD4;
                float objectYWS : TEXCOORD5; 
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _TargetColor;
                float4 _NewColor;
                float _Tolerance;
            CBUFFER_END

            float4 _GlobalPlayerPos;
            float _GlobalRadius;
            float _GlobalSoftness;

            Varyings vert (Attributes input) {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                
                // Get the object's pivot Y position
                output.objectYWS = TransformObjectToWorld(float3(0,0,0)).y;
                
                output.uv = input.uv;
                output.color = input.color * _Color;
                output.screenPos = ComputeScreenPos(output.positionCS);
                return output;
            }

            half4 frag (Varyings input) : SV_Target {
                // 1. COLOR SWAP
                half4 col = tex2D(_MainTex, input.uv);

                float d_color = distance(col.rgb, _TargetColor.rgb);
                if (d_color < _Tolerance) {
                    float brightness = (col.r + col.g + col.b) / 3.0;
                    col.rgb = _NewColor.rgb * (brightness * 1.5);
                    col.a *= _NewColor.a;
                }

                col *= input.color;

                // 2. SEE-THROUGH
                float d_dist = distance(input.positionWS.xyz, _GlobalPlayerPos.xyz);
                float mask = smoothstep(_GlobalRadius - _GlobalSoftness, _GlobalRadius, d_dist);
                
                if (_GlobalPlayerPos.y > input.objectYWS + 0.1) {
                    col.a *= mask;
                }

                // 3. UNITY 6 LIGHTING HOOK
                float2 uv = input.screenPos.xy / input.screenPos.w;
                half4 lightColor = tex2D(_ShapeLightTexture0, uv);
                col.rgb *= lightColor.rgb;

                return col;
            }
            ENDHLSL
        }
    }
}