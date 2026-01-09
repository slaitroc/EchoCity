using UnityEngine;

namespace EchoCity
{
    public class TutorialDoorInteractable : LinkableInteractable
    {
        [SerializeField] private SOSwitchLevelEvent switchLevelEvent;
        [SerializeField] private SceneEnum targetScene;
        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
                switchLevelEvent?.RaiseEvent(this, targetScene);
        }
    }
}
