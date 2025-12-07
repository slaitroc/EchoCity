using UnityEngine;

namespace EchoCity
{
    public abstract class LinkableInteractable : Interactable
    {
        protected override string _TYPE_LOG_TAG => "LINKABLE";
        [SerializeField] protected bool hasFixDetector = false;
        [SerializeField] protected InteractableFixDetector detector;
        protected override void Awake()
        {
            base.Awake();
            if (hasFixDetector)
            {
                gameObject.layer = 1; // Set to Default layer
                if (detector == null)
                {
                    Log.E($"LinkableInteractable on {gameObject.name} is set to have a Fix Detector but none is assigned!", _LOG_COLOR, _LOG_TAG);
                }
            }
            else
            {
                gameObject.layer = 6; // Set to Interactable layer
            }
        }
    }


}