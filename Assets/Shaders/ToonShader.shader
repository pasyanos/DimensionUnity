Shader "Custom/URPToonShader"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _ShadeLevels ("Shade Levels", Range(1,5)) = 3
        _MinBrightness ("Minimum Brightness", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _ShadeLevels;
                float _MinBrightness;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv;
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);

                // Light and shadow info
                float3 viewDirWS = GetWorldSpaceViewDir(IN.positionWS);
                InputData lightingInput = (InputData)0;
                lightingInput.positionWS = IN.positionWS;
                lightingInput.normalWS = normalWS;
                lightingInput.viewDirectionWS = normalize(viewDirWS);
                lightingInput.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);

                Light mainLight = GetMainLight(lightingInput.shadowCoord);

                // Basic toon shading
                float NdotL = max(0.0, dot(normalWS, -mainLight.direction)) * mainLight.shadowAttenuation;

                float toonStep = floor(NdotL * _ShadeLevels) / _ShadeLevels;

                // Apply minimum brightness floor
                toonStep = max(toonStep, _MinBrightness);

                float4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                float3 litColor = texColor.rgb * toonStep * mainLight.color;

                return float4(litColor, texColor.a);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
