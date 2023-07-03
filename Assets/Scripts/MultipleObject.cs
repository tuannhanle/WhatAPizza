using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class MultipleObject : MonoBehaviour
    {
        [SerializeField] private int _multipleRatio ;
        [SerializeField] private TextMeshPro _text ;

        public int GetMultipleRatio => _multipleRatio;

        private void Awake()
        {
            _text = GetComponentInChildren<TextMeshPro>();
        }

        async UniTaskVoid Start()
        {
            _text.text = $"x{_multipleRatio}";

            while (true)
            {
                await UniTask.Yield();
                if (this == null)
                    return;
                _text.transform.LookAt(Camera.main.transform);
                transform.Rotate(Vector3.up - new Vector3(0,180,0));

            }
        }
    }
}