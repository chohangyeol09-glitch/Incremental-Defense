Shader "IncrementalDefense/InstancedSprite"
{
    Properties
    {
        _MainTex ("Sprite Sheet", 2D) = "white" {}
        _Columns ("Columns", Float) = 4
        _Rows ("Rows", Float) = 1
        _QuadSize ("Quad Size", Float) = 1
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            Cull Off
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            StructuredBuffer<float2> _Positions;

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Columns;
                float _Rows;
                float _QuadSize;
                float _Cutoff;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input, uint instanceID : SV_InstanceID)
            {
                Varyings output;

                float2 instancePosition = _Positions[instanceID];
                float3 positionWS = float3(instancePosition + input.positionOS.xy * _QuadSize, 0.0);
                output.positionCS = TransformWorldToHClip(positionWS);

                float2 cellSize = 1.0 / float2(_Columns, _Rows);
                output.uv = (input.uv + float2(0.0, _Rows - 1.0)) * cellSize;

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                clip(color.a - _Cutoff);
                return color;
            }
            ENDHLSL
        }
    }
}