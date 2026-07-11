Shader "Custom/Outline"
{
    Properties
    {
        _OutlineColor ("Color", Color) = (1, 1, 1, 1)
        _OutlineWidth ("Width", Range(0, 0.1)) = 0.025
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 pos = input.positionOS.xyz + normalize(input.normalOS) * _OutlineWidth;
                output.positionCS = TransformObjectToHClip(pos);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}