void ReconstructWorldPosition_float(float4 screenPos, float rawDepth, out float3 worldPos)
{
    float2 ndc = (screenPos.xy / screenPos.w) * 2.0 - 1.0;
    float clipZ = rawDepth * 2.0 - 1.0;       // convert 0..1 to clip space
    float4 clip = float4(ndc, clipZ, 1.0);

    float4 world = mul(UNITY_MATRIX_I_VP, clip);
    worldPos = world.xyz / world.w;
}