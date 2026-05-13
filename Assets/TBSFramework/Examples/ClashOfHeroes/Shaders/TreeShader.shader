Shader "Custom/TreeSwayShader_MobileOptimized_Instanced"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WindStrength ("Wind Strength", Float) = 0.1
        _WindSpeed ("Wind Speed", Float) = 1.0
        _WindFrequency ("Wind Frequency", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert
        #pragma multi_compile_instancing

        #include "UnityCG.cginc"

        sampler2D _MainTex;

        // Define instanced properties
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float, _WindStrength)
            UNITY_DEFINE_INSTANCED_PROP(float, _WindSpeed)
            UNITY_DEFINE_INSTANCED_PROP(float, _WindFrequency)
        UNITY_INSTANCING_BUFFER_END(Props)

        struct Input
        {
            float2 uv_MainTex;
        };

        void vert (inout appdata_full v)
        {
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

            float windStrength = UNITY_ACCESS_INSTANCED_PROP(Props, _WindStrength);
            float windSpeed = UNITY_ACCESS_INSTANCED_PROP(Props, _WindSpeed);
            float windFrequency = UNITY_ACCESS_INSTANCED_PROP(Props, _WindFrequency);

            // Compute sway from wind parameters
            float sway = sin(_Time.y * windSpeed + worldPos.x * windFrequency) * windStrength;

            // Use a non-linear animation curve: 0 at the base and 1 at the top
            float factor = 1.0 - cos(v.vertex.y * (3.14159 * 0.5));
    
            // Offset the vertex positions using the computed factor
            v.vertex.x += sway * factor;
            v.vertex.z += sway * factor;
        }


        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
