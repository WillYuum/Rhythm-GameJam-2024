Shader "Custom/RoomTransition"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _Scale ("Scale", Range (0.1, 10)) = 1.0
        _CustomTime ("Custom Time", Range (0, 10)) = 1.0
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3
            ENDCG
            SetTexture[_MainTex]
            {
                combine texture
            }
        }
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float _Scale;
        float _CustomTime;

        struct Input
        {
            float2 uv_MainTex;
        };

        void vert(inout appdata_full v)
        {
            v.vertex.xy = (v.vertex.xy * 2.0) - 1.0;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Fractal effect logic here
            float2 uv = IN.uv_MainTex * _Scale;
            float2 p = uv;
            float2 i = p;

            // Use the % operator instead of mod
            p = abs(1.0 - (_CustomTime / 100.0 % 2.0) - 1.0);
            p = 2.0 * (p - floor(p)) - 1.0;
            p *= 1.0 - abs(2.0 * (i - floor(i)) - 1.0);

            // Use the color from the Image texture
            fixed4 imgColor = tex2D(_MainTex, IN.uv_MainTex);
            
            // Combine the fractal color with the Image texture color
            o.Albedo = imgColor.rgb + float3(p, 0.0);
            o.Alpha = imgColor.a;
        }
        ENDCG
    }

    Fallback "Diffuse"
}
