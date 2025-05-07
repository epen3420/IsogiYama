Shader "Custom/GlitchEffectRandomTimingURP_Irregular"
{
    Properties
    {
        _MainTex           ("Texture",          2D)    = "white" {}
        _BlockSize         ("Block Size",       Range(1,100))  = 20
        _GlitchAmount      ("Glitch Amount",    Range(0,1))    = 0.1
        _GlitchFrequency   ("Glitch Frequency", Range(0.1,10)) = 1
        _GlitchChance      ("Glitch Chance",    Range(0,1))    = 0.02
        _ColorSeparation   ("Color Separation", Range(0,0.1))  = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "GlitchPass"
            ZWrite Off Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _BlockSize;
            float _GlitchAmount;
            float _GlitchFrequency;
            float _GlitchChance;
            float _ColorSeparation;

            // シンプルな 2D ハッシュ関数
            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453);
            }

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv         = IN.uv;
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float time = _Time.y;

                // 不規則グリッチ判定：Frequency でサンプリングした乱数が Chance 以下ならアクティブ
                float sample = rand(float2(time * _GlitchFrequency, time * 0.37));
                float isActive = step(sample, _GlitchChance);

                // ブロック単位 UV
                float2 block = floor(uv * _BlockSize) / _BlockSize;

                // 基本オフセット
                float baseRand = rand(float2(block.y, time * _GlitchFrequency + 1.23));
                float offset = (baseRand * 2.0 - 1.0) * _GlitchAmount * isActive;

                // 各チャンネル用オフセット（色収差を大きめに）
                float sep = _ColorSeparation * isActive * 2.0; // 倍率 2.0 で強調

                float3 col;
                col.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(offset + sep, offset)).r;
                col.g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(offset,        offset)).g;
                col.b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(offset - sep, offset)).b;

                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
