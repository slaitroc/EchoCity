using UnityEngine;

namespace EchoCity
{
    public class DebugTimeScale : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 3f)] private float timeScale = 1f;

        private void Update()
        {
            Time.timeScale = timeScale;
        }
    }
}