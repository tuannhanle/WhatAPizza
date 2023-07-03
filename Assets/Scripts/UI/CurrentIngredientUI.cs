using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
    public class CurrentIngredientUI : MonoBehaviour
    {
        [SerializeField] private Image _img;

        public void ChangeImg(Sprite sprite)
        {
            _img.sprite = sprite;
            _img.enabled = true;
            if (sprite == null)
            {
                _img.enabled = false;
                return;
            }
        }
        
    }
}