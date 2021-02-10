﻿Shader "PostEffect/LED" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _Scale ("Scale", Float) = 20
    _Mask ("Mask", Float) = 3
  }
  SubShader {
    Cull Off ZWrite Off ZTest Always

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #define PI 3.14159265359f

      // Properties
      uniform sampler2D _MainTex;
      uniform float4    _MainTex_TexelSize;
      uniform half      _Scale;
      uniform half      _Mask;

      // Vertex Input
      struct appdata {
        float4 vertex : POSITION;
        float2 uv     : TEXCOORD0;
      };

      // Vertex to Fragment
      struct v2f {
        float4 pos : SV_POSITION;
        float2 uv  : TEXCOORD0;
      };

      //------------------------------------------------------------------------
      // Vertex Shader
      //------------------------------------------------------------------------
      v2f vert(appdata v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv  = v.uv;
        return o;
      }

      //------------------------------------------------------------------------
      // Fragment Shader
      //------------------------------------------------------------------------
      fixed4 frag(v2f i) : SV_Target {
        // Mosaic
        float2 scale = _MainTex_TexelSize.xy * _Scale;
        float2 uv    = (floor(i.uv / scale) + 0.5) * scale;

        fixed4 color = tex2D(_MainTex, uv);

        // LED
        //float2 mask = abs(sin(i.uv / scale * PI));
        //color.rgb *= saturate(mask.x * mask.y * _Mask);

        return color;
      }
      ENDCG
    }
  }
}
