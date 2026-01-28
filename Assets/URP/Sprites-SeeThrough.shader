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
        Blend One OneMinusSrcAlpha 

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
                float objectYWS : TEXCOORD3; // We store the object pivot Y here
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _Color;

            float4 _GlobalPlayerPos;
            float _GlobalRadius;
            float _GlobalSoftness;

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                
                // Get the World Y of the object pivot (bottom center)
                output.objectYWS = TransformObjectToWorld(float3(0,0,0)).y;
                
                output.positionCS = TransformWorldToHClip(output.positionWS);
                output.uv = input.uv;
                output.color = input.color * _Color;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                half4 col = tex2D(_MainTex, input.uv) * input.color;
                
                // 1. Calculate the distance for the circle
                // We use World XYZ for a true "bubble"
                float d = distance(input.positionWS.xyz, _GlobalPlayerPos.xyz);
                float mask = smoothstep(_GlobalRadius - _GlobalSoftness, _GlobalRadius, d);

                // 2. THE FRONT/BACK CHECK
                // If Player Y is less than Object Pivot Y, the player is in front.
                // We use a small threshold (0.1) to prevent flickering.
                if (_GlobalPlayerPos.y < input.objectYWS - 0.1)
                {
                    mask = 1.0; // Force opaque
                }

                col.a *= mask;
                col.rgb *= col.a; 
                
                return col;
            }
            ENDHLSL
        }
    }
}