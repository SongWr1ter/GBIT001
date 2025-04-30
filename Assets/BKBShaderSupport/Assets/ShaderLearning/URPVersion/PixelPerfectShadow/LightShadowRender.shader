Shader "Custom/URP/LightShadowRender"
{
    Properties
    {
        _ShadowLookupTexture ("Shadow Lookup Texture", 2D) = "white" {}
        _Resolution ("Resolution", Vector) = (256, 256, 0, 0)
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "LightShadowRender"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_ShadowLookupTexture);
            SAMPLER(sampler_ShadowLookupTexture);
            float2 _Resolution;
            float4 _LightColor;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float sample(float2 coord, float r)
            {
                return step(r, SAMPLE_TEXTURE2D(_ShadowLookupTexture, sampler_ShadowLookupTexture, coord).r);
            }

            float4 frag (Varyings i) : SV_Target
            {

                // Rectangular to polar coordinates
                float2 norm = i.uv * 2.0 - 1.0;
                float theta = atan2(norm.y, norm.x);
                float r = length(norm);
                float coord = (theta + PI) / (2.0 * PI);

                // Texture coordinate for 1D lookup
                float2 tc = float2(coord, 0.0);

                // Center sample for hard shadows
                float center = sample(tc, r);

                // Blur amount based on distance from center
                float blur = (1.0 / _Resolution.x) * smoothstep(0.0, 1.0, r);

                // Gaussian blur
                float sum = 0.0;
                sum += sample(float2(tc.x - 4.0 * blur, tc.y), r) * 0.05;
                sum += sample(float2(tc.x - 3.0 * blur, tc.y), r) * 0.09;
                sum += sample(float2(tc.x - 2.0 * blur, tc.y), r) * 0.12;
                sum += sample(float2(tc.x - 1.0 * blur, tc.y), r) * 0.15;

                sum += center * 0.16;

                sum += sample(float2(tc.x + 1.0 * blur, tc.y), r) * 0.15;
                sum += sample(float2(tc.x + 2.0 * blur, tc.y), r) * 0.12;
                sum += sample(float2(tc.x + 3.0 * blur, tc.y), r) * 0.09;
                sum += sample(float2(tc.x + 4.0 * blur, tc.y), r) * 0.05;

                // Multiply by distance for radial falloff
                float radialFalloff = smoothstep(1.0, 0.0, r);
                float finalIntensity = sum * radialFalloff;

                // Return the final color
                return _LightColor * float4(float3(1.0,1.0,1.0), finalIntensity);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}