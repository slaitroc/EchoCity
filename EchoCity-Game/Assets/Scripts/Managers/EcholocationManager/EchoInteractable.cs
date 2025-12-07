using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(AudioEmitter))]
    public class EchoTrigger : MonoBehaviour
    {
        [SerializeField] private TriggerType triggerType = TriggerType.OnClick;

        private AudioEmitter _audioEmitter;
        // Delay, not trigger collision at the beginning
        private bool _isGameStarted = false;

        public enum TriggerType
        {
            OnCollision,  // Triggered by collision
            OnClick,      // Triggered by mouse click
        }

        void Awake() => TryGetComponent(out _audioEmitter);

        void Start()
        {
            if (triggerType == TriggerType.OnClick || triggerType == TriggerType.OnCollision)
            {
                Collider col = GetComponent<Collider>();
                if (col == null) Log.W($"EchoInteractable '{gameObject.name}' requires a Collider!", "purple", "ECHOLOCATION");
                if (triggerType == TriggerType.OnCollision) col.isTrigger = true;
            }
            Invoke(nameof(SetGameStarted), 0.1f);
        }
        void Update()
        {
            if (triggerType == TriggerType.OnClick)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
                    {
                        if (_audioEmitter != null) _audioEmitter.EmitSound();
                    }
                }
            }
        }

        private void SetGameStarted()
        {
            _isGameStarted = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!_isGameStarted) return;
            if (triggerType == TriggerType.OnCollision)
            {
                if (_audioEmitter != null) _audioEmitter.EmitSound();
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!_isGameStarted) return;
            if (triggerType == TriggerType.OnCollision)
            {
                if (_audioEmitter != null) _audioEmitter.EmitSound();
            }
        }
    }
}
