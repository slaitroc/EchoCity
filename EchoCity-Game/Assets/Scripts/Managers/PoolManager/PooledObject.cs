using UnityEngine;
using UnityEngine.Pool;

namespace EchoCity
{
    public class PooledObject : MonoBehaviour
    {
        private IObjectPool<GameObject> _myPool;

        public void SetPool(IObjectPool<GameObject> pool) => _myPool = pool;

        public void ReturnToPool(float delay)
        {
            CancelInvoke(nameof(ReturnToPool));
            Invoke(nameof(ReturnToPool), delay);
        }
        public void ReturnToPool()
        {
            if (_myPool != null)
                _myPool.Release(this.gameObject);
            else
                Destroy(this.gameObject);
        }

        void OnDisable()
        {
            CancelInvoke();
        }

    }
}