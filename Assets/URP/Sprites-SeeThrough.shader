Shader "Custom/Sprites-SeeThrough"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "PreviewType"="Plane" }
        
        Cull Off 
        ZWrite Off 
        Blend One OneMinusSrcAlpha // Correct for SpriteRenderer premultiplied alpha

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
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _Color;

            // Global variables from C#
            float4 _GlobalPlayerPos;
            float _GlobalRadius;
            float _GlobalSoftness;

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                output.uv = input.uv;
                output.color = input.color * _Color;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                half4 col = tex2D(_MainTex, input.uv) * input.color;
                
                // Calculate 2D distance
                float d = distance(input.positionWS.xy, _GlobalPlayerPos.xy);

                // --- SAFETY CHECK ---
                // If radius is near zero, keep the sprite fully visible
                if (_GlobalRadius < 0.01) 
                {
                    col.rgb *= col.a;
                    return col;
                }

                // Create the transparency mask
                // d > Radius = 1 (Visible)
                // d < Radius - Softness = 0 (Transparent)
                float mask = smoothstep(_GlobalRadius - _GlobalSoftness, _GlobalRadius, d);
                
                col.a *= mask;
                col.rgb *= col.a; // Apply alpha to RGB for the Blend mode
                
                return col; 
            }
            ENDHLSL
        }
    }
}