﻿Shader "LimitlessGlitch/Glitch_3" 
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_Mask);
        SAMPLER(sampler_Mask);
        float _FadeMultiplier;
        float speed;
        float blockSize; 
        float maxOffsetX; 
        float maxOffsetY;

        inline float rand(float2 seed)
        {
            return frac(sin(dot(seed * floor(_Time.y * speed), float2(127.1, 311.7))) * 43758.5453123);
        }

         inline float rand(float seed)
         {
            return rand(float2(seed, 1.0));
         }

        float4 Frag(Varyings i) : SV_Target
        {
            if (_FadeMultiplier > 0)
            {
                float alpha_Mask = step(0.0001, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.uv).r);
                maxOffsetX *= alpha_Mask;
                maxOffsetY *= alpha_Mask;
                blockSize *= alpha_Mask;
            }
            float2 block = rand(floor(i.uv * blockSize));
            float OffsetX = pow(block.x, 8.0) * pow(block.x, 3.0) - pow(rand(7.2341), 17.0) * maxOffsetX;
            float OffsetY = pow(block.x, 8.0) * pow(block.x, 3.0) - pow(rand(7.2341), 17.0) * maxOffsetY;
            float4 r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
            float4 g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(OffsetX * 0.05 * rand(7.0), OffsetY*0.05*rand(12.0)));
            float4 b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - half2(OffsetX * 0.05 * rand(13.0), OffsetY*0.05*rand(12.0)));
            return half4(r.x, g.g, b.z, (r.a+g.a+b.a));
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex Vert
                #pragma fragment Frag

            ENDHLSL
        }
    }
}