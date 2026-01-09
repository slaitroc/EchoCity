using UnityEngine;

namespace EchoCity
{
    public abstract class LinkableInteractable : PlainInteractable
    {
        [SerializeField] protected bool hasFixDetector = false;
        [SerializeField] protected InteractableFixDetector detector;
        protected override void Awake()
        {
            base.Awake();
            if (hasFixDetector)
            {
                gameObject.layer = 1; // Set to Default layer
                Debug.Assert(detector != null, $"LinkableInteractable: hasFixDetector is true but no detector assigned.");
            }
            else
                gameObject.layer = 6; // Set to Interactable layer

        }
    }


}