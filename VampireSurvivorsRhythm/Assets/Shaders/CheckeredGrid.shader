Shader "Custom/CheckeredGrid_URP"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Color 2", Color) = (0, 0, 0, 1)
        _Rows ("Rows", Float) = 8
        _Columns ("Columns", Float) = 8
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color1;
                float4 _Color2;
                float _Rows;
                float _Columns;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                // Calculate which cell we're in
                float2 gridUV = IN.uv * float2(_Columns, _Rows);
                int2 gridCell = int2(floor(gridUV));
                
                // Checkered pattern: alternate colors based on sum of grid indices
                int checker = (gridCell.x + gridCell.y) % 2;
                
                // Return alternating colors
                return lerp(_Color1, _Color2, checker);
            }
            ENDHLSL
        }
    }
}