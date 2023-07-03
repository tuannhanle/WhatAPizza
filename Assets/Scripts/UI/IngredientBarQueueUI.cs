using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.UI
{
    public class IngredientBarQueueUI : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        [SerializeField] private RectTransform _refRect;
        [SerializeField] private ImageUI _imageUIPrefab;
        

        private Queue<KeyValuePair<string, ImageUI>> _queue = new();

        [SerializeField] IngredientCollection _ingredientCollection;

        private int _counting = 0;

        private void OnDestroy()
        {
            _queue.Clear();
        }

        private void Awake()
        {
            _imageUIPrefab.gameObject.AddToPool(4);

        }
        

        public void Add(string ingredientName)
        {
     
            var result  = _ingredientCollection.GetIngredientParams(ingredientName);
            if (result == null)
                return;
            var imageUI = CreateUI(result);
            VisualizeUI(imageUI.gameObject);;
            var pair = new KeyValuePair<string, ImageUI>(ingredientName, imageUI);
            _queue.Enqueue(pair);
        }

        public void Sub()
        {
            if (_queue.Count == 0)
                return;
            var current = _queue.Dequeue();
            current.Value.gameObject.ReturnToPool();
        }

        public void Clear()
        {
            while (_queue.Count > 0)
            {
                var current = _queue.Dequeue();
                current.Value.gameObject.ReturnToPool();
            }
            
        }
        

        private ImageUI CreateUI(IngredientParams ingredientParams)
        {
            
            var img = _imageUIPrefab.gameObject.GetFromPool();
            img.name = $"{img.name} [{_counting}]";
            var imageUI = img.GetComponent<ImageUI>();
            imageUI.ChangeImg(ingredientParams.sprite);
            _counting++;
            return imageUI;
        }

        private void VisualizeUI(GameObject gameObject)
        {
            gameObject.transform.SetParent(_root);
            gameObject.transform.SetSiblingIndex(gameObject.transform.childCount-1);
            var rect = gameObject.GetComponent<RectTransform>();
            rect.localScale =  _refRect.localScale;
            rect.rotation = _refRect.rotation;
            // rect.anchoredPosition = _refRect.anchoredPosition;
            rect.anchoredPosition3D = _refRect.anchoredPosition3D;

        }


    }
}