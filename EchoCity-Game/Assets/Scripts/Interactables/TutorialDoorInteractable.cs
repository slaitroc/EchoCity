using UnityEngine;

namespace EchoCity
{
    public class TutorialDoorInteractable : LinkableInteractable
    {

        [SerializeField] private SOShowUIEvent showUIEvent;
        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                //TODO trigger next level logic
            }
            else
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams("Finish the tutorial before proceeding.", Color.blue));
            }
        }
    }
}
