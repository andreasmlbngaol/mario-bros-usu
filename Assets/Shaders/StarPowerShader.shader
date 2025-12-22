Shader "Custom/StarPowerShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)
        _StarColor ("Star Power Color", Color) = (1,1,1,1)
        _Intensity ("Glow Intensity", Range(0, 2)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _StarColor;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = texColor * _Color;
                
                // Blend dengan Star Color berdasarkan brightness
                float brightness = (finalColor.r + finalColor.g + finalColor.b) / 3.0;
                finalColor.rgb = lerp(finalColor.rgb, _StarColor.rgb, brightness * _Intensity);
                
                return finalColor;
            }
            ENDCG
        }
    }
}