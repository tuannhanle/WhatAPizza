using System;
using System.Collections.Generic;
using DefaultNamespace.UI;
using Unity.BossRoom.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace UnityEngine.Scripting
{
    public class ExtensionOfNativeClassAttribute : Attribute
    {
    }
}

[UnityEngine.Scripting.ExtensionOfNativeClass]
public class GameManager : IStartable
{
    [Inject] IPublisher<ShareData.GameState> _gameStatePub;
    [Inject] IPublisher<ShareData.CurrentIngredient> _currentIngredientPub;
    [Inject] IPublisher<ShareData.PendingIngredient> _pendingIngredientPub;
    [Inject] IPublisher<ShareData.CastBullet> _castBulletPub;
    [Inject] IPublisher<ShareData.UpdateIngredientPoint> _updateIngredientPointPub;

    [Inject] private PanelResourcesUI _panelResourcesUI;
    [Inject] private ResultPanelUI _resultPanelUI;
    
    private Queue<string> _fakeQueueData = new();
    private Queue<string> _pendingIngredientQueue = new();

    readonly private Dictionary<string, int> _gameQuests = new() {
        {"tomato",100},{"cheese",100},{"pineapple",100},{"mushroom",100} };
    readonly string[] _randomIngredients = new[] 
        { "cheese", "tomato", "mushroom", "pineapple" };
    
    private Dictionary<string, int> _resolvingQuest = new() {
        { "tomato", 0 }, { "cheese", 0 }, { "pineapple", 0 }, { "mushroom", 0 } };
    
    private int _ingredientPendingAmount;
    private string _currentIngredient = null;
    private int _bulletsLeft = 0;
    private const int INGREDIENT_PENDING_AMOUNT = 20;
    private const int MAGAZINE_CAPACITY = 20;
    private List<int> _lastLuckyNumbers = new();
    private ShareData.GameState _gameRestart= new (){ IsGameRestart = true };

    public bool IsGameDone = false;
    

    void IStartable.Start()
    {
        _resultPanelUI.OnReplay -= ReplayHandle;
        _resultPanelUI.OnReplay += ReplayHandle;
        ReplayHandle();
        
    }

    public void Dispose()
    {
        _fakeQueueData.Clear();
        _pendingIngredientQueue.Clear();

    }

    public bool Next()
    {
        if (_ingredientPendingAmount > 0)
        {
            AddFakeIngredient();
            TransferIngredientToPending();
        }

        if (_pendingIngredientQueue.Count == 0)
            return false;
        _currentIngredient = _pendingIngredientQueue.Dequeue();
        _currentIngredientPub.Publish(new ShareData.CurrentIngredient(){IngredientName = _currentIngredient});
        _castBulletPub.Publish(new ShareData.CastBullet(){amount = _bulletsLeft, capacity = MAGAZINE_CAPACITY});
        return true;
    }

    public string GetCurrentIngredient()
    {
        return _currentIngredient;
    }

    private void AddFakeIngredient()
    {

        var luckyNumber = Random.Range(0, _randomIngredients.Length);
        var existLuckyNumberInList = false;
        if (_lastLuckyNumbers.Count == 4)
        {
            _lastLuckyNumbers = new();
        }
        foreach (var lastLuckyNumber in _lastLuckyNumbers)
        {
            if (lastLuckyNumber == luckyNumber)
            {
                existLuckyNumberInList = true;
            }
        }

        if (existLuckyNumberInList == false)
        {
            _lastLuckyNumbers.Add(luckyNumber);
        }
        else
        {
            AddFakeIngredient();
            return;
        }
        var ingredient = _randomIngredients[luckyNumber];
        _fakeQueueData.Enqueue(ingredient);
        
    }

    private void TransferIngredientToPending()
    {
        if (_fakeQueueData.Count == 0)
            return;
        _ingredientPendingAmount--;
        var ingredient = _fakeQueueData.Dequeue();
        _pendingIngredientQueue.Enqueue(ingredient);
        _pendingIngredientPub.Publish(new ShareData.PendingIngredient()
        {
            Amount = _ingredientPendingAmount,
            Name = ingredient
        });
    }

    public void CastBullet()
    {
        _bulletsLeft--;
        if (_bulletsLeft != 0)
        {
            _castBulletPub.Publish(new ShareData.CastBullet(){amount = _bulletsLeft, capacity = MAGAZINE_CAPACITY});
            return;
        }
        var hasNext = Next();
        if (hasNext)
        {
            _bulletsLeft = MAGAZINE_CAPACITY;
            _castBulletPub.Publish(new ShareData.CastBullet(){amount = _bulletsLeft, capacity = MAGAZINE_CAPACITY});
            return;

        }
        _castBulletPub.Publish(new ShareData.CastBullet(){amount = _bulletsLeft, capacity = MAGAZINE_CAPACITY});
        _currentIngredientPub.Publish(new ShareData.CurrentIngredient(){IngredientName = null});
        if (_ingredientPendingAmount != 0)
            return;
        IsGameDone = true;
        if (_resolvingQuest.Count > 0)
        {
            // you lose
            _resultPanelUI.Show(false);

        }
        
    }

    public void UpdatePoint(string ingredientName)
    {
        _updateIngredientPointPub.Publish(new ShareData.UpdateIngredientPoint(){ingredientName = ingredientName});

        if (!_resolvingQuest.TryGetValue(ingredientName, out var resolveAmount))
            return;
        if (!_gameQuests.TryGetValue(ingredientName, out var questAmount))
            return;
        if (resolveAmount >= questAmount)
            return;
        _resolvingQuest[ingredientName]++;
        if (_resolvingQuest[ingredientName] != questAmount)
            return;
        _resolvingQuest.Remove(ingredientName);
        if (_resolvingQuest.Count == 0)
        {
            IsGameDone = true;
            // you win
            _resultPanelUI.Show(true);

        }
        
    }

    public void ReplayHandle()
    {
        Dispose();
        _gameStatePub.Publish(_gameRestart);
        _resolvingQuest = new()
        {
            { "tomato", 0 }, { "cheese", 0 }, { "pineapple", 0 }, { "mushroom", 0 }
        };
        
         _ingredientPendingAmount = INGREDIENT_PENDING_AMOUNT;
         _bulletsLeft = MAGAZINE_CAPACITY;

         _currentIngredient = null;
        _lastLuckyNumbers = new();
        IsGameDone = false;
        

        // init 4 pending ingredient
        for (int i = 0; i < 4; i++)
        {
            AddFakeIngredient();
            TransferIngredientToPending();
        }
        Next();
        _panelResourcesUI.Init(_gameQuests);
    }
}


