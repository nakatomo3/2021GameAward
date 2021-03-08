Shader "Original/Player" {
	Properties {
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType" = "Transparent"
				"Queue" = "Transparent"
				"IgnoreProjector" = "True" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull off

		//ステンシルバッファのみ
		Pass {
			Stencil	{
				Ref 1
				Comp Equal
				Pass IncrSat
			}

			ColorMask 0
			ZTest Always
			ZWrite Off
		}

		//普通に見えている部分
		Pass {
			Stencil	{
				Ref 3
				Comp Always
				Pass Replace
			}

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			uniform fixed4 _Color;

			fixed4 frag(v2f_img i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				return col;
			}
			ENDCG
		}

		//隠れている部分
		Pass{
			Stencil{
				Ref 2
				Comp Equal  // 2と一致するもの
			}
			ZTest Always    // 深度に関わらず描画

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			uniform fixed4 _Color;

			fixed4 frag(v2f_img i) : SV_Target {
				fixed4 col = fixed4(0,0,0,1);
				return col;
			}
			ENDCG
		}
	}
}