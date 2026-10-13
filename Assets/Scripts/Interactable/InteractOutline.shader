// Solid color using the texture's alpha. Used by InteractOutline for both sprites and meshes.
Shader "Custom/InteractOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Width ("Mesh Outline Width", Float) = 0.05
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull [_Cull]
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Width;

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.xyz += v.normal * _Width; // inflate meshes (sprites use 0)
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = _Color;
                c.a *= tex2D(_MainTex, i.uv).a;
                return c;
            }
            ENDCG
        }
    }
}
