using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
    public class Aim : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private RectTransform _rectTransform;

        public Ray ray;
        
        async UniTaskVoid Start()
        {
            while (true)
            {
                await UniTask.WaitForFixedUpdate();
                ray = Camera.main.ScreenPointToRay( _rectTransform.anchoredPosition);
                Debug.DrawRay(ray.origin ,ray.direction * 1000, Color.green);
            }
        }
        
    }
}