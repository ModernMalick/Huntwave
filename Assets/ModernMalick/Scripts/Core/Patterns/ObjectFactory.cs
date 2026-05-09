using UnityEngine;
using UnityEngine.Pool;

namespace ModernMalick.Core.Patterns
{
    public class ObjectFactory : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxSize = 50;

        private IObjectPool<GameObject> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<GameObject>(
                CreateInstance,
                OnGetFromPool,
                OnReturnToPool,
                OnDestroyInstance,
                true,
                defaultCapacity,
                maxSize
            );
        }

        private GameObject CreateInstance()
        {
            var instance = Instantiate(prefab);
            return instance;
        }

        private void OnGetFromPool(GameObject instance)
        {
            instance.SetActive(true);
        }

        private void OnReturnToPool(GameObject instance)
        {
            instance.SetActive(false);
        }

        private void OnDestroyInstance(GameObject instance) => Destroy(instance);

        public GameObject Get() => _pool.Get();
        
        public void Release(GameObject instance) => _pool.Release(instance);
    }
}