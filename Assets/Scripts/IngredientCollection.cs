using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Ingredient Collection", menuName = "Collection", order = 0)]
    public class IngredientCollection : ScriptableObject
    {
        [SerializeField] private GenericDictionary<string, IngredientParams> _ingredientMap;
        // public GenericDictionary<string, IngredientParams> IngredientMap => _ingredientMap;

        public IngredientParams GetIngredientParams(string ingredientName)
        {
            if (_ingredientMap.TryGetValue(ingredientName, out var value))
                return value;
            Debug.LogError("Couldn't find " + ingredientName);
            return null;
        }
    }

    [Serializable]
    public class IngredientParams
    {
        public Ingredient ingredient;
        public Sprite sprite;
    }
    
}