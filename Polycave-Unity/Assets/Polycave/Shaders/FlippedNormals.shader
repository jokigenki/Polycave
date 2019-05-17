Shader "Nekologic/Unlit/FlippedNormals"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Transparency("Transparency", Range(0.0,1.0)) = 0.5
        _CutoutThresh("Cutout Threshold", Range(0.0,1.0)) = 0.2
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Cull Front
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Transparency;
            float _CutoutThresh;

        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 uv: TEXCOORD0;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv: TEXCOORD0;
            float3 normal: NORMAL;
            UNITY_FOG_COORDS(1)
        };
        
        v2f vert (appdata v) {
            v2f o;
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            o.pos = UnityObjectToClipPos(v.vertex );
            o.normal  = v.normal * -1;
            UNITY_TRANSFER_FOG(o,o.pos);
            return o;
        }
        
        fixed4 frag (v2f i) : SV_Target { 
            // sample the texture
            fixed4 col = tex2D(_MainTex, i.uv);
            col.a = 1 - _Transparency;
            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, col);
            return col;
        }

        ENDCG
        }
    }
}
