using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class Description : MonoBehaviour, IHasDescription
    {
        [TextArea]
        [SerializeField] private string descriptionText;
        private bool _interactable = false;

        string IHasDescription.Description => descriptionText;
        public bool isInteractable => _interactable;

    }
}