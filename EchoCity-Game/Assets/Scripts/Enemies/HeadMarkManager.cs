using UnityEngine;

namespace EchoCity
{
    public class HeadMarkManager : MonoBehaviour
    {
        [SerializeField] HeadMark chaseMark;
        [SerializeField] HeadMark checkMark;
        private Transform _enemyTransform;

        public void Initialize(Transform enemyTransform)
        {
            _enemyTransform = enemyTransform;
            ClearMarks();
        }

        public void ShowChaseMark()
        {
            ClearMarks();
            chaseMark.Activate();
        }

        public void ShowCheckMark()
        {
            ClearMarks();
            checkMark.Activate();
        }

        public void ClearMarks()
        {
            chaseMark.Deactivate();
            checkMark.Deactivate();
        }
    }
}