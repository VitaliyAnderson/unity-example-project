Shader "Custom/Shader"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ForwardOnly"

            Tags
            {
                "LightMode" = "UniversalForward"
            }
            
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                uint instanceID : SV_InstanceID;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
            };
            
            StructuredBuffer<float3> _InstancePositions;
            StructuredBuffer<float4> _InstanceColors;
            
            Varyings vert(Attributes input)
            {
                Varyings o;
                o.positionCS = TransformWorldToHClip(TransformObjectToWorld(_InstancePositions[input.instanceID]) + input.positionOS);
                o.color = _InstanceColors[input.instanceID];

                return o;
            }
            
            float4 frag(Varyings i) : SV_Target
            {
		        return i.color;
            }
	    
            ENDHLSL
        }
    }
}
