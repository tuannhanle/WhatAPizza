using System;
using System.Collections.Generic;
using Unity.BossRoom.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace DefaultNamespace.UI
{
    public class IngredientBarUI : MonoBehaviour
    {
        
        [SerializeField] private CurrentIngredientUI _currentIngredientUI;
        [SerializeField] private IngredientBarQueueUI _ingredientBarQueueUI;
        [SerializeField] private BulletBarUI _bulletBarUI;
        [SerializeField] IngredientCollection _ingredientCollection;
        [SerializeField] private Text _pendingIngredientLeft;
        protected DisposableGroup _subscriptions;


        
        [Inject]
        void InjectDependencies(
            ISubscriber<ShareData.GameState> gameStateSub,
            ISubscriber<ShareData.CurrentIngredient> currentIngredientSub,
            ISubscriber<ShareData.PendingIngredient> pendingIngredientSub,
            ISubscriber<ShareData.CastBullet> castBulletSub
            ) 
        {
            _subscriptions = new DisposableGroup();
            _subscriptions.Add(gameStateSub.Subscribe(OnGameStateUpdated));
            _subscriptions.Add(currentIngredientSub.Subscribe(OnCurrentIngredintUpdated));
            _subscriptions.Add(pendingIngredientSub.Subscribe(OnPendingIngredintUpdated));
            _subscriptions.Add(castBulletSub.Subscribe(OnCastBulletUpdated));
        }

        private void OnCastBulletUpdated(ShareData.CastBullet castBullet)
        {
            _bulletBarUI.SetFillUI(castBullet.capacity, castBullet.amount);
        }

        private void OnPendingIngredintUpdated(ShareData.PendingIngredient pendingIngredient)
        {
            if (pendingIngredient.Name == null)
                return;
            _ingredientBarQueueUI.Add(pendingIngredient.Name);
            _pendingIngredientLeft.text = pendingIngredient.Amount.ToString();

        }

        private void OnCurrentIngredintUpdated(ShareData.CurrentIngredient currentIngredient)
        {
            if (currentIngredient.IngredientName == null)
            {
                _currentIngredientUI.ChangeImg(null);
                return;
            }
            var ingredient = _ingredientCollection.GetIngredientParams(currentIngredient.IngredientName);
            _currentIngredientUI.ChangeImg(ingredient.sprite);
            _ingredientBarQueueUI.Sub();
        }

        private void OnGameStateUpdated(ShareData.GameState gameState)
        {
            if (gameState.IsGameRestart)
            {
                _ingredientBarQueueUI.Clear();
            }
        }

        private void OnDestroy()
        {
            _subscriptions.Dispose();
        }
        
    }
    
}