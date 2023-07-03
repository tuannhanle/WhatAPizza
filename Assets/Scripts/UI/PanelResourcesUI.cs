using System;
using System.Collections.Generic;
using Unity.BossRoom.Infrastructure;
using UnityEngine;
using VContainer;

namespace DefaultNamespace.UI
{
    public class PanelResourcesUI : MonoBehaviour
    {
        [SerializeField] private ResourceBarUI _resourceBar;
        [SerializeField] private ResourceBarUI _resourceBar2;
        [SerializeField] private ResourceBarUI _resourceBar3;
        [SerializeField] private ResourceBarUI _resourceBar4;

        private Dictionary<string, ResourceBarUI> _resourceBarMap = new();

        protected DisposableGroup _subscriptions;


        
        [Inject]
        void InjectDependencies(
            ISubscriber<ShareData.UpdateIngredientPoint> updateIngredientPointSub
         
        ) 
        {
            _subscriptions = new DisposableGroup();
            _subscriptions.Add(updateIngredientPointSub.Subscribe(OnUpdateIngredientPoint));
        }



        private void Awake()
        {
            _resourceBarMap.Add("tomato", _resourceBar);
            _resourceBarMap.Add("cheese", _resourceBar2);
            _resourceBarMap.Add("pineapple", _resourceBar3);
            _resourceBarMap.Add("mushroom", _resourceBar4);
        }

        public void Init(Dictionary<string, int> gameQuests)
        {
            foreach (var _resourceBar in _resourceBarMap)
            {
                if (gameQuests.TryGetValue(_resourceBar.Key, out var targetAmount))
                {
                    
                    _resourceBar.Value.Init(targetAmount, 0);

                }
            }
        }
        
        private void OnUpdateIngredientPoint(ShareData.UpdateIngredientPoint data)
        {
            var resourceBar = GetResourceBar(data.ingredientName);
            resourceBar.Increase();
        }

        private ResourceBarUI GetResourceBar(string key)
        {
            if (_resourceBarMap.TryGetValue(key, out var bar))
                return bar;
            return null;
        }
        
        
        
    }
}