Shader "Custom/URP/OutlineHLSLShader"
{
Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth ("Outline Width (pixels)", Float) = 11
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            Name "SpriteOutlineLit"
            Tags { "LightMode"="UniversalForward" }

            ZWrite Off
            ZTest Always
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // 라이팅 계산을 위해 필수 포함
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1; // 빛의 위치 계산을 위해 필요
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineWidth;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // 1. 기본 텍스처 및 아웃라인 계산
                float4 center = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float alpha = center.a;

                float2 offset = _MainTex_TexelSize.xy * _OutlineWidth;
                float a1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(offset.x, 0)).a;
                float a2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(-offset.x, 0)).a;
                float a3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(0, offset.y)).a;
                float a4 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(0, -offset.y)).a;

                float outline = max(max(a1, a2), max(a3, a4));
                outline = saturate(outline - alpha);

                // 2. 라이팅 합산 (핵심 수정 부분)
                half3 finalLight = half3(0, 0, 0);

                // [A] 메인 라이트 (Global Light 역할)
                float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                finalLight += mainLight.color * mainLight.distanceAttenuation;

                // [B] 추가 라이트 (플레이어의 Spot Light 등)
                // 씬에 있는 추가적인 빛들을 루프를 돌며 합산합니다.
                int pixelLightCount = GetAdditionalLightsCount();
                for (int j = 0; j < pixelLightCount; ++j)
                {
                    // 월드 포지션을 기준으로 해당 위치의 빛 강도를 가져옵니다.
                    Light addLight = GetAdditionalLight(j, i.positionWS);
                    finalLight += addLight.color * addLight.distanceAttenuation;
                }

                // 3. 컬러 조합
                float4 col = lerp(center, _OutlineColor, outline);
                
                // 계산된 빛의 총합을 곱해줍니다.
                col.rgb *= finalLight; 
                col.a = max(alpha, outline);

                return col;
            }
            ENDHLSL
        }
    }
}