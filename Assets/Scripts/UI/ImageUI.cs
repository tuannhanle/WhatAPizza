using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageUI : MonoBehaviour
    {
        [SerializeField] private Image _image;
        
        public void ChangeImg(Sprite sprite)
        {
            _image.sprite = sprite;
        }
    }
}