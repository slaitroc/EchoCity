void ReconstructWorldPosition_float(float4 screenPos, float depth, out float3 worldPos)
{
    float2 ndc = screenPos.xy * 2.0 - 1.0;

    float invDepth = rcp(max(depth, 1e-6));
    float rawDepth = (invDepth - _ZBufferParams.y) / _ZBufferParams.x;

    float clipZ = rawDepth * 2.0 - 1.0;
    float4 clipPos = float4(ndc, clipZ, 1.0);

    float4 world = mul(UNITY_MATRIX_I_VP, clipPos);
    worldPos = world.xyz / world.w;
}
