using UnityEngine;

public class AttractionManager : MonoBehaviour
{
    public static bool calculateAttraction = false;
    public bool calculate = false;

    void Update()
    {
        calculateAttraction = calculate;
    }

}
