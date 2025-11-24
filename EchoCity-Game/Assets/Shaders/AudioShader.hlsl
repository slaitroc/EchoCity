float4 _AudioSpherePositions[16];
float _AudioSphereRadii[16];
float _AudioSphereFrequencies[16];
float _AudioSphereIntensities[16];
int _AudioSphereCount;

float _GridSize = 1.0;
float _LineWidth = 0.05;

// Visualization mode properties
int _VisualizationMode = 0; // 0 = SolidColor, 1 = GridLines, 2 = GridPoints
float _PointSize = 0.1;

float CalculateGridPattern(float3 WorldPosition)
{
    float3 gridPos = WorldPosition / _GridSize;
    float3 grid = abs(frac(gridPos - 0.5) - 0.5) / fwidth(gridPos);
    float lineDistance = min(grid.x, min(grid.y, grid.z));

    return 1.0 - min(lineDistance, 1.0);
}

float CalculatePointPattern(float3 WorldPosition)
{
    float3 gridPos = WorldPosition / _GridSize;
    float3 cellPos = frac(gridPos);

    float3 centerOffset = cellPos - 0.5;
    float distanceFromCellCenter = length(centerOffset);

    float pointRadius = _PointSize / _GridSize;
    float pointIntensity = 1.0 - smoothstep(0.0, pointRadius, distanceFromCellCenter);

    return pointIntensity;
}

void CalculateAudioVisibility_float(float3 WorldPosition, float _ObjectFrequency, float4 LowColor, float4 MidColor,
                                    float4 HighColor, out float Visibility, out float3 OutColor)
{
    int objectFrequency = (int)round(_ObjectFrequency);
    float3 accumulatedColor = float3(0.0, 0.0, 0.0);
    float totalWeight = 0.0;

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
                float3 sphereColor;
                if (sphereFrequency <= 0)
                {
                    sphereColor = LowColor.rgb;
                }
                else if (sphereFrequency == 1)
                {
                    sphereColor = MidColor.rgb;
                }
                else
                {
                    sphereColor = HighColor.rgb;
                }

                accumulatedColor += sphereColor * sphereVisibility * sphereVisibility;
                totalWeight += sphereVisibility * sphereVisibility;
            }
        }
    }

    OutColor = accumulatedColor / totalWeight;

    float visualizationPattern = 1.0;

    if (_VisualizationMode == 0) // SolidColor
    {
        visualizationPattern = 1.0;
    }
    else if (_VisualizationMode == 1) // GridLines
    {
        visualizationPattern = CalculateGridPattern(WorldPosition);
    }
    else if (_VisualizationMode == 2) // GridPoints
    {
        visualizationPattern = CalculatePointPattern(WorldPosition);
    }

    // Prevent seeing through low-freq objects without any audio sphere
    if (objectFrequency <= 0 && totalWeight == 0.0)
    {
        Visibility = 1.0;
    }
    else
    {
        Visibility = saturate(totalWeight * visualizationPattern);
    }
}
