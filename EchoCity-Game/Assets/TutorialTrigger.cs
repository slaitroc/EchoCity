using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class TutorialTrigger : MonoBehaviour
    {
        [SerializeField] SODialogContainer tutorialDialogContainer;
        [SerializeField] SODialogDataEvent switchToNarrationStateEvent;

        void OnTriggerEnter(Collider other)
        {
            Log.D("Player entered tutorial trigger.", "TUTORIAL TRIGGER", "#00ff00ff");
            if (!other.CompareTag("Player")) return;
            switchToNarrationStateEvent?.RaiseEvent(new DialogData(tutorialDialogContainer));
            gameObject.SetActive(false);
        }
    }
}
