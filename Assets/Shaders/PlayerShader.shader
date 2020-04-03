Shader "Custom/PlayerShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Player("Player", Range(0,1)) = 0
		_Health("Player Health", float) = 100
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert

        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float3 localPos;
			float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		float _Player;
		float _PlayerHealth;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.localPos = v.vertex.xyz;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            //o.Albedo.z = lerp(0, 1, clamp(IN.localPos.z, 0, 1));
			/*if (_Player == 0) {
				_Color.x *= (sin(_Time.y) + 1) / 2;
				_Color.y *= (cos(_Time.y) + 1) / 2;
				_Color.z *= (sin(_Time.y + 3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679) + 1) / 2;
			}*/

			_Color.gb = lerp(0, 1, _PlayerHealth / 100);

			o.Albedo = _Color;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
