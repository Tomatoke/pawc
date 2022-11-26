Shader "Customs/SpriteAnimation" {
  Properties {
    _MainTex("Texture", 2D) = "white" {}
    _DivX("DivX", float) = 1
    _DivY("DivY", float) = 1
  }

  SubShader {
    Tags { "RenderType" = "Transparent" }
    LOD 100

    Blend SrcAlpha OneMinusSrcAlpha
    BlendOp Add

    Pass {
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
      float4 _MainTex_ST;
      float4 _MainTex_TexelSize;
      float _DivX;
      float _DivY;

      v2f vert(appdata v) {
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	return o;
      }

      fixed4 frag(v2f i) : SV_Target {
	half2x2 scaleMatrix = half2x2(1 / _DivX, 0, 0, 1 / _DivY);
	i.uv += float2(1 / (int)_DivX, 1 / (int)_DivY);
	i.uv = mul(i.uv, scaleMatrix);
	fixed4 col = tex2D(_MainTex, i.uv);
	return col;
      }
      ENDCG
    }
  }
}
