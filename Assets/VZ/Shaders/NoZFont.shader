Shader "VZ/NoZFont" {

   Properties 
   { 
      _MainTex ("Texture", 2D) = "white" {} 
      _Color ("Color", Color) = (1,1,1,1)
   }

	SubShader {

      Tags {
         "RenderType" = "Transparent"
         "Queue" = "Overlay"
      }

      Lighting Off
      ZWrite Off
      ZTest Off
      Blend SrcAlpha OneMinusSrcAlpha
      Cull Off
      Fog { Mode Off }

      Pass {
         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #include "UnityCG.cginc"

         struct v2f {
            float4 pos : SV_POSITION;
            fixed4 color : COLOR;
            float2 texcoord : TEXCOORD0;
         };

         sampler2D _MainTex;
         uniform fixed4 _Color;

         v2f vert (appdata_full v)
         {
            v2f o;
            o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
            o.color = v.color * _Color;
            o.texcoord = v.texcoord;
            return o;
         }

         fixed4 frag (v2f i) : SV_Target 
         { 
            fixed4 col = i.color;
            col.a *= tex2D(_MainTex, i.texcoord).a;
            return col;
         }

         ENDCG
      }
	} 
}
