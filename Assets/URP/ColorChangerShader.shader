Shader "Custom/ColorChangerShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _TargetColor ("Color to Replace", Color) = (0,1,0,1) // The Green
        _NewColor ("New Color", Color) = (0,0,1,1)        // The Blue
        _Tolerance ("Tolerance", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _TargetColor;
            fixed4 _NewColor;
            float _Tolerance;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Calculate how close the pixel color is to our "Target" green
                float dist = distance(col.rgb, _TargetColor.rgb);

                // If it's close enough, swap it to the new color
                if (dist < _Tolerance) {
                    return fixed4(_NewColor.rgb, col.a);
                }

                return col;
            }
            ENDCG
        }
    }
}