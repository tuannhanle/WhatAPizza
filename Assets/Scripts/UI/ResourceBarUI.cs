using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
    public class ResourceBarUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private Text _text;

        private int capacity;
        private int amount;
        public void Init(int capacity, int amount)
        {
            this.capacity = capacity;
            this.amount = amount;
            SetFillUI();
        }

        public void Increase()
        {
            if (amount >= capacity)
                return;
            amount++;
            SetFillUI();
        }
        
        private void SetFillUI()
        {
            _text.text = $"{amount}/{capacity}";
            _fillImage.fillAmount = (float)amount / (float)capacity;
        }
        
    }
}