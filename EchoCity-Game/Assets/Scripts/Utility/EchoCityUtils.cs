using UnityEngine;

namespace EchoCity
{
    public static class EchoCityUtils
    {
        public static int SelectClosestWaypoint(PatrolArea[] patrolAreas, Transform transform, out PatrolArea closestArea)
        {
            closestArea = default;
            float closestDistance = float.MaxValue;
            int closestWaypointIndex = -1;
            for (int i = 0; i < patrolAreas.Length; i++)
            {
                var area = patrolAreas[i];
                if (area.Waypoints == null || area.Waypoints.Length == 0)
                    continue;
                else
                {
                    for (int j = 0; j < area.Waypoints.Length; j++)
                    {
                        if (area.Waypoints[j] == null)
                            continue;

                        float distance = Vector3.Distance(transform.position, area.Waypoints[j].position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestArea = area;
                            closestWaypointIndex = j;
                        }
                    }
                }
            }
            return closestWaypointIndex;
        }
    }
}