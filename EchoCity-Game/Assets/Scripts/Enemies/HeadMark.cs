using UnityEngine;

namespace EchoCity
{
    public class HeadMark : MonoBehaviour
    {
        private Animator _animator;
        private MeshRenderer _meshRenderer;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }


        public void Activate()
        {
            _meshRenderer.enabled = true;
            _animator.enabled = true;
            _animator.speed = 1;

            _animator.Rebind();
            _animator.Play("Animate", 0, 0f);
        }

        public void Deactivate()
        {
            _animator.speed = 0;
            _animator.enabled = false;
            _meshRenderer.enabled = false;
        }
    }
}