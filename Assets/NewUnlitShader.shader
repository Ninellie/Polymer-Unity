Shader "Custom/URP/CircleFromQuad"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Softness ("Edge Softness", Range(0,0.5)) = 0.02
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "Forward"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            float4 _BaseColor;
            float _Softness;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color * _BaseColor;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 p = i.uv * 2.0 - 1.0;
                float d = dot(p, p);

                float edge = 1.0;
                float alpha = 1.0 - smoothstep(edge - _Softness, edge, d);

                if (alpha <= 0.0)
                    discard;
                
                return half4(i.color.rgb, i.color.a * alpha);
            }
            ENDHLSL
        }
    }
}
