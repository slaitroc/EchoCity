using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class Description : MonoBehaviour, IHasDescription
    {
        [TextArea]
        [SerializeField] private string descriptionText;

        string IHasDescription.Description => descriptionText;
        bool IHasDescription.HasRaycastDescription => true;
        bool IHasDescription.IsInteractable => false;

    }
}