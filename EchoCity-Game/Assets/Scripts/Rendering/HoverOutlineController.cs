using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Camera))]
    public class HoverOutlineController : MonoBehaviour
    {
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private LayerMask outlineLayers = (1 << 6) | (1 << 9);

        private Camera _camera;
        private Interactable _current;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnDisable() => InteractableOutlineRenderer.ClearHovered();

        private void Update()
        {
            if (_camera == null) return;

            var ray = new Ray(_camera.transform.position, _camera.transform.forward);
            Interactable interactable = null;
            if (Physics.Raycast(ray, out var hit, maxDistance, outlineLayers, QueryTriggerInteraction.Collide))
                interactable = hit.collider?.GetComponentInParent<Interactable>();

            if (interactable != _current)
            {
                _current = interactable;
                if (_current != null)
                    InteractableOutlineRenderer.SetHovered(_current.GetComponentsInChildren<Renderer>(false));
                else
                    InteractableOutlineRenderer.ClearHovered();
            }
        }
    }
}
