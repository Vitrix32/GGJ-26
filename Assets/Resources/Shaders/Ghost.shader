Shader "Unlit/Ghost"
{
    Properties
    {
        _Fade("Fade", Range(0,1)) = 1
        _MainTex ("Texture", 2D) = "white" {}
        _WarpIntensity ("Warp Intensity", Float) = 1.0
        _WarpSpeed ("Warp Speed", Float) = 1.0
        [HDR] _GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _Fade;
            sampler2D _MainTex;
            float _WarpIntensity;
            float _WarpSpeed;
            float4 _GlowColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 warpeduv = i.uv;
                warpeduv.x = i.uv.x + _WarpIntensity * sin(10 * i.uv.y + _Time.y * _WarpSpeed);
                float4 col = tex2D(_MainTex, warpeduv);
                col.a = min(col.a, i.uv.y * i.uv.y);
                col.a *= _Fade;
                col.rgb += _GlowColor.rgb;
                return col;
            }
            ENDCG
        }
    }
}
