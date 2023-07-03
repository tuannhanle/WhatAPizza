using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
    public class BulletBarUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImg;
        

        public void SetFillUI(int capacity, int amount)
        {
            _fillImg.fillAmount = (float)amount / (float)capacity;
        }
    }
}