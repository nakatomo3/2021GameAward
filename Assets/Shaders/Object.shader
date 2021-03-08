Shader "Original/Object" {
	Properties {
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
	}
		SubShader {
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass {
				Stencil	{
					Ref 1
					Comp Always
					Pass Replace
				}

				CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				uniform fixed4 _Color;

				fixed4 frag(v2f_img i) : COLOR {
					fixed4 col = tex2D(_MainTex, i.uv) * _Color;
					return col;
				}
				ENDCG
			}
		}
}