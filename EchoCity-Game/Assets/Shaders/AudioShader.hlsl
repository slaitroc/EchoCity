float4 _AudioSpherePositions[16];
float _AudioSphereRadii[16];
float _AudioSphereFrequencies[16];
float _AudioSphereIntensities[16];
int _AudioSphereCount;

static const float3 LOW_FREQ_COLOR = float3(0.2114455, 0.7478442, 0.9320595);
static const float3 MID_FREQ_COLOR = float3(0.9660662, 0.5664417, 0.1507453);
static const float3 HIGH_FREQ_COLOR = float3(0.8651463, 0.1946775, 0.1946775);

void CalculateAudioVisibility_float(float3 WorldPosition, float _ObjectFrequency, out float Visibility, out float3 OutColor)
{
    int objectFrequency = (int)round(_ObjectFrequency);
    float3 objectColor;
    float3 accumulatedColor = float3(0.0, 0.0, 0.0);
    float totalWeight = 0.0;
    
    if (objectFrequency <= 0)
    {
        objectColor = LOW_FREQ_COLOR;
    }
    else if (objectFrequency == 1)
    {
        objectColor = MID_FREQ_COLOR;
    }
    else
    {
        objectColor = HIGH_FREQ_COLOR;
    }

    for (int i = 0; i < _AudioSphereCount && i < 16; i++)
    {
        if (_AudioSphereRadii[i] > 0.0 && _AudioSphereIntensities[i] > 0.0)
        {
            float3 spherePos = _AudioSpherePositions[i].xyz;
            float sphereRadius = _AudioSphereRadii[i];
            int sphereFrequency = (int)round(_AudioSphereFrequencies[i]);
            float sphereIntensity = _AudioSphereIntensities[i];

            // Ignore spheres that are lower frequency than the object (object requires equal or higher frequency)
            if (sphereFrequency < objectFrequency)
            {
                continue;
            }

            float distance = length(WorldPosition - spherePos);
            float normalizedDistance = distance / sphereRadius;
            float sphereVisibility = saturate(1.0 - normalizedDistance) * sphereIntensity;

            // Determine color and update accumulate color
            if (sphereVisibility > 0.0)
            {

                accumulatedColor += objectColor * sphereVisibility * sphereVisibility;
                totalWeight += sphereVisibility * sphereVisibility;
            }
        }
    }

    OutColor = accumulatedColor / totalWeight;

    // Prevent seeing through low-freq objects without any audio sphere
    if (objectFrequency <= 0 && totalWeight == 0.0)
    {
        Visibility = 1.0;
    }
    else
    {
        Visibility = saturate(totalWeight);
    }
}
