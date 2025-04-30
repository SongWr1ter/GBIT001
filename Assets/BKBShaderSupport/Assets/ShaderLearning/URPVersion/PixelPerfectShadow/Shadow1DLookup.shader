Shader "Custom/URP/Shadow1DLookup"
{
    Properties
    {
        _OccludersTexture ("Occluders Texture", 2D) = "black" {}
        _Resolution ("Resolution", Vector) = (256, 256, 0, 0)
        _Threshold ("Alpha Threshold", Range(0, 1)) = 0.75
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "Shadow1DLookup"
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
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_OccludersTexture);
            SAMPLER(sampler_OccludersTexture);
            float2 _Resolution;
            float _Threshold;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float distance = 1.0;
                for (float y = 0.0; y < _Resolution.y; y += 1.0)
                {
                    // Rectangular to polar filter
                    float2 norm = float2(i.uv.x, y / _Resolution.y) * 2.0 - 1.0;
                    float theta = PI * 1.5 + norm.x * PI;
                    float r = (1.0 + norm.y) * 0.5;

                    // Coordinate to sample from occlusion map
                    float2 coord = float2(-r * sin(theta), -r * cos(theta)) / 2.0 + 0.5;

                    // Sample the occlusion map
                    float4 data = SAMPLE_TEXTURE2D(_OccludersTexture, sampler_OccludersTexture, coord);

                    // Current distance from the top
                    float dst = y / _Resolution.y;

                    // Check if we hit an occluder
                    float caster = data.a;
                    if (caster > _Threshold)
                    {
                        distance = min(distance, dst);
                        // Break early if possible
                        break;
                    }
                }

                // Return the distance as RGB value
                return float4(float3(distance,distance,distance), 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}