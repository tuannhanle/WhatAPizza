
#nullable enable
using DefaultNamespace;

public class ShareData
{
    public class GameState
    {
        public bool IsGameRestart = false;
        
    }

    public class CurrentIngredient
    {
        public string IngredientName = null;
    }

    public class PendingIngredient
    {
        public int Amount;
        public string Name = null;

    }

    public class CastBullet
    {
        public int capacity;
        public int amount;
    }

    public class MultiplyBullet
    {
        public Bullet bullet;
        public int multipleRatio;
    }

    public class UpdateIngredientPoint
    {
        public string ingredientName;
    }
}