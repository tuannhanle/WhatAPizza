using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _lifeTime;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private IngredientCollection _ingredientCollection;
        
        public Action<Bullet, MultipleObject> OnTriggerMultipleObject { get; set; }
        public Action<string> OnCollidePizza { get; set; }

        public HashSet<int> CacheIds = new HashSet<int>();
        private GameObject _cacheIngredientRenderer;
        private string _cacheIngredientName;

        void OnEnable()
        {
            Return().Forget();
        }

        private void Start()
        {
            OnEnable();
            if (_rb.IsSleeping())
            {
                _rb.WakeUp();
            }
        }

        public void Recovery(string ingredientName)
        {
            _cacheIngredientName = ingredientName;
            CacheIds = new();
            for (int i = 0; i < this.transform.childCount; i++)
            {
                var child = this.transform.GetChild(i);
                child.gameObject.ReturnToPool();
                _cacheIngredientRenderer.transform.SetParent(ObjectPoolManager.GetPoolRoot().transform);

            }
            var ingredientParams = _ingredientCollection.GetIngredientParams(ingredientName);
            var ingredient = ingredientParams.ingredient.gameObject.GetFromPool();
            ingredient.transform.SetParent(this.transform, false);
            ingredient.transform.localPosition = Vector3.zero;
            _cacheIngredientRenderer = ingredient;
        }

        public async UniTaskVoid Return()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_lifeTime));
            _rb.Sleep();
            if (_cacheIngredientRenderer.activeInHierarchy)
            {
                _cacheIngredientRenderer.ReturnToPool();
                _cacheIngredientRenderer.transform.SetParent(ObjectPoolManager.GetPoolRoot().transform);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));

            if (gameObject.activeInHierarchy)
            {
                this.gameObject.ReturnToPool();
            }
        }
        public void Shoot(Vector3 direction)
        {
            _rb.Sleep();
            _rb.WakeUp();
            _rb.AddForce(direction * _speed, ForceMode.Impulse);
            Return().Forget();
        }

        private void OnTriggerEnter(Collider other)
        {
            var multipleObject = other.gameObject.GetComponent<MultipleObject>();
            if (multipleObject == null)
                return;
            var id = multipleObject.GetInstanceID();
            if (CacheIds.TryGetValue(id, out var actualValue))
            {
                return;
            }
            CacheIds.Add(id);
            Multiply(multipleObject);
            Shoot(Vector3.down);            
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Pizza"))
            {
                OnCollidePizza?.Invoke(_cacheIngredientName);
            }
        }


        private void Multiply(MultipleObject multipleObject)
        {
            OnTriggerMultipleObject?.Invoke(this, multipleObject);
        }


    }
}